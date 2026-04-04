# Book Collection App

*(This README is intentionally rough so you can improve it with GitHub Copilot CLI)*

A Python app for managing books you have or want to read.
It can add, remove, and list books. Also mark them as read.
You can rate books, leave reviews, and see average ratings.

---

## Current Features

* Reads books from a JSON file (our database)
* Input checking is weak in some areas
* Some tests exist but probably not enough
* Rate books with 1-5 stars and add optional text reviews
* View all reviews for a book, including average rating
* See books sorted by average rating

---

## Files

* `book_app.py` - Main CLI entry point
* `books.py` - BookCollection class with data logic and Review dataclass
* `utils.py` - Helper functions for UI and input
* `data.json` - Sample book data
* `tests/test_books.py` - Pytest tests including review features

---

## Running the App

```bash
python book_app.py list
python book_app.py add
python book_app.py find
python book_app.py remove
python book_app.py mark-read "Book Title"
python book_app.py rate "Book Title" 5 "Optional review text"
python book_app.py view-reviews "Book Title"
python book_app.py list-reviews
python book_app.py help
```

## Running Tests

```bash
python -m pytest tests/
```

---

## Notes

* Not production-ready (obviously)
* Some code could be improved
* Could add more commands later
* Ratings must be between 1 and 5
