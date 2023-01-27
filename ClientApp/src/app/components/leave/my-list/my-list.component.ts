import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { GlobalService } from 'src/app/services/global/global.service';
import { LeaveService } from 'src/app/services/leave/leave.service';
import { MessageService } from 'src/app/services/message/message.service';

import { Leave } from 'src/app/shared/types/leave';
import { Common } from 'src/app/shared/common';

@Component({
  selector: 'app-my-list',
  templateUrl: './my-list.component.html',
  styleUrls: ['./my-list.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MyListComponent implements OnInit {

  leaveList: Leave[] = [];

  // pagination, NOTE: pagination is by 10 in server side and can't be set from client
  listCount: number = 0;
  currentPage: number = 1;
  totalPage: number = 1;

  constructor(
    private leaveService: LeaveService,
    private messageService: MessageService,
    private globalService: GlobalService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.getMyLeaveList();
  }

  getMyLeaveList(): void {
    this.leaveService.getMyLeaveList(this.currentPage).subscribe({
      next: (v) => {
        // console.log('MyListComponent: ' + JSON.stringify(v));
        let leaveList: Leave[] = JSON.parse(JSON.stringify(v)).leave_list;
        this.listCount = JSON.parse(JSON.stringify(v)).count;
        this.totalPage = Math.ceil(this.listCount / Common.PAGE_SIZE);

        leaveList.forEach(element => {
          if (element) {
            this.leaveList.push(element);
            // console.log('MyListComponent: id ' + element.id + ':' + element.name);
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

  onClick(item: Leave): void {
    this.leaveService.setCurrentLeave(item);
    this.globalService.navigate('leave/detail');
  }

  onFirstClick(): void {
    this.currentPage = 1;
    this.getMyLeaveList();
  }

  onLastClick(): void {
    this.currentPage = this.totalPage;
    this.getMyLeaveList();
  }

  onPreviousClick(): void {
    --this.currentPage;
    this.getMyLeaveList();
  }

  onNextClick(): void {
    ++this.currentPage;
    this.getMyLeaveList();
  }

  hasNextPage(): boolean {
    return !(this.currentPage >= this.totalPage);
  }

  hasPreviousPage(): boolean {
    return (this.currentPage * Common.PAGE_SIZE > Common.PAGE_SIZE);
  }

}
