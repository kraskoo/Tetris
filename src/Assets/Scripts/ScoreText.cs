using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    private Text score;

    public void Start() =>
        this.score = GameObject.FindObjectsOfType<Text>().FirstOrDefault(t => t.name == "Score");

    // Update is called once per frame
    public void Update() =>
        this.score.text = $"Score: {Results.Score}";
}