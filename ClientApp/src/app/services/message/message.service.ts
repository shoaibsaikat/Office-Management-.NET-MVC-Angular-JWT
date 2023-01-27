import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  errorMessages: string[] = [];
  messages: string[] = [];

  constructor() { }

  add(message: string): void {
    this.messages.push(message);
  }

  clear(): void {
    this.messages = [];
  }

  addError(message: string): void {
    this.errorMessages.push(message);
  }

  clearError(): void {
    this.errorMessages = [];
  }

  clearAll(): void {
    this.messages = [];
    this.errorMessages = [];
  }

}
