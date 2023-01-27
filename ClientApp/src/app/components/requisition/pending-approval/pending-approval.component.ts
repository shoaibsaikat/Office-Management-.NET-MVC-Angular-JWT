import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { GlobalService } from 'src/app/services/global/global.service';
import { RequisitionService } from 'src/app/services/requisition/requisition.service';
import { MessageService } from 'src/app/services/message/message.service';

import { Requisition } from 'src/app/shared/types/requisition';
import { User } from 'src/app/shared/types/user';
import { Common } from 'src/app/shared/common';

@Component({
  selector: 'app-pending-approval',
  templateUrl: './pending-approval.component.html',
  styleUrls: ['./pending-approval.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PendingApprovalComponent implements OnInit {

  requisitionList: Requisition[] = [];
  distributorList: User[] = [];
  requisitionFormList: FormGroup[] = [];

  // pagination, NOTE: pagination is by 10 in server side and can't be set from client
  listCount: number = 0;
  currentPage: number = 1;
  totalPage: number = 1;

  constructor(
    private requisitionService: RequisitionService,
    private messageService: MessageService,
    private globalService: GlobalService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.updateApprovalList();
  }

  updateApprovalList(): void {
    this.requisitionService.getApprovalList(this.currentPage).subscribe({
      next: (v) => {
        // console.log('ListComponent: ' + JSON.stringify(v));
        let requisitionList: Requisition[] = JSON.parse(JSON.stringify(v)).requisition_list;
        let distributorList: User[] = JSON.parse(JSON.stringify(v)).distributor_list;
        this.listCount = JSON.parse(JSON.stringify(v)).count;
        this.totalPage = Math.ceil(this.listCount / Common.PAGE_SIZE);

        requisitionList.forEach(element => {
          if (element) {
            this.requisitionList.push(element);
          }
        });

        distributorList.forEach(element => {
          if (element) {
            this.distributorList.push(element);
          }
        });

        // generate form groups
        this.requisitionList.forEach(element => {
          this.requisitionFormList.push(new FormGroup({
            distributor: new FormControl('', [Validators.required, ]),
          }));
        });
      },
      error: (e) => {
        // console.error(e);
        this.globalService.handleUnauthorizedAccess(e);
      },
      complete: () => {
        this.changeDetectorRef.markForCheck();
      }
    });
  }

  getPageSize(): number {
    return Common.PAGE_SIZE;
  }

  onDetailClick(item: Requisition): void {
    this.requisitionService.setCurrentRequisition(item);
    this.requisitionService.setDistributorList(this.distributorList);
    this.globalService.navigate('requisition/detail/' + Common.DETAIL_APPROVAL);
  }

  onApproveClick(index: number): void {
    this.requisitionService.approve(this.requisitionList[index].id, this.requisitionFormList[index].get('distributor')?.value).subscribe(data => {
      this.requisitionList.splice(index, 1);
      this.requisitionFormList.splice(index, 1);
      this.changeDetectorRef.markForCheck();
    });
  }

  onDenyClick(index: number): void {
    this.requisitionService.deny(this.requisitionList[index].id).subscribe(data => {
      this.requisitionList.splice(index, 1);
      this.requisitionFormList.splice(index, 1);
      this.changeDetectorRef.markForCheck();
    });
  }

  isDistributorSelected(index: number): boolean {
    return true ? this.requisitionFormList[index].get('distributor')?.value : false;
  }

  onFirstClick(): void {
    this.currentPage = 1;
    this.updateApprovalList();
  }

  onLastClick(): void {
    this.currentPage = this.totalPage;
    this.updateApprovalList();
  }

  onPreviousClick(): void {
    --this.currentPage;
    this.updateApprovalList();
  }

  onNextClick(): void {
    ++this.currentPage;
    this.updateApprovalList();
  }

  hasNextPage(): boolean {
    return !(this.currentPage >= this.totalPage);
  }

  hasPreviousPage(): boolean {
    return (this.currentPage * Common.PAGE_SIZE > Common.PAGE_SIZE);
  }

}
