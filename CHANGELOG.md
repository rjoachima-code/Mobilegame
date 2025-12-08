# Changelog

All notable changes to JACAMENO will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.9.0] - 2024-12-08 - Beta Release

### 🎉 Beta Testing Ready!

JACAMENO is now ready for beta testing! We welcome feedback and bug reports.

### Added

#### Web Version Features
- **High Score System**: High scores now persist across sessions using localStorage
- **Sound Effects**: Added simple audio feedback for game actions:
  - Move sound (left/right movement)
  - Rotate sound (piece rotation)
  - Drop sound (hard drop)
  - Merge sound (blocks combining)
  - Line clear sound (completing rows)
  - Game over sound
- **New High Score Celebration**: Special animation when beating high score
- **High Score Display**: Shows high score on main menu

#### Documentation
- **Beta Testing Guide** (docs/BETA_TESTING_GUIDE.md): Comprehensive guide for beta testers
- **Contributing Guide** (CONTRIBUTING.md): Guidelines for contributing to the project
- **Bug Report Template**: GitHub issue template for reporting bugs
- **Feature Request Template**: GitHub issue template for suggesting features
- **Changelog**: This file!

#### Android Version
- Already configured and ready for testing (see previous releases)
- Package: `com.jacameno.mobilegame`
- Minimum SDK: API 22 (Android 5.1)
- Target SDK: API 34 (Android 14)

### Changed

#### Web Version Improvements
- Enhanced visual feedback with new high score indicator
- Improved UI with high score display on menu screen
- Better user experience with audio feedback

### Fixed
- None (initial beta release)

### Known Issues

#### Web Version
- Power-ups not yet implemented
- No settings menu (can't toggle sound)
- No achievements system
- No online leaderboard
- Sound effects are simple beeps (no music)

#### Android Version
- Using default app icon (needs custom design)
- No splash screen
- Sound may not work on all devices

#### All Versions
- No save game state (can't resume after closing)
- No color-blind mode
- No alternative control schemes

## [0.8.0] - 2024-12-07 - Android Configuration Complete

### Added
- Complete Android native implementation
- Full documentation suite:
  - Quick Start Guide
  - Build Instructions
  - Deployment Checklist
  - Troubleshooting Guide
  - Readiness Summary

### Changed
- Updated package name to `com.jacameno.mobilegame`
- Updated app name to "JACAMENO"
- Configured native library as `jacameno`

### Technical
- CMakeLists.txt configured with all source files
- GameActivity integration for input handling
- OpenGL ES 3.0 rendering
- Touch input support
- Audio system with fallback
- Full-screen immersive mode

## [0.7.0] - 2024-12-06 - Web Version Launch

### Added
- Complete web version implementation (HTML/CSS/JavaScript)
- Tetris gameplay mechanics
- M2 Block merge system
- Score tracking and leveling
- Keyboard controls (Arrow keys, WASD, Space)
- Touch controls for mobile (swipe and tap)
- Responsive design
- Pause/Resume functionality

### Features
- 10x20 game grid
- 7 tetromino shapes (I, O, T, L, J, S, Z)
- Block values starting at 2, merging to create higher values
- Row clearing when lines are complete
- Ghost piece preview (optional)
- Next piece preview

## [0.6.0] - 2024-12-05 - Unity Implementation

### Added
- Unity project structure
- Core game scripts:
  - GridManager
  - Block
  - Tetromino
  - Spawner
  - InputManager
  - MergeLogic
  - ScoreManager
  - PowerUpManager
  - GameState
  - GameController
  - UIManager
- Unity scenes (MainMenu, Game, GameOver)
- Neon minimal art style
- Power-up system design

## [0.5.0] - 2024-12-04 - Initial Project Setup

### Added
- Project repository structure
- README documentation
- License
- .gitignore configuration
- Initial design documents

---

## Version Legend

- **Major.Minor.Patch** (e.g., 1.0.0)
  - **Major**: Breaking changes, major new features
  - **Minor**: New features, backwards compatible
  - **Patch**: Bug fixes, minor improvements

## Release Status

- **0.x.x**: Beta/Pre-release versions
- **1.x.x**: Stable releases
- **2.x.x**: Major updates

## How to Report Issues

See our [Beta Testing Guide](docs/BETA_TESTING_GUIDE.md) and use our [Bug Report Template](.github/ISSUE_TEMPLATE/bug_report.md).

## Roadmap

### v1.0.0 - Public Release (Target: Q1 2025)
- Fix all critical bugs from beta testing
- Add custom Android app icon
- Implement basic power-ups
- Add sound toggle option
- Polish UI/UX based on feedback

### v1.1.0 - Feature Update
- Additional power-ups
- Achievement system
- Enhanced visual effects
- Music tracks

### v1.2.0 - Social Features
- Online leaderboard
- Share scores
- Daily challenges

### Future Considerations
- iOS version
- Additional game modes
- Multiplayer features
- Cross-platform sync

---

**Current Version**: 0.9.0 (Beta)  
**Status**: Ready for beta testing  
**Feedback**: [Create an issue](https://github.com/rjoachima-code/Mobilegame/issues/new/choose)
