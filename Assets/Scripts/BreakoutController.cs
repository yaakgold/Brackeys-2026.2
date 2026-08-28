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
            paddle.transform.Translate(Vector2.left * (paddleSpeed * Time.deltaTime));
        }
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            paddle.transform.Translate(Vector2.right * (paddleSpeed * Time.deltaTime));
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
