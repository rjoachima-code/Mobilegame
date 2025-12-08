# Migration Guide: From Tetris to Octagon Merge Game

## Overview

This document helps you migrate from the old Tetris-based implementation (JACAMENO namespace) to the new octagon merge game (Jacameno namespace).

## Why the Change?

The game has been redesigned to remove Tetris concepts (falling blocks, rotation) and implement an M2 Block-style merge game with octagon-shaped columns. This provides:
- Simpler, more strategic gameplay
- Better mobile experience (drag and drop instead of rotation)
- Unique octagon shape evolution system
- More engaging scoring with combos and magnet effects

## Namespace Changes

### Old System (DEPRECATED - JACAMENO)
- All uppercase namespace: `JACAMENO`
- Tetris-based mechanics
- Files marked with `[System.Obsolete]` attribute

### New System (CURRENT - Jacameno)
- Proper case namespace: `Jacameno`
- Octagon merge mechanics
- Active development

## File Mapping

| Old File (JACAMENO) | Status | New File (Jacameno) | Purpose |
|---------------------|--------|---------------------|---------|
| `Tetromino.cs` | ❌ DEPRECATED | `ShapeController.cs` | Controls individual shapes |
| `GameController.cs` | ❌ DEPRECATED | `InputController.cs` | Game control and input |
| `Spawner.cs` | ❌ DEPRECATED | `InputController.cs` | Spawning shapes |
| `MergeLogic.cs` | ❌ DEPRECATED | `GridManager.cs` + `MergeMechanic.cs` | Merge detection and animation |
| `ScoreManager.cs` | ❌ DEPRECATED | `OctagonScoreManager.cs` | Scoring system |
| `UIManager.cs` | ❌ DEPRECATED | `OctagonUIManager.cs` | UI management |
| `Block.cs` | ⚠️ LEGACY | `ShapeController.cs` | Shape representation |
| `GridManager.cs` (JACAMENO) | ❌ DEPRECATED | `GridManager.cs` (Jacameno) | Grid management |
| `GameState.cs` | ✅ KEEP | `GameState.cs` | Game state (shared) |
| `PowerUpManager.cs` | ⚠️ TO BE UPDATED | TBD | Power-ups (needs adaptation) |

## Key Concept Changes

### 1. No More Rotation
**Old (Tetris):**
```csharp
// Rotate tetromino
tetromino.RotateClockwise();
```

**New (Octagon Merge):**
```csharp
// Shapes don't rotate - they just drop into columns
// No rotation needed!
```

### 2. Column-Based Gameplay
**Old (Tetris):**
```csharp
// 10x20 grid with falling pieces
GridManager.Instance.GetBlock(x, y);
```

**New (Octagon Merge):**
```csharp
// 5 columns with stacking shapes
GridManager.Instance.DropShape(columnIndex, shape);
```

### 3. Drag and Drop Input
**Old (Tetris):**
```csharp
// Arrow keys and rotate button
if (Input.GetKeyDown(KeyCode.LeftArrow))
    tetromino.Move(Vector2Int.left);
if (Input.GetKeyDown(KeyCode.UpArrow))
    tetromino.RotateClockwise();
```

**New (Octagon Merge):**
```csharp
// Drag shape horizontally, release to drop
InputController handles all drag-and-drop automatically
```

### 4. Shape Evolution
**Old (Tetris):**
```csharp
// Fixed tetromino shapes (I, O, T, L, J, S, Z)
// No evolution
```

**New (Octagon Merge):**
```csharp
// Shapes evolve through merging
ShapeData triangleData;  // 3 vertices
// Merge two triangles → get square (4 vertices)
// Merge two squares → get pentagon (5 vertices)
// etc.
ShapeData nextShape = currentShape.Data.NextEvolution;
```

### 5. Scoring System
**Old (Tetris):**
```csharp
// Score based on rows cleared
ScoreManager.Instance.AddRowClearScore(rowsCleared, combo);
```

