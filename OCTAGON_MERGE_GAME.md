# JACAMENO - Octagon Merge Game

## Game Concept

JACAMENO is a puzzle game inspired by M2 Block where you merge geometric shapes in octagon-shaped columns. Unlike Tetris, there is NO rotation - shapes simply drop into columns and merge with identical shapes to evolve into more complex forms.

## How to Play

### Basic Mechanics

1. **Drag and Drop**: Drag shapes left/right, then release to drop them into a column
2. **Shape Merging**: When 2+ identical shapes touch in a column, they merge into the next evolution
3. **No Rotation**: Shapes don't rotate - they drop straight down (unlike Tetris)
4. **Octagon Columns**: The game features octagon-shaped pieces in 5 vertical columns

### Shape Evolution

Shapes evolve through geometric progression:
- Triangle (3 vertices) → Square (4 vertices) → Pentagon (5 vertices) → Hexagon (6 vertices) → Heptagon (7 vertices) → Octagon (8 vertices) → and beyond!

Each evolution creates a more complex and valuable shape.

### Magnet Effect

When shapes merge, they can pull in matching shapes from adjacent columns, creating powerful combo chains!

## Scoring System

### Points Breakdown

1. **Base Merge Score**: 50 points + (vertex count × 10)
   - Triangle merge: ~80 points
   - Square merge: ~90 points
   - Pentagon merge: ~100 points
   - Octagon merge: ~130 points

2. **Combo Multiplier**: 1.5× per combo level
   - Consecutive merges within 2 seconds increase combo
   - Combo x2 = 1.5× multiplier
   - Combo x3 = 2.0× multiplier
   - Combo x4 = 2.5× multiplier
   - etc.

3. **Magnet Bonus**: +100 points per extra shape
   - Merging 3 shapes = +100 bonus
   - Merging 4 shapes = +200 bonus
   - Creates exciting multi-column cascade effects!

4. **Level Multiplier**: +10% per level
   - Level 1: 1.0× (base)
   - Level 5: 1.4×
   - Level 10: 1.9×
   - Level 20: 2.9×

5. **Combo Completion Bonus**: 50 × combo count
   - Awarded when combo chain ends (after 2 second timeout)

### Leveling Up

- Gain 1 level for every 1000 points
- Max level: 20
- Level up bonus: 100 × level number
- Each level increases all future scoring!

### Example Scoring

**Simple Triangle Merge (Level 1, No Combo):**
- Base: 50 + (3 × 10) = 80 points

**Octagon Merge with Combo x3 (Level 5):**
- Base: 50 + (8 × 10) = 130 points
- Combo: 130 × 2.0 = 260 points
- Level: 260 × 1.4 = 364 points
- **Total: 364 points**

**4-Shape Magnet Merge (Level 10, Combo x5):**
- Base: 50 + (6 × 10) = 110 points
- Magnet bonus: 110 + 200 = 310 points
- Combo: 310 × 3.0 = 930 points
- Level: 930 × 1.9 = 1767 points
- **Total: 1767 points** + eventual combo end bonus!

## Strategy Tips

1. **Build Combos**: Try to create chain reactions by setting up multiple merge opportunities
2. **Use Magnet Effect**: Position identical shapes in adjacent columns for powerful multi-merges
3. **Plan Ahead**: Think about where shapes will land and what merges they'll create
4. **Keep Columns Balanced**: Don't let one column get too tall
5. **Speed Matters**: Complete merges quickly to maintain combo chains

## Key Differences from Tetris

| Feature | Tetris | JACAMENO (Octagon Merge) |
|---------|--------|--------------------------|
| Pieces | Tetromino blocks | Geometric shapes (octagons, etc.) |
| Rotation | Yes, 4 rotation states | No rotation |
| Goal | Clear rows | Merge and evolve shapes |
| Scoring | Rows cleared | Shape merges + combos |
| Columns | 10 columns | 5 columns |
| Input | Arrow keys + rotate | Drag and drop |
| Gameplay | Fast-paced falling | Strategic shape placement |

## Technical Implementation

### Core Systems

1. **GridManager**: Manages 5 columns and merge detection
2. **ShapeController**: Controls individual shape behavior
3. **ShapeData**: ScriptableObject defining shape properties (vertices, color, evolution)
4. **InputController**: Handles drag-and-drop input
5. **MergeMechanic**: Animates merges and spawns evolved shapes
6. **OctagonScoreManager**: Calculates scores, combos, and levels
7. **OctagonUIManager**: Displays score, combos, and game state

### Files to Use (Jacameno namespace)

- `GridManager.cs` - Column-based grid management
- `ShapeController.cs` - Shape behavior and animation
- `ShapeData.cs` - Shape definitions
- `InputController.cs` - Drag-and-drop controls
- `MergeMechanic.cs` - Merge animations
- `OctagonScoreManager.cs` - Scoring system
- `OctagonUIManager.cs` - UI management

### Files to Ignore (JACAMENO namespace - Tetris-based)

These files are deprecated and represent the old Tetris concept:
- `Tetromino.cs` - Tetris piece with rotation (NOT USED)
- `GameController.cs` (JACAMENO) - Tetris game loop (NOT USED)
- `Spawner.cs` (JACAMENO) - Spawns tetrominoes (NOT USED)
- `MergeLogic.cs` (JACAMENO) - Old merge system (NOT USED)
- `ScoreManager.cs` (JACAMENO) - Tetris scoring (NOT USED)

## Development Roadmap

### Completed ✅
- [x] Column-based shape system
- [x] Shape merging and evolution
- [x] Magnet effect for multi-column merges
- [x] Comprehensive scoring system
- [x] Combo tracking and bonuses
- [x] Level progression

### Planned 🎯
- [ ] More shape types and evolutions
- [ ] Visual effects for merges
- [ ] Sound effects
- [ ] Tutorial mode
- [ ] Achievements system
- [ ] Online leaderboards
- [ ] Power-ups specific to shape merging

## Credits

Game concept inspired by M2 Block and 2048-style merge mechanics.
