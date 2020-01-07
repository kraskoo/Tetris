using UnityEngine;

public class Playfield : MonoBehaviour
{
    public const int W = 10;
    public const int H = 20;

    public static readonly Transform[,] Grid = new Transform[W, H];

    public static Vector2 RoundVec2(Vector2 v) => new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));

    public static bool InsideBorder(Vector2 pos) => (int)pos.x >= 0 && (int)pos.x < W && (int)pos.y >= 0;

    public static void DeleteRow(int y)
    {
        Group.LineRemoval.Play();
        for (int x = 0; x < W; x++)
        {
            GameObject.Destroy(Grid[x, y].gameObject);
            Grid[x, y] = null;
        }
    }

    public static void DecreaseRow(int y)
    {
        for (int x = 0; x < W; x++)
        {
            if (Grid[x, y] != null)
            {
                Grid[x, y - 1] = Grid[x, y];
                Grid[x, y] = null;

                // Update block position
                Grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public static void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < H; i++)
        {
            DecreaseRow(i);
        }
    }

    public static bool IsRowFull(int y)
    {
        for (int x = 0; x < W; x++)
        {
            if (Grid[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }

    public static void DeleteFullRows()
    {
        for (int y = 0; y < H; y++)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                --y;
            }
        }
    }
}