using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface ITestResultRepository
    {
        List<TestResult> GetAllTestResults();
        TestResult GetTestResultById(int id);
        List<TestResult> GetTestResultsByRecordId(int recordId);
        List<TestResult> GetTestResultsByUserId(int userId);
        bool AddTestResult(TestResultVM testResultVM);
        bool UpdateTestResult(int id, TestResultVM testResultVM);
        bool DeleteTestResult(int id);
        bool TestResultExists(int id);
        TestResultVM GetTestResultVMById(int id);
        TestResultVM CreateNewTestResultVM();
        List<TestResultVM> GetAllTestResultVMs();

    }
}
