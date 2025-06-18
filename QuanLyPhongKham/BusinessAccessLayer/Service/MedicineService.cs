using BusinessAccessLayer.IService;
using DataAccessLayer.IRepository;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Service
{
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _medicineRepository;

        public MedicineService(IMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public List<Medicine> GetAllMedicines()
        {
            return _medicineRepository.GetAllMedicines()
                .OrderBy(m => m.MedicineName)
                .ToList();
        }

        public Medicine GetMedicineById(int id)
        {
            return _medicineRepository.GetMedicineById(id);
        }

        public Medicine CreateMedicine(MedicineVM medicineVM)
        {
            var medicine = new Medicine
            {
                MedicineName = medicineVM.MedicineName?.Trim(),
                Unit = medicineVM.Unit?.Trim(),
                Usage = medicineVM.Usage?.Trim()
            };

            return _medicineRepository.AddMedicine(medicine);
        }

        public Medicine UpdateMedicine(int id, MedicineVM medicineVM)
        {
            var existingMedicine = _medicineRepository.GetMedicineById(id);
            if (existingMedicine == null)
                return null;

            existingMedicine.MedicineName = medicineVM.MedicineName?.Trim();
            existingMedicine.Unit = medicineVM.Unit?.Trim();
            existingMedicine.Usage = medicineVM.Usage?.Trim();

            return _medicineRepository.UpdateMedicine(existingMedicine);
        }

        public bool DeleteMedicine(int id)
        {
            return _medicineRepository.DeleteMedicine(id);
        }

        public List<Medicine> SearchMedicines(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllMedicines();

            return _medicineRepository.SearchMedicines(searchTerm.Trim())
                .OrderBy(m => m.MedicineName)
                .ToList();
        }

        public List<Medicine> GetMedicinesByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Medicine>();

            return _medicineRepository.GetMedicinesByName(name.Trim());
        }

        public List<Medicine> GetMedicinesByUnit(string unit)
        {
            if (string.IsNullOrWhiteSpace(unit))
                return new List<Medicine>();

            return _medicineRepository.GetMedicinesByUnit(unit.Trim());
        }

        public int GetTotalMedicinesCount()
        {
            return _medicineRepository.GetTotalMedicinesCount();
        }
    }
}