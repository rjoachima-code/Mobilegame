# Shape Data Setup Guide

## Overview

ShapeData are ScriptableObjects that define the properties of each geometric shape in the octagon merge game. Each shape has a vertex count, color, sprite, and evolution path.

## Shape Evolution Chain

The game uses a progression system where shapes evolve through merging:

```
Triangle (3) → Square (4) → Pentagon (5) → Hexagon (6) → Heptagon (7) → Octagon (8) → Nonagon (9) → Decagon (10) → ...
```

## Creating Shape Data Assets

### In Unity Editor

1. Right-click in Project window
2. Select `Create > Jacameno > ShapeData`
3. Name it appropriately (e.g., "Triangle_Shape", "Square_Shape", etc.)
4. Configure the properties:

### Shape Data Properties

| Property | Description | Example Values |
|----------|-------------|----------------|
| **Shape Name** | Display name | "Triangle", "Square", "Pentagon" |
| **Vertex Count** | Number of vertices | 3, 4, 5, 6, 7, 8, ... |
| **Icon** | Sprite to display | Geometric shape sprite |
| **Neon Color** | Glow color | Cyan, Pink, Green, Yellow, Orange, Purple |
| **Next Evolution** | Reference to next shape | Link to next ShapeData asset |

### Example Configuration

#### Triangle (Starting Shape)
```
Shape Name: "Triangle"
Vertex Count: 3
Icon: [Triangle Sprite]
Neon Color: Cyan (RGB: 0.2, 0.8, 1.0)
Next Evolution: [Square_Shape reference]
```

#### Square
```
Shape Name: "Square"
Vertex Count: 4
Icon: [Square Sprite]
Neon Color: Pink (RGB: 1.0, 0.4, 0.8)
Next Evolution: [Pentagon_Shape reference]
```

#### Pentagon
```
Shape Name: "Pentagon"
Vertex Count: 5
Icon: [Pentagon Sprite]
Neon Color: Green (RGB: 0.4, 1.0, 0.4)
Next Evolution: [Hexagon_Shape reference]
```

#### Hexagon
```
Shape Name: "Hexagon"
Vertex Count: 6
Icon: [Hexagon Sprite]
Neon Color: Yellow (RGB: 1.0, 0.8, 0.2)
Next Evolution: [Heptagon_Shape reference]
```

#### Heptagon
```
Shape Name: "Heptagon"
Vertex Count: 7
Icon: [Heptagon Sprite]
Neon Color: Orange (RGB: 1.0, 0.5, 0.2)
Next Evolution: [Octagon_Shape reference]
```

#### Octagon (Featured Shape)
```
Shape Name: "Octagon"
Vertex Count: 8
Icon: [Octagon Sprite]
Neon Color: Purple (RGB: 0.8, 0.2, 1.0)
Next Evolution: [Nonagon_Shape reference]
```

#### Nonagon
```
Shape Name: "Nonagon"
Vertex Count: 9
Icon: [Nonagon Sprite]
Neon Color: Blue (RGB: 0.2, 0.4, 1.0)
Next Evolution: [Decagon_Shape reference]
```

#### Decagon
```
Shape Name: "Decagon"
Vertex Count: 10
Icon: [Decagon Sprite]
Neon Color: Red (RGB: 1.0, 0.2, 0.2)
Next Evolution: [Hendecagon_Shape reference] or null for max
```

## Recommended Color Palette (Neon/Cyberpunk)

| Shape | Vertices | Color Name | RGB Values | Hex Code |
|-------|----------|------------|------------|----------|
| Triangle | 3 | Cyan | (0.2, 0.8, 1.0) | #33CCFF |
| Square | 4 | Pink | (1.0, 0.4, 0.8) | #FF66CC |
| Pentagon | 5 | Green | (0.4, 1.0, 0.4) | #66FF66 |
| Hexagon | 6 | Yellow | (1.0, 0.8, 0.2) | #FFCC33 |
| Heptagon | 7 | Orange | (1.0, 0.5, 0.2) | #FF8033 |
| Octagon | 8 | Purple | (0.8, 0.2, 1.0) | #CC33FF |
| Nonagon | 9 | Blue | (0.2, 0.4, 1.0) | #3366FF |
| Decagon | 10 | Red | (1.0, 0.2, 0.2) | #FF3333 |
| Hendecagon | 11 | Teal | (0.2, 1.0, 0.8) | #33FFCC |
| Dodecagon | 12 | Bright Yellow | (1.0, 1.0, 0.4) | #FFFF66 |

## Creating Sprites

### Using Built-in Unity Sprites
Unity provides basic shapes that can be used:
- Circle
- Square
- Triangle
- Hexagon

### Using Vector Graphics
For custom shapes, you can:
1. Create SVG files with geometric shapes
2. Import to Unity as sprites
3. Apply materials with neon glow shaders

### Using Sprite Renderer
In the Shape prefab:
1. Add `SpriteRenderer` component
2. Set sprite to appropriate shape
3. Set color to match ShapeData.NeonColor
4. Optionally add bloom/glow effect

## Linking the Evolution Chain

**Important**: After creating all ShapeData assets, link them together:

1. Open Triangle_Shape asset
2. Set "Next Evolution" field to Square_Shape
3. Open Square_Shape asset
4. Set "Next Evolution" field to Pentagon_Shape
5. Continue for all shapes in order

The final shape (e.g., Dodecagon) can either:
- Have `Next Evolution = null` (stops evolving)
- Loop back to a simpler shape
- Continue to more complex shapes

## Assigning to Spawner

In your game scene:
1. Select InputController GameObject
2. Set "Active Shape Prefab" to your shape prefab
3. On the shape prefab, ensure ShapeController component exists
4. The spawner will randomly assign ShapeData to new shapes

### Weighted Spawning (Optional)

For better game balance, spawn simpler shapes more frequently:
- 50% chance: Triangle (3 vertices)
- 30% chance: Square (4 vertices)
- 15% chance: Pentagon (5 vertices)
- 5% chance: Hexagon (6 vertices)

You can implement this in a custom spawner script or modify InputController.

## Score Values

Shape merge score is automatically calculated:
```
Base Score = 50 + (vertex_count × 10)

Triangle merge = 50 + (3 × 10) = 80 points
Square merge = 50 + (4 × 10) = 90 points
Pentagon merge = 50 + (5 × 10) = 100 points
Hexagon merge = 50 + (6 × 10) = 110 points
Heptagon merge = 50 + (7 × 10) = 120 points
Octagon merge = 50 + (8 × 10) = 130 points
```

Plus combo multipliers, magnet bonuses, and level multipliers!

## Testing

After setup:
1. Play game in Unity
2. Drop a shape into a column
3. Drop an identical shape into the same column
4. Verify they merge into the next evolution
5. Check that score increases correctly
6. Test the full evolution chain

## Troubleshooting

### Shapes not merging
- Check that both shapes have the same ShapeData reference
- Verify GridManager is in scene with proper settings
- Check that MergeMechanic component is assigned

### Wrong shape appears after merge
- Check NextEvolution field on ShapeData
- Ensure evolution chain is linked correctly

### No score awarded
- Verify OctagonScoreManager is in scene
- Check GridManager has scoring callbacks
- Ensure shapes have valid vertex count

### Sprites not showing
- Check Shape prefab has SpriteRenderer
- Verify sprites are assigned to ShapeData
- Check sprite import settings (should be "Sprite 2D")
