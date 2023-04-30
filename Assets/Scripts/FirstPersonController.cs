using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4.0f;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float aimFov = 50;
    [SerializeField] private float readyToShootFov = 55;
    [SerializeField] private float regularFov = 60;
    [SerializeField] private float timeBetweenShots = 0.7f;
    private float timeSinceLastShot = 0.0f;

    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera viewCamera;
    [SerializeField] private AudioSource gunshotSource;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Transform bulletOrigin;
    [SerializeField] private GameObject nonEnemyHitEffect;

    [SerializeField] private AnimationCurve aimCurve;
    private float aimTime = 0.0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float moveY = 0;
        float moveX = 0;
        timeSinceLastShot += Time.deltaTime;

        if(viewCamera.fieldOfView <= readyToShootFov)
        {
            crosshair.SetActive(true);
        }
        else
        {
            crosshair.SetActive(false);
        }

        if(Input.GetMouseButton(1))
        {
            aimTime += Time.deltaTime;
            float aimValue = aimCurve.Evaluate(aimTime);
            viewCamera.fieldOfView = Mathf.Lerp(regularFov, aimFov, aimValue);
            if(viewCamera.fieldOfView <= readyToShootFov)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    if(timeSinceLastShot >= timeBetweenShots)
                    {
                        RaycastHit hit;
                        if(Physics.Raycast(bulletOrigin.position, bulletOrigin.forward, out hit, 20.0f))
                        {
                            if(hit.collider.CompareTag("Enemy Body"))
                            {
                                DamageEnemyFromRaycast(hit, false);
                            }
                            else if(hit.collider.CompareTag("Enemy Head"))
                            {
                                DamageEnemyFromRaycast(hit, true);
                            }
                            else
                            {
                                Debug.Log("Hit something else");
                                Debug.Log(hit.point);
                                GameObject effectInstance = Instantiate(nonEnemyHitEffect, hit.point, Quaternion.identity);
                                Destroy(effectInstance, 0.5f);
                                effectInstance.transform.forward = hit.normal;
                            }
                        }

                        gunshotSource.Play();
                        timeSinceLastShot = 0.0f;
                    }
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

    private void DamageEnemyFromRaycast(RaycastHit hit, bool headshot)
    {
        GameObject enemy = hit.collider.transform.parent.gameObject;
        enemy.GetComponent<EnemyBase>().TakeDamage(headshot);
    }
}