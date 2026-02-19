# Duck Merge Game - Phase 1 MVP

A physics-based merge game for Android featuring ducks and water birds. Merge identical birds to evolve them through 11 tiers, from a tiny duckling to a majestic swan!

## Overview

Duck Merge is an Android game inspired by fruit merge games, featuring cute water birds instead. Players drop birds into a container, where they bounce and settle using realistic 2D physics. When two identical birds touch, they merge into the next evolution tier, earning points.

## Game Features

### Core Gameplay
- **Physics-Based Mechanics**: Realistic gravity, collisions, and physics simulation at 60 FPS
- **11 Bird Evolution Tiers**: From Duckling to Swan, each with unique visuals and increasing size
- **Merge System**: Identical birds merge on contact, creating the next tier
- **Scoring System**: Exponential point rewards (10 to 5,120 points per merge)
- **Game Over Condition**: Birds above the danger line for 2+ seconds trigger game over

### Bird Tiers
1. **Duckling** - Yellow, tiny (10 points)
2. **Baby Duck** - Fluffy, slightly larger (20 points)
3. **Juvenile Duck** - Growing duck (40 points)
4. **Mallard** - Adult male duck, green head (80 points)
5. **Wood Duck** - Colorful, medium-sized (160 points)
6. **Mandarin Duck** - Vibrant colors, larger (320 points)
7. **Goose** - Significantly bigger, white/grey (640 points)
8. **Canada Goose** - Large goose with distinctive markings (1,280 points)
9. **Pelican** - Very large water bird (2,560 points)
10. **Great Heron** - Tall, elegant bird (5,120 points)
11. **Swan** - Majestic white swan, final evolution (cannot merge further)

### User Interface
- **Main Menu**: Play button, high score display, settings, profile section
- **Game Screen**: Score display, high score, next bird preview, danger line indicator, pause button
- **Game Over Screen**: Final score, high score comparison, retry and menu buttons
- **Pause Screen**: Resume, restart, settings access, home button

### Audio
- **Sound Effects**: Drop sound, tier-based merge sounds, game over sound, UI clicks, special swan sound
- **Background Music**: Calming, upbeat water/nature-themed tracks with seamless looping
- **Audio Controls**: Toggle music/SFX independently, adjustable volume, persistent settings

### Persistence & Cloud
- **Local Storage**: High score, audio settings, tutorial completion flag
- **Google Sign-In**: Optional authentication for cloud features
- **Cloud Sync**: High score synchronization across devices (requires sign-in)
- **Guest Mode**: Play without signing in (local scores only)

## Technical Specifications

### Platform Requirements
- **Platform**: Android
- **Minimum SDK**: 24 (Android 7.0)
- **Target SDK**: Latest stable
- **Development Engine**: Unity 2022.3.10f1
- **Language**: C#
- **Physics**: Unity Physics2D (Box2D)

### Performance Targets
- **Frame Rate**: 60 FPS on mid-range devices (2019+)
- **Load Time**: < 3 seconds to main menu
- **App Size**: < 100MB
- **Max Birds on Screen**: 20+ without performance degradation

### Permissions
- **Internet**: For Google Sign-In and cloud saves
- **No other permissions required**

## Project Structure

```
duck-merge-game/
├── Assets/
│   ├── Scripts/
│   │   ├── GameConfig.cs          # Central configuration for game constants
│   │   ├── GameState.cs           # Game state enumeration
│   │   ├── GameManager.cs         # Main game manager (singleton)
│   │   ├── GameplayManager.cs     # Core gameplay mechanics
│   │   ├── Bird.cs                # Bird behavior and merge logic
│   │   ├── AudioManager.cs        # Audio system management
│   │   ├── MainMenuUI.cs          # Main menu UI controller
│   │   ├── GameUI.cs              # In-game UI controller
│   │   ├── GameOverUI.cs          # Game over screen controller
│   │   └── PauseUI.cs             # Pause menu controller
│   ├── Prefabs/                   # Bird prefabs and UI prefabs
│   ├── Scenes/                    # Game scenes (MainGame)
│   ├── Sprites/                   # Bird sprites and UI graphics
│   ├── Audio/                     # Music and sound effects
│   ├── Materials/                 # Physics materials
│   └── Plugins/                   # Google Play Games Services plugin
├── ProjectSettings/               # Unity project settings
├── Packages/                      # Unity package dependencies
└── README.md                      # This file
```

## Setup Instructions

