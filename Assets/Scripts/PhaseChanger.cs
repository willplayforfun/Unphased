using UnityEngine;
using System.Collections;

public class PhaseChanger : MonoBehaviour {

    public void SetPhase(Phase phase) {
        Debug.Log("New phase: " + phase);
    }
}
