using Assignment1.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context)
    {
        // Create the database and tables if they do not exist.
        await context.Database.EnsureCreatedAsync();

        // Avoid inserting duplicate seed data.
        if (await context.Events.AnyAsync())
        {
            return;
        }

        var events = new List<Event>
        {
            new Event
            {
                Title = "Career Fair",
                Description = "Meet employers and explore career opportunities.",
                Date = new DateTime(2026, 12, 1),
                Location = "Gym",
                Attendees = new List<Attendee>
                {
                    new Attendee
                    {
                        Name = "John Smith",
                        Email = "john@example.com"
                    },
                    new Attendee
                    {
                        Name = "Jane Doe",
                        Email = "jane@example.com"
                    }
                }
            },

            new Event
            {
                Title = "Tech Talk",
                Description = "Learn about current trends in technology.",
                Date = new DateTime(2026, 12, 8),
                Location = "Auditorium",
                Attendees = new List<Attendee>
                {
                    new Attendee
                    {
                        Name = "Alice Brown",
                        Email = "alice@example.com"
                    },
                    new Attendee
                    {
                        Name = "Bob Wilson",
                        Email = "bob@example.com"
                    }
                }
            },

            new Event
            {
                Title = "Hack Night",
                Description = "Collaborate on programming projects.",
                Date = new DateTime(2026, 12, 15),
                Location = "Library",
                Attendees = new List<Attendee>
                {
                    new Attendee
                    {
                        Name = "Charlie Davis",
                        Email = "charlie@example.com"
                    },
                    new Attendee
                    {
                        Name = "Emma Taylor",
                        Email = "emma@example.com"
                    }
                }
            }
        };

        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();
    }
}
