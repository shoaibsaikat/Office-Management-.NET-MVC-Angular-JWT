import { Component, OnInit } from '@angular/core';

import { FormGroup, FormControl, Validators } from '@angular/forms';

import { GlobalService } from 'src/app/services/global/global.service';
import { InventoryService } from 'src/app/services/inventory/inventory.service';
import { MessageService } from 'src/app/services/message/message.service';

import { Inventory } from 'src/app/shared/types/inventory';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {

  inventoryForm: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required, ]),
    unit: new FormControl('', [Validators.required, ]),
    count: new FormControl('', [Validators.required, ]),
    description: new FormControl(),
  });
  get name() { return this.inventoryForm.get('name'); }
  get unit() { return this.inventoryForm.get('unit'); }
  get count() { return this.inventoryForm.get('count'); }
  get description() { return this.inventoryForm.get('description'); }

  constructor(
    private inventoryService: InventoryService,
    private messageService: MessageService,
    private globalService: GlobalService) { }

  ngOnInit(): void {
  }

  onSubmit(): void {
    let item: Inventory = {
      id: -1,
      name: this.name?.value,
      count: this.count?.value,
      unit: this.unit?.value,
      description: this.description?.value,
    }
    this.inventoryService.createInventoryItem(item).subscribe(data => {
      // console.log('CreateComponent: item.name: ' + data);
      let msg = JSON.parse(JSON.stringify(data));
      this.messageService.add(msg);
      this.globalService.navigate('/inventory/list');
    });

  }

}
