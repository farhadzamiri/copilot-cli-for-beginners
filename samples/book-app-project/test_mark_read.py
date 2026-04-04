#!/usr/bin/env python3
"""Test script for the mark-read command."""

import sys
import os
import json

# Change to the script directory
os.chdir(os.path.dirname(os.path.abspath(__file__)))

from books import BookCollection

# Test 1: List books before marking
print("=== Test 1: List books before marking ===")
collection = BookCollection()
books = collection.list_books()
for i, book in enumerate(books, 1):
    status = "✓" if book.read else " "
    print(f"{i}. [{status}] {book.title} by {book.author}")

# Test 2: Mark "The Hobbit" as read
print("\n=== Test 2: Mark 'The Hobbit' as read ===")
result = collection.mark_as_read("The Hobbit")
print(f"Result: {result}")
if result:
    book = collection.find_book_by_title("The Hobbit")
    print(f"Book '{book.title}' read status: {book.read}")

# Test 3: Try to mark non-existent book
print("\n=== Test 3: Try to mark non-existent book ===")
result = collection.mark_as_read("Non-existent Book")
print(f"Result: {result}")

# Test 4: Reload and verify persistence
print("\n=== Test 4: Reload collection and verify persistence ===")
collection2 = BookCollection()
book = collection2.find_book_by_title("The Hobbit")
print(f"Book '{book.title}' read status after reload: {book.read}")

# Test 5: List all books after changes
print("\n=== Test 5: List all books after changes ===")
books = collection2.list_books()
for i, book in enumerate(books, 1):
    status = "✓" if book.read else " "
    print(f"{i}. [{status}] {book.title} by {book.author}")