**New (Octagon Merge):**
```csharp
// Score based on shape merges, combos, and complexity
OctagonScoreManager.Instance.AddMergeScore(mergedShape, shapesInvolved);
// Automatically handles:
// - Base score + vertex multiplier
// - Combo multiplier
// - Magnet bonus (multi-shape merges)
// - Level multiplier
```

### 6. UI Updates
**Old (Tetris):**
```csharp
// Tetris-specific UI
UIManager.Instance.UpdateScoreDisplay(score);
UIManager.Instance.ShowNextPiece(nextTetromino);
```

**New (Octagon Merge):**
```csharp
// Octagon merge UI (automatic via events)
OctagonUIManager.Instance.ShowGameOver();
// Score, combo, level all update automatically via events
```

## Migration Steps

### For Unity Developers

1. **Stop using JACAMENO namespace files**
   - Remove references to `Tetromino`, old `GameController`, old `Spawner`, etc.
   - Unity will show obsolete warnings to help identify usage

2. **Update Scene References**
   - Remove old manager GameObjects (JACAMENO namespace)
   - Add new manager GameObjects:
     - `GridManager` (Jacameno namespace)
     - `InputController` (Jacameno namespace)
     - `MergeMechanic`
     - `OctagonScoreManager`
     - `OctagonUIManager`
     - `GameState` (shared)

3. **Update Prefabs**
   - Replace Block prefab with Shape prefab
   - Ensure prefab has `ShapeController` component
   - Set up ShapeData ScriptableObjects for each shape type

4. **Update UI**
   - Connect `OctagonUIManager` to UI elements
   - Remove "Next Piece" preview (not needed)
   - Add combo display panel
   - Add level progress bar

5. **Test**
   - Verify drag-and-drop input works
   - Test shape merging and evolution
   - Verify scoring and combos
   - Test UI updates

### For Code References

Search and replace these patterns:

```csharp
// OLD
using JACAMENO;
Tetromino tetromino;
GameController.Instance
ScoreManager.Instance
UIManager.Instance
MergeLogic.Instance

// NEW
using Jacameno;
ShapeController shape;
InputController // or direct scene reference
OctagonScoreManager.Instance
OctagonUIManager.Instance
GridManager.Instance // for merges
```

## Testing Checklist

After migration:
- [ ] Game starts without errors
- [ ] Can drag and drop shapes into columns
- [ ] Shapes merge correctly when identical
- [ ] Evolved shapes appear with correct geometry
- [ ] Magnet effect pulls shapes from adjacent columns
- [ ] Score increases with merges
- [ ] Combo counter works
- [ ] Level progression works
- [ ] UI displays all information
- [ ] Game over triggers correctly
- [ ] High score saves/loads

## Common Issues

### Issue: "Type or namespace 'Tetromino' could not be found"
**Solution:** Update to use `ShapeController` from Jacameno namespace

### Issue: "No rotation happening"
**Solution:** This is correct! The new game has no rotation.

### Issue: "Shapes not merging"
**Solution:** Check that:
- `GridManager` (Jacameno) is in scene
- `MergeMechanic` component is assigned
- Shapes have `ShapeData` assigned
- `ShapeData` has `NextEvolution` set

### Issue: "Score not updating"
**Solution:** Ensure `OctagonScoreManager` is in scene and `GridManager` has proper scoring callbacks

### Issue: "Compilation errors about obsolete types"
**Solution:** These are warnings to help migration. Update to new types from Jacameno namespace.

## Support

For questions or issues:
1. Read [OCTAGON_MERGE_GAME.md](OCTAGON_MERGE_GAME.md) for gameplay details
2. Check this migration guide
3. Review example scenes (if available)
4. Open an issue on GitHub

## Deprecation Timeline

- **Now**: Old files marked obsolete with warnings
- **Future**: Old files may be moved to `/Deprecated` folder
- **Later**: Old files may be removed entirely

It's recommended to migrate as soon as possible.
