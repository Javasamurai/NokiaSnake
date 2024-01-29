using System;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int snakeLength = 3;
    [SerializeField] private GameObject snakeBody;
    private Vector3 _directionVector = Vector3.right;
    
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    
    private List<Transform> _snakeBodies = new List<Transform>();

    private void Start()
    {
        _snakeBodies.Add(transform);
        for (int i = 0; i < snakeLength; i++)
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
            for (int i = _snakeBodies.Count - 1; i > 0; i--)
            {
                _snakeBodies[i].position = _snakeBodies[i - 1].position;
            }
            var nextPosition = (transform.position + _directionVector * (moveSpeed * 0.1f));
            // nextPosition.x = Mathf.Round(nextPosition.x);
            // nextPosition.y = Mathf.Round(nextPosition.y);

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
            Debug.Log($"Powerup type: {powerupType}");
            
            switch (powerupType)
            {
                case Drop.PowerupType.Speed:
                    // Time.fixedDeltaTime -= 0.02f;
                    moveSpeed += 1f;
                    break;
                case Drop.PowerupType.Slow:
                    // Time.fixedDeltaTime += 0.02f;
                    moveSpeed -= 1f;
                    break;
                case Drop.PowerupType.Grow:
                    Grow();
                    break;
                case Drop.PowerupType.Shrink:
                    Shrink();
                    break;
                case Drop.PowerupType.Points:
                    GameController.Instance.UpdateScore(1);
                    break;
                default:
                    Debug.LogError($"Unknown powerup type: {powerupType}");
                    break;
            }
        }

        if (other.TryGetComponent<SnakeController>(out var snakeController))
        {
            // Hit self or other snake
            GameController.Instance.EndGame();
        }

        if (other.CompareTag("Tail"))
        {
            GameController.Instance.EndGame();
        }
    }

    private void Shrink()
    {
        if (_snakeBodies.Count > 1)
        {
            var body = _snakeBodies[_snakeBodies.Count - 1];
            _snakeBodies.Remove(body);
            Destroy(body.gameObject);
        }
    }

    private void Grow()
    {
        var body = Instantiate(snakeBody, _snakeBodies[_snakeBodies.Count - 1].position, Quaternion.identity);
        _snakeBodies.Add(body.transform);
    }
}
