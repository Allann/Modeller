# Icon Assets

This directory contains the icon assets for the Modeller CLI.

## Files

- `icon.svg` - Vector CLI-specific icon design (with terminal indicator)
- `icon.png` - Current icon for NuGet package (256x256, shared with VSCode extension)

## Current Status

The `icon.png` is currently identical to the VSCode extension icon to maintain brand consistency. The `icon.svg` provides an alternative CLI-specific design with terminal indicators (green dots and accent line).

## Future Enhancement

You can replace `icon.png` with a CLI-specific design by generating it from `icon.svg`:

### Generating PNG from SVG

The PNG icon needs to be generated from the SVG source. You can use any of these methods:

### Option 1: Using Inkscape (Recommended)

```bash
inkscape icon.svg --export-type=png --export-filename=icon.png --export-width=256 --export-height=256
```

### Option 2: Using ImageMagick

```bash
magick convert -background none -size 256x256 icon.svg icon.png
```

### Option 3: Using Online Tools

1. Open https://cloudconvert.com/svg-to-png
2. Upload `icon.svg`
3. Set dimensions to 256x256
4. Download as `icon.png`

### Option 4: Using Visual Studio Code + Extension

1. Install "SVG" extension by jock
2. Right-click `icon.svg`
3. Select "SVG: Export PNG"
4. Set width/height to 256

### Option 5: Using Node.js (sharp package)

```bash
npm install -g sharp-cli
sharp -i icon.svg -o icon.png resize 256 256
```

## Design Notes

The icon is designed to match the Modeller VSCode extension while indicating it's a CLI tool:

- **Blue color scheme** (`#2D5A8C`, `#4A90E2`) - Matches the VSCode extension
- **"M" lettermark** - Represents "Modeller" brand
- **Green accent dots** (`#4AE290`) - Suggests terminal/CLI interaction
- **Geometric, modern style** - Professional and clean

## Consistency with VSCode Extension

The CLI icon uses the same color palette as the VSCode extension icon located at:
`../../editors/vscode-modeller/images/modeller-icon.png`

Both icons should be immediately recognizable as part of the same product family.
