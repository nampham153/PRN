using DataAccessLayer.dbcontext;
using DataAccessLayer.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAO
{
    public class MedicineDAO
    {
        private readonly ClinicDbContext _context;

        public MedicineDAO(ClinicDbContext context)
        {
            _context = context;
        }

        public List<Medicine> GetAllMedicines()
        {
            return _context.Medicines.ToList();
        }

        public Medicine GetMedicineById(int id)
        {
            return _context.Medicines.FirstOrDefault(m => m.MedicineId == id);
        }

        public Medicine AddMedicine(Medicine medicine)
        {
            _context.Medicines.Add(medicine);
            _context.SaveChanges();
            return medicine;
        }

        public Medicine UpdateMedicine(Medicine medicine)
        {
            _context.Medicines.Update(medicine);
            _context.SaveChanges();
            return medicine;
        }

        public bool DeleteMedicine(int id)
        {
            var medicine = _context.Medicines.FirstOrDefault(m => m.MedicineId == id);
            if (medicine == null)
                return false;

            _context.Medicines.Remove(medicine);
            _context.SaveChanges();
            return true;
        }

        public bool MedicineExists(int id)
        {
            return _context.Medicines.Any(m => m.MedicineId == id);
        }

        public List<Medicine> SearchMedicines(string searchTerm)
        {
            return _context.Medicines
                .Where(m => m.MedicineName.Contains(searchTerm) ||
                           m.Unit.Contains(searchTerm) ||
                           m.Usage.Contains(searchTerm))
                .ToList();
        }

        public List<Medicine> GetMedicinesByName(string name)
        {
            return _context.Medicines
                .Where(m => m.MedicineName.ToLower().Contains(name.ToLower()))
                .OrderBy(m => m.MedicineName)
                .ToList();
        }

        public List<Medicine> GetMedicinesByUnit(string unit)
        {
            return _context.Medicines
                .Where(m => m.Unit.ToLower() == unit.ToLower())
                .ToList();
        }

        public int GetTotalMedicinesCount()
        {
            return _context.Medicines.Count();
        }
    }
}