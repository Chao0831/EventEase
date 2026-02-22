using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Registration
    {
        public int Id { get; set; }
        
        [Required]
        public int EventId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string Phone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Number of attendees is required")]
        [Range(1, 10, ErrorMessage = "Number of attendees must be between 1 and 10")]
        public int NumberOfAttendees { get; set; }
        
        [StringLength(500, ErrorMessage = "Special requests cannot exceed 500 characters")]
        public string? SpecialRequests { get; set; }
        
        public DateTime RegistrationDate { get; set; }
        
        public RegistrationStatus Status { get; set; }
        
        // 导航属性
        public Event? Event { get; set; }
        public User? User { get; set; }
    }
    
    public enum RegistrationStatus
    {
        Confirmed,
        Cancelled,
        Attended,
        NoShow
    }
}