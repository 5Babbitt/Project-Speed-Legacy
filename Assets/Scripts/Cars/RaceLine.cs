using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceLine : MonoBehaviour
{
    public GameObject startLine;
    public GameObject finishLine;

    private void OnTriggerEnter(Collider other) {
        startLine.SetActive(true);
        finishLine.SetActive(false);

    }
}
