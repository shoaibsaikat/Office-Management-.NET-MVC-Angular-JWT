import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { AssetService } from 'src/app/services/asset/asset.service';
import { MessageService } from 'src/app/services/message/message.service';
import { GlobalService } from 'src/app/services/global/global.service';

import { Asset } from 'src/app/shared/types/asset';
import { Common } from 'src/app/shared/common';

@Component({
  selector: 'app-pending-list',
  templateUrl: './pending-list.component.html',
  styleUrls: ['./pending-list.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PendingListComponent implements OnInit {

  assetList: Asset[] = [];

  // pagination, NOTE: pagination is by 10 in server side and can't be set from client
  listCount: number = 0;
  currentPage: number = 1;
  totalPage: number = 1;

  constructor(
    private assetService: AssetService,
    private globalService: GlobalService,
    private messageService: MessageService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.updatePendingList();
  }

  updatePendingList(): void {
    this.assetService.getPendingAssetList(this.currentPage).subscribe({
      next: (v) => {
        // console.log('MyListComponent: ' + JSON.stringify(v));
        let assetList: Asset[] = JSON.parse(JSON.stringify(v)).asset_list;
        this.listCount = JSON.parse(JSON.stringify(v)).count;
        this.totalPage = Math.ceil(this.listCount / Common.PAGE_SIZE);

        assetList.forEach(element => {
          if (element) {
            this.assetList.push(element);
            // console.log('MyListComponent: id ' + element.id + ' '  + element.name + ' : ' + element.user);
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

  onApprove(item: number, index: number): void {
    // console.log('PendingListComponent: onApprove() ' + item);
    this.assetService.approvePendingAsset(item).subscribe(data => {
      let msg: string = JSON.parse(JSON.stringify(data));
      this.messageService.add(msg);

      // update local data
      this.assetList.splice(index, 1);

      this.changeDetectorRef.markForCheck();
    });
  }

  onDecline(item: number, index: number): void {
    // console.log('PendingListComponent: onDecline() ' + item);
    this.assetService.declinePendingAsset(item).subscribe(data => {
      let msg: string = JSON.parse(JSON.stringify(data));
      this.messageService.add(msg);

      // update local data
      this.assetList.splice(index, 1);

      this.changeDetectorRef.markForCheck();
    });
  }

  getPageSize(): number {
    return Common.PAGE_SIZE;
  }

  onFirstClick(): void {
    this.currentPage = 1;
    this.updatePendingList();
  }

  onLastClick(): void {
    this.currentPage = this.totalPage;
    this.updatePendingList();
  }

  onPreviousClick(): void {
    --this.currentPage;
    this.updatePendingList();
  }

  onNextClick(): void {
    ++this.currentPage;
    this.updatePendingList();
  }

  hasNextPage(): boolean {
    return !(this.currentPage >= this.totalPage);
  }

  hasPreviousPage(): boolean {
    return (this.currentPage * Common.PAGE_SIZE > Common.PAGE_SIZE);
  }

}
