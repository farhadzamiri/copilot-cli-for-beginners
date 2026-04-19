from typing import Tuple, List
from books import Book


def format_rating(avg_rating: float) -> str:
    """
    Format average rating as a string with star symbol.
    
    Args:
        avg_rating: Average rating value
        
    Returns:
        Formatted rating string (e.g., " [★ 4.5/5.0]") or empty string if rating is 0
    """
    if avg_rating == 0:
        return ""
    return f" [★ {avg_rating:.1f}/5.0]"


def print_menu() -> None:
    print("\n📚 Book Collection App")
    print("1. Add a book")
    print("2. List books")
    print("3. Mark book as read")
    print("4. Remove a book")
    print("5. Exit")


def get_user_choice() -> str:
    while True:
        choice = input("Choose an option (1-5): ").strip()
        if choice in ('1', '2', '3', '4', '5'):
            return choice
        print("Invalid choice. Please enter a number between 1 and 5.")


def get_book_details() -> Tuple[str, str, int]:
    while True:
        title = input("Enter book title: ").strip()
        if title:
            break
        print("Title cannot be empty. Please try again.")

    while True:
        author = input("Enter author: ").strip()
        if author:
            break
        print("Author cannot be empty. Please try again.")

    while True:
        year_input = input("Enter publication year: ").strip()
        try:
            year = int(year_input)
            if 1000 <= year <= 2100:
                break
            print("Year should be between 1000 and 2100. Please try again.")
        except ValueError:
            print("Invalid year format. Please enter a valid number.")

    return title, author, year


def print_books(books: List[Book]) -> None:
    if not books:
        print("No books in your collection.")
        return

    print("\nYour Books:")
    for index, book in enumerate(books, start=1):
        status = "✅ Read" if book.read else "📖 Unread"
        print(f"{index}. {book.title} by {book.author} ({book.year}) - {status}")


def print_books_with_ratings(books: List[Book], get_rating_fn) -> None:
    """
    Display books with their read status and average ratings.
    
    Args:
        books: List of Book objects to display
        get_rating_fn: Callable that takes book title and returns average rating
    """
    if not books:
        print("No books found.")
        return

    print("\nYour Book Collection:\n")

    for index, book in enumerate(books, start=1):
        status = "✓" if book.read else " "
        rating_str = format_rating(get_rating_fn(book.title))
        print(f"{index}. [{status}] {book.title} by {book.author} ({book.year}){rating_str}")

    print()


def print_books_by_author(books: List[Book], author: str, get_rating_fn) -> None:
    """
    Display books by a specific author with ratings and review counts.
    
    Args:
        books: List of Book objects to display
        author: Author name to display in header
        get_rating_fn: Callable that takes book title and returns average rating
    """
    if not books:
        print("No books found.")
        return

    print(f"\nBooks by {author}:\n")
    for index, book in enumerate(books, start=1):
        rating_str = format_rating(get_rating_fn(book.title))
        review_count = len(book.reviews)
        reviews_text = f" ({review_count} review{'s' if review_count != 1 else ''})" if review_count > 0 else ""
        print(f"{index}. {book.title} ({book.year}){rating_str}{reviews_text}")

    print()


def print_reviews(title: str, reviews: list, avg_rating: float) -> None:
    """
    Display reviews for a book with its average rating.
    
    Args:
        title: Book title
        reviews: List of Review objects
        avg_rating: Average rating for the book
    """
    if not reviews:
        print(f"\nNo reviews for '{title}'.\n")
        return

    print(f"\nReviews for '{title}' (Average: {avg_rating:.1f}/5.0)\n")

    for index, review in enumerate(reviews, start=1):
        print(f"{index}. Rating: {review.rating}/5")
        if review.text:
            print(f"   Review: {review.text}")
        print(f"   Added: {review.timestamp}\n")


def print_books_by_rating(books: List[Book], get_rating_fn) -> None:
    """
    Display books sorted by average rating in descending order.
    
    Args:
        books: List of books with reviews
        get_rating_fn: Callable that takes book title and returns average rating
    """
    if not books:
        print("\nNo books have been rated yet.\n")
        return

    sorted_books = sorted(books, 
                          key=lambda b: get_rating_fn(b.title), 
                          reverse=True)

    print("\nBooks by Average Rating:\n")
    for index, book in enumerate(sorted_books, start=1):
        avg_rating = get_rating_fn(book.title)
        review_count = len(book.reviews)
        print(f"{index}. [{avg_rating:.1f}/5.0] {book.title} by {book.author} ({review_count} review{'s' if review_count != 1 else ''})")

    print()
