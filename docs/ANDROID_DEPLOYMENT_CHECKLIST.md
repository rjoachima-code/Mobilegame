# Android Deployment Checklist for JACAMENO

## Pre-Build Checklist

### Code & Configuration
- [x] Package name set correctly: `com.jacameno.mobilegame`
- [x] App name set: JACAMENO
- [x] Version code and version name configured
- [x] Minimum SDK: API 22 (Android 5.1)
- [x] Target SDK: API 34 (Android 14)
- [x] Native library name: `jacameno`
- [ ] All Unity scenes included in build settings (if using Unity export)
- [ ] ProGuard rules configured for release build
- [ ] All required permissions in AndroidManifest.xml

### Assets & Resources
- [ ] App icon created for all densities (mdpi, hdpi, xhdpi, xxhdpi, xxxhdpi)
- [ ] Splash screen/launch screen configured
- [ ] All game assets included in APK
- [ ] Audio files present and correctly named
- [ ] Textures optimized for mobile (compressed formats)
- [ ] String resources localized (if supporting multiple languages)

### Native Code
- [x] CMakeLists.txt configured with all source files
- [x] Native library loads correctly in MainActivity
- [ ] Native code builds without errors
- [ ] JNI methods properly declared and implemented
- [ ] Memory leaks checked and fixed
- [ ] Crash handling implemented

## Build & Testing Checklist

### Development Build Testing
- [ ] App builds successfully in debug mode
- [ ] App installs on device/emulator
- [ ] App launches without crashes
- [ ] Logcat shows no critical errors
- [ ] Touch controls responsive
- [ ] Game mechanics work correctly
- [ ] Audio plays without issues
- [ ] Performance acceptable (target 60 FPS)

### Release Build Testing
- [ ] Release APK builds successfully
- [ ] APK size is reasonable (target < 100 MB)
- [ ] ProGuard/R8 doesn't break functionality
- [ ] Release APK installs and runs on test devices
- [ ] No debug logs in release build
- [ ] Signing key properly configured
- [ ] Test on multiple Android versions (5.1, 8.0, 10, 12, 14)
- [ ] Test on different screen sizes and densities

### Device Compatibility Testing
Test on:
- [ ] Low-end device (2GB RAM, older CPU)
- [ ] Mid-range device (4GB RAM)
- [ ] High-end device (8GB+ RAM)
- [ ] Tablet (different aspect ratio)
- [ ] Different Android versions (5.1, 8.0, 10, 12, 14)

## Play Store Preparation Checklist

### App Signing
- [ ] Signing key generated and backed up securely
- [ ] App signed with release key
- [ ] Key passwords documented securely
- [ ] Upload key configured (if using Play App Signing)

### Store Listing
- [ ] App title (max 30 characters): "JACAMENO"
- [ ] Short description (max 80 characters): Ready
- [ ] Full description (max 4000 characters): Ready
- [ ] Screenshots (minimum 2 for each supported device type):
  - [ ] Main menu
  - [ ] Gameplay
  - [ ] Game over screen
  - [ ] Power-ups in action
- [ ] Feature graphic (1024x500): Ready
- [ ] App icon (512x512): Ready
- [ ] Promo video (optional): Ready or N/A

### Content Rating
- [ ] Content rating questionnaire completed
- [ ] Age rating obtained (likely PEGI 3 / ESRB E)

### Privacy & Legal
- [ ] Privacy policy created and hosted
- [ ] Privacy policy URL added to store listing
- [ ] Data safety form completed
- [ ] Terms of service (if applicable)

### App Categories & Tags
- [ ] Primary category: Games > Puzzle
- [ ] Secondary category: N/A or Casual
- [ ] Tags/keywords selected for discoverability

## Pre-Release Checklist

### Internal Testing
- [ ] Upload to Google Play Internal Testing track
- [ ] Add internal testers
- [ ] Distribute to internal testers
- [ ] Collect feedback
- [ ] Fix critical issues

### Closed Beta (Optional)
- [ ] Upload to Closed Testing track
- [ ] Invite beta testers
- [ ] Set up feedback channels (email, forum)
- [ ] Monitor crash reports in Play Console
- [ ] Address critical bugs

### Pre-Launch Report
- [ ] Review Pre-Launch Report in Play Console
- [ ] Address any compatibility issues
- [ ] Fix crashes detected by automated testing
- [ ] Verify app works on popular devices

## Release Checklist

### Final Verification
- [ ] All beta testing feedback addressed
- [ ] No critical bugs remaining
- [ ] Performance optimized
- [ ] All store assets finalized
- [ ] Final APK/AAB tested on real devices

### Release Process
- [ ] Create production release in Play Console
- [ ] Upload final AAB file
- [ ] Set rollout percentage (e.g., 5%, 10%, 50%, 100%)
- [ ] Write release notes
- [ ] Submit for review
- [ ] Monitor review status

### Post-Release Monitoring
- [ ] Monitor crash rates in Play Console
- [ ] Check user reviews and ratings
- [ ] Track installs and uninstalls
- [ ] Monitor performance metrics
- [ ] Respond to user feedback
- [ ] Prepare hotfix if critical issues found

## Marketing Checklist (Optional)

- [ ] Social media announcement
- [ ] Press release
- [ ] Reach out to game reviewers
- [ ] Create trailer video
- [ ] Website/landing page
- [ ] Community engagement (Discord, Reddit, etc.)

## Monetization (If Applicable)

- [ ] Ad integration tested (if using ads)
- [ ] In-app purchases configured (if applicable)
- [ ] Payment testing completed
- [ ] Ad IDs registered
- [ ] Revenue tracking set up

## Maintenance Plan

### Regular Updates
- [ ] Bug fix schedule established
- [ ] Feature update roadmap
- [ ] Community feedback process
- [ ] Analytics and metrics tracking

### Monitoring
- [ ] Set up alerting for crash rate spikes
- [ ] Monitor ANR (Application Not Responding) rate
- [ ] Track key metrics (DAU, retention, session length)
- [ ] Regular Play Console check-ins

## Tools & Services

### Required
- [x] Google Play Developer Account ($25 one-time fee)
- [ ] Signing key and keystore

### Recommended
- [ ] Firebase (Analytics, Crashlytics)
- [ ] Google AdMob (if monetizing with ads)
- [ ] Beta testing platform (TestFlight alternative for Android)

## Documentation

- [x] Build instructions documented
- [x] Deployment guide created
- [ ] User manual or FAQ (optional)
- [ ] Developer onboarding docs (for team members)

## Notes

- The minimum SDK of API 22 (Android 5.1) covers ~99% of active Android devices
- Target SDK should be updated annually to meet Play Store requirements
- Store listing updates don't require app review (except category changes)
- Production releases can be paused or halted after submission
- Staged rollouts help identify issues before 100% deployment

## Critical Paths for First Release

Minimum requirements to launch:
1. Working APK/AAB that installs and runs
2. Store listing with required assets (icon, screenshots, descriptions)
3. Content rating completed
4. Privacy policy (if collecting any user data)
5. Signed with release key

Everything else can be iteratively improved!
