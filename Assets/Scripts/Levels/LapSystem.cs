using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class LapSystem : MonoBehaviour
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    private List<int> lapCounts;
    private List<Transform> carTransforms;
    
    private int maxLaps;

    [SerializeField] private TMP_Text lapCounter;

    private void Awake() 
    {
        trackCheckpoints.OnPlayerCrossStartLine += TrackCheckpoints_OnPlayerCrossStartLine;
        
        carTransforms = trackCheckpoints.GetCarTransformList();
        lapCounts = trackCheckpoints.playerLapCounts;

        maxLaps = (int)PhotonNetwork.CurrentRoom.CustomProperties["NumLaps"];
    }

    private void Start() 
    {
        UpdateLapCountText(0);
    }

    private void TrackCheckpoints_OnPlayerCrossStartLine(object sender, System.EventArgs e)
    {
        UpdateLapCountText(lapCounts.IndexOf(PhotonNetwork.LocalPlayer.ActorNumber % PhotonNetwork.CurrentRoom.PlayerCount));
    }

    public void UpdateLapCountText(int laps)
    {
        if(PhotonNetwork.LocalPlayer.IsLocal)
        {
            lapCounter.text = laps.ToString() + "/" + maxLaps.ToString() +" Laps";
            if(laps > maxLaps)
            {
                
            }
        }
        else return;
    }
}
