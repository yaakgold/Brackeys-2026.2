using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallController : MonoBehaviour
{
    private bool _isMoving = false;
    private Vector2 _moveDirection = Vector2.zero;
    private float _speed;
    private BreakoutController _breakoutController;

    private void Update()
    {
        if (!_isMoving) return;
        
        transform.Translate(_moveDirection.normalized * (Time.deltaTime * _speed));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Brick")
            || other.gameObject.CompareTag("Paddle")
            || other.gameObject.CompareTag("TopWall"))
        {
            if (other.gameObject.CompareTag("Brick"))
            {
                _breakoutController.RemoveBrick(other.gameObject);
            }
            _moveDirection.y *= -1;
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            _moveDirection.x *= -1;
        }
        else
        {
            _breakoutController.StopMinigame();
        }
    }

    public void StartMoving(float spd, BreakoutController breakoutController)
    {
        _breakoutController = breakoutController;
        _speed = spd;
        _isMoving = true;

        _moveDirection = new Vector2(Random.Range(-1.0f, 1), Random.Range(0.0f, 1));
    }

    public void StopMoving()
    {
        _isMoving = false;
    }
}