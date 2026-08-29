using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BucketDropController : Minigame
{
    [SerializeField] private float paddleSpeed;
    [SerializeField] private float fallSpeed;
    [SerializeField] private GameObject bucket;
    [SerializeField] private List<Transform> dropSlots;
    [SerializeField] private float timeBetweenDrops;
    [SerializeField] private DropController dropPrefab;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Sprite dropSprite;

    private bool _isRunning;
    private readonly List<DropController> _drops = new();
    private int _numDropsCollected;

    private void Start()
    {
        StartMinigame();
    }

    private void Update()
    {
        if (!_isRunning) return;
        
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            bucket.transform.Translate(Vector2.left * (paddleSpeed * Time.deltaTime));
        }
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            bucket.transform.Translate(Vector2.right * (paddleSpeed * Time.deltaTime));
        }

        foreach (var drop in _drops)
        {
            drop.Move();
        }
    }

    public void CollectDrop(DropController drop)
    {
        _drops.Remove(drop);
        _numDropsCollected++;
        
        Destroy(drop.gameObject);
    }
    
    public override void StartMinigame()
    {
        base.StartMinigame();
        
        _isRunning = true;
        StartCoroutine(SpawnDrops());
        StartCoroutine(CountdownTimer());
    }

    public override void StopMinigame()
    {
        base.StopMinigame();
        
        _isRunning = false;
        
        onCompleteMinigame.Invoke(_numDropsCollected);
    }

    private IEnumerator CountdownTimer()
    {
        var count = 10;
        while (_isRunning)
        {
            yield return new WaitForSeconds(1);
            if (count == 0)
            {
                _isRunning = false;
                break;
            }
            count--;
            
            timerText.text = count.ToString();
        }
        
        StopMinigame();
    }

    private IEnumerator SpawnDrops()
    {
        while (true)
        {
            var randomSpawn = dropSlots[Random.Range(0, dropSlots.Count)];

            var d = Instantiate(dropPrefab, randomSpawn.position, Quaternion.identity, transform);
            d.SetController(this);
            d.SetSprite(dropSprite);
            _drops.Add(d);
            yield return new WaitForSeconds(timeBetweenDrops);
        }
    }

    public void RemoveDrop(DropController drop)
    {
        _drops.Remove(drop);
    }
}
