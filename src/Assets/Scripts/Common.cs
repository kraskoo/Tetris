using UnityEngine;

public static class Common
{
    public static bool IsValidGridPos(this GameObject group)
    {
        foreach (Transform child in group.transform)
        {
            var v = Playfield.RoundVec2(child.position);

            // Not inside Border?
            if (!Playfield.InsideBorder(v))
            {
                return false;
            }

            // Block in grid cell (and not part of same group)?
            if (Playfield.Grid[(int)v.x, (int)v.y] != null &&
                Playfield.Grid[(int)v.x, (int)v.y].parent != group.transform)
            {
                return false;
            }
        }

        return true;
    }

    public static GameObject RotateFigure(this GameObject group)
    {
        group.transform.Rotate(0, 0, -90);

        // See if valid
        if (IsValidGridPos(group))
        {
            // It's valid. Update grid.
            UpdateGrid(group);
            Group.Rotate.Play();
        }
        else
        {
            // It's not valid. revert.
            group.transform.Rotate(0, 0, 90);
        }

        return group;
    }

    public static void UpdateGrid(this GameObject group)
    {
        // Remove old children from grid
        for (int y = 0; y < Playfield.H; ++y)
        {
            for (int x = 0; x < Playfield.W; ++x)
            {
                if (Playfield.Grid[x, y] != null)
                {
                    if (Playfield.Grid[x, y].parent == group.transform)
                    {
                        Playfield.Grid[x, y] = null;
                    }
                }
            }
        }
        
        // Add new children to grid
        foreach (Transform child in group.transform)
        {
            var v = Playfield.RoundVec2(child.position);
            Playfield.Grid[(int)v.x, (int)v.y] = child;
        }
    }

    public static int GetPointOfFallenFigure(this string figureName)
    {
        switch (figureName.Replace("(Clone)", string.Empty))
        {
            case "GroupI":
                return 13;
            case "GroupJ":
                return 7;
            case "GroupL":
                return 9;
            case "GroupO":
                return 15;
            case "GroupS":
                return 5;
            case "GroupT":
                return 11;
            case "GroupZ":
                return 2;
            default:
                return 0;
        }
    }
}