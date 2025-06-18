using DataAccessLayer.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.IService
{
    public interface IPatientService
    {
        List<DataAccessLayer.models.Patient> GetAllPatients();
        DataAccessLayer.models.Patient GetPatientById(int id);
        void AddPatient(DataAccessLayer.models.Patient patient);
        void UpdatePatient(DataAccessLayer.models.Patient patient);
        void DeletePatient(int id);
        List<DataAccessLayer.models.Patient> SearchPatients(string fullName, string phone, string email, string address, string gender, DateTime? dobFrom, DateTime? dobTo);
    }
}
