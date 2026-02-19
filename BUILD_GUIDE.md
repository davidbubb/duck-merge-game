# Building Duck Merge Game for Android

This guide walks you through building the Duck Merge Game for Android devices.

## Prerequisites

Before building, ensure you have:

1. **Unity 2022.3.10f1 or later** installed via Unity Hub
2. **Android Build Support** module installed (via Unity Hub)
   - Android SDK & NDK Tools
   - OpenJDK
3. **Android device** or emulator for testing
4. **USB Debugging enabled** on your Android device (if testing on hardware)

## Setup Steps

### 1. Install Android Build Support

If you haven't already installed Android Build Support:

1. Open **Unity Hub**
2. Click on **Installs** tab
3. Find your Unity version (2022.3.10f1)
4. Click the gear icon and select **Add Modules**
5. Check:
   - Android Build Support
   - Android SDK & NDK Tools
   - OpenJDK
6. Click **Done** and wait for installation

### 2. Configure Android SDK

1. Open Unity Editor
2. Go to **Edit > Preferences** (Windows/Linux) or **Unity > Preferences** (Mac)
3. Select **External Tools**
4. Under **Android** section, verify paths are set:
   - **Android SDK path**: Should be auto-detected
   - **Android NDK path**: Should be auto-detected
   - **JDK path**: Should be auto-detected

### 3. Configure Project Settings

1. Open **File > Build Settings**
2. Select **Android** platform
3. Click **Switch Platform** (if not already on Android)
4. Click **Player Settings** button
5. Configure the following:

#### Company & Product
- **Company Name**: DuckMergeStudio (or your name)
- **Product Name**: Duck Merge Game

#### Identification
- **Package Name**: com.duckmergestudio.duckmerge (or your own)
- **Version**: 1.0
- **Bundle Version Code**: 1

#### Minimum API Level
- Set to **API Level 24 (Android 7.0 'Nougat')**

#### Target API Level
- Set to **API Level 33 (Android 13)** or latest

#### Scripting Backend
- **IL2CPP** (recommended for release)
- **Mono** (faster builds for development)

#### Target Architectures
- Check **ARMv7** (for older devices)
- Check **ARM64** (required for Google Play)

#### Other Settings
- **Write Permission**: Internal Only
- **Internet Access**: Require
- **Install Location**: Automatic

## Building the APK

### Debug Build (for Testing)

1. Open **File > Build Settings**
2. Ensure **Android** is selected
3. Click **Add Open Scenes** to include current scene
4. Check **Development Build** (optional, for debugging)
5. Click **Build**
6. Choose save location (e.g., `Builds/duck-merge-debug.apk`)
7. Wait for build to complete

### Release Build (for Distribution)

1. Open **File > Build Settings**
2. Ensure **Android** is selected
3. **Uncheck** Development Build
4. Click **Player Settings**
5. Navigate to **Publishing Settings**
6. Create or select a keystore:
   - Click **Keystore Manager**
   - Create new keystore or use existing
   - Fill in keystore details
   - **IMPORTANT**: Save keystore credentials securely!
7. Return to **Build Settings**
8. Click **Build**
9. Choose save location (e.g., `Builds/duck-merge-release.apk`)
10. Wait for build to complete

## Installing the APK

### Install via USB (Android Device)

1. Connect your Android device via USB
2. Enable **USB Debugging** on device:
   - Go to Settings > About Phone
   - Tap Build Number 7 times to enable Developer Options
   - Go to Settings > Developer Options
   - Enable USB Debugging
3. Open terminal/command prompt
4. Navigate to build directory
5. Run:
   ```bash
   adb install duck-merge-game.apk
   ```

### Install via Unity (Android Device)

1. Connect device via USB
2. In Unity, go to **File > Build Settings**
3. Select **Android**
4. Click **Build And Run**
5. Unity will build and automatically install on connected device

### Install via File Transfer

1. Copy APK file to your Android device (via USB, cloud storage, etc.)
2. On device, use a file manager to locate the APK
3. Tap the APK file
4. Allow installation from unknown sources if prompted
5. Tap **Install**

## Testing the Build

After installation:

1. Launch the app from your device
2. Test the following:
   - ✅ App launches without crashes
   - ✅ Main menu appears and is responsive
   - ✅ Play button starts the game
   - ✅ Birds drop and physics works smoothly
   - ✅ Merging works correctly
   - ✅ Scoring updates properly
   - ✅ Game over triggers correctly
   - ✅ Audio plays (music and SFX)
   - ✅ Pause/resume works
   - ✅ High score saves and loads
   - ✅ Game runs at 60 FPS (check with Unity Profiler)

