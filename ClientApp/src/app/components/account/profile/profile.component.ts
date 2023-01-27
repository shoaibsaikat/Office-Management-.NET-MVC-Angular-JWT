import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { GlobalService } from 'src/app/services/global/global.service';
import { AccountService } from 'src/app/services/account/account.service';
import { MessageService } from 'src/app/services/message/message.service';

import { User } from 'src/app/shared/types/user';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class ProfileComponent implements OnInit {
  user: User = this.globalService.getUser();
  profileForm: FormGroup = new FormGroup({
    firstName: new FormControl('', [Validators.required, ]),
    lastName: new FormControl('', [Validators.required, ]),
    email: new FormControl('', [Validators.required, Validators.email, ]),
  });
  // get is not mandatory, it's for less code, if get is not added in that case from html we can get FormControl by using, profileForm.get('')
  get firstName() { return this.profileForm.get('firstName'); }
  get lastName() { return this.profileForm.get('lastName'); }
  get email() { return this.profileForm.get('email'); }

  constructor(
    private globalService: GlobalService,
    private messageService: MessageService,
    private accountService: AccountService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.firstName?.setValue(this.user.first_name);
    this.lastName?.setValue(this.user.last_name);
    this.email?.setValue(this.user.email);
  }

  onSubmit(): void {
    if (this.firstName?.errors || this.lastName?.errors || this.email?.errors) {
      return;
    }

    let first: string = this.profileForm.value.firstName || '';
    let last: string = this.profileForm.value.lastName || '';
    let email: string = this.profileForm.value.email || '';

    let user = {
      'id': 0,
      'username': '',
      'first_name': first.trim(),
      'last_name': last.trim(),
      'email': email.trim(),
    }

    this.accountService.changeInfo(user).subscribe({
      next: (v) => {
		     // console.log('ProfileComponent: ' + data.detail);
         this.globalService.getUser().first_name = user.first_name;
         this.globalService.getUser().last_name = user.last_name;
         this.globalService.getUser().email = user.email;
         this.globalService.saveCurrentUser();
         this.globalService.navigate('');
      },
      error: (e) => {
        // console.error(e);
        if (!this.globalService.handleUnauthorizedAccess(e)) {
          this.globalService.handleHttpErrorMessage(e, 400);
        }
        this.changeDetectorRef.markForCheck();
      },
      complete: () => {
        // Incase of navigation this.changeDetectorRef.markForCheck(); is not needed as page change will trigger change
        // console.log('complete');
        this.messageService.addError('Profile Changed successfully');
        this.globalService.navigate('');
      }
    });
  }
}
