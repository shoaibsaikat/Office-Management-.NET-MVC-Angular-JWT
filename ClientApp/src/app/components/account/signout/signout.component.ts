import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { GlobalService } from 'src/app/services/global/global.service';
import { AccountService } from '../../../services/account/account.service';
import { MessageService } from 'src/app/services/message/message.service';

@Component({
  selector: 'app-signout',
  templateUrl: './signout.component.html',
  styleUrls: ['./signout.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SignoutComponent implements OnInit {

  constructor(
    private globalService: GlobalService,
    private accountService: AccountService,
    private messageService: MessageService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
  }

  signout(): void  {
    this.accountService.logOut().subscribe(data => {
      // console.log('SignoutComponent: ' + data.detail);
      this.globalService.logOut();
      this.changeDetectorRef.markForCheck();
    });
  }

}
