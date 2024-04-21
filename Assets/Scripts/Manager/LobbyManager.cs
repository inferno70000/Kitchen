using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

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
        if (
            joinedLobby == null && 
            AuthenticationService.Instance.IsSignedIn && 
            SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString()
            )
        {
            LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
        }
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

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(GameNetworkManager.MAX_PLAYERS - 1);

            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);

            return default;
        }
    }

    private async Task<string> GetJoinRelayCode(Allocation allocation)
    {
        try
        {
            string joinRelayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return joinRelayCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);

            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string relayCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);

            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);

            return default;
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

            //relay
            Allocation allocation = await AllocateRelay();

            string joinRelayCode = await GetJoinRelayCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new(DataObject.VisibilityOptions.Member, joinRelayCode)}
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new(allocation, "dtls"));

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

            JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new(joinAllocation, "dtls"));

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

            JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new(joinAllocation, "dtls"));

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

            JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new(joinAllocation, "dtls"));

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
