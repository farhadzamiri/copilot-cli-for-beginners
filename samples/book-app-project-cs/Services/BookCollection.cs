using System.Text.Json;
using BookApp.Models;

namespace BookApp.Services;

public class BookCollection
{
    private readonly string _dataFile;
    private List<Book> _books = [];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>
    /// Initializes a new BookCollection instance and loads books from the data file.
    /// </summary>
    /// <param name="dataFile">Optional path to the JSON data file. If null, uses "data.json" in the application directory.</param>
    /// <example>
    /// <code>
    /// // Create a collection using the default data file
    /// var collection = new BookCollection();
    /// 
    /// // Create a collection using a custom data file for testing
    /// var testCollection = new BookCollection("/tmp/test-books.json");
    /// </code>
    /// </example>
    public BookCollection(string? dataFile = null)
    {
        _dataFile = dataFile ?? Path.Combine(AppContext.BaseDirectory, "data.json");
        LoadBooks();
    }

    /// <summary>
    /// Gets a read-only view of all books in the collection.
    /// </summary>
    /// <value>An immutable list of Book objects.</value>
    public IReadOnlyList<Book> Books => _books;

    /// <summary>
    /// Loads books from the JSON data file into memory.
    /// If the file doesn't exist, starts with an empty collection.
    /// If the JSON is corrupted, displays a warning and starts with an empty collection.
    /// </summary>
    /// <remarks>
    /// This method is called automatically in the constructor.
    /// It handles gracefully:
    /// - Missing data files (FileNotFoundException)
    /// - Invalid JSON format (JsonException)
    /// </remarks>
    /// <exception cref="System.IO.IOException">Thrown if file cannot be read due to permission issues.</exception>
    private void LoadBooks()
    {
        try
        {
            var json = File.ReadAllText(_dataFile);
            _books = JsonSerializer.Deserialize<List<Book>>(json, JsonOptions) ?? [];
        }
        catch (FileNotFoundException)
        {
            _books = [];
        }
        catch (JsonException)
        {
            Console.WriteLine("Warning: data.json is corrupted. Starting with empty collection.");
            _books = [];
        }
    }

    /// <summary>
    /// Persists the current book collection to the JSON data file.
    /// This method is called automatically after any modification to the collection.
    /// </summary>
    /// <remarks>
    /// The file is formatted with indentation for readability.
    /// Property names are converted to camelCase in the JSON.
    /// </remarks>
    /// <exception cref="System.IO.IOException">Thrown if the file cannot be written (e.g., disk full, permission denied).</exception>
    /// <exception cref="System.UnauthorizedAccessException">Thrown if the application lacks write permissions to the data file.</exception>
    private void SaveBooks()
    {
        var json = JsonSerializer.Serialize(_books, JsonOptions);
        File.WriteAllText(_dataFile, json);
    }

    /// <summary>
    /// Adds a new book to the collection and persists it to disk.
    /// </summary>
    /// <param name="title">The title of the book (required).</param>
    /// <param name="author">The author of the book (required).</param>
    /// <param name="year">The publication year of the book (required).</param>
    /// <returns>The newly created Book object.</returns>
    /// <example>
    /// <code>
    /// var collection = new BookCollection();
    /// var newBook = collection.AddBook("The Great Gatsby", "F. Scott Fitzgerald", 1925);
    /// Console.WriteLine($"Added: {newBook.Title}");
    /// </code>
    /// </example>
    /// <exception cref="System.IO.IOException">Thrown if the book cannot be saved to disk.</exception>
    public Book AddBook(string title, string author, int year)
    {
        var book = new Book { Title = title, Author = author, Year = year };
        _books.Add(book);
        SaveBooks();
        return book;
    }

    /// <summary>
    /// Returns all books in the collection.
    /// </summary>
    /// <returns>A list of all Book objects.</returns>
    /// <remarks>Use the Books property for a read-only view, or this method for a modifiable list.</remarks>
    /// <example>
    /// <code>
    /// var collection = new BookCollection();
    /// var allBooks = collection.ListBooks();
    /// Console.WriteLine($"Total books: {allBooks.Count}");
    /// </code>
    /// </example>
    public List<Book> ListBooks() => _books;

