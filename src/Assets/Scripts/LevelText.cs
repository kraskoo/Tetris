using UnityEngine;
using UnityEngine.UI;

public class LevelText : MonoBehaviour
{
    public static void IncreaseByScore()
    {
        Results.Level = System.Convert.ToInt32(Results.Score / 1000) + 1;
    }

    // Update is called once per frame
    public void Update()
    {
        GameObject.FindObjectsOfType<Text>()[1].text = $"Level: {Results.Level}";
    }
}