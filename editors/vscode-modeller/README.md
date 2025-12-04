# Modeller DSL - VS Code Extension

Syntax highlighting and language support for the Modeller domain definition language.

## Features

- ✅ Syntax highlighting for all Modeller DSL file types
- ✅ Comment toggling with `Ctrl+/` (uses `#`)
- ✅ Bracket matching and auto-closing for `[]`, `()`, and `""`
- ✅ Code folding for blocks (domain...end, entity...end, etc.)
- ✅ Proper indentation rules

## Supported File Extensions

| Extension | Description | Example |
|-----------|-------------|---------|
| `.def` | Domain definitions | `domain.def` |
| `.entity` | Entity definitions | `customer.entity` |
| `.enum` | Enum/flags definitions | `status.enum` |
| `.service` | Service definitions | `orders.service` |
| `.command` | Command definitions | `create-order.command` |
| `.query` | Query definitions | `get-customer.query` |

---

## Installation

### Option 1: Quick Install (Copy to Extensions Folder)

This is the simplest method - just copy the folder to your VS Code extensions directory.

#### Windows (PowerShell)

```powershell
# Navigate to the Modeller repository
cd M:\Modeller

# Copy the extension to VS Code extensions folder
Copy-Item -Recurse -Force editors\vscode-modeller "$env:USERPROFILE\.vscode\extensions\modeller-dsl"

# Restart VS Code to activate the extension
```

#### Windows (Command Prompt)

```cmd
xcopy /E /I /Y editors\vscode-modeller "%USERPROFILE%\.vscode\extensions\modeller-dsl"
```

#### macOS / Linux

```bash
cp -r editors/vscode-modeller ~/.vscode/extensions/modeller-dsl
```

After copying, **restart VS Code** or reload the window (`Ctrl+Shift+P` → "Reload Window").

---

### Option 2: Install from VSIX Package

A VSIX is a packaged extension file that can be shared and installed easily.

#### Step 1: Install the packaging tool

```bash
npm install -g @vscode/vsce
```

#### Step 2: Package the extension

```bash
cd editors/vscode-modeller
vsce package
```

This creates a file like `modeller-dsl-1.0.0.vsix`.

#### Step 3: Install the VSIX in VS Code

1. Open VS Code
2. Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac)
3. Type **"Install from VSIX"** and select it
4. Browse to the `.vsix` file and select it
5. Click **Reload** when prompted

---

### Option 3: Development Mode (For Testing/Debugging)

Use this when making changes to the extension.

1. Open VS Code
2. Go to **File → Open Folder** and select `editors/vscode-modeller`
3. Press `F5` to launch a new VS Code window with the extension loaded
4. Open any `.entity`, `.def`, or other Modeller file to test

---

## Verifying Installation

After installation, verify the extension is working:

1. Open VS Code
2. Go to **Extensions** panel (`Ctrl+Shift+X`)
3. Search for "Modeller" - you should see "Modeller DSL" listed
4. Open any Modeller file (e.g., `samples/modeller/domain.def`)
5. You should see syntax highlighting applied

**Check file association:**
- Look at the bottom-right of VS Code when a file is open
- It should show "Modeller Domain", "Modeller Entity", etc. instead of "Plain Text"

---

## Syntax Highlighting Reference

The extension highlights different elements with distinct colors:

| Element | Examples | Color (typical theme) |
|---------|----------|----------------------|
| **Declaration keywords** | `domain`, `entity`, `enum`, `service`, `command`, `query`, `key`, `flags` | Purple/Blue |
| **Block keywords** | `end`, `input`, `output`, `errors`, `publishes`, `entities`, `enums`, `services`, `references`, `returns`, `index` | Blue |
| **Data types** | `text`, `integer`, `boolean`, `guid`, `datetime`, `decimal`, etc. | Cyan/Teal |
| **Relationships** | `has_one`, `has_many`, `belongs_to`, `many_to_many` | Purple |
| **Modifiers** | `optional`, `default`, `generated`, `unique`, `many`, `as`, `max` | Purple |
| **Boolean values** | `true`, `false` | Orange |
| **Strings** | `"Description text"` | Green/Orange |
| **Comments** | `# This is a comment` | Gray/Green |
| **Numbers** | `100`, `1.5` | Orange |

---

## Usage Examples

### Entity Definition (`.entity`)

```
# Customer Entity
# Represents a customer in the system

entity Customer
  "A customer who can place orders"

  Name: text(100) "Customer's full name"
  Email: email, optional "Contact email"
  IsActive: boolean, default(true) "Account status"

  has_many Order
  belongs_to Region
end

key Customer
  CustomerId: guid, generated

  index Email unique
  index [Region, Name]
end
```

### Domain Definition (`.def`)

```
# E-Commerce Domain

domain ECommerce
  "Online shopping platform"

  company "MyCompany"
  version "2.0.0"

  services
    Customers
    Orders
    Inventory
  end
end
```

### Command Definition (`.command`)

```
# CreateOrder Command

command CreateOrder
  "Creates a new order for a customer"

  input
    CustomerId: guid "The customer placing the order"
    Items: OrderItem, many "Items to order"
  end

  output
    Order "The created order"
  end

  errors
    CustomerNotFound "Customer does not exist"
    InsufficientStock "Not enough inventory"
  end

  publishes
    OrderCreated
    InventoryReserved
  end
end
```

---

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl+/` | Toggle line comment |
| `Ctrl+Shift+[` | Fold block |
| `Ctrl+Shift+]` | Unfold block |
| `Ctrl+K Ctrl+0` | Fold all |
| `Ctrl+K Ctrl+J` | Unfold all |

---

## Troubleshooting

### Extension not showing in list
- Make sure you copied to the correct folder
- Restart VS Code completely (not just reload)
- Check the folder name doesn't have extra nesting

### Syntax highlighting not working
- Check the file has the correct extension (`.entity`, `.def`, etc.)
- Look at the language mode in the bottom-right corner
- Try clicking it and manually selecting "Modeller Entity", etc.

### Colors look wrong
- Syntax highlighting colors depend on your VS Code theme
- Try a different theme to see variations
- The extension uses standard TextMate scopes that all themes support

---

## Uninstalling

### If installed via copy:
```powershell
# Windows
Remove-Item -Recurse "$env:USERPROFILE\.vscode\extensions\modeller-dsl"

# macOS/Linux
rm -rf ~/.vscode/extensions/modeller-dsl
```

### If installed via VSIX:
1. Go to Extensions panel (`Ctrl+Shift+X`)
2. Find "Modeller DSL"
3. Click the gear icon → Uninstall

---

## License

MIT

