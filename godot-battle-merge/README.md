# Battle Merge - Godot 4 Implementation

A mobile puzzle game combining M2 Block-style merge mechanics with lane defense gameplay.

## 🎮 Game Overview

**Battle Merge** is a vertical, column-based merge puzzle game where you defend against descending virus threats while building up powerful firewall blocks through strategic merging.

### Core Mechanics

1. **M2 Block Style Gameplay**
   - 5 columns × 8 rows grid
   - Tap a column to drop the current block
   - Blocks fall to the lowest empty position
   - Identical blocks merge when vertically adjacent

2. **Defense Layer**
   - Viruses spawn at the top every 10 seconds
   - Viruses descend after each player turn
   - Merged blocks act as firewall turrets
   - Block value ≥ Virus value → Virus destroyed
   - Block value < Virus value → Both destroyed

3. **In-Game Economy**
   - Earn coins from merges (based on 2^n value)
   - Four purchasable items:
     - **Swap** (50 coins): Exchange current and next block
     - **Sledgehammer** (100 coins): Remove any block
     - **Rainbow Joker** (250 coins): Next block matches what it lands on
     - **Column Nuke** (500 coins): Clear entire column

### Visual Theme
- Neon Cyberpunk aesthetic
- Dark background with bright, glowing blocks
- High contrast colors (cyan, magenta, yellow, etc.)

## 🚀 How to Run

### Option 1: Godot Editor (Development)

1. **Install Godot 4.3+**
   - Download from [godotengine.org](https://godotengine.org/download)

2. **Open Project**
   ```bash
   # Navigate to this directory
   cd godot-battle-merge
   
   # Open in Godot
   godot project.godot
   ```

3. **Play in Editor**
   - Press F5 or click the Play button
   - Use mouse clicks to simulate touch input

### Option 2: Web Export

1. **Export from Godot**
   - Project → Export
   - Add "Web" preset if not present
   - Configure HTML5 export settings
   - Export project

2. **Run Locally**
   ```bash
   # Serve the exported files
   python3 -m http.server 8000
   
   # Open browser to:
   # http://localhost:8000
   ```

3. **Deploy to GitHub Pages**
   - Export to `docs/` or `gh-pages` branch
   - Enable GitHub Pages in repository settings

## 📱 Controls

- **Touch/Mouse Click**: Tap a column to drop the current block
- **Item Buttons**: Use purchased items (bottom of screen)
- **Restart Button**: Appears on game over

## 🎯 Game Rules

### Merge Mechanics
- Blocks merge vertically (top into bottom)
- Merged value = 2× original value
- Chain reactions occur automatically
- Gravity applies after merges

### Scoring
- Points = merged block value
- Coins = ⌈n/3⌉ × 2 (where 2^n = value)
- Example: 1024 (2^10) → ⌈10/3⌉ × 2 = 8 coins

### Game Over Conditions
1. Column becomes full (can't place block)
2. Virus reaches the bottom row

### Strategy Tips
- Build high-value blocks early for better defense
- Use items strategically to avoid game over
- Plan ahead - next block is always shown
- Virus spawn is predictable (every 10 seconds)

## 🏗️ Project Structure

```
godot-battle-merge/
├── project.godot          # Godot project configuration
├── scenes/
│   ├── Main.tscn          # Main game scene
│   └── Block.tscn         # Reusable block prefab
├── scripts/
│   ├── Game.gd            # Main game controller
│   └── Block.gd           # Block behavior
├── assets/
│   ├── sprites/           # Game sprites (to be added)
│   ├── fonts/             # Fonts (to be added)
│   └── audio/             # Sound effects (to be added)
└── README.md              # This file
```

## 🔧 Implementation Details

### Grid System
- 2D Array: `grid[col][row]` (5×8)
- Row 7 = bottom (landing zone)
- Row 0 = top (virus spawn zone)
- Positive values = player blocks
- Negative values = virus blocks
- 0 = empty cell

### Game State
- `current_block_value`: Block being placed
- `next_block_value`: Preview of next block
- `score`: Total points earned
- `coins`: Currency for items
- `virus_spawn_timer`: Countdown to next virus wave

### Key Functions
- `handle_column_tap(col)`: Main player action
- `_check_merge(col, row)`: Recursive merge logic
- `_apply_gravity(col)`: Slide blocks down
- `_spawn_viruses()`: Add virus threats
- `_descend_viruses()`: Move viruses down
- `_defense_interaction()`: Block vs virus combat

## 🎨 Customization

### Block Colors
Edit `block_colors` dictionary in `scripts/Block.gd`:
```gdscript
var block_colors = {
    2: Color(0.0, 1.0, 1.0),    # Cyan
    4: Color(1.0, 0.0, 1.0),    # Magenta
    # ... add more
}
```

### Economy Balance
Edit values in `scripts/Game.gd`:
```gdscript
var economy = {
    "swap_cost": 50,
    "sledgehammer_cost": 100,
    "joker_cost": 250,
    "nuke_cost": 500
}
```

### Virus Spawn Rate
```gdscript
const VIRUS_SPAWN_INTERVAL = 10.0  # seconds
```

## 🐛 Known Issues / TODO

- [ ] Add particle effects for merges
- [ ] Add laser effects for defense
- [ ] Add sound effects
- [ ] Add background music
- [ ] Improve visual polish (sprites vs colored squares)
- [ ] Add difficulty progression
- [ ] Add high score persistence
- [ ] Add touch gesture improvements for mobile

## 📄 License

This is a demonstration project. See repository root for license information.

## 🤝 Contributing

This game was implemented based on the "Battle Merge" specification combining M2 Block mechanics with tower defense elements.

For bug reports or feature requests, please open an issue in the main repository.

---

**Tech Stack**: Godot 4.3, GDScript
**Target Platform**: Web (HTML5), Mobile (Android/iOS)
**Input**: Touchscreen optimized, mouse compatible
