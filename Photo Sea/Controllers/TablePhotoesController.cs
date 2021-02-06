using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Photo_Sea.Models;

namespace Photo_Sea.Controllers
{
    [Authorize]
    public class TablePhotoesController : Controller
    {
        private readonly MyPhotoDataContext _context;
        
        private readonly IWebHostEnvironment _hostEnvironment;

        public TablePhotoesController(MyPhotoDataContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;

        }

        // GET: TablePhotoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TablePhoto.ToListAsync());
        }

        // GET: TablePhotoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tablePhoto = await _context.TablePhoto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tablePhoto == null)
            {
                return NotFound();
            }

            return View(tablePhoto);
        }

        // GET: TablePhotoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TablePhotoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Cname,PictureName,PhotoName,Date")] TablePhoto tablePhoto)
        {
            if (ModelState.IsValid)
            {

                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(tablePhoto.PhotoName.FileName);
                string extensioName = Path.GetExtension(tablePhoto.PhotoName.FileName);
                tablePhoto.PictureName = fileName = fileName + extensioName;
                string path = Path.Combine(wwwRootPath + "/Photo/", fileName);

                tablePhoto.Cname = @User.Identity.Name;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await tablePhoto.PhotoName.CopyToAsync(fileStream);
                }
                    _context.Add(tablePhoto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tablePhoto);
        }

        // GET: TablePhotoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tablePhoto = await _context.TablePhoto.FindAsync(id);
            if (tablePhoto == null)
            {
                return NotFound();
            }
            return View(tablePhoto);
        }

        // POST: TablePhotoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cname,PictureName,Type,Date")] TablePhoto tablePhoto)
        {
            if (id != tablePhoto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tablePhoto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TablePhotoExists(tablePhoto.Id))
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
            return View(tablePhoto);
        }

        // GET: TablePhotoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tablePhoto = await _context.TablePhoto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tablePhoto == null)
            {
                return NotFound();
            }

            return View(tablePhoto);
        }

        // POST: TablePhotoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tablePhoto = await _context.TablePhoto.FindAsync(id);
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Photo", tablePhoto.PictureName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
            _context.TablePhoto.Remove(tablePhoto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TablePhotoExists(int id)
        {
            return _context.TablePhoto.Any(e => e.Id == id);
        }
    }
}
