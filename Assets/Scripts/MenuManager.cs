using UnityEngine;

public class MenuManager : MonoBehaviour {

    public enum NextScene
    {
        Menu,
        Game
    }
    public NextScene nextScene;

    public void Update() {
        if (Input.GetButtonDown("Start"))
        {
            switch (nextScene)
            {
                case NextScene.Menu:
                    Application.LoadLevel(0);
                    break;
                case NextScene.Game:
                    Application.LoadLevel(1);
                    break;
            }
        }
    }    
}

