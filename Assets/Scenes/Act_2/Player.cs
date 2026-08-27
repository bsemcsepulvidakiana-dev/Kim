using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NetworkPlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Player Controls")]
    [SerializeField] private bool useWASD = true;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundOffset = -0.1f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // PLAYER 1 - WASD
        if (useWASD)
        {
            if (Input.GetKey(KeyCode.A))
                horizontal = -1f;

            if (Input.GetKey(KeyCode.D))
                horizontal = 1f;

            if (Input.GetKey(KeyCode.W))
                vertical = 1f;

            if (Input.GetKey(KeyCode.S))
                vertical = -1f;
        }
        // PLAYER 2 - ARROW KEYS
        else
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                horizontal = -1f;

            if (Input.GetKey(KeyCode.RightArrow))
                horizontal = 1f;

            if (Input.GetKey(KeyCode.UpArrow))
                vertical = 1f;

            if (Input.GetKey(KeyCode.DownArrow))
                vertical = -1f;
        }

        // Movement Direction
        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        if (movement.magnitude > 1f)
            movement.Normalize();

        // ROTATION
        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Animation
        float speed = movement.magnitude;

        // Gravity
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = groundOffset;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        movement.y = verticalVelocity;

        controller.Move(movement * moveSpeed * Time.deltaTime);
    }

    private void HandleShooting()
    {
        // Player 1 (WASD) uses Spacebar to shoot
        if (useWASD && Input.GetKeyDown(KeyCode.Space))
        {
            ShootServerRpc();
        }
        // Player 2 (Arrow Keys) uses Right Shift or Keypad 0 to shoot
        else if (!useWASD && (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.Keypad0)))
        {
            ShootServerRpc();
        }
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Missing BulletPrefab or FirePoint on " + gameObject.name);
            return;
        }

        // 1. Create bullet instance at fire point position & rotation
        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 2. Set the owner ID on NetworkBullet script
        NetworkBullet bulletScript = bulletInstance.GetComponent<NetworkBullet>();
        if (bulletScript != null)
        {
            bulletScript.OwnerClientId = OwnerClientId;
        }

        // 3. Network Spawn the bullet across all clients
        NetworkObject netObj = bulletInstance.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
    }
}