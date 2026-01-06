# Wood Game 🪵
A 3D first-person/third-person simulation game developed in Unity as a final high school thesis. The player engages in wood harvesting, resource management, and exploration in a forest terrain.

<img src="Assets/Images/logo.png" alt="Wood Game Logo" width="256" height="256" style="image-rendering: pixelated;"/>

## Table of Contents 📚
- [About the Project ℹ️](#about-the-project-ℹ️)
- [Key Features ✨](#key-features-✨)
- [Game World 🌍](#game-world-🌍)
- [Controls 🎮](#controls-🎮)
- [Feature Roadmap (Programming)](#feature-roadmap-programming)
- [Feature Roadmap (Modeling)](#feature-roadmap-modeling--others)
- [Used Assets](#used-assets)
- [Used Materials](#used-materials)
- [Technologies 🔧](#technologies-🔧)

## About the Project ℹ️
This project documents the development of a video game focused on wood harvesting, economic systems, and environmental physics. The user controls a character that interacts with the game world, uses purchased tools (axes), and manages resources (wood/money).

## Key Features ✨
* **Physics-Based Chopping**: Trees are cut based on where the player hits them. Logs fall dynamically and react to gravity.
* **Economy System**: Earn "Shmeckles" ($H) by selling wood. Buy better axes and vehicles in the shop.
* **Save & Load System**: Saving game state (Player position, Inventory, Money, World objects, Vehicle position & cargo) into a JSON file.
* **Inventory System**: Pick up, carry, and store items.
* **Vehicle System**: Purchase and drive a truck to transport larger quantities of wood.
* **Dynamic UI**: Main menu, Pause menu, Settings (Graphics, Audio, Resolution), and interactive shop interface.
* **Custom Audio**: Original soundtrack and sound effects created specifically for the game.

## Game World 🌍
The map is divided into three distinct biomes, offering progression in difficulty and reward:
1.  **Oak Forest**: Starting area with the least valuable trees.
2.  **Cactus Forest**: Medium difficulty and value.
3.  **Frozen Forest**: Hardest to reach, contains the most valuable trees.

## Controls 🎮
* **WASD**: Movement
* **RMB + Move Mouse**: Look around
* **LMB (Left Click)**: Use item / Chop tree
* **E**: Interact / Pick up object / Enter car
* **ESC**: Pause Menu
* **C**: Toggle between First-Person and Third-Person view.
---

## Feature Roadmap (Programming)
✅ Movement mechanics + Collisions  
✅ Tree cutting physics with visible progress bar   
✅ Carry objects with cursor    
✅ Hide cursor in third person  
✅ Allow more cut points on log     
⚠️ Fix issue when switching from FP to TP to center the "hold point"    
✅️ Have equipped objects in your hand       
✅️ Implement inventory system   
✅️ Enable the player to carry an axe (allow cutting only with axe, carrying with hand)  
✅️ Implement game currency and add to UI     
✅️ Add player HP and add to UI   
✅️ Ability to sell logs at a specific location  
✅️ Allow player to hold an object on a specific point, not the mass center     
⚠️ Tree spawning system     
▶️ Vehicles for transporting logs   
✅️ Progress saving mechanics (JSON)     
✅️ Functional store, where you can buy axes     
✅️ Make each axe a different strength     
✅️ Add a popup to show the axe price    
❌ Save objects in car trunk   
❌ Make functional doors in houses      
❌ Fix controller bug when getting out of car      
❌ Fix loading screen sprite quality      
❌ Add option to show FPS in settings     
❌ Implement buying the car from the store      
❌ Implement fuel system for vehicles     
❌ Implement wood processing mechanics      
❌ Implement fishing mechanics      

## Feature Roadmap (Modeling + others)
✅️ Create Axe model     
✅️ Have more purchasable axes   
▶️ Create a basic game map with mountains as borders  
❌ Add rigged character sprite with animations  
✅️ A place to sell logs     
✅️ A store to buy a better axe for the currency     
✅️ Different wood textures + leaves     
▶️ Create UI     
✅️ Add Items in inventory to UI     
✅️ Create inventory item icons   
✅️ Create a main menu   
✅️ Make a functional settings menu   
❌ Vehicle model for transporting logs   
❌ NPC models to interact with   
✅️ Create a calm game soundtrack   
▶️ Add sound effects for chopping, walking, selling logs, etc.  
✅️ Create a logo for the game

---

## Used Assets
- [Unity FirstPerson Starter Asset](https://assetstore.unity.com/packages/essentials/starter-assets-firstperson-updates-in-new-charactercontroller-pa-196525)
- [Unity ThirdPerson Starter Asset](https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526)
- [Ezreal Car Controller](https://assetstore.unity.com/packages/tools/physics/ezereal-car-controller-302577)

## Used Materials
- [How to Make Beautiful Terrain in Unity 2020 - UGuruz](https://www.youtube.com/watch?v=ddy12WHqt-M)
- [Kickstart your game with First and Third Person Controllers! - Code Monkey](https://www.youtube.com/watch?v=jXz5b_9z0Bc)
- [How to Pick up and Drop Objects/Items! - Code Monkey](https://www.youtube.com/watch?v=2IhzPTS4av4)
- [SAVE & LOAD SYSTEM in Unity](https://www.youtube.com/watch?v=XOjd_qU2Ido)
- [How to make a Save & Load System in Unity](https://www.youtube.com/watch?v=aUi9aijvpgs)
- [5 Minute MAIN MENU Unity Tutorial](https://youtu.be/-GWjA6dixV4?si=QBcQC8476n_pqzoU)
- [Unity Loading Screen | Beginner Tutorial](https://www.youtube.com/watch?v=NyFYNsC3H8k)
- [SETTINGS MENU in Unity](https://www.youtube.com/watch?v=YOaYQrN1oYQ)

## Technologies 🔧
* **[Unity Engine](https://unity.com/)** (C#) — Core game engine & Physics
* **[Aseprite](https://www.aseprite.org/)** — 2D Pixel art (UI elements, Menu backgrounds)
* **[Bitwig Studio](https://www.bitwig.com/)** — Music composition & Sound design
* **[EZDrummer 3](https://www.toontrack.com/product/ezdrummer-3/)** — Drum synthesis
* **[Audacity](https://www.audacityteam.org/)** — Audio recording and post-processing
* **[Blender](https://www.blender.org/)** — 3D Modeling (External collaboration)

---
**Programming, 2D Art, Sound & Music by:** Ondřej Honus     
**3D Modeling, 2D Art by:** Matyáš Bezděk