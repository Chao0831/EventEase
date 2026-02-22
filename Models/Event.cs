using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Event
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Event name must be between 3 and 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Event date is required")]
        [FutureDate(ErrorMessage = "Event date must be in the future")]
        public DateTime Date { get; set; }
        
        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Maximum attendees is required")]
        [Range(1, 1000, ErrorMessage = "Maximum attendees must be between 1 and 1000")]
        public int MaxAttendees { get; set; }
        
        [Required(ErrorMessage = "Price is required")]
        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        
        // 跟踪当前已注册人数
        public int CurrentAttendees { get; set; }
        
        // 活动状态
        public EventStatus Status { get; set; }
        
        // 是否可注册
        public bool CanRegister => CurrentAttendees < MaxAttendees && Status == EventStatus.Active;
        
        // 剩余名额
        public int AvailableSpots => MaxAttendees - CurrentAttendees;
    }
    
    public enum EventStatus
    {
        Active,
        Cancelled,
        Completed,
        Postponed
    }
    
    // 自定义验证属性 - 确保日期在未来
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is DateTime date && date > DateTime.Now;
        }
    }
}