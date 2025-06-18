using DataAccessLayer.dbcontext;
using DataAccessLayer.IRepository;
using DataAccessLayer.models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ClinicDbContext _context;

        public PatientRepository(ClinicDbContext context)
        {
            _context = context;
        }

        public List<Patient> GetAllPatients()
        {
            return _context.Patients.ToList();
        }

        public Patient GetPatientById(int id)
        {
            return _context.Patients.Find(id);
        }

        public void AddPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        public void UpdatePatient(Patient patient)
        {
            _context.Patients.Update(patient);
            _context.SaveChanges();
        }

        public void DeletePatient(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                _context.SaveChanges();
            }
        }

        public bool IsPhoneExists(string phone, int? excludePatientId = null)
        {
            return _context.Patients
                .Any(p => p.Phone == phone && (excludePatientId == null || p.PatientId != excludePatientId));
        }

        public List<Patient> SearchPatients(string fullName, string phone, string email, string address, string gender, DateTime? dobFrom, DateTime? dobTo)
        {
            var query = _context.Patients.AsQueryable();

            if (!string.IsNullOrEmpty(fullName))
            {
                query = query.Where(p => p.FullName.Contains(fullName));
            }

            if (!string.IsNullOrEmpty(phone))
            {
                query = query.Where(p => p.Phone.Contains(phone));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.Email != null && p.Email.Contains(email));
            }

            if (!string.IsNullOrEmpty(address))
            {
                query = query.Where(p => p.Address != null && p.Address.Contains(address));
            }

            if (!string.IsNullOrEmpty(gender))
            {
                query = query.Where(p => p.Gender == gender);
            }

            if (dobFrom.HasValue)
            {
                query = query.Where(p => p.DOB >= dobFrom.Value);
            }

            if (dobTo.HasValue)
            {
                query = query.Where(p => p.DOB <= dobTo.Value);
            }

            return query.ToList();
        }
        public bool IsPhoneExists(string phone, int excludePatientId)
        {
            try
            {
                if (string.IsNullOrEmpty(phone))
                    return false;

                return _context.Patients
                    .Any(p => p.Phone == phone && p.PatientId != excludePatientId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra số điện thoại: {ex.Message}");
            }
        }
    }
}