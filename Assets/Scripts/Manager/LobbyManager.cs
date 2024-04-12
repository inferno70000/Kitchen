using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public event EventHandler OnCreateLobby;
    public event EventHandler OnCreateFailed;
    public event EventHandler OnJoinLobby;
    public event EventHandler OnJoinFailed;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> LobbyList;
    }

    private Lobby joinedLobby;

    private void Awake()
    {
        Instance = this;

        Authenticate();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        float queryLobbyTime = 0f;
        float queryLobbyRepeatRate = 2f;
        InvokeRepeating(nameof(ListLobbies), queryLobbyTime, queryLobbyRepeatRate);    
    }

    private void HandleHeartbeatLobby()
    {
        LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void Authenticate()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new();
            options.SetProfile("Player" + UnityEngine.Random.Range(0, 1000));

            await UnityServices.InitializeAsync(options);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new()
            {
                Filters = new List<QueryFilter> { new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT) }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                LobbyList = queryResponse.Results
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            OnCreateLobby?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameNetworkManager.MAX_PLAYERS, new CreateLobbyOptions
            {
                IsPrivate = isPrivate
            });

            if (IsLobbyHost())
            {
                float heartBeatTime = 0f;
                float heartBeatRepeatRate = 15f;
                InvokeRepeating(nameof(HandleHeartbeatLobby), heartBeatTime, heartBeatRepeatRate);
            }

            GameNetworkManager.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectionScene);
        }
        catch (LobbyServiceException e)
        {
            OnCreateFailed?.Invoke(this, EventArgs.Empty);

            Debug.Log(e);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            OnJoinLobby?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            GameNetworkManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);

            Debug.Log(e);
        }
    }

    public async void JoinLobbyWithId(string id)
    {
        try
        {
            OnJoinLobby?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id);

            GameNetworkManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            OnJoinFailed?.Invoke(this, EventArgs.Empty);

            Debug.Log(e);
        }
    }

    public async void JoinLobbyWithCode(string code)
    {
        try
        {
            OnJoinLobby?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);

            GameNetworkManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            OnJoinFailed?.Invoke(this, EventArgs.Empty);

            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public Lobby GetJoinedLobby()
    {
        return joinedLobby;
    }
}
