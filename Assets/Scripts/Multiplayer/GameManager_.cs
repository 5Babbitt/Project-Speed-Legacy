using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;

public class GameManager_ : MonoBehaviourPunCallbacks
{
    
    public static GameManager_ instance;

    public Transform[] spawnPoints;

    private void Start() 
    {
        instance = this;
    }
    
    
    #region Photon Callbacks
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Main Menu");
    }

    #endregion


    #region Public Methods
    public void LeaveRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public void ReturnToLobby()
    {
        PhotonNetwork.LoadLevel("LobbyTest");
    }

    #endregion
}