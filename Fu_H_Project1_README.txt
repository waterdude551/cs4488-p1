Harold Hou Fu
hfu86@gatech.edu
903856956

Press 'G' to regenerate a dungeon with the current DungeonGen settings.
	- If generation fails, please ensure dungeon width and height are sufficiently high compared to the max room size.
	- You can adjust the gain of room size and placement to make rooms more extremely sized or centrally placed. As a dungeon generates, the size automatically decreases and the placements automatically move towards the center.

Press 'R' to pseudorandomize the seed.
Press number keys 1-7 to adjust camera angle and speed.

Dungeon rooms themed after the Backrooms, with Fridges as the chests.
Floor's carpet texture is Perlin noise, wall and doorframe textures are third-party:
- walls @ https://www.deviantart.com/planetary4820/art/High-quality-backroom-s-wallpaper-texture-1341888350
- doorframes @ https://www.pinterest.com/ideas/wood-texture-seamless/947975305257/

In case you need to increase performance:
1. Disable the Light component in Assets/Prefabs/LightTile 
2. Enable the Directional Light in the Hierarchy.
3. Set "Rendering Path" in the Camera component of Main Camera to "Forward"