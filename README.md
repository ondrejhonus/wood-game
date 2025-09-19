# Wood Game 🪵  
A low poly style game developed in Unity, where you can cut down trees, collect the wood, process the wood and then maybe sell it.

## Table of Contents 📚  
- [Features ✨](#planned-features-✨)  
- [Game Logic](#game-logic)
- [Roadmap](#feature-roadmap)
- [Used Assets](#used-assets)
- [Technologies 🔧](#technologies-🔧)  

## Planned Features ✨  
#### **Will definetly happen:**
- **Wood Gathering**: Chop down trees, collect logs.
- **Basic map**: With interactable elements like trees and a sell point.
- **Player Model**: A simple humanoid character with animations.
- **Economy Management**: Sell wood to earn money, invest in better tools.
- **Tree generation**: Make the trees grow on random places on the map.
- **Vehicles for transport**: Purchase a vehicle to transport logs faster and more effectively.

#### **If it goes well:**
- **Process wood**: After gathering your wood, process it to make it gain value.
- **Local saving**: Save your progress to continue another day (locally).
- **NPCs**: Stationary NPCs throughout the map. 
- **Fishing + Cooking**: Fish for fishies to have food, cook them so they don't taste so bland.

#### **If it goes really well:**
- **Animations**: Animations corresponding to user action.
- **Car Modification**: Car mechanic for vehicle modification.
- **Clothing**: Make your character wear clothing of your choice. 
- **Building**: Use the wood to build yourself a house. 
- **Multiplayer Mode**: Cut down trees with your friends (Server based/Coop).
- **Cloud saving**: Save your progress to continue another day and never lose it.

## Game logic
- The player starts on a square with shops, where they buys the default axe and gets to know the place of his own plot.
- They then goes out to gather some trees, which they will sell at the wood sell point for some Shmeckles ($H).
- They can then use the money to buy better axes, a vehicle for transporting logs, or other items which will be implemented, etc. a wood processer.
- The vehicle will either be able to be upgraded (engine, wheel size, fuel tank size), or the player will simply buy a better one.
- They can refill their fuel for exchange for some $H at a specified gas station, that will appear at different set locations.
- There will be more types of forrests that will introduce more difficulty of getting into, and gear requirements such as better axes and more durable cars (Wood hardness, rivers for drowning small cars, hard terrain).
- The more far away and difficult the forest will be, the better quality wood it will contain (More expensive, heavier, thicker wood).
- There will also be a food system, they will either need to buy some food, or fish for fishies so they don't have to pay.


## Feature Roadmap
✅ Movement mechanics + Collisions  
✅ Tree cutting physics with visible progress bar   
✅ Carry objects with cursor    
▶️ Add rigged character sprite with animations  
✅ Hide cursor in third person  
✅ Allow more cut points on log     
❌ Fix issue when switching from FP to TP to center the "hold point"    
✅️ Have equipped objects in your hand       
✅️ Implement inventory system   
✅️ Enable the player to carry an axe (allow cutting only with axe, carrying with hand)  
▶️ Create at least some basic game map, which has some interactable objects such as a sell point     
❌ Add Items in inventory to UI     
▶️ Create UI     
✅️ Implement game currency and add to UI     
✅️ Add player HP and add to UI   
✅️ Ability to sell logs at a specific location  
✅️ Allow player to hold an object on a specific point, not the mass center      
❌ Vehicles for transporting logs   
❌ Tree spawning system     
❌ Progress saving mechanics    
❌ A store to buy a better axe for the currency     
❌ Add sound effects and background music   

## Used Assets
- [Unity FirstPerson Starter Asset](https://assetstore.unity.com/packages/essentials/starter-assets-firstperson-updates-in-new-charactercontroller-pa-196525)
- [Unity Thirdperson Starter Asset](https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526)

## Technologies 🔧  
- Unity engine (C#)
- Blender
