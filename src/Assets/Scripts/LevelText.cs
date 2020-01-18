using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class LevelText : MonoBehaviour
{
    private Text level;

    public void Start() =>
        this.level = GameObject.FindObjectsOfType<Text>().FirstOrDefault(t => t.name == "Level");

    public void Update() =>
        this.level.text = $"Level: {Results.Level}";
}