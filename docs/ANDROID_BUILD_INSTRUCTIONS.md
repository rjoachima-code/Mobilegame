# Android Build and Testing Instructions for JACAMENO

## Overview
This document provides comprehensive instructions for building and testing the JACAMENO mobile game on Android devices.

## Prerequisites

### Required Software
1. **Android Studio** (Arctic Fox 2020.3.1 or later)
   - Download from: https://developer.android.com/studio
   
2. **Android SDK**
   - Minimum API Level: 22 (Android 5.1 Lollipop)
   - Target API Level: 34 (Android 14)
   - Install via Android Studio SDK Manager
   
3. **Android NDK** (for native C++ code)
   - Version: 25.1.8937393 or later
   - Install via Android Studio SDK Manager
   
4. **CMake** (for building native code)
   - Version: 3.22.1 or later
   - Install via Android Studio SDK Manager

5. **Java Development Kit (JDK)**
   - Version: 11 or later
   - Comes bundled with Android Studio

### Optional for Unity Export
6. **Unity 2021.3 LTS or later** (if exporting from Unity)
   - With Android Build Support module

## Project Configuration

The project has been configured with:
- **Package Name**: `com.jacameno.mobilegame`
- **App Name**: JACAMENO
- **Native Library Name**: `jacameno`
- **Minimum SDK**: API 22 (Android 5.1)
- **Target SDK**: API 34 (Android 14)
- **Compile SDK**: API 34

## Build Options

### Option 1: Build APK Directly (Recommended for Quick Testing)

1. **Open the Project in Android Studio**
   ```bash
   cd /path/to/Mobilegame
   # Open Android Studio and select "Open an Existing Project"
   # Navigate to the Mobilegame directory
   ```

2. **Sync Gradle**
   - Android Studio will automatically prompt to sync Gradle
   - If not, go to `File > Sync Project with Gradle Files`
   - Wait for all dependencies to download

3. **Connect an Android Device or Start an Emulator**
   
   **For Physical Device:**
   - Enable USB Debugging on your device:
     - Go to Settings > About Phone
     - Tap "Build Number" 7 times to enable Developer Options
     - Go to Settings > Developer Options
     - Enable "USB Debugging"
   - Connect device via USB
   - Accept the USB debugging prompt on the device
   
   **For Emulator:**
   - Open AVD Manager: `Tools > AVD Manager`
   - Create a new Virtual Device:
     - Select a device definition (e.g., Pixel 5)
     - Select a system image (API 22 or higher)
     - Finish and click "Play" to start the emulator

4. **Build and Run**
   - Click the green "Run" button (▶) in Android Studio, or
   - Use menu: `Run > Run 'app'`
   - Select your target device/emulator
   - The app will build, install, and launch automatically

5. **View Logs (Optional)**
   - Open Logcat window (View > Tool Windows > Logcat)
   - Filter by tag "JACAMENO" or "Unity" to see game-specific logs

### Option 2: Build Signed Release APK

1. **Generate a Signing Key**
   ```bash
   keytool -genkey -v -keystore jacameno-release-key.keystore \
           -alias jacameno -keyalg RSA -keysize 2048 -validity 10000
   ```
   - Store the keystore file securely
   - Remember the keystore password and key alias password

2. **Configure Signing in build.gradle.kts**
   
   Add to `app/build.gradle.kts` inside the `android` block:
   ```kotlin
   signingConfigs {
       create("release") {
           storeFile = file("path/to/jacameno-release-key.keystore")
           storePassword = "your_keystore_password"
           keyAlias = "jacameno"
           keyPassword = "your_key_password"
       }
   }
   
   buildTypes {
       release {
           signingConfig = signingConfigs.getByName("release")
           isMinifyEnabled = true
           proguardFiles(
               getDefaultProguardFile("proguard-android-optimize.txt"),
               "proguard-rules.pro"
           )
       }
   }
   ```

3. **Build Release APK**
   - In Android Studio: `Build > Generate Signed Bundle / APK`
   - Select "APK"
   - Choose the keystore and enter passwords
   - Select "release" build variant
   - Click "Finish"
   
   Or via command line:
   ```bash
   ./gradlew assembleRelease
   ```
   
   The APK will be in: `app/build/outputs/apk/release/app-release.apk`

