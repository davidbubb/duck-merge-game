# Unity Scene Setup Guide

This guide explains how to set up the Unity scenes and UI for the Duck Merge Game. Follow these steps after opening the project in Unity Editor.

## Prerequisites

1. Unity 2022.3.10f1 or later installed
2. Project opened in Unity Editor
3. All scripts already imported (GameManager.cs, Bird.cs, etc.)

## Scene Structure

The game uses a single scene architecture with UI panels that show/hide based on game state.

### Creating the Main Game Scene

1. **Create New Scene**:
   - File > New Scene
   - Choose "2D (URP)" or "2D" template
   - Save as `Assets/Scenes/MainGame.unity`

2. **Set Scene in Build Settings**:
   - File > Build Settings
   - Click "Add Open Scenes"
   - Ensure MainGame is at index 0

## Setting Up Game Objects

### 1. Game Manager Setup

1. Create empty GameObject: `GameObject > Create Empty`
2. Rename to "GameManager"
3. Add component: `GameManager` script
4. Create GameConfig asset:
   - Right-click in Project window
   - Create > Duck Merge > Game Config
   - Name it "GameConfig"
   - Assign to GameManager's Config field

### 2. Audio Manager Setup

1. Create empty GameObject: "AudioManager"
2. Add component: `AudioManager` script
3. Create two child GameObjects:
   - "MusicSource" with AudioSource component
   - "SFXSource" with AudioSource component
4. Assign audio sources to AudioManager fields

### 3. Google Play Manager Setup

1. Create empty GameObject: "GooglePlayManager"
2. Add component: `GooglePlayManager` script

## Creating the Game Container

### 1. Container GameObject

1. Create 2D Sprite: `GameObject > 2D Object > Sprite`
2. Rename to "Container"
3. Position: (0, 0, 0)
4. Add components:
   - BoxCollider2D (set to container size)
   - Configure as:
     - Size: (6, 10) - matches GameConfig
     - Is Trigger: No

### 2. Container Walls

Create four edge colliders for container boundaries:

**Bottom Wall**:
- Create empty GameObject: "BottomWall"
- Parent to Container
- Add EdgeCollider2D
- Set points: (-3, -5) to (3, -5)

**Left Wall**:
- Create empty GameObject: "LeftWall"
- Parent to Container
- Add EdgeCollider2D
- Set points: (-3, -5) to (-3, 5)

**Right Wall**:
- Create empty GameObject: "RightWall"
- Parent to Container
- Add EdgeCollider2D
- Set points: (3, -5) to (3, 5)

Note: No top wall - birds drop from above!

### 3. Danger Line

1. Create 2D Line Renderer or Sprite
2. Rename to "DangerLine"
3. Parent to Container
4. Position Y at 85% of container height (see GameConfig)
5. Make it red and semi-transparent
6. Scale to span container width

### 4. Drop Indicator

1. Create GameObject with LineRenderer
2. Rename to "DropIndicator"
3. Configure LineRenderer:
   - Positions: 2 points (will be set by code)
   - Width: 0.05
   - Color: White with 50% alpha
   - Material: Default Line material

### 5. Spawn Point

1. Create empty GameObject: "SpawnPoint"
2. Position: (0, 5.5, 0) - above container
3. This is where birds spawn

## Creating the Bird Prefab

### 1. Create Bird GameObject

1. Create 2D Sprite: `GameObject > 2D Object > Sprite > Circle`
2. Rename to "Bird"
3. Add components:
   - Rigidbody2D:
     - Body Type: Dynamic
     - Mass: 1
     - Linear Drag: 0
     - Angular Drag: 0.05
     - Gravity Scale: 2
   - CircleCollider2D:
     - Radius: 0.5
   - Bird script
4. Create prefab:
   - Drag Bird GameObject to `Assets/Prefabs/` folder
   - Delete from scene

### 2. Setup Gameplay Manager

1. Create empty GameObject: "GameplayManager"
2. Add component: `GameplayManager` script
3. Assign fields:
   - Game Config: The GameConfig asset
   - Bird Prefab: The Bird prefab from Assets/Prefabs/
   - Spawn Point: The SpawnPoint GameObject
   - Container: The Container GameObject
   - Danger Line: The DangerLine GameObject
   - Drop Indicator: The DropIndicator LineRenderer

