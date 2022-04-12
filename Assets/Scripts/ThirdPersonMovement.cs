using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement: MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    public float groundDistance;
    public GameObject cameraTargetPoint;
    private GameObject player;
    private Animator animator;

    //Audio
    public AudioClip footstep;
    private AudioSource audioSource;
    bool isAudioPlaying;

    //Jumping
    public float jumpHeight = 4f;
    Vector3 velocity;
    bool isGrounded;
    public float gravity = -9.81f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        isAudioPlaying = false;
    }

    void Update()
    {
        //Camera target transform
        cameraTargetPoint.transform.position = new Vector3(player.transform.position.x - 1, player.transform.position.y + 4f, player.transform.position.z);

        //Jumping
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -5f;
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jumped");
            velocity.y = Mathf.Sqrt(jumpHeight * -1 * gravity);
        }

        //Gravity, since the rigidbody is kinematic
        velocity.y += gravity * Time.deltaTime * 1.5f;
        controller.Move(velocity * Time.deltaTime);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;

        if(dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            //Animations
            animator.SetBool("Walk", true);
            if (!isAudioPlaying)
                StartCoroutine(playAudioClip(footstep));
        }
        else
        {
            animator.SetBool("Walk", false);
        }
    }
    
    private IEnumerator playAudioClip(AudioClip audioClip)
    {
        Debug.Log("Here");
        isAudioPlaying = true;
        audioSource.clip = audioClip;
        audioSource.Play();
        yield return new WaitForSeconds(audioClip.length);
        isAudioPlaying = false;
    }
}
