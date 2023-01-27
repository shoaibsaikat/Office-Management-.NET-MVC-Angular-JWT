import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { NgForm } from '@angular/forms';

import { HttpErrorResponse } from '@angular/common/http';

import { GlobalService } from 'src/app/services/global/global.service';
import { AccountService } from '../../../services/account/account.service';
import { MessageService } from 'src/app/services/message/message.service';

import { SignIn } from '../../../shared/types/signin';
import { User } from 'src/app/shared/types/user';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SigninComponent implements OnInit {

  constructor(
    private globalService: GlobalService,
    private accountService: AccountService,
    private messageService: MessageService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
  }

  login(form: NgForm): void {
    let logIn: SignIn = form.value;
    this.accountService.logIn(logIn).subscribe({
      next: (v) => {
		    // console.log(JSON.stringify(v));
        let refresh_token = JSON.parse(JSON.stringify(v)).refresh;
        let access_token = JSON.parse(JSON.stringify(v)).access;
		
        // console.log('SigninComponent: refresh ' + refresh_token);
        // console.log('SigninComponent: access ' + access_token);

        let user: User = this.globalService.getUser();
        user.access_token = access_token;
        user.refresh_token = refresh_token;
        this.globalService.setCurrentUser(user);

        this.globalService.getUserInfo();
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

}
