using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    // Define the grid size.
    [SerializeField]
    int dungeonWidth;
    [SerializeField]
    int dungeonHeight;

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
    
    private int[,] dungeonGrid;
    private int nextRoomWidth;
    private int nextRoomHeight;
    private int roomNumber;

    void Start()
    {
        InitDungeon();
    }

    void InitDungeon()
    {
        Random.InitState(seed);
        generationActive = true;
        roomNumber = 0;
        dungeonGrid = new int[dungeonHeight,dungeonWidth];
        currMaxRoomSize = maxRoomSize;
        roomPlacementGain = startingPlacementGain;
    }

    void RandomizeSize()
    {
        float rw = Random.value;
        float rh = Random.value;
        // Random integer in min-max range, bias to center
        nextRoomWidth = (int) ((currMaxRoomSize-minRoomSize) * Rnd.gain(rw,roomSizeGain)) + minRoomSize;
        nextRoomHeight = (int) ((currMaxRoomSize-minRoomSize) * Rnd.gain(rh,roomSizeGain)) + minRoomSize;
        // Debug.Log("Sized room with width " + nextRoomWidth + " and height " + nextRoomHeight);
    }

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
            int currX = (int) (Rnd.gain(Random.value, roomPlacementGain) * (dungeonWidth - nextRoomWidth));
            int currY = (int) (Rnd.gain(Random.value, roomPlacementGain) * (dungeonHeight - nextRoomHeight));
            // Debug.Log("Attempting room placement at " + currX + ", " + currY);
            
            // check room collision
            for (int y = currY; y < currY + nextRoomHeight; y++) 
            {
                for (int x = currX; x < currX + nextRoomWidth; x++)
                {
                    // Debug.Log("With size " + nextRoomWidth + "," + nextRoomHeight + ", checking at " + x + "," + y);
                    // blocked if any adjacents aren't clear (needlessly redundant)
                    if (dungeonGrid[y,x] != 0 
                    || dungeonGrid[Mathf.Max(0,y-1),x] != 0 
                    || dungeonGrid[y,Mathf.Max(0,x-1)] != 0
                    || dungeonGrid[Mathf.Min(dungeonHeight-1,y+1),x] != 0 
                    || dungeonGrid[y,Mathf.Min(dungeonWidth-1,x+1)] != 0)
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
                        dungeonGrid[y,x] = roomNumber;
                    }
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

    void GenerateRooms()
    {
        InitDungeon();
        while (generationActive)
        {
            // RandomizeSize();
            FindRoomPlacement();
        }
        Debug.Log("Generated " + roomNumber + "rooms");
    }

    void Update()
    {



        // DEBUG:
        ReadDebugInputs();
    }

    void ReadDebugInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogGrid();
        }
        if (Input.GetKeyDown(KeyCode.G)) {
            GenerateRooms();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            seed = (int) (Random.value * 10000000);
        }
    }

    void LogGrid()
    {   
        string grid = "";
        // Discard rightmost column and bottommost row
        for (int y = 0; y < dungeonHeight - 1; y++)
        {
            for (int x = 0; x < dungeonWidth - 1; x++)
            {
                grid += " [" + dungeonGrid[y,x] + "] ";
            }
            grid += "\n";
        }
        Debug.Log(grid);
    }
}
