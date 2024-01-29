using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int snakeLength = 3;
    [SerializeField] private GameObject snakeBody;
    [SerializeField] public string snakeName = "Snake 1";
    [SerializeField] private Transform spawnPoint;
    private Vector3 _directionVector = Vector3.right;
    private int scoreMultiplier = 1;

    public int score
    {
        get => _score;
        set
        {
            _score = value;
        }
    }
    
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    
    private List<Transform> _snakeBodies = new List<Transform>();
    private int _score;
    
    public void StartGame()
    {
        _score = 0;
        scoreMultiplier = 1;

        transform.position = spawnPoint.position;
        
        _snakeBodies.Add(transform);
        
        for (int i = 1; i <= snakeLength; i++)
        {
            var body = Instantiate(snakeBody, transform.position - new Vector3(i, 0, 0), Quaternion.identity);
            _snakeBodies.Add(body.transform);
        }
    }

    void Update()
    {
        if (GameController.Instance.IsPlaying())
        {
            TakeInput();
            Render();
        }
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.IsPlaying())
        {
            var nextPosition = transform.position + _directionVector * moveSpeed;
            nextPosition.x = Mathf.Round(nextPosition.x);
            nextPosition.y = Mathf.Round(nextPosition.y);

            for (int i = _snakeBodies.Count - 1; i > 0; i--)
            {
                _snakeBodies[i].position = _snakeBodies[i - 1].position;
            }
            
            if (!UpdateIfOutOfBounds())
            {
                transform.position = nextPosition;
            }
        }
    }

    private bool UpdateIfOutOfBounds()
    {
        if (transform.position.x > 9f)
        {
            transform.position = new Vector3(-9f, transform.position.y, transform.position.z);
            return true;
        }
        else
        {
            if (transform.position.x < -9f)
            {
                transform.position = new Vector3(9f, transform.position.y, transform.position.z);
                return true;
            }
            if (transform.position.y > 3f)
            {
                transform.position = new Vector3(transform.position.x, -4f, transform.position.z);
                return true;
            }

            if (transform.position.y < -4f)
            {
                transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
                return true;
            }
        }

        return false;
    }

    private void Render()
    {
        if (_directionVector == Vector3.left)
        {
            spriteRenderer.flipX = true;
            spriteRenderer.transform.rotation = Quaternion.identity;
        }
        else if (_directionVector == Vector3.right)
        {
            spriteRenderer.flipX = false;
            spriteRenderer.transform.rotation = Quaternion.identity;
        }
        else if (_directionVector == Vector3.up)
        {
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 90);
            spriteRenderer.flipX = false;
        }
        else if (_directionVector == Vector3.down)
        {
            spriteRenderer.flipX = false;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
    }

    private void TakeInput()
    {
        if (Input.GetKeyDown(leftKey))
        {
            //Move left
            _directionVector = Vector3.left;
        }
        else if (Input.GetKeyDown(rightKey))
        {
            //Move right
            _directionVector = Vector3.right;
        }
        else if (Input.GetKeyDown(upKey))
        {
            //Move up
            _directionVector = Vector3.up;
        }
        else if (Input.GetKeyDown(downKey))
        {
            //Move down
            _directionVector = Vector3.down;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameController.Instance.IsPlaying())
        {
            return;
        }
        if (other.TryGetComponent<Drop>(out var drop))
        {
            var powerupType = drop.GetPowerupType();
            
            switch (powerupType)
            {
                case Drop.PowerupType.Speed:
                    // Time.fixedDeltaTime -= 0.02f;
                    moveSpeed += .1f;
                    break;
                case Drop.PowerupType.Slow:
                    // Time.fixedDeltaTime += 0.02f;
                    moveSpeed -= .1f;
                    break;
                case Drop.PowerupType.Grow:
                    score += 10 * scoreMultiplier;
                    StartCoroutine(ResetScoreMultiplier());
                    Grow();
                    break;
                case Drop.PowerupType.Shrink:
                    Shrink();
                    break;
                case Drop.PowerupType.Points:
                    scoreMultiplier *= 2;
                    break;
                default:
                    Debug.LogError($"Unknown powerup type: {powerupType}");
                    break;
            }
        }

        if (other.TryGetComponent<SnakeController>(out var snakeController))
        {
            GameController.Instance.EndGame();
        }

        if (other.CompareTag("Tail"))
        {
            GameController.Instance.EndGame();
        }
    }
    
    public void ClearSnake()
    {
        for (int i = 0; i < _snakeBodies.Count - 1; i++)
        {
            Shrink();
        }
    }
    
    private IEnumerator ResetScoreMultiplier()
    {
        yield return new WaitForSeconds(5f);
        scoreMultiplier = 1;
    }

    private void Shrink()
    {
        var body = _snakeBodies[_snakeBodies.Count - 1];
        // We can't destroy the head
        if (body.gameObject.GetComponent<SnakeController>() != null)
        {
            return;
        }
        _snakeBodies.Remove(body);
        Destroy(body.gameObject);
    }

    private void Grow()
    {
        var body = Instantiate(snakeBody, _snakeBodies[_snakeBodies.Count - 1].position, Quaternion.identity);
        _snakeBodies.Add(body.transform);
    }
}
