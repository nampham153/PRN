using DataAccessLayer.dbcontext;
using DataAccessLayer.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAO
{
    public class TestResultDAO
    {
        private readonly ClinicDbContext _context;

        public TestResultDAO(ClinicDbContext context)
        {
            _context = context;
        }

        public List<TestResult> GetAllTestResults()
        {
            try
            {
                return _context.TestResults
                    .Include(tr => tr.Test)
                    .Include(tr => tr.User)
                    .Include(tr => tr.MedicalRecord)
                        .ThenInclude(mr => mr.Patient)
                    .OrderByDescending(tr => tr.TestDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception("Error retrieving test results", ex);
            }
        }

        public TestResult GetTestResultById(int id)
        {
            try
            {
                return _context.TestResults
                    .Include(tr => tr.Test)
                    .Include(tr => tr.User)
                    .Include(tr => tr.MedicalRecord)
                        .ThenInclude(mr => mr.Patient)
                    .FirstOrDefault(tr => tr.ResultId == id);
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception($"Error retrieving test result with ID {id}", ex);
            }
        }

        public List<TestResult> GetTestResultsByRecordId(int recordId)
        {
            try
            {
                return _context.TestResults
                    .Include(tr => tr.Test)
                    .Include(tr => tr.User)
                    .Where(tr => tr.RecordId == recordId)
                    .OrderByDescending(tr => tr.TestDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception($"Error retrieving test results for record ID {recordId}", ex);
            }
        }

        public List<TestResult> GetTestResultsByTechnicianId(int technicianId)
        {
            try
            {
                return _context.TestResults
                    .Include(tr => tr.Test)
                    .Include(tr => tr.MedicalRecord)
                        .ThenInclude(mr => mr.Patient)
                    .Where(tr => tr.UserId == technicianId)
                    .OrderByDescending(tr => tr.TestDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception($"Error retrieving test results for technician ID {technicianId}", ex);
            }
        }

        public bool AddTestResult(TestResult testResult)
        {
            try
            {
                // Validate foreign key relationships
                if (!_context.MedicalRecords.Any(mr => mr.RecordId == testResult.RecordId))
                    throw new ArgumentException("Invalid Medical Record ID");

                if (!_context.Tests.Any(t => t.TestId == testResult.TestId))
                    throw new ArgumentException("Invalid Test ID");

                if (!_context.Users.Any(u => u.UserId == testResult.UserId))
                    throw new ArgumentException("Invalid Technician ID");

                _context.TestResults.Add(testResult);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception here
                return false;
            }
        }

        public bool UpdateTestResult(TestResult testResult)
        {
            try
            {
                var existingResult = _context.TestResults.Find(testResult.ResultId);
                if (existingResult == null)
                    return false;

                // Validate foreign key relationships
                if (!_context.MedicalRecords.Any(mr => mr.RecordId == testResult.RecordId))
                    throw new ArgumentException("Invalid Medical Record ID");

                if (!_context.Tests.Any(t => t.TestId == testResult.TestId))
                    throw new ArgumentException("Invalid Test ID");

                if (!_context.Users.Any(u => u.UserId == testResult.UserId))
                    throw new ArgumentException("Invalid Technician ID");

                // Update properties
                existingResult.RecordId = testResult.RecordId;
                existingResult.TestId = testResult.TestId;
                existingResult.UserId = testResult.UserId;
                existingResult.ResultDetail = testResult.ResultDetail;
                existingResult.TestDate = testResult.TestDate;

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception here
                return false;
            }
        }

        public bool DeleteTestResult(int id)
        {
            try
            {
                var testResult = _context.TestResults.Find(id);
                if (testResult != null)
                {
                    _context.TestResults.Remove(testResult);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log exception here
                return false;
            }
        }

        public bool TestResultExists(int id)
        {
            try
            {
                return _context.TestResults.Any(tr => tr.ResultId == id);
            }
            catch (Exception ex)
            {
                // Log exception here
                return false;
            }
        }
    }
}