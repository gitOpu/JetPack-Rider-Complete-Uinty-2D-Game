using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public ParticleSystem jetpack;

    // Mouse live status
    private bool isDead = false;
    // Coins count
    private uint coins = 0;
    // to show coin count on display
    public Text coinsCollectedLabel;
    // Restart game after mouuse is dead
    public Button restartButton;
    // for coin sound
    public AudioClip coinCollectSound;
    // play footstep or jetpac
    public AudioSource jetpackAudio;
    public AudioSource footstepsAudio;
    // for rander second camera 
    public ParallaxScroll parallax;
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        mouseAnimator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
       
    }
    //FixedUpdate is called by Unity at a fixed time interval. All physics-related code is written in this method.
    private void FixedUpdate()
    {
        // Left Mouse click or tap on screen to flay player
        bool jetpackActive = Input.GetButton("Fire1");
        jetpackActive = jetpackActive && !isDead;
        if (jetpackActive)
        {
            playerRigidbody.AddForce(new Vector2
                (0, jetpackForce));
        }
        if (!isDead)
        {
            Vector2 newValue = playerRigidbody.velocity;
            newValue.x = forwardMovementSpeed;
            playerRigidbody.velocity = newValue;
        }
        // Auto forwarding force
        
        UpdateGroundedStatus();
        AdjustJetpack(jetpackActive);
        
        if (isDead && isGrounded)
        {
            restartButton.gameObject.SetActive(true);
        }
        AdjustFootstepsAndJetpackSound(jetpackActive);
        parallax.offset = transform.position.x;

    }
    // Animatin control
    void AdjustJetpack(bool jetpackActive)
    {
        var jetpackEmission = jetpack.emission;
        jetpackEmission.enabled = !isGrounded;
        if (jetpackActive)
        {
            jetpackEmission.rateOverTime = 300.0f;
        }
        else
        {
            jetpackEmission.rateOverTime = 75.0f;
        }
    }

    void UpdateGroundedStatus()
    {
        //1
        isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundCheckLayerMask);
        //2
        mouseAnimator.SetBool("isGrounded", isGrounded);
        
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Coin"))
        {
            CollectCoin(collider);
        }
        else
        {
            HitByLaser(collider);
        }
       
    }
    void CollectCoin(Collider2D coinCollider)
    {
        coins++;
        AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);

        Destroy(coinCollider.gameObject);
        coinsCollectedLabel.text = coins.ToString();
    }
    void HitByLaser(Collider2D laserCollider)
    {
        if (!isDead)
        {
            AudioSource laserZap = laserCollider.gameObject.GetComponent<AudioSource>();
            laserZap.Play();
        }

        isDead = true;
        mouseAnimator.SetBool("isDead", isDead);
        //Debug.Log(gameObject.name + " just hit " + laserCollider.gameObject.name);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    void AdjustFootstepsAndJetpackSound(bool jetpackActive)
    {
        footstepsAudio.enabled = !isDead && isGrounded;
        jetpackAudio.enabled = !isDead && !isGrounded;
        if (jetpackActive)
        {
            jetpackAudio.volume = 1.0f;
        }
        else
        {
            jetpackAudio.volume = 0.5f;
        }
    }

}
