using UnityEngine;

public class Playfield : MonoBehaviour
{
    public static Vector2 RoundVec2(Vector2 v) => new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));

    public static bool InsideBorder(Vector2 pos) =>
        (int)pos.x >= 0 &&
        (int)pos.x < Constants.GridWidth &&
        (int)pos.y >= 0 &&
        (int)pos.y < Constants.GridHeight;

    public static void DeleteRow(int y)
    {
        Common.LineRemovalSound.Play();
        for (int x = 0; x < Constants.GridWidth; x++)
        {
            Behaviour.Destroy(Common.Grid[x, y].gameObject);
            Common.Grid[x, y] = null;
        }
    }

    public static void DecreaseRow(int y)
    {
        for (int x = 0; x < Constants.GridWidth; x++)
        {
            if (Common.Grid[x, y] != null)
            {
                Common.Grid[x, y - 1] = Common.Grid[x, y];
                Common.Grid[x, y] = null;

                // Update block position
                Common.Grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public static void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < Constants.GridHeight; i++)
        {
            DecreaseRow(i);
        }
    }

    public static bool IsRowFull(int y)
    {
        for (int x = 0; x < Constants.GridWidth; x++)
        {
            if (Common.Grid[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }

    public static void DeleteFullRows()
    {
        for (int y = 0; y < Constants.GridHeight; y++)
        {
            if (IsRowFull(y))
            {
                Results.Score += 100;
                Common.IncreaseByScore();
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                --y;
            }
        }
    }
}