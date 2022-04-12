using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static utilities.UtilityMethods;

public class PlayerController : MonoBehaviour
{

    List<GameObject> collisions;
    private GameObject gameController;
    private GameControllerScript gsc;
    private CharacterController charController;
    private GameObject playerGunHolster;
    private float lastPressed = 0;
    public GameObject equippedGun;
    public Rigidbody currentBulletPrefab;
    public float bulletSpeed;
    private GameObject mainCam;
    private ThirdPersonMovement tpm;
    private Animator animator;
    private UIControllerScript ucs;
    public bool hasGun;

    //Audio
    public AudioClip teleportSound;
    public AudioClip timerSound;
    public AudioClip buySound;
    //TODO:: Move above to GameControllerScript.cs
    public AudioClip deathSound;
    private bool isAudioPlaying;

    //Player stats
    public int health;
    private bool playerAlive;

    // Start is called before the first frame update
    void Start()
    {
        collisions = new List<GameObject>();
        gameController = GameObject.FindGameObjectWithTag("GameController");
        gsc = gameController.GetComponent<GameControllerScript>();
        ucs = gameController.GetComponent < UIControllerScript > ();
        charController = gameObject.GetComponent<CharacterController>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        animator = GetComponent<Animator>();
        health = 100;
        hasGun = false;
        playerGunHolster = GameObject.FindGameObjectWithTag("PlayerGunHolster");
        ucs.hideEquippedGunPanel();
        isAudioPlaying = false;
        playerAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasGun)
            animator.SetBool("hasGun", true);
        GunScript currentGunScript = equippedGun.GetComponent<GunScript>();

        if (currentGunScript.gtype != GunType.NONE && currentGunScript.bulletCount == 0)
        {
            currentGunScript.reloadWeapon();
            ucs.updateCurrentBulletCount(currentGunScript.bulletCount, currentGunScript.magCount);
        }

