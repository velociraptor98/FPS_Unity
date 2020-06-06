using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static PlayerController instance;
    private Animator anim;
    [SerializeField] private float playerSpeed = 8.0f;
    [SerializeField] private float gravityModifier = 2.0f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private int currentGun;
    private CharacterController character;
    private Vector3 moveInput;
    [SerializeField]private Transform camera;
    [SerializeField] private float mouseSensitivity = 2;
    [SerializeField] private bool isMouseInverted = false;
    private bool canJump;
    [SerializeField]
    private Transform groundCheckPoint;
    [SerializeField]
    private float runSpeed = 15.0f;
    [SerializeField]
    private LayerMask whatIsGround;
    private bool canDoubleJump;
    //[SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;
    [SerializeField]private Gun ActiveGun;
    [SerializeField] private List<Gun> guns;
    [SerializeField] private Transform ads,gunHolder;
    private Vector3 startPos;
    private float adsSpeed = 2.0f;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        character = this.GetComponent<CharacterController>();
       // currentGun = 0;
        ActiveGun = guns[currentGun];
        ActiveGun.gameObject.SetActive(true);
        firePoint = ActiveGun.FirePoint;
        startPos = gunHolder.localPosition;
        UIController.instance.ammoText.text = "Ammo: " + ActiveGun.currentAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        float yStore = moveInput.y;
        Vector3 vertMove = transform.forward * Input.GetAxis("Vertical");
        Vector3 horMove = transform.right * Input.GetAxis("Horizontal") ;
        moveInput = vertMove + horMove;
        moveInput.Normalize();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveInput *= runSpeed;
        }
        else
        {
            moveInput *= playerSpeed;
        }
        moveInput.y = yStore;
        moveInput.y += Physics.gravity.y * gravityModifier* Time.deltaTime;
        if(character.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifier*Time.deltaTime;
        }
        canJump = Physics.OverlapSphere(groundCheckPoint.position, 0.25f,whatIsGround).Length>0 ; 
        if(canJump)
        {
            canDoubleJump = false;
        }
        if(Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            moveInput.y = jumpHeight;
            canDoubleJump = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && canDoubleJump)
        {
            moveInput.y = jumpHeight;
            canDoubleJump = false;
        }
        if (character)
        {
            character.Move(moveInput * Time.deltaTime);
        }
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y+mouseInput.x,transform.rotation.eulerAngles.z);
        camera.rotation = Quaternion.Euler(camera.rotation.eulerAngles + new Vector3(-mouseInput.y, 0.0f, 0.0f));
        if(Input.GetButtonDown("Fire1") && ActiveGun.fireCounter <=0)
        {
            RaycastHit hit;
            if(Physics.Raycast(camera.transform.position,camera.forward,out hit,50.0f))
            {
                if(Vector3.Distance(camera.transform.position,hit.point)>2.0f)
                    firePoint.LookAt(hit.point);
            }
            else
            {
                firePoint.LookAt(camera.position + (camera.forward * 30.0f));
            }
            FireShot();
        }
        // auto fire repeater
        if(Input.GetMouseButton(0) && ActiveGun.canAutoFire)
        {
            if(ActiveGun.fireCounter<=0)
            {
                FireShot();
            }
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchGun();
        }
        
            if (Input.GetMouseButtonDown(1))
            {
                CameraController.instance.ZoomIn(ActiveGun.GetZoomAmount());
            }
        if (Input.GetMouseButton(1))
        {
            gunHolder.position = Vector3.MoveTowards(gunHolder.position,ads.position,adsSpeed*Time.deltaTime);
        }
        else
        {
            gunHolder.localPosition = Vector3.MoveTowards(gunHolder.localPosition, startPos, adsSpeed * Time.deltaTime);
        }
        if (Input.GetMouseButtonUp(1))
            {
                CameraController.instance.ZoomOut();
            }
        
        anim.SetFloat("MoveSpeed",moveInput.magnitude);
        anim.SetBool("OnGround", canJump);
    }
    public void FireShot()
    {
        if (ActiveGun.currentAmmo > 0)
        {
            --ActiveGun.currentAmmo;
            UIController.instance.ammoText.text = "Ammo: " + ActiveGun.currentAmmo;
            Instantiate(ActiveGun.GetBullet(), firePoint.position, firePoint.rotation);
            ActiveGun.fireCounter = ActiveGun.fireRate;
        }
    }
    public Gun GetActiveGun()
    {
        return this.ActiveGun;
    }
    public void SwitchGun()
    {
        ActiveGun.gameObject.SetActive(false);
        ++currentGun;
        if(currentGun>2)
        {
            currentGun = 0;
        }
        ActiveGun = guns[currentGun];
        ActiveGun.gameObject.SetActive(true);
        firePoint = ActiveGun.FirePoint;
        UIController.instance.ammoText.text = "Ammo: " + ActiveGun.currentAmmo;
    }
}
