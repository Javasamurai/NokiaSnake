using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] GamePlayStruct[] gameplayStates;
    [SerializeField] private SnakeController snake1;
    [SerializeField] private SnakeController snake2;

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
        restartButton.onClick.AddListener(StartGame);
        snake1.enabled = false;
        snake2.enabled = false;
    }
    
    public bool IsPlaying()
    {
        return _gameplayState == GameplayState.Playing;
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
        snake1.enabled = true;
        snake2.enabled = true;
        snake1.StartGame();
        snake2.StartGame();
    }

    public void EndGame()
    {
        Debug.Log("Game Over");
        string winner;
        if (snake1.score > snake2.score)
        {
            winner = snake1.snakeName;
        }
        else
        {
            if (snake1.score < snake2.score)
            {
                winner = snake2.snakeName;
            }
            else
            {
                winner = "Nobody";
            }
        }
        resultText.text = winner + " wins!";
        SwitchGameplayState(GameplayState.GameOver);
        snake1.ClearSnake();
        snake2.ClearSnake();
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(StartGame);
        restartButton.onClick.RemoveListener(StartGame);
    }
}
