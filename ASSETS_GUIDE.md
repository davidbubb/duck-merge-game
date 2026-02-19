# Assets Guide for Duck Merge Game

This guide explains what assets are needed for the game and where to find or create them.

## Overview

The Duck Merge Game MVP requires:
- **Bird Sprites** (11 tiers)
- **UI Graphics** (buttons, panels, backgrounds)
- **Audio** (music and sound effects)
- **Fonts** (UI text)

## Quick Start: Placeholder Assets

For rapid MVP development, you can use:

### 1. Bird Sprites (Auto-Generated)

Use the built-in sprite generator:

1. Open Unity Editor
2. Create a GameConfig asset (if not already created)
3. Go to **Duck Merge > Generate Bird Sprites**
4. Assign your GameConfig
5. Click "Generate All Bird Sprites"
6. Simple colored circles will be created in `Assets/Sprites/Birds/`

This creates basic placeholder sprites that are functional for testing.

### 2. UI Graphics (Unity Defaults)

Unity provides default UI components:
- Use Unity's default Button graphics
- Use solid color panels
- Create simple backgrounds with solid colors or gradients

### 3. Audio (Free Resources)

#### Background Music
Free sources:
- **Incompetech** (https://incompetech.com/)
  - Search for: "calm", "upbeat", "casual game"
  - Royalty-free with attribution
  
- **Free Music Archive** (https://freemusicarchive.org/)
  - Filter by: "Game", "Casual"
  
- **Bensound** (https://www.bensound.com/)
  - Free tracks with attribution

Recommended tracks style:
- Calm, upbeat melodies
- 60-120 BPM
- Acoustic/light electronic
- Loop-friendly

#### Sound Effects
Free sources:
- **Freesound.org** (https://freesound.org/)
  - Search for: "plop", "splash", "bubble", "pop", "chime"
  - CC0 or CC-BY licensed sounds
  
- **Zapsplat** (https://www.zapsplat.com/)
  - Free sound effects library
  
- **Mixkit** (https://mixkit.co/free-sound-effects/)
  - Free sound effects

Required sound effects:
1. **Drop Sound**: Soft "plop" or water droplet sound
2. **Merge Sounds** (5-7 variations):
   - Low pitch for low tiers
   - Higher pitch for higher tiers
   - "Pop" or "splash" sounds work well
3. **Game Over Sound**: Sad trombone or descending tone
4. **Button Click**: Quick "tick" or "click"
5. **Swan Sound** (tier 11): Special triumphant sound (chime, fanfare)

### 4. Fonts (Free)

Unity includes TextMeshPro with several free fonts. For additional fonts:

- **Google Fonts** (https://fonts.google.com/)
  - Recommended: "Fredoka", "Rubik", "Poppins", "Quicksand"
  - Download .ttf file
  - Import into Unity
  - Create TextMeshPro font asset

## Asset Organization

Organize assets in Unity project:

```
Assets/
├── Sprites/
│   ├── Birds/
│   │   ├── Bird_Tier01_Duckling.png
│   │   ├── Bird_Tier02_BabyDuck.png
│   │   ├── ...
│   │   └── Bird_Tier11_Swan.png
│   ├── UI/
│   │   ├── Buttons/
│   │   ├── Panels/
│   │   └── Backgrounds/
│   └── Effects/
│       └── Particles/
├── Audio/
│   ├── Music/
│   │   ├── BackgroundMusic_01.mp3
│   │   └── BackgroundMusic_02.mp3
│   └── SFX/
│       ├── Drop.wav
│       ├── Merge_01.wav
│       ├── Merge_02.wav
│       ├── Merge_03.wav
│       ├── GameOver.wav
│       ├── ButtonClick.wav
│       └── Swan_Special.wav
├── Fonts/
│   └── CustomFont.ttf
└── Materials/
    └── BirdMaterial.physicsMaterial2D
```

## Detailed Asset Specifications

### Bird Sprites

**Format**: PNG with transparency
**Size**: 512x512 pixels recommended
**Style**: Simple, colorful, cute
**Requirements**:
- Distinct color for each tier (see GameConfig)
- Circular or oval shape for physics
- Progressive size increase (handled by Unity scale)

**Tier Colors** (from GameConfig):
1. Yellow (Duckling)
2. Gold (Baby Duck)
3. Tan (Juvenile Duck)
4. Forest Green (Mallard)
5. Brown (Wood Duck)
6. Orange (Mandarin Duck)
7. White (Goose)
8. Dark Brown (Canada Goose)
9. Light Yellow (Pelican)
10. Blue Grey (Great Heron)
11. Pure White (Swan)

### UI Graphics

**Main Menu Background**:
- Size: 1080x1920 (portrait)
- Style: Water-themed, calm blues/teals
- Gradient or simple pattern

**Container Background**:
- Transparent PNG or solid color
- Light blue/teal color
- Optional: wave patterns

**Buttons**:
- Normal state: 400x120 pixels
- Small buttons: 100x100 pixels
- Style: Rounded corners, solid colors
- Include hover/pressed states (optional for MVP)

**Panels**:
- Semi-transparent backgrounds
- Rounded corners
- Drop shadow or glow (optional)

### Audio Specifications

**Music**:
- Format: MP3 or OGG
- Length: 1-3 minutes (loops)
- Bitrate: 128-192 kbps
- Volume: Normalized to -6dB to -3dB peak
- Fade in/out at loop points

**Sound Effects**:
- Format: WAV or OGG
- Length: 0.1-1 second
- Sample Rate: 44.1 kHz
- Bit Depth: 16-bit
- Mono (saves space)
- Volume: Normalized to -6dB peak

## Creating Custom Assets

### Bird Sprites (Advanced)

If creating custom sprites:

1. **Design Principles**:
   - Cute, friendly style
   - Clear silhouettes
   - Distinct features per tier
   - Vibrant colors

2. **Tools**:
   - Adobe Illustrator (vector)
   - Affinity Designer (vector)
   - Procreate (iPad)
   - GIMP (free, raster)
   - Inkscape (free, vector)

3. **Process**:
   - Sketch designs
   - Create vector or high-res raster
   - Export as PNG with transparency
   - Test in Unity

### Background Music (Advanced)

If creating custom music:

1. **Tools**:
   - GarageBand (Mac/iOS)
   - FL Studio
   - Ableton Live
   - Caustic (mobile)

2. **Style Guide**:
   - Tempo: 80-120 BPM
   - Instruments: Piano, strings, light percussion, marimba
   - Mood: Relaxing, cheerful, playful
   - Structure: Simple, repetitive patterns
   - No jarring transitions

### Sound Effects (Advanced)

Create custom SFX:

1. **Tools**:
   - Audacity (free)
   - Bfxr (retro game sounds, free)
   - ChipTone (chiptune sounds, free)

2. **Tips**:
   - Layer multiple sounds for richness
   - Add reverb for depth
   - Pitch shift for variations
   - Compress/limit to prevent clipping

## Importing Assets into Unity

### Sprites

1. Drag PNG files into `Assets/Sprites/` folder
2. Select sprite in Project window
3. In Inspector, set:
   - **Texture Type**: Sprite (2D and UI)
   - **Pixels Per Unit**: 100 (or custom)
   - **Filter Mode**: Bilinear
   - **Format**: Compressed or Truecolor
4. Click "Apply"

### Audio

1. Drag audio files into `Assets/Audio/` folder
2. Select audio clip in Project window
3. In Inspector, set:
   - **Load Type**: 
     - Music: Streaming
     - SFX: Decompress On Load
   - **Compression Format**:
     - Music: Vorbis (quality 70-80%)
     - SFX: ADPCM or PCM
4. Click "Apply"

### Fonts

1. Drag .ttf file into `Assets/Fonts/` folder
2. Right-click and select "Create > TextMeshPro > Font Asset"
3. Configure font atlas settings
4. Use in TextMeshPro components

## Asset Budget (Mobile)

Keep within these limits for good performance:

- **Total app size**: < 100 MB
- **Texture memory**: < 50 MB
- **Audio memory**: < 20 MB
- **Sprite count**: < 100 unique sprites
- **Audio clips**: < 30 clips

### Optimization Tips

1. **Sprites**:
   - Use sprite atlases to reduce draw calls
   - Compress textures (ASTC for Android)
   - Share sprites between tiers where possible

2. **Audio**:
   - Use Vorbis compression for music
   - Use ADPCM for short SFX
   - Keep music tracks under 3 MB each
   - Keep SFX under 100 KB each

3. **General**:
   - Remove unused assets before build
   - Use Unity's AssetBundle for future content
   - Test on low-end devices

## Production-Ready Assets (Phase 2+)

For polished release, consider:

### Professional Art
- Hire artist on Fiverr/Upwork
- Commission on ArtStation
- Buy asset packs on Unity Asset Store

### Licensed Music
- Purchase royalty-free tracks
- Commission original music
- Use premium libraries (Epidemic Sound, Artlist)

### Professional SFX
- Purchase SFX libraries
- Record custom sounds
- Hire sound designer

### Fonts
- Purchase premium game fonts
- Commission custom font
- Use high-quality free fonts

## Asset Checklist

Before building:

- [ ] All 11 bird sprites created and imported
- [ ] Bird sprites assigned to bird prefab variants
- [ ] Background image/gradient for game area
- [ ] UI buttons have graphics (or use Unity default)
- [ ] At least 1 background music track added
- [ ] All required SFX imported (drop, merge, game over, button, swan)
- [ ] Audio clips assigned to AudioManager in scene
- [ ] Font selected and applied to all UI text
- [ ] All assets properly organized in folders
- [ ] No "missing asset" errors in console
- [ ] Build size under 100 MB

## Testing Assets

Test each asset:

1. **Sprites**: Visible in game, correct size, no pixelation
2. **Audio**: Plays correctly, good volume, no distortion
3. **UI**: Readable text, buttons clickable, proper layout
4. **Performance**: 60 FPS maintained with all assets loaded

## Replacing Placeholder Assets

To swap placeholder assets later:

1. Create new asset with same name
2. Replace file in Assets folder
3. Unity will auto-update references
4. Test to ensure no broken references

Or:

1. Import new asset
2. Manually reassign in Inspector
3. Delete old asset

## Resources

### Asset Marketplaces
- Unity Asset Store
- itch.io
- OpenGameArt.org
- Kenney.nl (free game assets)

### Tools
- GIMP (free image editor)
- Audacity (free audio editor)
- Inkscape (free vector graphics)
- Blender (free 3D/2D)

### Learning
- Unity Learn (tutorials)
- YouTube (asset creation tutorials)
- Game Dev Market (tutorials + assets)

---

Good luck creating your assets! 🎨🎵
