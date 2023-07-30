using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;

    private MeshRenderer meshRenderer;

    public Transform spawnPoint {get; private set;}

    private void Awake() 
    {
        meshRenderer = GetComponent<MeshRenderer>();
        spawnPoint = transform.GetChild(0);
    }

    private void Start() 
    {
        Hide();
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            trackCheckpoints.CarThroughCheckpoint(this, other.transform);
        }
    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }

    public void Show()
    {
        meshRenderer.enabled = true;
    }
}
