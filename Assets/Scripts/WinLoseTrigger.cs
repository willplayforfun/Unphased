using UnityEngine;
using System.Collections;

public class WinLoseTrigger : MonoBehaviour {

    public enum Condition
    {
        Win,
        Lose
    }
    public Condition condition;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == 9) {
            switch (condition)
            {
                case Condition.Lose:
                    Application.LoadLevel(2);
                    break;
                case Condition.Win:
                    Application.LoadLevel(3);
                    break;
            }
        }
    }
}
