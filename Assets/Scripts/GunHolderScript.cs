using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolderScript : MonoBehaviour
{
    public int price;

    void Start()
    {

    }

    public void buyGun()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().equipGun(transform.GetChild(0).gameObject.GetComponent<GunScript>().gtype);
    }
}
