JACAMENO — Native Android Puzzle Game (C++ / GameActivity / OpenGL ES 3.0)

This repository contains a native Android game written in C++ using Google GameActivity and OpenGL ES 3.0.

Quick build & install (Windows PowerShell)

1) Environment (examples — adjust to your machine):

```powershell
$env:JAVA_HOME = "C:\Program Files\Java\jdk-17"
$env:ANDROID_SDK_ROOT = "C:\Users\<you>\AppData\Local\Android\Sdk"
$env:ANDROID_NDK_HOME = "C:\Users\<you>\AppData\Local\Android\Sdk\ndk\<version>"
$env:PATH += ";$env:ANDROID_SDK_ROOT\platform-tools"
```

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

Git workflow (create branch, commit local changes, push):

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

