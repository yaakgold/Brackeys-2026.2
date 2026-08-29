using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BreakoutController : Minigame
{
    [SerializeField] private float paddleSpeed;
    [SerializeField] private float ballSpeed;
    [SerializeField] private GameObject paddle;
    [SerializeField] private BallController ballController;
    [SerializeField] private List<GameObject> bricks;

    private bool _isRunning;
    private int _brickStartCount;

    public bool IsMovingLeft { get; private set; }
    public bool IsMovingRight { get; private set; }
    
    private void Start()
    {
        _brickStartCount = bricks.Count;
        StartMinigame();
    }

    private void Update()
    {
        if (!_isRunning) return;
        
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            IsMovingLeft = true;
            IsMovingRight = false;
            paddle.transform.Translate(Vector2.left * (paddleSpeed * Time.deltaTime));
        }
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            IsMovingLeft = false;
            IsMovingRight = true;
            paddle.transform.Translate(Vector2.right * (paddleSpeed * Time.deltaTime));
        }
        else
        {
            IsMovingLeft = IsMovingRight = false;
        }
    }

    public override void StartMinigame()
    {
        base.StartMinigame();
        
        _isRunning = true;
        ballController.StartMoving(ballSpeed, this);
    }

    public override void StopMinigame()
    {
        base.StopMinigame();
        
        _isRunning = false;
        ballController.StopMoving();
        
        onCompleteMinigame.Invoke(_brickStartCount - bricks.Count);
    }

    public void RemoveBrick(GameObject brick)
    {
        bricks.Remove(brick);
        Destroy(brick);

        if (bricks.Count == 0)
        {
            StopMinigame();
        }
    }
}
