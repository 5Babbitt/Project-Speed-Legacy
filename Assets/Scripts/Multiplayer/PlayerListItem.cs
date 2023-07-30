using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerListItem : MonoBehaviourPun
{
    [SerializeField] private GameObject hostIcon;
    [SerializeField] private TMP_Text username;

    public void SetHost(bool isHost)
    {
        if (isHost == true)
        {
            hostIcon.SetActive(true);
        }
        else
        {
            hostIcon.SetActive(false);
        }
    }

    public void SetName(string name)
    {
        username.text = name;
    }
}
