using Assignment1.Data;
using Assignment1.Models;
using Assignment1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers;

[Route("events")]
public class EventController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly BlobStorageService _blobStorage;

    public EventController(
        ApplicationDbContext context,
        BlobStorageService blobStorage)
    {
        _context = context;
        _blobStorage = blobStorage;
    }

    // GET: /events
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var events = await _context.Events
            .OrderBy(e => e.Date)
            .ToListAsync();

        return View(events);
    }

    // GET: /events/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var selectedEvent = await _context.Events
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (selectedEvent == null)
        {
            return NotFound();
        }

        return View(selectedEvent);
    }

    // GET: /events/create
    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /events/create
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Title,Description,Date,Location")] Event eventItem,
        IFormFile? bannerImage)
    {
        if (!ModelState.IsValid)
        {
            return View(eventItem);
        }

        if (bannerImage != null)
        {
            try
            {
                eventItem.BannerUrl =
                    await _blobStorage.UploadAsync(bannerImage);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("bannerImage", ex.Message);
                return View(eventItem);
            }
        }

        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /events/1/edit
    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var selectedEvent = await _context.Events.FindAsync(id);

        if (selectedEvent == null)
        {
            return NotFound();
        }

        return View(selectedEvent);
    }

    // POST: /events/1/edit
    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Title,Description,Date,Location")] Event eventItem,
        IFormFile? bannerImage)
    {
        var existingEvent = await _context.Events.FindAsync(id);

        if (existingEvent == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            eventItem.Id = id;
            eventItem.BannerUrl = existingEvent.BannerUrl;
            return View(eventItem);
        }

        string bannerUrl = existingEvent.BannerUrl;

        if (bannerImage != null)
        {
            try
            {
                bannerUrl = await _blobStorage.UploadAsync(bannerImage);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("bannerImage", ex.Message);
                eventItem.Id = id;
                eventItem.BannerUrl = existingEvent.BannerUrl;
                return View(eventItem);
            }
        }

        existingEvent.Title = eventItem.Title;
        existingEvent.Description = eventItem.Description;
        existingEvent.Date = eventItem.Date;
        existingEvent.Location = eventItem.Location;
        existingEvent.BannerUrl = bannerUrl;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /events/1/delete
    [HttpGet("{id:int}/delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var selectedEvent = await _context.Events.FindAsync(id);

        if (selectedEvent == null)
        {
            return NotFound();
        }

        return View(selectedEvent);
    }

    // POST: /events/1/delete
    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var selectedEvent = await _context.Events.FindAsync(id);

        if (selectedEvent == null)
        {
            return NotFound();
        }

        _context.Events.Remove(selectedEvent);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
