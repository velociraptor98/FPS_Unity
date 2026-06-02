using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] private float playerSpeed    = 8.0f;
    [SerializeField] private float gravityModifier = 2.0f;
    [SerializeField] private float jumpHeight      = 2.0f;
    [SerializeField] private int   currentGun;
    [SerializeField] private Transform camera;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private bool  isMouseInverted  = false;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float runSpeed = 15.0f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Gun ActiveGun;
    [SerializeField] private List<Gun> guns;
    [SerializeField] private Transform ads, gunHolder;
    [SerializeField] private InputActionAsset inputActions;

    public GameObject muzzle;

    private Animator          anim;
    private CharacterController character;
    private Vector3           moveInput;
    private Vector3           startPos;
    private bool              canJump;
    private bool              canDoubleJump;
    private float             adsSpeed    = 2.0f;
    private float             bounceAmount;
    private bool              bounce;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _fireAction;
    private InputAction _adsAction;
    private InputAction _switchGunAction;

    private void Awake()
    {
        instance = this;

        var playerMap    = inputActions.FindActionMap("Player", throwIfNotFound: true);
        _moveAction      = playerMap.FindAction("Move",      throwIfNotFound: true);
        _lookAction      = playerMap.FindAction("Look",      throwIfNotFound: true);
        _jumpAction      = playerMap.FindAction("Jump",      throwIfNotFound: true);
        _sprintAction    = playerMap.FindAction("Sprint",    throwIfNotFound: true);
        _fireAction      = playerMap.FindAction("Fire",      throwIfNotFound: true);
        _adsAction       = playerMap.FindAction("ADS",       throwIfNotFound: true);
        _switchGunAction = playerMap.FindAction("SwitchGun", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        if (_moveAction == null) return;
        _moveAction.Enable();
        _lookAction.Enable();
        _jumpAction.Enable();
        _sprintAction.Enable();
        _fireAction.Enable();
        _adsAction.Enable();
        _switchGunAction.Enable();
    }

    private void OnDisable()
    {
        if (_moveAction == null) return;
        _moveAction.Disable();
        _lookAction.Disable();
        _jumpAction.Disable();
        _sprintAction.Disable();
        _fireAction.Disable();
        _adsAction.Disable();
        _switchGunAction.Disable();
    }

    private void Start()
    {
        anim      = GetComponent<Animator>();
        character = GetComponent<CharacterController>();

        ActiveGun = guns[currentGun];
        ActiveGun.gameObject.SetActive(true);
        firePoint = ActiveGun.FirePoint;
        startPos  = gunHolder.localPosition;
        UIController.instance.ammoText.text = "Ammo: " + ActiveGun.currentAmmo;
    }

    private void Update()
    {
        if (UIController.instance.PauseScreen.activeInHierarchy)
            return;

        HandleMovement();
        HandleLook();
        HandleShooting();
        HandleADS();
        HandleGunSwitch();

        anim.SetFloat("MoveSpeed", moveInput.magnitude);
        anim.SetBool("OnGround", canJump);
    }

    private void HandleMovement()
    {
        float yStore  = moveInput.y;

        Vector2 moveVal = _moveAction.ReadValue<Vector2>();
        moveInput = (transform.forward * moveVal.y) + (transform.right * moveVal.x);
        moveInput.Normalize();
        moveInput *= _sprintAction.IsPressed() ? runSpeed : playerSpeed;

        moveInput.y  = yStore;
        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

        if (character.isGrounded)
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;

        canJump = Physics.OverlapSphere(groundCheckPoint.position, 0.25f, whatIsGround).Length > 0;
        if (canJump)
            canDoubleJump = false;

        if (_jumpAction.WasPressedThisFrame() && canJump)
        {
            moveInput.y   = jumpHeight;
            canDoubleJump = true;
        }
        else if (_jumpAction.WasPressedThisFrame() && canDoubleJump)
        {
            moveInput.y   = jumpHeight;
            canDoubleJump = false;
        }

        if (bounce)
        {
            bounce        = false;
            moveInput.y   = bounceAmount;
            canDoubleJump = true;
        }

        character.Move(moveInput * Time.deltaTime);
    }

    private void HandleLook()
    {
        // <Mouse>/delta returns raw pixels; 0.1f matches the old Input Manager's default scale
        Vector2 look = _lookAction.ReadValue<Vector2>() * (mouseSensitivity * 0.1f);
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + look.x,
            transform.rotation.eulerAngles.z);

        float pitchDelta = isMouseInverted ? look.y : -look.y;
        camera.rotation  = Quaternion.Euler(camera.rotation.eulerAngles + new Vector3(pitchDelta, 0f, 0f));
    }

    private void HandleShooting()
    {
        muzzle.SetActive(false);

        if (_fireAction.WasPressedThisFrame() && ActiveGun.fireCounter <= 0)
        {
            AimFirePoint();
            FireShot();
        }

        if (_fireAction.IsPressed() && ActiveGun.canAutoFire && ActiveGun.fireCounter <= 0)
            FireShot();
    }

    private void AimFirePoint()
    {
        if (Physics.Raycast(camera.transform.position, camera.forward, out RaycastHit hit, 50f))
        {
            if (Vector3.Distance(camera.transform.position, hit.point) > 2f)
                firePoint.LookAt(hit.point);
        }
        else
        {
            firePoint.LookAt(camera.position + camera.forward * 30f);
        }
    }

    private void HandleADS()
    {
        if (_adsAction.WasPressedThisFrame())
            CameraController.instance.ZoomIn(ActiveGun.GetZoomAmount());

        if (_adsAction.IsPressed())
            gunHolder.position = Vector3.MoveTowards(gunHolder.position, ads.position, adsSpeed * Time.deltaTime);
        else
            gunHolder.localPosition = Vector3.MoveTowards(gunHolder.localPosition, startPos, adsSpeed * Time.deltaTime);

        if (_adsAction.WasReleasedThisFrame())
            CameraController.instance.ZoomOut();
    }

    private void HandleGunSwitch()
    {
        if (_switchGunAction.WasPressedThisFrame())
            SwitchGun();
    }

    public void FireShot()
    {
        if (ActiveGun.currentAmmo > 0)
        {
            --ActiveGun.currentAmmo;
            UIController.instance.ammoText.text = "Ammo: " + ActiveGun.currentAmmo;
            Instantiate(ActiveGun.GetBullet(), firePoint.position, firePoint.rotation);
            ActiveGun.fireCounter = ActiveGun.fireRate;
            muzzle.SetActive(true);
        }
    }

    public Gun GetActiveGun() => ActiveGun;

    public void SwitchGun()
    {
        ActiveGun.gameObject.SetActive(false);
        currentGun = (currentGun + 1) % guns.Count;
        ActiveGun  = guns[currentGun];
        ActiveGun.gameObject.SetActive(true);
        firePoint  = ActiveGun.FirePoint;
        UIController.instance.ammoText.text = "Ammo: " + ActiveGun.currentAmmo;
    }

    public void Bounce(float bounceForce)
    {
        bounceAmount = bounceForce;
        bounce       = true;
    }
}
