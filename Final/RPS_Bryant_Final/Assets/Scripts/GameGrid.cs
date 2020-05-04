using UnityEngine;

public class GameGrid : MonoBehaviour
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    public MeshCollider plane;


    public GameGrid(int width, int height, float cellSize) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridArray = new int[width, height];

        // Draw grid with dimensions width and height at world position 0,0,0
        for (int i = 0; i < gridArray.GetLength(0); i++) {
            for (int j = 0; j < gridArray.GetLength(1); j++) {
                // Requires gizmo's turned on in game interface
                Debug.DrawLine(GetWorldPosition(i, j), (GetWorldPosition(i, j + 1)), Color.blue, 100f);
                Debug.DrawLine(GetWorldPosition(i, j), (GetWorldPosition(i + 1, j)), Color.blue, 100f);
            }
        }
    }

    // Get current 2d world position, ignoring y (up) axis with respect to cell size
    private Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, 0, y) * cellSize;
    }
    // Get's current X and Y values based on world position
    private void GetXY(Vector3 worldPostion, out int x, out int y) {
        x = Mathf.FloorToInt(worldPostion.x / cellSize);
        y = Mathf.FloorToInt(worldPostion.y / cellSize);
    }
    // Set cell [x,y] value
    public void SetValue(int x, int y, int value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
        }
    }
    // Set cell value based on world position
    public void SetValue(Vector3 worldPosition, int value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }
    public int GetValue(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else
            return 0;
    }

    public int GetValue(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }
    
}
