# GitHub Copilot Instructions for copilot-cli-for-beginners

This is an **educational course repository** teaching GitHub Copilot CLI workflows. The codebase includes 7 progressive chapters, sample applications in multiple languages, and agent/skill templates—all designed to be beginner-friendly.

## Build and Release

This repository generates **demonstration GIFs** for course materials using VHS terminal recordings. There is no traditional build/compile step.

**Demo Generation Pipeline:**
```bash
npm install                    # Install Node.js dependencies
npm run release               # Full pipeline: create:tapes → generate:vhs → verify:gifs
npm run create:tapes          # Generate .tape files from chapter image directories
npm run generate:vhs          # Render .tape files as GIFs using VHS
npm run verify:gifs           # Validate generated GIFs
```

**For Sample Project Development:**
The primary sample (`samples/book-app-project/` Python) is used throughout all chapters:
```bash
cd samples/book-app-project
python -m pytest tests/                    # Run all tests
python -m pytest tests/test_books.py::TestBooks::test_add_book -v  # Run single test
python book_app.py help                    # View all CLI commands
```

## Architecture and Structure

### Repository Layout

| Path | Purpose |
|------|---------|
| `00-07/` | **Chapters**: Each numbered folder follows: Real-World Analogy → Core Concepts → Hands-On Examples → Assignment → Preview Next |
| `samples/book-app-project/` | **PRIMARY SAMPLE**: Python book collection CLI app—evolved through all chapters |
| `samples/book-app-project-{cs,js}/` | Same app in C# and JavaScript for language flexibility |
| `samples/book-app-buggy/` | Intentionally buggy sample for Ch 03 debugging exercises—**DO NOT FIX** |
| `samples/agents/` | Agent template examples (`.agent.md` format) |
| `samples/skills/` | Skill template examples (`SKILL.md` files) |
| `samples/mcp-configs/` | MCP server configuration examples |
| `.github/scripts/` | Demo generation infrastructure (Node.js scripts) |
| `appendices/` | Supplementary reference material |

### Key Design Principles

- **Single Evolving Example**: The Python book app appears in every chapter, progressively adding features (ratings, reviews, testing, agents, etc.)
- **Multi-Language Support**: Same logic available in Python, C#, and JavaScript so learners can choose their preferred language
- **Intentional Bugs**: `book-app-buggy/` and `samples/buggy-code/` contain bugs for hands-on debugging—these are **learning exercises, not bugs to fix**
- **Copy-Paste Ready**: All bash examples in chapters are guaranteed runnable—test them before committing
- **Beginner-Friendly**: Explain AI/ML terminology; assume no prior Copilot experience

## Key Conventions

### File Naming and Structure

**Chapters:**
- Numbered format: `NN-descriptive-name/` (e.g., `04-agents-custom-instructions`)
- Each chapter has a `README.md` with learning objectives, prerequisites, and examples
- Demo `.tape` files live in `NN-*/images/` for VHS rendering

**Agent Files:**
```
samples/agents/python-reviewer/
├── .agent.md           # YAML frontmatter + markdown instructions
├── README.md           # Description for humans
└── (optional files)

# .agent.md YAML frontmatter:
---
name: python-reviewer
description: "Reviews Python code for quality, bugs, and security"
tools: ["read", "grep", "view"]  # Optional tool restrictions
---
```

**Skill Files:**
```
samples/skills/pytest-gen/
├── SKILL.md            # YAML frontmatter + markdown instructions
├── README.md           # Description for humans
└── (optional files)

# SKILL.md YAML frontmatter:
---
name: pytest-gen
description: "Generate comprehensive pytest tests"
---
```

### Documentation Links (Don't Duplicate)

