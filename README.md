**Jetpack Joyride**


![](doc/cover1.gif)
### What we will learn
1. Configuring the Game View
2. Camera Setting
3. Sprite Editor
4. Create Game Player, Jump, Forward Movement
5. Particle System
6. Camera Follow (Script)
7. Endless Background
8. Prefab
9. Animation
10. Mask Layer (Render Layer)
11. Create Enemy
12. GUI
13. SceneManagement
14. Audio Management
15. ParallaxScroll (Parallax Camera), Quads


**Script Keyword**
GameObject
uint, Text, Button
Rigidbody2D
Transform
LayerMask
Animator
ParticleSystem
AudioClip
AudioSource
GameObject[]
List<GameObject>
Physics2D
Sprite
SpriteRenderer
IEnumerator

**Properties**
GetComponent, GetButton, AddForce, velocity, OverlapCircle, Collider2D, Camera.main, Time.deltaTime, Vector3.forward

**Method**
OnTriggerEnter2D, PlayClipAtPoint, Destroy, Add, Remove, LoadScene, transform.Find,  Mathf.Max, Random.Range, (GameObject)Instantiate,  Vector3, RotateAround, Renderer, background.material.mainTextureOffset


### Configuration and Procedure
---
Crate New 2D Project, Game -> Change Aspect Radio, add Iphone Landscape 1136x640 the select it.

Add Sprite folder contains
Background 2 part: background and window, each of size: 480x640, need at lest 3 peace to make a background (640x1440).
Book self: 260x400, 
Laser on off, same image of two different color 39x370,
Coins 59x57,
Player(contains 8 image grid for animation of same size) fly-1,fall-1,run-4,dead-2. Total size 640x312 each size: 156x162
Parallax Scroll foreground and Background of size, 1136x492
Mouse Hole: 115x108

Select Main Camera, Projection Orthographic and Size: 3.2

Select Player grid->Inspector: Sprite Mode: Multiple, click on Sprite Editor. On editor, Input, Type: Grid By Cell Size, Pixel Size, X:162, Y: 156, Click on Slice then click on Apply. Then select one by one and name it say mouse_run_0,mouse_run_1,mouse_fly_1 and Apply AGAIN

Drag and drop any one mouse sprite out of 8 into the scene, position it 0,0,0, add Circle Collider 2D/Polygon Collider 2D, adjust its radius or path, add RigidBody 2D, freeze Z rotation,  From Sprite Renderer -> Sorting layer, add 4 layer: Background, Decorations, Objects, Player and select Player
Create empty Game Object, scale : 14.4,1,1, name: floor, position it on the bottom of scene, add Box Collider 2D, do same for Ceiling
Crate a particle system: GO->Effects->Particle System, make child to mouse. edit particle system: position, ration, start Lifetime, Start Size, Start Color, Emission Shape, Shape. Check Color over life time, click on it and change right slider alpha to 0. Add sorting layer to Player, Order in Layer -1.

Add Widow and wall scene to scene, position it to zero, add wall 2 side, hold V to vertex snap. From Sprite Renderer -> Sorting layer select Background. Add Book self to scene add sorting layer to Decorations

Now if we play the game player will start falling and stay on the floor. **MouseController->Player**
Mouse click to jump, fall on gravity. gravity control: **Project Setting -> Physics 2D-> Gravity Y:-15**
```C#
public float jetpackForce = 75.0f;
private Rigidbody2D playerRigidbody;

void Start(){
//Returns the component of Type type if the game object has one attached, null if it doesn't. Using gameObject. GetComponent will return the first component that is found and the order is undefined.
    playerRigidbody = GetComponent<Rigidbody2D>();
}
void FixedUpdate() 
{
    bool jetpackActive = Input.GetButton("Fire1"); // Right Mouse click or Tap
    if (jetpackActive)
    {
        playerRigidbody.AddForce(new Vector2(0, jetpackForce));
    }
}

```
### Mouse forward movement
**MouseController->Player**
```Swift
public float forwordingMovementSpeed = 2.0f;
void FixedUpdate() 
{
   Vector2 newVelocity = playerRigidbody.velocity
   newVelocity.x = fowardingMovementSpeed;
   playerRigidbody.velocity = newVelocity;
}
```

