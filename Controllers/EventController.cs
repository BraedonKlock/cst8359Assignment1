using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers;

public class EventController : Controller
{
    private static readonly List<Event> events =
    [
        new Event
        {
            Id = 1,
            Title = "Career Fair",
            Date = new DateTime(2026, 2, 1),
            Location = "Gym"
        },

        new Event
        {
            Id = 2,
            Title = "Tech Talk",
            Date = new DateTime(2026, 2, 8),
            Location = "Auditorium"
        },

        new Event
        {
            Id = 3,
            Title = "Hack Night",
            Date = new DateTime(2026, 2, 15),
            Location = "Library"
        }
    ];

    public IActionResult Index()
    {
        ViewData["Title"] = "Event Manager";

        return View(events);
    }

    [HttpGet]
    public IActionResult ManageAttendees(int id)
    {
        Event? selectedEvent = events.FirstOrDefault(e => e.Id == id);

        if (selectedEvent == null)
        {
            return NotFound();
        }

        ViewData["EventName"] = selectedEvent.Title;

        return View(selectedEvent);
    }

    [HttpPost]
    public IActionResult ManageAttendees(int id, Attendee attendee)
    {
        Event? selectedEvent = events.FirstOrDefault(e => e.Id == id);

        if (selectedEvent == null)
        {
            return NotFound();
        }

        selectedEvent.Attendees.Add(attendee);

        ViewData["EventName"] = selectedEvent.Title;
        ViewData["Message"] = "Attendee registered!";

        return View(selectedEvent);
    }
}
