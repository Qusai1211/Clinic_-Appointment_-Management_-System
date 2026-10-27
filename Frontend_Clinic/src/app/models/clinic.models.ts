export interface Specialty {
  specialtyId: number;
  specialtyName: string;
  description?: string;
  doctorCount?: number;
  appointmentCount?: number;
}

export interface DoctorWorkingHours {
  workingHoursId: number;
  dayOfWeek: string;
  startTime: string;
  endTime: string;
  doctorId: number;
}

export interface AppointmentSlot {
  slotId: number;
  slotDate: string;
  slotTime: string;
  isAvailable: number;
  doctorId: number;
  doctorName?: string;
}

export interface Doctor {
  doctorId: number;
  fullName: string;
  qualifications?: string;
  consultationFee: number;
  profileImageUrl?: string;
  isFeatured: number;
  specialtyId?: number;
  specialtyName?: string;
  workingHours?: DoctorWorkingHours[];
  availableSlots?: AppointmentSlot[];
}

export interface Patient {
  patientId: number;
  fullName: string;
  phoneNumber: string;
  email: string;
  dateOfBirth: string;
}

export interface Appointment {
  appointmentId: number;
  referenceNumber: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string; // 'Scheduled' | 'Completed' | 'Cancelled'
  doctorId?: number;
  doctorName?: string;
  specialtyName?: string;
  slotId?: number;
  patientId?: number;
  patientName?: string;
  patientPhone?: string;
  patientEmail?: string;
  consultationFee?: number;
}

export interface AdminDashboardStats {
  totalDoctors: number;
  totalPatients: number;
  totalAppointments: number;
  scheduledAppointments: number;
  completedAppointments: number;
  cancelledAppointments: number;
}

export interface SpecialtyAppointmentChartItem {
  specialtyId: number;
  specialtyName: string;
  appointmentCount: number;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  success: boolean;
  token?: string;
  message?: string;
  adminId?: number;
  adminName?: string;
  expiresAt?: string;
}

export interface RegisterPatientRequest {
  fullName: string;
  phoneNumber: string;
  email: string;
  dateOfBirth: string;
}

export interface BookAppointmentRequest {
  doctorId: number;
  patientId?: number;
  slotId: number;
  appointmentDate: string;
  appointmentTime: string;
  patientName?: string;
  patientPhone?: string;
  patientEmail?: string;
  patientDateOfBirth?: string;
}

export interface BookAppointmentResponse {
  success: boolean;
  message?: string;
  appointmentId?: number;
  referenceNumber?: string;
  patientName?: string;
  doctorName?: string;
  specialtyName?: string;
  appointmentDate?: string;
  appointmentTime?: string;
  status?: string;
  consultationFee?: number;
}

export interface DoctorUpsertDto {
  fullName: string;
  qualifications?: string;
  consultationFee: number;
  profileImageUrl?: string;
  isFeatured: number;
  specialtyId?: number;
}

export interface SpecialtyUpsertDto {
  specialtyName: string;
  description?: string;
}
