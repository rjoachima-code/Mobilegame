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
├── Plugins/
│   └── Android/            - Android Studio export templates
│       ├── mainTemplate.gradle
│       ├── launcherTemplate.gradle
│       ├── baseProjectTemplate.gradle
│       ├── gradleTemplate.properties
│       └── settingsTemplate.gradle
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

### Android (Direct APK Build)
1. File > Build Settings
2. Select Android platform
3. Configure Player Settings (package name, icons, etc.)
4. Click Build

### Android Studio Integration

This project is configured for Android Studio development and testing. Follow these steps to export and run the game in Android Studio:

#### Prerequisites
- **Android Studio** (Arctic Fox 2020.3.1 or later recommended)
- **Android SDK** (API Level 22+ for minimum, API 33+ for target)
- **Android NDK** (if using IL2CPP scripting backend)
- **Unity 2021.3 LTS or later** with Android Build Support module

#### Exporting to Android Studio

1. **Open Unity Project**
   - Open this project in Unity Editor

2. **Switch to Android Platform**
   - Go to `File > Build Settings`
   - Select `Android` from the platform list
   - Click `Switch Platform` (wait for asset reimport if needed)

3. **Configure Player Settings**
   - Click `Player Settings...` in Build Settings window
   - Verify the following settings:
     - **Company Name**: JACAMENO
     - **Product Name**: JACAMENO
     - **Package Name**: `com.jacameno.mobilegame`
     - **Minimum API Level**: Android 5.1 'Lollipop' (API level 22)

4. **Enable Export Project**
   - In Build Settings, check the box **"Export Project"**
   - This exports a Gradle project instead of building an APK directly

5. **Export the Project**
   - Click `Export` button
   - Choose a destination folder (e.g., `AndroidStudioBuild/`)
   - Wait for Unity to generate the Gradle project

6. **Open in Android Studio**
   - Launch Android Studio
   - Select `File > Open`
   - Navigate to and select the exported folder
   - Click `OK` to open the project
   - Wait for Gradle sync to complete

#### Running in Android Studio

1. **Connect Device or Start Emulator**
   - Connect an Android device via USB with USB debugging enabled, OR
   - Start an Android Emulator (AVD) from Android Studio

2. **Build and Run**
   - Click the green **Run** button (▶) in Android Studio, OR
   - Use `Run > Run 'launcher'` from the menu
   - Select your target device/emulator
   - Wait for the build and installation

3. **Debugging**
   - Use Android Studio's **Logcat** to view Unity debug logs
   - Filter by tag `Unity` to see game-specific logs
   - Set breakpoints in Java/Kotlin code if needed

#### Project Structure (After Export)

```
ExportedProject/
├── launcher/                 # Main application module
│   ├── src/
│   │   └── main/
│   │       ├── AndroidManifest.xml
│   │       └── res/
│   └── build.gradle
├── unityLibrary/             # Unity game library
│   ├── libs/                 # Unity native libraries
│   ├── src/
│   │   └── main/
│   │       ├── assets/       # Game assets
│   │       ├── jniLibs/      # Native .so libraries
│   │       └── java/
│   └── build.gradle
├── build.gradle              # Root build script
├── settings.gradle           # Project settings
└── gradle.properties         # Gradle configuration
```

#### Custom Gradle Templates

This project includes custom Gradle templates in `Assets/Plugins/Android/`:
- `mainTemplate.gradle` - Unity library module configuration
- `launcherTemplate.gradle` - Launcher application configuration
- `baseProjectTemplate.gradle` - Root project configuration
- `gradleTemplate.properties` - Gradle properties
- `settingsTemplate.gradle` - Project settings

These templates are automatically used when exporting to Android Studio, ensuring consistent build configuration.

#### Troubleshooting

**Gradle Sync Failed**
- Ensure Android Studio has the correct SDK path configured
- Check `File > Project Structure > SDK Location`
- Try `File > Sync Project with Gradle Files`

**Build Errors**
- Update Android Gradle Plugin if prompted
- Ensure NDK is installed if using IL2CPP
- Check minimum SDK version compatibility

**App Crashes on Launch**
- Check Logcat for error messages (filter by `Unity`)
- Verify all required scenes are included in Build Settings
- Ensure TextMesh Pro resources are imported

**Performance Issues on Emulator**
- Use a physical device for better performance
- Enable hardware acceleration in emulator settings
- Use x86_64 system images for Intel/AMD processors

### iOS
1. File > Build Settings
2. Select iOS platform
3. Configure Player Settings
4. Click Build to generate Xcode project
5. Open in Xcode and build/deploy

## Development Tips

### Testing on Android Device
For the fastest iteration cycle:
1. Connect Android device via USB
2. Enable USB Debugging on device
3. In Unity: `File > Build And Run`
4. Unity will build and deploy directly to device

### Using Android Studio for Advanced Debugging
Android Studio export is useful when you need:
- Native Android code debugging
- Profiling with Android Studio tools
- Integration with Android-specific features
- Custom Android manifest modifications
- Third-party Android SDK integration

## License

This project is for educational purposes.