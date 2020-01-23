using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour
{
    private int index = 2;
    private int level = 0;
    private Text[] texts;
    private Slider[] sliders;

    // Start is called before the first frame update
    public void Start()
    {
        Cursor.visible = false;
        this.texts = GameObject.FindObjectsOfType<Text>();
        this.sliders = GameObject.FindObjectsOfType<Slider>();
        if (Common.GameOptions == null)
        {
            Common.GameOptions = OptionsStreamer.LoadOptions();
        }

        this.level = Common.GameOptions.Level;
        this.sliders[0].value = Common.GameOptions.SoundVolume * 100;
        this.sliders[1].value = Common.GameOptions.MusicVolume * 100;
        this.texts[this.index].text = $"{this.texts[this.index].text} {this.level}";
        this.texts[this.index].color = Color.yellow;
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var previousIndex = this.index;
            this.IncreaseIndex();
            this.ChangeColors(previousIndex);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            var previousIndex = this.index;
            this.DecreaseIndex();
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
            if (this.index == 3)
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
        Common.GameOptions.SoundVolume = this.sliders[0].value / 100f;
        Common.GameOptions.MusicVolume = this.sliders[1].value / 100f;
        OptionsStreamer.SaveOptions(Common.GameOptions);
        if (Common.IsGameStarted)
        {
            Common.SetupOptions();
            Common.SetByHiddenLevel();
        }
    }

    // Game Level - 2
    // Sound Volume - 1
    // Music Volume - 0
    private void PressLeftArrow()
    {
        switch (this.index)
        {
            case 2:
                if (this.level > 1)
                {
                    this.texts[this.index].text = $"Game Level: {--this.level}";
                    Common.IsLevelChangedManual = true;
                }

                break;
            case 1:
                this.sliders[0].value -= 10;
                break;
            case 0:
                this.sliders[1].value -= 10;
                break;
        }
    }

    private void PressRightArrow()
    {
        switch (this.index)
        {
            case 2:
                if (this.level < 30)
                {
                    this.texts[this.index].text = $"Game Level: {++this.level}";
                    Common.IsLevelChangedManual = true;
                }

                break;
            case 1:
                this.sliders[0].value += 10;
                break;
            case 0:
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