import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { FormGroup, FormControl, Validators } from '@angular/forms';

import { GlobalService } from 'src/app/services/global/global.service';
import { InventoryService } from 'src/app/services/inventory/inventory.service';

import { Inventory } from 'src/app/shared/types/inventory';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditComponent implements OnInit {

  id: number = 0;
  unit: string = '';
  name: string = '';
  inventoryForm: FormGroup = new FormGroup({
    count: new FormControl('', [Validators.required, ]),
    description: new FormControl(),
  });
  get count() { return this.inventoryForm.get('count'); }
  get description() { return this.inventoryForm.get('description'); }

  constructor(
    private inventoryService: InventoryService,
    private globalService: GlobalService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    let item = this.inventoryService.getCurrentInventory();
    this.id = item.id;
    this.name = item.name;
    this.unit = item.unit;
    this.inventoryForm.get('count')?.setValue(item.count);

    this.inventoryService.getEditInfo(this.id).subscribe({
      next: (v) => {
        // console.log('EditComponent: ' + JSON.stringify(v));
        let description: string = JSON.parse(JSON.stringify(v)).description;
        // console.log('EditComponent: ' + description);
        this.inventoryForm.get('description')?.setValue(description);
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
    let item:Inventory = {
      id: this.id,
      name: this.name,
      count: this.count?.value,
      unit: this.unit,
      description: this.description?.value,
    }
    // console.log('EditComponent: item.name: ' + this.name?.value);
    this.inventoryService.updateInventory(item).subscribe(data => {
      // console.log('ManagerComponent: ' + data.detail);
      this.globalService.navigate('inventory/list');
    });

  }

}
