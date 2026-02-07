# Numeris

## Table of Contents

### 1.  Game Overview

### 2. Installation Guide

### 3. System Requirements

### 4. Key Controls

### 5. Features

### 6. Asset Credits

## Game Overview

Numeris is a VR math learning game that delivers math learning through an interactive and
immersive experience. By leveraging VR technology, students are placed directly into a
fantasy world where learning occurs through action and exploration rather than passive
consumption.

The game is designed around simple and familiar game mechanics, such as selecting answers,
charging a weapon, and defeating enemies, which many students are already exposed to
through mainstream games. This minimises the learning curve to playing the game, allowing
students to focus on understanding and applying math concepts instead.

To support learners of varying abilities, Numeris includes optional power-ups that can reduce
difficulty (e.g., slowing time or removing incorrect answers) without removing the core
learning challenge. These mechanics provide encouragement and assistance while still
requiring students to think critically and problem solve.

## Installation Guide

1.  Download and install Unity from https://unity.com/download
2.  Download and install GitHub Desktop from https://github.com/apps/desktop
3.  In GitHub Desktop
    i.  Go to File -> Clone Repository -> URL
    ii.  Input the URL https://github.com/CheangWeiCheng/Numeris.git
    iii. Choose your local path
    iv. Click Clone.
4. In Unity Hub

```
i.  Select "Open" → "Add project from disk"
ii.  Navigate to the cloned repository folder
```
5.  Ensure these packages are installed:
    i.  Input System (Window > Package Manager)
    ii.  Cinemachine (for camera controls)

## System Requirements

Platform: Meta 

#### Recommended Components

#### OS Meta Horizon  - OS

#### Processor  - Snapdragon XR2 Gen 2

#### CPU  - Octa-core Kryo CPU

#### GPU  - Adreno 740

#### RAM  - 8 GB

## Key Controls

#### Move Right  - Joystick

#### Look  - Left Joystick

#### Jump  - B

#### Interact  - A

#### Fire Weapon  - Trigger

#### Grab Object  - Grip


## Features

Lobby

At the shop, players can buyhealth potions or power-ups with coins gained from defeating enemies for use in the
levels to be able to better face tougher enemies

Players can pick their levels, each with their respective theme/subject:

1.  (The Crypt): Addition, Subtraction, Multiplication, Division.
2.  (The Great Hall): Geometry, Mass, and Volume.
3.  (The Spire): Fractions and complex problem-solving.

Levels

In each levels there are checkpoints which players can use to enter the next level and progress

To enter the next level, players have to defeat a certain number of enemies to reach the threshold so as to
Encourage learning in the game

There can also be chests you can find in the levels that you can open with keys to get loot, such as power-ups

Enemies

Encounter enemies patrolling with finite state machines

Interact with them to enter a battle

A battle system based on primary school math question sets, randomly chosen based on the level

Answers are written on orbs that float in front of the player

Pick up the correct answer orb and place it into the staff to charge it

Use your charged staff to kill the enemy to gain points, coins and power-ups

Power-Ups

Enemies can drop Power-Ups, which can grant advantages in battle

```
The Extra-Life Power-Up can allow players to prevent damage one time
The Skip Power-Up allows the player to skip an enemy
The 50:50 Power-Up allows players to remove two wrong answers
Double-Dip allows players to pick two different answers
Slow Time allows the player to have more time to answer
```
Power-Ups upon collection will be added to and can be used from the player's inventory, which can be accessed
with the UI

Questions

Should the player fail the question, the enemy’s chase state will be enabled, and the enemy will start chasing the
player

Should the enemy catch the player, they will lose health

If the player loses all their health, they will have to respawn back in the lobby


Should the player be able to evade the enemy’s view, the enemy will return to patrolling

## Asset Credits

Level 1 Trees

https://assetstore.unity.com/packages/3d/environments/lowpoly-environment-nature-free-
medieval-fantasy-series-

Level 1 Terrain

https://assetstore.unity.com/packages/tools/terrain/procedural-terrain-painter-free-automatic-
terrain-texturing-

Level 1 Tower Textures

Roof - https://www.sharetextures.com/textures/wall/blue-brick-wall-

Wall - https://www.sharetextures.com/textures/wood/wood-parquet-

Wall Middle - https://www.sharetextures.com/textures/concrete/concrete-

Lining - https://www.sharetextures.com/textures/wood/wood-plank-

Chest Textures

Wood - https://www.sharetextures.com/textures/wood/wood-plank-

Inner - https://www.sharetextures.com/textures/wood/wood-parquet-

Lining - https://www.sharetextures.com/textures/wood/dark_wood_parquet_

Metals - https://www.sharetextures.com/textures/metal/steel


