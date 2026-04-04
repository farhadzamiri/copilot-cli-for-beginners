# Search and Filter Guide - C# Book App

This guide demonstrates how to use the new search and filter capabilities in the C# book application.

## New Commands

### 1. Search by Title
Search for books by partial title matching (case-insensitive).

**Command:** `dotnet run search`

**Example Session:**
```
$ dotnet run search

Search Books by Title

Search term: Hobbit

Your Book Collection:

1. [ ] The Hobbit by J.R.R. Tolkien (1937)

```

**Features:**
- Partial matching: "Hobbit" finds "The Hobbit"
- Case-insensitive: "hobbit" also works
- Returns all books with matching titles

---

### 2. Advanced Filtering
Filter books using multiple criteria combined with AND logic.

**Command:** `dotnet run filter`

**Example Session 1 - Filter by Read Status:**
```
$ dotnet run filter

Filter Books

Filter by read status (read/unread/all) [all]: unread
Filter by year range? (yes/no) [no]: no
Filter by author (optional): 

Your Book Collection:

1. [ ] The Hobbit by J.R.R. Tolkien (1937)
2. [ ] Dune by Frank Herbert (1965)
3. [ ] To Kill a Mockingbird by Harper Lee (1960)

```

**Example Session 2 - Filter by Year Range:**
```
$ dotnet run filter

Filter Books

Filter by read status (read/unread/all) [all]: all
Filter by year range? (yes/no) [no]: yes
Start year: 1945
End year: 1970
Filter by author (optional): 

Your Book Collection:

1. [✓] 1984 by George Orwell (1949)
2. [ ] Dune by Frank Herbert (1965)
3. [ ] To Kill a Mockingbird by Harper Lee (1960)

```

**Example Session 3 - Combined Filters:**
```
$ dotnet run filter

Filter Books

Filter by read status (read/unread/all) [all]: read
Filter by year range? (yes/no) [no]: yes
Start year: 1940
End year: 1960
Filter by author (optional): Orwell

Your Book Collection:

1. [✓] 1984 by George Orwell (1949)

```

## Filter Options

### Read Status Filter
- **read** - Show only books marked as read
- **unread** - Show only unread books
- **all** - Show all books (default)

### Year Range Filter
- Enter "yes" or "y" to enable
- Enter starting year (e.g., 1950)
- Enter ending year (e.g., 2020)
- **Note:** Range is inclusive on both ends
- Year range is optional

### Author Filter
- Enter partial author name (e.g., "Smith")
- Case-insensitive matching
- Returns books by authors containing the search term
- Leave empty to skip this filter

## Combined Filter Examples

### Example 1: Unread books by a specific author
```
Read status: unread
Year range: no
Author: Tolkien

Result: Unread books by authors with "Tolkien" in their name
```

### Example 2: Unread classic literature (pre-1960)
```
Read status: unread
Year range: yes (1900-1960)
Author: (leave empty)

Result: Unread books published before 1960
```

### Example 3: Read books by George Orwell from 1940s-1950s
```
Read status: read
Year range: yes (1940-1959)
Author: George

Result: Books by authors with "George" published 1940-1959 that are marked as read
```

## Default Behavior

- **No filters applied** (all fields empty or "all" selected) = Shows entire collection
- **Invalid year range** (start > end) = Shows error and returns to menu
- **No matches** = Displays "No books found."
- **Empty search/filter term** (for search command) = Shows error message

## Tips

1. **Partial Matching**: You don't need to enter the full title or author name
   - "1984" and "1984" both work
   - "Tolkien" finds "J.R.R. Tolkien"

2. **Case-Insensitive**: Capitalization doesn't matter
   - "hobbit", "Hobbit", "HOBBIT" all find the same books

3. **Combining Filters**: All filters must match (AND logic)
   - If you select "unread" and "1950-1960", the result is unread books from 1950-1960
   - A book must satisfy ALL conditions

4. **Year Range**: Both boundaries are inclusive
   - Year range 1950-1960 includes books from both 1950 and 1960

## Data Format

Books are stored in `data.json` with this structure:
```json
{
  "title": "The Hobbit",
  "author": "J.R.R. Tolkien",
  "year": 1937,
  "read": false
}
```

The search and filter functionality works on these fields:
- **title** - Used for title search
- **author** - Used for author filtering
- **year** - Used for year range filtering
- **read** - Used for read status filtering
