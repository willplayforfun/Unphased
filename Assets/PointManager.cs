using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointManager : MonoBehaviour {
	
	public float gravityConstant = 1;
	public float attractionConstant = 1;
	public float repulsionConstant = 1;

	private List<Transform> points = new List<Transform>();

	void FixedUpdate(){
		// loop through points
		foreach (Transform point in points) {
			Rigidbody2D pointComponent = GetComponent<Rigidbody2D>();

			// Add Graivty Force to point
			pointComponent.AddForce (gravityConstant * Vector2.down);

			// remove point from points to not apply focres to it
			points.Remove(point);

			// loop through other points
			foreach (Transform otherPoint in points) {
	
				// Add attraction Force to point
				Vector2 appliedForce = -attractionConstant* (pointComponent.velocity / (point.position - otherPoint.position).magnitude);
				pointComponent.AddForce (appliedForce);

				// Add repulsive Force to otherPoint
				appliedForce = repulsionConstant * (pointComponent.velocity / (point.position - otherPoint.position).magnitude);
				pointComponent.AddForce (appliedForce);
				pointComponent.AddForce (repulsionConstant * Vector2.right);

				points.Add(point);
			}
		}
	}
}
