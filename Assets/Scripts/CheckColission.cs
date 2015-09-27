using UnityEngine;
using System.Collections;

public class CheckColission : MonoBehaviour {
	
	IEnumerator OnColissionEnter2D (Collider2D coll) {
		Debug.Log ("FLASHING!!!!!");
		yield return StartCoroutine ("flashingScreen");
	}

	IEnumerator flashingScreen() {
		GameObject.FindWithTag ("FlashingScreen").gameObject.SetActive(true);
		yield return new WaitForSeconds(1f);
		GameObject.FindWithTag ("FlashingScreen").gameObject.SetActive(false);
	}
}
