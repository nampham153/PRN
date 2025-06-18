using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using DataAccessLayer.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface ITestRepository
    {
        List<Test> GetAllTests();
        Test GetTestById(int id);
        void CreateTest(TestVM testVM);
        void UpdateTest(int id, TestVM testVM);
        void DeleteTest(int id);

        // Thêm methods mới cho search và filter
        List<Test> SearchTests(string searchTerm);
        PaginatedResult<Test> GetTestsWithFilter(SearchFilterVM filter);
    }
}
