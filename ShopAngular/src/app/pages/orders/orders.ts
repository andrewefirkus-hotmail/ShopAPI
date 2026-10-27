import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { TableModule } from 'primeng/table';
import { Dialog } from 'primeng/dialog';
import { Button } from 'primeng/button';
import { InputNumber } from 'primeng/inputnumber';
import { Select } from 'primeng/select';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { OrderService } from '../../services/order.service';
import { UserService } from '../../services/user.service';
import { ProductService } from '../../services/product.service';
import { Order, CreateOrder, UpdateOrder } from '../../models/order.model';
import { User } from '../../models/user.model';
import { Product } from '../../models/product.model';

interface SelectOption { label: string; value: string; }

@Component({
  selector: 'app-orders',
  imports: [FormsModule, DatePipe, CurrencyPipe, TableModule, Dialog, Button, InputNumber, Select, Toast],
  providers: [MessageService],
  templateUrl: './orders.html',
  styleUrl: './orders.scss',
})
export class OrdersPage implements OnInit {
  private orderService   = inject(OrderService);
  private userService    = inject(UserService);
  private productService = inject(ProductService);
  private messageService = inject(MessageService);

  orders   = signal<Order[]>([]);
  users    = signal<User[]>([]);
  products = signal<Product[]>([]);
  loading  = signal(false);

  showCreate = false;
  showEdit = false;
  showDelete = false;
  confirmPending = false;

  createForm: CreateOrder = this.emptyCreate();
  editForm: Order = this.emptyOrder();
  deleteTarget: Order | null = null;

  userOptions = computed<SelectOption[]>(() =>
    this.users().map(u => ({ label: `${u.firstName} ${u.lastName}`, value: u.userId }))
  );

  productOptions = computed<SelectOption[]>(() =>
    this.products().map(p => ({ label: `${p.productName} (${this.formatPrice(p.price)})`, value: p.productId }))
  );

  ngOnInit() {
    this.loadOrders();
    this.userService.getAll().subscribe(u => this.users.set(u));
    this.productService.getAll().subscribe(p => this.products.set(p));
  }

  loadOrders() {
    this.loading.set(true);
    this.orderService.getAll().subscribe({
      next: orders => { this.orders.set(orders); this.loading.set(false); },
      error: () => { this.loading.set(false); },
    });
  }

  getUserName(userId: string): string {
    const u = this.users().find(u => u.userId === userId);
    return u ? `${u.firstName} ${u.lastName}` : userId.substring(0, 8) + '…';
  }

  getProductName(productId: string): string {
    const p = this.products().find(p => p.productId === productId);
    return p ? p.productName : productId.substring(0, 8) + '…';
  }

  // ── Create ──────────────────────────────────────────────────
  openCreate() {
    this.createForm = this.emptyCreate();
    this.showCreate = true;
  }

  submitCreate() {
    this.orderService.create(this.createForm).subscribe({
      next: () => {
        this.showCreate = false;
        this.loadOrders();
        this.messageService.add({ severity: 'success', summary: 'Created', detail: 'Order created successfully.' });
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to create order.' });
      },
    });
  }

  // ── Edit ────────────────────────────────────────────────────
  openEdit(order: Order) {
    this.editForm = { ...order };
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
    const dto: UpdateOrder = {
      userId:    this.editForm.userId,
      productId: this.editForm.productId,
      totalPaid: this.editForm.totalPaid,
    };
    this.orderService.update(this.editForm.orderId, dto).subscribe({
      next: () => {
        this.closeEdit();
        this.loadOrders();
        this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Order updated successfully.' });
      },
      error: () => {
        this.confirmPending = false;
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update order.' });
      },
    });
  }

  // ── Delete ──────────────────────────────────────────────────
  openDelete(order: Order) {
    this.deleteTarget = order;
    this.showDelete = true;
  }

  confirmDelete() {
    if (!this.deleteTarget) return;
    this.orderService.delete(this.deleteTarget.orderId).subscribe({
      next: () => {
        this.showDelete = false;
        this.deleteTarget = null;
        this.loadOrders();
        this.messageService.add({ severity: 'warn', summary: 'Deleted', detail: 'Order deleted.' });
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete order.' });
      },
    });
  }

  // ── Helpers ─────────────────────────────────────────────────
  private emptyCreate(): CreateOrder {
    return { userId: '', productId: '', totalPaid: 0 };
  }

  private emptyOrder(): Order {
    return { orderId: '', userId: '', productId: '', totalPaid: 0, isDeleted: false, createdAt: '', updatedAt: '' };
  }

  private formatPrice(price: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(price);
  }
}
