using DataAccessLayer.DAO;
using DataAccessLayer.dbcontext;
using DataAccessLayer.IRepository;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class TestResultRepository : ITestResultRepository
    {
        private readonly TestResultDAO _testResultDAO;
        private readonly ClinicDbContext _context;

        public TestResultRepository(ClinicDbContext context)
        {
            _context = context;
            _testResultDAO = new TestResultDAO(context);
        }

        public List<TestResult> GetAllTestResults()
        {
            return _testResultDAO.GetAllTestResults();
        }

        public TestResult GetTestResultById(int id)
        {
            return _testResultDAO.GetTestResultById(id);
        }

        public List<TestResult> GetTestResultsByRecordId(int recordId)
        {
            return _testResultDAO.GetTestResultsByRecordId(recordId);
        }

        public List<TestResult> GetTestResultsByUserId(int userId)
        {
            return _testResultDAO.GetTestResultsByTechnicianId(userId);
        }

        public bool AddTestResult(TestResultVM testResultVM)
        {
            var testResult = new TestResult
            {
                RecordId = testResultVM.RecordId,
                TestId = testResultVM.TestId,
                UserId = testResultVM.UserId,
                ResultDetail = testResultVM.ResultDetail,
                TestDate = testResultVM.TestDate
            };
            return _testResultDAO.AddTestResult(testResult);
        }

        public bool UpdateTestResult(int id, TestResultVM testResultVM)
        {
            try
            {
                var existingResult = _context.TestResults.Find(id);
                if (existingResult == null)
                    return false;

                // Validate foreign key relationships
                if (!_context.MedicalRecords.Any(mr => mr.RecordId == testResultVM.RecordId))
                    throw new ArgumentException("Invalid Medical Record ID");

                if (!_context.Tests.Any(t => t.TestId == testResultVM.TestId))
                    throw new ArgumentException("Invalid Test ID");

                if (!_context.Users.Any(u => u.UserId == testResultVM.UserId))
                    throw new ArgumentException("Invalid User ID");

                // Update properties
                existingResult.RecordId = testResultVM.RecordId;
                existingResult.TestId = testResultVM.TestId;
                existingResult.UserId = testResultVM.UserId;
                existingResult.ResultDetail = testResultVM.ResultDetail;
                existingResult.TestDate = testResultVM.TestDate;

                return _testResultDAO.UpdateTestResult(existingResult);
            }
            catch (Exception ex)
            {
                // Log exception here
                return false;
            }
        }

        public bool DeleteTestResult(int id)
        {
            return _testResultDAO.DeleteTestResult(id);
        }

        public bool TestResultExists(int id)
        {
            return _testResultDAO.TestResultExists(id);
        }

        public TestResultVM GetTestResultVMById(int id)
        {
            var testResult = _testResultDAO.GetTestResultById(id);
            if (testResult == null) return null;

            return new TestResultVM
            {
                RecordId = testResult.RecordId,
                TestId = testResult.TestId,
                UserId = testResult.UserId,
                ResultDetail = testResult.ResultDetail,
                TestDate = testResult.TestDate,
                TestName = testResult.Test?.TestName,
                TestDescription = testResult.Test?.Description,
                UserName = testResult.User?.FullName,
                PatientName = testResult.MedicalRecord?.Patient?.FullName,
                MedicalRecordDate = testResult.MedicalRecord?.Date.ToString("dd/MM/yyyy"),
                Diagnosis = testResult.MedicalRecord?.Diagnosis
            };
        }

        public TestResultVM CreateNewTestResultVM()
        {
            return new TestResultVM
            {
                TestDate = DateTime.Now
            };
        }

        // Additional method to get TestResultVM with full entity data for display
        public List<TestResultVM> GetAllTestResultVMs()
        {
            var testResults = GetAllTestResults();
            return testResults.Select(tr => new TestResultVM
            {
                RecordId = tr.RecordId,
                TestId = tr.TestId,
                UserId = tr.UserId,
                ResultDetail = tr.ResultDetail,
                TestDate = tr.TestDate,
                TestName = tr.Test?.TestName,
                TestDescription = tr.Test?.Description,
                UserName = tr.User?.FullName,
                PatientName = tr.MedicalRecord?.Patient?.FullName,
                MedicalRecordDate = tr.MedicalRecord?.Date.ToString("dd/MM/yyyy"),
                Diagnosis = tr.MedicalRecord?.Diagnosis
            }).ToList();
        }
    }
}