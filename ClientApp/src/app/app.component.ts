import { Component, ChangeDetectionStrategy } from '@angular/core';

import { Observable } from 'rxjs';

import { User } from './shared/types/user';

import { GlobalService } from './services/global/global.service';
import { LoadingService } from './services/loading/loading.service';

// NOTE: never ever inject / import AppComponent somewhere else

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  //styleUrls: ['./app.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent {
  title = 'Office Management';

  constructor(
    private loadingService: LoadingService,
    private globalService: GlobalService) {
    this.globalService.loadCurrentUser();
    // if (this.isValidUser()) {
    //   globalService.getNewAccessToken();
    // }
  }

  // some functions are just forwared to GlobalService, this is done to make minimal change,
  // otherwise, we've to change call to GlobalService from a lot of components

  ngOnInit(): void {
    this.globalService.clearAllMessage();
  }

  setError(msg: string): void {
    this.globalService.setError(msg);
  }

  navigate(path: string): void {
    this.globalService.navigate(path);
  }

  isLoading(): Observable<boolean> {
    return this.loadingService.isLoading();
  }

  getEmptyUser() {
    return this.globalService.getEmptyUser();
  }

  saveEmptyUser(): void {
    this.globalService.saveEmptyUser();
  }

  loadCurrentUser(): void {
    this.globalService.loadCurrentUser();
  }

  saveCurrentUser(): void {
    this.globalService.saveCurrentUser();
  }

  setCurrentUser(user: User): void {
    this.globalService.setCurrentUser(user);
  }

  isValidUser(): boolean {
    return this.globalService.isValidUser();
  }

  logOut(): void {
    this.globalService.logOut();
    this.globalService.unsubscribeAccessToken();
  }

  getUser(): User {
    return this.globalService.getUser();
  }

}
