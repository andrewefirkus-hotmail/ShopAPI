import { inject, Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Order, CreateOrder, UpdateOrder } from '../models/order.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private api = inject(ApiService);

  getAll()                             { return this.api.get<Order[]>('/api/orders'); }
  getById(id: string)                  { return this.api.get<Order>(`/api/orders/${id}`); }
  create(dto: CreateOrder)             { return this.api.post<Order>('/api/orders', dto); }
  update(id: string, dto: UpdateOrder) { return this.api.put<Order>(`/api/orders/${id}`, dto); }
  delete(id: string)                   { return this.api.delete<void>(`/api/orders/${id}`); }
}
