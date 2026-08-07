// phân quyền người dùng
export type UserRole = 'Patient' | 'Receptionist' | 'Doctor' | 'Admin'

export interface AuthUser {
  id: number
  name: string
  email: string
  role: UserRole
}

export interface LoginPayload {
  email: string
  password: string
  role: UserRole
}

export interface LoginResponse {
  accessToken: string
  refreshToken?: string
  user: AuthUser
}

export interface RegisterPayload {
  fullName: string
  phoneNumber: string
  email: string
  password: string
  gender: 'Male' | 'Female' | 'Other'
  dateOfBirth: string
}

export interface User {
  id: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  role: 'Patient' | 'Admin' | 'Receptionist' | 'Doctor';
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}