## Troubleshooting

### Build Fails

**Problem**: Build fails with SDK/NDK errors
- **Solution**: Verify Android SDK, NDK, and JDK paths in Unity preferences
- **Solution**: Update Unity Hub and reinstall Android Build Support

**Problem**: "Unable to find Unity Engine"
- **Solution**: Restart Unity Editor
- **Solution**: Reimport all assets (Assets > Reimport All)

### App Won't Install

**Problem**: "App not installed" error
- **Solution**: Uninstall previous version first
- **Solution**: Enable "Install from Unknown Sources" in device settings
- **Solution**: Check if device has enough storage space

**Problem**: APK won't install via adb
- **Solution**: Check USB debugging is enabled
- **Solution**: Run `adb devices` to verify device is detected
- **Solution**: Try `adb install -r duck-merge-game.apk` (reinstall flag)

### Performance Issues

**Problem**: Game runs slowly or lags
- **Solution**: Ensure target architecture matches device (ARM64 vs ARMv7)
- **Solution**: Reduce graphics quality in Unity settings
- **Solution**: Check if Development Build is enabled (disable for release)
- **Solution**: Profile with Unity Profiler to identify bottlenecks

**Problem**: Physics issues or inconsistent behavior
- **Solution**: Verify Time.fixedDeltaTime is set correctly
- **Solution**: Check Physics2D settings match configuration
- **Solution**: Test on multiple devices

### Google Play Games Issues

**Problem**: Can't sign in with Google
- **Solution**: Verify google-services.json is in correct location
- **Solution**: Check OAuth credentials are properly configured
- **Solution**: Ensure app is published to Google Play Console (internal testing)
- **Solution**: SHA-1 fingerprint matches keystore

## Building for Google Play

### Requirements

1. **Google Play Console** account ($25 one-time fee)
2. **Signed APK or AAB** (Android App Bundle)
3. **App assets**: Icon, screenshots, description
4. **Privacy Policy** (required for apps with Google Sign-In)

### Creating App Bundle (AAB)

1. Open **File > Build Settings**
2. Select **Android**
3. Check **Build App Bundle (Google Play)**
4. Click **Build**
5. Choose save location
6. AAB file will be created instead of APK

### Uploading to Google Play

1. Go to [Google Play Console](https://play.google.com/console)
2. Create new application
3. Fill in store listing details
4. Upload AAB to **Internal Testing** track
5. Configure testers
6. Submit for review
7. Once approved, share test link with testers

### Important Notes

- **First upload**: Must be signed with same keystore for all future updates
- **Backup keystore**: Store keystore file and credentials securely
- **Version codes**: Must increment for each upload
- **Target API**: Must meet Google Play requirements (currently API 33+)

## Next Steps

After successful build:

1. Test thoroughly on multiple devices
2. Gather feedback from testers
3. Fix bugs and improve performance
4. Add more features (Phase 2)
5. Prepare for public release

## Build Optimization Tips

### Reduce APK Size

1. **Enable IL2CPP** stripping
   - Player Settings > Other Settings > Managed Stripping Level: High
2. **Compress textures** properly
   - Use ASTC compression for Android
3. **Remove unused assets**
   - Clean up unused prefabs, scripts, textures
4. **Disable unused modules**
   - Player Settings > Other Settings > Strip Engine Code: Yes

### Improve Performance

1. **Object pooling** for frequently spawned objects (birds)
2. **Optimize physics** settings
   - Reduce max colliders per bird
   - Optimize collision detection settings
3. **Use sprite atlases** to reduce draw calls
4. **Profile regularly** with Unity Profiler
5. **Test on low-end devices** (2-3 year old phones)

## Common Commands

```bash
# Check connected devices
adb devices

# Install APK
adb install duck-merge-game.apk

# Install and replace existing
adb install -r duck-merge-game.apk

# Uninstall app
adb uninstall com.duckmergestudio.duckmerge

# View device logs
adb logcat -s Unity

# Clear device logs
adb logcat -c

# Take screenshot
adb shell screencap -p /sdcard/screenshot.png

# Pull screenshot to computer
adb pull /sdcard/screenshot.png
```

## Support

For build issues:
- Check Unity documentation: https://docs.unity3d.com/Manual/android.html
- Unity forums: https://forum.unity.com/
- Project issues: https://github.com/davidbubb/duck-merge-game/issues

---

Happy building! 🦆🎮
