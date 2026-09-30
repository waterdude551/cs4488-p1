using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    FridgeGenerator fridgeGen;
    [SerializeField]
    GameObject lightPrefab;
    // Define the grid size.
    public int dungeonWidth;
    public int dungeonHeight;

    [SerializeField]
    int seed;

    // Room generation fields
    [SerializeField] 
    int maxRoomSize;
    [SerializeField]
    int minRoomSize;
    int currMaxRoomSize; // Max side length of a room. Biased to medium values.
    [SerializeField]
    int maxPlacementAttempts;
    [SerializeField, Range(0f, 1f)]
    float roomSizeGain; // Closer to 1 = more extreme values.
    [SerializeField, Range(0f, 1f)]
    float startingPlacementGain; // Closer to 1 = more extreme values.
    float roomPlacementGain; // Closer to 1 = more extreme values.
    bool generationActive;
    
    private int[,] grid;
    private int nextRoomWidth;
    private int nextRoomHeight;
    private int roomNumber;
    void Awake()
    {
        FullGenerateDungeon();
    }

    void FullGenerateDungeon()
    {
        InitDungeon();
            if (transform.childCount > 0)
                DeleteDungeon();
            GenerateRooms();
            FormWalls();
            FormDoors();
            // LogTiles();
            fridgeGen.JitterPlaceFridges();
            SpawnDungeon();
    }
    void Start()
    {
        
    }

    /// <summary>
    /// Initializes fields for creating a new dungeon.
    /// </summary>
    void InitDungeon()
    {
        Random.InitState(seed);
        generationActive = true;
        roomNumber = 0;
        grid = new int[dungeonHeight,dungeonWidth];
        currMaxRoomSize = maxRoomSize;
        roomPlacementGain = startingPlacementGain;
        transform.position = new Vector3(-dungeonWidth*2.5f, 0, dungeonHeight*2.5f);
        fridgeGen.transform.position = transform.position;
    }

    /// <summary>
    /// Randomizes a width and height for the next room.
    /// </summary>
    void RandomizeSize()
    {
        float rw = Random.value;
        float rh = Random.value;
        // Random integer in min-max range, bias to center
        nextRoomWidth = (int) ((currMaxRoomSize-minRoomSize) * Rnd.gain(rw,roomSizeGain)) + minRoomSize;
        nextRoomHeight = (int) ((currMaxRoomSize-minRoomSize) * Rnd.gain(rh,roomSizeGain)) + minRoomSize;
        // Debug.Log("Sized room with width " + nextRoomWidth + " and height " + nextRoomHeight);
    }

    /// <summary>
    /// Attempts to find a placement for a room, shrinking and centering until no more placements are valid.
    /// </summary>
    void FindRoomPlacement()
    {
        int placementAttempts = 0;
        bool placementFailed = false;
        
        // attempt up to n placements
        while (placementAttempts < maxPlacementAttempts)
        {
            placementAttempts++;
            placementFailed = false;
            // try random
            RandomizeSize();
            int currX = (int) (Rnd.gain(Random.value, roomPlacementGain) * (dungeonWidth - nextRoomWidth + 1));
            int currY = (int) (Rnd.gain(Random.value, roomPlacementGain) * (dungeonHeight - nextRoomHeight + 1));
            // Debug.Log("Attempting room placement at " + currX + ", " + currY);
            
            // check room collision
            for (int y = currY; y < currY + nextRoomHeight; y++) 
            {
                for (int x = currX; x < currX + nextRoomWidth; x++)
                {
                    // Debug.Log("With size " + nextRoomWidth + "," + nextRoomHeight + ", checking at " + x + "," + y);
                    // blocked if any adjacents aren't clear (needlessly redundant but not particularly slow)
                    if (grid[y,x] != 0 
                    || grid[Mathf.Max(0,y-1),x] != 0 
                    || grid[y,Mathf.Max(0,x-1)] != 0
                    || grid[Mathf.Min(dungeonHeight-1,y+1),x] != 0 
                    || grid[y,Mathf.Min(dungeonWidth-1,x+1)] != 0
                    // diagonals: ul, bl, ur, br
                    || grid[Mathf.Max(0,y-1),Mathf.Max(0,x-1)] != 0 
                    || grid[Mathf.Min(dungeonHeight-1,y+1),Mathf.Max(0,x-1)] != 0 
                    || grid[Mathf.Max(0,y-1),Mathf.Min(dungeonWidth-1,x+1)] != 0 
                    || grid[Mathf.Min(dungeonHeight-1,y+1),Mathf.Min(dungeonWidth-1,x+1)] != 0)
                    {
                        // failed to place room; eject from for-loops
                        y = currY + nextRoomHeight;
                        x = currX + nextRoomWidth;
                        placementFailed = true;
                        
                    }
                }
            }

            if (!placementFailed)
            { // Populate room
                Debug.Log("Found valid placement at " + currX + ", " + currY + " after " + placementAttempts + " attempts.");
                roomNumber++;
                for (int y = currY; y < currY + nextRoomHeight; y++) 
                {
                    for (int x = currX; x < currX + nextRoomWidth; x++)
                    {
                        grid[y,x] = roomNumber;
                    }
                }
                // maybe cut a corner out
                if (Random.value < excisionRate)
                {
                    ExciseRoom(currX,currY, nextRoomWidth, nextRoomHeight);
                }

                // end while
                placementAttempts = maxPlacementAttempts;
            }
        } 

        
        if (placementFailed)
        { // Termination logic
            currMaxRoomSize--; // shrink room size
            roomPlacementGain *= 0.95f; // tend to center
            if (currMaxRoomSize < minRoomSize || roomPlacementGain <= 0) // room size collapse
            {
                Debug.Log("Failed to find room placement. Marking generation as complete.");
                generationActive = false;
            } else {
                Debug.Log("Failed to find room placement within " + maxPlacementAttempts + " attempts. Reducing max dimensions to " + currMaxRoomSize);
            }
            
        }
    }

    /// <summary>
    /// Fully populates a dungeon with rooms.
    /// </summary>
    void GenerateRooms()
    {
        InitDungeon();
        while (generationActive)
        {
            FindRoomPlacement();
        }
        Debug.Log("Generated " + roomNumber + " rooms");
    }

    void Update()
    {



        // DEBUG:
        ReadDebugInputs();
    }

    /// <summary>
    /// Input for debug logs.
    /// </summary>
    void ReadDebugInputs()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha1)) {
        //     GenerateRooms();
        //     LogGrid();
        // }
        // if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     FormWalls();
        //     FormDoors();
        //     LogTiles();
        //     SpawnDungeon();
        // }
        if (Input.GetKeyDown(KeyCode.G))
        {
            FullGenerateDungeon();
        }
        if (Input.GetKeyDown(KeyCode.R)) { // i just realized this isn't very random
            seed = (int) (Random.value * 10000000);
        }
    }

    /// <summary>
    /// Print the generation grid to console.
    /// </summary>
    void LogGrid()
    {   
        string grid = "";
        // Discard rightmost column and bottommost row
        for (int y = 0; y < dungeonHeight; y++)
        {
            for (int x = 0; x < dungeonWidth; x++)
            {
                grid += " [" + this.grid[y, x] + "] ";
            }
            grid += "\n";
        }
        Debug.Log(grid);
    }
    
    [SerializeField] // prefab room walls
    GameObject[] tiles;
    public int[,] tileMap;
    GameObject dungeon;
    /// <summary>
    /// Using the current dungeon grid, instantiate tiles forming the dungeon.
    /// </summary>
    void FormWalls()
    {
        dungeon = new GameObject("dungeon");
        dungeon.transform.SetParent(transform);
        dungeon.transform.localPosition = Vector3.zero;
        tileMap = new int[dungeonHeight, dungeonWidth];
        // for each cell, wall top and left if edge is dungeon edge or different num
        // sufficient for rectangular rooms: tilemap with 9 options
        // 9-slice 012345678 where 0 is empty, 1,3,6,8 are nw,ne,sw,se corners, 2,4,5,7 are n,w,e,s edges
        for (int y = 0; y < dungeonHeight; y++)
        {
            for (int x = 0; x < dungeonWidth; x++)
            {
                int gNum = grid[y,x];
                
                // NW case: N and W are dungeon edge or different roomNum
                if ((x == 0 || grid[y,x-1] != gNum && gNum != 0) && (y == 0 || grid[y-1,x] != gNum && gNum != 0))
                {
                    tileMap[y,x] = 8;
                    continue;
                }
                // SW case: S and W are edge or different
                if ((x == 0 || grid[y,x-1] != gNum && gNum != 0) && (y == dungeonHeight-1 || grid[y+1,x] != gNum && gNum != 0))
                {
                    tileMap[y,x] = 7;
                    continue;
                }
                // NE case: N and E are edge or different
                if ((x == dungeonWidth-1 || grid[y,x+1] != gNum && gNum != 0) && (y == 0 || grid[y-1,x] != gNum && gNum != 0))
                {
                    tileMap[y,x] = 5;
                    continue;
                }
                // SE case: S and E are edge or different
                if ((x == dungeonWidth-1 || grid[y,x+1] != gNum && gNum != 0) && (y == dungeonHeight-1 || grid[y+1,x] != gNum && gNum != 0))
                {
                    tileMap[y,x] = 6;
                    continue;
                }
                // N,S,W,E case: direction is edge or different
                // if hallway, only wall if edge of dungeon
                if (y == 0 || grid[y-1,x] != gNum && gNum != 0)
                {
                    tileMap[y,x] = 1; // N
                    continue;
                }
                if (y == dungeonHeight-1 || grid[y+1,x] != gNum && gNum != 0)
                {
                    tileMap[y,x] = 2; // S
                    continue;
                }
                if (x == 0 || grid[y,x-1] != gNum && gNum != 0)
                {
                    tileMap[y,x] = 4; // W
                    continue;
                }
                if (x == dungeonWidth-1 || grid[y,x+1] != gNum && gNum != 0)
                {
                    tileMap[y,x] = 3; // E
                    continue;
                }
                
                // otherwise, 0
            }
        }

    }

    /// <summary>
    /// Deletes the first child of the dungeon generator: the first remaining dungeon.
    /// </summary>
    void DeleteDungeon()
    {
        GameObject.Destroy(transform.GetChild(0).gameObject);
    }
    /// <summary>
    /// Prints tilemap to console.
    /// </summary>
    void LogTiles()
    {   
        string grid = "";
        for (int y = 0; y < dungeonHeight; y++)
        {
            for (int x = 0; x < dungeonWidth; x++)
            {
                grid += " [" + this.tileMap[y, x] + "] ";
            }
            grid += "\n";
        }
        // Note rightmost and bottom not discarded. yet.
        Debug.Log(grid);
    }

    /// <summary>
    /// Randomly adds doors to connect rooms to hallways.
    /// </summary>
    void FormDoors()
    {
        // for each room, break down a random non-edge NSEW wall
        for (int roomNum = 0; roomNum < roomNumber; roomNum++)
        {
            int perimeter = 0;
            List<Vector2> perimCoords = new List<Vector2>();
            // skip edges of dungeon
            // unfortunately, loop twice to find perimeter then randomly place a door along it
            for (int y = 1; y < dungeonHeight-1; y++)
            {
                for (int x = 1; x < dungeonWidth-1; x++)
                {
                    // skip nonmatching rooms, empty, corners
                    if (tileMap[y,x] == 0 || grid[y,x] != roomNum+1 || tileMap[y,x] >= 5)
                    {
                        continue;
                    } else
                    {
                        perimCoords.Add(new Vector2(x,y));
                        perimeter++;
                    }
                }
            }

            Vector2 doorCoords = perimCoords[(int) (Random.value * perimCoords.Count)];
            tileMap[(int) doorCoords.y, (int) doorCoords.x] += 8;
        }
    }

    /// <summary>
    /// Instantiates and places tiles, forming the dungeon.
    /// </summary>
    void SpawnDungeon()
    {
        for (int y = 0; y < dungeonHeight; y++)
        {
            for (int x = 0; x < dungeonWidth; x++)
            {
                GameObject newTile = GameObject.Instantiate(tiles[tileMap[y,x]]);
                newTile.transform.SetParent(dungeon.transform);
                newTile.transform.localPosition = new Vector3(x * 5f, 0f, -y * 5f);
                if (y % 3 == 0 && x % 3 == 0)
                {
                    GameObject light = GameObject.Instantiate(lightPrefab);
                    light.transform.SetParent(dungeon.transform);
                    light.transform.localPosition = new Vector3(x * 5f, 0f, -y * 5f);
                }
            }
        }
    }

    // run before geo starts
    [SerializeField, Range(0f,1f)]
    float excisionRate; // chance of excising from a room

    void ExciseRoom(int leftX, int topY, int roomWidth, int roomHeight)
    {
        // cut up to 2/3 of it
        float excisionWidth = Rnd.gain(Random.value, 0.3f) * 2/3f; 
        float excisionHeight = Rnd.gain(Random.value, 0.3f) * 2/3f; 
        // Debug.Log("Before excision:");
        // LogGrid();
        // int squaresChanged = 0;

        // initialize cut area at edges
        int startY = topY;
        int startX = leftX;
        int endY = startY + roomHeight;
        int endX = startX + roomWidth;
        // cut out a random corner, 0123 = tl,tr,bl,br. 
        int cornerToCut = (int) (Random.value * (4 - Mathf.Epsilon));

        switch (cornerToCut)
        {
            // if room is in a corner, reject that corner from excision
            case 0: // cut top left
                if (startX <= 0 && startY <= 0) return;
                endY = (int)(topY + roomHeight * excisionHeight);
                endX = (int)(leftX + roomWidth * excisionWidth);
                break;
            case 1: // top right
                if (endX >= dungeonWidth && startY <= 0) return;
                endY = (int)(topY + roomHeight * excisionHeight);
                startX = (int)(leftX + roomWidth*(1-excisionWidth));
                break;
            case 2: // bottom left
                if (startX <= 0 && endY >= dungeonHeight) return;
                startY = (int)(topY + roomHeight*(1-excisionHeight));
                endX = (int)(leftX + roomWidth * excisionWidth);
                break;
            case 3: // bottom right
                if (endX >= dungeonWidth && endY >= dungeonHeight) return;
                startY = (int)(topY + roomHeight*(1-excisionHeight));
                startX = (int)(leftX + roomWidth*(1-excisionWidth));
                break;
            default:
                Debug.Log("Random returned impossible value.");
                break;
        }

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                grid[y,x] = 0;
                // squaresChanged++;
            }
        }

        // Debug.Log("After excision: changed from" + startX + "," +startY + " to " + endX + "," + endY + " for corner " + cornerToCut + ".");
        // LogGrid();
    }
}
