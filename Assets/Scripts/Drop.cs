using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Drop: MonoBehaviour
{
    public enum PowerupType
    {
        Speed,
        Slow,
        Grow,
        Shrink,
        Points
    }
    
    [SerializeField] float dropDelay = 3f;

    private PowerupType powerupType;
    private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private PowerupStruct[] powerupStructs;
    [Serializable]
    private struct PowerupStruct
    {
        public PowerupType powerupType;
        public Sprite sprite;
        public Color color;
    }

    public PowerupType GetPowerupType()
    {
        return powerupType;
    }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Respawn();
    }

    private PowerupType SelectPowerupType()
    {
        var random = Random.Range(0, powerupStructs.Length);
        powerupType = powerupStructs[random].powerupType;
        return powerupType;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _spriteRenderer.color = Color.clear;
            StartCoroutine(Respawn());
        }
    }
    
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(dropDelay);
        // Spawn at any new location
        var randomX = Mathf.Round(UnityEngine.Random.Range(-9f, 9f));
        var randomY = Mathf.Round(UnityEngine.Random.Range(-4f, 3f));
        transform.position = new Vector3(randomX, randomY, 0f);
        powerupType = SelectPowerupType();
        var powerup  = Array.Find(powerupStructs, x => x.powerupType == powerupType);
        _spriteRenderer.color = powerup.color;
    }
}