### Camera follow the mouse
**Camera Controller->MainCamera**
```Swift
public GameObject player; // Player Object
    
private void Update()
{
float playerX = player.transform.position.x;
   
Vector3 position = transform.position;
position.x = playerX;
transform.position = position;
    }
    // if we want to maintanance x offset between player and camera
    private float distanceToTarget;
void Start(){
    distanceToTarget = transform.position.x - targetObject.transform.position.x;
}
private void Update(){
float targetObjectX = targetObject.transform.position.x;
Vector3 newCameraPosition = transform.position;
newCameraPosition.x = targetObjectX;
transform.position = newCameraPosition;

}
```

### Creating Prefab of background
First of all we need to drag and drop all objects/image at scene, position and reassemble it, create Empty game object, make all object child to the Game Object, rename it, Drag the game object into our project folder for making prefab. Vary careful about position of prepab.

### Automatically background generate if camera move
for automatically background generate we need to two array one is available background and another is current background. Create an script and attach it to player, case we need player position to calculate where background will add and remove. we also need camera focus width, we can easily catch camera from any script by Camera.main properties.

Here we imagine two points which maintain distance from current camera focusing width. when current background end cross the imaginary min point we remove it and when background start will be bigger than imaginary max point we stop creation new background.

```Swift
 public GameObject[] collectionOfRooms;
    public List<GameObject> currentRoom ;
    private float screenWidthInPoints;
    void Start()
    {
        float screenHeight = 2.0f * Camera.main.orthographicSize;
        screenWidthInPoints = screenHeight * Camera.main.aspect;
         StartCoroutine(GenenerateRoom());
    }
    private IEnumerator GenenerateRoom()
    {
        while (true)
        {
            GenerateRoomIfRequired();
            yield return new WaitForSeconds(0.25f);
        }
    }
    private void GenerateRoomIfRequired()
    {
        List<GameObject> removeRoom = new List<GameObject>();
        bool AddRoom = true;
        float playerX = transform.position.x;
        float removeAtX = playerX-screenWidthInPoints;
        float AddAtX = removeAtX + screenWidthInPoints;
        float farthestRoomEndX = 0f;
        foreach (var obj in currentRoom)
        {
            float currentRoomWidth = obj.transform.Find("floor").localScale.x;// selected object width
            float currentRoomStartX = obj.transform.position.x - (currentRoomWidth * 0.5f);
            float currentRoomEndX = currentRoomStartX + currentRoomWidth ;
           
            if (currentRoomStartX>AddAtX)
            {
                AddRoom = false; 
            }
            if (currentRoomEndX < removeAtX)
            {
                removeRoom.Add(obj);
            }
            farthestRoomEndX = Mathf.Max(farthestRoomEndX, currentRoomEndX);
        }
       foreach(var room in removeRoom)
        {
            currentRoom.Remove(room);
            
            Destroy(room);
            
        }
        if (AddRoom)
        {
            AddNewRoom(farthestRoomEndX);
        }
    }
    void AddNewRoom(float farthestRoom)
    {
        
        int random = UnityEngine.Random.Range(0, collectionOfRooms.Length);
        GameObject room = (GameObject)Instantiate(collectionOfRooms[random]);

       
        float selectedRoomWidth = room.transform.Find("floor").localScale.x;
        float selectedRoomCenterX = farthestRoom + selectedRoomWidth * 0.5f;
        
        room.transform.position = new Vector3(selectedRoomCenterX, 0, 0);
        currentRoom.Add(room);
    }
```

### Animating Player
for animating mouse we need to crate an animator and another animation clip. open animator and animation both windows from windows. Select player from Hierarchy from Animation window crate first animation name it fly, it automatically crate Animator file. also create another animation from Create Another Animation under current animation button. drag animation's figure from project to animation frame. decrease sample rate 60 to 8. now goto Animator window, crate Transition animation to animation, create variable from parameter assign them to Transition condition.

Checking if the Mouse is Grounded
Create an empty object make it child to Player, name it groundCheck, position it bottom of Player, add a placeholder button to it. Add a Layer Mask by name Ground from upper right corner of Inspector to floor object of room prefabs. Now we can easily find out if groundCheck collide to LayerMask Ground.
**MouserController**
```swift
 public Transform groundCheckTransform;
    public LayerMask groundLayerMask;
    private bool isGround = false;
    private Animator playerAnimator;

    void Start()
    {
        //Returns the component of Type type if the game object has one attached, null if it doesn't. Using gameObject. GetComponent will return the first component that is found and the order is undefined.
       playerAnimator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        findCollision();
    }
    void findCollision()
    {
        isGround = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundLayerMask);
        if (isGround)
        {
            playerAnimator.SetBool("isGround", isGround);
        }
      
    }
```

