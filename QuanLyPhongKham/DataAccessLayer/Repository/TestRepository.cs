
using DataAccessLayer.DAO;
using DataAccessLayer.IRepository;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using DataAccessLayer.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class TestRepository : ITestRepository
    {
        private readonly TestDAO _testDAO;

        public TestRepository(TestDAO testDAO)
        {
            _testDAO = testDAO;
        }

        public List<Test> GetAllTests()
        {
            return _testDAO.GetTests();
        }

        public Test GetTestById(int id)
        {
            return _testDAO.GetTestById(id);
        }

        public void CreateTest(TestVM testVM)
        {
            var test = new Test
            {
                TestName = testVM.TestName,
                Description = testVM.Description,
                TestResults = new List<TestResult>()
            };
            _testDAO.SaveTest(test);
        }

        public void UpdateTest(int id, TestVM testVM)
        {
            var existingTest = _testDAO.GetTestById(id);
            if (existingTest == null)
                throw new ArgumentException($"Test with ID {id} not found");

            existingTest.TestName = testVM.TestName;
            existingTest.Description = testVM.Description;
            _testDAO.UpdateTest(existingTest);
        }

        public void DeleteTest(int id)
        {
            var existingTest = _testDAO.GetTestById(id);
            if (existingTest == null)
                throw new ArgumentException($"Test with ID {id} not found");

            _testDAO.DeleteTest(existingTest);
        }

        // Implement search method
        public List<Test> SearchTests(string searchTerm)
        {
            return _testDAO.SearchTests(searchTerm);
        }

        // Implement filter method with pagination
        public PaginatedResult<Test> GetTestsWithFilter(SearchFilterVM filter)
        {
            var tests = _testDAO.GetTestsWithFilter(
                filter.SearchTerm,
                filter.SortBy,
                filter.SortDescending,
                filter.PageNumber,
                filter.PageSize
            );

            var totalRecords = _testDAO.GetTestsCount(filter.SearchTerm);

            return new PaginatedResult<Test>(tests, totalRecords, filter.PageNumber, filter.PageSize);
        }
    }
}
