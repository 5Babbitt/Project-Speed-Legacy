using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerListItemArchived : MonoBehaviourPun
{
    [SerializeField] private GameObject hostIcon;
    [SerializeField] private TMP_Text username;

    [SerializeField] private Sprite ready;
    [SerializeField] private Sprite notReady;
    
    [SerializeField] private bool _isReady = false;

    [SerializeField] private Button readyButton;

    public Toggle readyToggle {get; private set;}

    public void Initialize(int playerID, string _username)
    {
        username.text = _username;

        if (PhotonNetwork.IsMasterClient)
        {
            hostIcon.SetActive(true);
        }
        else
        {
            hostIcon.SetActive(false);
        }

        if (PhotonNetwork.LocalPlayer.ActorNumber != playerID)
        {
            readyButton.gameObject.SetActive(false);
        }
        else
        {
            ExitGames.Client.Photon.Hashtable initialProps = new ExitGames.Client.Photon.Hashtable(){{ReadyUpScript._PlayerReady, _isReady}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);

            readyButton.onClick.AddListener(() =>
            {
                _isReady = !_isReady;
                SetReady(_isReady);

                ExitGames.Client.Photon.Hashtable newProps = new ExitGames.Client.Photon.Hashtable(){{ReadyUpScript._PlayerReady,_isReady}};
                PhotonNetwork.LocalPlayer.SetCustomProperties(newProps);
            });
        }
    }

    public void SetReady(bool isReady)
    {
        if (isReady == true)
        {
            readyButton.GetComponent<Image>().sprite = ready;
            readyButton.GetComponentInChildren<TMP_Text>().text = "Ready!";
            readyToggle.isOn = true;
        }
        else
        {
            readyButton.GetComponent<Image>().sprite = notReady;
            readyButton.GetComponentInChildren<TMP_Text>().text = "Ready?";
            readyToggle.isOn = false;
        }
    }
}
