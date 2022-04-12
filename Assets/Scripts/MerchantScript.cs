using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantScript : NPCScript 
{
    public GameObject gameController;
    private List<string> npcState1Messages = new List<string>{
        "Glad you could make it. Our control center is in danger.",
        "Monsters have taken over the lobby, and we need your help.",
        "Take this gun. It's not the most powerful one but it'll keep you safe.",
        "I'll active the portal for you. Go and kill those monsters",
    };
    //Questions for branching dialog
    //private string npcState1Question = "Are you looking to buy better gear?";
    private List<string> npcState2Messages = new List<string>{
        "You can always return to me and upgrade your gear, for a reasonable price of course.",
        "Now go and take out those zombies!",
    };
    private List<string> npcState1Answers = new List<string> { "Yes", "No" };

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        floatingName.GetComponent<TextMesh>().text = npcName;
        npcMessages.Add(1, npcState1Messages);
        npcMessages.Add(2, npcState2Messages);
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update(); 
    }

    public IEnumerator TalkToNPC()
    {
        Cursor.lockState = CursorLockMode.None;

        //Freeze camera rotation while talking to NPC

        GameObject ThirdPersonCam = GameObject.FindGameObjectWithTag("ThirdPersonCamera");
        ThirdPersonCam.GetComponent<CinemachineFreeLook>().enabled = false;



        ThirdPersonMovement player3rdScript = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonMovement>();
        player3rdScript.enabled = false;

        CharacterController playerCharController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        playerCharController.enabled = false;

        UIControllerScript uiController = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIControllerScript>();
        uiController.hideOptionButtons();
        uiController.showContinueButton();
        for(int i = 0; i < npcState1Messages.Count; i++)
        {
            uiController.updateIngameDialogText(npcMessages[npcTalkState][currentMessageIndex]);
            currentMessageIndex++;
            yield return uiController.WaitContinueButtonPress();
        }

        uiController.hideContinueButton();
        uiController.hideDialogMenu();
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().equipGun(GunType.STARTING_PISTOL);
        
        /* NOTE:: Uncomment when the merchant buy menu is implemented, currently just as a reminder how we can branch dialogs
        uiController.updateIngameDialogText(npcState1Question);
        uiController.showOptionButtons();

        yield return uiController.WaitForDialogButtonPress();

        //Branch out here
        int optionRes = uiController.collectDialogOptionResult();
        if(optionRes == 1)
        {
            Debug.Log("Option 1 pressed");
            Debug.Log("Opening merchant menu");
        }
        else if(optionRes == 2)
        {
            Debug.Log("Option 2 pressed");
            npcTalkState++;
            currentMessageIndex = 0;
            uiController.hideOptionButtons();
            uiController.showContinueButton();

            for(int i = 0; i < npcState1Messages.Count; i++)
            {
                uiController.updateIngameDialogText(npcMessages[npcTalkState][currentMessageIndex]);
                currentMessageIndex++;
                yield return uiController.WaitContinueButtonPress();
            }
        }


        uiController.hideDialogMenu();
        */

        npcTalkState = 0;
        currentMessageIndex = 0;


        playerCharController.enabled = true;
        player3rdScript.enabled = true;
        ThirdPersonCam.GetComponent<CinemachineFreeLook>().enabled = true;
        gameController.GetComponent<GameControllerScript>().ActivatePortal(PortalType.STARTING_ROOM_PORTAL);
    }
}
