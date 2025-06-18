using DataAccessLayer.dbcontext;
using DataAccessLayer.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAO
{
    public class PatientDAO
    {
        private readonly ClinicDbContext _context;

        public PatientDAO(ClinicDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả bệnh nhân, bao gồm MedicalRecords liên quan
        public List<Patient> GetPatients()
        {
            return _context.Patients.Include(p => p.MedicalRecords).ToList();
        }

        // Lấy bệnh nhân theo ID, bao gồm MedicalRecords
        public Patient GetPatientById(int id)
        {
            return _context.Patients
                .Include(p => p.MedicalRecords)
                .FirstOrDefault(p => p.PatientId == id);
        }

        // Thêm bệnh nhân mới
        public void SavePatient(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        // Cập nhật thông tin bệnh nhân
        public void UpdatePatient(Patient patient)
        {
            _context.Entry(patient).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // Xóa bệnh nhân
        public void DeletePatient(Patient patient)
        {
            var p = _context.Patients.SingleOrDefault(c => c.PatientId == patient.PatientId);
            if (p != null)
            {
                _context.Patients.Remove(p);
                _context.SaveChanges();
            }
        }
    }
}
