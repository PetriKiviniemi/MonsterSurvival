using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieScript : Entity 
{
    private GameControllerScript gcs;
    private GameObject player;
    private NavMeshAgent navMeshAgent;
    public float acceleration = 2f;
    public float deceleration = 60f;
    public float closeEnoughMeters = 4f;
    public float lastAttackTimestamp;
    public Animator animator;
    public bool isAlive;

    //Audio
    public AudioClip deathSound;
    public AudioClip slapSound;
    public AudioClip footstepSound;
    private AudioSource audioSource;
    private bool isAudioPlaying;
    

    // Start is called before the first frame update
    void Start()
    {
        health = 20;
        strength = 20;
        level = 1;
        gcs = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastAttackTimestamp = 0;
        animator = GetComponent<Animator>();
        isAlive = true;
        audioSource = GetComponent<AudioSource>();
        isAudioPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        navigateToPlayer();
    }

    public void takeDamage(int dmg)
    {
        health -= dmg;
        if (health < 0 && isAlive)
            StartCoroutine(die());
    }

    public void dealDamage(int dmg)
    {
        //TODO:: Play damage animation
        if(Time.time - lastAttackTimestamp > 2)
        {
            lastAttackTimestamp = Time.time;
            animator.SetTrigger("Attack");
            StartCoroutine(playAudioClip(audioSource, slapSound));
            gcs.player.GetComponent<PlayerController>().takeDamage(dmg);
        }
    }

    public IEnumerator die()
    {
        isAlive = false;
        //Play dying animation
        //Remove object after
        StartCoroutine(playAudioClip(audioSource, deathSound));
        gcs.reduceZombieCount();
        //TODO:: Add points dependant whether the killing blow was a headshot etc.
        gcs.addPoints(100);
        gcs.addCash(60);
        navMeshAgent.isStopped = true;
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    public void navigateToPlayer()
    {
        if (!isAlive)
            return;
        if(compareVectorLengths(navMeshAgent.transform.position, player.transform.position) <= 4)
        {
            animator.SetBool("Walk", false);
            dealDamage(5);
        }
        else
        {
            if (!isAudioPlaying)
                StartCoroutine(playAudioClip(audioSource, footstepSound));
            navMeshAgent.destination = player.transform.position;
            animator.SetBool("Walk", true);
        }
    }

    //TODO:: Add to utils helper script
    public float compareVectorLengths(Vector3 f, Vector3 s)
    {
        return Mathf.Abs(f.x - s.x) + Mathf.Abs(f.y - s.y) + Mathf.Abs(f.z - s.z);
    }
    private IEnumerator playAudioClip(AudioSource audioSource, AudioClip audioClip, float length = 0)
    {
        isAudioPlaying = true;
        audioSource.clip = audioClip;
        audioSource.Play();
        if (length == 0)
            yield return new WaitForSeconds(audioClip.length);
        else
            yield return new WaitForSeconds(length);
        isAudioPlaying = false;
    }
}
