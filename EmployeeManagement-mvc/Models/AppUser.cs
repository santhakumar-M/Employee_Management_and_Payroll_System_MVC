using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeHrSystem.Models
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        [MaxLength(50, ErrorMessage = "Role cannot exceed 50 characters.")]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        // Nullable FK — links a system user to an employee record
        public int? EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }
    }
}
