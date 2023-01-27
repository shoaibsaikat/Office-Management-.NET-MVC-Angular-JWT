import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable, of } from 'rxjs';

import { User } from '../../shared/types/user';
import { SignIn } from '../../shared/types/signin';
import { Common } from '../../shared/common';


@Injectable({
  providedIn: 'root'
})

export class AccountService {

  private baseUrl: string = Common.getBaseUrl().concat('user/');
  private logInUrl: string = this.baseUrl.concat('login/');
  // private refreshUrl: string = Common.getBaseUrl().concat('token/refresh/');
  private logOutUrl: string = this.baseUrl.concat('signout/');
  private profileUrl: string = this.baseUrl.concat('change_profile/');
  private managerUrl: string = this.baseUrl.concat('change_manager/');
  private passwordUrl: string = this.baseUrl.concat('change_password/');
  private userUrl: string = this.baseUrl.concat('get/');

  constructor(private http: HttpClient) { }

  logIn(user: SignIn): Observable<string> {
    return this.http.post<string>(this.logInUrl, {
      username: user.username,
      password: user.password1,
    });
  }

  // getAccessToken(refreshToken: string): Observable<string> {
  //   return this.http.post<string>(this.refreshUrl, {
  //     refresh: refreshToken,
  //   });
  // }

  logOut(): Observable<string> {
    return this.http.post<string>(this.logOutUrl, null, Common.getHttpHeader());
  }

  getUserInfo(): Observable<User> {
    return this.http.get<User>(this.userUrl, Common.getHttpHeader());
  }

  changeInfo(user: User): Observable<string> {
    return this.http.post<string>(this.profileUrl, {
      first_name: user.first_name,
      last_name: user.last_name,
      email: user.email || '',
    }, Common.getHttpHeader());
  }

  getMangerList(): Observable<string> {
    return this.http.get<string>(this.managerUrl, Common.getHttpHeader());
  }

  setManger(id: number): Observable<string> {
    return this.http.post<string>(this.managerUrl, {
      manager: id,
    }, Common.getHttpHeader());
  }

  setPassword(last_pass: string, new_pass: string): Observable<string> {
    return this.http.post<string>(this.passwordUrl, {
      lastpassword: last_pass,
      newpassword: new_pass,
    }, Common.getHttpHeader());
  }

}
