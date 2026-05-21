# Changelog

All notable changes to the Modeller DSL extension will be documented in this file.

## [1.4.0] - 2026-05-22

### Changed

- Minimum VS Code engine version raised to 1.95.0
- Updated devDependencies: `@types/vscode` ^1.95.0, `@types/node` ^22.0.0
- Added `@vscode/vsce` to devDependencies for local packaging
- Added `package` and `publish` npm scripts
- Added MIT LICENSE file

## [1.3.0] - 2024-12-04

### Added

- **Auto-prompt for icon theme**: Extension now prompts to enable custom icons on first install
- Options: "Yes, enable icons", "No thanks", or "Remind me later"

## [1.2.0] - 2024-12-04

### Added

- **Icon Theme**: Custom file icons for all Modeller DSL file types
  - Unique icons for each file type (.def, .entity, .key, .enum, .flags, .service, .command, .query, .value, .shared, .event, .projection)
  - Enable via: File > Preferences > File Icon Theme > Modeller DSL Icons

## [1.1.0] - 2024-12-04

### Added

- New file type support:
  - Key definitions (`.key`)
  - Flags definitions (`.flags`)
  - Value object definitions (`.value`)
  - Shared/lookup data definitions (`.shared`)
  - Event definitions (`.event`)
  - Projection definitions (`.projection`)
- New syntax highlighting keywords:
  - Declaration: `value`, `shared`, `event`, `projection`
  - Block: `attributes`, `identity`, `ownership`, `data`, `fields`, `owned`
  - Modifiers: `immutable`, `on_create`, `through`, `parent`

## [1.0.0] - 2024-12-04

### Added

- Initial release
- Syntax highlighting for Modeller DSL files
  - Domain definitions (`.def`)
  - Entity definitions (`.entity`)
  - Enum definitions (`.enum`)
  - Service definitions (`.service`)
  - Command definitions (`.command`)
  - Query definitions (`.query`)
- Language configuration
  - Line comments with `#`
  - Bracket matching for `[]` and `()`
  - Auto-closing pairs
  - Code folding for block structures
  - Indentation rules
- Highlighting support for:
  - Declaration keywords (`domain`, `entity`, `enum`, `service`, `command`, `query`, `key`, `flags`)
  - Block keywords (`end`, `input`, `output`, `errors`, `publishes`, etc.)
  - Built-in data types (`text`, `integer`, `boolean`, `guid`, etc.)
  - Relationship keywords (`has_one`, `has_many`, `belongs_to`, `many_to_many`)
  - Modifiers (`optional`, `default`, `generated`, `unique`)
  - Boolean literals and numbers
  - Strings and comments

