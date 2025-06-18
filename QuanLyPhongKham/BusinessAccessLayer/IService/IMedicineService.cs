using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.IService
{
    public interface IMedicineService
    {
        List<Medicine> GetAllMedicines();
        Medicine GetMedicineById(int id);
        Medicine CreateMedicine(MedicineVM medicineVM);
        Medicine UpdateMedicine(int id, MedicineVM medicineVM);
        bool DeleteMedicine(int id);
        List<Medicine> SearchMedicines(string searchTerm);
        List<Medicine> GetMedicinesByName(string name);
        List<Medicine> GetMedicinesByUnit(string unit);
        int GetTotalMedicinesCount();
    }
}
