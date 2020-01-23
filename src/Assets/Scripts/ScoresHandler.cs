using System;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoresHandler : MonoBehaviour
{
    private Text[] resultTexts;

    public void Start()
    {
        Cursor.visible = false;
        if (!ScoresStreamer.IsFileExist())
        {
            ScoresStreamer.SaveOptions(Common.GameScores);
        }

        Common.ApplyNewScore();
        this.resultTexts = GameObject.FindObjectsOfType<Text>().Where(t => t.name.StartsWith("n")).ToArray();
        foreach (var text in this.resultTexts)
        {
            switch (text.name)
            {
                case "n1":
                    text.text = $"1. {Common.GameScores.N1}";
                    break;
                case "n2":
                    text.text = $"2. {Common.GameScores.N2}";
                    break;
                case "n3":
                    text.text = $"3. {Common.GameScores.N3}";
                    break;
                case "n4":
                    text.text = $"4. {Common.GameScores.N4}";
                    break;
                case "n5":
                    text.text = $"5. {Common.GameScores.N5}";
                    break;
                case "n6":
                    text.text = $"6. {Common.GameScores.N6}";
                    break;
                case "n7":
                    text.text = $"7. {Common.GameScores.N7}";
                    break;
                case "n8":
                    text.text = $"8. {Common.GameScores.N8}";
                    break;
                case "n9":
                    text.text = $"9. {Common.GameScores.N9}";
                    break;
                case "n10":
                    text.text = $"10. {Common.GameScores.N10}";
                    break;
            }

            if (int.Parse(text.text.Split(new[] { ". " }, StringSplitOptions.None)[1]) == Results.Score)
            {
                text.color = Color.yellow;
            }
        }

        Results.Level = 0;
        Results.Score = 0;
        Results.HiddenLevel = 0;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
}
