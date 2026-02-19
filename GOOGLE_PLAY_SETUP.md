# Google Play Games Services Configuration

This guide explains how to set up Google Play Games Services for the Duck Merge Game to enable Google Sign-In and cloud save features.

## Overview

Google Play Games Services provides:
- **Authentication**: Let players sign in with their Google account
- **Cloud Save**: Sync high scores across devices
- **Profile Info**: Display player name and avatar
- **Future Features**: Leaderboards and achievements (Phase 2+)

## Prerequisites

1. **Google Play Console Account** ($25 one-time registration fee)
2. **Unity Project** with Duck Merge Game
3. **Package Name** (e.g., com.duckmergestudio.duckmerge)
4. **Keystore** for signing your app

## Step-by-Step Setup

### 1. Create App in Google Play Console

1. Go to [Google Play Console](https://play.google.com/console)
2. Click **Create App**
3. Fill in app details:
   - **App name**: Duck Merge Game
   - **Default language**: English (United States)
   - **App or game**: Game
   - **Free or paid**: Free
4. Accept declarations and click **Create App**

### 2. Set Up Play Games Services

1. In Google Play Console, go to your app
2. Navigate to **Grow > Play Games Services > Setup and management > Configuration**
3. Click **Create** to set up Play Games Services
4. Choose **No** for "Is this a web game?"
5. Fill in game details:
   - **Name**: Duck Merge Game
   - **Description**: A physics-based duck merge puzzle game
   - **Category**: Puzzle
   - **Graphic**: Upload app icon (512x512 PNG)
6. Click **Save**

### 3. Create OAuth 2.0 Credentials

#### 3.1 Get SHA-1 Fingerprint

First, you need your keystore's SHA-1 fingerprint:

**For Debug Keystore** (testing only):
```bash
# Windows
keytool -list -v -keystore "%USERPROFILE%\.android\debug.keystore" -alias androiddebugkey -storepass android -keypass android

# Mac/Linux
keytool -list -v -keystore ~/.android/debug.keystore -alias androiddebugkey -storepass android -keypass android
```

**For Release Keystore**:
```bash
keytool -list -v -keystore path/to/your/release.keystore -alias your_alias
```

Copy the **SHA-1 fingerprint** (it looks like: `A1:B2:C3:D4:E5:...`)

#### 3.2 Create OAuth Client

1. In Play Games Services Configuration, go to **Credentials**
2. Click **Add credential**
3. Select **Android**
4. Fill in:
   - **Name**: Duck Merge Game Android
   - **Package name**: com.duckmergestudio.duckmerge (your package name)
   - **SHA-1 certificate fingerprint**: Paste your SHA-1 from above
5. Click **Save**
6. Click **Create OAuth client**

**Important**: Create credentials for both debug AND release keystores!

### 4. Get Application ID

1. In Play Games Services, go to **Setup and management > Configuration**
2. Find your **Application ID** (13-digit number)
3. **Copy this ID** - you'll need it in Unity

### 5. Download Configuration File

1. In Play Games Services Configuration
2. Click **Get resources**
3. Download **Android XML** resources
4. You'll get an XML file with your configuration

### 6. Set Up Unity Plugin

#### 6.1 Import Google Play Games Plugin

1. Download the [Google Play Games Plugin for Unity](https://github.com/playgameservices/play-games-plugin-for-unity/releases)
2. Download latest `.unitypackage` file
3. In Unity, go to **Assets > Import Package > Custom Package**
4. Select the downloaded `.unitypackage`
5. Import all files

#### 6.2 Configure Plugin

1. In Unity, go to **Window > Google Play Games > Setup > Android setup**
2. Enter your **Application ID** from Step 4
3. Click **Setup**

#### 6.3 Add Required Configuration

Create the file `Assets/Plugins/Android/AndroidManifest.xml` if it doesn't exist:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application>
        <meta-data
            android:name="com.google.android.gms.games.APP_ID"
            android:value="@string/app_id" />
    </application>
</manifest>
```

Create the file `Assets/Plugins/Android/res/values/games-ids.xml`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <string name="app_id">YOUR_APPLICATION_ID_HERE</string>
</resources>
```

Replace `YOUR_APPLICATION_ID_HERE` with your 13-digit Application ID.

### 7. Implement Sign-In in Unity

Create a new script `GooglePlayManager.cs`:

```csharp
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;
using System;

namespace DuckMergeGame
{
    public class GooglePlayManager : MonoBehaviour
    {
        public static GooglePlayManager Instance { get; private set; }
        
        private bool isAuthenticated = false;
        
        public bool IsAuthenticated => isAuthenticated;
        public string PlayerName => PlayGamesPlatform.Instance.GetUserDisplayName();
        public string PlayerId => PlayGamesPlatform.Instance.GetUserId();
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Configure Google Play Games
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .EnableSavedGames()
                .Build();
            
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
        }
        
        private void Start()
        {
            // Auto sign-in on start
            SignIn();
        }
        
        public void SignIn()
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }
        
        private void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                isAuthenticated = true;
                Debug.Log($"Signed in as {PlayerName}");
                LoadCloudHighScore();
            }
            else
            {
                isAuthenticated = false;
                Debug.Log($"Sign-in failed: {status}");
            }
        }
        
        public void SignOut()
        {
            PlayGamesPlatform.Instance.SignOut();
            isAuthenticated = false;
            Debug.Log("Signed out");
        }
        
        public void SaveHighScoreToCloud(int score)
        {
            if (!isAuthenticated) return;
            
            OpenSavedGame("HighScore", (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    byte[] data = System.BitConverter.GetBytes(score);
                    SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder()
                        .WithUpdatedDescription($"High Score: {score}")
                        .Build();
                    
                    PlayGamesPlatform.Instance.SavedGame.CommitUpdate(game, update, data, OnSaveComplete);
                }
            });
        }
        
        private void LoadCloudHighScore()
        {
            if (!isAuthenticated) return;
            
            OpenSavedGame("HighScore", (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, OnLoadComplete);
                }
            });
        }
        
        private void OpenSavedGame(string filename, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
        {
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                filename,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                callback
            );
        }
        
        private void OnSaveComplete(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                Debug.Log("High score saved to cloud");
            }
        }
        
        private void OnLoadComplete(SavedGameRequestStatus status, byte[] data)
        {
            if (status == SavedGameRequestStatus.Success && data != null && data.Length > 0)
            {
                int cloudScore = System.BitConverter.ToInt32(data, 0);
                Debug.Log($"Loaded cloud high score: {cloudScore}");
                
                // Update local high score if cloud score is higher
                if (GameManager.Instance != null && cloudScore > GameManager.Instance.HighScore)
                {
                    // TODO: Update high score
                }
            }
        }
    }
}
```

### 8. Testing

#### 8.1 Internal Testing

1. In Google Play Console, go to **Testing > Internal testing**
2. Create a new release
3. Upload your signed APK/AAB
4. Add test users (their Gmail addresses)
5. Click **Save** and then **Review release**
6. Click **Start rollout to Internal testing**

#### 8.2 Test on Device

1. Build and install the app on an Android device
2. The device must be signed in with a test user's Google account
3. Launch the app
4. Sign-in dialog should appear
5. Sign in and verify it works

**Important**: Sign-in will ONLY work if:
- App is uploaded to Play Console (even internal testing)
- Device is signed in with a test user account
- OAuth credentials match your keystore SHA-1

### 9. Common Issues & Solutions

#### Sign-In Fails

**Problem**: "Sign-in failed with status: InternalError"
- **Solution**: Verify Application ID is correct in Unity and XML files
- **Solution**: Check SHA-1 fingerprint matches your keystore
- **Solution**: Ensure app is published to Internal Testing

**Problem**: "Developer error" message
- **Solution**: OAuth credentials not properly configured
- **Solution**: Package name mismatch between Unity and Play Console
- **Solution**: Wait 24 hours after creating OAuth credentials (can take time to propagate)

**Problem**: Sign-in dialog doesn't appear
- **Solution**: Check internet connection
- **Solution**: Verify Google Play Games plugin is properly installed
- **Solution**: Check AndroidManifest.xml has correct metadata

#### Cloud Save Issues

**Problem**: Data not syncing
- **Solution**: Verify "Saved Games" is enabled in OAuth configuration
- **Solution**: Check user is signed in before saving
- **Solution**: Handle conflict resolution properly

**Problem**: Old data appears after sync
- **Solution**: Use conflict resolution strategy (e.g., UseLongestPlaytime)
- **Solution**: Implement custom conflict resolution if needed

### 10. Production Checklist

Before releasing to production:

- [ ] Test sign-in with multiple accounts
- [ ] Test cloud save synchronization
- [ ] Test guest mode (play without sign-in)
- [ ] Test offline → online sync
- [ ] Create RELEASE OAuth credentials (different from debug)
- [ ] Update AndroidManifest.xml with correct Application ID
- [ ] Test on multiple devices
- [ ] Verify privacy policy is accessible
- [ ] Test account switching
- [ ] Handle sign-in errors gracefully

### 11. Privacy & Compliance

When using Google Sign-In:

1. **Privacy Policy Required**: Create and link privacy policy in Play Console
2. **Data Usage Declaration**: Declare what data you collect
3. **User Consent**: Inform users about data collection
4. **Data Deletion**: Provide way to delete user data

### 12. Future Enhancements (Phase 2+)

Once basic sign-in works, you can add:

- **Leaderboards**: Global high score ranking
- **Achievements**: Unlock achievements for milestones
- **Events**: Time-limited challenges
- **Quests**: Daily and weekly quests
- **Player Stats**: Track gameplay statistics

## Resources

- [Google Play Games Services Documentation](https://developers.google.com/games/services)
- [Unity Plugin GitHub](https://github.com/playgameservices/play-games-plugin-for-unity)
- [OAuth Configuration Guide](https://developers.google.com/games/services/console/enabling)
- [Saved Games Guide](https://developers.google.com/games/services/android/savedgames)

## Support

For Google Play Games issues:
- [Stack Overflow - google-play-games](https://stackoverflow.com/questions/tagged/google-play-games)
- [Unity Forums - Google Play Games](https://forum.unity.com/)
- [Project Issues](https://github.com/davidbubb/duck-merge-game/issues)

---

Good luck with Google Play Games integration! 🎮☁️
