using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FilmCatalog.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FilmCatalog.Controllers
{
    public class HomeController : Controller
    {
        private readonly FilmCatalogContext dbContext;

        public HomeController(FilmCatalogContext context)
        {
            dbContext = context;
        }

        public async Task<IActionResult> Index(int page=1)
        {
            int pageSize = 6;
            IQueryable<Film> filmsIQ = dbContext.Films;
            var count = await filmsIQ.CountAsync();
            var films = await filmsIQ.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexViewModel indexViewModel = new IndexViewModel
            {
                PageViewModel = pageViewModel,
                Films = films,
            };
            return View(indexViewModel);
        }
    }
}
