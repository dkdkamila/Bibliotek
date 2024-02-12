using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Titel är obligatorisk!")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Författare är obligatorisk!")]
        public string? Author { get; set; }

        [Required(ErrorMessage = "Genre är obligatorisk!")]
        public string? Genre { get; set; }
        [Required(ErrorMessage = "Låntagarens namn är obligatoriskt!")]
        public string? BorrowerName { get; set; }
        public DateTime? BorrowedDate { get; set; }
        public string? Status { get; set; }
        public bool IsBorrowed()
        {
            if (BorrowedDate.HasValue)
            {
                // Beräkna antalet dagar som har gått sedan boken lånades ut
                var daysSinceBorrowed = (DateTime.Now - BorrowedDate.Value).Days;

                // Om det har gått mindre än 14 dagar sedan boken lånades ut, anses den vara utlånad
                return daysSinceBorrowed < 14;
            }
            else
            {
                // Om boken inte har lånats ut alls, anses den inte vara utlånad
                return false;
            }
        }
    }
}


