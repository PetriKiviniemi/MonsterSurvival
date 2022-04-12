using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour
{
    public GameObject elevatorFloor;
    public Vector3 elevFloorDownPosition;
    public Vector3 elevFloorUpPosition;
    public float animationTime = 2f;
    private float elapsedTime = 0;

    public bool animShouldPlay;
    public int elevFloorState; //0 -> up, 1 -> down
    public float startTime;
    public float distanceToTravel;

    //Audio
    public AudioClip elevatorSound;
    private AudioSource audioSource;
    private bool isAudioPlaying;

    void Start()
    {
        elevFloorUpPosition = elevatorFloor.transform.position;
        elevFloorDownPosition = new Vector3(elevFloorUpPosition.x, elevFloorUpPosition.y - 14f, elevFloorUpPosition.z);
        animShouldPlay = false;
        elevFloorState = 0;
        audioSource = GetComponent<AudioSource>();
        isAudioPlaying = false;
    }

    void Update()
    {
        if(animShouldPlay)
        {
            animShouldPlay = false;
            if (elevFloorState == 0)
                StartCoroutine(animateElevFloorDown());
            else
                StartCoroutine(animateElevFloorUp());
        }
    }

    public void turnSwitch()
    {
        animShouldPlay = true;
        StartCoroutine(playAudioClip(audioSource, elevatorSound));
    }

    public IEnumerator animateElevFloorDown()
    {
        elevFloorState = 1;
        while(elapsedTime < animationTime)
        {
            elevatorFloor.transform.position = Vector3.Lerp(elevFloorUpPosition, elevFloorDownPosition, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elevatorFloor.transform.position = elevFloorDownPosition;
        yield return null;
    }

    public IEnumerator animateElevFloorUp()
    {
        elevFloorState = 0;
        while(elapsedTime < animationTime)
        {
            elevatorFloor.transform.position = Vector3.Lerp(elevFloorDownPosition, elevFloorUpPosition, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elevatorFloor.transform.position = elevFloorUpPosition;
        yield return null;
    }
    private IEnumerator playAudioClip(AudioSource audioSource, AudioClip audioClip, float waitTime = 0)
    {
        isAudioPlaying = true;
        audioSource.clip = audioClip;
        audioSource.Play();
        if (waitTime == 0)
            yield return new WaitForSeconds(audioClip.length);
        else
            yield return new WaitForSeconds(waitTime);
        isAudioPlaying = false;
    }
}
