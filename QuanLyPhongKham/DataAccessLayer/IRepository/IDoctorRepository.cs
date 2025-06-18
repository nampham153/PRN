using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.models;

namespace DataAccessLayer.IRepository
{
    public interface IDoctorRepository
    {
        List<User> GetAllDoctors(string searchTerm = "");
        User GetDoctorByAccountId(int accountId);
        void CreateDoctor(User doctor);
        void UpdateDoctor(User doctor);
        void DeleteDoctor(User doctor);
    }


}
