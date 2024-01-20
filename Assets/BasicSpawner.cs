using Fusion.Sockets;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private Vector3 lookAt;
    private bool mouseButtonSampled = false;
    private PlayerRef serverPlayerRef;

    public void ClientSetLookat(Vector3 _lookAt)
    {
        lookAt = _lookAt;
    }

    public void Update()
    {
        if (!mouseButtonSampled)
        {
            mouseButtonSampled = Input.GetButtonDown("Fire1");
        }
    }
    private int playerCount = 0;
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            if (serverPlayerRef == null)
            {
                serverPlayerRef = player;
            }
            // Create a unique position for the player
            Vector3 spawnPosition;
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("spawnpoint");
            
            if (spawnPoints.Length > 0)
            {
                spawnPosition = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
            }
            else
            {
                spawnPosition = new Vector3(UnityEngine.Random.Range(-1f, 1f), 1.10f, UnityEngine.Random.Range(-1f, 1f));
            }
            

            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            PlayerNetwork pl = networkPlayerObject.GetComponent<PlayerNetwork>();
            pl.playerRef = player;
            pl.serverPlayerRef = serverPlayerRef;
            pl.runner = runner;
            pl.playerId = playerCount++;
            _runner.SetPlayerObject(player, networkPlayerObject);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
        }

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }

    }

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.right;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.Q))
            data.buttons.Set(NetworkInputData.BUTTON_FIRSTABILITY, true);

        if (Input.GetKey(KeyCode.E))
            data.buttons.Set(NetworkInputData.BUTTON_SECONDABILITY, true);

        if (Input.GetKey(KeyCode.Tab))
            data.buttons.Set(NetworkInputData.BUTTON_NOABILITY, true);

        data.lookAt = lookAt;
        data.buttons.Set(NetworkInputData.BUTTON_ATTACK, mouseButtonSampled);
        mouseButtonSampled = false;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {
        var data = new NetworkInputData();
        data.direction = Vector3.zero;
        data.lookAt = lookAt;
        data.buttons.Set(NetworkInputData.BUTTON_ATTACK, Input.GetButtonDown("Fire1"));
        input.Set(data);
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    private NetworkRunner _runner;

    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }
}
