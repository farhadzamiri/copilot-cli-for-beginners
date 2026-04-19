import sys
from books import BookCollection
from utils import format_rating, print_books_with_ratings, print_books_by_author, print_reviews, print_books_by_rating


# Global collection instance
collection = BookCollection()


def handle_list():
    books = collection.list_books()
    print_books_with_ratings(books, collection.get_average_rating)


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
    print_books_by_author(books, author, collection.get_average_rating)


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
    avg_rating = collection.get_average_rating(title)
    print_reviews(title, reviews, avg_rating)


def handle_list_reviews():
    books_with_reviews = [b for b in collection.list_books() if b.reviews]
    print_books_by_rating(books_with_reviews, collection.get_average_rating)


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


# Command dispatch table: maps command names to handler functions
COMMANDS = {
    "list": handle_list,
    "add": handle_add,
    "remove": handle_remove,
    "find": handle_find,
    "mark-read": handle_mark_read,
    "rate": handle_rate,
    "view-reviews": handle_view_reviews,
    "list-reviews": handle_list_reviews,
    "help": show_help,
}


def main():
    if len(sys.argv) < 2:
        show_help()
        return

    command = sys.argv[1].lower()
    
    # Look up and execute the command handler, or show error if not found
    handler = COMMANDS.get(command)
    if handler:
        handler()
    else:
        print("Unknown command.\n")
        show_help()


if __name__ == "__main__":
    main()
