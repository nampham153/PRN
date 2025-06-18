using DataAccessLayer.dbcontext;
using DataAccessLayer.models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DAO 
{ 
    public class TestDAO
    {
        private readonly ClinicDbContext _context;

        public TestDAO(ClinicDbContext context)
        {
            _context = context;
        }

        public List<Test> GetTests()
        {
            return _context.Tests.Include(t => t.TestResults).ToList();
        }

        // Thêm method search tests
        public List<Test> SearchTests(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetTests();

            return _context.Tests
                .Include(t => t.TestResults)
                .Where(t => t.TestName.Contains(searchTerm) || 
                           t.Description.Contains(searchTerm))
                .ToList();
        }

        // Thêm method filter tests với pagination
        public List<Test> GetTestsWithFilter(string searchTerm = null, 
                                           string sortBy = "TestName", 
                                           bool sortDescending = false,
                                           int pageNumber = 1, 
                                           int pageSize = 10)
        {
            var query = _context.Tests.Include(t => t.TestResults).AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t => t.TestName.Contains(searchTerm) || 
                                        t.Description.Contains(searchTerm));
            }

            // Apply sorting
            switch (sortBy.ToLower())
            {
                case "testname":
                    query = sortDescending ? query.OrderByDescending(t => t.TestName) 
                                           : query.OrderBy(t => t.TestName);
                    break;
                case "description":
                    query = sortDescending ? query.OrderByDescending(t => t.Description) 
                                           : query.OrderBy(t => t.Description);
                    break;
                case "testid":
                    query = sortDescending ? query.OrderByDescending(t => t.TestId) 
                                           : query.OrderBy(t => t.TestId);
                    break;
                default:
                    query = query.OrderBy(t => t.TestName);
                    break;
            }

            // Apply pagination
            return query.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }

        // Thêm method đếm total records cho pagination
        public int GetTestsCount(string searchTerm = null)
        {
            var query = _context.Tests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t => t.TestName.Contains(searchTerm) || 
                                        t.Description.Contains(searchTerm));
            }

            return query.Count();
        }

        public void SaveTest(Test t)
        {
            _context.Tests.Add(t);
            _context.SaveChanges();
        }

        public void UpdateTest(Test t)
        {
            _context.Entry(t).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteTest(Test t)
        {
            var t1 = _context.Tests.SingleOrDefault(c => c.TestId == t.TestId);
            if (t1 != null)
            {
                _context.Tests.Remove(t1);
                _context.SaveChanges();
            }
        }

        public Test GetTestById(int id)
        {
            return _context.Tests.Include(t => t.TestResults).FirstOrDefault(c => c.TestId == id);
        }
    }
}