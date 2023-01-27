import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { FormGroup, FormControl, Validators } from '@angular/forms';

import { GlobalService } from 'src/app/services/global/global.service';
import { AssetService } from 'src/app/services/asset/asset.service';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CreateComponent implements OnInit {

  assetForm: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required, ]),
    model: new FormControl('', [Validators.required, ]),
    serial: new FormControl('', [Validators.required, ]),
    purchaseDate: new FormControl('', [Validators.required, ]),
    warranty: new FormControl('', [Validators.required, ]),
    type: new FormControl('', [Validators.required, ]),
    status: new FormControl('', [Validators.required, ]),
    description: new FormControl(),
  });
  get name() { return this.assetForm.get('name'); }
  get model() { return this.assetForm.get('model'); }
  get serial() { return this.assetForm.get('serial'); }
  get purchaseDate() { return this.assetForm.get('purchaseDate'); }
  get warranty() { return this.assetForm.get('warranty'); }
  get type() { return this.assetForm.get('type'); }
  get status() { return this.assetForm.get('status'); }
  get description() { return this.assetForm.get('description'); }

  statusList: Map<number, string> = new Map<number, string>();
  typeList: Map<number, string> = new Map<number, string>();

  constructor(
    private assetService: AssetService,
    private globalService: GlobalService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.assetService.getAddInfo().subscribe({
      next: (v) => {
        // console.log('CreateComponent: ' + JSON.stringify(v));
        let statusObj: any = JSON.parse(JSON.stringify(v)).status;
        let typeObj: any = JSON.parse(JSON.stringify(v)).type;

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
      },
      error: (e) => {
        // console.log(e);
        // handleUnauthorizedAccess contains logout which will redirect, so mark for change is not needed
        this.globalService.handleUnauthorizedAccess(e);
      },
      complete: () => {
        this.changeDetectorRef.markForCheck();
      }
    });
  }

  onSubmit(): void {
    let asset = {
      name: this.name?.value,
      model: this.model?.value,
      serial: this.serial?.value,
      purchaseDate: (new Date(this.purchaseDate?.value).getTime() / 1000).toString(),
      warranty: this.warranty?.value,
      status: this.status?.value,
      type: this.type?.value,
      description: this.description?.value || '',
    };

    this.assetService.createAsset(asset).subscribe(data => {
      // console.log('ManagerComponent: ' + data.detail);
      this.globalService.navigate('asset/all_list');
    });
    // console.log('CreateComponent: ' + asset.description + ', ' + asset.status + ', ' + asset.type + ', ' + asset.warranty + ', ' + asset.purchaseDate);
  }

}
