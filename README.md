JACAMENO — Native Android Puzzle Game (C++ / GameActivity / OpenGL ES 3.0)

This repository contains a native Android game written in C++ using Google GameActivity and OpenGL ES 3.0.

## 🚀 Beta Testing Now Open!

**JACAMENO is ready for beta testing!** We welcome your feedback to make the game even better.

- 📖 **[Beta Testing Guide](docs/BETA_TESTING_GUIDE.md)** - How to help test the game
- 🐛 **[Report Bugs](https://github.com/rjoachima-code/Mobilegame/issues/new?template=bug_report.md)** - Found an issue? Let us know!
- 💡 **[Suggest Features](https://github.com/rjoachima-code/Mobilegame/issues/new?template=feature_request.md)** - Have ideas? We'd love to hear them!
- 📝 **[Changelog](CHANGELOG.md)** - See what's new in version 0.9.0

## 🎮 Play Now

**Web Version Available!** Play JACAMENO directly in your browser:
- 🌐 **[Play Online](https://rjoachima-code.github.io/Mobilegame/)** - Live demo on GitHub Pages!
- 📱 **[Open web/index.html](web/index.html)** in any browser for local play
- 🚀 **No installation required** - works on desktop and mobile
- 🎯 **Quick Start**: See [web/README.md](web/README.md) for instructions
- ⭐ **NEW**: High scores now save automatically!
- 🔊 **NEW**: Sound effects added!

## 🎮 Game Overview

**JACAMENO** is an octagon-shaped puzzle game inspired by M2 Block, where you merge geometric shapes to create increasingly complex forms!

### Game Concept
- **No Tetris mechanics**: No rotation, no falling tetrominoes
- **M2 Block style**: Drag and drop shapes into columns
- **Octagon shapes**: Merge triangles → squares → pentagons → hexagons → octagons!
- **Strategic merging**: Create combo chains and use the magnet effect
- **Scoring system**: Points based on shape complexity, combos, and level

📖 **[Complete Game Guide](OCTAGON_MERGE_GAME.md)** - Learn strategies and scoring details!

### Core Gameplay
1. Drag shapes horizontally across the play area
2. Drop shapes into one of 5 columns
3. Identical shapes merge automatically when they touch
4. Merged shapes evolve into more complex geometric forms
5. Build combos by creating chain reactions
6. Use the magnet effect to pull shapes from adjacent columns!

### Project Structure
```
Assets/
├── Scripts/
│   ├── GridManager.cs           - Column-based grid (5 columns)
│   ├── ShapeController.cs       - Individual shape behavior
│   ├── ShapeData.cs             - Shape definitions (octagon, etc.)
│   ├── InputController.cs       - Drag-and-drop input
│   ├── MergeMechanic.cs         - Merge animations
│   ├── OctagonScoreManager.cs   - Scoring and leveling
│   ├── OctagonUIManager.cs      - Game UI
│   ├── GameState.cs             - Game state management
│   ├── MainMenuUI.cs            - Main menu
│   └── GameOverUI.cs            - Game over screen
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

- Unity 2021.3 LTS or later (for Unity-based development)
- TextMesh Pro package (for Unity)
- **OR** Android Studio (for native C++ development - current implementation)

## Project Status

**This project currently has THREE implementations:**

1. **Web Version** (web/ folder) - HTML5/JavaScript implementation that runs in any browser
   - ✅ **Ready to play now!** Just open `web/index.html`
   - ✅ Works on desktop and mobile browsers
   - ✅ No installation required
   - See [web/README.md](web/README.md) for details

2. **Unity Implementation** (Assets/ folder) - Original design with Unity scenes and scripts
   - For Unity-based development and builds

3. **Native Android Implementation** (app/ folder) - C++ native implementation using Android Game SDK
   - ✅ **Ready for testing** in Android Studio
   - For high-performance native Android builds

## Quick Start Options

### 🌐 Play in Browser (Easiest - No Installation)

```bash
# Open the web version
cd web
python3 -m http.server 8000
# Then open: http://localhost:8000
```

Or simply open `web/index.html` in your browser!

See [web/README.md](web/README.md) for full instructions.

### 📱 Testing on Android (Native Implementation)

**⚡ Fastest way to test the game on Android devices:**

See [docs/QUICK_START_ANDROID.md](docs/QUICK_START_ANDROID.md) for a 5-minute setup guide.

**Summary:**
1. Install Android Studio
2. Open this project in Android Studio
3. Connect an Android device or start an emulator
4. Click Run ▶
5. Game will install and launch!

For complete build instructions, see [docs/ANDROID_BUILD_INSTRUCTIONS.md](docs/ANDROID_BUILD_INSTRUCTIONS.md).

For deployment checklist, see [docs/ANDROID_DEPLOYMENT_CHECKLIST.md](docs/ANDROID_DEPLOYMENT_CHECKLIST.md).

## Getting Started (Unity)

1. Clone this repository
2. Open the project in Unity
3. Open `Assets/Scenes/MainMenu.unity`
4. Press Play to test the game

## Building

### Android Native Build (Current Implementation - READY FOR TESTING)

**The project is now configured and ready for Android testing!**

#### What's Been Configured:
- ✅ Package name: `com.jacameno.mobilegame`
- ✅ App name: JACAMENO
- ✅ Native library: `jacameno` (C++ implementation)
- ✅ Minimum SDK: API 22 (Android 5.1 - covers 99% of devices)
- ✅ Target SDK: API 34 (Android 14)
- ✅ All source files properly configured in CMakeLists.txt
- ✅ MainActivity and native bridge set up
- ✅ Touch input handling via GameActivity
- ✅ Audio support with fallback
- ✅ Full-screen immersive mode

#### Build in Android Studio:
1. Open the project root in Android Studio
2. Let Gradle sync (may take a few minutes first time)
3. Connect device or start emulator
4. Click Run ▶
5. APK builds, installs, and launches automatically

See [docs/QUICK_START_ANDROID.md](docs/QUICK_START_ANDROID.md) for detailed walkthrough.

#### Build from Command Line:
```bash
# Debug APK
./gradlew assembleDebug

# Release APK (requires signing configuration)
./gradlew assembleRelease

# Install to connected device
./gradlew installDebug

# Build and install
./gradlew installDebug
```

Output APK location: `app/build/outputs/apk/debug/app-debug.apk`

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

2) Build debug APK:

```powershell
cd "C:\Users\Ve7gasKnights\AndroidStudioProjects\MyApplication"
.\gradlew :app:assembleDebug -x lint
```

3) Install on a connected device/emulator:

```powershell
adb devices
adb install -r .\app\build\outputs\apk\debug\app-debug.apk
adb logcat -v time | Select-String -Pattern "GameActivity","myapplication","jni"
```

4) Run unit tests (native tests run at startup via initializeNativeCode which prints test output to logcat).

## Android Testing Documentation

Comprehensive guides are available in the `docs/` folder:

- **📘 [Quick Start Guide](docs/QUICK_START_ANDROID.md)** - Get up and running in 5 minutes
- **📗 [Complete Build Instructions](docs/ANDROID_BUILD_INSTRUCTIONS.md)** - All build options and configurations
- **📙 [Deployment Checklist](docs/ANDROID_DEPLOYMENT_CHECKLIST.md)** - Pre-release requirements
- **📕 [Readiness Summary](docs/ANDROID_READINESS.md)** - Current status and what's completed
- **🔧 [Troubleshooting Guide](docs/TROUBLESHOOTING.md)** - Common issues and solutions

### What's Ready for Android Testing

✅ **Fully Configured:**
- Package name: `com.jacameno.mobilegame`
- Native C++ game implementation with GameActivity
- Touch input handling
- Audio support
- OpenGL ES rendering
- Full-screen immersive mode
- Build scripts and Gradle configuration

✅ **Ready to Build:**
```bash
# Open in Android Studio and click Run ▶
# Or use command line:
./gradlew assembleDebug
```

✅ **Documentation:**
- Complete setup guides
- Build instructions
- Deployment checklist
- Troubleshooting guide

**Next Step:** Follow the [Quick Start Guide](docs/QUICK_START_ANDROID.md) to build and test!

## Development Tips
1. File > Build Settings
2. Select iOS platform
3. Configure Player Settings
4. Click Build to generate Xcode project
5. Open in Xcode and build/deploy

## Development Tips

```powershell
cd "C:\Users\Ve7gasKnights\AndroidStudioProjects\MyApplication"
git checkout -b feat/native-hud-input-audio
git add -A
git commit -m "Native: audio JNI, input mapping, SDF HUD shader, shader uniform helper"
git push origin feat/native-hud-input-audio
```

Notes & next steps
- The renderer now loads a font atlas bitmap from Java (MainActivity.createFontAtlas) and uses an SDF-friendly shader; for best results replace the bitmap with a true SDF atlas.
- Audio uses `NativeAudioBridge` (Java SoundPool/ToneGenerator). For low-latency native audio you can integrate Oboe and update CMake.
- CI: a sample GitHub Actions workflow is included in `.github/workflows/android-ci.yml` to build the app in CI. Ensure the runner has Android SDK/NDK or set appropriate environment variables in your repo secrets.

If you want, I can now:
- Install and run the APK on a device/emulator (you need to connect one and confirm), or
- Prepare a branch and attempt a push (local credentials needed), or
- Continue with native Oboe integration and a proper SDF atlas generator.

