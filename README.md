# 🎮 Numeris

> An immersive VR fantasy game designed to teach mathematics through exploration, combat, and interactive problem-solving.

---

<details>
<summary><strong>📑 Table of Contents (Click to Expand)</strong></summary>

- [Game Overview](#game-overview)
- [Installation Guide](#installation-guide)
- [System Requirements](#system-requirements)
- [Key Controls](#key-controls)
- [Features](#features)
- [Asset Credits](#asset-credits)

</details>

---

## Game Overview

**Numeris** is a VR math learning game that delivers education through immersive gameplay rather than passive instruction. Players explore a fantasy world where progress is achieved by solving math challenges and engaging in interactive combat.

The game uses familiar mechanics such as selecting answers, charging weapons, and defeating enemies to minimise the learning curve. This allows students to focus on understanding mathematical concepts while enjoying gameplay.

To support learners of varying skill levels, Numeris includes optional power-ups that can reduce difficulty while preserving the core educational challenge. These mechanics encourage critical thinking and active problem-solving.

![Lobby](Images/Lobby.jpeg)

---

## Installation Guide

### Required Software

- [Unity Hub](https://unity.com/download)
- [GitHub Desktop](https://github.com/apps/desktop)

### Setup Steps

1. Clone the repository using GitHub Desktop:
   - File → Clone Repository → URL
   - Repository URL:
     ```
     https://github.com/CheangWeiCheng/Numeris.git
     ```
   - Choose a local folder
   - Click **Clone**

2. Open the project in Unity Hub:
   - Select **Open → Add project from disk**
   - Navigate to the cloned project folder

3. Install required Unity packages:
   - Input System (Window → Package Manager)
   - Cinemachine

---

## System Requirements

**Platform:** Meta VR Devices  

### Recommended Specifications

| Component | Specification |
|---|---|
| OS | Meta Horizon OS |
| Processor | Snapdragon XR2 Gen 2 |
| CPU | Octa-core Kryo CPU |
| GPU | Adreno 740 |
| RAM | 8 GB |

---

## Key Controls

| Action | Control |
|---|---|
| Move | Left Joystick |
| Look | Right Joystick |
| Lock On | B / A Button |
| Open Level Menu | Y Button |
| Open Inventory | X Button |
| INteract with UI | Trigger |

---

## Features

### 🏪 Lobby

- Purchase health potions and power-ups using coins earned from enemies
- Select levels based on math topics:

| Level | Topic |
|---|---|
| The Crypt | Basic Arithmetic |
| The Great Hall | Geometry, Mass, Volume |
| The Spire | Fractions & Advanced Problems |

![Mountains](Images/Mountains.jpeg)

---

### 🗺️ Levels

- Checkpoints allow player progression
- Enemy defeat thresholds encourage learning engagement
- Hidden chests contain loot and power-ups

---

### 👾 Enemies & Combat

- Finite State Machine enemy AI
- A lock on system to detect and engage specific enemy for battle
- Battles triggered through interaction
- Math questions determine combat outcomes
- Floating orbs represent answer choices
- Correct answers charge the player’s staff
- Charged staff defeats enemies and grants rewards
- Enemy death animation and vfx

---

### ⚡ Power-Ups

| Power-Up | Effect |
|---|---|
| Extra-Life | Negate one instance of damage |
| Skip | Skip an enemy encounter |
| 50:50 | Remove two incorrect answers |
| Double-Dip | Allow two answer attempts |
| Slow Time | Increase answering time |

Collected power-ups are stored in the player inventory and accessed through the UI.

---

### ❓ Question System

- Incorrect answers trigger enemy chase mode
- Captured players lose health
- Zero health results in respawn at lobby
- Escaping enemy vision resets enemy patrol behaviour

---

## Asset Credits

### Environment Assets

- Trees:  
https://assetstore.unity.com/packages/3d/environments/lowpoly-environment-nature-free-medieval-fantasy-series-

- Terrain:  
https://assetstore.unity.com/packages/tools/terrain/procedural-terrain-painter-free-automatic-terrain-texturing-

### Tower Textures

- Roof: https://www.sharetextures.com/textures/wall/blue-brick-wall-  
- Wall: https://www.sharetextures.com/textures/wood/wood-parquet-  
- Wall Middle: https://www.sharetextures.com/textures/concrete/concrete-  
- Lining: https://www.sharetextures.com/textures/wood/wood-plank-  

### Chest Textures

- Wood: https://www.sharetextures.com/textures/wood/wood-plank-  
- Inner: https://www.sharetextures.com/textures/wood/wood-parquet-  
- Lining: https://www.sharetextures.com/textures/wood/dark_wood_parquet_  
- Metals: https://www.sharetextures.com/textures/metal/steel

---