        //Cap the n. of actions user can do per second
        if(Time.time - lastPressed > 0.3f)
        {
            //Action button was pressed
            if(Input.GetKeyDown("r"))
            {
                lastPressed = Time.time;
                currentGunScript.reloadWeapon();
                ucs.updateCurrentBulletCount(currentGunScript.bulletCount, currentGunScript.magCount);
            }
            if(Input.GetKeyDown("e"))
            {
                lastPressed = Time.time;
                //Check for NPC talk zone
                foreach (GameObject gObj in collisions)
                {
                    if(gObj.CompareTag("NPCTalkZone"))
                    {
                        //Have to do this check for every type of NPC
                        //Interract with NPC, NPC Object handles the talking states
                        if(gObj.transform.parent.gameObject.CompareTag("Merchant"))
                        {
                            animator.Play("Idle");
                            MerchantScript merchant = gObj.transform.parent.GetComponent<MerchantScript>();
                            StartCoroutine(merchant.TalkToNPC());
                        }
                    }
                    if(gObj.CompareTag("GunHolder"))
                    {
                        GunHolderScript gunHolder = gObj.transform.gameObject.GetComponent<GunHolderScript>();
                        if(gsc.playerCash > gunHolder.price)
                        {
                            gunHolder.buyGun();
                            StartCoroutine(playAudioClip(buySound));
                            gsc.playerCash -= gunHolder.price;
                            ucs.updatePlayerCash(gsc.playerCash);
                        }
                    }
                    if(gObj.CompareTag("ElevatorSwitch"))
                    {
                        gObj.GetComponent<ElevatorScript>().turnSwitch();
                    }
                }
            }
            if(Input.GetMouseButtonDown(0))
            {
                lastPressed = Time.time;
                //Shoot a ray from the camera to the middle of the screen
                //Rotate the player to look at the point hit
                //If the point was an enemy, deal damage
                //TODO:: Rotate the player in x-axis, but only rotate the weapon / Player hands in x & y axis, to instantiate the bullets in correct rotation
                //left mouse button for attack action
                shoot();

                //Turn player to look at fire direction
                //Projectiles from a bulletSpawnPoint
                /*
                GameObject bulletSpawnPoint = equippedWeapon.transform.GetChild(0).gameObject;
                Rigidbody bulletClone = (Rigidbody) Instantiate(currentBulletPrefab, bulletSpawnPoint.transform.position, Quaternion.identity);
                bulletClone.transform.LookAt(mainCam.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.41f, 0.65f, 100)));
                charController.transform.rotation = new Quaternion(charController.transform.rotation.x, bulletClone.transform.rotation.y, charController.transform.rotation.z, bulletClone.transform.rotation.w);
                bulletClone.velocity = bulletClone.transform.forward * bulletSpeed;
                */
            }
            if (Input.GetKeyDown(KeyCode.Escape))
                gsc.pauseGame();
        }
    }
    public void equipGun(GunType gunType)
    {
        hasGun = true; 
        equippedGun = GetComponent<PlayerGunHolster>().equipGun(gunType);
        GunScript gunScript = equippedGun.GetComponent<GunScript>();
        ucs.updateCurrentBulletCount(gunScript.startBulletCount, gunScript.startMagCount);
        ucs.updateEquippedGunText(gunScript.gtype);
        animator.SetInteger("GunType", (int)gunType);
    }
    private void shoot()
    {
        GunScript currentGunScript = equippedGun.GetComponent<GunScript>();
        if (currentGunScript.gtype == GunType.NONE || currentGunScript.GetTotalBulletCount() <= 0)
            return;
        currentGunScript.shoot();
        ucs.updateCurrentBulletCount(currentGunScript.bulletCount, currentGunScript.magCount);

        animator.Play("GunRecoilPistol", 0, 0);

        Camera cam = mainCam.GetComponent<Camera>();
        Ray ray = new Ray(cam.transform.position + cam.transform.forward * 5, cam.transform.forward);
        RaycastHit hitTarget;
        float maxRayDist = 100f;

        if (Physics.Raycast(ray, out hitTarget, maxRayDist))
        {
            GunScript curGun = equippedGun.GetComponent<GunScript>();

            Quaternion originalRotation = gameObject.transform.rotation;
            Transform temp = gameObject.transform;
            temp.LookAt(cam.transform.position + ray.direction * maxRayDist);
            gameObject.transform.rotation = new Quaternion(originalRotation.x, temp.rotation.y, originalRotation.z, temp.rotation.w);
            //TODO:: Play rotation animation ?

            GameObject objectHit = hitTarget.transform.gameObject;
            if(objectHit.CompareTag("Enemy"))
            {
                objectHit.GetComponent<ZombieScript>().takeDamage(curGun.gunDamage);
            }
        }
    }
    
    public void takeDamage(int dmg)
    {
        health -= 46;
        if (health <= 0 && playerAlive)
            die();
    }

    public void die()
    {
        playerAlive = false;
        animator.Play("Death");
        StartCoroutine(playAudioClip(deathSound));
        gameController.GetComponent<GameControllerScript>().GameOver();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!collisions.Contains(other.gameObject))
            collisions.Add(other.gameObject);
        if (other.gameObject.CompareTag("GameStartZone"))
        {
            StartCoroutine(gameController.GetComponent<GameControllerScript>().StartZombieGame());
        }
        if(other.gameObject.CompareTag("ElevatorSwitch"))
        {
            gsc.GetComponent<UIControllerScript>().updateIngameGuideText("Press e to activate");
        }
        if (other.gameObject.CompareTag("GunHolder"))
        {
            gsc.GetComponent<UIControllerScript>().updateIngameGuideText(
                "Press e to buy a weapon. Cost: " + other.gameObject.GetComponent<GunHolderScript>().price.ToString()
            );
        }
        if (other.gameObject.CompareTag("NPCTalkZone"))
        {
            gsc.GetComponent<UIControllerScript>().updateIngameGuideText("Press e to talk");
        }
        if(other.gameObject.CompareTag("ElevatorTriggerArea"))
        {
            if(gsc.elevatorSwitch.GetComponent<ElevatorScript>().elevFloorState == 1)
            {
                StartCoroutine(gsc.elevatorSwitch.GetComponent<ElevatorScript>().animateElevFloorUp());
                StartCoroutine(teleportPlayerBack());
            }
        }
        if(other.gameObject.CompareTag("ZombieRoomEntrance"))
        {
            die();
        }
    }

    public IEnumerator teleportPlayerBack()
    {
        StartCoroutine(playAudioClip(timerSound, waitTime: 10, shouldLoop: true));
        yield return new WaitForSeconds(10);
        StartCoroutine(playAudioClip(teleportSound));
        GetComponent<CharacterController>().enabled = false;
        gameObject.transform.position = new Vector3(-2.0f, -47f, -16f);
        GetComponent<CharacterController>().enabled = true;
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("NPCTalkZone"))
        {
            if(other.gameObject.transform.parent.gameObject.CompareTag("Merchant"))
            {
                other.transform.parent.gameObject.GetComponent<MerchantScript>().ResetNPCTalkTree();
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        gsc.GetComponent<UIControllerScript>().hideIngameGuideText();

        collisions.Remove(other.gameObject);
    }
    //TODO:: Move to UtilityScript.cs
    private IEnumerator playAudioClip(AudioClip audioClip, AudioSource audioSource = null, float waitTime = 0, bool shouldLoop = false)
    {
        AudioSource temp;
        if (!audioSource)
        {
            //NOTE:: Every object using audio must have an audiosource with proper settings!
            //Copy audio source component and its settings

            /*
            AudioSource tmp = gameObject.GetComponent<AudioSource>();
            System.Type type = tmp.GetType();
            temp = gameObject.AddComponent<AudioSource>();
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(temp, field.GetValue(tmp));
            }
            */
            temp = gameObject.AddComponent<AudioSource>();
            if (audioClip.name.Equals("teleport") || audioClip.name.Equals("tick-tock"))
                temp.spatialBlend = 0;
            else
                temp.spatialBlend = 1;
            temp.volume = 0.05f;
        }
        else
            temp = audioSource;

        isAudioPlaying = true;
        temp.clip = audioClip;
        temp.loop = shouldLoop;
        temp.Play();
        if (waitTime == 0)
            yield return new WaitForSeconds(audioClip.length);
        else
            yield return new WaitForSeconds(waitTime);
        isAudioPlaying = false;

        if(!audioSource)
            Destroy(temp);
    }
}
