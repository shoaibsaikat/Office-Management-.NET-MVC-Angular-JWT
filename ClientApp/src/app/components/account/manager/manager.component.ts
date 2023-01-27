import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

import { GlobalService } from 'src/app/services/global/global.service';
import { AccountService } from '../../../services/account/account.service';

import { User } from 'src/app/shared/types/user';

@Component({
  selector: 'app-manager',
  templateUrl: './manager.component.html',
  styleUrls: ['./manager.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class ManagerComponent implements OnInit {

  managerList: User[] = [];
  managerForm = new FormGroup({
    manager: new FormControl(),
  });
  currentManger: number = -1;
  currentManagerName: string = "";

  constructor(
    private globalService: GlobalService,
    private accountService: AccountService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit() {
    this.getMangerList();
    this.currentManger = this.globalService.getUser().manager_id || -1;
    // console.log('ManagerComponent: current ' + this.currentManagerName);
  }

  getMangerList() {
    this.accountService.getMangerList().subscribe({
      next: (v) => {
        // console.log('ManagerComponent: ' + JSON.parse(JSON.stringify(v)).user_list.result);
        this.managerList = [];
        let objList: User[] = JSON.parse(JSON.stringify(v)).user_list.result;
        objList.forEach(element => {
          if (element && element.id != this.currentManger) {
            this.managerList.push(element);
            // console.log('ManagerComponent: id ' + element.id + ' '  + element.first_name);
          } else {
            // save current manager name
            this.currentManagerName = element.first_name + ' ' + element.last_name;
            // console.log('ManagerComponent: ' + this.currentManagerName);
          }
        });
      },
      error: (e) => {
        // console.error(e);
        // handleUnauthorizedAccess contains logout which will redirect, so mark for change is not needed
        this.globalService.handleUnauthorizedAccess(e);
      },
      complete: () => {
        // This mark for change is needed
        this.changeDetectorRef.markForCheck();
      }
    });
  }

  onSubmit(): void {
    // console.log('ManagerComponent: selected ' + this.managerForm.value.manager);
    this.accountService.setManger(this.managerForm.value.manager).subscribe(data => {
      // console.log('ManagerComponent: ' + data.detail);
      this.globalService.getUser().manager_id = this.managerForm.value.manager;
      this.globalService.saveCurrentUser();
      this.currentManger = this.managerForm.value.manager;
      this.globalService.navigate('');
    });
  }

}