Before writing inline explanations, check if docs already exist:
- **Repository overview**: See `README.md` and `AGENTS.md`
- **AI/CLI terminology**: See `GLOSSARY.md` (always link, don't re-explain)
- **Sample app details**: See `samples/book-app-project/README.md`
- **Agent/Skill format**: See `samples/agents/README.md` and `samples/skills/README.md`
- **Contribution rules**: See `CONTRIBUTING.md`

### Python Sample Project

**Location**: `samples/book-app-project/`

**Environment:**
- Python 3.10+ (specified in `pyproject.toml`)
- Dependencies: pytest (for testing)

**Test Structure:**
- Tests live in `tests/` directory
- Use pytest convention: `test_*.py` and `Test*` classes
- Run single test: `python -m pytest tests/test_books.py::TestClassName::test_method_name -v`
- Run full suite: `python -m pytest tests/`

**App Commands** (all available via `python book_app.py help`):
- `list` - Show all books
- `add` - Add a book
- `find` - Search books
- `remove` - Delete a book
- `mark-read "Title"` - Mark as read
- `rate "Title" RATING ["optional review"]` - Rate 1-5 stars
- `view-reviews "Title"` - See ratings and reviews for a book
- `list-reviews` - Show all rated books

## Common Development Tasks

### Adding a New Chapter

1. Create `0N-descriptive-name/` folder
2. Add `README.md` with learning objectives, prerequisites, and examples
3. Add chapter to course table in main `README.md`
4. Use `samples/book-app-project/` paths for any primary examples

### Updating Demo GIFs

1. Edit or create `.tape` files in `NN-*/images/`
2. Run `npm run generate:demos`
3. Commit the generated `.gif` files

### Testing the Primary Sample

```bash
cd samples/book-app-project
python -m pytest tests/ -v              # Full test suite with verbose output
python -m pytest tests/ -x              # Stop on first failure
python -m pytest tests/ -k keyword      # Run tests matching keyword
```

### Creating a New Agent Template

Place in `samples/agents/agent-name/` with `.agent.md` frontmatter including `name` and `description`.

### Creating a New Skill Template

Place in `samples/skills/skill-name/` with `SKILL.md` frontmatter including `name` and `description`.

## Important Reminders

- **Do not fix intentional bugs** in `samples/book-app-buggy/` or `samples/buggy-code/` — they're learning exercises
- **Keep explanations beginner-friendly** — assume no AI/ML background; link to `GLOSSARY.md` for terminology
- **Test bash examples before committing** — all should be copy-paste ready
- **Link to existing docs** rather than duplicating explanations (GLOSSARY.md, README.md, CONTRIBUTING.md)
- **Use Python and pytest** as the primary example context (though C# and JS samples exist)
- **Update README.md** course table when adding new chapters

## MCP Server Configuration (Chapter 6)

This course includes **Chapter 6: MCP Servers**, which teaches how to connect Copilot to external services (GitHub, documentation, filesystem, custom APIs). When working on course content involving MCP:

**Key Files:**
- `06-mcp-servers/README.md` — Chapter content and conceptual overview
- `06-mcp-servers/mcp-custom-server.md` — Guide for building custom MCP servers
- `samples/mcp-configs/mcp-config.json` — Reference MCP configuration examples

**Built-in MCP Servers Documented in Course:**
- **GitHub MCP** — Access to GitHub issues, PRs, commits (built-in, requires `GITHUB_TOKEN`)
- **Filesystem MCP** — Local project file exploration
- **Context7 MCP** — Up-to-date library documentation

**For Development:**
If adding new MCP server examples, use `samples/mcp-configs/mcp-config.json` as the reference format. Always test that examples are copy-paste ready and explain how to set required environment variables (e.g., `GITHUB_TOKEN`).

## Useful References

- `README.md` — Course overview, learning path, prerequisites
- `AGENTS.md` — Repository structure and contributor guidelines for AI agents
- `GLOSSARY.md` — Terminology reference (link here instead of re-explaining AI/ML concepts)
- `CONTRIBUTING.md` — Contribution guidelines
- `samples/book-app-project/README.md` — Primary sample app features and commands
- `samples/agents/README.md` — Agent file format specification
- `samples/skills/README.md` — Skill file format specification
- `06-mcp-servers/README.md` — MCP servers chapter and conceptual guide
