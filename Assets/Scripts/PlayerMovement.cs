using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    
    private Vector2 _moveInput;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        //Disable the input if I am not the owner
        if (!IsOwner)
            GetComponent<PlayerInput>().enabled = false;
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
        InteractionManager.Instance.Interact();
    }
    
    //RPCs to interact with the server
    
    [Rpc(SendTo.Server)]
    private void UpdateInputServerRpc(Vector2 move)
    {
        Move(move);
    }
}

