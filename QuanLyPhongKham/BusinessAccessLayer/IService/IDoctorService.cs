using DataAccessLayer.models;
using DataAccessLayer.ViewModels;

using System.Collections.Generic;
using DataAccessLayer.models;

namespace BusinessAccessLayer.IService
{
    public interface IDoctorService
    {
        List<User> GetAllDoctors(string searchTerm = "");
        User GetDoctorByAccountId(int accountId);
        void CreateDoctor(User doctor);
        void UpdateDoctor(int accountId, User doctor);
        void DeleteDoctor(int accountId);
    }
}