### Particle Control
we can easily get particle system UI using public variable and toggle emission status with rateOverTime
**MouserController**
```swift
public ParticleSystem jetpack;
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

```

### Add Laser(Enemy) 
On hit laser player will be dead. Drag and drop a laser to Scene, rename it, Add Box Collider 2D to it, Resize box of collider, Check Is Trigger.
We will rated it, toggle different image and dynamically crate it, player hit on it to die.

Rotate and Toggle between two Sprite/Image
**LaserController**
```Swift
    //1
    public Sprite laserOnSprite;
    public Sprite laserOffSprite;
    //2
    public float toggleInterval = 0.5f;
    public float rotationSpeed = 30.0f;
    //3
    private bool isLaserOn = true;
    private float timeUntilNextToggle;
   private SpriteRenderer laserRenderer;
   
  // private Collider2D laserCollider;
  void Start()
    {
         laserRenderer = GetComponent<SpriteRenderer>();
         // laserCollider = GetComponent<Collider2D>();
        timeUntilNextToggle = toggleInterval;
    }
    void Update()
    {
        timeUntilNextToggle -= Time.deltaTime;
        if(timeUntilNextToggle <= 0)
        {
            isLaserOn = !isLaserOn;
            if (isLaserOn)
            {
                laserRenderer.sprite = laserOnSprite;
            }
            else
            {
                laserRenderer.sprite = laserOffSprite;
            }
            timeUntilNextToggle = toggleInterval;
        }
        transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
}
```
### Stop Player on Collider
Here we Trigger Box Collider of Laser, From any script using OnTriggerEnter2D(Collider2D collider) method, we can find it out when collision happen. **MouseController**
```Swift
private bool isDead = false;

void OnTriggerEnter2D(Collider2D collider)
{
    HitByLaser(collider);
}

void HitByLaser(Collider2D laserCollider)
{
    isDead = true;
}
void FixedUpdate(){
   if (!isDead)
{
    // Player only move now
   
}else{
 // mouseAnimator.SetBool("isDead", true);
 
 }
}

```

when mouse is dead, assign it to animation parameter isDead and also add a trigger parameter to dead animation so that its cant return.

Add Coin/Working with Tag
Add coin & add a Tag to it by named Coin. add Circle Collider 2D and check Is Trigger, make prefab. 
```Swift
private uint coins = 0;

if (collider.gameObject.CompareTag("Coins"))
{
    CollectCoin(collider);
}
else
{
    HitByLaser(collider);
}

void CollectCoin(Collider2D coinCollider)
{
    coins++;
    Destroy(coinCollider.gameObject);
}
// OnTriggerEnter2D

```

### Dynamically create Coin and Laser
**GeneratorScript**
```Swift
public GameObject[] availableObjects; // laser and coin prefab
public List<GameObject> objects; 

public float objectsMinDistance = 5.0f;
public float objectsMaxDistance = 10.0f;

public float objectsMinY = -1.4f;
public float objectsMaxY = 1.4f;

public float objectsMinRotation = -45.0f;
public float objectsMaxRotation = 45.0f;

void GenerateObjectsIfRequired()
{
    //1
    float playerX = transform.position.x;
    float removeObjectsX = playerX - screenWidthInPoints;
    float addObjectX = playerX + screenWidthInPoints;
    float farthestObjectX = 0;
    //2
    List<GameObject> objectsToRemove = new List<GameObject>();
    foreach (var obj in objects)
    {
        //3
        float objX = obj.transform.position.x;
        //4
        farthestObjectX = Mathf.Max(farthestObjectX, objX);
        //5
        if (objX < removeObjectsX) 
        {           
            objectsToRemove.Add(obj);
        }
    }
    //6
    foreach (var obj in objectsToRemove)
    {
        objects.Remove(obj);
        Destroy(obj);
    }
    //7
    if (farthestObjectX < addObjectX)
    {
        AddObject(farthestObjectX);
    }
}

void AddObject(float lastObjectX)
{
    //1
    int randomIndex = Random.Range(0, availableObjects.Length);
    //2
    GameObject obj = (GameObject)Instantiate(availableObjects[randomIndex]);
    //3
    float objectPositionX = lastObjectX + Random.Range(objectsMinDistance, objectsMaxDistance);
    float randomY = Random.Range(objectsMinY, objectsMaxY);
    obj.transform.position = new Vector3(objectPositionX,randomY,0); 
    //4
    float rotation = Random.Range(objectsMinRotation, objectsMaxRotation);
    obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
    //5
    objects.Add(obj);            
}


```
### Adding GUI Elements
**MouseController** 


