using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using UnityEngine;

public static class Common
{
    private static readonly SortedSet<int> SortedScores = new SortedSet<int>(Enumerable.Empty<int>(), new IntComparer((a, b) => b.CompareTo(a)));

    public static GameOptions GameOptions { get; set; } = new GameOptions();

    public static GameScores GameScores { get; set; } = new GameScores();

    public static Transform[,] Grid { get; set; } = new Transform[Constants.GridWidth, Constants.GridHeight];

    public static AudioSource GameMusic { get; set; }

    public static AudioSource LineRemovalSound { get; set; }

    public static AudioSource RotateSound { get; set; }

    public static AudioSource HitSound { get; set; }

    public static bool IsGameStarted { get; set; } = false;

    public static bool IsRunning { get; set; } = true;

    public static void IncreaseByScore()
    {
        Results.Level = Convert.ToInt32(Results.Score / 1000) + 1;
    }

    public static bool IsValidGridPos(this GameObject go)
    {
        foreach (Transform child in go.transform)
        {
            var v = Playfield.RoundVec2(child.position);

            // Not inside Border?
            if (!Playfield.InsideBorder(v))
            {
                return false;
            }

            // Block in grid cell (and not part of same go)?
            if (Grid[(int)v.x, (int)v.y] != null &&
                Grid[(int)v.x, (int)v.y].parent != go.transform)
            {
                return false;
            }
        }

        return true;
    }

    public static GameObject RotateFigure(this GameObject go)
    {
        go.RotateF();
        go.CorrectPositionToBorders(
            t => t.position.x,
            (oldP, diff) => new Vector3(oldP.x + diff, oldP.y, oldP.z),
            (oldP, diff, max) => new Vector3(oldP.x - (diff - max), oldP.y, oldP.z));

        // See if valid
        if (go.IsValidGridPos())
        {
            // It's valid. Update grid.
            go.UpdateGrid();
            RotateSound.Play();
        }
        else
        {
            // It's not valid. revert.
            go.UnRotateF();
        }

        return go;
    }

    public static void RotateF(this GameObject go)
    {
        go.transform.Rotate(0, 0, -90, Space.World);
    }

    public static void UnRotateF(this GameObject go)
    {
        go.transform.Rotate(0, 0, 90, Space.World);
    }

    // Add new children to grid
    public static void UpdateGrid(this GameObject go)
    {
        for (int y = 0; y < Constants.GridHeight; ++y)
        {
            for (int x = 0; x < Constants.GridWidth; ++x)
            {
                if (Grid[x, y] != null)
                {
                    if (Grid[x, y].parent == go.transform)
                    {
                        Grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform child in go.transform)
        {
            var v = Playfield.RoundVec2(child.position);
            var x = (int)v.x;
            var y = (int)v.y;
            if (x >= 0 && x < Grid.GetLength(0) && y >= 0 && y < Grid.GetLength(1))
            {
                Grid[x, y] = child;
            }
        }
    }

    public static void ApplyNewScore()
    {
        GameScores = ScoresStreamer.LoadOptions();
        SortedScores.Add(GameScores.N1);
        SortedScores.Add(GameScores.N2);
        SortedScores.Add(GameScores.N3);
        SortedScores.Add(GameScores.N4);
        SortedScores.Add(GameScores.N5);
        SortedScores.Add(GameScores.N6);
        SortedScores.Add(GameScores.N7);
        SortedScores.Add(GameScores.N8);
        SortedScores.Add(GameScores.N9);
        SortedScores.Add(GameScores.N10);
        SortedScores.Add(Results.Score);
        var firstTen = SortedScores.Take(10).ToArray();
        var otherEmpties = Enumerable.Repeat(0, 10 - firstTen.Length);
        firstTen = firstTen.Concat(otherEmpties).ToArray();
        for (int i = 0; i < firstTen.Length; i++)
        {
            switch (i)
            {
                case 0:
                    GameScores.N1 = firstTen[i];
                    break;
                case 1:
                    GameScores.N2 = firstTen[i];
                    break;
                case 2:
                    GameScores.N3 = firstTen[i];
                    break;
                case 3:
                    GameScores.N4 = firstTen[i];
                    break;
                case 4:
                    GameScores.N5 = firstTen[i];
                    break;
                case 5:
                    GameScores.N6 = firstTen[i];
                    break;
                case 6:
                    GameScores.N7 = firstTen[i];
                    break;
                case 7:
                    GameScores.N8 = firstTen[i];
                    break;
                case 8:
                    GameScores.N9 = firstTen[i];
                    break;
                case 9:
                    GameScores.N10 = firstTen[i];
                    break;
            }
        }

        ScoresStreamer.SaveOptions(GameScores);
    }

    public static void SetupOptions()
    {
        GameMusic.volume = GameOptions.MusicVolume;
        RotateSound.volume = GameOptions.SoundVolume;
        HitSound.volume = GameOptions.SoundVolume;
        LineRemovalSound.volume = GameOptions.SoundVolume;
        Results.HiddenLevel = GameOptions.Level;
    }

    public static void IncreaseByHiddenLevel()
    {
        if (Results.HiddenLevel > 1 && Results.HiddenLevel > Results.Level)
        {
            Results.Score = ((Results.HiddenLevel - 1) * 1000) + (Results.Score / 1000);
            Results.Level = Results.HiddenLevel;
        }
    }

    public static void CorrectPositionToBorders(
        this GameObject go,
        Expression<Func<Transform, float>> prop,
        Expression<Func<Vector3, float, Vector3>> correctionToMin,
        Expression<Func<Vector3, float, float, Vector3>> correctionToMax,
        float min = 0,
        float max = 9f)
    {
        var compiledProperty = prop.Compile();
        var compiledMin = correctionToMin.Compile();
        var compiledMax = correctionToMax.Compile();
        bool isIncorrect = false;
        bool incorrectInLeft = false;
        float diffValue = -1;
        foreach (Transform child in go.transform)
        {
            if (compiledProperty(child) < min)
            {
                diffValue = Math.Min(diffValue, compiledProperty(child));
                if (!isIncorrect)
                {
                    isIncorrect = true;
                    incorrectInLeft = true;
                }
            }
            else if (compiledProperty(child) > max)
            {
                diffValue = Math.Max(diffValue, compiledProperty(child));
                if (!isIncorrect)
                {
                    isIncorrect = true;
                }
            }
        }

        if (isIncorrect)
        {
            if (incorrectInLeft)
            {
                diffValue = Math.Abs(diffValue);
                foreach (Transform child in go.transform)
                {
                    var oldPosition = child.position;
                    child.position = compiledMin(oldPosition, diffValue);
                }
            }
            else
            {
                foreach (Transform child in go.transform)
                {
                    var oldPosition = child.position;
                    child.position = compiledMax(oldPosition, diffValue, max);
                }
            }
        }
    }
}