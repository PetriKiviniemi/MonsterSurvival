using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private AssetBundle myLoadedAssetBundle;
    private string[] scenePaths;
    public GameObject MainMenuPanel;
    public GameObject PlayButton;
    public GameObject HighscoresButton;
    public GameObject QuitButton;

    void Start()
    {
        StartCoroutine(WaitForMainMenuPanelButtonPress());
    }

    public IEnumerator WaitForMainMenuPanelButtonPress()
    {
        Debug.Log("Waiting for UI button press");
        Button playBtn = PlayButton.GetComponent<Button>();
        Button highscoreBtn = HighscoresButton.GetComponent<Button>();
        Button quitBtn = QuitButton.GetComponent<Button>();
        Button[] btns = new Button[3];
        btns[0] = playBtn;
        btns[1] = highscoreBtn;
        btns[2] = quitBtn;

        var waitForButton = new utilities.WaitForUIButtons(btns);
        yield return waitForButton.GetButtonInput();
        Debug.Log("Button pressed");
        //TODO:: Save score in highscores db
        if(waitForButton.pressedButton.Equals(playBtn.GetComponent<Button>()))
        {
            SceneManager.LoadScene("GameMainScene");
        }
        else if(waitForButton.pressedButton.Equals(highscoreBtn.GetComponent<Button>()))
        {
            SceneManager.LoadScene("HighscoreScene", LoadSceneMode.Single);
        }
        else
        {
            Application.Quit();
        }
    }
}
