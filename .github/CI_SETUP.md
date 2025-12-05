# CI/CD Setup Guide for JACAMENO

This guide explains how to set up automated builds using GitHub Actions.

## Prerequisites

1. A Unity account with a valid license
2. GitHub repository secrets configured

## Required Secrets

Go to your repository Settings > Secrets and variables > Actions, and add these secrets:

### Unity License (Required)
- `UNITY_LICENSE` - Your Unity license file content (see below for activation)
- `UNITY_EMAIL` - Your Unity account email
- `UNITY_PASSWORD` - Your Unity account password

### Android Signing (Optional - for signed APKs)
- `ANDROID_KEYSTORE_BASE64` - Base64-encoded keystore file
- `ANDROID_KEY_ALIAS` - Key alias in the keystore
- `ANDROID_KEYSTORE_PASSWORD` - Keystore password
- `ANDROID_KEY_PASSWORD` - Key password

## Getting Your Unity License

### For Personal/Plus Licenses:
1. Install Unity Hub and Unity Editor locally
2. Activate your license through Unity Hub
3. Find your license file:
   - **Windows**: `C:\ProgramData\Unity\Unity_lic.ulf`
   - **macOS**: `/Library/Application Support/Unity/Unity_lic.ulf`
   - **Linux**: `~/.local/share/unity3d/Unity/Unity_lic.ulf`
4. Copy the entire content of this file to the `UNITY_LICENSE` secret

### For Professional Licenses:
Use the game-ci activation workflow: https://game.ci/docs/github/activation

## Workflows

### Build Workflow (`build.yml`)
Triggers on push to main/master and creates builds for:
- Windows (64-bit)
- macOS
- Linux (64-bit)
- Android (APK)
- iOS (Xcode project)
- WebGL

### Test Workflow (`test.yml`)
Runs Unity EditMode and PlayMode tests on every push and PR.

## Downloading Builds

1. Go to Actions tab in your repository
2. Click on a successful workflow run
3. Scroll down to Artifacts section
4. Download the build for your platform

## Troubleshooting

### License Issues
- Ensure the license file content is copied exactly, including all XML tags
- Check that UNITY_EMAIL and UNITY_PASSWORD match your Unity account

### Build Failures
- Check Unity version compatibility (project uses 2021.3 LTS)
- Review the workflow logs for specific error messages

### Cache Issues
- Clear the cache by changing the cache key in the workflow
- Or manually delete caches from the Actions > Caches section

## Manual Trigger

You can manually trigger builds using the "Run workflow" button in the Actions tab.
