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
    private float countdownToStartTime = 1f; //3f
    private float gamePlayingTime;
    private float gamePlayingTimeMax = 500f; //120f
    private bool isGamePause;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GameManager instance.");
        }

        Instance = this;
        //state = State.waitingToStart;
    }

    private void Start()
    {
        InputManager.Instance.OnInteractAction += InputManager_OnInteractAction;
    }

    private void InputManager_OnInteractAction(object sender, EventArgs e)
    {
        if (state == State.waitingToStart)
        {
            //state = State.CountdownToStart;
            //OnStateChanged?.Invoke(this, EventArgs.Empty);

            state = State.GamePlaying;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
            gamePlayingTime = gamePlayingTimeMax;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.waitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTime -= Time.deltaTime;
                if (countdownToStartTime < 0f)
                {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    gamePlayingTime = gamePlayingTimeMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTime -= Time.deltaTime;
                if (gamePlayingTime < 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Get boolean whether game is playing
    /// </summary>
    /// <returns>boolean</returns>
    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    /// <summary>
    /// Get countdown timer
    /// </summary>
    /// <returns>countdownToStartTime</returns>
    public float GetCountDownToStartTime()
    {
        return countdownToStartTime;
    }

    /// <summary>
    /// Get boolean whether game is countdown
    /// </summary>
    /// <returns>boolean</returns>
    public bool IsCountdownState()
    {
        return state == State.CountdownToStart;
    }

    /// <summary>
    /// Get boolean whether game is over
    /// </summary>
    /// <returns>boolean</returns>
    public bool IsGameOverState()
    {
        return state == State.GameOver;
    }

    /// <summary>
    /// Get gamePlaying time nomalized
    /// </summary>
    /// <returns>float</returns>
    public float GetGamePlayingTimeNomalized()
    {
        return 1 - (gamePlayingTime / gamePlayingTimeMax);
    }

    /// <summary>
    /// Toggle pause
    /// </summary>
    public void TogglePause()
    {
        isGamePause = !isGamePause;

        if (isGamePause)
        {
            Cursor.visible = true;
            OnGamePause?.Invoke(this, EventArgs.Empty);
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.visible = false;
            OnGameUnPause?.Invoke(this, EventArgs.Empty);
            Time.timeScale = 1f;
        }
    }
}
