using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DataAccessLayer.models
{
    public class Test
    {
        [Key]
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public ICollection<TestResult> TestResults { get; set; }
    }
}
