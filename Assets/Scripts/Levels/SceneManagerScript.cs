using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SceneManagerScript : MonoBehaviour
{
    public void LoadTrack(string trackName)
    {
        PhotonNetwork.LoadLevel(trackName);
    }

    public void DisconnectFromServer()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
