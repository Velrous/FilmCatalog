using Microsoft.AspNetCore.Mvc;
using FilmCatalog.Models;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace FilmCatalog.Controllers
{
    public class FilmController : Controller
    {
        private readonly FilmCatalogContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public FilmController(FilmCatalogContext context, IWebHostEnvironment hostEnvironment)
        {
            dbContext = context;
            webHostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Info(int? filmId)
        {
            if (filmId == null)
                return Redirect("~/Home/Index");
            Film film = new Film();
            film = await dbContext.Films.FirstOrDefaultAsync(u => u.Id == filmId);
            return View(film);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(FilmViewModel model)
        {
            if(ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model.Image);
                Film film = new Film
                {
                    Title = model.Title,
                    Desc = model.Desc,
                    Year = model.Year,
                    Director = model.Director,
                    UserEmail = User.Identity.Name,
                    Image = uniqueFileName,
                };
                dbContext.Add(film);
                await dbContext.SaveChangesAsync();
                return Redirect("~/Home/Index");
            }
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int? filmId)
        {
            if(filmId!=null)
            {
                Film film = await dbContext.Films.AsNoTracking().FirstOrDefaultAsync(u => u.Id == filmId);
                if (film != null && film.UserEmail == User.Identity.Name)
                {
                    FilmEditModel filmEditModel = new FilmEditModel
                    {
                        Id = film.Id,
                        Title = film.Title,
                        Desc = film.Desc,
                        Year = film.Year,
                        Director = film.Director,
                        UserEmail = film.UserEmail,
                        Image = film.Image,
                        ImageNew = null,
                    };
                    return View(filmEditModel); ;
                }
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FilmEditModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageNew != null)
                {
                    string uniqueFileName = UploadedFile(model.ImageNew);
                    model.Image = uniqueFileName;
                }
                Film film = new Film
                {
                    Id = model.Id,
                    Title = model.Title,
                    Desc = model.Desc,
                    Year = model.Year,
                    Director = model.Director,
                    UserEmail = model.UserEmail,
                    Image = model.Image,
                };
                dbContext.Films.Update(film);
                await dbContext.SaveChangesAsync();
                return Redirect("~/Film/Info/?filmId=" + @model.Id);
            }
            return View(model);
        }

        private string UploadedFile(IFormFile file)
        {
            string uniqueFileName = null;
            if (file != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
