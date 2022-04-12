using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public int gunDamage = 0;
    public int bulletCount = 0;
    public int magCount = 0;
    public int startBulletCount = 20;
    public int startMagCount = 3;
    public GunType gtype;
    public AudioSource aSource;
    public AudioClip gunSound;
    public AudioClip reloadSound;
    private bool isAudioPlaying;


    void Start()
    {
        //Store the bullet count / mag count values saved in inspector
        //TODO:: This gun class could be inherited to different gun types,
        //which would give us ability to define gun related values without inspector
        bulletCount = startBulletCount;
        magCount = startMagCount;
        if(transform.childCount > 0)
            transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        aSource = GetComponent<AudioSource>();
        isAudioPlaying = false;
    }

    void Update()
    {
        transform.rotation = transform.parent.transform.rotation;
    }

    public void PlayGunAnimation()
    {
        /*TODO::
        Particle system gunflare
        Gun recoil
         */
        transform.GetChild(0).GetComponent<ParticleSystem>().Play();
    }

    public void shoot()
    {
        PlayGunAnimation();
        StartCoroutine(playAudioClip(gunSound, audioSource: aSource));
        bulletCount -= 1;
    }

    public int GetTotalBulletCount()
    {
        return magCount * bulletCount + bulletCount;
    }

    public void reloadWeapon()
    {
        if (magCount > 0)
        {
            StartCoroutine(playAudioClip(reloadSound));
            bulletCount = startBulletCount;
            magCount -= 1;
        }
    }
    private IEnumerator playAudioClip(AudioClip audioClip, AudioSource audioSource = null, float waitTime = 0, bool shouldLoop = false)
    {
        AudioSource temp;
        if (!audioSource)
        {
            //NOTE:: Every object using audio must have an audiosource with proper settings!
            //Copy audio source component and its settings

            temp = gameObject.AddComponent<AudioSource>();
            temp.spatialBlend = 1;
            temp.volume = 0.05f;
            temp.clip = audioClip;
            temp.loop = shouldLoop;
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
