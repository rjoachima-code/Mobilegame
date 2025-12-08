# Battle Merge - Godot 4 Game Implementation

This directory contains a complete implementation of **Battle Merge**, a mobile puzzle game combining M2 Block-style merge mechanics with lane defense gameplay.

## 🎮 Quick Play (Web Version)

**Play instantly in your browser** without installing anything:

```bash
cd godot-battle-merge/web-standalone
python3 -m http.server 8000
# Open http://localhost:8000 in your browser
```

Or use any web server to serve the `web-standalone` directory.

## 📋 What's Included

### Godot 4 Implementation
- **Complete game logic** in GDScript
- **Reusable scene files** (Main.tscn, Block.tscn)
- **Portrait mode** optimized for mobile
- **Web export** configuration ready

### Standalone Web Version
- **HTML5/JavaScript** implementation
- **Identical gameplay** to Godot version
- **No build step** required - runs directly in browser
- **Touch and mouse** input support

## 🎯 Game Features

### Core Mechanics (M2 Block Style)
- ✅ 5 columns × 8 rows grid
- ✅ Tap columns to drop blocks
- ✅ Vertical merge with chain reactions
- ✅ Gravity system for falling blocks

### Defense Layer
- ✅ Virus blocks spawn every 10 seconds
- ✅ Viruses descend after each player turn
- ✅ Block vs Virus combat system
- ✅ Game over when virus reaches bottom

### Economy & Items
- ✅ Coin earning from merges
- ✅ **Swap** (50 coins) - Exchange current/next block
- ✅ **Sledgehammer** (100 coins) - Remove any block
- ✅ **Rainbow Joker** (250 coins) - Guaranteed merge
- ✅ **Column Nuke** (500 coins) - Clear entire column

### Visual Theme
- ✅ Neon Cyberpunk aesthetic
- ✅ Glowing blocks with distinct colors
- ✅ Dark background with vibrant UI
- ✅ Screen shake on merge

## 📁 Directory Structure

```
godot-battle-merge/
├── README.md              # Complete game documentation
├── project.godot          # Godot 4 project file
├── export_presets.cfg     # Web export configuration
│
├── scenes/
│   ├── Main.tscn          # Main game scene
│   └── Block.tscn         # Block prefab
│
├── scripts/
│   ├── Game.gd            # Main game controller (480 lines)
│   └── Block.gd           # Block behavior
│
├── web-standalone/
│   ├── index.html         # Standalone web version
│   └── game.js            # Complete game logic in JS
│
└── assets/
    ├── sprites/           # (Future: Add custom sprites)
    ├── fonts/             # (Future: Add custom fonts)
    └── audio/             # (Future: Add sound effects)
```

## 🚀 How to Use

### Option 1: Play Web Version (Easiest)
```bash
cd godot-battle-merge/web-standalone
python3 -m http.server 8000
# Open http://localhost:8000
```

### Option 2: Open in Godot Editor
```bash
# Install Godot 4.3+ from godotengine.org
cd godot-battle-merge
godot project.godot
# Press F5 to play
```

### Option 3: Export from Godot
1. Open project in Godot
2. Project → Export
3. Select "Web" preset
4. Click "Export All"

## 🎮 How to Play

### Basic Controls
- **Click/Tap** a column to drop the current block
- Blocks fall to the lowest empty position
- Identical blocks merge when vertically adjacent

### Merge System
- Merge creates 2× value (2→4, 4→8, 8→16, etc.)
- Chain reactions occur automatically
- Earn coins and score from merges

### Defense Strategy
- Red blocks are **viruses**
- Build high-value blocks to defend
- If your block ≥ virus value: Virus destroyed
- If virus > block: Both destroyed
- Don't let viruses reach the bottom!

### Using Items
- **Swap**: Swap current and next block
- **Sledgehammer**: Click to remove any block
- **Joker**: Next block becomes a wildcard
- **Nuke**: Click a column to clear it completely

## 🎨 Visual Design

