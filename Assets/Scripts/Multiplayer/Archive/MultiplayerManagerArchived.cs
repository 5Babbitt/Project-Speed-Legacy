using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MultiplayerManagerArchived : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject usernamePanel;
    [SerializeField] private GameObject connectPanel;
    [SerializeField] private GameObject roomPanel;
    
    [SerializeField] private InputField hostInput;
    [SerializeField] private InputField joinInput;
    [SerializeField] private InputField usernameInput;

    [SerializeField] private Button enterButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_Text pingText;

    [SerializeField] private GameObject startButton;

    [SerializeField] private GameObject playerList;
    [SerializeField] private GameObject playerListItem;

    private Dictionary<int, GameObject> playerListItems;

    private void Awake() 
    {
        usernamePanel.SetActive(true);
        connectPanel.SetActive(false);
        roomPanel.SetActive(false);

        hostButton.interactable = false;
        enterButton.interactable = false;
        joinButton.interactable = false;

        StartCoroutine("UpdatePing");
    }
    
    public void HostGame()
    {
        PhotonNetwork.CreateRoom(hostInput.text, new RoomOptions() {MaxPlayers = 8}, null); 
    }

    public void JoinGame()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public void LeaveGame()
    {
        
    }

    public override void OnJoinedRoom()
    {
        connectPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerlist();
        startButton.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        connectPanel.SetActive(true);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startButton.SetActive(CheckPlayersReady());
        }
    }


    public void ChangeUsername()
    {
        if (usernameInput.text.Length >=3)
        {
            enterButton.interactable = true;
        }
        else
        {
            enterButton.interactable = false;
        }
    }

    public void SetUsername()
    {
        PhotonNetwork.NickName = usernameInput.text;
        Debug.Log("username set: "+ usernameInput.text);
        usernamePanel.SetActive(false);
        connectPanel.SetActive(true);
    }

    public void HostName()
    {
        if (hostInput.text.Length >=1)
        {
            hostButton.interactable = true;
        }
        else
        {
            hostButton.interactable = false;
        }
    }

    public void JoinName()
    {
        
        if (joinInput.text.Length >= 1)
        {
            joinButton.interactable = true;          
        }
        else
        {
            joinButton.interactable = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player _player)
    {
        UpdatePlayerlist();

        startButton.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player _player)
    {
        UpdatePlayerlist();

        startButton.SetActive(CheckPlayersReady());
    }

    private void UpdatePlayerlist()
    {
        for(int i =0; i < playerList.transform.childCount; i++)
        {
            Destroy(playerList.transform.GetChild(i).gameObject);
        }

        var players = PhotonNetwork.PlayerList;
        
        if (playerListItems==null)
        {
	        playerListItems = new Dictionary<int, GameObject>();
        }


        foreach (Player player in players)
        {
            GameObject listItem = Instantiate(playerListItem, new Vector2(0, 0), Quaternion.identity);
            listItem.transform.SetParent(playerList.transform, false);

            
            PlayerListItemArchived listItemScript = listItem.GetComponent<PlayerListItemArchived>();
            
            listItemScript.Initialize(player.ActorNumber, player.NickName);

            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(ReadyUpScript._PlayerReady, out isPlayerReady))
            {
		        listItemScript.SetReady((bool)isPlayerReady);
            }

            playerListItems.Add(player.ActorNumber, playerListItem);

        }
    }

    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {        
        GameObject playerListGameObject;
        if (playerListItems.TryGetValue(target.ActorNumber,out playerListGameObject ))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(ReadyUpScript._PlayerReady,out isPlayerReady ))
            {
                playerListGameObject.GetComponent<PlayerListItemArchived>().SetReady((bool)isPlayerReady);
            }
        }

        startButton.SetActive(CheckPlayersReady());
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(ReadyUpScript._PlayerReady, out isPlayerReady ))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator UpdatePing()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            pingText.text = ("ping: " + PhotonNetwork.GetPing().ToString() + "ms");
        }
    }
}
