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
    int maxRoomSize; // Max side length of a room. Biased to medium values.
    [SerializeField]
    int maxPlacementAttempts;
    [SerializeField, Range(0f, 1f)]
    float roomSizeGain; // Closer to 1 = more extreme values.
    [SerializeField, Range(0f, 1f)]
    float roomPlacementGain; // Closer to 1 = more extreme values.
    bool generationActive;
    
    private int[,] dungeonGrid;
    private int nextRoomWidth;
    private int nextRoomHeight;
    private int roomNumber;

    void Start()
    {
        Random.InitState(seed);
        generationActive = true;
        roomNumber = 1;
        dungeonGrid = new int[dungeonHeight,dungeonWidth];
    }

    void RandomizeSize()
    {
        float rw = Random.value;
        nextRoomWidth = (int) (maxRoomSize * Rnd.gain(rw, roomSizeGain));
        float rh = Random.value;
        nextRoomHeight = (int) (maxRoomSize * Rnd.gain(rh, roomSizeGain));
        Debug.Log("Sized room with width " + nextRoomWidth + " and height " + nextRoomHeight);
    }

    void FindRoomPlacement()
    {
        bool placed = false;
        bool placementFailed = false;
        int placementAttempts = 0;

        // attempt up to n placements
        while (!placed && placementAttempts < maxPlacementAttempts)
        {
            placementAttempts++;
            // try random
            int currX = (int) Rnd.gain(Random.value, roomPlacementGain) * (dungeonWidth - nextRoomWidth);
            int currY = (int) Rnd.gain(Random.value, roomPlacementGain) * (dungeonHeight - nextRoomHeight);
            Debug.Log("Attempting room placement at " + currX + ", " + currY);
            // check room collision
            for (int y = currY; y < currY + nextRoomHeight; y++) 
            {
                for (int x = currX; x < currX + nextRoomWidth; x++)
                {
                    if (dungeonGrid[y,x] != 0)
                    {
                        // failed to place room; eject from for-loops
                        y = int.MaxValue;
                        x = int.MaxValue;
                        placementFailed = true;
                    }
                }
            }
            // found valid placement
            if (!placementFailed)
            {
                for (int y = currY; y < currY + nextRoomHeight; y++) 
                {
                    for (int x = currX; x < currX + nextRoomWidth; x++)
                    {
                        dungeonGrid[y,x] = roomNumber;
                        placed = true;
                        roomNumber++;
                    }
                }
            }
        }

        // after nfail failed attempts, end 
        if (!placed)
        {
            // Debug.Log("Failed to find room placement within " + maxPlacementAttempts + " attempts. Reducing max dimensions.");
            // maxRoomSize -= 10;
            Debug.Log("Failed to find room placement. Marking generation as complete.");
            generationActive = false;
            // if (maxRoomSize <= 0)
            // {
                
            // }
        }        
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
        if (Input.GetKeyDown(KeyCode.F)) {
            Debug.Log("Randomizing Room Size:");
            RandomizeSize();
            Debug.Log("Beginning Room Search:");
            FindRoomPlacement();
        }
    }

    void LogGrid()
    {   
        string grid = "";
        for (int y = 0; y < dungeonHeight; y++)
        {
            for (int x = 0; x < dungeonWidth; x++)
            {
                grid += " [" + dungeonGrid[y,x] + "] ";
            }
            grid += "\n";
        }
        Debug.Log(grid);
    }
}
