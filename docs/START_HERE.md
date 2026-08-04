# START HERE

This file is the entry point for developers and AI agents.

## Read Order

1. START_HERE.md
2. CLAUDE.md
3. docs/ARCHITECTURE.md
4. docs/REPOSITORY_MAP.md
5. docs/FEATURES.md
6. docs/CODING_STANDARDS.md

---

## Project Overview

Endpoint Status Checker provides monitoring and availability tracking for endpoints and services.

Primary goals:
- Fast status visibility
- Reliable endpoint validation
- Historical reporting
- Easy deployment

---

## Repository Structure

See:
docs/REPOSITORY_MAP.md

---

## Current Priorities

See:
docs/ROADMAP.md

---

## Architecture

See:
docs/ARCHITECTURE.md

---

## Design Principles

- Keep changes small.
- Prefer extension over rewrites.
- Preserve backward compatibility.
- Add tests for new behavior.
- Update documentation when functionality changes.

---

## Before Modifying Code

Determine:

- Which feature is affected?
- Which layer is affected?
- Is there an existing implementation?
- Is the change architectural or localized?

Record significant decisions in:
docs/DECISIONS.md

---

## AI Agent Workflow

1. Understand task.
2. Locate affected component.
3. Identify existing implementation.
4. Implement smallest viable change.
5. Run tests.
6. Update documentation.
7. Produce implementation summary.

---

## Human Approval Required

- Large refactoring
- Security model changes
- Authentication changes
- Storage format changes
- Dependency replacement
- Breaking API changes