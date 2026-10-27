export interface User {
  userId: string;
  firstName: string;
  lastName: string;
  address: string;
  email: string;
  isDeleted: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUser {
  firstName: string;
  lastName: string;
  address: string;
  email: string;
}

export interface UpdateUser {
  firstName: string;
  lastName: string;
  address: string;
  email: string;
}
