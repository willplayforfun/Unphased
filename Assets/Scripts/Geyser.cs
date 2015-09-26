using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Geyser : MonoBehaviour {

	public float appliedVelocity = 50f;

	InputManager.Mode phase;
	GameObject cloudManager;

	void onAwake() {
		cloudManager = GameObject.Find("CloudManager");
	}

	void OnUpdate() {
		phase = GetComponent<InputManager>().currentMode;
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (phase == InputManager.Mode.Solid) {
			coll.attachedRigidbody.velocity *= (appliedVelocity * -1);
		}
		if (phase == InputManager.Mode.Liquid || phase == InputManager.Mode.Gas) {
			// instead of pushing a single ball push the whole object
			cloudManager.gameObject.GetComponent<Rigidbody2D>().velocity *= (appliedVelocity * -1);
		}
	}
}