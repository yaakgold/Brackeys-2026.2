using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropController : MonoBehaviour
{
    [SerializeField] private float minSpeed, maxSpeed;
    [SerializeField] private SpriteRenderer gfx;

    private float _fallSpeed;
    private BucketDropController _controller;
    
    public void SetController(BucketDropController controller) => _controller = controller;
    
    private void Start()
    {
        _fallSpeed = Random.Range(minSpeed, maxSpeed);
    }

    private void Update()
    {
        gfx.transform.Rotate(Vector3.forward, Random.Range(minSpeed, maxSpeed) * .15f);
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

    public void SetSprite(Sprite spr)
    {
        gfx.sprite = spr;
    }

    public void Move()
    {
        transform.Translate(Vector2.down * (_fallSpeed * Time.deltaTime));
    }
}