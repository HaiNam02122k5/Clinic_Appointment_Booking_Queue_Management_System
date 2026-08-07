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