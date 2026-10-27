export interface Order {
  orderId: string;
  userId: string;
  productId: string;
  totalPaid: number;
  isDeleted: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateOrder {
  userId: string;
  productId: string;
  totalPaid: number;
}

export interface UpdateOrder {
  userId: string;
  productId: string;
  totalPaid: number;
}
