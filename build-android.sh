#!/bin/bash
# Build script for JACAMENO Android game
# Usage: ./build-android.sh [debug|release]

set -e

BUILD_TYPE="${1:-debug}"

echo "========================================"
echo "Building JACAMENO for Android"
echo "Build type: $BUILD_TYPE"
echo "========================================"

# Make gradlew executable
chmod +x gradlew

if [ "$BUILD_TYPE" = "release" ]; then
    echo "Building release APK..."
    echo "Note: Ensure signing is configured in app/build.gradle.kts"
    ./gradlew assembleRelease
    
    if [ $? -eq 0 ]; then
        echo ""
        echo "✅ Release APK built successfully!"
        echo "Location: app/build/outputs/apk/release/app-release.apk"
        ls -lh app/build/outputs/apk/release/*.apk
    fi
elif [ "$BUILD_TYPE" = "debug" ]; then
    echo "Building debug APK..."
    ./gradlew assembleDebug
    
    if [ $? -eq 0 ]; then
        echo ""
        echo "✅ Debug APK built successfully!"
        echo "Location: app/build/outputs/apk/debug/app-debug.apk"
        ls -lh app/build/outputs/apk/debug/*.apk
    fi
else
    echo "❌ Invalid build type: $BUILD_TYPE"
    echo "Usage: ./build-android.sh [debug|release]"
    exit 1
fi

echo ""
echo "To install on connected device:"
echo "  adb install app/build/outputs/apk/$BUILD_TYPE/app-$BUILD_TYPE.apk"
echo ""
echo "Or use: ./gradlew install${BUILD_TYPE^}"
