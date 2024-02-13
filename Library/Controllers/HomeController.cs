using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using Library.Data;


namespace Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LibraryDbContext _context;

        public HomeController(ILogger<HomeController> logger, LibraryDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: Home/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Genre")] Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Boken har lagts till.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Ett fel uppstod när boken skulle skapas. Vänligen försök igen.");
                    _logger.LogError($"Databasfel vid försök att skapa bok: {ex.Message}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ett oväntat fel uppstod när boken skulle skapas. Vänligen försök igen.");
                    _logger.LogError($"Oväntat fel vid försök att skapa bok: {ex.Message}");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Boken kunde inte läggas till. Vänligen kontrollera inmatningsfälten.";
            }

            return View(book);
        }
        // GET: Home/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Genre")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            return View(book);
        }


        // GET: Home/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Home/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        // GET: Home/Search
        public async Task<IActionResult> Search(string searchTerm, string searchBy)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var books = _context.Books.AsQueryable();

            if (!String.IsNullOrEmpty(searchTerm))
            {
                if (searchBy == "Title")
                {
                    books = books.Where(b => b.Title.Contains(searchTerm));
                }
                else if (searchBy == "Author")
                {
                    books = books.Where(b => b.Author.Contains(searchTerm));
                }
                else if (searchBy == "Genre")
                {
                    books = books.Where(b => b.Genre.Contains(searchTerm));
                }
            }

            return View(await books.ToListAsync());
        }

        // GET: Home/Borrow/5
        public async Task<IActionResult> Borrow(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost, ActionName("Borrow")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(int id, string borrowerName)
        {
            // Check if the book exists
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            // Check if the borrower's name is valid
            if (string.IsNullOrWhiteSpace(borrowerName))
            {
                ModelState.AddModelError("BorrowerName", "Låntagarens namn får inte vara tomt.");
            }

            // Check if the book is available for lending
            if (!book.IsBorrowed())
            {
                book.BorrowerName = borrowerName;

                book.BorrowedDate = DateTime.Now;
                book.Status = "Utlånad";

                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Boken har lånats ut till " + borrowerName + ".";
            }
            else
            {
                TempData["ErrorMessage"] = "Boken är redan utlånad och kan inte lånas ut igen.";
            }

            return RedirectToAction("Index");
        }



    }
}
