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
        float rh = Random.value;
        // Random integer from 1 to maxRoomSize
        nextRoomWidth = (int) ((maxRoomSize-1) * Rnd.gain(rw, roomSizeGain)) + 1;
        nextRoomHeight = (int) ((maxRoomSize-1) * Rnd.gain(rh, roomSizeGain)) + 1;
        Debug.Log("Sized room with width " + nextRoomWidth + " and height " + nextRoomHeight);
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
            int currX = (int) (Rnd.gain(Random.value, roomPlacementGain) * (dungeonWidth - nextRoomWidth));
            int currY = (int) (Rnd.gain(Random.value, roomPlacementGain) * (dungeonHeight - nextRoomHeight));
            // Debug.Log("Attempting room placement at " + currX + ", " + currY);
            
            // check room collision
            for (int y = currY; y < currY + nextRoomHeight; y++) 
            {
                for (int x = currX; x < currX + nextRoomWidth; x++)
                {
                    // Debug.Log("With size " + nextRoomWidth + "," + nextRoomHeight + ", checking at " + x + "," + y);
                    if (dungeonGrid[y,x] != 0 
                    && dungeonGrid[Mathf.Max(0,y-1),x] != 0 
                    && dungeonGrid[y,Mathf.Max(0,x-1)] != 0)
                    {
                        // failed to place room; eject from for-loops
                        y = currY + nextRoomHeight;
                        x = currX + nextRoomWidth;
                        placementFailed = true;
                        
                    }
                }
            }

            if (!placementFailed)
            {
                Debug.Log("Found valid placement at " + currX + ", " + currY + " after " + placementAttempts + " attempts.");
                for (int y = currY; y < currY + nextRoomHeight - 1; y++) 
                {
                    for (int x = currX; x < currX + nextRoomWidth - 1; x++)
                    {
                        dungeonGrid[y,x] = roomNumber;
                        // end while
                        placementAttempts = maxPlacementAttempts;
                    }
                }
                roomNumber++;
            }
        } 

        
        if (placementFailed)
        {
            maxRoomSize--;
            if (maxRoomSize <= 1)
            {
                Debug.Log("Failed to find room placement. Marking generation as complete.");
                generationActive = false;
            } else {
                Debug.Log("Failed to find room placement within " + maxPlacementAttempts + " attempts. Reducing max dimensions to " + maxRoomSize);
            }
            
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
