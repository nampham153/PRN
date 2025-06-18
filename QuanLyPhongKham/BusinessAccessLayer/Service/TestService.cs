using BusinessAccessLayer.IService;
using DataAccessLayer.IRepository;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using DataAccessLayer.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Service
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;

        public TestService(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public List<Test> GetAllTests()
        {
            return _testRepository.GetAllTests();
        }

        public Test GetTestById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Test ID must be greater than 0");

            var test = _testRepository.GetTestById(id);
            if (test == null)
                throw new ArgumentException($"Test with ID {id} not found");

            return test;
        }

        public void CreateTest(TestVM testVM)
        {
            // Validation
            if (testVM == null)
                throw new ArgumentNullException(nameof(testVM));
            if (string.IsNullOrWhiteSpace(testVM.TestName))
                throw new ArgumentException("Test name is required");
            if (string.IsNullOrWhiteSpace(testVM.Description))
                throw new ArgumentException("Description is required");

            _testRepository.CreateTest(testVM);
        }

        public void UpdateTest(int id, TestVM testVM)
        {
            // Validation
            if (id <= 0)
                throw new ArgumentException("Test ID must be greater than 0");
            if (testVM == null)
                throw new ArgumentNullException(nameof(testVM));
            if (string.IsNullOrWhiteSpace(testVM.TestName))
                throw new ArgumentException("Test name is required");
            if (string.IsNullOrWhiteSpace(testVM.Description))
                throw new ArgumentException("Description is required");

            _testRepository.UpdateTest(id, testVM);
        }

        public void DeleteTest(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Test ID must be greater than 0");

            _testRepository.DeleteTest(id);
        }

        // Implement search method với validation
        public List<Test> SearchTests(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllTests();

            return _testRepository.SearchTests(searchTerm.Trim());
        }

        // Implement filter method với validation
        public PaginatedResult<Test> GetTestsWithFilter(SearchFilterVM filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            // Validate filter parameters
            if (filter.PageNumber <= 0)
                filter.PageNumber = 1;

            if (filter.PageSize <= 0 || filter.PageSize > 100)
                filter.PageSize = 10;

            var validSortColumns = new[] { "TestName", "Description", "TestId" };
            if (!validSortColumns.Contains(filter.SortBy, StringComparer.OrdinalIgnoreCase))
                filter.SortBy = "TestName";

            return _testRepository.GetTestsWithFilter(filter);
        }
    }
}
