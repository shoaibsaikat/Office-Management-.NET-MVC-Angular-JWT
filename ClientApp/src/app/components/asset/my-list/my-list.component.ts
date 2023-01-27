import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { FormGroup, FormControl } from '@angular/forms';

import { GlobalService } from 'src/app/services/global/global.service';
import { AssetService } from 'src/app/services/asset/asset.service';
import { MessageService } from 'src/app/services/message/message.service';

import { User } from '../../../shared/types/user';
import { Asset } from 'src/app/shared/types/asset';
import { Common } from 'src/app/shared/common';

@Component({
  selector: 'app-my-list',
  templateUrl: './my-list.component.html',
  styleUrls: ['./my-list.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MyListComponent implements OnInit {

  userList: User[] = [];
  assetList: Asset[] = [];
  assignFormList: FormGroup[] = [];

  // pagination, NOTE: pagination is by 10 in server side and can't be set from client
  listCount: number = 0;
  currentPage: number = 1;
  totalPage: number = 1;

  constructor(
    private assetService: AssetService,
    private messageService: MessageService,
    private globalService: GlobalService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.updateMyList();
  }

  updateMyList(): void {
    this.assetService.getMyAssetList(this.currentPage).subscribe({
      next: (v) => {
        // console.log('MyListComponent: ' + JSON.stringify(v));
        let userList: User[] = JSON.parse(JSON.stringify(v)).user_list;
        let assetList: Asset[] = JSON.parse(JSON.stringify(v)).asset_list;
        this.listCount = JSON.parse(JSON.stringify(v)).count;
        this.totalPage = Math.ceil(this.listCount / Common.PAGE_SIZE);

        assetList.forEach(element => {
          if (element) {
            this.assetList.push(element);
            // console.log('MyListComponent: id ' + element.id + ':' + element.user + ':' + element.next_user + ','  + element.name);
          }
        });

        userList.forEach(element => {
          if (element && element.id != this.globalService.getUser().id) {
            this.userList.push(element);
          }
        });
        // console.log('MyListComponent: updateMyList() ' + this.userList.length);

        // generate form groups
        this.assetList.forEach(element => {
          this.assignFormList.push(new FormGroup({
            user: new FormControl(),
          }));
        });
        // console.log('MyListComponent: updateMyList() ' + this.assignFormList.length);
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

  onSubmit(index: number): void {
    // console.log('MyListComponent: index: ' + index + ', item.id: ' + this.assetList[index].id + ', user id: ' + this.assignFormList[index].get('user')?.value);
    this.assetService.assignAsset(this.assetList[index].id, this.assignFormList[index].get('user')?.value).subscribe(data => {
      // console.log('MyListComponent: ' + JSON.stringify(data));
      let msg: string = JSON.parse(JSON.stringify(data));
      this.messageService.add(msg);
      // update local data
      this.assetList[index].next_user = this.assignFormList[index].get('user')?.value;

      this.changeDetectorRef.markForCheck();
    });
  }

  getPageSize(): number {
    return Common.PAGE_SIZE;
  }

  onFirstClick(): void {
    this.currentPage = 1;
    this.updateMyList();
  }

  onLastClick(): void {
    this.currentPage = this.totalPage;
    this.updateMyList();
  }

  onPreviousClick(): void {
    --this.currentPage;
    this.updateMyList();
  }

  onNextClick(): void {
    ++this.currentPage;
    this.updateMyList();
  }

  hasNextPage(): boolean {
    return !(this.currentPage >= this.totalPage);
  }

  hasPreviousPage(): boolean {
    return (this.currentPage * Common.PAGE_SIZE > Common.PAGE_SIZE);
  }

}
