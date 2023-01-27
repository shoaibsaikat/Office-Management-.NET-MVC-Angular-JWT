import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { Common } from '../../shared/common';
import { User } from 'src/app/shared/types/user';
import { Requisition } from 'src/app/shared/types/requisition';

@Injectable({
  providedIn: 'root'
})
export class RequisitionService {

  private common: Common = new Common(this.http);

  private currentRequisition: Requisition = {
    id: 0,
    amount: 0,
    approver: 0,
    approver_name: '',
    date: '',
    item_name: '',
    title: '',
    total: 0,
    unit: '',
    user: 0,
    user_name: '',
  };
  private distributorList: User[] = [];

  private baseUrl: string = Common.getBaseUrl().concat('inventory/requisition/');
  private historyUrl: string = this.baseUrl.concat('history/');
  private myRequisitionUrl: string = this.baseUrl.concat('my_list/');
  private detailUrl: string = this.baseUrl.concat('detail/');
  private createUrl: string = this.baseUrl.concat('create/');
  private approvalUrl: string = this.baseUrl.concat('approval/');
  private denialUrl: string = this.baseUrl.concat('denial/');
  private distributionUrl: string = this.baseUrl.concat('distribution/');

  constructor(private http: HttpClient) { }

  getHistory(page: number = 1): Observable<string> {
    let historyUrl = this.historyUrl.concat(page + '');
    return this.http.get<string>(historyUrl, Common.getHttpHeader());
  }

  getMyRequisitionList(page: number = 1): Observable<string> {
    let myRequisitionUrl = this.myRequisitionUrl.concat(page + '');
    return this.http.get<string>(myRequisitionUrl, Common.getHttpHeader());
  }

  getDetail(item: number): Observable<string> {
    let detailItemUrl: string = this.detailUrl.concat(item + '/');
    return this.http.get<string>(detailItemUrl, Common.getHttpHeader());
  }

  getAddInfo(): Observable<string> {
    return this.http.get<string>(this.createUrl, Common.getHttpHeader());
  }

  createRequisition(title: string, inventory: number, approver: number, amount: number, comment: string): Observable<string> {
    // console.log(title + ":" + inventory + ":" + approver + ":" + amount + ":" + comment);
    return this.http.post<string>(this.createUrl, {
      title: title,
      inventory: inventory,
      approver: approver,
      amount: amount,
      comment: comment,
    }, Common.getHttpHeader());
  }

  getApprovalList(page: number = 1): Observable<string> {
    let approvalUrl = this.approvalUrl.concat(page + '');
    return this.http.get<string>(approvalUrl, Common.getHttpHeader());
  }

  approve(id: number, distributor: number): Observable<string> {
    return this.http.post<string>(this.approvalUrl, {
      pk: id,
      distributor: distributor,
    }, Common.getHttpHeader());
  }

  deny(id: number): Observable<string> {
    return this.http.post<string>(this.denialUrl, {
      pk: id
    }, Common.getHttpHeader());
  }

  getDistributionList(page: number = 1): Observable<string> {
    let distributionUrl = this.distributionUrl.concat(page + '');
    return this.http.get<string>(distributionUrl, Common.getHttpHeader());
  }

  distribute(id: number): Observable<string> {
    return this.http.post<string>(this.distributionUrl, {
      pk: id,
    }, Common.getHttpHeader());
  }

  setCurrentRequisition(item: Requisition): void {
    this.currentRequisition = item;
  }

  getCurrentRequisition(): Requisition {
    return this.currentRequisition;
  }

  setDistributorList(users: User[]): void {
    this.distributorList = users;
  }

  getDistributorList(): User[] {
    return this.distributorList;
  }
}
