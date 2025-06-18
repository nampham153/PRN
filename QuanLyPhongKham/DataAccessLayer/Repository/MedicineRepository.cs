using DataAccessLayer.DAO;
using DataAccessLayer.IRepository;
using DataAccessLayer.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly MedicineDAO _medicineDAO;

        public MedicineRepository(MedicineDAO medicineDAO)
        {
            _medicineDAO = medicineDAO;
        }

        public List<Medicine> GetAllMedicines()
        {
            return _medicineDAO.GetAllMedicines();
        }

        public Medicine GetMedicineById(int id)
        {
            return _medicineDAO.GetMedicineById(id);
        }

        public Medicine AddMedicine(Medicine medicine)
        {
            return _medicineDAO.AddMedicine(medicine);
        }

        public Medicine UpdateMedicine(Medicine medicine)
        {
            return _medicineDAO.UpdateMedicine(medicine);
        }

        public bool DeleteMedicine(int id)
        {
            return _medicineDAO.DeleteMedicine(id);
        }

        public bool MedicineExists(int id)
        {
            return _medicineDAO.MedicineExists(id);
        }

        public List<Medicine> SearchMedicines(string searchTerm)
        {
            return _medicineDAO.SearchMedicines(searchTerm);
        }

        public List<Medicine> GetMedicinesByName(string name)
        {
            return _medicineDAO.GetMedicinesByName(name);
        }

        public List<Medicine> GetMedicinesByUnit(string unit)
        {
            return _medicineDAO.GetMedicinesByUnit(unit);
        }

        public int GetTotalMedicinesCount()
        {
            return _medicineDAO.GetTotalMedicinesCount();
        }
    }
}