### Neon Cyberpunk Theme
- **Background**: Dark blue gradient (#0a0a1a → #1a0a2e)
- **Grid**: Glowing cyan border with neon effects
- **Blocks**: Vibrant colors with glow:
  - 2: Cyan (#00ffff)
  - 4: Magenta (#ff00ff)
  - 8: Yellow (#ffff00)
  - 16: Green (#00ff00)
  - Higher values: Orange, Purple, Red, Gold...
- **Viruses**: Dark Red (#cc0000)

## 📊 Game Balance

### Coin Formula
```
Base Payout = ⌈n/3⌉ × 2  (where 2^n = merged value)
Chain Bonus = 2× payout (if part of chain)

Examples:
- 1024 (2^10) → ⌈10/3⌉ × 2 = 8 coins
- 256 (2^8) → ⌈8/3⌉ × 2 = 6 coins
```

### Item Costs
| Item | Cost | Strategic Value |
|------|------|-----------------|
| Swap | 50c | Early game flexibility |
| Sledgehammer | 100c | Remove blockers |
| Joker | 250c | Guaranteed high merge |
| Nuke | 500c | Emergency column clear |

### Virus Behavior
- **Spawn Rate**: Every 10 seconds
- **Count**: 1-2 viruses per wave
- **Values**: Randomly 2 or 4
- **Descent**: After each player turn

## 🔧 Technical Details

### Godot Implementation
- **Engine**: Godot 4.3+
- **Language**: GDScript
- **Target**: Mobile (portrait mode)
- **Resolution**: 720×1280 (portrait)
- **Input**: Touch + Mouse support

### Web Implementation
- **Pure HTML5/JavaScript** (no frameworks)
- **Canvas rendering** with glow effects
- **Responsive design** (mobile-friendly)
- **No build process** required

### Grid System
```gdscript
grid[col][row]  # 2D array
- Positive values: Player blocks
- Negative values: Virus blocks
- Zero: Empty cell
```

## 🧪 Testing Checklist

- [x] Block placement in all columns
- [x] Vertical merging
- [x] Chain reactions
- [x] Gravity system
- [x] Virus spawning
- [x] Virus descent
- [x] Defense interactions
- [x] Coin earning
- [x] All 4 items functional
- [x] Game over conditions
- [x] UI updates
- [x] Touch input (web)
- [x] Mouse input (web)

## 🎯 Implemented Requirements

All requirements from the specification have been implemented:

### Section 1: Core Game Structure ✅
- [x] 5×8 grid system
- [x] Block placement and shooting
- [x] Recursive vertical merge logic
- [x] Gravity system

### Section 2: Defense Layer ✅
- [x] Virus spawning (10s timer)
- [x] Virus descent mechanics
- [x] Defense interactions
- [x] Game over on virus reaching bottom

### Section 3: Economy & Items ✅
- [x] Coin payout calculation
- [x] All 4 items implemented

### Section 4: Visual & Juice ✅
- [x] Neon cyberpunk theme
- [x] Screen shake on merge
- [x] UI with score, coins, queue
- [x] Game over panel

## 🚧 Future Enhancements

### Visual Polish (Optional)
- [ ] Particle effects for merges
- [ ] Laser effects for defense
- [ ] Custom sprite assets
- [ ] Animation tweening

### Audio (Optional)
- [ ] Background music
- [ ] Merge sound effects
- [ ] Virus spawn sounds
- [ ] Item use sounds

### Additional Features (Optional)
- [ ] High score persistence
- [ ] Difficulty progression
- [ ] Achievement system
- [ ] Tutorial mode

## 📝 Notes

### Why Two Implementations?

1. **Godot Version**: Full-featured, intended for mobile export
2. **Web Standalone**: Instant playability, no Godot required

Both implementations have identical game logic and can be used interchangeably.

### Cross-Platform Export

The Godot version can export to:
- **Web** (HTML5) - Already configured
- **Android** (APK)
- **iOS** (requires Mac)
- **Desktop** (Windows, Mac, Linux)

## 📞 Support

For issues or questions about this implementation:
1. Check the main README.md in the repository root
2. Review the code comments in Game.gd
3. Test the web standalone version for immediate verification

---

**Game Status**: ✅ **Complete and Playable**

All core mechanics, defense layer, economy, and visual requirements have been successfully implemented in both Godot and web standalone versions.
