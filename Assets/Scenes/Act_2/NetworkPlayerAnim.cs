using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerAnimation : NetworkBehaviour
{
    [SerializeField] private Animator animator;

    private NetworkVariable<float> networkSpeed = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        networkSpeed.OnValueChanged += OnSpeedChanged;

        // Initial animation state
        UpdateAnimation(networkSpeed.Value);
    }

    public override void OnNetworkDespawn()
    {
        networkSpeed.OnValueChanged -= OnSpeedChanged;
    }

    private void Update()
    {
        // Only the owner sends movement animation state
        if (!IsOwner)
            return;

        float speed = 0f;

        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.UpArrow) ||
            Input.GetKey(KeyCode.DownArrow) ||
            Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            speed = 1f;
        }

        networkSpeed.Value = speed;
    }

    private void OnSpeedChanged(float oldSpeed, float newSpeed)
    {
        UpdateAnimation(newSpeed);
    }

    private void UpdateAnimation(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }
}