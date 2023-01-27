import { Injectable } from '@angular/core';

import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {

  private isLoadingSubject = new BehaviorSubject<boolean>(false);
  isLoadingObserver = this.isLoadingSubject.asObservable();

  constructor() { }

  setLoading(): void {
    this.isLoadingSubject.next(true);
  }

  unsetLoading(): void {
    this.isLoadingSubject.next(false);
  }

  isLoading(): Observable<boolean> {
    return this.isLoadingObserver;
  }
}
