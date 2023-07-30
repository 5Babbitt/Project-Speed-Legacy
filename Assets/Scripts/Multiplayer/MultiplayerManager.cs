using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{    
    [SerializeField] private SceneManagerScript sceneManager;
    
    [Header("Username Panel Variables")]
    [SerializeField] private GameObject usernamePanel;
    [SerializeField] private InputField usernameInput;
    [SerializeField] private Button enterButton;

    [Header("Connect Panel Variables")]
    [SerializeField] private GameObject connectPanel;
    [SerializeField] private InputField hostInput;
    [SerializeField] private InputField joinInput;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    [Header("Room Panel Variables")]
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_Text pingText;  

    [Header("Player List Variables")]
    [SerializeField] private GameObject playerList;
    [SerializeField] private GameObject playerListItem;
    [SerializeField] private GameObject startButton;


    private void Awake() 
    {
        usernamePanel.SetActive(true);
        connectPanel.SetActive(false);
        roomPanel.SetActive(false);

        hostButton.interactable = false;
        enterButton.interactable = false;
        joinButton.interactable = false;
    }

    private void Start() 
    {
        //Syncs Scene for everyone in room with the host
        PhotonNetwork.AutomaticallySyncScene = true;
        
        StartCoroutine("UpdatePing");
        
    }
    
    public void HostGame()
    {
        PhotonNetwork.CreateRoom(hostInput.text, new RoomOptions() {MaxPlayers = 6}, null);       
    }

    public void JoinGame()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public void LeaveGame()
    {
        PhotonNetwork.Disconnect();
        sceneManager.DisconnectFromServer();
    }

    public override void OnJoinedRoom()
    {
        connectPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerlist();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        connectPanel.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player _player)
    {
        UpdatePlayerlist();
    }

    public override void OnPlayerLeftRoom(Player _player)
    {
        UpdatePlayerlist();
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
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

    public void ChangeHostName()
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

    public void ChangeJoinName()
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

    public void OnStartClicked()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        else
        {
            string mapName = (string)PhotonNetwork.CurrentRoom.CustomProperties["Map"];
        
            PhotonNetwork.LoadLevel(mapName);
        }
        
        
    }

    private void UpdatePlayerlist()
    {
        var players = PhotonNetwork.PlayerList;

        for(int i =0; i < playerList.transform.childCount; i++)
        {
            Destroy(playerList.transform.GetChild(i).gameObject);
        }
        
        foreach (Player player in players)
        {
            GameObject listItem = Instantiate(playerListItem, new Vector2(0, 0), Quaternion.identity);
            listItem.transform.SetParent(playerList.transform, false);
            PlayerListItem listItemScript = listItem.GetComponent<PlayerListItem>();

            listItemScript.SetHost(player.IsMasterClient);

            listItemScript.SetName(player.NickName);
        }

        if(PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
            {
                startButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                startButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    IEnumerator UpdatePing()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            pingText.text = ("ping: " + PhotonNetwork.GetPing().ToString() + "ms");
        }
    }
}
