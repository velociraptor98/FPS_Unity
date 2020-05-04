using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 10.0f;
    private CharacterController character;
    private Vector3 moveInput;
    [SerializeField]private Transform camera;
    [SerializeField] private float mouseSensitivity = 1;
    [SerializeField] private bool isMouseInverted = false;

    // Start is called before the first frame update
    void Start()
    {
        character = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //moveInput.x = Input.GetAxis("Horizontal")*playerSpeed*Time.deltaTime;
        //moveInput.z = Input.GetAxis("Vertical")*playerSpeed*Time.deltaTime;
        Vector3 vertMove = transform.forward * Input.GetAxis("Vertical");
        Vector3 horMove = transform.right * Input.GetAxis("Horizontal") ;
        moveInput = vertMove + horMove;
        moveInput.Normalize();
        moveInput *= playerSpeed;
        if(character)
        {
            character.Move(moveInput * Time.deltaTime);
        }
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y+mouseInput.x,transform.rotation.eulerAngles.z);
        camera.rotation = Quaternion.Euler(camera.rotation.eulerAngles + new Vector3(-mouseInput.y, 0.0f, 0.0f));
        
    }
}
