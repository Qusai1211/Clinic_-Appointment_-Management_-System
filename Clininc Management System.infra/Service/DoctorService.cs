// تأكد من إضافة مساحة الأسماء الخاصة بالنماذج (Models) والمستودع (Repository)
using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Repository;
using Clininc_Management_System.Core.Service;
using Clininc_Management_System.infra.Repository;

namespace Clinic_Management_System.infra.Service
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }
    
    // باقي الدوال...


        // جلب جميع الأطباء
        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _doctorRepository.GetAllDoctorsAsync();
        }

        // إضافة طبيب جديد
        public async Task<bool> CreateDoctor(Doctor doctor)
        {
            // يمكنك إضافة قواعد التحقق (Validation) هنا قبل الحفظ
             await _doctorRepository.CreateDoctor(doctor);
            return true;
        }

        // تعديل بيانات طبيب
public async Task<bool> UpdateDoctor(Doctor doctor)
{
     await _doctorRepository.UpdateDoctor(doctor);
            return true;
        }        
           

        // حذف طبيب
        public async Task<bool> DeleteDoctor(int id)
        {
            
            await _doctorRepository.DeleteDoctor(id);
            return true;
        }
        // جلب طبيب معين بناءً على المعرف (ID)
        public async Task<Doctor?> GetDoctorById(int id)
        {
            return await _doctorRepository.GetDoctorById(id);
        }
    }
}