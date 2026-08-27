using System;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : NetworkBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject gfx;
    [SerializeField] private Animator anim;
    
    private Vector2 _moveInput;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        //Disable the input if I am not the owner
        if (!IsOwner) return;
        GetComponent<PlayerInput>().enabled = true;
        
        if (Camera.main != null && Camera.main.TryGetComponent(out CameraHolder ch))
        {
            ch.SetCamera(transform);
        }
        
        SetColorRpc(OwnerClientId);

        UIManager.Instance.SetPlayer(this);

        if (NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject()
            .TryGetComponent(out MpPlayerController player))
        {
            GameManager.Instance.onTick.AddListener(() => player.UpdateSanity(-GameManager.Instance.sanityDecreaseAmount));
        }
    }

    private void LateUpdate()
    {
        if (!IsOwner)
            return;

        if (_moveInput != Vector2.zero)
        {
            UpdateInputServerRpc(_moveInput);
        }
        else
        {
            anim.SetBool(IsMoving, false);
        }
    }

    private void Move(Vector2 move)
    {
        transform.Translate(move * (Time.deltaTime * moveSpeed));
        anim.SetBool(IsMoving, true);
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
    
    //RPCs to interact with the server
    
    [Rpc(SendTo.Server)]
    private void UpdateInputServerRpc(Vector2 move)
    {
        Move(move);
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

