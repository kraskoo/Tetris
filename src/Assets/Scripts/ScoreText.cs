using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    // Update is called once per frame
    public void Update()
    {
        GameObject.FindObjectsOfType<Text>()[0].text = $"Score: {Results.Score}";
    }
}