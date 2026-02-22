using EventEase.Models;

namespace EventEase.Services
{
    public class EventService
    {
        private List<Event> _events = new List<Event>
        {
            new Event
            {
                Id = 1,
                Name = "Tech Conference 2024",
                Date = new DateTime(2024, 6, 15),
                Location = "Convention Center, New York",
                Description = "Annual technology conference featuring latest innovations",
                MaxAttendees = 500,
                Price = 299.99m
            },
            new Event
            {
                Id = 2,
                Name = "Corporate Networking Mixer",
                Date = new DateTime(2024, 7, 20),
                Location = "City View Hotel, Los Angeles",
                Description = "Networking event for business professionals",
                MaxAttendees = 200,
                Price = 49.99m
            },
            new Event
            {
                Id = 3,
                Name = "Summer Gala",
                Date = new DateTime(2024, 8, 10),
                Location = "Beach Resort, Miami",
                Description = "Annual summer fundraising gala",
                MaxAttendees = 300,
                Price = 149.99m
            }
        };

        public List<Event> GetEvents()
        {
            return _events;
        }

        public Event? GetEventById(int id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }
    }
}