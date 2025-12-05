# JACAMENO

A Unity mobile game combining M2 Block and Tetris mechanics with a neon minimal art style.

## Game Overview

JACAMENO is a unique puzzle game that combines:
- **Tetris-style gameplay**: Falling tetromino shapes that you can move and rotate
- **M2 Block merge mechanics**: When blocks with the same value touch, they merge and double in value
- **Combo system**: Chain merges together for score multipliers
- **Power-ups**: Special abilities to help clear the board

## Features

### Core Gameplay
- 10x20 grid playfield
- 7 tetromino shapes (I, O, T, L, J, S, Z)
- Block values start at 2 and merge to create higher values
- Row clearing when lines are complete
- Ghost piece preview

### Controls
- **Arrow Keys / WASD**: Move left/right, soft drop, rotate
- **Space**: Hard drop
- **Escape / P**: Pause game
- **Touch**: Swipe to move, tap to rotate

### Power-Ups
- **Clear Row**: Removes the bottom row
- **Bomb**: Destroys blocks in a 3x3 area
- **Freeze**: Temporarily stops the current piece
- **Slow Down**: Reduces drop speed
- **Color Bomb**: Removes all blocks of a random value
- **Shuffle**: Randomizes all block values

## Project Structure

```
Assets/
├── Scripts/
│   ├── GridManager.cs      - Manages the 10x20 grid
│   ├── Block.cs            - Individual block behavior
│   ├── Tetromino.cs        - Falling shapes
│   ├── Spawner.cs          - Spawns new tetrominoes
│   ├── InputManager.cs     - Handles player input
│   ├── MergeLogic.cs       - Block merging and combos
│   ├── ScoreManager.cs     - Scoring system
│   ├── PowerUpManager.cs   - Power-up effects
│   ├── GameState.cs        - Game state management
│   ├── GameController.cs   - Main game loop
│   ├── UIManager.cs        - In-game UI
│   ├── MainMenuUI.cs       - Main menu scene
│   └── GameOverUI.cs       - Game over scene
├── Scenes/
│   ├── MainMenu.unity
│   ├── Game.unity
│   └── GameOver.unity
├── Prefabs/
│   └── Block.prefab
├── Materials/
│   ├── NeonBlock.mat
│   ├── GridBackground.mat
│   └── GridLine.mat
└── Sprites/
```

## Requirements

- Unity 2021.3 LTS or later
- TextMesh Pro package

## Getting Started

1. Clone this repository
2. Open the project in Unity
3. Open `Assets/Scenes/MainMenu.unity`
4. Press Play to test the game

## Building

### Android
1. File > Build Settings
2. Select Android platform
3. Configure Player Settings (package name, icons, etc.)
4. Click Build

### iOS
1. File > Build Settings
2. Select iOS platform
3. Configure Player Settings
4. Click Build to generate Xcode project
5. Open in Xcode and build/deploy

## License

This project is for educational purposes.