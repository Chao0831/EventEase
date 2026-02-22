using EventEase.Models;

namespace EventEase.Services
{
    public class EventService
    {
        private List<Event> _events;
        private readonly ILogger<EventService> _logger;
        
        public EventService(ILogger<EventService> logger)
        {
            _logger = logger;
            _events = InitializeEvents();
        }
        
        private List<Event> InitializeEvents()
        {
            return new List<Event>
            {
                new Event
                {
                    Id = 1,
                    Name = "Tech Conference 2024",
                    Date = new DateTime(2024, 12, 15), // 未来日期
                    Location = "Convention Center, New York",
                    Description = "Annual technology conference featuring latest innovations in AI, Cloud Computing, and Software Development. Network with industry leaders and attend hands-on workshops.",
                    MaxAttendees = 500,
                    CurrentAttendees = 150,
                    Price = 299.99m,
                    Status = EventStatus.Active
                },
                new Event
                {
                    Id = 2,
                    Name = "Corporate Networking Mixer",
                    Date = new DateTime(2024, 11, 20), // 未来日期
                    Location = "City View Hotel, Los Angeles",
                    Description = "Networking event for business professionals. Connect with potential partners, clients, and mentors in a relaxed atmosphere.",
                    MaxAttendees = 200,
                    CurrentAttendees = 180,
                    Price = 49.99m,
                    Status = EventStatus.Active
                },
                new Event
                {
                    Id = 3,
                    Name = "Summer Gala",
                    Date = new DateTime(2025, 6, 10), // 明年
                    Location = "Beach Resort, Miami",
                    Description = "Annual summer fundraising gala supporting local charities. Enjoy gourmet dining, live entertainment, and silent auction.",
                    MaxAttendees = 300,
                    CurrentAttendees = 45,
                    Price = 149.99m,
                    Status = EventStatus.Active
                }
            };
        }
        
        public List<Event> GetEvents()
        {
            return _events;
        }
        
        public Event? GetEventById(int id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }
        
        // 更新活动参与人数
        public async Task<bool> UpdateEventAttendance(int eventId, int changeAmount)
        {
            try
            {
                var eventItem = GetEventById(eventId);
                if (eventItem == null)
                    return false;
                    
                var newAttendees = eventItem.CurrentAttendees + changeAmount;
                if (newAttendees < 0 || newAttendees > eventItem.MaxAttendees)
                    return false;
                    
                eventItem.CurrentAttendees = newAttendees;
                _logger.LogInformation("Event {EventId} attendance updated: {NewAttendees}/{MaxAttendees}", 
                    eventId, newAttendees, eventItem.MaxAttendees);
                    
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event attendance");
                return false;
            }
        }
        
        // 获取热门活动
        public List<Event> GetPopularEvents(int count)
        {
            return _events
                .Where(e => e.Status == EventStatus.Active)
                .OrderByDescending(e => (double)e.CurrentAttendees / e.MaxAttendees)
                .Take(count)
                .ToList();
        }
        
        // 搜索活动
        public List<Event> SearchEvents(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _events;
                
            return _events.Where(e => 
                e.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }
    }
}