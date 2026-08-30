using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : NetworkBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject gfx;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TMP_Text nameText;
    
    private Vector2 _moveInput;
    private CameraHolder _ch;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        //Disable the input if I am not the owner
        if (!IsOwner) return;
        GetComponent<PlayerInput>().enabled = true;
        
        if (Camera.main != null && Camera.main.TryGetComponent(out CameraHolder ch))
        {
            _ch = ch;
            _ch.SetCamera(transform);
        }
        
        SetColorRpc(OwnerClientId);

        UIManager.Instance.SetPlayer(this);

        if (NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject()
            .TryGetComponent(out MpPlayerController player))
        {
            SetNameRpc(player.name);
            GameManager.Instance.onTick.AddListener(() => player.UpdateSanity(-GameManager.Instance.sanityDecreaseAmount));
        }
        
        ProjectManager.Instance.UpdateQualityRpc(0, OwnerClientId);
    }

    [Rpc(SendTo.Server)]
    private void SetNameRpc(string n)
    {
        SetNameClientRpc(n);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetNameClientRpc(string n)
    {
        name = n;
        nameText.text = name;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!IsOwner) return;

        _ch.SwitchToKickCamera();
        UIManager.Instance.SetKicked();
        UIManager.Instance.HideHud();
    }

    private Vector3 _lastPos;
    
    private void LateUpdate()
    {
        if (!IsOwner)
        {
            anim.SetBool(IsMoving, _lastPos != transform.position);
            _lastPos = transform.position;

            return;
        }

        UpdateInputServerRpc(_moveInput);
    }

    private void Move(Vector2 move)
    {
        var sanity = UIManager.Instance.GetSanity();
        var moveSlowdown = sanity / 50.0f;
        moveSlowdown = Mathf.Clamp(moveSlowdown, 0.25f, 1.0f);
        
        rb.AddForce(move * (moveSpeed * moveSlowdown), ForceMode2D.Impulse);

        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 6);
    }
    
    //Get inputs from the PlayerInput script
    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnInteract()
    {
        InteractionManager.Instance.Interact(this);
    }

    public void OnPause()
    {
        PausePanelController.Instance.Open();
    }
    
    //RPCs to interact with the server
    
    [Rpc(SendTo.Server)]
    private void UpdateInputServerRpc(Vector2 move)
    {
        Move(move);
        UpdateClientMovementRpc(move);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateClientMovementRpc(Vector2 move)
    {
        if (move.sqrMagnitude > .01f)
        {
            AudioManager.Instance.Play("Walk");
            anim.SetBool(IsMoving, true);
        }
        else
        {
            AudioManager.Instance.Stop("Walk");
            anim.SetBool(IsMoving, false);
        }
    }

    [Rpc(SendTo.Server)]
    private void SetColorRpc(ulong ownerId)
    {
        var color = GameSetup.Instance.GetPlayerController(ownerId).GetColor();

        if (gfx.TryGetComponent(out SpriteRenderer sr))
        {
            sr.color = color;
        }
        
        SetColorRpc(color);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetColorRpc(Color color)
    {
        if (gfx.TryGetComponent(out SpriteRenderer sr))
        {
            sr.color = color;
        }
    }
}


