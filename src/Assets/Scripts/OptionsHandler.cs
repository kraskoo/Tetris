using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour
{
    private int index = 0;
    private int level = 0;
    private bool advancedFigures;
    private Text[] texts;
    private Slider[] sliders;

    // Start is called before the first frame update
    public void Start()
    {
        Cursor.visible = false;
        this.texts = GameObject.FindObjectsOfType<Text>();
        var tempTexts = new Text[this.texts.Length];
        tempTexts[0] = this.texts.FirstOrDefault(t => t.name == "Level");
        tempTexts[1] = this.texts.FirstOrDefault(t => t.name == "AdvancedFigures");
        tempTexts[2] = this.texts.FirstOrDefault(t => t.name == "Sound");
        tempTexts[3] = this.texts.FirstOrDefault(t => t.name == "Music");
        tempTexts[4] = this.texts.FirstOrDefault(t => t.name == "Back");
        this.texts = tempTexts;
        this.sliders = GameObject.FindObjectsOfType<Slider>();
        if (Common.GameOptions == null)
        {
            Common.GameOptions = OptionsStreamer.LoadOptions();
        }

        this.level = Common.GameOptions.Level;
        this.advancedFigures = Common.GameOptions.AdvancedFigures;
        this.sliders[0].value = Common.GameOptions.SoundVolume * 100;
        this.sliders[1].value = Common.GameOptions.MusicVolume * 100;
        this.texts[this.index].text = $"{this.texts[this.index].text} {this.level}";
        this.texts[this.index].color = Color.yellow;
        this.texts[1].text = $"Advanced Figures: {(this.advancedFigures ? "Enable" : "Disable")}";
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var previousIndex = this.index;
            this.DecreaseIndex();
            this.ChangeColors(previousIndex);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            var previousIndex = this.index;
            this.IncreaseIndex();
            this.ChangeColors(previousIndex);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.PressLeftArrow();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.PressRightArrow();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (this.index == 4)
            {
                this.SaveNewOptionsAndAssign();
                this.BackToMenuScene();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.SaveNewOptionsAndAssign();
            this.BackToMenuScene();
        }
    }

    private void BackToMenuScene()
    {
        var options = SceneManager.GetSceneByBuildIndex(2);
        var gameScene = SceneManager.GetSceneByBuildIndex(0);
        SceneManager.SetActiveScene(gameScene);
        gameScene.GetRootGameObjects().ToList().ForEach(
            go =>
                {
                    go.SetActive(true);
                    foreach (Transform child in go.transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                });
        SceneManager.UnloadSceneAsync(options);
    }

    private void SaveNewOptionsAndAssign()
    {
        Common.GameOptions.Level = this.level;
        Common.GameOptions.AdvancedFigures = this.advancedFigures;
        Common.GameOptions.SoundVolume = this.sliders[0].value / 100f;
        Common.GameOptions.MusicVolume = this.sliders[1].value / 100f;
        OptionsStreamer.SaveOptions(Common.GameOptions);
        if (Common.IsGameStarted)
        {
            Common.SetupOptions();
            Common.SetByHiddenLevel();
        }
    }

    // Game Level - 0
    // Advanced Figures - 1
    // Sound Volume - 2
    // Music Volume - 3
    // Back - 4
    private void PressLeftArrow()
    {
        switch (this.index)
        {
            case 0:
                if (this.level > 1)
                {
                    this.texts[this.index].text = $"Game Level: {--this.level}";
                    Common.IsLevelChangedManual = true;
                }

                break;
            case 1:
                this.advancedFigures = !this.advancedFigures;
                this.texts[this.index].text = $"Advanced Figures: {(this.advancedFigures ? "Enable" : "Disable")}";
                Common.IsLevelChangedManual = true;
                break;
            case 2:
                this.sliders[0].value -= 10;
                break;
            case 3:
                this.sliders[1].value -= 10;
                break;
        }
    }

    private void PressRightArrow()
    {
        switch (this.index)
        {
            case 0:
                if (this.level < 30)
                {
                    this.texts[this.index].text = $"Game Level: {++this.level}";
                    Common.IsLevelChangedManual = true;
                }

                break;
            case 1:
                this.advancedFigures = !this.advancedFigures;
                this.texts[this.index].text = $"Advanced Figures: {(this.advancedFigures ? "Enable" : "Disable")}";
                Common.IsLevelChangedManual = true;
                break;
            case 2:
                this.sliders[0].value += 10;
                break;
            case 3:
                this.sliders[1].value += 10;
                break;
        }
    }

    private void ChangeColors(int previousIndex)
    {
        this.texts[previousIndex].color = Color.white;
        this.texts[this.index].color = Color.yellow;
    }

    private void IncreaseIndex() => this.index = (this.index + 1) % this.texts.Length;

    private void DecreaseIndex() => this.index = (this.texts.Length + (this.index - 1)) % this.texts.Length;
}