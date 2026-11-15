using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers;

[Route("events/{eventId:int}/attendees")]
public class AttendeesController : Controller
{
    private readonly ApplicationDbContext _context;

    public AttendeesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /events/1/attendees
    [HttpGet("")]
    public async Task<IActionResult> Index(int eventId)
    {
        var selectedEvent = await _context.Events
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (selectedEvent == null)
        {
            return NotFound();
        }

        return View(selectedEvent);
    }

    // GET: /events/1/attendees/create
    [HttpGet("create")]
    public async Task<IActionResult> Create(int eventId)
    {
        var selectedEvent = await _context.Events.FindAsync(eventId);

        if (selectedEvent == null)
        {
            return NotFound();
        }

        ViewBag.EventId = eventId;
        ViewBag.EventTitle = selectedEvent.Title;

        return View();
    }

    // POST: /events/1/attendees/create
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int eventId,
        [Bind("Name,Email")] Attendee attendee)
    {
        var selectedEvent = await _context.Events.FindAsync(eventId);

        if (selectedEvent == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.EventId = eventId;
            ViewBag.EventTitle = selectedEvent.Title;

            return View(attendee);
        }

        attendee.Id = Guid.NewGuid().ToString();
        attendee.EventId = eventId;

        _context.Attendees.Add(attendee);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { eventId });
    }

    // GET: /events/1/attendees/{id}/edit
    [HttpGet("{id}/edit")]
    public async Task<IActionResult> Edit(int eventId, string id)
    {
        var attendee = await _context.Attendees
            .FirstOrDefaultAsync(a =>
                a.Id == id && a.EventId == eventId);

        if (attendee == null)
        {
            return NotFound();
        }

        return View(attendee);
    }

    // POST: /events/1/attendees/{id}/edit
    [HttpPost("{id}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int eventId,
        string id,
        [Bind("Name,Email")] Attendee attendee)
    {
        var existingAttendee = await _context.Attendees
            .FirstOrDefaultAsync(a =>
                a.Id == id && a.EventId == eventId);

        if (existingAttendee == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            attendee.Id = id;
            attendee.EventId = eventId;

            return View(attendee);
        }

        existingAttendee.Name = attendee.Name;
        existingAttendee.Email = attendee.Email;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { eventId });
    }

    // GET: /events/1/attendees/{id}/delete
    [HttpGet("{id}/delete")]
    public async Task<IActionResult> Delete(int eventId, string id)
    {
        var attendee = await _context.Attendees
            .FirstOrDefaultAsync(a =>
                a.Id == id && a.EventId == eventId);

        if (attendee == null)
        {
            return NotFound();
        }

        return View(attendee);
    }

    // POST: /events/1/attendees/{id}/delete
    [HttpPost("{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        int eventId,
        string id)
    {
        var attendee = await _context.Attendees
            .FirstOrDefaultAsync(a =>
                a.Id == id && a.EventId == eventId);

        if (attendee == null)
        {
            return NotFound();
        }

        _context.Attendees.Remove(attendee);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { eventId });
    }
}
