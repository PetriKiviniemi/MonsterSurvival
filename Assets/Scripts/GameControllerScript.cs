using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PortalType 
{
    STARTING_ROOM_PORTAL,
    ZOMBIE_ROOM_PORTAL,
}

public class GameControllerScript : MonoBehaviour
{
    public GameObject player;
    public GameObject startingRoom;
    public GameObject zombieRoom;
    public List<GameObject> portals;
    public GameObject gameStartZone;
    public List<GameObject> zombieSpawnPoints;
    public GameObject zombiePrefab;
    public GameObject elevatorSwitch;
    private bool gamePaused;

    //Zombie game
    public bool isZombieGameRunning = false;
    public int roundNumber;
    public int roundZombieTotal;
    public int currentZombieCount;
    public int playerPoints;
    public int playerCash;

    //UI
    private UIControllerScript uiController;

    //Audio
    public AudioClip ambientSound;
    public AudioClip roundChangeSound;
    private AudioSource audioSource;
    private bool isAudioPlaying;

    void Start()
    {
        uiController = gameObject.GetComponent<UIControllerScript>();
        elevatorSwitch = GameObject.FindGameObjectWithTag("ElevatorSwitch");
        deactivatePortals();
        roundNumber = 0;
        roundZombieTotal = 5;
        currentZombieCount = 0;
        playerPoints = 0;
        playerCash = 0;
        isAudioPlaying = false;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(playAudioClip(audioSource, ambientSound,0,true));
        gamePaused = false;
    }

    void Update()
    {
        
    }

    public void ActivatePortal(PortalType pType)
    {
        foreach(GameObject portal in portals)
        {
            if(portal.GetComponent<PortalScript>().portalType.Equals(pType))
            {
                //TODO:: Play portal animation
                portal.GetComponent<MeshRenderer>().enabled = true;
                portal.GetComponent<MeshCollider>().isTrigger = true;
            }
        }
    }

    public void deactivatePortals()
    {
        foreach(GameObject portal in portals)
        {
            portal.GetComponent<MeshRenderer>().enabled = false;
            portal.GetComponent<MeshCollider>().isTrigger = false;
        }
    }

    //This is the zombie game logic
    public IEnumerator StartZombieGame()
    {
        Debug.Log("Game started");
        deactivatePortals();
        gameStartZone.SetActive(false);
        isZombieGameRunning = true;
        while(isZombieGameRunning)
        {
            nextRound();
            yield return new WaitUntil(() => currentZombieCount <= 0);
        }
    }

    public void nextRound()
    {
        StartCoroutine(playAudioClip(audioSource, roundChangeSound));
        roundNumber++;
        uiController.updateZombieRoundNumber(roundNumber);
        roundZombieTotal = 5 + (2 * roundNumber);
        currentZombieCount = roundZombieTotal;
        StartCoroutine(spawnZombies());
    }

    public IEnumerator spawnZombies()
    {
        //Spawn zombies with time delay
        for(int i = 0; i < roundZombieTotal; i++)
        {
            //Spawn a zombie
            //Randomly choose one of the spawn points
            Debug.Log("Spawning a zombie!");
            spawnZombieAtSpawnerIdx(Random.Range(0, zombieSpawnPoints.Count));
            yield return new WaitForSeconds(2);
        }
    }

    public void spawnZombieAtSpawnerIdx(int idx)
    {
        Vector3 spawnPosition = zombieSpawnPoints[idx].transform.position;
        spawnPosition.y += 4;
        Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
    }

    public void pauseGame()
    {
        if (gamePaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            uiController.hideIngameUIPausePanel();
            gamePaused = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            StartCoroutine(uiController.WaitForPausePanelButtonPress());
            Time.timeScale = 0;
            uiController.showIngameUIPausePanel();
            gamePaused = true;
        }
    }

    public void GameOver()
    {
        isZombieGameRunning = false;
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<ThirdPersonMovement>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        uiController.showIngameUIScorePanel();
        uiController.updateIngameScoreText(playerPoints.ToString());
        StartCoroutine(uiController.WaitForScorePanelButtonPress());
    }

    //Points are the total score from the game, that is increased from various sources
    public void addPoints(int pointAmount)
    {
        playerPoints += pointAmount;
    }

    //Cash is ingame currency generated from killing a zombie
    public void addCash(int cashAmount)
    {
        playerCash += cashAmount;
        uiController.updatePlayerCash(playerCash);
    }

    public void reduceCash(int cashAmount)
    {
        if(playerCash - cashAmount >= 0)
            playerCash -= cashAmount;
        uiController.updatePlayerCash(playerCash);
    }

    public void resetZombieGame()
    {
        roundNumber = 0;
    }

    public void reduceZombieCount()
    {
        currentZombieCount -= 1;
    }
    private IEnumerator playAudioClip(AudioSource audioSource, AudioClip audioClip, float waitTime = 0, bool isLooping = false)
    {
        Debug.Log("Here");
        isAudioPlaying = true;
        audioSource.clip = audioClip;
        audioSource.Play();
        audioSource.loop = isLooping;
        if (waitTime == 0)
            yield return new WaitForSeconds(audioClip.length);
        else
            yield return new WaitForSeconds(waitTime);
        isAudioPlaying = false;
    }
}
