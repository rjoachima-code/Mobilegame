# JACAMENO Beta Testing Guide

Welcome to the JACAMENO beta testing program! Thank you for helping us improve the game before its official release.

## 📋 Table of Contents

- [Getting Started](#getting-started)
- [What to Test](#what-to-test)
- [How to Report Issues](#how-to-report-issues)
- [Testing Checklist](#testing-checklist)
- [Known Issues](#known-issues)
- [Feedback Channels](#feedback-channels)

## 🚀 Getting Started

### Web Version (Easiest - Start Here!)

1. **Open the game**: Navigate to `web/index.html` in your browser
   
2. **For local testing**:
   ```bash
   cd web
   python3 -m http.server 8000
   # Then open: http://localhost:8000
   ```

3. **Mobile testing**: Open the same URL on your mobile device

### Android Version

Follow the [Quick Start Android Guide](QUICK_START_ANDROID.md) to build and install the Android version.

## 🎯 What to Test

### Core Gameplay (Priority 1)

1. **Basic Controls**
   - Keyboard controls (Arrow keys, WASD, Space)
   - Touch controls (swipe, tap)
   - Pause/Resume functionality

2. **Game Mechanics**
   - Piece movement (left, right, down)
   - Piece rotation
   - Hard drop
   - Block merging (same values combine)
   - Line clearing
   - Scoring system

3. **Difficulty Progression**
   - Speed increases with level
   - Level progression (every 10 lines)
   - Game becomes challenging but fair

### User Interface (Priority 2)

1. **Main Menu**
   - Start button works
   - High score displays correctly
   - Controls information is clear

2. **In-Game UI**
   - Score updates in real-time
   - Level and lines display correctly
   - Next piece preview works
   - Pause button accessible

3. **Game Over Screen**
   - Final score displays
   - High score updates and saves
   - "New High Score" message when applicable
   - Play Again and Menu buttons work

### Features (Priority 3)

1. **High Score System**
   - High score persists across sessions
   - Updates when beaten
   - Displays on menu screen

2. **Sound Effects** (Web version)
   - Move sound plays
   - Rotate sound plays
   - Drop sound plays
   - Merge sound plays
   - Line clear sound plays
   - Game over sound plays
   - Sounds can be heard but not too loud

3. **Responsiveness**
   - Game works on different screen sizes
   - Mobile layout adjusts properly
   - Touch targets are large enough

## 🐛 How to Report Issues

### What Makes a Good Bug Report

Include the following information:

1. **Title**: Clear, specific description
   - ✅ Good: "Rotate button doesn't work after pausing on iPhone"
   - ❌ Bad: "Game broken"

2. **Steps to Reproduce**:
   ```
   1. Start a new game
   2. Pause the game using P key
   3. Resume the game
   4. Try to rotate a piece
   5. Rotation doesn't work
   ```

3. **Expected Behavior**: What should happen
   - "Piece should rotate 90 degrees clockwise"

4. **Actual Behavior**: What actually happens
   - "Piece doesn't rotate at all"

5. **Environment**:
   - Browser: Chrome 120 / Safari 17 / Firefox 121
   - OS: Windows 11 / macOS 14 / Android 13
   - Device: Desktop / iPhone 14 / Samsung Galaxy S23
   - Screen size: 1920x1080 / Mobile

6. **Severity**:
   - **Critical**: Game crashes or becomes unplayable
   - **High**: Major feature doesn't work
   - **Medium**: Feature works but has issues
   - **Low**: Minor visual or cosmetic issue

### Where to Report

- **GitHub Issues**: [Create a new issue](https://github.com/rjoachima-code/Mobilegame/issues/new)
- **Use the bug report template** (see ISSUE_TEMPLATE.md)

## ✅ Testing Checklist

### First Play Session

- [ ] Game loads without errors
- [ ] Can start a new game
- [ ] All controls respond
- [ ] Blocks fall automatically
- [ ] Can complete a line
- [ ] Merging works when blocks touch
- [ ] Game over triggers correctly
- [ ] Can restart the game

### Extended Testing

- [ ] Play for 10+ minutes
- [ ] Reach level 5+
- [ ] Clear 20+ lines
- [ ] Test all tetromino shapes (I, O, T, L, J, S, Z)
- [ ] Test pause/resume multiple times
- [ ] Close and reopen (high score persists)
- [ ] Try on different devices
- [ ] Test on different browsers

### Edge Cases

- [ ] Rotate piece against wall
- [ ] Rotate piece near floor
- [ ] Fill grid to the top
- [ ] Multiple merges in one drop
- [ ] Pause during piece drop
- [ ] Rapid key presses
- [ ] Switch between keyboard and touch
- [ ] Minimize/restore window
- [ ] Lose internet connection (web version)

### Performance Testing

- [ ] Game runs smoothly (no lag)
- [ ] Frame rate stays consistent
- [ ] No memory leaks (play for 30+ minutes)
- [ ] Battery usage is reasonable (mobile)
- [ ] Game doesn't overheat device
- [ ] Loading time is acceptable

### Usability Testing

- [ ] Controls are intuitive
- [ ] Objective is clear
- [ ] Scoring system makes sense
- [ ] Visual feedback is helpful
- [ ] Game difficulty feels balanced
- [ ] Tutorial/instructions are clear
- [ ] Error messages are helpful
- [ ] Can recover from mistakes

## 🎮 Specific Test Scenarios

### Scenario 1: Quick Play Session

1. Start the game
2. Play for 5 minutes
3. Get game over
4. Check if score was saved
5. Start a new game immediately
6. Try to beat your score

**What to check**: Does everything work smoothly for a quick session?

### Scenario 2: Extended Play

1. Start the game
2. Play for 20+ minutes
3. Reach level 10+
4. Test all controls throughout

**What to check**: Does performance degrade? Any bugs appear over time?

### Scenario 3: Interruption Handling

1. Start the game
2. Minimize the browser/app
3. Wait 5 minutes
4. Return to the game
5. Continue playing

**What to check**: Does the game handle interruptions gracefully?

### Scenario 4: Mobile Specific

1. Play on mobile device
2. Rotate device (portrait/landscape)
3. Receive a notification
4. Take a phone call
5. Return to game

**What to check**: Mobile-specific issues and interruptions

## ⚠️ Known Issues

### Current Limitations

1. **Web Version**:
   - No power-ups implemented yet
   - Sound effects are simple beeps (no music)
   - No achievements system
   - No online leaderboard

2. **Android Version**:
   - Using default app icon
   - No splash screen
   - Sound may not work on all devices

3. **All Versions**:
   - No save game state (can't resume after closing)
   - No settings menu (sound toggle, etc.)
   - No color-blind mode
   - No alternative control schemes

### Won't Fix (By Design)

- Ghost piece preview (deliberately not included)
- Hold piece feature (not in scope)
- 7-bag randomization (using true random)

## 📊 What We're Looking For

### Critical Questions

1. **Is the game fun?**
   - Would you play it again?
   - Would you recommend it to others?
   - What's the most enjoyable part?

2. **Is it too easy or too hard?**
   - Does difficulty increase appropriately?
   - Can you reach level 5 on your first try?
   - Does it stay challenging?

3. **Are the controls intuitive?**
   - Did you figure out controls without reading instructions?
   - Any control frustrations?
   - Better control scheme suggestions?

4. **Is the merge mechanic clear?**
   - Did you understand how merging works?
   - Is it satisfying when blocks merge?
   - Does it add to the gameplay?

5. **Does it work on your device?**
   - Any performance issues?
   - Does it fit your screen?
   - Touch targets big enough?

## 💡 Providing Feedback

### Feedback Template

```markdown
## Overall Impression
[Your general thoughts about the game]

## What Worked Well
- Feature 1: [why it worked]
- Feature 2: [why it worked]

## What Needs Improvement
- Issue 1: [description and suggestion]
- Issue 2: [description and suggestion]

## Suggestions
- Idea 1: [description]
- Idea 2: [description]

## Technical Issues
- Bug 1: [see bug report format above]
- Bug 2: [see bug report format above]

## Testing Environment
- Device: [device name]
- OS: [operating system and version]
- Browser: [browser and version]
- Screen Size: [resolution]

## Playtime
- Duration: [how long you played]
- Sessions: [number of times you played]
- Highest Score: [your best score]
- Highest Level: [highest level reached]
```

## 📞 Feedback Channels

1. **GitHub Issues**: For bugs and technical issues
   - [Create a new issue](https://github.com/rjoachima-code/Mobilegame/issues/new)

2. **GitHub Discussions**: For general feedback and suggestions
   - [Start a discussion](https://github.com/rjoachima-code/Mobilegame/discussions)

3. **Email**: For detailed feedback or private concerns
   - [Contact information here]

## 🎁 Thank You!

Your feedback is invaluable in making JACAMENO the best game it can be. Every bug report, suggestion, and comment helps us improve.

### Beta Tester Recognition

All beta testers who provide substantial feedback will be:
- Credited in the game (optional)
- Listed in CONTRIBUTORS.md
- Given early access to future updates

### Testing Rewards

- Find a critical bug: Get mentioned in release notes
- Provide comprehensive feedback: Special beta tester badge
- Most helpful tester: Early access to premium features (if any)

## 🔄 Testing Updates

This guide will be updated as we fix bugs and add features. Check back regularly for:
- New features to test
- Fixed issues
- Updated known issues list
- New testing scenarios

**Current Beta Version**: 0.9.0  
**Last Updated**: December 2024  
**Next Update**: After initial beta feedback

---

**Happy Testing! 🎮**

For questions about beta testing, see our [FAQ](TROUBLESHOOTING.md) or reach out through GitHub Issues.
