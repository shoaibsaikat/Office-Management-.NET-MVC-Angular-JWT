import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { AssetService } from 'src/app/services/asset/asset.service';
import { GlobalService } from 'src/app/services/global/global.service';

import { Asset } from 'src/app/shared/types/asset';
import { AssetViewModel } from 'src/app/shared/types/asset_viewmodel';

import { Common } from 'src/app/shared/common';

@Component({
  selector: 'app-all-list',
  templateUrl: './all-list.component.html',
  styleUrls: ['./all-list.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AllListComponent implements OnInit {

  assetList: AssetViewModel[] = [];
  statusList: Map<number, string> = new Map<number, string>();
  typeList: Map<number, string> = new Map<number, string>();

  // pagination, NOTE: pagination is by 10 in server side and can't be set from client
  listCount: number = 0;
  currentPage: number = 1;
  totalPage: number = 1;

  constructor(
    private globalService: GlobalService,
    private assetService: AssetService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.updateAllList();
  }

  updateAllList(): void {
    this.assetService.getAllAssetList(this.currentPage).subscribe({
      next: (v) => {
        // console.log('MyListComponent: ' + JSON.stringify(v));
        let assetList: Asset[] = JSON.parse(JSON.stringify(v)).asset_list;
        let statusObj: any = JSON.parse(JSON.stringify(v)).status;
        let typeObj: any = JSON.parse(JSON.stringify(v)).type;
        this.listCount = JSON.parse(JSON.stringify(v)).count;
        this.totalPage = Math.ceil(this.listCount / Common.PAGE_SIZE);

        let i: number = 0;
        while(true) {
          if (statusObj[i]) {
            // console.log(statusObj[i]);
            this.statusList.set(i, statusObj[i]);
          } else {
            break;
          }
          i++;
        }
        i = 0;
        while(true) {
          if (typeObj[i]) {
            // console.log(typeObj[i]);
            this.typeList.set(i, typeObj[i]);
          } else {
            break;
          }
          i++;
        }

        assetList.forEach(element => {
          if (element) {
            let assetViewModel: AssetViewModel = {
              id: element.id,
              model: element.model,
              name: element.name,
              purchase_date: element.purchase_date,
              serial: element.serial,
              status: this.statusList.get(element.status) || '',
              type: this.typeList.get(element.type) || '',
              user: element.user,
              user_first_name: element.user_first_name,
              user_last_name: element.user_last_name,
              warranty: element.warranty,
              description: element.description,
              next_user: element.next_user,
            }
            this.assetList.push(assetViewModel);
            // console.log('AllListComponent: id:' + assetViewModel.id + ', user:' + element.user, ', status:' + assetViewModel.status);
          }
        });
      },
      error: (e) => {
        // console.error(e);
        // handleUnauthorizedAccess contains logout which will redirect, so mark for change is not needed
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
    this.updateAllList();
  }

  onLastClick(): void {
    this.currentPage = this.totalPage;
    this.updateAllList();
  }

  onPreviousClick(): void {
    --this.currentPage;
    this.updateAllList();
  }

  onNextClick(): void {
    ++this.currentPage;
    this.updateAllList();
  }

  hasNextPage(): boolean {
    return !(this.currentPage >= this.totalPage);
  }

  hasPreviousPage(): boolean {
    return (this.currentPage * Common.PAGE_SIZE > Common.PAGE_SIZE);
  }

}
