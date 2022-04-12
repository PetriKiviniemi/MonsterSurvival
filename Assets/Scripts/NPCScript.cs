using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:: Create interface that requires implementation of following methods: 
//void TalkToNPC()

public class NPCScript : Entity
{
    protected GameObject npcTalkZone;
    protected GameObject floatingName;
    protected GameObject floatingNameContainer;
    protected Dictionary<int, List<string>> npcMessages = new Dictionary<int, List<string>>();
    protected int npcTalkState = 1;
    protected int currentMessageIndex = 0;
    public string npcName;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = 200;
        level = 20;
        strength = 10;
        entityType = EntityType.NPC;
        //TODO:: Do merchant class separately
        npcTalkState = 1;
        currentMessageIndex = 0;

        //NPC talk zone
        npcTalkZone = new GameObject("NPCTalkZone");
        npcTalkZone.AddComponent<BoxCollider>().transform.localScale = new Vector3(6f, 6f, 6f);
        npcTalkZone.GetComponent<BoxCollider>().isTrigger = true;
        npcTalkZone.transform.SetParent(gameObject.transform);
        npcTalkZone.tag = "NPCTalkZone";

        //Floating name
        floatingNameContainer = new GameObject("FloatingNameContainer");
        floatingNameContainer.transform.SetParent(gameObject.transform);
        

        floatingName = new GameObject("FloatingName");
        floatingName.AddComponent<TextMesh>();
        floatingName.GetComponent<TextMesh>().text = npcName;
        floatingName.GetComponent<TextMesh>().fontSize = 180;
        floatingName.GetComponent<TextMesh>().anchor = TextAnchor.UpperCenter;
        floatingName.GetComponent<TextMesh>().characterSize = 0.03f;
        floatingName.transform.SetParent(floatingNameContainer.transform);
    }

    protected virtual void Update()
    {
        npcTalkZone.transform.position = gameObject.transform.position;
        floatingName.transform.rotation = new Quaternion(gameObject.transform.rotation.x, gameObject.transform.rotation.y * -1, gameObject.transform.rotation.z, gameObject.transform.rotation.w);
        floatingName.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 3, gameObject.transform.position.z);
    }
    
    //If player exists the trigger area
    public void ResetNPCTalkTree()
    {
        npcTalkState = 1;
        currentMessageIndex = 0;
    }
}
