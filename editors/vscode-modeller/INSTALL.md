# Quick Installation Guide

This guide will help you install the Modeller DSL syntax highlighting extension in VS Code.

## Prerequisites

- Visual Studio Code installed ([Download here](https://code.visualstudio.com/))

---

## Fastest Method: Copy the Folder

### Windows

1. Open **PowerShell** or **Command Prompt**

2. Run this command (adjust the path if your Modeller repo is elsewhere):

   ```powershell
   Copy-Item -Recurse -Force "M:\Modeller\editors\vscode-modeller" "$env:USERPROFILE\.vscode\extensions\modeller-dsl"
   ```

3. **Restart VS Code** completely (close all windows and reopen)

4. Done! Open any `.entity`, `.def`, `.enum`, `.service`, `.command`, or `.query` file to see syntax highlighting.

### macOS / Linux

1. Open **Terminal**

2. Run this command:

   ```bash
   cp -r /path/to/Modeller/editors/vscode-modeller ~/.vscode/extensions/modeller-dsl
   ```

3. **Restart VS Code**

4. Done!

---

## Verify It's Working

1. Open VS Code
2. Open any file from `samples/modeller/`, for example:
   - `samples/modeller/domain.def`
   - `samples/modeller/entities/entity.entity`
   - `samples/modeller/enums/data-type.enum`
3. You should see colors applied to keywords, strings, comments, etc.
4. Look at the bottom-right corner of VS Code - it should show "Modeller Domain" or similar (not "Plain Text")

---

## If It's Not Working

### Check the extension is installed

1. Press `Ctrl+Shift+X` to open Extensions panel
2. Type "Modeller" in the search
3. You should see "Modeller DSL" in the list

### Check the folder location

The extension folder should be at:
- **Windows**: `C:\Users\<YourName>\.vscode\extensions\modeller-dsl\`
- **macOS/Linux**: `~/.vscode/extensions/modeller-dsl/`

Inside that folder, you should see:
```
modeller-dsl/
├── package.json
├── language-configuration.json
├── syntaxes/
│   └── modeller.tmLanguage.json
└── README.md
```

### Force VS Code to reload

Press `Ctrl+Shift+P`, type "Reload Window", and press Enter.

---

## Updating the Extension

If you make changes to the extension or pull updates:

1. Delete the old extension folder
2. Copy the new version
3. Restart VS Code

```powershell
# Windows - Remove old, copy new
Remove-Item -Recurse -Force "$env:USERPROFILE\.vscode\extensions\modeller-dsl"
Copy-Item -Recurse "M:\Modeller\editors\vscode-modeller" "$env:USERPROFILE\.vscode\extensions\modeller-dsl"
```

---

## Need More Help?

See the full [README.md](README.md) for:
- Packaging as VSIX for distribution
- Development mode for testing changes
- Full syntax reference
- Troubleshooting tips

