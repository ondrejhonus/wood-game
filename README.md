# Wood Game 🪵  
A low poly style game developed in Unity, where you can cut down trees, collect the wood, process the wood and then maybe sell it.

## Table of Contents 📚  
- [Features ✨](#features-✨)  
- [Process](#process)  
- [Technologies 🔧](#technologies-🔧)  

## Planned Features ✨  
#### **Basic:**
- **Basic map**: With interactable elements like trees and a sell point.
- **Wood Gathering**: Chop down trees, collect logs.
- **Economy Management**: Sell wood to earn money, invest in better tools.
- **Player Model**: Simple looking blocky character.

#### **If it goes well:**
- **Process wood**: After gathering your wood, process it to make it gain value.
- **Random tree generation**: Make the trees grow on random places on the map.
- **Vehicles for transport**: Use your vehicle to transport wood faster.
- **Local saving**: Save your progress to continue another day (locally).
- **NPCs**: Stationary NPCs throughout the map. 

#### **Final steps:**
- **Animations**: Animations corresponding to user action.
- **Car Modification**: Car mechanic for vehicle modification.
- **Clothing**: Make your character wear clothing of your choice. 
- **Building**: Use the wood to build yourself a house. 
- **Multiplayer Mode**: Cut down trees with your friends (Server based/Coop).
- **Cloud saving**: Save your progress to continue another day and never lose it.

## Game logic
- The player starts on a square with shops, where he buys the default axe and gets to know the place of his own plot
- He then goes out to gather some trees, which he will sell at the wood sell point for some Shmeckles ($H)
- He can then use the money to buy better axes, a vehicle for transporting logs, or other items which will be implemented, etc. a wood processer
- The vehicle will either be able to be upgraded (engine, wheel size, fuel tank size), or the player will simply buy a better one
- You can refill your fuel for exchange for some $H at a specified gas station, that will appear at different set locations
- There will be more types of forrests that will introduce more difficulty of getting into, and gear requirements such as better axes and more durable cars (Wood hardness, rivers for drowning small cars, hard terrain)
- The more far away and difficult the forest will be, the better quality wood it will contain (More expensive, heavier, thicker wood)


## Feature Roadmap (changes periodically) 🗺️
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
❌ Implement game currency  
❌ Add player HP    
❌ Ability to sell logs at a specific location  
❌ Allow player to hold an object on a specific point, not the mass center      
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
