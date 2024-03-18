using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePause;
    public event EventHandler OnGameUnPause;

    public enum State
    {
        waitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private State state;
    private float waitingToStartTime = 1f;
    private float countdownToStartTime = 3f;
    private float gamePlayingTime;
    private float gamePlayingTimeMax = 20f;
    private bool isGamePause;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GameManager instance.");
        }

        Instance = this;
        state = State.waitingToStart;
    }

    private void Update()
    {
        switch (state)
        {
            case State.waitingToStart:
                waitingToStartTime -= Time.deltaTime;
                if (waitingToStartTime < 0f)
                {
                    state = State.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                Debug.Log("Waiting to start.");
                break;
            case State.CountdownToStart:
                countdownToStartTime -= Time.deltaTime;
                if (countdownToStartTime < 0f)
                {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    gamePlayingTime = gamePlayingTimeMax;
                }
                Debug.Log("Countdown to start.");
                break;
            case State.GamePlaying:
                gamePlayingTime -= Time.deltaTime;
                if (gamePlayingTime < 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                Debug.Log("Game is Playing.");
                break;
            case State.GameOver:
                Debug.Log("Game Over");
                break;
            default:
                break;
        }
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public float GetCountDownToStartTime()
    {
        return countdownToStartTime;
    }

    public bool IsCountdownState()
    {
        return state == State.CountdownToStart;
    }

    public bool IsGameOverState()
    {
        return state == State.GameOver;
    }

    public float GetGamePlayingTimeNomalized()
    {
        return 1 - (gamePlayingTime / gamePlayingTimeMax);
    }

    public void TogglePause()
    {
        isGamePause = !isGamePause;

        if (isGamePause)
        {
            OnGamePause?.Invoke(this, EventArgs.Empty);
            Time.timeScale = 0f;
        }
        else
        {
            OnGameUnPause?.Invoke(this, EventArgs.Empty);
            Time.timeScale = 1f;
        }

    }
}
