﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ThienASPMVC08032023.Database;
using ThienASPMVC08032023.Models;
using X.PagedList;

namespace ThienASPMVC08032023.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin, manager")]
    [Area("Admin")]
    [Route("/{controller}/{action=Index}/{id:int?}")]
    public class ClipsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClipsController> _logger;
        private readonly UserManager<AppUser> _userManager;

        [TempData]
        public string? StatusMessage { get; set; }

        public ClipsController(AppDbContext context, ILogger<ClipsController> logger, UserManager<AppUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;

        }


        // GET: Clips
        [HttpGet]
        public ActionResult Index(string sortOrder, int? currentPage, int? pageSize)
        {
            var qrClips = from c in _context.Clips
                          select c;

            //sort

            ViewData["SortbyName"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SortbyTimeCreated"] = sortOrder == "TimeCreated" ? "TimeCreated_desc" : "TimeCreated";

            switch (sortOrder)
            {

                case "name_desc":
                    qrClips = qrClips.OrderByDescending(c => c.Name);
                    break;
                case "TimeCreated":
                    qrClips = qrClips.OrderBy(c => c.TimeCreated);
                    break;

                case "TimeCreated_desc":
                    qrClips = qrClips.OrderByDescending(c => c.TimeCreated);
                    break;
                default:
                    qrClips = qrClips.OrderBy(c => c.Name);
                    break;
            }

            // paging
            if (currentPage == null)
            {
                currentPage = 1;
            }

            if (pageSize == null)
            {
                pageSize = 5;
            }

            return View(qrClips.ToPagedList((int)currentPage, (int)pageSize));

        }

        // GET: Clips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound("Not found clipId"); }
            if (_context.Clips == null)
            {
                return NotFound("Not found any Clip in db");
            }

            var clip = await _context.Clips
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clip == null) { return NotFound($"Not found clip has id = {id}"); }


            return View(clip);
        }

        // GET: Clips/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Url,TimeCreated,AuthorId,AuthorUsername")] Clip clip)
        {


            if (ModelState.IsValid)
            {
                AppUser currentUser = await _userManager.GetUserAsync(User);
                clip.AuthorUsername = currentUser.UserName;
                clip.AuthorUser = currentUser;

                _context.Add(clip);
                await _context.SaveChangesAsync();

                StatusMessage = $"Created Clip name :  {clip.Name} Successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(clip);
        }


        // GET: Clips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null) { return NotFound("Not found clipId"); }
            if (_context.Clips == null)
            {
                return NotFound("Not found any Clip in db");
            }
            var clip = await _context.Clips.FindAsync(id);
            if (clip == null) { return NotFound($"Not found clip has id = {id}"); }

            return View(clip);
        }

        // POST: Clips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Name,Description,Url,TimeCreated")] Clip clip)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    AppUser currentUser = await _userManager.GetUserAsync(User);
                    clip.AuthorUser = currentUser;
                    clip.AuthorUsername = currentUser.UserName;

                    _context.Update(clip);
                    await _context.SaveChangesAsync();
                    StatusMessage = $"Edited Clip name : {clip.Name} Successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClipExists(clip.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(clip);
        }

        // GET: Clips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { return NotFound("Not found clipId"); }
            if (_context.Clips == null)
            {
                return NotFound("Not found any Clip in db");
            }
            var clip = await _context.Clips.FirstOrDefaultAsync(c => c.Id == id);

            if (clip == null) { return NotFound($"Not found clip has id = {id}"); }

            return View(clip);
        }

        // POST: Clips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clips == null)
            {
                return NotFound("Not found any Clip in db");
            }
            var clip = await _context.Clips.FindAsync(id);
            if (clip != null)
            {
                _context.Clips.Remove(clip);
            }

            await _context.SaveChangesAsync();
            StatusMessage = $"Deleted Clip {clip?.Name} Successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool ClipExists(int id)
        {
            return _context.Clips!.Any(e => e.Id == id);
        }
    }
}