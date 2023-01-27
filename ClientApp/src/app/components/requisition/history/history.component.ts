import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { GlobalService } from 'src/app/services/global/global.service';
import { RequisitionService } from 'src/app/services/requisition/requisition.service';
import { MessageService } from 'src/app/services/message/message.service';

import { Requisition } from 'src/app/shared/types/requisition';
import { Common } from 'src/app/shared/common';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HistoryComponent implements OnInit {

  requisitionList: Requisition[] = [];

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
    this.updateHistory();
  }

  updateHistory(): void {
    this.requisitionService.getHistory(this.currentPage).subscribe({
      next: (v) => {
        // console.log('ListComponent: ' + JSON.stringify(v));
        let requisitionList: Requisition[] = JSON.parse(JSON.stringify(v)).requisition_list;
        this.listCount = JSON.parse(JSON.stringify(v)).count;
        this.totalPage = Math.ceil(this.listCount / Common.PAGE_SIZE);

        requisitionList.forEach(element => {
          if (element) {
            this.requisitionList.push(element);
            // console.log('ListComponent: id ' + element.id + ':' + element.name);
          }
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

  onClick(item: Requisition): void {
    this.requisitionService.setCurrentRequisition(item);
    this.globalService.navigate('requisition/detail/' + Common.DETAIL_NORMAL);
  }

  onFirstClick(): void {
    this.currentPage = 1;
    this.updateHistory();
  }

  onLastClick(): void {
    this.currentPage = this.totalPage;
    this.updateHistory();
  }

  onPreviousClick(): void {
    --this.currentPage;
    this.updateHistory();
  }

  onNextClick(): void {
    ++this.currentPage;
    this.updateHistory();
  }

  hasNextPage(): boolean {
    return !(this.currentPage >= this.totalPage);
  }

  hasPreviousPage(): boolean {
    return (this.currentPage * Common.PAGE_SIZE > Common.PAGE_SIZE);
  }

}
