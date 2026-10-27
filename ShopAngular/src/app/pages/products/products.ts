import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { TableModule } from 'primeng/table';
import { Dialog } from 'primeng/dialog';
import { Button } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { InputNumber } from 'primeng/inputnumber';
import { Toast } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ProductService } from '../../services/product.service';
import { Product, CreateProduct, UpdateProduct } from '../../models/product.model';

@Component({
  selector: 'app-products',
  imports: [FormsModule, DatePipe, CurrencyPipe, TableModule, Dialog, Button, InputText, InputNumber, Toast],
  providers: [MessageService],
  templateUrl: './products.html',
  styleUrl: './products.scss',
})
export class ProductsPage implements OnInit {
  private productService = inject(ProductService);
  private messageService = inject(MessageService);

  products = signal<Product[]>([]);
  loading = signal(false);

  showCreate = false;
  showEdit = false;
  showDelete = false;
  confirmPending = false;

  createForm: CreateProduct = this.emptyCreate();
  editForm: Product = this.emptyProduct();
  deleteTarget: Product | null = null;

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.productService.getAll().subscribe({
      next: products => { this.products.set(products); this.loading.set(false); },
      error: () => { this.loading.set(false); },
    });
  }

  // ── Create ──────────────────────────────────────────────────
  openCreate() {
    this.createForm = this.emptyCreate();
    this.showCreate = true;
  }

  submitCreate() {
    this.productService.create(this.createForm).subscribe({
      next: () => {
        this.showCreate = false;
        this.load();
        this.messageService.add({ severity: 'success', summary: 'Created', detail: 'Product created successfully.' });
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to create product.' });
      },
    });
  }

  // ── Edit ────────────────────────────────────────────────────
  openEdit(product: Product) {
    this.editForm = { ...product };
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
    const dto: UpdateProduct = {
      productName: this.editForm.productName,
      description: this.editForm.description,
      price:       this.editForm.price,
    };
    this.productService.update(this.editForm.productId, dto).subscribe({
      next: () => {
        this.closeEdit();
        this.load();
        this.messageService.add({ severity: 'success', summary: 'Updated', detail: 'Product updated successfully.' });
      },
      error: () => {
        this.confirmPending = false;
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update product.' });
      },
    });
  }

  // ── Delete ──────────────────────────────────────────────────
  openDelete(product: Product) {
    this.deleteTarget = product;
    this.showDelete = true;
  }

  confirmDelete() {
    if (!this.deleteTarget) return;
    this.productService.delete(this.deleteTarget.productId).subscribe({
      next: () => {
        this.showDelete = false;
        this.deleteTarget = null;
        this.load();
        this.messageService.add({ severity: 'warn', summary: 'Deleted', detail: 'Product deleted.' });
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete product.' });
      },
    });
  }

  // ── Helpers ─────────────────────────────────────────────────
  private emptyCreate(): CreateProduct {
    return { productName: '', description: '', price: 0 };
  }

  private emptyProduct(): Product {
    return { productId: '', productName: '', description: '', price: 0, isDeleted: false, createdAt: '', updatedAt: '' };
  }
}
