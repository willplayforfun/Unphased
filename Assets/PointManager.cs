using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointManager : MonoBehaviour {
	
    [HideInInspector]
	public float gravityConstant = 1;
    [HideInInspector]
    public float attractionConstant = 1;
    [HideInInspector]
    public float repulsionConstant = 1;

    public float attractiveLimit;
    public float repulsiveLimit;

	public float distanceFromCenter;

	public List<Transform> points;

    public GameObject pointPrefab;

	public Vector2 centerOfMass = Vector2.zero;

    void Awake()
    {
        points = new List<Transform>();
    }

    void Start()
    {
        List<GameObject> objs = new List<GameObject>(GameObject.FindGameObjectsWithTag("Point"));
        foreach(GameObject obj in objs)
        {
            points.Add(obj.transform);
        }
        /*
        for (int i = 0; i < 10; i++)
        {
            GameObject newPoint = Instantiate(pointPrefab);
            newPoint.transform.position = this.transform.position + Vector3.up * Random.Range(-1.5f, 1.5f) + Vector3.right * Random.Range(-1.5f, 1.5f) ;
            points.Add(newPoint.transform);
        }
        */
    }

	void FixedUpdate(){
        Vector3 tmpCenterMass = Vector3.zero;

        List<Transform> markedForRemoval = new List<Transform>();

		// loop through points
		foreach (Transform point in points) {
			Rigidbody2D pointComponent = point.GetComponent<Rigidbody2D>();

			// Center of Mass
            tmpCenterMass += point.position;

			// Add Graivty Force to point
			pointComponent.AddForce (gravityConstant * Vector2.down);

			// Instantiate New PointManager when out of bound
			if(((Vector2)point.position - centerOfMass).magnitude > distanceFromCenter) {
                markedForRemoval.Add(point);
			}

			// loop through other points
			foreach (Transform otherPoint in points) {

                if (!point.Equals(otherPoint))
                {
                    Vector2 distance = (point.position - otherPoint.position);

                    // Add attraction Force to point
                    pointComponent.AddForce(-attractionConstant * Vector2.ClampMagnitude((distance.normalized / Mathf.Pow(distance.magnitude, 2)), attractiveLimit));

                    // Add repulsive Force to otherPoint
                    pointComponent.AddForce(repulsionConstant * Vector2.ClampMagnitude((distance.normalized / Mathf.Pow(distance.magnitude, 3)), repulsiveLimit));
                }
			}
		}

        tmpCenterMass /= points.Count;
        centerOfMass = tmpCenterMass;
		//Debug.Log (centerOfMass.magnitude);
        
        foreach(Transform t in markedForRemoval)
        {
            points.Remove(t);
            Destroy(t.gameObject);
        }

        transform.position = centerOfMass;
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceFromCenter);
    }
}
