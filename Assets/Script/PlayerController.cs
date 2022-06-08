using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public GameObject breakSoundObj;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 10.0f;
     // Pause UI
    public GameObject PauseWindow;
    private bool isPause;
    public Vector3 targetPosition;

    float interval = 0, particleInterval = 0;
    AudioSource audioSource;
    ParticleSystem particleSystem;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    [HideInInspector]
    public bool canMove = true;
    World world;

    void Start()
    {
        world = GameObject.Find("world").GetComponent<World>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPause = false;
        audioSource = breakSoundObj.GetComponent<AudioSource>();
        particleSystem = breakSoundObj.GetComponent<ParticleSystem>();
        
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;


        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();


        createAndDelete();

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
              
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -90, 90);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void createAndDelete()
    {
        if(interval > 0)
        {
            interval -= Time.deltaTime;
            return;
        }
        if(particleInterval > 0)
        {
            particleInterval -= Time.deltaTime;
            particleSystem.Stop();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Input.GetMouseButton(1) && Physics.Raycast(ray, out hit, 5.0f))
        {
            // Debug.Log(world.VoxelInfo.Id);
            world.terrain.CreateVoxel(hit.point, ray.direction, world.VoxelInfo.Id);
            interval = 0.3f;
            particleInterval = 1f;
            audioSource.Play();
            breakSoundObj.transform.position = hit.point + Vector3.up * .3f;
            particleSystem.Play();
        }

        if (Input.GetMouseButton(0) && Physics.Raycast(ray, out hit, 5.0f))
        {
            // Debug.Log(hit.transform.name);
            // Debug.Log(hit.point);
            // Debug.Log(world.VoxelInfo.Id);
            // Debug.Log(ray.direction);
            world.terrain.RemoveVoxel(hit.point, ray.direction);
            interval = 0.3f;
            particleInterval = 1f;
            audioSource.Play();
            breakSoundObj.transform.position = hit.point + Vector3.up * .3f;
            particleSystem.Play();
        }
    }

    void PauseGame()
    {
        isPause = !isPause;
        if (isPause == true)
        {
            PauseWindow.gameObject.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            PauseWindow.gameObject.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}