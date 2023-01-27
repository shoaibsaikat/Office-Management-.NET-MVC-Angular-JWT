import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { FormGroup, FormControl, Validators } from '@angular/forms';

import { GlobalService } from 'src/app/services/global/global.service';
import { RequisitionService } from 'src/app/services/requisition/requisition.service';
import { MessageService } from 'src/app/services/message/message.service';

import { User } from 'src/app/shared/types/user';
import { Inventory } from 'src/app/shared/types/inventory';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CreateComponent implements OnInit {

  requisitionForm: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required, ]),
    inventory: new FormControl('', [Validators.required, ]),
    approver: new FormControl('', [Validators.required, ]),
    amount: new FormControl('', [Validators.required, ]),
    comment: new FormControl(),
  });
  get title() { return this.requisitionForm.get('title'); }
  get inventory() { return this.requisitionForm.get('inventory'); }
  get approver() { return this.requisitionForm.get('approver'); }
  get amount() { return this.requisitionForm.get('amount'); }
  get comment() { return this.requisitionForm.get('comment'); }

  inventoryList: Map<number, string> = new Map<number, string>();
  approverList: Map<number, string> = new Map<number, string>();

  constructor(
    private requisitionService: RequisitionService,
    private messageService: MessageService,
    private globalService: GlobalService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.requisitionService.getAddInfo().subscribe({
      next: (v) => {
        // console.log('CreateComponent: ' + JSON.stringify(v));
        let inventoryList: Inventory[] = JSON.parse(JSON.stringify(v)).inventory_list;
        let approverList: User[] = JSON.parse(JSON.stringify(v)).approver_list;

        inventoryList.forEach(element => {
          if (element) {
            this.inventoryList.set(element.id, element.name);
          }
        });

        approverList.forEach(element => {
          if (element) {
            this.approverList.set(element.id, element.first_name + ' ' + element.last_name);
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

  onSubmit(): void {
    this.requisitionService.createRequisition(this.title?.value, this.inventory?.value, this.approver?.value, this.amount?.value, this.comment?.value).subscribe(data => {
      this.globalService.navigate('requisition/my_list');
    });
  }

}
