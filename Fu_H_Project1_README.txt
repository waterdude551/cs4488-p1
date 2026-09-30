Harold Hou Fu
hfu86@gatech.edu
903856956

Press 'G' to regenerate a dungeon with the current DungeonGen settings.
	- If generation fails, please ensure dungeon width and height are sufficiently high compared to the max room size. Max room size must also be at least min room size for rooms to generate.
	- You can adjust the gain of room size and placement to make rooms more extremely sized or centrally placed. As a dungeon generates, the size automatically decreases and the placements automatically move towards the center.
		- Recall that gain closer to 0 creates centered values, while gain closer to 1 creates extreme values. Gain at 0.5 is uniformly random.
	- You can adjust the spacing and max jitter of the fridge placement. If jitter exceeds half of spacing, the chances of a fridge spawning inside another become non-zero.

Press 'R' to pseudorandomize the seed.
Press number keys 1-7 to adjust camera angle and speed. '1' is first-person, while '7' is fully zoomed out and top-down.
Hold 'Left Shift' to sprint, moving 1.5x faster.
Approach a fridge in first-person view to open it.

Dungeon rooms themed after the Backrooms, with Fridges as the chests.

Floor's carpet texture is Perlin noise, wall and doorframe textures are third-party:
- walls @ https://www.deviantart.com/planetary4820/art/High-quality-backroom-s-wallpaper-texture-1341888350
- doorframes @ https://www.pinterest.com/ideas/wood-texture-seamless/947975305257/

In case you need to increase performance:
1. Disable the Light component in Assets/Prefabs/LightTile 
2. Enable the Directional Light in the Hierarchy.
3. Set "Rendering Path" in the Camera component of Main Camera to "Forward"