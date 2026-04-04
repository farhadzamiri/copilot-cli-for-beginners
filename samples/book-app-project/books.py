import json
from dataclasses import dataclass, asdict, field
from typing import List, Optional
from datetime import datetime

DATA_FILE = "data.json"


@dataclass
class Review:
    rating: int
    text: str = ""
    timestamp: str = field(default_factory=lambda: datetime.now().isoformat())

    def __post_init__(self):
        if not (1 <= self.rating <= 5):
            raise ValueError("Rating must be between 1 and 5")


@dataclass
class Book:
    title: str
    author: str
    year: int
    read: bool = False
    reviews: List[Review] = field(default_factory=list)


class BookCollection:
    def __init__(self):
        self.books: List[Book] = []
        self.load_books()

    def load_books(self):
        """Load books from the JSON file if it exists."""
        try:
            with open(DATA_FILE, "r") as f:
                data = json.load(f)
                self.books = []
                for b in data:
                    reviews_data = b.pop("reviews", [])
                    reviews = [Review(**r) for r in reviews_data]
                    book = Book(**b, reviews=reviews)
                    self.books.append(book)
        except FileNotFoundError:
            self.books = []
        except json.JSONDecodeError:
            print("Warning: data.json is corrupted. Starting with empty collection.")
            self.books = []

    def save_books(self):
        """Save the current book collection to JSON."""
        with open(DATA_FILE, "w") as f:
            json.dump([asdict(b) for b in self.books], f, indent=2)

    def add_book(self, title: str, author: str, year: int) -> Book:
        book = Book(title=title, author=author, year=year)
        self.books.append(book)
        self.save_books()
        return book

    def list_books(self) -> List[Book]:
        return self.books

    def find_book_by_title(self, title: str) -> Optional[Book]:
        for book in self.books:
            if book.title.lower() == title.lower():
                return book
        return None

    def mark_as_read(self, title: str) -> bool:
        book = self.find_book_by_title(title)
        if book:
            book.read = True
            self.save_books()
            return True
        return False

    def remove_book(self, title: str) -> bool:
        """Remove a book by title."""
        book = self.find_book_by_title(title)
        if book:
            self.books.remove(book)
            self.save_books()
            return True
        return False

    def find_by_author(self, author: str) -> List[Book]:
        """Find all books by a given author."""
        return [b for b in self.books if b.author.lower() == author.lower()]

    def add_review(self, title: str, rating: int, text: str = "") -> bool:
        """Add a review to a book."""
        if not (1 <= rating <= 5):
            raise ValueError("Rating must be between 1 and 5")
        book = self.find_book_by_title(title)
        if book:
            review = Review(rating=rating, text=text)
            book.reviews.append(review)
            self.save_books()
            return True
        return False

    def get_reviews(self, title: str) -> List[Review]:
        """Get all reviews for a book."""
        book = self.find_book_by_title(title)
        return book.reviews if book else []

    def get_average_rating(self, title: str) -> float:
        """Calculate the average rating for a book."""
        reviews = self.get_reviews(title)
        if not reviews:
            return 0.0
        return sum(r.rating for r in reviews) / len(reviews)
