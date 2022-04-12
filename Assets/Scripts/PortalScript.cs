using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public GameObject portalSpawnPoint;
    public PortalType portalType;

    //Audio
    public AudioClip teleportSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            StartCoroutine(playAudioClip(audioSource, teleportSound));
            other.gameObject.GetComponent<CharacterController>().enabled = false;
            other.gameObject.transform.position = portalSpawnPoint.transform.position;
            other.gameObject.GetComponent<CharacterController>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator playAudioClip(AudioSource audioSource, AudioClip audioClip, float waitTime = 0)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        if (waitTime == 0)
            yield return new WaitForSeconds(audioClip.length);
        else
            yield return new WaitForSeconds(waitTime);
    }
}
