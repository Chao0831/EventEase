using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;
        
        public DateTime LastLoginTime { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public List<Registration> Registrations { get; set; } = new();
    }
}