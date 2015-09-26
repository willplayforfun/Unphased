using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum Phase {
    Solid,
    Liquid,
    Gas
}

public class PhaseManager : MonoBehaviour {
    //public PhaseChanger phaseChanger = null;
    public RawImage solidImage, liquidImage, gasImage;
    public Texture2D solidActiveTexture,
        solidInactiveTexture,
        liquidActiveTexture,
        liquidInactiveTexture,
        gasActiveTexture,
        gasInactiveTexture;
    private Phase _phase;
    public Phase phase
    {
        get
        {
            return _phase;
        }
        set
        {
            switch (_phase)
            {
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

            switch (value)
            {
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

            _phase = value;
        }
    }

	// Use this for initialization
	void Start () {
	    solidImage.texture  = phase == Phase.Solid  ? solidActiveTexture  : solidInactiveTexture;
        liquidImage.texture = phase == Phase.Liquid ? liquidActiveTexture : liquidInactiveTexture;
        gasImage.texture    = phase == Phase.Gas    ? gasActiveTexture    : gasInactiveTexture;
    }
}
