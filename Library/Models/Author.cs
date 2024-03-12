using System.ComponentModel.DataAnnotations;
namespace Library.Models;
public class Author
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Författarens namn är obligatoriskt!")]
    public string? Name { get; set; }

    public ICollection<BookAuthor>? BookAuthors { get; set; }


}