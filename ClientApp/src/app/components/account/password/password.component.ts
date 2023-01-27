import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { Validators, FormBuilder, FormGroup, FormControl, FormArray, ValidatorFn, ValidationErrors, AbstractControl } from '@angular/forms';

import { GlobalService } from 'src/app/services/global/global.service';
import { AccountService } from '../../../services/account/account.service';
import { MessageService } from 'src/app/services/message/message.service';

@Component({
  selector: 'app-password',
  templateUrl: './password.component.html',
  styleUrls: ['./password.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PasswordComponent implements OnInit {

  passwordForm = new FormGroup({
    oldPassword: new FormControl('', [Validators.required]),
    newPassword1: new FormControl('', [Validators.required]),
    newPassword2: new FormControl('', [Validators.required]),
  }, {
    validators: [this.passwordMatchValidation()],
  });
  get oldPassword() { return this.passwordForm.get('oldPassword'); }
  get newPassword1() { return this.passwordForm.get('newPassword1'); }
  get newPassword2() { return this.passwordForm.get('newPassword2'); }

  constructor(
    private globalService: GlobalService,
    private messageService: MessageService,
    private accountService: AccountService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
  }

  onSubmit(): void {
    if (this.oldPassword?.errors || this.newPassword1?.errors || this.newPassword2?.errors) {
      return;
    }

    this.accountService.setPassword(this.passwordForm.value.oldPassword || '', this.passwordForm.value.newPassword1 || '').subscribe({
      next: (v) => {
		    // console.log('PasswordComponent: ' + data.detail);
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
        this.messageService.addError('Passowrd Changed successfully');
        this.globalService.navigate('');
      }
    });
  }

  passwordMatchValidation(): ValidatorFn {
    return (control:AbstractControl): ValidationErrors | null => {
      if (!this.passwordForm || !this.passwordForm.get('newPassword1') || !this.passwordForm.get('newPassword2')) {
        return null;
      }

      if (this.passwordForm.get('newPassword1')?.value != this.passwordForm.get('newPassword2')?.value) {
        return { noMatch: true };
      }
      return null;
    };
  }

}
