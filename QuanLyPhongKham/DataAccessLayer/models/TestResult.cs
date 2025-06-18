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
    public class TestResult
    {
        [Key]
        public int ResultId { get; set; }

        public int RecordId { get; set; }

        [JsonIgnore]
        [ForeignKey("RecordId")]
        public MedicalRecord MedicalRecord { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; }
       
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }
        public string ResultDetail { get; set; }
        public DateTime TestDate { get; set; }
    }
}