Click GameObject/UI/Image to add an Image element. Hierarchy-> size, position and add a image to it. Similar way add a text to it, increase font size.

```swif
using UnityEngine.UI;
public Text coinsCollectedLabel;
coins++;
coinsCollectedLabel.text = coins.ToString();
```

### Restart Game/ SceneManagement
**MouserController**
Similar way Click GameObject/UI/Button -> create new button, set it inactive, make it active when player is dead.

```Swift
using UnityEngine.SceneManagement;
public Button restartButton; // Asign button to it
void FixedUpdate (){
    if (isDead && isGrounded)
{
    restartButton.gameObject.SetActive(true);
}
}
public void RestartGame();
{
    SceneManager.LoadScene("RocketMouse");
}
```
To add restart button on click to restart, select button from inspector add an event to it, assign mouse to Object and select restart method from function, select on runtime.

### Adding Sound and Music
*Approach 1* : Select target prefab, add Audio Source to it, add Audio clip to it, unchecked Play on Awake. 
**MouseController** 
```Swift
if (!isDead)
{
    AudioSource laserZap = collider.gameObject.GetComponent<AudioSource>(); // collider from OnTriggerEnter2D(Collider2D collider)
    laserZap.Play();
}
```
Collecting Coin Sound
Approach 2: add a public variable of type audioClip. Play it on collision with coin.
**MouseController** 
```Swift
public AudioClip coinCollectSound; //attach coin sound here
AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);
```

Add footsteps and jetpack
*Approach 3*: Add two audio source to  player, assign footstep and jetpack to the audio source, create two variable of type AudioSource not AudioClip and drag AudioSource from mouse Audio source. 
```Swift
public AudioSource jetpackAudio;
public AudioSource footstepsAudio;
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
```
*Approach 4*: Add Background Music.
Add AudioSource to MainCamera, add sound to audio clip. enable Play On Awake and Loop and down volume to 0.3

### Adding a Parallax Background:
1. Select target background, Inspector-> Texture Type: Default, Wrap Mode: **Repeat**->Apply
2. Create another camera, rename it to ParallaxCamera, position it (0, **10**, 0), Projection to Orthographic, Size: 3.2, Disabled Audio Listener
3. Create two GameObject ▸ 3D Object ▸ Quad, renamed it parallaxBackground, parallaxForeground. Make children to Parallax Camera. parallaxBackground, Position : (0, 0.7, **10**) and Scale : (11.36, 4.92, 1). parallaxForeground Position to (0, 0.7, **9**) and Scale to (11.36, 4.92, 1). Drag and Drop sprite/images to Quad. For both quad->Shader: **Unlit ▸ Transparent**. Be sure Mesh Renderer of quad is checked.
4. Create a new C# Script called ParallaxScroll and attach it to ParallaxCamera.
```Swift
//1
public Renderer background;// reference to the Mesh Renderer component
public Renderer foreground;
//2
public float backgroundSpeed = 0.02f;
public float foregroundSpeed = 0.06f;
//3
public float offset = 0.0f; this // Player/Mouse transform postion of x
void Update(){
    float backgroundOffset = offset * backgroundSpeed;
float foregroundOffset = offset * foregroundSpeed;
background.material.mainTextureOffset = new Vector2(backgroundOffset, 0);
foreground.material.mainTextureOffset = new Vector2(foregroundOffset, 0);
}
```
Drag and drop parallaxForeground and parallaxBackground to the Renderer variable.
5. Open Player controller, add following line of code
```swift
public ParallaxScroll parallax; // Reference of ParallaxCamera but type of ParallaxScroll Class(Script)
void FixedUpdate {
  parallax.offset = transform.position.x;  
}
```
Drag and drop ParallaxCamera to ParallaxScroll variable.
6. Select ParallaxCamera, set Depth to **-2** must lower than main camera depth, main camera depth may be set in -1. Set main camera Clear Flags: Skybox to **Depth Only**

End
<a href="https://www.raywenderlich.com/5458-how-to-make-a-game-like-jetpack-joyride-in-unity-2d-part-1" target="_blank">...</a>
