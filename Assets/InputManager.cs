using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    public enum Mode
    {
        Gas,
        Liquid,
        Solid
    }
    public Mode currentMode;

    public Transform camera;

	// Use this for initialization
	void Start () {
        pm = GetComponent<PointManager>();
        SetMode(Mode.Liquid);
	}

    PointManager pm;

    public float SolidAttraction;
    public float FluidAttraction;

    public float GasGravity;
    public float LiquidGravity;
    public float SolidGravity;

    public float RepulsionConstant;

    public float rampTime;

    public void SetMode(Mode m)
    {
        currentMode = m;
        switch(m)
        {
            case Mode.Gas:
                Debug.Log("Gas!");
                StartCoroutine(RampAttraction(rampTime,FluidAttraction));
                pm.attractionConstant = FluidAttraction;
                pm.gravityConstant = GasGravity;
                pm.repulsionConstant = RepulsionConstant;
                break;
            case Mode.Liquid:
                Debug.Log("Liquid!");
                StartCoroutine(RampAttraction(rampTime, FluidAttraction));
                pm.gravityConstant = LiquidGravity;
                pm.repulsionConstant = RepulsionConstant;
                break;
            case Mode.Solid:
                Debug.Log("Solid!");
                StartCoroutine(RampAttraction(rampTime, SolidAttraction));
                pm.gravityConstant = SolidGravity;
                pm.repulsionConstant = RepulsionConstant;
                break;
        }
    }

    IEnumerator RampAttraction(float length, float target)
    {
        float start = pm.attractionConstant;
        float timer = 0;
        while(timer < length)
        {
            pm.attractionConstant = Mathf.Lerp(start, target, timer / length);
            timer += Time.deltaTime;
            yield return null;
        }

        pm.attractionConstant = target;
    }

	// Update is called once per frame
	void Update () {

        camera.position = new Vector3(pm.centerOfMass.x,pm.centerOfMass.y, camera.position.z);

        if (Input.GetKey(KeyCode.D))
        {
            foreach (Transform t in pm.points)
            {
                t.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 5 + Vector2.up * 2);
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            foreach (Transform t in pm.points)
            {
                t.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 5 + Vector2.up * 2);
            }
        }
        //gas
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetMode(Mode.Gas);
        }
        //liquid
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetMode(Mode.Liquid);
        }
        //solid
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetMode(Mode.Solid);
        }
	}
}
