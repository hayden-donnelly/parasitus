using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4.0f;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float aimFov = 50;
    [SerializeField] private float readyToShootFov = 46;
    [SerializeField] private float regularFov = 60;

    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera viewCamera;

    [SerializeField] private AnimationCurve aimCurve;
    private float aimTime = 0.0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float moveY = 0;
        float moveX = 0;

        if(Input.GetMouseButton(1))
        {
            aimTime += Time.deltaTime;
            float aimValue = aimCurve.Evaluate(aimTime);
            viewCamera.fieldOfView = Mathf.Lerp(regularFov, aimFov, aimValue);
            if(viewCamera.fieldOfView >= readyToShootFov)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Shoot");
                }
            }
        }
        else
        {
            aimTime = 0.0f;
            viewCamera.fieldOfView = regularFov;
            if(Input.GetKey(KeyCode.W))
            {
                moveY += 1;
            }
            if(Input.GetKey(KeyCode.S))
            {
                moveY -= 1;
            }
            if(Input.GetKey(KeyCode.A))
            {
                moveX -= 1;
            }
            if(Input.GetKey(KeyCode.D))
            {
                moveX += 1;
            }
        }

        Vector3 movement = new Vector3(moveX, 0, moveY).normalized * moveSpeed;
        movement = transform.TransformDirection(movement);
        controller.Move(movement * Time.deltaTime);
    
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime);
        cameraTransform.Rotate(Vector3.right, -mouseY * rotationSpeed * Time.deltaTime);
    }
}