import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Specialty,
  Doctor,
  DoctorWorkingHours,
  AppointmentSlot,
  Patient,
  Appointment,
  AdminDashboardStats,
  SpecialtyAppointmentChartItem,
  RegisterPatientRequest,
  BookAppointmentRequest,
  BookAppointmentResponse,
  DoctorUpsertDto,
  SpecialtyUpsertDto
} from '../models/clinic.models';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = 'http://localhost:5069/api';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    return headers;
  }

  // --- Specialties ---
  getSpecialties(): Observable<Specialty[]> {
    return this.http.get<Specialty[]>(`${this.baseUrl}/specialties`);
  }

  getSpecialty(id: number): Observable<Specialty> {
    return this.http.get<Specialty>(`${this.baseUrl}/specialties/${id}`);
  }

  createSpecialty(dto: SpecialtyUpsertDto): Observable<any> {
    return this.http.post(`${this.baseUrl}/specialties`, dto, { headers: this.getAuthHeaders() });
  }

  updateSpecialty(id: number, dto: SpecialtyUpsertDto): Observable<any> {
    return this.http.put(`${this.baseUrl}/specialties/${id}`, dto, { headers: this.getAuthHeaders() });
  }

  deleteSpecialty(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/specialties/${id}`, { headers: this.getAuthHeaders() });
  }

  // --- Doctors ---
  getDoctors(search?: string, specialtyId?: number, sortBy?: string): Observable<Doctor[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (specialtyId) params = params.set('specialtyId', specialtyId.toString());
    if (sortBy) params = params.set('sortBy', sortBy);
    return this.http.get<Doctor[]>(`${this.baseUrl}/doctors`, { params });
  }

  getFeaturedDoctors(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(`${this.baseUrl}/doctors/featured`);
  }

  getDoctor(id: number): Observable<Doctor> {
    return this.http.get<Doctor>(`${this.baseUrl}/doctors/${id}`);
  }

  createDoctor(dto: DoctorUpsertDto): Observable<any> {
    return this.http.post(`${this.baseUrl}/doctors`, dto, { headers: this.getAuthHeaders() });
  }

  updateDoctor(id: number, dto: DoctorUpsertDto): Observable<any> {
    return this.http.put(`${this.baseUrl}/doctors/${id}`, dto, { headers: this.getAuthHeaders() });
  }

  deleteDoctor(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/doctors/${id}`, { headers: this.getAuthHeaders() });
  }

  getDoctorWorkingHours(doctorId: number): Observable<DoctorWorkingHours[]> {
    return this.http.get<DoctorWorkingHours[]>(`${this.baseUrl}/doctors/${doctorId}/working-hours`);
  }

  addDoctorWorkingHours(doctorId: number, data: { dayOfWeek: string; startTime: string; endTime: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/doctors/${doctorId}/working-hours`, data, { headers: this.getAuthHeaders() });
  }

  deleteDoctorWorkingHours(workingHoursId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/doctors/working-hours/${workingHoursId}`, { headers: this.getAuthHeaders() });
  }

  updateDoctorWorkingHours(workingHoursId: number, data: { dayOfWeek: string; startTime: string; endTime: string }): Observable<any> {
    return this.http.put(`${this.baseUrl}/doctors/working-hours/${workingHoursId}`, data, { headers: this.getAuthHeaders() });
  }

  getDoctorSlots(doctorId: number, date?: string, availableOnly = false): Observable<AppointmentSlot[]> {
    let params = new HttpParams();
    if (date) params = params.set('date', date);
    if (availableOnly) params = params.set('availableOnly', 'true');
    return this.http.get<AppointmentSlot[]>(`${this.baseUrl}/doctors/${doctorId}/slots`, { params });
  }

  addDoctorSlot(doctorId: number, data: { slotDate: string; slotTime: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/doctors/${doctorId}/slots`, data, { headers: this.getAuthHeaders() });
  }

  updateDoctorSlotAvailability(slotId: number, isAvailable: boolean): Observable<any> {
    return this.http.put(`${this.baseUrl}/doctors/slots/${slotId}/availability`, { isAvailable }, { headers: this.getAuthHeaders() });
  }

  deleteDoctorSlot(slotId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/doctors/slots/${slotId}`, { headers: this.getAuthHeaders() });
  }

  // --- Patients ---
  getPatients(search?: string): Observable<Patient[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    return this.http.get<Patient[]>(`${this.baseUrl}/patients`, { params });
  }

  getPatient(id: number): Observable<Patient> {
    return this.http.get<Patient>(`${this.baseUrl}/patients/${id}`);
  }

  registerPatient(data: RegisterPatientRequest): Observable<{ success: boolean; message: string; patient?: Patient }> {
    return this.http.post<{ success: boolean; message: string; patient?: Patient }>(`${this.baseUrl}/patients/register`, data);
  }

  updatePatient(id: number, data: { fullName: string; phoneNumber: string; email: string; dateOfBirth: string }): Observable<any> {
    return this.http.put(`${this.baseUrl}/patients/${id}`, data, { headers: this.getAuthHeaders() });
  }

  deletePatient(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/patients/${id}`, { headers: this.getAuthHeaders() });
  }

  // --- Appointments ---
  getAppointments(search?: string, date?: string, status?: string): Observable<Appointment[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (date) params = params.set('date', date);
    if (status) params = params.set('status', status);
    return this.http.get<Appointment[]>(`${this.baseUrl}/appointments`, { params });
  }

  getAppointment(id: number): Observable<Appointment> {
    return this.http.get<Appointment>(`${this.baseUrl}/appointments/${id}`);
  }

  getAppointmentByReference(referenceNumber: string): Observable<Appointment> {
    return this.http.get<Appointment>(`${this.baseUrl}/appointments/reference/${referenceNumber}`);
  }

  bookAppointment(data: BookAppointmentRequest): Observable<BookAppointmentResponse> {
    return this.http.post<BookAppointmentResponse>(`${this.baseUrl}/appointments/book`, data);
  }

  cancelAppointment(id: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/appointments/${id}/cancel`, {}, { headers: this.getAuthHeaders() });
  }

  updateAppointmentStatus(id: number, status: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/appointments/${id}/status`, { status }, { headers: this.getAuthHeaders() });
  }

  // --- Dashboard ---
  getDashboardStats(): Observable<AdminDashboardStats> {
    return this.http.get<AdminDashboardStats>(`${this.baseUrl}/dashboard/stats`, { headers: this.getAuthHeaders() });
  }

  getSpecialtyChartData(): Observable<SpecialtyAppointmentChartItem[]> {
    return this.http.get<SpecialtyAppointmentChartItem[]>(`${this.baseUrl}/dashboard/specialty-chart`, { headers: this.getAuthHeaders() });
  }
}

