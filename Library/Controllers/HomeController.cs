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
        public async Task<IActionResult> Create(BookAndAuthorViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Skapa en ny författare baserat på ViewModel
                var author = new Author { Name = viewModel.AuthorName };

                // Skapa en ny bok baserat på ViewModel
                var book = new Book
                {
                    Title = viewModel.Title,
                    Genre = viewModel.Genre,
                    // Initialisera BookAuthors-samlingen och lägg till den nya författaren
                    BookAuthors = new List<BookAuthor>()
                };

                // Koppla boken med den nya författaren genom BookAuthor
                book.BookAuthors.Add(new BookAuthor { Book = book, Author = author });

                // Lägg till den nya boken 
                // i  DbContext och spara ändringarna
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                // Redirect efter framgångsrik skapande
                return RedirectToAction(nameof(Index));
            }


            return View(viewModel);
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
            // Hitta boken med det angivna id:et
            var book = await _context.Books.FindAsync(id);

            // Kontrollera om boken är null innan du försöker ta bort den
            if (book == null)
            {
                // Boken finns inte, utför lämplig felhantering, till exempel en 404-sida
                return NotFound();
            }

            // Boken finns, ta bort den från kontexten och spara ändringarna
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            // Omdirigera tillbaka till listan med böcker
            return RedirectToAction(nameof(Index));
        }


        // GET: Home/Details/5
        // Hämta bokdetaljer inklusive författaren
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
         .Include(b => b.BookAuthors!)
             .ThenInclude(ba => ba.Author)
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

            var booksQuery = _context.Books.AsQueryable();

            if (!String.IsNullOrEmpty(searchTerm))
            {
                if (searchBy == "Title")
                {
                    booksQuery = booksQuery.Where(b => b.Title != null && b.Title.Contains(searchTerm));
                }
                else if (searchBy == "Author")
                {
                    booksQuery = booksQuery.Where(b => b.BookAuthors != null && b.BookAuthors.Any(ba => ba.Author != null && ba.Author.Name != null && ba.Author.Name.Contains(searchTerm)));
                }
                else if (searchBy == "Genre")
                {
                    booksQuery = booksQuery.Where(b => b.Genre != null && b.Genre.Contains(searchTerm));
                }
            }

            var books = await booksQuery
              .Include(ba => ba!.Author)
                .ToListAsync();

            return View(books);
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
