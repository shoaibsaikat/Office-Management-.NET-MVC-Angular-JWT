import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { GlobalService } from 'src/app/services/global/global.service';
import { LeaveService } from 'src/app/services/leave/leave.service';
import { MessageService } from 'src/app/services/message/message.service';

import { Common } from 'src/app/shared/common';

import { LeaveSummary } from 'src/app/shared/types/leave_summary';

@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SummaryComponent implements OnInit {

  leaveList: LeaveSummary[] = [];
  selectedYear: number = new Date().getFullYear();

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
    this.updateHistory();
  }

  onYearSelect(): void {
    this.updateHistory();
  }

  updateHistory(): void {
    this.leaveService.getLeaveSummaryList(this.selectedYear, this.currentPage).subscribe({
      next: (v) => {
        // console.log('SummaryComponent: ' + JSON.stringify(v));
        let leaveList: LeaveSummary[] = JSON.parse(JSON.stringify(v)).leave_list;
        this.listCount = JSON.parse(JSON.stringify(v)).count;
        this.totalPage = Math.ceil(this.listCount / Common.PAGE_SIZE);

        this.leaveList = [];
        leaveList.forEach(element => {
          if (element) {
            this.leaveList.push(element);
            // console.log('SummaryComponent: ' + element.user + ':' + element.first_name);
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
