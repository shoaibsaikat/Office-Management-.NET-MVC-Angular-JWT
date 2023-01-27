import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { GoogleChartsModule } from 'angular-google-charts';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
//import { NavMenuComponent } from './nav-menu/nav-menu.component';
//import { CounterComponent } from './counter/counter.component';
//import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { SigninComponent } from './components/account/signin/signin.component';
import { SignoutComponent } from './components/account/signout/signout.component';
import { HomeComponent } from './components/home/home.component';
import { ProfileComponent } from './components/account/profile/profile.component';
import { ManagerComponent } from './components/account/manager/manager.component';
import { PasswordComponent } from './components/account/password/password.component';
import { MessageComponent } from './components/message/message.component';
import { MyListComponent as AssetMyListComponent } from './components/asset/my-list/my-list.component';
import { AllListComponent as AssetAllListComponent } from './components/asset/all-list/all-list.component';
import { PendingListComponent as AssetPendingListComponent } from './components/asset/pending-list/pending-list.component';
import { CreateComponent as AssetCreateComponent } from './components/asset/create/create.component';
import { EditComponent as AssetEditComponent } from './components/asset/edit/edit.component';
import { ListComponent as InventoryListComponent } from './components/inventory/list/list.component';
import { CreateComponent as InventoryCreateComponent } from './components/inventory/create/create.component';
import { EditComponent as InventoryEditComponent } from './components/inventory/edit/edit.component';
import { CreateComponent as RequisitionCreateComponent } from './components/requisition/create/create.component';
import { HistoryComponent as RequisitionHistoryComponent } from './components/requisition/history/history.component';
import { MyListComponent as RequisitionMyListComponent } from './components/requisition/my-list/my-list.component';
import { DetailComponent as RequisitionDetailComponent } from './components/requisition/detail/detail.component';
import { PendingApprovalComponent as RequisitionPendingApprovalComponent } from './components/requisition/pending-approval/pending-approval.component';
import { PendingDistributionComponent as RequisitionPendingDistributionComponent } from './components/requisition/pending-distribution/pending-distribution.component';
import { MyListComponent as LeaveMyListComponent } from './components/leave/my-list/my-list.component';
import { DetailComponent as LeaveDetailComponent } from './components/leave/detail/detail.component';
import { RequestListComponent as LeaveRequestListComponent } from './components/leave/request-list/request-list.component';
import { SummaryComponent as LeaveSummaryComponent } from './components/leave/summary/summary.component';
import { CreateComponent as LeaveCreateComponent } from './components/leave/create/create.component';
import { LoadingComponent } from './components/loading/loading.component';

@NgModule({
  declarations: [
    AppComponent,
    //NavMenuComponent,
    //CounterComponent,
    //FetchDataComponent,
    SigninComponent,
    SignoutComponent,
    HomeComponent,
    ProfileComponent,
    ManagerComponent,
    PasswordComponent,
    MessageComponent,
    AssetMyListComponent,
    AssetAllListComponent,
    AssetPendingListComponent,
    AssetCreateComponent,
    AssetEditComponent,
    InventoryListComponent,
    InventoryCreateComponent,
    InventoryEditComponent,
    RequisitionCreateComponent,
    RequisitionHistoryComponent,
    RequisitionMyListComponent,
    RequisitionDetailComponent,
    RequisitionPendingApprovalComponent,
    RequisitionPendingDistributionComponent,
    LeaveMyListComponent,
    LeaveDetailComponent,
    LeaveRequestListComponent,
    LeaveSummaryComponent,
    LeaveCreateComponent,
    LoadingComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    GoogleChartsModule,
    //RouterModule.forRoot([
    //  { path: '', component: HomeComponent, pathMatch: 'full' },
    //  { path: 'counter', component: CounterComponent },
    //  { path: 'fetch-data', component: FetchDataComponent },
    //])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
