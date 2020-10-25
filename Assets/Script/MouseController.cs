using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float jetpackForce = 75.0f;
    public float forwardMovementSpeed = 3.0f;
    private Rigidbody2D playerRigidbody;
   
    // variable to controll animation
    public Transform groundCheckTransform;
    private bool isGrounded;
    public LayerMask groundCheckLayerMask;
    private Animator mouseAnimator;
    // Get Jetpac
    public ParticleSystem jetpac;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        mouseAnimator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
       // jetpac.ise
    }
    //FixedUpdate is called by Unity at a fixed time interval. All physics-related code is written in this method.
    private void FixedUpdate()
    {
        // Left Mouse click or tap on screen to flay player
        bool jetPackActive = Input.GetButton("Fire1");
        if (jetPackActive)
        {
            playerRigidbody.AddForce(new Vector2
                (0, jetpackForce));
        }
        // Auto forwarding force
        Vector2 newValue = playerRigidbody.velocity;
        newValue.x = forwardMovementSpeed;
        playerRigidbody.velocity = newValue;
        UpdateGroundedStatus();

    }
    // Animatin control
    void UpdateGroundedStatus()
    {
        //1
        isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundCheckLayerMask);
        //2
        mouseAnimator.SetBool("isGrounded", isGrounded);
        
    }

}
