# Contributing to JACAMENO

Thank you for your interest in contributing to JACAMENO! This document provides guidelines for beta testers and contributors.

## 🎮 Beta Testing

### How to Get Started

1. **Read the Beta Testing Guide**: See [docs/BETA_TESTING_GUIDE.md](../docs/BETA_TESTING_GUIDE.md)
2. **Play the game**: Try both web and Android versions if possible
3. **Report issues**: Use our bug report template
4. **Provide feedback**: Share your thoughts via GitHub Discussions

### What We Need

- **Bug reports**: Help us find and fix issues
- **Usability feedback**: Tell us what works and what doesn't
- **Feature suggestions**: Share your ideas for improvements
- **Performance testing**: Report lag, crashes, or slowdowns
- **Device testing**: Test on various devices and browsers

## 🐛 Reporting Issues

### Before Reporting

1. **Search existing issues**: Check if someone already reported it
2. **Try the latest version**: Make sure you're testing the current version
3. **Reproduce the issue**: Confirm you can make it happen again

### Creating a Bug Report

1. Go to [Issues](https://github.com/rjoachima-code/Mobilegame/issues)
2. Click "New Issue"
3. Select "Bug Report" template
4. Fill out all sections completely
5. Add screenshots/videos if helpful
6. Submit the issue

### Issue Severity Guidelines

- **Critical**: Game crashes, data loss, completely unplayable
- **High**: Major feature broken, significant gameplay issues
- **Medium**: Feature works but has problems, moderate impact
- **Low**: Cosmetic issues, minor inconveniences

## 💡 Suggesting Features

### Good Feature Suggestions Include

1. **Clear description**: What the feature does
2. **Use case**: Why it's needed
3. **Examples**: How it would work in practice
4. **Mockups**: Visual representation (if applicable)

### Feature Request Process

1. Check existing feature requests first
2. Create a new issue with "Feature Request" template
3. Engage in discussion about the feature
4. Be open to alternative solutions

## 🔧 Contributing Code (Advanced)

If you're a developer interested in contributing code:

### Development Setup

#### Web Version
```bash
cd web
python3 -m http.server 8000
# Edit HTML/CSS/JS files directly
```

#### Android Version
```bash
# See docs/ANDROID_BUILD_INSTRUCTIONS.md
./gradlew assembleDebug
```

### Code Contribution Guidelines

1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/your-feature-name`
3. **Make your changes**: Follow existing code style
4. **Test thoroughly**: Ensure nothing breaks
5. **Commit with clear messages**: Describe what and why
6. **Push to your fork**: `git push origin feature/your-feature-name`
7. **Create a Pull Request**: Describe your changes

### Code Style

#### JavaScript (Web Version)
- Use ES6+ features
- Clear variable names
- Comment complex logic
- Keep functions focused and small
- Consistent indentation (4 spaces)

#### C++ (Android Version)
- Follow existing code structure
- Use meaningful variable names
- Add comments for non-obvious code
- Keep memory management clean

#### Commits
- Use present tense: "Add feature" not "Added feature"
- Be descriptive: "Fix rotation bug when piece near wall"
- Reference issues: "Fix #123: Rotation collision detection"

## 📝 Documentation

### Improving Documentation

Documentation contributions are always welcome:
- Fix typos or unclear instructions
- Add missing information
- Improve examples
- Update outdated content

### Documentation Guidelines

- Write clearly and concisely
- Use proper markdown formatting
- Include code examples where appropriate
- Test all instructions yourself
- Keep it beginner-friendly

## 🎨 Design Contributions

### Areas for Design Contributions

- App icon design
- Splash screen
- UI/UX improvements
- Color scheme suggestions
- Animation ideas

### Submitting Design Work

1. Create a discussion post with your designs
2. Include mockups/screenshots
3. Explain your design choices
4. Be open to feedback and iteration

## 🧪 Testing Contributions

### Help with Testing

- Test on different devices
- Try edge cases
- Stress test (long play sessions)
- Performance testing
- Accessibility testing

### Creating Test Cases

Document test scenarios that reveal issues:
1. Description of test
2. Steps to perform
3. Expected result
4. Actual result (if bug)

## 💬 Community Guidelines

### Be Respectful

- Be kind and constructive
- Respect different opinions
- Help others when you can
- Stay on topic

### Communication

- Use clear, professional language
- Provide context for your feedback
- Be patient with responses
- Thank contributors

## 🏆 Recognition

### Beta Testers

- Listed in CONTRIBUTORS.md
- Mentioned in release notes (optional)
- Beta tester badge (if implemented)

### Code Contributors

- Git commit history
- Listed in CONTRIBUTORS.md
- Mentioned in relevant changelogs

### Documentation Contributors

- Listed in CONTRIBUTORS.md
- Acknowledged in documentation

## 📞 Getting Help

### Need Help Contributing?

- **General Questions**: Use GitHub Discussions
- **Bug Help**: Comment on the relevant issue
- **Technical Help**: See TROUBLESHOOTING.md
- **Other Questions**: Create a new discussion

## 🔄 Contribution Workflow

```
1. Fork repo (if code contributor)
   ↓
2. Test/explore the game
   ↓
3. Find issues or ideas
   ↓
4. Check if already reported
   ↓
5. Create issue/discussion
   ↓
6. Engage with maintainers
   ↓
7. Implement fix (if applicable)
   ↓
8. Submit PR (if code change)
   ↓
9. Address review feedback
   ↓
10. Get merged! 🎉
```

## 📅 Release Cycle

### Current Phase: Beta Testing

- **Focus**: Finding and fixing bugs
- **Timeline**: Ongoing until stable
- **Goal**: Smooth, polished experience

### Future Phases

1. **Beta** (Current): Testing and feedback
2. **RC** (Release Candidate): Final testing
3. **Release**: Public launch
4. **Post-Release**: Updates and features

## ❓ FAQ

### Q: I found a typo. Should I report it?

A: Yes! Small fixes are welcome. Either create an issue or submit a PR.

### Q: Can I suggest new game modes?

A: Absolutely! Use the feature request template.

### Q: I'm not a developer. Can I still contribute?

A: Yes! Testing, feedback, and documentation are all valuable.

### Q: How long until my issue is addressed?

A: It depends on severity and complexity. Critical bugs are prioritized.

### Q: Can I work on any open issue?

A: Comment on the issue first to make sure no one else is working on it.

### Q: What if my feature suggestion is rejected?

A: Not all features align with the game's vision. We appreciate all suggestions!

## 📜 License

By contributing, you agree that your contributions will be licensed under the same license as the project.

## 🙏 Thank You!

Every contribution, no matter how small, helps make JACAMENO better. We appreciate your time and effort!

---

**Questions?** Open a discussion or issue, and we'll be happy to help!

**Ready to contribute?** Check out the [Beta Testing Guide](docs/BETA_TESTING_GUIDE.md) to get started!