## Creating the UI

### 1. Canvas Setup

1. Create UI Canvas: `GameObject > UI > Canvas`
2. Rename to "UICanvas"
3. Configure Canvas:
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1080 x 1920 (portrait)
     - Screen Match Mode: Match Width Or Height
     - Match: 0.5

### 2. Main Menu Panel

1. Create Panel: Right-click UICanvas > UI > Panel
2. Rename to "MainMenuPanel"
3. Add UI elements (all as children of MainMenuPanel):

   **Title Text**:
   - UI > Text - TextMeshPro
   - Text: "DUCK MERGE"
   - Font Size: 120
   - Alignment: Center
   - Position: Top center
   
   **Play Button**:
   - UI > Button - TextMeshPro
   - Text: "PLAY"
   - Size: 400 x 120
   - Position: Center
   
   **High Score Text**:
   - UI > Text - TextMeshPro
   - Text: "High Score: 0"
   - Position: Below title
   
   **Settings Button**:
   - UI > Button - TextMeshPro
   - Text: "⚙"
   - Size: 100 x 100
   - Position: Top right
   
   **Quit Button** (optional):
   - UI > Button - TextMeshPro
   - Text: "QUIT"
   - Size: 300 x 100
   - Position: Bottom

4. Add MainMenuUI script to MainMenuPanel
5. Assign UI references in Inspector

### 3. Game Panel

1. Create Panel: "GamePanel"
2. Set active to false initially
3. Add UI elements:

   **Score Text**:
   - UI > Text - TextMeshPro
   - Text: "Score: 0"
   - Position: Top left
   - Font Size: 48
   
   **High Score Text**:
   - UI > Text - TextMeshPro
   - Text: "Best: 0"
   - Position: Top right
   - Font Size: 36
   
   **Pause Button**:
   - UI > Button - TextMeshPro
   - Text: "||"
   - Size: 80 x 80
   - Position: Top right corner

4. Add GameUI script to GamePanel
5. Assign UI references

### 4. Pause Panel

1. Create Panel: "PausePanel"
2. Set active to false initially
3. Add semi-transparent dark background
4. Add UI elements:

   **Title Text**:
   - Text: "PAUSED"
   - Font Size: 80
   - Position: Top center
   
   **Resume Button**:
   - Text: "RESUME"
   - Size: 400 x 100
   - Position: Center
   
   **Restart Button**:
   - Text: "RESTART"
   - Size: 400 x 100
   - Position: Below Resume
   
   **Menu Button**:
   - Text: "MENU"
   - Size: 400 x 100
   - Position: Below Restart

5. Add PauseUI script to PausePanel
6. Assign UI references

### 5. Game Over Panel

1. Create Panel: "GameOverPanel"
2. Set active to false initially
3. Add UI elements:

   **Title Text**:
   - Text: "GAME OVER"
   - Font Size: 80
   - Position: Top
   
   **Final Score Text**:
   - Text: "Score: 0"
   - Font Size: 64
   - Position: Center
   
   **High Score Text**:
   - Text: "High Score: 0"
   - Font Size: 48
   - Position: Below score
   
   **New High Score Text**:
   - Text: "NEW HIGH SCORE!"
   - Font Size: 56
   - Color: Gold
   - Position: Below high score
   - Initially hidden
   
   **Retry Button**:
   - Text: "RETRY"
   - Size: 400 x 100
   - Position: Lower center
   
   **Menu Button**:
   - Text: "MENU"
   - Size: 400 x 100
   - Position: Below Retry

4. Add GameOverUI script to GameOverPanel
5. Assign UI references

### 6. Settings Panel

