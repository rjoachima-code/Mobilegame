# Quick Start Guide: Testing JACAMENO on Android

## What You Need

1. **Android Studio** - Download from https://developer.android.com/studio
2. **Android Device or Emulator** - Physical device recommended for best performance
3. **USB Cable** (if using physical device)

## 5-Minute Setup

### Step 1: Install Android Studio
- Download and install Android Studio
- During setup, accept default settings
- Install Android SDK when prompted

### Step 2: Open the Project
```bash
# Clone if you haven't already
git clone https://github.com/rjoachima-code/Mobilegame.git
cd Mobilegame

# Open in Android Studio
# File > Open > Select the Mobilegame directory
```

### Step 3: Sync Dependencies
- Android Studio will automatically start syncing
- Click "Sync Now" if prompted
- Wait for "Gradle sync finished" message (may take a few minutes on first run)

### Step 4: Connect Your Device

**Option A: Physical Device (Recommended)**
1. Enable Developer Options on your phone:
   - Go to Settings > About Phone
   - Tap "Build Number" 7 times
   - You'll see "You are now a developer!"

2. Enable USB Debugging:
   - Go to Settings > Developer Options
   - Toggle on "USB Debugging"

3. Connect phone to computer via USB
   - Accept "Allow USB debugging?" prompt on phone
   - Your device should appear in Android Studio's device dropdown

**Option B: Use Emulator**
1. In Android Studio: Tools > AVD Manager
2. Click "Create Virtual Device"
3. Select "Pixel 5" (or any device)
4. Select system image "R" (API 30) and download if needed
5. Click "Finish" then "Play" button to start emulator

### Step 5: Build and Run
1. Click the green "Run" button (▶) in Android Studio toolbar
2. Select your device from the dropdown if not already selected
3. Wait for build to complete (~2-5 minutes first time)
4. App will automatically install and launch on your device!

## What to Test

Once the app launches:
- ✅ Main menu appears
- ✅ Tap screen to interact
- ✅ Start game works
- ✅ Touch controls responsive (swipe/tap)
- ✅ Blocks fall and can be controlled
- ✅ Score updates
- ✅ Audio plays (if enabled)

## Troubleshooting

### "Gradle sync failed"
- Ensure you have internet connection
- Try: File > Invalidate Caches and Restart

### "No connected devices"
- For physical device: Check USB connection, try different USB port
- For emulator: Wait 30 seconds for it to fully boot

### "App crashes immediately"
- Check Logcat (View > Tool Windows > Logcat)
- Look for red error messages
- Report the error to the development team

### "Build takes too long"
- First build can take 5-10 minutes
- Subsequent builds will be much faster (30 seconds - 2 minutes)

## View Logs

To see what's happening in the app:
1. Open Logcat: View > Tool Windows > Logcat
2. Select your device from dropdown
3. Filter by package name: "com.jacameno.mobilegame"
4. Look for errors (red) or warnings (orange)

## Building an APK to Share

To create an APK file you can share with others:
1. Build > Build Bundle(s) / APK(s) > Build APK(s)
2. Wait for "Build Successful" notification
3. Click "locate" in the notification
4. APK file will be in `app/build/outputs/apk/debug/`
5. Transfer this APK to another device to test

**Note:** Debug APKs are larger and slower than release builds. See full build instructions for creating optimized release APKs.

## Next Steps

- ✅ **You've successfully built and run the app!**
- 📖 Read [ANDROID_BUILD_INSTRUCTIONS.md](./ANDROID_BUILD_INSTRUCTIONS.md) for complete build options
- 📋 Check [ANDROID_DEPLOYMENT_CHECKLIST.md](./ANDROID_DEPLOYMENT_CHECKLIST.md) before releasing

## Getting Help

- Check existing GitHub Issues
- Review Logcat for error messages
- Ensure you're using Android Studio Arctic Fox or later
- Make sure Android SDK API 22+ is installed

## Performance Tips

- **Physical device recommended**: Emulators can be slow for games
- **Use x86_64 emulator images** on Intel/AMD computers for better performance
- **Enable hardware acceleration** in emulator settings
- **Close other apps** to free up resources

---

**Estimated time from start to seeing the game running: 15-30 minutes** (including downloads)
