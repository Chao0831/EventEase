using EventEase.Models;

namespace EventEase.Services
{
    public class AttendanceTrackerService
    {
        private readonly List<Registration> _registrations = new();
        private readonly ILogger<AttendanceTrackerService> _logger;
        
        // 统计事件
        public event EventHandler<AttendanceChangedEventArgs>? AttendanceChanged;
        
        public AttendanceTrackerService(ILogger<AttendanceTrackerService> logger)
        {
            _logger = logger;
            InitializeSampleData();
        }
        
        private void InitializeSampleData()
        {
            // 添加一些示例注册数据
            _registrations.Add(new Registration
            {
                Id = 1,
                EventId = 1,
                UserId = 1,
                FullName = "John Doe",
                Email = "john@example.com",
                Phone = "123-456-7890",
                NumberOfAttendees = 2,
                RegistrationDate = DateTime.Now.AddDays(-5),
                Status = RegistrationStatus.Confirmed
            });
            
            _registrations.Add(new Registration
            {
                Id = 2,
                EventId = 1,
                UserId = 2,
                FullName = "Jane Smith",
                Email = "jane@example.com",
                Phone = "098-765-4321",
                NumberOfAttendees = 1,
                RegistrationDate = DateTime.Now.AddDays(-3),
                Status = RegistrationStatus.Confirmed
            });
        }
        
        // 获取活动的所有注册
        public List<Registration> GetRegistrationsForEvent(int eventId)
        {
            return _registrations.Where(r => r.EventId == eventId).ToList();
        }
        
        // 获取用户的所有注册
        public List<Registration> GetRegistrationsForUser(int userId)
        {
            return _registrations.Where(r => r.UserId == userId).ToList();
        }
        
        // 添加新注册
        public async Task<RegistrationResult> AddRegistration(Registration registration)
        {
            try
            {
                // 验证
                if (registration.NumberOfAttendees <= 0)
                {
                    return RegistrationResult.CreateFailure("Number of attendees must be greater than 0");
                }
                
                // 检查是否已经注册
                var existingRegistration = _registrations.FirstOrDefault(r => 
                    r.EventId == registration.EventId && 
                    r.Email == registration.Email);
                    
                if (existingRegistration != null)
                {
                    return RegistrationResult.CreateFailure("You have already registered for this event");
                }
                
                // 设置ID和日期
                registration.Id = _registrations.Any() ? _registrations.Max(r => r.Id) + 1 : 1;
                registration.RegistrationDate = DateTime.Now;
                registration.Status = RegistrationStatus.Confirmed;
                
                _registrations.Add(registration);
                
                // 触发事件
                OnAttendanceChanged(new AttendanceChangedEventArgs(
                    registration.EventId,
                    registration.NumberOfAttendees,
                    "Added"
                ));
                
                _logger.LogInformation("Registration added: {RegistrationId} for Event {EventId}", 
                    registration.Id, registration.EventId);
                
                return RegistrationResult.CreateSuccess(registration, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding registration");
                return RegistrationResult.CreateFailure($"An error occurred: {ex.Message}");
            }
        }
        
        // 更新注册状态
        public async Task<bool> UpdateRegistrationStatus(int registrationId, RegistrationStatus newStatus)
        {
            try
            {
                var registration = _registrations.FirstOrDefault(r => r.Id == registrationId);
                if (registration == null)
                    return false;
                    
                var oldStatus = registration.Status;
                registration.Status = newStatus;
                
                OnAttendanceChanged(new AttendanceChangedEventArgs(
                    registration.EventId,
                    0,
                    $"Status changed from {oldStatus} to {newStatus}"
                ));
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating registration status");
                return false;
            }
        }
        
        // 取消注册
        public async Task<bool> CancelRegistration(int registrationId)
        {
            var registration = _registrations.FirstOrDefault(r => r.Id == registrationId);
            if (registration == null)
                return false;
                
            registration.Status = RegistrationStatus.Cancelled;
            
            OnAttendanceChanged(new AttendanceChangedEventArgs(
                registration.EventId,
                -registration.NumberOfAttendees,
                "Cancelled"
            ));
            
            return true;
        }
        
        // 获取活动统计数据
        public EventAttendanceStats GetEventStats(int eventId)
        {
            var eventRegistrations = _registrations.Where(r => r.EventId == eventId).ToList();
            
            return new EventAttendanceStats
            {
                EventId = eventId,
                TotalRegistrations = eventRegistrations.Count,
                TotalAttendees = eventRegistrations.Sum(r => r.NumberOfAttendees),
                ConfirmedRegistrations = eventRegistrations.Count(r => r.Status == RegistrationStatus.Confirmed),
                CancelledRegistrations = eventRegistrations.Count(r => r.Status == RegistrationStatus.Cancelled),
                AttendedCount = eventRegistrations.Count(r => r.Status == RegistrationStatus.Attended),
                NoShowCount = eventRegistrations.Count(r => r.Status == RegistrationStatus.NoShow)
            };
        }
        
        private void OnAttendanceChanged(AttendanceChangedEventArgs e)
        {
            AttendanceChanged?.Invoke(this, e);
        }
    }
    
    // 注册结果类 - 修复名称冲突
    public class RegistrationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public Registration? Registration { get; set; }
        
        public static RegistrationResult CreateSuccess(Registration registration, string message = "Registration successful")
        {
            return new RegistrationResult
            {
                IsSuccess = true,
                Registration = registration,
                Message = message
            };
        }
        
        public static RegistrationResult CreateFailure(string message)
        {
            return new RegistrationResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
    
    // 事件参数
    public class AttendanceChangedEventArgs : EventArgs
    {
        public int EventId { get; }
        public int ChangeAmount { get; }
        public string ChangeType { get; }
        
        public AttendanceChangedEventArgs(int eventId, int changeAmount, string changeType)
        {
            EventId = eventId;
            ChangeAmount = changeAmount;
            ChangeType = changeType;
        }
    }
    
    // 活动统计
    public class EventAttendanceStats
    {
        public int EventId { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalAttendees { get; set; }
        public int ConfirmedRegistrations { get; set; }
        public int CancelledRegistrations { get; set; }
        public int AttendedCount { get; set; }
        public int NoShowCount { get; set; }
        public double AttendanceRate => TotalRegistrations > 0 
            ? (double)AttendedCount / TotalRegistrations * 100 
            : 0;
    }
}