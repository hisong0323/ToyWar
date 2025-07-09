using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField]
    private NetworkRunner runnerPrefab;

    [SerializeField]
    private int maxPlayer = 2; // 최대 플레이어 수를 4로 지정

    [SerializeField]
    private SceneRef _gameSceneRef;

    [SerializeField] private NetworkPrefabRef _playerPrefab;

    private int currentPlayerCount = 0;

    private NetworkRunner _runner;

    private bool _spawnButton;

    private Vector3 _spawnPosition;
    public NetworkRunner Runner => _runner;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        //_spawnButton = _spawnButton | Input.GetKeyDown(KeyCode.F);
        if (Input.GetKeyDown(KeyCode.F))
        {
            _spawnButton = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
            {
                _spawnPosition = hit.point;
                _spawnPosition.y = 2;
            }
        }
    }

    public void StartRandomMatching()
    {
        Debug.Log("랜덤 매칭 시작...");
        _runner = CreateRunner();

        // Fusion의 자동 랜덤 매칭: SessionName = null, GameMode = AutoHostOrClient
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = null, // null로 두면 Fusion이 알아서 랜덤 매칭
            PlayerCount = maxPlayer,
           // SceneManager = runnerPrefab.GetComponent<NetworkSceneManagerDefault>()

        };

        _runner.StartGame(startGameArgs);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("서버에 연결됨!");

        // 현재 참가한 세션(방) 이름을 출력
        if (runner.SessionInfo != null && runner.SessionInfo.IsValid)
        {
            Debug.Log($"현재 참가한 방 이름: {runner.SessionInfo.Name}");
        }
        else
        {
            Debug.Log("SessionInfo가 유효하지 않습니다.");
        }
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"서버에 연결 실패! 이유: {reason}");
        // 연결 실패 후 재시도할지 결정하는 로직을 추가할 수 있습니다.
        // 재시도 로직이 여기에 추가될 수 있습니다.

        // 연결 실패 후 기존 러너를 종료합니다.

        _runner.Shutdown();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 입장: {player.PlayerId}");

        currentPlayerCount = runner.ActivePlayers.Count(); // 현재 방의 실제 인원 수
        Debug.Log($"현재 플레이어 수: {currentPlayerCount}/{maxPlayer}");


        if (runner.IsServer)
        {

            Debug.Log($"[Host] 현재 플레이어 수: {currentPlayerCount}/{maxPlayer}");
            if (currentPlayerCount >= maxPlayer)
            {
                _runner.ProvideInput = true;
                Debug.Log("최대 인원 도달! 게임 씬으로 이동합니다.");
                Join(runner);
            }
        }
    }

    private async Task Join(NetworkRunner runner)
    {
        await runner.LoadScene(_gameSceneRef);
        runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 퇴장: {player.PlayerId}");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        data.Buttons.Set(NetworkInputData.SpawnButton, _spawnButton);

        data.SpawnPosition = _spawnPosition;

        _spawnButton = false;
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log($"Runner 종료: {shutdownReason}"); }
    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.Log("서버와의 연결이 끊어졌습니다"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { Debug.Log("로드 완료!"); }
    public void OnSceneLoadStart(NetworkRunner runner) { Debug.Log("로드 시작..."); }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void CancelMatching()
    {
        Debug.Log("매칭 취소: 러너 셧다운");
        Destroy(GetComponent<NetworkSceneManagerDefault>());
        _runner.Shutdown();
    }

    private NetworkRunner CreateRunner()
    {
        var runner = Instantiate(runnerPrefab);
        runner.AddCallbacks(this);
        runner.ProvideInput = true;
        return runner;
    }
}