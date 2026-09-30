using UnityEngine;

public class FridgeGenerator : MonoBehaviour
{
    const float GRID_SIZE = 5f;
    [SerializeField]
    GameObject fridgePrefab;
    [SerializeField]
    DungeonGenerator dunGen;
    [SerializeField]
    int maxJitter;
    [SerializeField]
    int spacing;

    void DeleteFridges()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void JitterPlaceFridges()
    {
        for (int y = 1; y < dunGen.dungeonHeight; y += spacing)
            for (int x = 1; x < dunGen.dungeonWidth; x += spacing)
            {
                float rx = maxJitter * Rnd.gain(Random.value,0.65f);
                rx -= rx/2;
                float ry = maxJitter * Rnd.gain(Random.value,0.65f);
                ry -= ry/2;
                if (x+rx >= 0 && x+rx < dunGen.dungeonWidth && y+ry >= 0 && y+ry < dunGen.dungeonHeight)
                    PlaceFridge((int) (x+rx), (int) (y+ry));
            }
    }

    void PlaceFridge(int x, int y)
    {
        // reject placement if on or adjacent to door tile, or in corner tile
        if (dunGen.tileMap[y,x] > 4 
            || dunGen.tileMap[Mathf.Max(0,y-1),x] > 8
            || dunGen.tileMap[y,Mathf.Max(0,x-1)] > 8
            || dunGen.tileMap[Mathf.Min(dunGen.dungeonHeight-1,y+1),x] > 8
            || dunGen.tileMap[y,Mathf.Min(dunGen.dungeonWidth-1,x+1)] > 8)
        {
            return;
        }

        GameObject newFridge = GameObject.Instantiate(fridgePrefab);
        newFridge.transform.SetParent(transform);
        newFridge.transform.localPosition = new Vector3(x*GRID_SIZE, 0f, -y*GRID_SIZE);
        switch (dunGen.tileMap[y,x])
        {
            case 1: // NWall, south facing fridge
                newFridge.transform.rotation = Quaternion.Euler(0,180,0);
                break;
            case 2: // S
                newFridge.transform.rotation = Quaternion.Euler(0,0,0);
                break;
            case 3: // E
                newFridge.transform.rotation = Quaternion.Euler(0,270,0);
                break;
            case 4: // W
                newFridge.transform.rotation = Quaternion.Euler(0,90,0);
                break;
            default: 
                newFridge.transform.rotation = Quaternion.Euler(0,90f*(int)(Random.value * (4 - Mathf.Epsilon)),0);
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            DeleteFridges();
        }
    }
}
