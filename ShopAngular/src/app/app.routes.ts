import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'users', pathMatch: 'full' },
  {
    path: 'users',
    loadComponent: () => import('./pages/users/users').then(m => m.UsersPage),
  },
  {
    path: 'products',
    loadComponent: () => import('./pages/products/products').then(m => m.ProductsPage),
  },
  {
    path: 'orders',
    loadComponent: () => import('./pages/orders/orders').then(m => m.OrdersPage),
  },
];
