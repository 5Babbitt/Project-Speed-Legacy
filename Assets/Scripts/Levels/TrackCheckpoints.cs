using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler OnPlayerCorrectCheckpoint;
    public event EventHandler OnPlayerWrongCheckpoint;
    public event EventHandler OnPlayerCrossStartLine;
    public event EventHandler OnPlayerFinished;

    private List<Transform> carTransformList;
    public List<int> playerLapCounts {get; private set;}
    public List<CheckpointSingle> checkpointSingleList {get; private set;}
    public List<int> nextCheckpointSingleIndexList {get; private set;}

    [SerializeField] private Transform checkpointsTransform;
    [SerializeField] private Transform startline;
    
    private void Awake() 
    {
        checkpointSingleList = new List<CheckpointSingle>();
        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {   
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
            checkpointSingleList.Add(checkpointSingle);
        }
        
        carTransformList = new List<Transform>();

        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");

        foreach (GameObject car in cars)
        {
            carTransformList.Add(car.transform);
        }
        
        nextCheckpointSingleIndexList = new List<int>();
        playerLapCounts = new List<int>();
        foreach (Transform carTransform in carTransformList)
        {
            nextCheckpointSingleIndexList.Add(0);
            playerLapCounts.Add(0);
        }
    }

    public void CarThroughCheckpoint(CheckpointSingle checkpointSingle, Transform carTransform)
    {
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            Debug.Log("Passed Correct Checkpoint");

            CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];
            correctCheckpointSingle.Hide();
            
            nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)] = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            OnPlayerCorrectCheckpoint?.Invoke(this, EventArgs.Empty);             
        }
        else
        {
            Debug.Log("Wrong Checkpoint");
            OnPlayerWrongCheckpoint?.Invoke(this, EventArgs.Empty);

            CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];
            correctCheckpointSingle.Show();
        }

        if (checkpointsTransform.GetChild(checkpointSingleList.IndexOf(checkpointSingle)) == startline && checkpointSingleList.IndexOf(startline.GetComponent<CheckpointSingle>()) == nextCheckpointSingleIndex)
        {
            playerLapCounts[carTransformList.IndexOf(carTransform)] += 1;
            Debug.Log(playerLapCounts[carTransformList.IndexOf(carTransform)].ToString());
            OnPlayerCrossStartLine?.Invoke(this, EventArgs.Empty);
        }
    }

    public float DistanceToStart(Transform carTransform)
    {
        int currentNextCheckpoint = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
        float distanceToStart = DistanceToNextCheckpoint(carTransform);
        
        for(int i = currentNextCheckpoint; i <= checkpointsTransform.childCount; i++)
        {
            distanceToStart += Vector3.Distance(checkpointsTransform.GetChild(i).position, checkpointsTransform.GetChild(i + 1).position);
        }

        return distanceToStart;
    }

    public float DistanceToNextCheckpoint(Transform carTransform)
    {
        int currentNextCheckpoint = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
        float DistanceToNextCheckpoint = Vector3.Distance(carTransform.position, checkpointsTransform.GetChild(currentNextCheckpoint).position);
        return DistanceToNextCheckpoint;
    }

    public int GetLapNumber(Transform carTransform)
    {
        int lapNum = playerLapCounts[carTransformList.IndexOf(carTransform)];
        return lapNum;
    }

    public List<Transform> GetCarTransformList()
    {
        return carTransformList;
    }

    public Transform GetCarTransform(Transform carTransform)
    {
        return carTransform;
    }

    public void FinshRace(Transform carTransform)
    {
        if(PhotonNetwork.LocalPlayer.IsLocal)
        {
            carTransform.GetComponent<CarController>().enabled = false;
        }
    }
}