### Option 3: Build Android App Bundle (AAB) for Play Store

1. **Build AAB**
   ```bash
   ./gradlew bundleRelease
   ```
   
   Or in Android Studio:
   - `Build > Generate Signed Bundle / APK`
   - Select "Android App Bundle"
   - Choose keystore and sign
   
   The AAB will be in: `app/build/outputs/bundle/release/app-release.aab`

2. **Upload to Play Store**
   - Go to Google Play Console
   - Create a new app or select existing
   - Navigate to Release > Production
   - Upload the AAB file
   - Complete store listing and publish

## Testing

### Unit Tests

Run unit tests:
```bash
./gradlew test
```

Or in Android Studio:
- Right-click on test directory
- Select "Run Tests"

### Instrumented Tests (on Device/Emulator)

Run instrumented tests:
```bash
./gradlew connectedAndroidTest
```

Or in Android Studio:
- Right-click on androidTest directory
- Select "Run Tests"

### Manual Testing Checklist

- [ ] App launches without crashes
- [ ] Main menu displays correctly
- [ ] Can start a new game
- [ ] Touch controls work (swipe, tap)
- [ ] Tetromino pieces fall and can be controlled
- [ ] Block merging works correctly
- [ ] Score updates properly
- [ ] Game over condition triggers correctly
- [ ] Power-ups activate and function
- [ ] Audio plays (merge sounds, clear sounds)
- [ ] Pause/resume works
- [ ] Return to main menu works
- [ ] Game state persists across app lifecycle
- [ ] Performance is smooth (60 FPS target)
- [ ] No visual artifacts or glitches

## Troubleshooting

### Build Fails with "NDK not found"
- Open Android Studio > Tools > SDK Manager
- Go to SDK Tools tab
- Check "NDK (Side by side)" and install

### Build Fails with "CMake not found"
- Open Android Studio > Tools > SDK Manager
- Go to SDK Tools tab
- Check "CMake" and install

### App Crashes on Launch
- Check Logcat for error messages
- Common issues:
  - Missing native library: Ensure `libja cameno.so` is built
  - Missing assets: Ensure all required assets are in `src/main/assets`
  - Permissions: Check AndroidManifest.xml for required permissions

### Native Code Doesn't Build
- Verify CMakeLists.txt path in build.gradle.kts
- Check that all .cpp/.h files are listed in CMakeLists.txt
- Clean and rebuild: `./gradlew clean build`

### Touch Controls Don't Work
- Verify InputManager.cpp handles Android touch events
- Check that GameActivity is properly initialized
- Test on physical device (emulator touch can be less responsive)

### Performance Issues
- Use Android Profiler (View > Tool Windows > Profiler)
- Check CPU, Memory, and GPU usage
- Consider reducing graphics quality for lower-end devices
- Optimize frame rate and reduce overdraw

### Audio Doesn't Play
- Check that audio files exist in assets/
- Verify NativeAudioBridge.java is correctly initialized
- Check device volume and audio settings
- Look for audio-related errors in Logcat

## Distribution

### Beta Testing (Google Play Internal Testing)

1. Create a release in Play Console
2. Upload AAB
3. Add internal testers (email addresses)
4. Share the testing link with testers

### Open Beta or Production

1. Complete Play Store listing:
   - App name, description
   - Screenshots (required: at least 2)
   - Feature graphic
   - App icon
   - Content rating
   - Privacy policy

2. Set pricing (free or paid)
3. Select countries
4. Submit for review

## Performance Optimization

### Recommendations
- Use ProGuard/R8 for code shrinking (already enabled in release)
- Optimize textures (use compressed formats like ETC2)
- Implement object pooling for frequently created objects
- Profile using Android Profiler
- Test on various device tiers (low, mid, high-end)

### Native Code Optimization
- Enable compiler optimizations in CMakeLists.txt
- Use ARM NEON instructions for vector operations
- Profile with Android Studio CPU Profiler

## Additional Resources

- [Android Developer Guide](https://developer.android.com/guide)
- [Android Game Development](https://developer.android.com/games)
- [Android NDK Guide](https://developer.android.com/ndk/guides)
- [GameActivity Documentation](https://developer.android.com/games/agdk/game-activity)

## Support

For issues specific to JACAMENO, check the project's GitHub Issues page.
