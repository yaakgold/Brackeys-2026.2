using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropController : MonoBehaviour
{
    [SerializeField] private float minSpeed, maxSpeed;

    private float _fallSpeed;
    private BucketDropController _controller;
    
    public void SetController(BucketDropController controller) => _controller = controller;
    
    private void Start()
    {
        _fallSpeed = Random.Range(minSpeed, maxSpeed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bucket"))
        {
            _controller.CollectDrop(this);
        }
        else
        {
            _controller.RemoveDrop(this);
            Destroy(gameObject);
        }
    }

    public void Move()
    {
        transform.Translate(Vector2.down * (_fallSpeed * Time.deltaTime));
    }
}