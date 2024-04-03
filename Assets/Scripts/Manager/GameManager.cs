using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePause;
    public event EventHandler OnGameUnPause;    
    public event EventHandler OnMultiPlayerGamePause;
    public event EventHandler OnMultiPlayerGameUnPause;
    public event EventHandler OnLocalPlayerReady;

    public enum State
    {
        waitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private NetworkVariable<State> state = new(State.waitingToStart);
    private NetworkVariable<float> countdownToStartTime = new(3f);
    private NetworkVariable<float> gamePlayingTime = new(0f);
    private NetworkVariable<bool> isGamePause = new(false);
    private float gamePlayingTimeMax = 120f;
    private bool isLocalGamePause;
    private Dictionary<ulong, bool> playersReadyDictionary = new();
    private Dictionary<ulong, bool> playersPauseDictionary = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GameManager instance.");
        }

        Instance = this;
    }

    private void Start()
    {
        InputManager.Instance.OnInteractAction += InputManager_OnInteractAction;
        state.OnValueChanged += State_OnValueChanged;
        isGamePause.OnValueChanged += IsGamePause_OnValueChanged;
    }

    private void IsGamePause_OnValueChanged(bool previousValue, bool newValue)
    {
        if (newValue)
        {
            Time.timeScale = 0f;
            OnMultiPlayerGamePause?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnMultiPlayerGameUnPause?.Invoke(this, EventArgs.Empty);
        }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void InputManager_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.waitingToStart)
        {
            OnLocalPlayerReady?.Invoke(this, EventArgs.Empty);

            UpdatePlayersReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)] 
    private void UpdatePlayersReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playersReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allPlayersReady = true;
        foreach (ulong  clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playersReadyDictionary.ContainsKey(clientId) || !playersReadyDictionary[clientId])
            {
                allPlayersReady = false;
            }
        }

        if(allPlayersReady)
        {
            state.Value = State.CountdownToStart;
        }
    }

    private void Update()
    {
        if (!IsServer) { return; }

        switch (state.Value)
        {
            case State.waitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTime.Value -= Time.deltaTime;
                if (countdownToStartTime.Value < 0f)
                {
                    state.Value = State.GamePlaying;
                    gamePlayingTime.Value = gamePlayingTimeMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTime.Value -= Time.deltaTime;
                if (gamePlayingTime.Value < 0f)
                {
                    state.Value = State.GameOver;
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
        return state.Value == State.GamePlaying;
    }

    /// <summary>
    /// Get countdown timer
    /// </summary>
    /// <returns>countdownToStartTime</returns>
    public float GetCountDownToStartTime()
    {
        return countdownToStartTime.Value;
    }

    /// <summary>
    /// Get boolean whether game is countdown
    /// </summary>
    /// <returns>boolean</returns>
    public bool IsCountdownState()
    {
        return state.Value == State.CountdownToStart;
    }

    /// <summary>
    /// Get boolean whether game is over
    /// </summary>
    /// <returns>boolean</returns>
    public bool IsGameOverState()
    {
        return state.Value == State.GameOver;
    }

    /// <summary>
    /// Get gamePlaying time nomalized
    /// </summary>
    /// <returns>float</returns>
    public float GetGamePlayingTimeNomalized()
    {
        return 1 - (gamePlayingTime.Value / gamePlayingTimeMax);
    }

    /// <summary>
    /// Toggle pause
    /// </summary>
    public void TogglePause()
    {
        isLocalGamePause = !isLocalGamePause;

        if (isLocalGamePause)
        {
            OnGamePause?.Invoke(this, EventArgs.Empty);
            PauseMultiPlayerServerRpc();
        }
        else
        {
            OnGameUnPause?.Invoke(this, EventArgs.Empty);
            UnpauseMultiPlayerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseMultiPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playersPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;

        UpdateGamePauseState();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void UnpauseMultiPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playersPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;

        UpdateGamePauseState();
    }

    private void UpdateGamePauseState()
    {

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playersPauseDictionary.ContainsKey(clientId) && playersPauseDictionary[clientId])
            {
                //One player paused
                isGamePause.Value = true;
                return;
            }
        }

        //All player unpause
        isGamePause.Value = false;
    }
}
