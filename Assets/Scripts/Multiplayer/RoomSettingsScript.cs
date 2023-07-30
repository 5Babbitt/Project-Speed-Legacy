using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RoomSettingsScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject lapsDropdownObject;
    [SerializeField] private TMP_Dropdown lapsDropdown;
    [SerializeField] private TMP_Text numOfLaps;
    [SerializeField] private GameObject nextMapButton;
    [SerializeField] private Image mapThumbnail;
    [SerializeField] private TMP_Text mapName;
    [SerializeField] private Map[] maps;
    [SerializeField] private int currentMapSelected = 0;
    [SerializeField] private int numLapsInt;

    private ExitGames.Client.Photon.Hashtable _customProps = new ExitGames.Client.Photon.Hashtable();

    private void Start() 
    {
        lapsDropdown = lapsDropdownObject.GetComponent<TMP_Dropdown>();
        
        currentMapSelected = 0;
        UpdateMapSelection(currentMapSelected % maps.Length);

        if(PhotonNetwork.IsMasterClient)
        {
            lapsDropdownObject.SetActive(true);
            nextMapButton.SetActive(true);
        }    
        else
        {
            lapsDropdownObject.SetActive(false);
            nextMapButton.SetActive(false);
        }

        UpdateCustomProps();
    }

    public void NextMap()
    {
        currentMapSelected++;
        UpdateMapSelection(currentMapSelected % maps.Length);
    }

    public void OnDropdownValueChanged()
    {
        ChangeNumberOfLaps();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            lapsDropdownObject.SetActive(true);
            nextMapButton.SetActive(true);
        }    
        else
        {
            lapsDropdownObject.SetActive(false);
            nextMapButton.SetActive(false);
        }
    }

    private void UpdateMapSelection(int x)
    {
        mapThumbnail.sprite = maps[x].thumbnail;
        mapName.text = maps[x].trackName;
        UpdateCustomProps();
    }

    private void ChangeNumberOfLaps()
    {
        if(lapsDropdown.value == 0) numLapsInt = 1;
        else if (lapsDropdown.value == 1) numLapsInt = 2;
        else if (lapsDropdown.value == 2) numLapsInt = 3;
        else if (lapsDropdown.value == 3) numLapsInt = 4;
        else if (lapsDropdown.value == 4) numLapsInt = 5;
        
        UpdateLapsNum(numLapsInt);

        UpdateCustomProps();
    }

    private void UpdateLapsNum(int y)
    {
        numOfLaps.text = y.ToString();
    }

    private void UpdateCustomProps()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        _customProps["Map"] = maps[currentMapSelected % maps.Length].sceneName;
        _customProps["NumLaps"] = numLapsInt;

        //string _mapName = (string)_customProps["Map"];
        //int _numLaps = (int)_customProps["NumLaps"];

        Debug.Log("Map Name: " + _customProps["Map"] + "\nNumber of Laps: " + _customProps["NumLaps"]);
        
        PhotonNetwork.CurrentRoom.SetCustomProperties(_customProps);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {     
        for (int i = 0; i < maps.Length; i++)
        {
            if(maps[i].sceneName == (string)propertiesThatChanged["Map"])
            {
                UpdateMapSelection(i);
            }
        }

        UpdateLapsNum((int)propertiesThatChanged["NumLaps"]);
    }

}