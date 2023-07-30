using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonPlayer : MonoBehaviour
{
    
    public GameObject playerPrefab;

    GameObject _player;

    PhotonView photonView;

    Player[] allPlayers;
    int myPosition = 0;

    public static GameObject LocalPlayerInstance;

    private void Start() 
    {
        photonView = GetComponent<PhotonView>();
        
        allPlayers = PhotonNetwork.PlayerList;

        foreach (Player player in allPlayers)
        {
            if(player != PhotonNetwork.LocalPlayer)
            {
                myPosition++;
            }
        }

        if(photonView.IsMine)
        {
            _player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Car"), GameManager_.instance.spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber % allPlayers.Length].position, Quaternion.identity, 0);
            _player.transform.forward = GameManager_.instance.spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber % allPlayers.Length].transform.right;
        }
    }
}
