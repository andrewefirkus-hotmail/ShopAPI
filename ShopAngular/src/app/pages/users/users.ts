import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { TableModule } from 'primeng/table';
import { Dialog } from 'primeng/dialog';
import { Button } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { UserService } from '../../services/user.service';
import { User, CreateUser, UpdateUser } from '../../models/user.model';

@Component({
  selector: 'app-users',
  imports: [FormsModule, DatePipe, TableModule, Dialog, Button, InputText, Toast],
  providers: [MessageService],
  templateUrl: './users.html',
  styleUrl: './users.scss',
})
export class UsersPage implements OnInit {
  private userService = inject(UserService);
  private messageService = inject(MessageService);

  users = signal<User[]>([]);
  loading = signal(false);

  showCreate = false;
  showEdit = false;
  showDelete = false;
  confirmPending = false;

  createForm: CreateUser = this.emptyCreate();
  editForm: User = this.emptyUser();
  deleteTarget: User | null = null;

  createError = '';
  editError = '';

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.userService.getAll().subscribe({
      next: users => { this.users.set(users); this.loading.set(false); },
      error: () => { this.loading.set(false); },
    });
  }

  // ── Create ──────────────────────────────────────────────────
  openCreate() {
    this.createForm = this.emptyCreate();
    this.createError = '';
    this.showCreate = true;
  }

  submitCreate() {
    this.userService.create(this.createForm).subscribe({
      next: () => {
        this.showCreate = false;
        this.load();
        this.messageService.add({ severity: 'success', summary: 'Created', detail: 'User created successfully.' });
      },
      error: err => {
        if (err.status === 409) {
          this.createError = err.error;
        } else {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to create user.' });
        }
      },
    });
  }

  // ── Edit ────────────────────────────────────────────────────
  openEdit(user: User) {
    this.editForm = { ...user };
    this.editError = '';
    this.confirmPending = false;
    this.showEdit = true;
  }

  requestConfirm() { this.confirmPending = true; }
  cancelConfirm()  { this.confirmPending = false; }

  closeEdit() {
    this.showEdit = false;
    this.confirmPending = false;
  }

  confirmEdit() {
    const dto: UpdateUser = {
      firstName: this.editForm.firstName,
      lastName:  this.editForm.lastName,
      address:   this.editForm.address,
      email:     this.editForm.email,
    };
    this.userService.update(this.editForm.userId, dto).subscribe({
      next: () => {
        this.closeEdit();
        this.load();
        this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'User updated successfully.' });
      },
      error: err => {
        this.confirmPending = false;
        if (err.status === 409) {
          this.editError = err.error;
        } else {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update user.' });
        }
      },
    });
  }

  // ── Delete ──────────────────────────────────────────────────
  openDelete(user: User) {
    this.deleteTarget = user;
    this.showDelete = true;
  }

  confirmDelete() {
    if (!this.deleteTarget) return;
    this.userService.delete(this.deleteTarget.userId).subscribe({
      next: () => {
        this.showDelete = false;
        this.deleteTarget = null;
        this.load();
        this.messageService.add({ severity: 'warn', summary: 'Deleted', detail: 'User deleted.' });
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete user.' });
      },
    });
  }

  // ── Helpers ─────────────────────────────────────────────────
  private emptyCreate(): CreateUser {
    return { firstName: '', lastName: '', address: '', email: '' };
  }

  private emptyUser(): User {
    return { userId: '', firstName: '', lastName: '', address: '', email: '', isDeleted: false, createdAt: '', updatedAt: '' };
  }
}
