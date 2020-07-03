// =================================
//
//	PhotonManager.cs
//	Created by Takuya Himeji
//
// =================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public abstract class PhotonManager : Photon.MonoBehaviour
{
    #region Inspector Settings
    [Header("PhotonSettings")]
    [SerializeField] protected byte version = 1;
    [Header("RoomInfo")]
    [SerializeField] private string roomName = "MyRoom";
    [SerializeField] protected byte maxPlayers = 5;
    [Header("Dev")]
    [SerializeField] private bool showGUI = true;

    #endregion // Inspector Settings


    #region Member Field
    
    public UnityEvent OnJoinedRoomEvent;

    #endregion // Member Field


    #region MonoBehaviour Methods
    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        // ロビーへの参加を自動化する
        PhotonNetwork.autoJoinLobby = true;
        // シーンの遷移を自動化する
        PhotonNetwork.automaticallySyncScene = true;

        // Photonに接続されていなければ接続する
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings(version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
        }
    }

    protected virtual void Update()
    {
        
    }

    // GUI処理
    // Photonの情報表示用
    void OnGUI()
    {
        if(!showGUI) return;    // GUIの表示判定


        // GUI用の解像度を調整する
        Vector2 guiScreenSize = new Vector2(800, 480);
        if (Screen.width > Screen.height)
        {
            // 横画面
            GUIUtility.ScaleAroundPivot(new Vector2(Screen.width / guiScreenSize.x, Screen.height / guiScreenSize.y), Vector2.zero);
        }
        else
        {
            // 縦画面
            GUIUtility.ScaleAroundPivot(new Vector2(Screen.width / guiScreenSize.y, Screen.height / guiScreenSize.x), Vector2.zero);
        }

        GUILayout.Label("Connection State : " + PhotonNetwork.connectionState);
        GUILayout.Label("Connection State Detailed : " + PhotonNetwork.connectionStateDetailed);

        // プレイヤーIDの表示
        if (PhotonNetwork.player != null)
        {
            GUILayout.Label("My Player ID : " + PhotonNetwork.player.ID);
        }

        // ルーム情報の取得
        Room room = PhotonNetwork.room;
        if (room != null)
        {
            // ルーム名の表示
            GUILayout.Label("Room Name : " + room.Name);

            // ルーム内に存在するプレイヤー数の表示
            GUILayout.Label("PlayerCount : " + room.PlayerCount);

            // ルームがオープンかクローズか
            GUILayout.Label("Room IsOpen : " + room.IsOpen);
        }
        
        // 部屋からの離脱ボタン処理
        if(PhotonNetwork.inRoom)
        {
            if (GUILayout.Button("退室", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(64) }))
            {
                // 部屋から離脱する
                PhotonNetwork.LeaveRoom();

                // Photonとの接続を切断
                PhotonNetwork.Disconnect();
            }
        }
        else
        {
            // ルーム作成、入室ボタン
            if (GUILayout.Button("入室", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(64) }))
            {
                CreateRoomOrJoin(roomName);
            }
        }
    }
    #endregion // MonoBehaviour Methods


    #region PUN Callback Methods
    // マスターサーバーへアクセス
    public virtual void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster()");
    }

    // ロビーへ参加
    public virtual void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby()");
    }

    // ルームへの入室失敗
    public virtual void OnPhotonJoinRoomFailed()
    {
        Debug.Log("OnPhotonJoinRoomFailed()");
    }

    // Photonサーバーへの接続失効
    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    // ルーム作成
    public virtual void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom()");
    }

    // ルーム入室
    public virtual void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        // JoinRoomイベント呼び出し
        OnJoinedRoomEvent.Invoke();
    }

    // Photonサーバー切断時
    public virtual void OnDisconnectedFromPhoton()
    {
        // シーンをリロード
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
    }

    #endregion // PUN Callback Methods


    #region Member Methods
    // ルームを作成する
    public void OnCreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = maxPlayers }, null);
    }

    // ルーム作成 or 入室する
    public void CreateRoomOrJoin(string roomName)
    {
        // Photonに接続されている
        if (PhotonNetwork.connected)
        {
            // ルームの存在確認
            bool exist = false;
            foreach (RoomInfo r in PhotonNetwork.GetRoomList())
            {
                if (r.Name == roomName) { exist = true; break; }
            }

            if (exist)
            {
                // すでに同名のルームが存在している場合はJoin
                PhotonNetwork.JoinRoom(roomName);
            }
            else
            {
                // ルームが存在しない場合はCreate
                PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = maxPlayers }, null);
            }
        }
    }
    #endregion // Member Methods
}