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
    public class TestResultService : ITestResultService
    {
        private readonly ITestResultRepository _testResultRepository;

        public TestResultService(ITestResultRepository testResultRepository)
        {
            _testResultRepository = testResultRepository;
        }

        public List<TestResult> GetAllTestResults()
        {
            return _testResultRepository.GetAllTestResults();
        }

        public List<TestResultVM> GetAllTestResultVMs()
        {
            return _testResultRepository.GetAllTestResultVMs();
        }

        public TestResult GetTestResultById(int id)
        {
            if (id <= 0) return null;
            return _testResultRepository.GetTestResultById(id);
        }

        public List<TestResult> GetTestResultsByRecordId(int recordId)
        {
            if (recordId <= 0) return new List<TestResult>();
            return _testResultRepository.GetTestResultsByRecordId(recordId);
        }

        public List<TestResult> GetTestResultsByUserId(int userId)
        {
            if (userId <= 0) return new List<TestResult>();
            return _testResultRepository.GetTestResultsByUserId(userId);
        }

        public bool CreateTestResult(TestResultVM testResultVM)
        {
            if (!ValidateTestResult(testResultVM, out List<string> errors))
                return false;

            return _testResultRepository.AddTestResult(testResultVM);
        }

        public bool UpdateTestResult(int id, TestResultVM testResultVM)
        {
            if (id <= 0) return false;

            if (!ValidateTestResult(testResultVM, out List<string> errors))
                return false;

            if (!_testResultRepository.TestResultExists(id))
                return false;

            return _testResultRepository.UpdateTestResult(id, testResultVM);
        }

        public bool DeleteTestResult(int id)
        {
            if (id <= 0) return false;

            if (!_testResultRepository.TestResultExists(id))
                return false;

            return _testResultRepository.DeleteTestResult(id);
        }

        public bool TestResultExists(int id)
        {
            return _testResultRepository.TestResultExists(id);
        }

        public TestResultVM GetTestResultForEdit(int id)
        {
            if (id <= 0) return null;
            return _testResultRepository.GetTestResultVMById(id);
        }

        public TestResultVM GetNewTestResultVM()
        {
            return _testResultRepository.CreateNewTestResultVM();
        }

        public bool ValidateTestResult(TestResultVM testResultVM, out List<string> errors)
        {
            errors = new List<string>();

            if (testResultVM == null)
            {
                errors.Add("Test result data is required");
                return false;
            }

            if (testResultVM.RecordId <= 0)
                errors.Add("Valid Medical Record is required");

            if (testResultVM.TestId <= 0)
                errors.Add("Valid Test is required");

            if (testResultVM.UserId <= 0)
                errors.Add("Valid User is required");

            if (string.IsNullOrWhiteSpace(testResultVM.ResultDetail))
                errors.Add("Result Detail is required");
            else if (testResultVM.ResultDetail.Length > 1000)
                errors.Add("Result Detail cannot exceed 1000 characters");

            if (testResultVM.TestDate == default(DateTime))
                errors.Add("Valid Test Date is required");
            else if (testResultVM.TestDate > DateTime.Now)
                errors.Add("Test Date cannot be in the future");

            return errors.Count == 0;
        }
    }
}