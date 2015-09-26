using UnityEngine;
using System.Collections;

public class FadeOutAfterTime : MonoBehaviour {

    public float duration;

	void Start () {
        StartCoroutine(Fade(duration));
	}
	
	IEnumerator Fade (float time) {
        float timer = 0;
        while (timer < time)
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.clear, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
