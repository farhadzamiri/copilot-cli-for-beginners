using BookApp.Models;
using BookApp.Services;

var collection = new BookCollection();

void ShowBooks(List<Book> books)
{
    if (books.Count == 0)
    {
        Console.WriteLine("No books found.");
        return;
    }

    Console.WriteLine("\nYour Book Collection:\n");

    for (int i = 0; i < books.Count; i++)
    {
        var book = books[i];
        var status = book.Read ? "✓" : " ";
        Console.WriteLine($"{i + 1}. [{status}] {book.Title} by {book.Author} ({book.Year})");
    }

    Console.WriteLine();
}

void HandleList()
{
    var books = collection.ListBooks();
    ShowBooks(books);
}

void HandleAdd()
{
    Console.WriteLine("\nAdd a New Book\n");

    Console.Write("Title: ");
    var title = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Author: ");
    var author = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Year: ");
    var yearStr = Console.ReadLine()?.Trim() ?? "";

    if (int.TryParse(yearStr, out var year))
    {
        collection.AddBook(title, author, year);
        Console.WriteLine("\nBook added successfully.\n");
    }
    else
    {
        Console.WriteLine($"\nError: '{yearStr}' is not a valid year.\n");
    }
}

void HandleRemove()
{
    Console.WriteLine("\nRemove a Book\n");

    Console.Write("Enter the title of the book to remove: ");
    var title = Console.ReadLine()?.Trim() ?? "";
    collection.RemoveBook(title);

    Console.WriteLine("\nBook removed if it existed.\n");
}

void HandleFind()
{
    Console.WriteLine("\nFind Books by Author\n");

    Console.Write("Author name: ");
    var author = Console.ReadLine()?.Trim() ?? "";
    var books = collection.FindByAuthor(author);

    ShowBooks(books);
}

void HandleSearch()
{
    Console.WriteLine("\nSearch Books by Title\n");

    Console.Write("Search term: ");
    var searchTerm = Console.ReadLine()?.Trim() ?? "";
    
    if (string.IsNullOrWhiteSpace(searchTerm))
    {
        Console.WriteLine("\nError: Please enter a search term.\n");
        return;
    }

    var books = collection.SearchByTitle(searchTerm);
    ShowBooks(books);
}

void HandleFilter()
{
    Console.WriteLine("\nFilter Books\n");

    // Read status filter
    Console.Write("Filter by read status (read/unread/all) [all]: ");
    var statusInput = Console.ReadLine()?.Trim().ToLower() ?? "all";
    bool? readStatus = statusInput switch
    {
        "read" => true,
        "unread" => false,
        "all" => null,
        _ => null
    };

    // Year range filter
    int? startYear = null;
    int? endYear = null;
    Console.Write("Filter by year range? (yes/no) [no]: ");
    var yearFilterInput = Console.ReadLine()?.Trim().ToLower() ?? "no";
    if (yearFilterInput == "yes" || yearFilterInput == "y")
    {
        Console.Write("Start year: ");
        if (int.TryParse(Console.ReadLine()?.Trim() ?? "", out var start))
        {
            startYear = start;
        }

        Console.Write("End year: ");
        if (int.TryParse(Console.ReadLine()?.Trim() ?? "", out var end))
        {
            endYear = end;
        }

        if (startYear.HasValue && endYear.HasValue && startYear > endYear)
        {
            Console.WriteLine("\nError: Start year cannot be greater than end year.\n");
            return;
        }
    }

    // Author filter
    Console.Write("Filter by author (optional): ");
    var authorTerm = Console.ReadLine()?.Trim() ?? "";
    if (string.IsNullOrWhiteSpace(authorTerm))
    {
        authorTerm = null;
    }

    var books = collection.ApplyFilters(readStatus, startYear, endYear, authorTerm);
    ShowBooks(books);
}

void ShowHelp()
{
    Console.WriteLine("""

    Book Collection Helper

    Commands:
      list     - Show all books
      add      - Add a new book
      remove   - Remove a book by title
      find     - Find books by author
      search   - Search books by title
      filter   - Filter books by status, year, and/or author
      help     - Show this help message
    """);
}

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "list":
        HandleList();
        break;
    case "add":
        HandleAdd();
        break;
    case "remove":
        HandleRemove();
        break;
    case "find":
        HandleFind();
        break;
    case "search":
        HandleSearch();
        break;
    case "filter":
        HandleFilter();
        break;
    case "help":
        ShowHelp();
        break;
    default:
        Console.WriteLine("Unknown command.\n");
        ShowHelp();
        break;
}
