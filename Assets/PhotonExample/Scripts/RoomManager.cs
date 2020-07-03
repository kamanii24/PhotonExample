// =================================
//
//	RoomManager.cs
//	Created by Takuya Himeji
//
// =================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : PhotonManager
{
    #region Inspector Settings
    [Header("----")]
    [SerializeField] private string prefabName = "Avatar";
    [SerializeField] private Transform[] playerPositions;

    #endregion // Inspector Settings


    #region Member Field
    
    #endregion // Member Field


    #region MonoBehaviour Methods

    override protected void Awake()
    {
        base.Awake();	// 親クラスAwake
    }

    override protected void Start()
    {
        base.Start();   // 親クラスStart

        // ルームに入ったときのイベント登録
        OnJoinedRoomEvent.AddListener(OnInstatiateAvatar);
    }

    override protected void Update()
    {
        base.Update();   // 親クラスUpdate
    }

    #endregion // MonoBehaviour Methods


    #region Member Methods
    // プレイヤーアバターを生成する
    private void OnInstatiateAvatar()
    {
        int index = PhotonNetwork.room.PlayerCount - 1;

        // 表示するプレイヤーアバターの座標、回転値を設定
        Vector3 p = playerPositions[index].position;
        Quaternion r = playerPositions[index].rotation;

        // プレイヤーアバターをインタンス化     (Prefab名, 座標, 回転, グループID?)
        GameObject player = PhotonNetwork.Instantiate(prefabName, p, r, 0);
    }
    #endregion // Member Methods
}