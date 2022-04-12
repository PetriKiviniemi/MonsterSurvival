using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControllerScript : MonoBehaviour
{
    //UI
    public GameObject ingameUI;
    public GameObject ingameUIDialogPanel;
    public GameObject ingameUIGuidePanel;
    public GameObject ingameUIPlayerCashPanel;
    public GameObject ingameUIEquippedGunPanel;
    public GameObject ingameUIScorePanel;
    public GameObject ingameUIPausePanel;
    public GameObject zombieRoundNumberPanel;
    public GameObject OptionBtn1;
    public GameObject OptionBtn2;
    public GameObject PlayAgainButton;
    public GameObject MainMenuButton;
    public GameObject ContinueBtn;
    public GameObject ResumeButton;
    public GameObject QuitButton;
    public int pressedOptionButton;

    // Start is called before the first frame update
    void Start()
    {
        ingameUIDialogPanel.SetActive(false);
        hideIngameUIScorePanel();
        hideZombieGameUIelements();
        pressedOptionButton = 0;
    }

    public void updateIngameGuideText(string newText)
    {
        ingameUIGuidePanel.SetActive(true);
        ingameUIDialogPanel.SetActive(false);
        ingameUIGuidePanel.GetComponentInChildren<TextMeshProUGUI>().text = newText;
    }

    public void updateIngameDialogText(string newText)
    {
        ingameUIGuidePanel.SetActive(false);
        ingameUIDialogPanel.SetActive(true);
        ingameUIDialogPanel.GetComponentInChildren<TextMeshProUGUI>().text = newText;
    }

    public void updateIngameScoreText(string newScore)
    {
        ingameUIScorePanel.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = newScore;
    }

    public void hideIngameGuideText()
    {
        ingameUIGuidePanel.SetActive(false);
    }

    public void showIngameUIScorePanel()
    {
        ingameUIScorePanel.SetActive(true);
    }

    public void hideIngameUIScorePanel()
    {
        ingameUIScorePanel.SetActive(false);
    }

    public void showIngameUIPausePanel()
    {
        ingameUIPausePanel.SetActive(true);
    }
    public void hideIngameUIPausePanel()
    {
        ingameUIPausePanel.SetActive(false);
    }

    public void updateZombieRoundNumber(int newRound)
    {
        showZombieGameUIelements();
        zombieRoundNumberPanel.GetComponentInChildren<TextMeshProUGUI>().text = newRound.ToString();
    }

    public void updatePlayerCash(int newCash)
    {
        showZombieGameUIelements();
        ingameUIPlayerCashPanel.GetComponentInChildren<TextMeshProUGUI>().text = newCash.ToString() + "$";
    }

    public void showOptionButtons()
    {
        OptionBtn1.SetActive(true);
        OptionBtn2.SetActive(true);
    }

    public void showZombieGameUIelements()
    {
        zombieRoundNumberPanel.SetActive(true);
        ingameUIPlayerCashPanel.SetActive(true);
    }
    public void hideZombieGameUIelements()
    {
        zombieRoundNumberPanel.SetActive(false);
        ingameUIPlayerCashPanel.SetActive(false);
    }

    public void hideOptionButtons()
    {
        OptionBtn1.SetActive(false);
        OptionBtn2.SetActive(false);
    }

    public void showContinueButton()
    {
        ContinueBtn.SetActive(true);
    }
    public void hideContinueButton()
    {
        ContinueBtn.SetActive(false);
    }

    public void hideDialogMenu()
    {
        ingameUIDialogPanel.SetActive(false);
    }

    public void ShowActionButtonHint(string message)
    {

    }

    public IEnumerator WaitForDialogButtonPress()
    {
        Button Option1BtnComponent = OptionBtn1.GetComponent<Button>();
        Button Option2BtnComponent = OptionBtn2.GetComponent<Button>();
        Button[] btns = new Button[2];
        btns[0] = Option1BtnComponent;
        btns[1] = Option2BtnComponent;

        var waitForButton = new utilities.WaitForUIButtons(btns);
        yield return waitForButton.GetButtonInput();
        if (waitForButton.pressedButton.Equals(OptionBtn1.GetComponent<Button>()))
        {
            //Option 1 was pressed
            pressedOptionButton = 1;
        }
        else
        {
            pressedOptionButton = 2;
        }
    }

    public IEnumerator WaitForScorePanelButtonPress()
    {
        Button playAgainBtn = PlayAgainButton.GetComponent<Button>();
        Button mainMenuBtn = MainMenuButton.GetComponent<Button>();
        Button[] btns = new Button[2];
        btns[0] = playAgainBtn;
        btns[1] = mainMenuBtn;

        var waitForButton = new utilities.WaitForUIButtons(btns);
        yield return waitForButton.GetButtonInput();
        //TODO:: Save score in highscores db
        if(waitForButton.pressedButton.Equals(playAgainBtn.GetComponent<Button>()))
        {
            SceneManager.LoadScene("GameMainScene", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
    public IEnumerator WaitForPausePanelButtonPress()
    {
        Button resumeBtn = ResumeButton.GetComponent<Button>();
        Button quitBtn = QuitButton.GetComponent<Button>();
        Button[] btns = new Button[2];
        btns[0] = resumeBtn;
        btns[1] = quitBtn;

        var waitForButton = new utilities.WaitForUIButtons(btns);
        yield return waitForButton.GetButtonInput();
        //TODO:: Save score in highscores db
        if(waitForButton.pressedButton.Equals(resumeBtn.GetComponent<Button>()))
        {
            GetComponent<GameControllerScript>().pauseGame();
        }
        else
        {
            Application.Quit();
        }
    }

    public int collectDialogOptionResult()
    {
        int temp = pressedOptionButton;
        pressedOptionButton = 0;
        return temp;
    }

    public IEnumerator WaitContinueButtonPress()
    {
        //Waiting for continue button press
        var waitForButton = new utilities.WaitForUIButtons(ContinueBtn.GetComponent<Button>());
        yield return waitForButton.GetButtonInput();
    }


    //Gun stuff
    public void updateEquippedGunText(GunType type)
    {
        ingameUIEquippedGunPanel.SetActive(true);
        TextMeshProUGUI equippedGunText = ingameUIEquippedGunPanel.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        switch (type)
        {
            case GunType.STARTING_PISTOL:
                equippedGunText.text = "Pistol";
                break;
            case GunType.RAY_PISTOL:
                equippedGunText.text = "Ray pistol";
                break;
            case GunType.RAY_RIFLE:
                equippedGunText.text = "Ray rifle";
                break;
            case GunType.SHOTGUN:
                equippedGunText.text = "Shotgun";
                break;
            default:
                equippedGunText.text = "";
                break;
        }
    }

    public void updateCurrentBulletCount(int bulletCount, int magCount)
    {
        ingameUIEquippedGunPanel.SetActive(true);
        TextMeshProUGUI bulletCountText = ingameUIEquippedGunPanel.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        bulletCountText.text = bulletCount.ToString() + "/" + magCount.ToString();
    }

    public void hideEquippedGunPanel()
    {
        ingameUIEquippedGunPanel.SetActive(false);
    }
}
