﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointManager : MonoBehaviour {
	
	public float gravityConstant = 1;
	public float attractionConstant = 1;
	public float repulsionConstant = 1;

    public float repulsiveLimit;

	public float distanceFromCenter;

	public List<Transform> points = new List<Transform>();

    public GameObject pointPrefab;

	public Vector2 centerOfMass = Vector2.zero;

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
		// loop through points
		foreach (Transform point in points) {
			Rigidbody2D pointComponent = point.GetComponent<Rigidbody2D>();

			// Center of Mass
			centerOfMass += (Vector2)point.position;

			// Add Graivty Force to point
			pointComponent.AddForce (gravityConstant * Vector2.down);

			// Instantiate New PointManager when out of bound
			if(((Vector2)point.position - centerOfMass).magnitude > distanceFromCenter) {
				points.Remove(point);
			}

			// loop through other points
			foreach (Transform otherPoint in points) {

                if (!point.Equals(otherPoint))
                {
                    Vector2 distance = (point.position - otherPoint.position);

                    // Add attraction Force to point
                    pointComponent.AddForce(-attractionConstant * Vector2.ClampMagnitude((distance.normalized / Mathf.Pow(distance.magnitude, 2)), repulsiveLimit));

                    // Add repulsive Force to otherPoint
                    pointComponent.AddForce(repulsionConstant * Vector2.ClampMagnitude((distance.normalized / Mathf.Pow(distance.magnitude, 3)), repulsiveLimit));
                }
			}
		}

		centerOfMass /= points.Count;
		Debug.Log (centerOfMass.magnitude);
	}
}