using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    private int index = 2;
    private Text[] texts;

    public void Start()
    {
        if (!OptionsStreamer.IsFileExist())
        {
            OptionsStreamer.SaveOptions(new GameOptions());
        }

        this.texts = GameObject.FindObjectsOfType<Text>();
        if (!Common.IsGameStarted)
        {
            Common.GameOptions = OptionsStreamer.LoadOptions();
            this.texts[this.index].color = Color.gray;
            this.texts = this.texts.Where(t => t.text != "Resume").ToArray();
        }

        this.texts[this.index].color = Color.yellow;
    }

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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            this.MenuAction(this.texts[this.index].text);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Common.IsGameStarted)
            {
                this.MenuAction("Resume");
            }
            else
            {
                Application.Quit(0);
            }
        }
    }

    private void MenuAction(string text)
    {
        switch (text)
        {
            case "Resume":
                Common.SetByHiddenLevel();
                var mainMenuScene = SceneManager.GetSceneByBuildIndex(0);
                var gameScene = SceneManager.GetSceneByBuildIndex(1);
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
                Common.IsRunning = true;
                SceneManager.UnloadSceneAsync(mainMenuScene);
                return;
            case "New Game":
                Common.IsGameStarted = true;
                SceneManager.LoadScene(1);
                return;
            case "Options":
                FindObjectsOfType<GameObject>().ToList().ForEach(go => go.SetActive(false));
                SceneManager.LoadScene(2, LoadSceneMode.Additive);
                return;
            case "Exit":
                Application.Quit(0);
                return;
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
