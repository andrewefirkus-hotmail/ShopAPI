import { inject, Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Product, CreateProduct, UpdateProduct } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private api = inject(ApiService);

  getAll()                              { return this.api.get<Product[]>('/api/products'); }
  getById(id: string)                   { return this.api.get<Product>(`/api/products/${id}`); }
  create(dto: CreateProduct)            { return this.api.post<Product>('/api/products', dto); }
  update(id: string, dto: UpdateProduct){ return this.api.put<Product>(`/api/products/${id}`, dto); }
  delete(id: string)                    { return this.api.delete<void>(`/api/products/${id}`); }
}
