import { Injectable, signal } from '@angular/core';

export interface ToastMessage {
  id: number;
  type: 'success' | 'error' | 'info' | 'warning';
  text: string;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  readonly toasts = signal<ToastMessage[]>([]);
  private counter = 0;

  success(text: string): void {
    this.add('success', text);
  }

  error(text: string): void {
    this.add('error', text);
  }

  info(text: string): void {
    this.add('info', text);
  }

  warning(text: string): void {
    this.add('warning', text);
  }

  remove(id: number): void {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }

  private add(type: 'success' | 'error' | 'info' | 'warning', text: string): void {
    const id = ++this.counter;
    this.toasts.update(list => [...list, { id, type, text }]);
    setTimeout(() => this.remove(id), 5000);
  }
}

