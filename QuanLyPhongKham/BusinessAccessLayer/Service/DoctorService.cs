using System;
using System.Collections.Generic;
using System.Linq;
using BusinessAccessLayer.IService;
using DataAccessLayer.IRepository;
using DataAccessLayer.models;

namespace BusinessAccessLayer.Service
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public List<User> GetAllDoctors(string searchTerm = "")
        {
            var doctors = _doctorRepository.GetAllDoctors();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                doctors = doctors
                    .Where(d => d.FullName != null && d.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return doctors;
        }

        public User GetDoctorByAccountId(int accountId)
        {
            return _doctorRepository.GetDoctorByAccountId(accountId);
        }

        public void CreateDoctor(User doctor)
        {
            _doctorRepository.CreateDoctor(doctor);
        }

        public void UpdateDoctor(int accountId, User updatedDoctor)
        {
            var existingDoctor = _doctorRepository.GetDoctorByAccountId(accountId);
            if (existingDoctor == null) return;

            existingDoctor.FullName = updatedDoctor.FullName;
            existingDoctor.Gender = updatedDoctor.Gender;
            existingDoctor.DOB = updatedDoctor.DOB;
            existingDoctor.Phone = updatedDoctor.Phone;
            existingDoctor.Email = updatedDoctor.Email;

            _doctorRepository.UpdateDoctor(existingDoctor);
        }

        public void DeleteDoctor(int accountId)
        {
            var doctor = _doctorRepository.GetDoctorByAccountId(accountId);
            if (doctor == null) return;

            _doctorRepository.DeleteDoctor(doctor);
        }
    }
}
