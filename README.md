# CM3030 – Custom Car Driving System (Unity) 🚗

- A Unity-based driving prototype built for the CM3030 Game Development module, focusing on creating a believable and responsive car driving experience in a compact city environment.

- The project explores how physics, level design, UI, and audio work together to create a simple but immersive driving experience with different weather and time-of-day modes.

  ---
## Project Overview 🎯
  This project aims to build a small but realistic driving experience where:
- The car feels stable and predictable to control
- A compact city provides clear navigation and landmarks
- Players can choose between different moods (Day / Night / Rainy)
- The world feels alive through audio and environmental design

The focus is on playability, clarity, and performance, not full simulation complexity.

---

## Features Implemented 🎮
### Start Menu System 🚦
- Left-side UI menu with 3 modes:
  - Day
  - Night
  - Rainy
- One-click scene start
- Persistent game state using a settings manager
- Clean scene transition flow

---

## Weather & Mood System 🌤️ 
- Predefined environmental presets:
   - Day (bright lighting)☀️
   - Night (low light, readable darkness)🌙
   - Rainy (rain particles + wet effect + ambience)🌧️ 
- Lightweight system using presets instead of dynamic simulation
  
---

## Vehicle Controller 🚗 
- Unity WheelCollider-based physics
- Smooth steering, braking, and acceleration
- Speed-based steering reduction for stability
- Lowered center of mass for realistic handling feel
- HUD-integrated speed feedback

---

## City Environment 🏙️ 
- Compact drivable city block
- Long straights + readable corners
- Landmark-based navigation (buildings for reference points)
- Optimized with:
   - Static batching
   - LOD Groups
   - Shared materials

---

## Audio System 🎵 
- Engine sound with speed-based pitch variation
- Brake and skid sound effects
- Mode-based ambience:
    - Day: birds / wind
    - Night: calm city ambience
    - Rainy: rain loop
- UI click feedback sounds
- Basic audio mixer separation (SFX / UI / Ambience)

---

## HUD System 🧭 
- Real-time speed display
- Simple timer for testing runs
- Clean, non-intrusive UI layout using TextMeshPro

---
## System Architecture 🧱

### Core Scripts
- GameSettings.cs → Stores selected mode across scenes  
- MenuController.cs → Handles mode selection + scene loading  
- SceneInitializer.cs → Applies correct mood settings on load  
- PlayerCarController.cs → Vehicle physics and control  
- Speedometer.cs → HUD speed display  
- Timer.cs → Run timing system  

---

## How It Works ⚙️

- Player launches game  
- Selects a mode (Day / Night / Rainy)  
- Scene loads with saved state  
- SceneInitializer applies correct lighting + effects  
- Player drives in a compact city environment  
- HUD + audio provide feedback during gameplay  

---

## Testing Summary 🧪

The project was tested using repeatable driving routes under all 3 modes.

### Successfully Tested ✔️
- Menu → scene transitions  
- Car handling stability  
- HUD accuracy  
- City readability  
- Performance consistency  
---
## Game Interface 🎮

---

## Performance Notes 📊

- Smooth frame rate in Day and Night modes  
- Slight FPS drop in Rainy mode due to particles  
- Optimized using static batching and LODs  
- Stable across repeated runs and scene reloads  

---

## Tech Stack 🛠️

- Unity Engine  
- C# scripting  
- WheelCollider physics  
- TextMeshPro UI  
- Unity Audio System  
- Basic particle systems  

---

## Project Structure 📁

```
CM3030/
├── Assets/
│   ├── Scripts/
│   │   ├── GameSettings.cs
│   │   ├── MenuController.cs
│   │   ├── SceneInitializer.cs
│   │   ├── PlayerCarController.cs
│   │   ├── Speedometer.cs
│   │   └── Timer.cs
│   ├── Scenes/
│   ├── Prefabs/
│   ├── Audio/
│   └── UI/
```

---
## Built By 🙋‍♀️😄

Cheparthi Sri Nikhitha
- [LinkedIn](https://www.linkedin.com/in/cheparthi-sri-nikhitha-886b381b1)
- [Portfolio](https://nikhithaprofessionalportfolio.netlify.app/)
  
