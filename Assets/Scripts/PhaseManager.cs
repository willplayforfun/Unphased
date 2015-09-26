using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum Phase {
    Solid,
    Liquid,
    Gas
}

public class PhaseManager : MonoBehaviour {
    public PhaseChanger phaseChanger = null;
    public RawImage solidImage, liquidImage, gasImage;
    public Texture2D solidActiveTexture,
        solidInactiveTexture,
        liquidActiveTexture,
        liquidInactiveTexture,
        gasActiveTexture,
        gasInactiveTexture;
    public Phase phase;

	// Use this for initialization
	void Start () {
	    solidImage.texture  = phase == Phase.Solid  ? solidActiveTexture  : solidInactiveTexture;
        liquidImage.texture = phase == Phase.Liquid ? liquidActiveTexture : liquidInactiveTexture;
        gasImage.texture    = phase == Phase.Gas    ? gasActiveTexture    : gasInactiveTexture;
    }
	
	// Update is called once per frame
	void Update () {
	    bool buttonDown = false;
	    Phase newPhase = 0;
	    if (Input.GetButton("MakeSolid")) {
	        newPhase = Phase.Solid;
	        buttonDown = true;
	    } else if (Input.GetButton("MakeLiquid")) {
	        newPhase = Phase.Liquid;
	        buttonDown = true;
	    } else if (Input.GetButton("MakeGas")) {
	        newPhase = Phase.Gas;
	        buttonDown = true;
	    }

	    if (buttonDown && newPhase != phase) {
	        switch (phase) {
	            case Phase.Solid:
	                solidImage.texture = solidInactiveTexture;
	                break;
	            case Phase.Liquid:
	                liquidImage.texture = liquidInactiveTexture;
	                break;
	            case Phase.Gas:
	                gasImage.texture = gasInactiveTexture;
	                break;
	        }

            switch (newPhase) {
                case Phase.Solid:
                    solidImage.texture = solidActiveTexture;
                    break;
                case Phase.Liquid:
                    liquidImage.texture = liquidActiveTexture;
                    break;
                case Phase.Gas:
                    gasImage.texture = gasActiveTexture;
                    break;
            }

            if (phaseChanger != null)
                phaseChanger.SetPhase(newPhase);

            phase = newPhase;
	    }
	}
}
