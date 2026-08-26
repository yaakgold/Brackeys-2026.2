using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject gfx;
    
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
    }

    private void LateUpdate()
    {
        if (!IsOwner)
            return;

        if (_moveInput != Vector2.zero)
        {
            UpdateInputServerRpc(_moveInput);
        }
    }

    private void Move(Vector2 move)
    {
        //TODO: Make this actually respect physics for walls and stuff
        transform.Translate(move * (Time.deltaTime * moveSpeed));
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
    private void SetColorRpc(ulong id)
    {
        var color = GameManager.Instance.GetPlayerController().GetColor();
        
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

