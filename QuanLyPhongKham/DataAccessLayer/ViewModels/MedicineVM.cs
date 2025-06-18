using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels
{
    public class MedicineVM
    {
        [Required(ErrorMessage = "Medicine name is required")]
        [StringLength(100, ErrorMessage = "Medicine name cannot exceed 100 characters")]
        public string MedicineName { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        [StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
        public string Unit { get; set; }

        [StringLength(500, ErrorMessage = "Usage cannot exceed 500 characters")]
        public string Usage { get; set; }
    }
}
