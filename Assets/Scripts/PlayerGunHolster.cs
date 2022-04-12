using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType {
    NONE = 0,
    //First guns with pistol like recoil animation
    STARTING_PISTOL = 1,
    RAY_PISTOL = 2,
    SHOTGUN = 3,

    RAY_RIFLE = 4,
};
public class PlayerGunHolster : MonoBehaviour
{
    GameObject heldGun;
    GameObject gunHolster;
    public int price;
    public GunType gunType;
    public static GameObject[] gunPrefabsList;

    void Start()
    {
        gunPrefabsList = Resources.LoadAll<GameObject>("Prefabs/Guns");

        gunHolster = GameObject.FindGameObjectWithTag("PlayerGunHolster");
        if(gunHolster.transform.childCount> 0)
            heldGun = gunHolster.transform.GetChild(0).gameObject;
    }

    //Prefabs are not supported in C# standard dictionaries, should implement some serializible way
    public GameObject FindGunOfType(GunType type)
    {
        foreach(GameObject g in gunPrefabsList)
        {
            if (g.GetComponent<GunScript>().gtype == type)
                return g;
        }
        return new GameObject();
    }

    public GunType buyGun()
    {
        return gunType;
    }

    //Returns the gun gameobject for further access
    public GameObject equipGun(GunType gunType)
    {
        //Unequip current gun if there's any
        if(gunHolster.transform.childCount > 0)
        {
            Destroy(gunHolster.transform.GetChild(0).gameObject);
            if(heldGun)
                Destroy(heldGun);
        }
        GameObject newGun = FindGunOfType(gunType);
        Vector3 newPos = gunHolster.transform.position;
        heldGun = Instantiate(newGun, newPos, Quaternion.identity);
        heldGun.transform.parent = gunHolster.transform;
        return heldGun;
    }
}
