using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] GamePlayStruct[] gameplayStates;

    [Serializable]
    struct GamePlayStruct
    {
        public GameplayState gameplayState;
        public GameObject panel;
    }
    
    private static GameController _instance;
    public static GameController Instance
    {
        get
        {
            return _instance;
        }
    }

    private int score;


    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;    
        }
    }

    private GameplayState _gameplayState = GameplayState.Menu;
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }
    
    public bool IsPlaying()
    {
        return _gameplayState == GameplayState.Playing;
    }
    
    public void UpdateScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }
    
    private void SwitchGameplayState(GameplayState newGameplayState)
    {
        RenderGameplayState(newGameplayState);
        _gameplayState = newGameplayState;
    }
    
    private void RenderGameplayState(GameplayState _gameplayState)
    {
        foreach (var gameplayState in gameplayStates)
        {
            if (gameplayState.gameplayState == _gameplayState)
            {
                gameplayState.panel.SetActive(true);
            }
            else
            {
                gameplayState.panel.SetActive(false);
            }
        }
    }
    private void StartGame()
    {
        SwitchGameplayState(GameplayState.Playing);
    }

    public void EndGame()
    {
        Debug.Log("Game Over");
        SwitchGameplayState(GameplayState.GameOver);
    }
}