### Prerequisites
1. **Unity Hub** and **Unity 2022.3.10f1** or later
2. **Android SDK** and **NDK** (installed via Unity Hub)
3. **JDK** (version 11 or later)
4. **Google Play Console** account (for Google Play Games Services)

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/davidbubb/duck-merge-game.git
   cd duck-merge-game
   ```

2. **Open in Unity**:
   - Open Unity Hub
   - Click "Open" and select the `duck-merge-game` folder
   - Unity will import all assets (may take a few minutes)

3. **Configure Android Build Settings**:
   - Go to `File > Build Settings`
   - Select "Android" platform
   - Click "Switch Platform"
   - Set minimum API level to 24 (Android 7.0)

4. **Setup Google Play Games Services** (Optional - for cloud features):
   - See [Google Play Games Configuration Guide](#google-play-games-configuration) below

### Building the Game

#### Debug Build (for testing)
1. Open `File > Build Settings`
2. Ensure Android is selected
3. Click "Build" and choose output location
4. Install APK on device: `adb install duck-merge-game-debug.apk`

#### Release Build
1. Open `File > Build Settings`
2. Select "Android" platform
3. Check "Development Build" OFF
4. Configure signing settings in Player Settings
5. Click "Build" and choose output location

### Running in Unity Editor
1. Open the `MainGame` scene from `Assets/Scenes/`
2. Click the Play button in Unity Editor
3. Use mouse clicks to drop birds
4. Test all game features

## Google Play Games Configuration

### Step 1: Create App in Google Play Console
1. Go to [Google Play Console](https://play.google.com/console)
2. Create a new application
3. Note your Application ID

### Step 2: Configure OAuth 2.0
1. In Google Play Console, go to "Play Games Services" > "Setup and management" > "Configuration"
2. Create OAuth 2.0 credentials
3. Download `google-services.json`
4. Place file in `Assets/Plugins/Android/`

### Step 3: Import Google Play Games Plugin
1. Download [Google Play Games Plugin for Unity](https://github.com/playgameservices/play-games-plugin-for-unity)
2. Import package into Unity project
3. Configure plugin with your Application ID

### Step 4: Setup Cloud Save
1. In Google Play Console, enable Saved Games feature
2. Configure leaderboard (optional for Phase 1)
3. Test sign-in and cloud save functionality

## Game Configuration

All game constants are centralized in `GameConfig` ScriptableObject:

### Editable Parameters
- **Bird Tiers**: Number of tiers (default: 11)
- **Bird Sizes**: Base size and multiplier per tier
- **Scoring**: Points per merge for each tier
- **Physics**: Gravity scale, bounciness, friction
- **Game Over**: Danger line height ratio, delay time
- **Container**: Width and height in world units
- **Audio**: Master volume, music volume, SFX volume

### Creating GameConfig Asset
1. In Unity, right-click in Project window
2. Select `Create > Duck Merge > Game Config`
3. Configure parameters in Inspector
4. Assign to GameManager in scene

## Controls

### In-Game
- **Mouse/Touch**: Move bird horizontally
- **Click/Tap**: Drop bird
- **Pause Button**: Pause game

### Menus
- Click/tap UI buttons to navigate

## Known Issues & Limitations

### Phase 1 MVP Limitations
- **Simple Graphics**: Birds use colored circles for MVP (placeholder art)
- **Basic Audio**: Uses placeholder sound effects
- **No Leaderboards**: Only local and cloud high score (global leaderboards in Phase 2)
- **No Achievements**: Achievement system planned for Phase 2
- **No Power-ups**: Power-ups planned for Phase 2
- **Single Game Mode**: Additional modes planned for Phase 2

### Technical Notes
- Merge detection uses collision callbacks (may have rare edge cases with rapid merges)
- Physics simulation optimized for 60 FPS on mid-range devices
- Cloud save requires active internet connection
- Google Sign-In requires properly configured OAuth credentials

## Testing Checklist

### Core Gameplay
- [ ] Birds drop and fall with realistic physics
- [ ] Two identical birds merge correctly
- [ ] All 11 tiers can be created through merging
- [ ] Score increases correctly for each merge
- [ ] Game over triggers when bird stays above danger line for 2+ seconds
- [ ] Physics runs smoothly at 60 FPS with 20+ birds

### UI/UX
- [ ] Main menu displays and navigates correctly
- [ ] Game UI shows current score and high score
- [ ] Pause menu appears and functions correctly
- [ ] Game over screen displays final score and high score
- [ ] All buttons respond to clicks/taps

### Audio
- [ ] Background music plays and loops
- [ ] Sound effects play for drops, merges, and game over
- [ ] Audio toggles work correctly
- [ ] Settings persist after restart

### Persistence
- [ ] High score saves locally
- [ ] High score syncs to cloud (when signed in)
- [ ] Audio settings persist
- [ ] Game state resets correctly on new game

### Google Sign-In
- [ ] Sign-in flow works correctly
- [ ] Guest mode allows local play
- [ ] Profile information displays
- [ ] Cloud save syncs high score
- [ ] Offline mode works gracefully

### Multi-Device Testing
- [ ] Test on phones with different screen sizes
- [ ] Test on tablets
- [ ] Test on different Android versions (7.0+)
- [ ] Test on different aspect ratios

## Asset Attribution

### Placeholder Assets (to be replaced)
- Bird graphics: Simple colored circles (Unity primitives)
- UI elements: Unity default UI components
- Sound effects: Free sound libraries (to be specified)
- Background music: Free music tracks (to be specified)

For production, replace with:
- Custom bird sprite artwork
- Professional UI graphics
- Licensed or original audio assets

## Development Timeline

### Phase 1 MVP (Current)
- ✅ Core game architecture
- ✅ Physics-based merge gameplay
- ✅ 11 bird tiers
- ✅ Scoring system
- ✅ Basic UI (all screens)
- ✅ Audio system
- ⏳ Google Sign-In integration
- ⏳ Visual polish and effects
- ⏳ Testing and optimization

### Phase 2 (Future)
- Achievements system
- Global leaderboards
- Daily challenges
- Power-ups
- Additional game modes
- Social features
- Monetization

## Contributing

This is a personal project, but feedback and suggestions are welcome!

## License

Copyright © 2026 David Bubb. All rights reserved.

## Contact

For questions or support:
- GitHub: [davidbubb/duck-merge-game](https://github.com/davidbubb/duck-merge-game)
- Issues: [Report bugs here](https://github.com/davidbubb/duck-merge-game/issues)

---

**Enjoy merging ducks! 🦆**
