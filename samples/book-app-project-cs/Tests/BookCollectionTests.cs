using BookApp.Services;

namespace BookApp.Tests;

public class BookCollectionTests : IDisposable
{
    private readonly string _tempFile;
    private readonly BookCollection _collection;

    public BookCollectionTests()
    {
        _tempFile = Path.GetTempFileName();
        File.WriteAllText(_tempFile, "[]");
        _collection = new BookCollection(_tempFile);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
    }

    [Fact]
    public void AddBook_ShouldAddAndPersist()
    {
        var initialCount = _collection.Books.Count;
        _collection.AddBook("1984", "George Orwell", 1949);

        Assert.Equal(initialCount + 1, _collection.Books.Count);

        var book = _collection.FindBookByTitle("1984");
        Assert.NotNull(book);
        Assert.Equal("George Orwell", book.Author);
        Assert.Equal(1949, book.Year);
        Assert.False(book.Read);
    }

    [Fact]
    public void MarkAsRead_ShouldSetReadTrue()
    {
        _collection.AddBook("Dune", "Frank Herbert", 1965);
        var result = _collection.MarkAsRead("Dune");

        Assert.True(result);
        Assert.True(_collection.FindBookByTitle("Dune")!.Read);
    }

    [Fact]
    public void MarkAsRead_NonexistentBook_ShouldReturnFalse()
    {
        var result = _collection.MarkAsRead("Nonexistent Book");
        Assert.False(result);
    }

    [Fact]
    public void RemoveBook_ShouldRemoveExistingBook()
    {
        _collection.AddBook("The Hobbit", "J.R.R. Tolkien", 1937);
        var result = _collection.RemoveBook("The Hobbit");

        Assert.True(result);
        Assert.Null(_collection.FindBookByTitle("The Hobbit"));
    }

    [Fact]
    public void RemoveBook_NonexistentBook_ShouldReturnFalse()
    {
        var result = _collection.RemoveBook("Nonexistent Book");
        Assert.False(result);
    }

    [Fact]
    public void SearchByTitle_ShouldFindPartialMatches()
    {
        _collection.AddBook("The Hobbit", "J.R.R. Tolkien", 1937);
        _collection.AddBook("The Lord of the Rings", "J.R.R. Tolkien", 1954);
        _collection.AddBook("1984", "George Orwell", 1949);

        var results = _collection.SearchByTitle("The");
        Assert.Equal(2, results.Count);
        Assert.Contains(results, b => b.Title == "The Hobbit");
        Assert.Contains(results, b => b.Title == "The Lord of the Rings");
    }

    [Fact]
    public void SearchByTitle_ShouldBeCaseInsensitive()
    {
        _collection.AddBook("Dune", "Frank Herbert", 1965);

        var results = _collection.SearchByTitle("dune");
        Assert.Single(results);
        Assert.Equal("Dune", results[0].Title);
    }

    [Fact]
    public void SearchByTitle_NoMatches_ShouldReturnEmpty()
    {
        _collection.AddBook("1984", "George Orwell", 1949);

        var results = _collection.SearchByTitle("Nonexistent");
        Assert.Empty(results);
    }

    [Fact]
    public void FilterByReadStatus_ShouldReturnReadBooks()
    {
        _collection.AddBook("1984", "George Orwell", 1949);
        _collection.AddBook("Dune", "Frank Herbert", 1965);
        _collection.MarkAsRead("1984");

        var readBooks = _collection.FilterByReadStatus(true);
        Assert.Single(readBooks);
        Assert.Equal("1984", readBooks[0].Title);

        var unreadBooks = _collection.FilterByReadStatus(false);
        Assert.Single(unreadBooks);
        Assert.Equal("Dune", unreadBooks[0].Title);
    }

    [Fact]
    public void FilterByYearRange_ShouldReturnBooksInRange()
    {
        _collection.AddBook("The Hobbit", "J.R.R. Tolkien", 1937);
        _collection.AddBook("1984", "George Orwell", 1949);
        _collection.AddBook("Dune", "Frank Herbert", 1965);

        var results = _collection.FilterByYearRange(1945, 1960);
        Assert.Single(results);
        Assert.Equal("1984", results[0].Title);
    }

    [Fact]
    public void FilterByYearRange_InclusiveBoundaries_ShouldIncludeEdgeYears()
    {
        _collection.AddBook("Book1", "Author1", 1950);
        _collection.AddBook("Book2", "Author2", 1955);
        _collection.AddBook("Book3", "Author3", 1960);

        var results = _collection.FilterByYearRange(1950, 1960);
        Assert.Equal(3, results.Count);
    }

    [Fact]
    public void FilterByAuthorPartial_ShouldFindPartialMatches()
    {
        _collection.AddBook("Book1", "John Smith", 1950);
        _collection.AddBook("Book2", "John Doe", 1960);
        _collection.AddBook("Book3", "Jane Smith", 1970);

        var results = _collection.FilterByAuthorPartial("John");
        Assert.Equal(2, results.Count);
        Assert.Contains(results, b => b.Author == "John Smith");
        Assert.Contains(results, b => b.Author == "John Doe");
    }

    [Fact]
    public void FilterByAuthorPartial_ShouldBeCaseInsensitive()
    {
        _collection.AddBook("Dune", "Frank Herbert", 1965);

        var results = _collection.FilterByAuthorPartial("frank");
        Assert.Single(results);
        Assert.Equal("Frank Herbert", results[0].Author);
    }

    [Fact]
    public void ApplyFilters_WithReadStatus_ShouldFilterCorrectly()
    {
        _collection.AddBook("1984", "George Orwell", 1949);
        _collection.AddBook("Dune", "Frank Herbert", 1965);
        _collection.MarkAsRead("1984");

        var results = _collection.ApplyFilters(readStatus: true);
        Assert.Single(results);
        Assert.Equal("1984", results[0].Title);
    }

    [Fact]
    public void ApplyFilters_WithYearRange_ShouldFilterCorrectly()
    {
        _collection.AddBook("The Hobbit", "J.R.R. Tolkien", 1937);
        _collection.AddBook("1984", "George Orwell", 1949);
        _collection.AddBook("Dune", "Frank Herbert", 1965);

        var results = _collection.ApplyFilters(startYear: 1945, endYear: 1960);
        Assert.Single(results);
        Assert.Equal("1984", results[0].Title);
    }

    [Fact]
    public void ApplyFilters_WithAuthor_ShouldFilterCorrectly()
    {
        _collection.AddBook("Book1", "John Smith", 1950);
        _collection.AddBook("Book2", "Jane Smith", 1960);

        var results = _collection.ApplyFilters(authorTerm: "John");
        Assert.Single(results);
        Assert.Equal("John Smith", results[0].Author);
    }

    [Fact]
    public void ApplyFilters_CombineMultiple_ShouldApplyAllFilters()
    {
        _collection.AddBook("Book1", "John Smith", 1950);
        _collection.AddBook("Book2", "John Smith", 1960);
        _collection.AddBook("Book3", "Jane Smith", 1955);
        _collection.MarkAsRead("Book1");
        _collection.MarkAsRead("Book3");

        var results = _collection.ApplyFilters(readStatus: true, startYear: 1955, endYear: 1960, authorTerm: "Smith");
        Assert.Single(results);
        Assert.Equal("Book3", results[0].Title);
    }

    [Fact]
    public void ApplyFilters_AllNull_ShouldReturnAll()
    {
        _collection.AddBook("1984", "George Orwell", 1949);
        _collection.AddBook("Dune", "Frank Herbert", 1965);

        var results = _collection.ApplyFilters();
        Assert.Equal(2, results.Count);
    }
}
