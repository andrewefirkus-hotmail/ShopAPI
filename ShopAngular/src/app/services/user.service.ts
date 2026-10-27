import { inject, Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { User, CreateUser, UpdateUser } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private api = inject(ApiService);

  getAll()                          { return this.api.get<User[]>('/api/users'); }
  getById(id: string)               { return this.api.get<User>(`/api/users/${id}`); }
  create(dto: CreateUser)           { return this.api.post<User>('/api/users', dto); }
  update(id: string, dto: UpdateUser) { return this.api.put<User>(`/api/users/${id}`, dto); }
  delete(id: string)                { return this.api.delete<void>(`/api/users/${id}`); }
}
