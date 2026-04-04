import sys
from books import BookCollection


# Global collection instance
collection = BookCollection()


def format_rating(avg_rating: float) -> str:
    """Format average rating as a string."""
    if avg_rating == 0:
        return ""
    return f" [★ {avg_rating:.1f}/5.0]"


def show_books(books):
    """Display books in a user-friendly format."""
    if not books:
        print("No books found.")
        return

    print("\nYour Book Collection:\n")

    for index, book in enumerate(books, start=1):
        status = "✓" if book.read else " "
        rating_str = format_rating(collection.get_average_rating(book.title))
        print(f"{index}. [{status}] {book.title} by {book.author} ({book.year}){rating_str}")

    print()


def handle_list():
    books = collection.list_books()
    show_books(books)


def handle_add():
    print("\nAdd a New Book\n")

    title = input("Title: ").strip()
    author = input("Author: ").strip()
    year_str = input("Year: ").strip()

    try:
        year = int(year_str) if year_str else 0
        collection.add_book(title, author, year)
        print("\nBook added successfully.\n")
    except ValueError as e:
        print(f"\nError: {e}\n")


def handle_remove():
    print("\nRemove a Book\n")

    title = input("Enter the title of the book to remove: ").strip()
    collection.remove_book(title)

    print("\nBook removed if it existed.\n")


def handle_find():
    print("\nFind Books by Author\n")

    author = input("Author name: ").strip()
    books = collection.find_by_author(author)

    if not books:
        print("No books found.")
        return

    print(f"\nBooks by {author}:\n")
    for index, book in enumerate(books, start=1):
        rating_str = format_rating(collection.get_average_rating(book.title))
        review_count = len(book.reviews)
        reviews_text = f" ({review_count} review{'s' if review_count != 1 else ''})" if review_count > 0 else ""
        print(f"{index}. {book.title} ({book.year}){rating_str}{reviews_text}")

    print()


def handle_mark_read():
    if len(sys.argv) < 3:
        print("\nError: Please provide a book title.\n")
        print("Usage: python book_app.py mark-read \"Book Title\"\n")
        return

    title = sys.argv[2]
    if collection.mark_as_read(title):
        print(f"\nBook '{title}' marked as read.\n")
    else:
        print(f"\nBook '{title}' not found.\n")


def handle_rate():
    if len(sys.argv) < 4:
        print("\nError: Please provide a book title and rating.\n")
        print("Usage: python book_app.py rate \"Book Title\" <1-5> [optional review text]\n")
        return

    title = sys.argv[2]
    try:
        rating = int(sys.argv[3])
    except ValueError:
        print("\nError: Rating must be a number between 1 and 5.\n")
        return

    review_text = " ".join(sys.argv[4:]) if len(sys.argv) > 4 else ""

    try:
        if collection.add_review(title, rating, review_text):
            print(f"\nRating added to '{title}'.\n")
        else:
            print(f"\nBook '{title}' not found.\n")
    except ValueError as e:
        print(f"\nError: {e}\n")


def handle_view_reviews():
    if len(sys.argv) < 3:
        print("\nError: Please provide a book title.\n")
        print("Usage: python book_app.py view-reviews \"Book Title\"\n")
        return

    title = sys.argv[2]
    reviews = collection.get_reviews(title)

    if not reviews:
        print(f"\nNo reviews for '{title}'.\n")
        return

    avg_rating = collection.get_average_rating(title)
    print(f"\nReviews for '{title}' (Average: {avg_rating:.1f}/5.0)\n")

    for index, review in enumerate(reviews, start=1):
        print(f"{index}. Rating: {review.rating}/5")
        if review.text:
            print(f"   Review: {review.text}")
        print(f"   Added: {review.timestamp}\n")


def handle_list_reviews():
    books_with_reviews = [b for b in collection.list_books() if b.reviews]

    if not books_with_reviews:
        print("\nNo books have been rated yet.\n")
        return

    sorted_books = sorted(books_with_reviews, 
                          key=lambda b: collection.get_average_rating(b.title), 
                          reverse=True)

    print("\nBooks by Average Rating:\n")
    for index, book in enumerate(sorted_books, start=1):
        avg_rating = collection.get_average_rating(book.title)
        review_count = len(book.reviews)
        print(f"{index}. [{avg_rating:.1f}/5.0] {book.title} by {book.author} ({review_count} review{'s' if review_count != 1 else ''})")

    print()


def show_help():
    print("""
Book Collection Helper

Commands:
  list           - Show all books
  add            - Add a new book
  remove         - Remove a book by title
  find           - Find books by author
  mark-read      - Mark a book as read
  rate           - Add a rating and optional review to a book
  view-reviews   - View all reviews for a book
  list-reviews   - Show all books sorted by average rating
  help           - Show this help message
""")


def main():
    if len(sys.argv) < 2:
        show_help()
        return

    command = sys.argv[1].lower()

    if command == "list":
        handle_list()
    elif command == "add":
        handle_add()
    elif command == "remove":
        handle_remove()
    elif command == "find":
        handle_find()
    elif command == "mark-read":
        handle_mark_read()
    elif command == "rate":
        handle_rate()
    elif command == "view-reviews":
        handle_view_reviews()
    elif command == "list-reviews":
        handle_list_reviews()
    elif command == "help":
        show_help()
    else:
        print("Unknown command.\n")
        show_help()


if __name__ == "__main__":
    main()
