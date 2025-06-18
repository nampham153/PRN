using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.IService
{
    public interface ITestResultService
    {
        List<TestResult> GetAllTestResults();
        List<TestResultVM> GetAllTestResultVMs();
        TestResult GetTestResultById(int id);
        List<TestResult> GetTestResultsByRecordId(int recordId);
        List<TestResult> GetTestResultsByUserId(int userId);
        bool CreateTestResult(TestResultVM testResultVM);
        bool UpdateTestResult(int id, TestResultVM testResultVM);
        bool DeleteTestResult(int id);
        bool TestResultExists(int id);
        TestResultVM GetTestResultForEdit(int id);
        TestResultVM GetNewTestResultVM();
        bool ValidateTestResult(TestResultVM testResultVM, out List<string> errors);
    }
}