export interface Product {
  productId: string;
  productName: string;
  description: string;
  price: number;
  isDeleted: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProduct {
  productName: string;
  description: string;
  price: number;
}

export interface UpdateProduct {
  productName: string;
  description: string;
  price: number;
}
