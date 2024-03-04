using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mission06_Aken.Models;
using Mission6_Aken2.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Mission6_Aken.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetToKnowJoel()
        {
            _logger.LogInformation("Accessed the Get to Know Joel page");
            return View();
        }
        public IActionResult Create()
        {
            PrepareCreateEditViewData();
            return View(new Movie()); // Ensure Movie's constructor doesn't expect arguments.
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Title,Year,Director,Rating,Edited,CopiedToPlex,LentTo,Notes")] Movie movie)
        {
            if (ModelState.IsValid && CategoryExists(movie.CategoryId))
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, it means something failed; re-populate ViewBag and return view.
            PrepareCreateEditViewData(movie.CategoryId, movie.Rating);
            return View(movie);
        }

        public async Task<IActionResult> JoelMovies()
        {
            var movies = await _context.Movies.Include(m => m.Category).ToListAsync();
            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            PrepareCreateEditViewData(movie.CategoryId, movie.Rating);
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,CategoryId,Title,Year,Director,Rating,Edited,CopiedToPlex,LentTo,Notes")] Movie movie)
        {
            if (id != movie.MovieId) return NotFound();

            if (ModelState.IsValid && CategoryExists(movie.CategoryId))
            {
                _context.Update(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PrepareCreateEditViewData(movie.CategoryId, movie.Rating);
            return View(movie);
        }

        // GET: HomeController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.Include(m => m.Category).FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        // POST: HomeController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int? categoryId)
        {
            if (!categoryId.HasValue)
                return false;

            return _context.Categories.Any(e => e.CategoryId == categoryId.Value);
        }

        private bool MovieExists(int id) => _context.Movies.Any(e => e.MovieId == id);

        private void PrepareCreateEditViewData(int? selectedCategoryId = null, string? selectedRating = null)
        {
            ViewBag.Categories = new SelectList(_context.Categories.OrderBy(c => c.CategoryName), "CategoryId", "CategoryName", selectedCategoryId);

            var ratings = new List<string> { "G", "PG", "PG13", "R" };
            ViewBag.RatingTypes = new SelectList(ratings, selectedRating);
        }


        private void PopulateRatingTypesDropDownList(object selectedRating = null)
        {
            var ratingsQuery = Enum.GetValues(typeof(RatingType))
                                .Cast<RatingType>()
                                .Select(r => new { Value = r.ToString(), Text = r.ToString() });

            ViewBag.RatingTypes = new SelectList(ratingsQuery, "Value", "Text", selectedRating);
        }
    }
}
