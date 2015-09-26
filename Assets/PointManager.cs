using UnityEngine;
using System.Collections;

public class PointManager : MonoBehaviour {

	private List<Transform> points = new List<Transoform>();
	public const Vector2 gravityConstant;
	public const Vector2 attractionConstant;
	public const Vector2 repulsionConstant;

	void FixedUpdate(){
		// loop through points
		foreach (Transform point in points) {
			// loop through other points
			foreach (Transform otherPoint in points.Remove(point)) {
				// Add Graivty Force to point
				GetComponent<Rigidbody2D>().AddForce (/*Vector*/);
	
				// Add attraction Force to point
				GetComponent<Rigidbody2D>().AddForce (/*Vector*/);

				// Add repulsive Force to otherPoint
				GetComponent<Rigidbody2D>().AddForce (/*Vector*/);
			}
		}
	}
}
