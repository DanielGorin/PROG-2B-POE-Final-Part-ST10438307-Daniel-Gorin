using System.ComponentModel.DataAnnotations;

using System;
using System.ComponentModel.DataAnnotations;

namespace PROG_2B_POE_Final_Part_ST10438307_Daniel_Gorin.Models
{
    public class Claims
    {
        [Key]
        public int ClaimId { get; set; }

        [Required]
        public string ClaimantName { get; set; }

        [Required]
        public decimal HourlyRate { get; set; }

        [Required]
        public int HoursWorked { get; set; }

        [Required]
        public string ClaimantComments { get; set; }

        [Required]
        public DateTime DateLogged { get; set; }

        public string UploadedFiles { get; set; } = "file";//sets a default value

        [Required]
        public string Status { get; set; }
    }
}
