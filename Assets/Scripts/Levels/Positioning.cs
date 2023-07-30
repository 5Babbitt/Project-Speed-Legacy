using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Positioning : MonoBehaviour
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;

    private List<int> lapCounts;
    private List<Transform> carTransforms;

    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject returnButton;

    [SerializeField] private TMP_Text positionText;

    public int position {get; private set;}
    
    private void Awake() 
    {
        trackCheckpoints.OnPlayerFinished += TrackCheckpoints_OnPlayerFinished;
        
        carTransforms = trackCheckpoints.GetCarTransformList();
        lapCounts = trackCheckpoints.playerLapCounts;

        endScreen.SetActive(false);
        position = PhotonNetwork.PlayerList.Length;
    }

    private void Update() 
    {
        if(PhotonNetwork.LocalPlayer.IsLocal)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                foreach (Player _player in PhotonNetwork.PlayerListOthers)
                {
                    if (trackCheckpoints.DistanceToStart(carTransforms[player.ActorNumber]) > trackCheckpoints.DistanceToStart(carTransforms[_player.ActorNumber]) && trackCheckpoints.GetLapNumber(carTransforms[player.ActorNumber]) == trackCheckpoints.GetLapNumber(carTransforms[_player.ActorNumber]))
                    {
                        position -= 1;
                    }
                }
            }
        }
        else return;
    }

    private void TrackCheckpoints_OnPlayerFinished(object sender, System.EventArgs e)
    {
        endScreen.SetActive(true);

        if(!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            returnButton.SetActive(false);
        }
    }
}