    /// <summary>
    /// Finds a single book by exact title match (case-insensitive).
    /// </summary>
    /// <param name="title">The title to search for.</param>
    /// <returns>The first Book with a matching title, or null if not found.</returns>
    /// <remarks>
    /// The search is case-insensitive and looks for an exact match.
    /// For partial matches, use SearchByTitle() instead.
    /// </remarks>
    /// <example>
    /// <code>
    /// var collection = new BookCollection();
    /// var book = collection.FindBookByTitle("The Great Gatsby");
    /// if (book != null)
    /// {
    ///     Console.WriteLine($"Found: {book.Author}");
    /// }
    /// </code>
    /// </example>
    public Book? FindBookByTitle(string title)
    {
        return _books.Find(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Marks a book as read and persists the change to disk.
    /// </summary>
    /// <param name="title">The title of the book to mark as read.</param>
    /// <returns>true if the book was found and marked; false if the book was not found.</returns>
    /// <remarks>
    /// This method finds the book by exact title match (case-insensitive) and updates its Read status.
    /// If the book is already read, this method has no effect but still returns true.
    /// </remarks>
    /// <example>
    /// <code>
    /// var collection = new BookCollection();
    /// if (collection.MarkAsRead("The Great Gatsby"))
    /// {
    ///     Console.WriteLine("Book marked as read.");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("Book not found.");
    /// }
    /// </code>
    /// </example>
    /// <exception cref="System.IO.IOException">Thrown if changes cannot be saved to disk.</exception>
    public bool MarkAsRead(string title)
    {
        var book = FindBookByTitle(title);
        if (book is null) return false;
        book.Read = true;
        SaveBooks();
        return true;
    }

    /// <summary>
    /// Removes a book from the collection and persists the change to disk.
    /// </summary>
    /// <param name="title">The title of the book to remove.</param>
    /// <returns>true if the book was found and removed; false if the book was not found.</returns>
    /// <remarks>
    /// This method finds the book by exact title match (case-insensitive) and removes it from the collection.
    /// </remarks>
    /// <example>
    /// <code>
    /// var collection = new BookCollection();
    /// if (collection.RemoveBook("The Great Gatsby"))
    /// {
    ///     Console.WriteLine("Book removed.");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("Book not found.");
    /// }
    /// </code>
    /// </example>
    /// <exception cref="System.IO.IOException">Thrown if changes cannot be saved to disk.</exception>
    public bool RemoveBook(string title)
    {
        var book = FindBookByTitle(title);
        if (book is null) return false;
        _books.Remove(book);
        SaveBooks();
        return true;
    }

    /// <summary>
    /// Finds all books by an exact author name match (case-insensitive).
    /// </summary>
    /// <param name="author">The author name to search for.</param>
    /// <returns>A list of books by the specified author. Returns an empty list if no matches found.</returns>
    /// <remarks>
    /// The search is case-insensitive and looks for an exact author name match.
    /// For partial author name matches, use FilterByAuthorPartial() instead.
    /// </remarks>
    /// <example>
    /// <code>
    /// var collection = new BookCollection();
    /// var books = collection.FindByAuthor("F. Scott Fitzgerald");
    /// Console.WriteLine($"Found {books.Count} books by this author.");
    /// </code>
    /// </example>
    public List<Book> FindByAuthor(string author)
    {
        return _books
            .Where(b => b.Author.Equals(author, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Searches for books by partial title match (case-insensitive substring search).
    /// </summary>
    /// <param name="searchTerm">The search term to match within book titles.</param>
    /// <returns>A list of books whose titles contain the search term. Returns an empty list if no matches found.</returns>
    /// <remarks>
    /// The search is case-insensitive and matches partial strings.
    /// For exact title matches, use FindBookByTitle() instead.
    /// An empty search term returns all books.
    /// </remarks>
    /// <example>
    /// <code>
    /// var collection = new BookCollection();
    /// var books = collection.SearchByTitle("Great");
    /// // May return "The Great Gatsby", "The Great Expectations", etc.
    /// </code>
    /// </example>
    public List<Book> SearchByTitle(string searchTerm)
    {
        return _books
            .Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Filters books by their read status.
    /// </summary>
    /// <param name="isRead">true to return only read books; false to return only unread books.</param>
    /// <returns>A list of books matching the specified read status. Returns an empty list if no matches found.</returns>
    /// <example>
    /// <code>
    /// var collection = new BookCollection();
    /// var unreadBooks = collection.FilterByReadStatus(false);
    /// Console.WriteLine($"You have {unreadBooks.Count} unread books.");
    /// </code>
    /// </example>
    public List<Book> FilterByReadStatus(bool isRead)
    {
        return _books.Where(b => b.Read == isRead).ToList();
    }

    /// <summary>
    /// Filters books by their publication year within a specified range (inclusive).
    /// </summary>
    /// <param name="startYear">The minimum publication year (inclusive).</param>
    /// <param name="endYear">The maximum publication year (inclusive).</param>
    /// <returns>A list of books published within the specified year range. Returns an empty list if no matches found.</returns>
    /// <remarks>
    /// Both startYear and endYear are inclusive. The range is applied as: startYear &lt;= year &lt;= endYear.
    /// </remarks>
    /// <example>
    /// <code>
    /// var collection = new BookCollection();
    /// var twentyCenturyBooks = collection.FilterByYearRange(1900, 1999);
    /// Console.WriteLine($"Found {twentyCenturyBooks.Count} books from the 20th century.");
    /// </code>
    /// </example>
    public List<Book> FilterByYearRange(int startYear, int endYear)
    {
        return _books
            .Where(b => b.Year >= startYear && b.Year <= endYear)
            .ToList();
    }

    /// <summary>
    /// Filters books by partial author name match (case-insensitive substring search).
    /// </summary>
    /// <param name="authorTerm">The search term to match within author names.</param>
    /// <returns>A list of books whose author names contain the search term. Returns an empty list if no matches found.</returns>
    /// <remarks>
    /// The search is case-insensitive and matches partial strings.
    /// For exact author name matches, use FindByAuthor() instead.
    /// An empty search term returns all books.
    /// </remarks>
    /// <example>
    /// <code>
    /// var collection = new BookCollection();
    /// var books = collection.FilterByAuthorPartial("Scott");
    /// // May return books by "F. Scott Fitzgerald", "Scott Fitzgerald Jr.", etc.
    /// </code>
    /// </example>
    public List<Book> FilterByAuthorPartial(string authorTerm)
    {
        return _books
            .Where(b => b.Author.Contains(authorTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Applies multiple filters to the book collection at once.
    /// Only non-null filter parameters are applied; null parameters are ignored.
    /// </summary>
    /// <param name="readStatus">Optional: Filter by read status. If null, ignores this filter.</param>
    /// <param name="startYear">Optional: Minimum publication year (inclusive). If null, no lower year limit is applied.</param>
    /// <param name="endYear">Optional: Maximum publication year (inclusive). If null, no upper year limit is applied.</param>
    /// <param name="authorTerm">Optional: Partial author name search term (case-insensitive). If null or empty, ignores this filter.</param>
    /// <returns>A list of books matching all specified filters. Returns an empty list if no matches found.</returns>
    /// <remarks>
    /// All filters are combined with AND logic: a book must match all specified conditions to be included.
    /// For year filtering to be applied, both startYear and endYear must have values.
    /// </remarks>
    /// <example>
    /// <code>
    /// var collection = new BookCollection();
    /// 
    /// // Find all unread books published between 1900-1999 by authors named "Scott"
    /// var results = collection.ApplyFilters(
    ///     readStatus: false,
    ///     startYear: 1900,
    ///     endYear: 1999,
    ///     authorTerm: "Scott"
    /// );
    /// 
    /// // Find all unread books (year filters ignored since only one is specified)
    /// var unreadOnly = collection.ApplyFilters(readStatus: false);
    /// </code>
    /// </example>
    public List<Book> ApplyFilters(bool? readStatus = null, int? startYear = null, int? endYear = null, string? authorTerm = null)
    {
        var results = _books.AsEnumerable();

        if (readStatus.HasValue)
        {
            results = results.Where(b => b.Read == readStatus.Value);
        }

        if (startYear.HasValue && endYear.HasValue)
        {
            results = results.Where(b => b.Year >= startYear.Value && b.Year <= endYear.Value);
        }

        if (!string.IsNullOrWhiteSpace(authorTerm))
        {
            results = results.Where(b => b.Author.Contains(authorTerm, StringComparison.OrdinalIgnoreCase));
        }

        return results.ToList();
    }
}
