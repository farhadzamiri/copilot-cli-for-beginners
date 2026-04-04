import sys
import os
sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

import pytest
import books
from books import BookCollection, Review


@pytest.fixture(autouse=True)
def use_temp_data_file(tmp_path, monkeypatch):
    """Use a temporary data file for each test."""
    temp_file = tmp_path / "data.json"
    temp_file.write_text("[]")
    monkeypatch.setattr(books, "DATA_FILE", str(temp_file))


def test_add_book():
    collection = BookCollection()
    initial_count = len(collection.books)
    collection.add_book("1984", "George Orwell", 1949)
    assert len(collection.books) == initial_count + 1
    book = collection.find_book_by_title("1984")
    assert book is not None
    assert book.author == "George Orwell"
    assert book.year == 1949
    assert book.read is False

def test_mark_book_as_read():
    collection = BookCollection()
    collection.add_book("Dune", "Frank Herbert", 1965)
    result = collection.mark_as_read("Dune")
    assert result is True
    book = collection.find_book_by_title("Dune")
    assert book.read is True

def test_mark_book_as_read_invalid():
    collection = BookCollection()
    result = collection.mark_as_read("Nonexistent Book")
    assert result is False

def test_remove_book():
    collection = BookCollection()
    collection.add_book("The Hobbit", "J.R.R. Tolkien", 1937)
    result = collection.remove_book("The Hobbit")
    assert result is True
    book = collection.find_book_by_title("The Hobbit")
    assert book is None

def test_remove_book_invalid():
    collection = BookCollection()
    result = collection.remove_book("Nonexistent Book")
    assert result is False


# Review and Rating Tests
def test_add_review():
    collection = BookCollection()
    collection.add_book("The Great Gatsby", "F. Scott Fitzgerald", 1925)
    result = collection.add_review("The Great Gatsby", 5, "Excellent classic!")
    assert result is True
    reviews = collection.get_reviews("The Great Gatsby")
    assert len(reviews) == 1
    assert reviews[0].rating == 5
    assert reviews[0].text == "Excellent classic!"

def test_add_review_invalid_rating_low():
    collection = BookCollection()
    collection.add_book("Test Book", "Author", 2020)
    with pytest.raises(ValueError):
        collection.add_review("Test Book", 0, "Bad rating")

def test_add_review_invalid_rating_high():
    collection = BookCollection()
    collection.add_book("Test Book", "Author", 2020)
    with pytest.raises(ValueError):
        collection.add_review("Test Book", 6, "Bad rating")

def test_add_review_nonexistent_book():
    collection = BookCollection()
    result = collection.add_review("Nonexistent", 5, "Review")
    assert result is False

def test_add_multiple_reviews():
    collection = BookCollection()
    collection.add_book("Book A", "Author A", 2000)
    collection.add_review("Book A", 5, "Great!")
    collection.add_review("Book A", 4, "Good")
    collection.add_review("Book A", 5, "Loved it")
    reviews = collection.get_reviews("Book A")
    assert len(reviews) == 3

def test_get_average_rating():
    collection = BookCollection()
    collection.add_book("Rated Book", "Author", 2010)
    assert collection.get_average_rating("Rated Book") == 0.0
    
    collection.add_review("Rated Book", 5)
    assert collection.get_average_rating("Rated Book") == 5.0
    
    collection.add_review("Rated Book", 3)
    assert collection.get_average_rating("Rated Book") == 4.0
    
    collection.add_review("Rated Book", 4)
    assert abs(collection.get_average_rating("Rated Book") - 4.0) < 0.01

def test_get_average_rating_nonexistent():
    collection = BookCollection()
    assert collection.get_average_rating("Nonexistent Book") == 0.0

def test_add_review_without_text():
    collection = BookCollection()
    collection.add_book("Silent Book", "Author", 2015)
    result = collection.add_review("Silent Book", 4)
    assert result is True
    reviews = collection.get_reviews("Silent Book")
    assert reviews[0].text == ""

def test_data_persistence():
    """Test that reviews are saved and loaded correctly."""
    collection1 = BookCollection()
    collection1.add_book("Persistent Book", "Author", 2020)
    collection1.add_review("Persistent Book", 5, "Persisted!")
    
    collection2 = BookCollection()
    book = collection2.find_book_by_title("Persistent Book")
    assert book is not None
    reviews = collection2.get_reviews("Persistent Book")
    assert len(reviews) == 1
    assert reviews[0].rating == 5
    assert reviews[0].text == "Persisted!"
