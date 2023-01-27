import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { FormGroup, FormControl, Validators } from '@angular/forms';

import { GlobalService } from 'src/app/services/global/global.service';
import { AssetService } from 'src/app/services/asset/asset.service';

import { Asset } from 'src/app/shared/types/asset';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditComponent implements OnInit {

  id: number = 0;

  assetForm: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required, ]),
    warranty: new FormControl('', [Validators.required, ]),
    status: new FormControl('', [Validators.required, ]),
    description: new FormControl(),
  });
  get name() { return this.assetForm.get('name'); }
  get warranty() { return this.assetForm.get('warranty'); }
  get status() { return this.assetForm.get('status'); }
  get description() { return this.assetForm.get('description'); }

  statusList: Map<number, string> = new Map<number, string>();

  constructor(
    private activatedRoute: ActivatedRoute,
    private assetService: AssetService,
    private globalService: GlobalService,
    private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {

    this.activatedRoute.paramMap.subscribe(params => {
        this.id = Number(params.get('id'));
        // console.log('EditComponent: ' + this.id);

        this.assetService.getEditInfo(this.id).subscribe({
          next: (v) => {
            // console.log('EditComponent: ' + JSON.stringify(v));
            let objAsset: Asset = JSON.parse(JSON.stringify(v)).asset;
            let objStatusList: any = JSON.parse(JSON.stringify(v)).status;
            // console.log('EditComponent: ' + JSON.stringify(objAsset));
            // console.log('EditComponent: ' + JSON.stringify(objStatusList));
            // parsing status list
            var i = 0;
            while(true) {
              if (objStatusList[i] == null || objStatusList[i] == undefined) {
                break;
              }
              this.statusList.set(i, objStatusList[i++]);
            }
            this.name?.setValue(objAsset.name);
            this.warranty?.setValue(objAsset.warranty);
            this.description?.setValue(objAsset.description);
            this.status?.setValue(objAsset.status);
          },
          error: (e) => {
            // console.error(e);
            this.globalService.handleUnauthorizedAccess(e);
          },
          complete: () => {
            this.changeDetectorRef.markForCheck();
          }
        });
    });
  }

  onSubmit(): void {
    let asset = {
      id: this.id,
      name: this.name?.value,
      warranty: this.warranty?.value,
      status: this.status?.value,
      description: this.description?.value || '',
    };

    this.assetService.updateAsset(asset).subscribe(data => {
      // console.log('ManagerComponent: ' + data.detail);
      this.globalService.navigate('asset/all_list');
    });
    // console.log('EditComponent: ' + asset.description + ', ' + asset.status + ', ' + asset.warranty);
  }

  checkIfSelected(item: number): boolean {
    // console.log('EditComponent: checkIfSelected()' + item + ', ' + this.status?.value);
    return true ? item == this.status?.value : false;
  }

}
