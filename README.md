# Generic Title / Hell Rush
This respository contains source code for Hell Rush, an Infinite Runner FPS prototype. 
The player runs on a 1-to-5 lane circuit, shooting enemies down, avoiding obstacles and picking up objects.

## Scripts
Only the scripts associated with the project have been uploaded. It features:
- AI scripts
  - Detector-based search algorithms for the enemy
- FPS scripts
  - Projectile system
- HUD scripts
  - Camera controller
  - Reload system
- Level Manager (2 game modes):
  - Easy to use, script-based level builder for story mode
  - Infinite runner, with dynamic and low-cost instantiation/destruction of assets
  - Render optimizations (based on camera orientation and player choices...)
- Player (and AI) movement
- Obstacles and Items
  - Pick-up mechanic
  - Moving obstacles
  - Destroyable obstacles
- Utils
  - Event handlers
