using DataAccessLayer.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IMedicineRepository
    {
        List<Medicine> GetAllMedicines();
        Medicine GetMedicineById(int id);
        Medicine AddMedicine(Medicine medicine);
        Medicine UpdateMedicine(Medicine medicine);
        bool DeleteMedicine(int id);
        bool MedicineExists(int id);
        List<Medicine> SearchMedicines(string searchTerm);
        List<Medicine> GetMedicinesByName(string name);
        List<Medicine> GetMedicinesByUnit(string unit);
        int GetTotalMedicinesCount();
    }
}
