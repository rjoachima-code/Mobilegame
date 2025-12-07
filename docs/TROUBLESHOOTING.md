# Android Testing Troubleshooting Guide

This guide helps resolve common issues when building and testing JACAMENO on Android.

## Build Issues

### "Gradle sync failed" or "Plugin not found"

**Symptoms:**
- Android Studio shows "Gradle sync failed"
- Error mentions Android Gradle Plugin not found
- Build fails immediately

**Solutions:**
1. **Check Internet Connection**
   - Gradle needs to download dependencies on first sync
   - Ensure you can access dl.google.com and repo.maven.apache.org
   
2. **Invalidate Caches**
   ```
   File > Invalidate Caches and Restart
   ```

3. **Check SDK Location**
   ```
   File > Project Structure > SDK Location
   Ensure Android SDK path is set correctly
   ```

4. **Manual Gradle Sync**
   ```
   File > Sync Project with Gradle Files
   ```

5. **Check Gradle Version**
   - The project uses Gradle 8.13
   - Android Studio should download this automatically

### "SDK location not found"

**Solution:**
1. Open SDK Manager: `Tools > SDK Manager`
2. Install Android SDK Platform 34
3. Install Android SDK Build-Tools 34
4. Install NDK (Side by side)
5. Install CMake

### "NDK not configured" or "CMake not found"

**Solution:**
1. Open SDK Manager: `Tools > SDK Manager`
2. Go to "SDK Tools" tab
3. Check:
   - [x] NDK (Side by side)
   - [x] CMake
4. Click "Apply" to install
5. Restart Android Studio

### "Could not resolve: androidx.games:games-activity"

**Solution:**
1. Ensure Google Maven repository is accessible
2. Check `settings.gradle.kts` has `google()` repository
3. Try with VPN if regional blocking suspected
4. Update to latest version of Android Studio

### Build is extremely slow

**Solutions:**
1. **Enable parallel builds**
   - Add to `gradle.properties`:
   ```properties
   org.gradle.parallel=true
   org.gradle.caching=true
   org.gradle.jvmargs=-Xmx4096m
   ```

2. **Use Gradle daemon**
   - Already enabled by default in modern Gradle

3. **Disable antivirus scanning** of project directory temporarily

4. **Increase RAM** allocated to Gradle (see above)

## Device Connection Issues

### "No devices" or device not showing

**For Physical Device:**
1. **Enable USB Debugging**
   - Settings > About Phone
   - Tap "Build Number" 7 times
   - Settings > Developer Options
   - Enable "USB Debugging"

2. **Trust Computer**
   - When you connect, phone will ask "Allow USB debugging?"
   - Check "Always allow" and tap "OK"

3. **Try Different USB Cable/Port**
   - Some cables are charge-only
   - Try a USB 2.0 port instead of 3.0

4. **Check USB Mode**
   - Pull down notification shade
   - Tap USB notification
   - Select "File Transfer" or "PTP" mode

5. **Install USB Drivers** (Windows)
   - Download from phone manufacturer's website
   - Or use universal ADB drivers

**For Emulator:**
1. **Wait for Full Boot**
   - Emulator can take 30-60 seconds to fully start
   - Wait until you see the Android home screen

2. **Enable Hardware Acceleration**
   - Tools > AVD Manager > Edit AVD
   - Graphics: Hardware - GLES 2.0

3. **Increase Emulator RAM**
   - Edit AVD > Advanced Settings
   - RAM: 2048 MB or more

### "Device unauthorized"

**Solution:**
1. Unplug device
2. On phone: Settings > Developer Options
3. Tap "Revoke USB debugging authorizations"
4. Plug in again
5. Accept the authorization prompt (check "Always allow")

## Runtime Issues

### App crashes immediately on launch

**Debug Steps:**
1. **Check Logcat**
   ```
   View > Tool Windows > Logcat
   Filter: package:com.jacameno.mobilegame
   ```

2. **Common Causes:**
   - Native library not found: Check CMakeLists.txt
   - Missing assets: Verify assets directory
   - Permissions issue: Check AndroidManifest.xml
   - Out of memory: Test on device with more RAM

3. **Look for specific errors:**
   - `UnsatisfiedLinkError`: Native library not loaded
   - `NullPointerException`: Check initialization order
   - `SecurityException`: Missing permissions

### App shows black screen

**Solutions:**
1. **Check OpenGL initialization**
   - Some devices have OpenGL ES issues
   - Check Logcat for GL errors

2. **Verify GameActivity setup**
   - MainActivity extends GameActivity
   - Native library loads successfully

3. **Check asset loading**
   - Assets might not be packaged correctly
   - Verify assets exist in APK

4. **Test on different device**
   - Could be device-specific issue

### Touch input doesn't work

**Solutions:**
1. **Check InputManager**
   - Verify touch events are being received
   - Add logs in InputManager.cpp

2. **Test tap and swipe separately**
   - Simple taps vs swipe gestures

