using UnityEngine;

public class MenuManager : MonoBehaviour {

    public void Update() {
        if (Input.GetButtonDown("Start"))
        {
            Application.LoadLevel(1);
        }
    }    
}