1. Create Panel: "SettingsPanel"
2. Set active to false initially
3. Add UI elements:

   **Title**:
   - Text: "SETTINGS"
   - Position: Top
   
   **Music Toggle**:
   - UI > Toggle
   - Label: "Music"
   
   **SFX Toggle**:
   - UI > Toggle
   - Label: "Sound Effects"
   
   **Music Volume Slider**:
   - UI > Slider
   - Label: "Music Volume"
   
   **SFX Volume Slider**:
   - UI > Slider
   - Label: "SFX Volume"
   
   **Player Name Text**:
   - Text: "Guest"
   - Position: Below sliders
   
   **Sign In Button**:
   - Text: "SIGN IN WITH GOOGLE"
   - Size: 400 x 100
   
   **Sign Out Button**:
   - Text: "SIGN OUT"
   - Size: 400 x 100
   - Initially hidden
   
   **Close Button**:
   - Text: "CLOSE"
   - Size: 400 x 100
   - Position: Bottom

4. Add SettingsUI script to SettingsPanel
5. Assign all UI references

## Camera Setup

1. Select Main Camera
2. Set:
   - Projection: Orthographic
   - Size: 10 (adjust to fit container)
   - Position: (0, 0, -10)
   - Background: Water blue color (#4A90E2)

## Lighting (2D)

1. Create 2D Global Light:
   - GameObject > Light > 2D > Global Light 2D
   - Intensity: 1
   - Color: White

## Physics Materials (Optional)

For more realistic physics:

1. Create Physics Material 2D:
   - Assets > Create > 2D > Physics Material 2D
   - Name: "BirdMaterial"
   - Friction: 0.4
   - Bounciness: 0.3

2. Assign to Bird prefab's CircleCollider2D

## Testing in Editor

1. **Play Mode**:
   - Press Play button
   - Main menu should appear
   - Click Play to start game
   - Test bird dropping with mouse

2. **Verify**:
   - Birds fall with physics
   - Merging works when identical birds touch
   - Score increases correctly
   - Game over triggers after 2 seconds above danger line
   - UI transitions work

3. **Debug**:
   - Check Console for errors
   - Use Debug.Log statements
   - Verify all component references are assigned

## Common Issues

### Objects Not Showing

- Check Z position (should be 0 for 2D objects)
- Verify camera size and position
- Check sorting layers

### Physics Not Working

- Verify Rigidbody2D is set to Dynamic
- Check Gravity Scale is > 0
- Ensure colliders are properly sized
- Check Physics2D settings

### UI Not Responding

- Verify EventSystem exists (auto-created with Canvas)
- Check Canvas render mode
- Ensure buttons have GraphicRaycaster
- Verify UI references are assigned in scripts

### Scripts Not Compiling

- Check for syntax errors in Console
- Verify all using statements are correct
- Ensure script names match class names
- Reimport all assets if needed

## Next Steps

After scene setup:

1. Add placeholder graphics (colored circles for birds)
2. Add audio assets (sound effects and music)
3. Test all game features
4. Build for Android
5. Test on device
6. Iterate and polish

## Advanced Setup (Optional)

### Particle Systems

For merge effects:
1. GameObject > Effects > Particle System
2. Configure for splash/sparkle effect
3. Create prefab
4. Spawn on merge

### Animations

For UI transitions:
1. Window > Animation
2. Create animations for panel transitions
3. Use Animation Controller

### Post-Processing

For visual polish:
1. Install URP or Post-Processing package
2. Add Bloom, Color Grading effects
3. Create Volume profile

---

## Quick Reference

### Scene Hierarchy

```
MainGame
├── GameManager
├── AudioManager
├── GooglePlayManager
├── GameplayManager
├── Container
│   ├── BottomWall
│   ├── LeftWall
│   ├── RightWall
│   └── DangerLine
├── SpawnPoint
├── DropIndicator
├── Main Camera
├── Global Light 2D
└── UICanvas
    ├── MainMenuPanel (MainMenuUI)
    ├── GamePanel (GameUI)
    ├── PausePanel (PauseUI)
    ├── GameOverPanel (GameOverUI)
    └── SettingsPanel (SettingsUI)
```

### Required Prefabs

- Bird prefab (in Assets/Prefabs/)

### Required Assets

- GameConfig ScriptableObject
- Bird sprites (11 tiers) - can use colored circles initially
- UI button/panel graphics
- Audio clips (music and SFX)

Good luck with your Unity setup! 🦆