3. **Check system UI**
   - Immersive mode might interfere
   - Try commenting out `hideSystemUi()` temporarily

4. **Test on physical device**
   - Emulator touch can be unreliable

### No sound/audio

**Solutions:**
1. **Check device volume**
   - Press volume up button
   - Ensure media volume (not ringer) is up

2. **Verify assets**
   - Check `app/src/main/assets/` for .wav files
   - Files should be: `sfx_merge.wav`, `sfx_clear.wav`

3. **Check NativeAudioBridge**
   - Logs should show if fallback ToneGenerator is used
   - Look for "Failed to load sfx" in Logcat

4. **Test on physical device**
   - Emulator audio can be problematic

### Performance issues / Low FPS

**Solutions:**
1. **Test on physical device**
   - Emulators are generally slower

2. **Check CPU/GPU usage**
   ```
   View > Tool Windows > Profiler
   Select your device and app
   ```

3. **Reduce graphics quality**
   - Lower texture resolutions
   - Reduce particle effects
   - Decrease update frequency

4. **Enable hardware acceleration**
   - For emulator: Graphics settings
   - For device: Usually automatic

5. **Check for memory leaks**
   - Use Android Studio Memory Profiler
   - Look for growing memory usage

## Logcat Tips

### Filter to see only your app
```
package:com.jacameno.mobilegame
```

### Filter by severity
- Click "Error" to see only errors
- Click "Warn" to include warnings

### Search for specific text
- Use the search box in Logcat toolbar
- Example: search for "JACAMENO" or "crash"

### Common error patterns to look for
- `FATAL EXCEPTION`: App crash
- `ANR`: App Not Responding
- `OutOfMemoryError`: Memory issue
- `UnsatisfiedLinkError`: Native library problem
- `SecurityException`: Permission problem

## Building APK Issues

### "Execution failed for task ':app:mergeDebugNativeLibs'"

**Solution:**
1. Clean project: `Build > Clean Project`
2. Rebuild: `Build > Rebuild Project`
3. Check NDK is installed
4. Verify CMakeLists.txt has no errors

### APK is too large

**Solutions:**
1. **Enable ProGuard** (for release builds)
   - Already enabled in build.gradle.kts

2. **Use APK splits**
   - Split by ABI (arm, x86, etc.)
   - Configured in launcherTemplate.gradle

3. **Optimize assets**
   - Compress textures
   - Use appropriate audio quality
   - Remove unused resources

4. **Build AAB instead of APK**
   - App Bundle is smaller
   - Google Play optimizes downloads per device

### "Failed to install APK"

**Solutions:**
1. **Uninstall existing version**
   ```
   adb uninstall com.jacameno.mobilegame
   ```
   Then try installing again

2. **Check storage space**
   - Device needs enough free space

3. **Check signing**
   - Debug and release use different signatures
   - Can't install release over debug and vice versa

## Getting More Help

### Useful Commands

**Check connected devices:**
```bash
adb devices
```

**Install APK manually:**
```bash
adb install app/build/outputs/apk/debug/app-debug.apk
```

**View live logs:**
```bash
adb logcat | grep JACAMENO
```

**Uninstall app:**
```bash
adb uninstall com.jacameno.mobilegame
```

**Clear app data:**
```bash
adb shell pm clear com.jacameno.mobilegame
```

**Check APK info:**
```bash
./gradlew app:dependencies
```

### Where to Look for Help

1. **Project Documentation**
   - `docs/QUICK_START_ANDROID.md`
   - `docs/ANDROID_BUILD_INSTRUCTIONS.md`
   - `docs/ANDROID_DEPLOYMENT_CHECKLIST.md`

2. **Android Developer Resources**
   - [Android Developer Guide](https://developer.android.com)
   - [Android Studio User Guide](https://developer.android.com/studio/intro)
   - [GameActivity Docs](https://developer.android.com/games/agdk/game-activity)

3. **Stack Overflow**
   - Tag: `[android] [android-studio] [android-ndk]`

4. **GitHub Issues**
   - Check existing issues in the repository
   - Create new issue with:
     - Android version
     - Device model
     - Steps to reproduce
     - Logcat output

## Still Having Issues?

**Create a GitHub Issue with:**

1. **Environment Info**
   ```
   - Android Studio version
   - Gradle version (run: ./gradlew --version)
   - Android SDK version
   - NDK version
   - Device/Emulator model and Android version
   ```

2. **What You Tried**
   - Steps you followed
   - What you expected
   - What actually happened

3. **Logs**
   - Relevant Logcat output
   - Build error messages
   - Screenshots if applicable

4. **Reproducibility**
   - Does it happen every time?
   - Does it happen on multiple devices?
   - Did it ever work before?

---

**Most issues are resolved by:**
1. Checking internet connection
2. Ensuring SDK/NDK are installed
3. Cleaning and rebuilding
4. Testing on a physical device
5. Checking Logcat for specific errors
