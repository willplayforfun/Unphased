using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Outliner : MonoBehaviour
{
    private int outsideVertexCount;

    void Update()
    {
        JarvisWrap();
    }

    public MeshFilter mf;

    public void JarvisWrap()
    {


        //Start with point with smallest y-coordinate.
        PointManager pm = GetComponent<PointManager>();

        Debug.Log(pm.points.Count);


        Transform lowestPoint = pm.points[0];

        foreach(Transform point in pm.points)
        {
            if(point.position.y < lowestPoint.position.y)
            {
                lowestPoint = point;
            }
        }

        Debug.Log(lowestPoint.name);

        Transform currentPoint = lowestPoint;
        Vector3 lastEdge = Vector3.right;

        List<Vector3> outsidePositions = new List<Vector3>();
        outsideVertexCount = 0;

        int count = 0;
        do
        {
            Transform bestPoint = null;
            float bestAngle = Mathf.Infinity;

            //Compute angle between current point and all remaining points.
            //Pick smallest angle larger than current angle.
            foreach (Transform point in pm.points)
            {
                if (!point.Equals(currentPoint))
                {
                    float CCWangle = Vector3.Angle(lastEdge, (point.position - currentPoint.position));

                    Vector3 cross = Vector3.Cross(lastEdge, (point.position - currentPoint.position));
                    if(cross.z < 0)
                    {
                        CCWangle = 360 - CCWangle;
                    }

                    if (bestPoint == null || CCWangle < bestAngle || (CCWangle == bestAngle && (point.position - currentPoint.position).magnitude < (bestPoint.position - currentPoint.position).magnitude))
                    {
                        bestPoint = point;
                        bestAngle = CCWangle;
                    }
                }
            }

            Debug.Log(bestPoint.name + " " + bestAngle);
            Debug.DrawLine(currentPoint.position, bestPoint.position, Color.green);

            outsidePositions.Add(currentPoint.position);

            //Repeat
            lastEdge = (bestPoint.position - currentPoint.position);
            currentPoint = bestPoint;
            
            count++;
            outsideVertexCount++;

        } while (!currentPoint.Equals(lowestPoint) && count < pm.points.Count);

        outsidePositions.Add(currentPoint.position);


        LineRenderer lr = GetComponent<LineRenderer>();
        lr.SetVertexCount(outsidePositions.Count);
        for(int i = 0; i < outsidePositions.Count; i++)
        {
            lr.SetPosition(i, outsidePositions[i]);
        }

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[outsidePositions.Count];
        Vector2[] uv = new Vector2[outsidePositions.Count];
        Vector3[] normals = new Vector3[outsidePositions.Count];
        for (int i = 0; i < outsidePositions.Count; i++)
        {
            vertices[i] = outsidePositions[i] - transform.position;
            uv[i] = (Vector2)outsidePositions[i];
            normals[i] = Vector3.forward;
        }

        Vector2[] vertices2D = new Vector2[outsidePositions.Count];
        for (int i = 0; i < outsidePositions.Count; i++)
        {
            vertices2D[i] = (Vector2)outsidePositions[i];
        }
        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.normals = normals;
        //mesh.RecalculateNormals();
        //mesh.RecalculateBounds();

        mf.mesh = mesh;
    }
}

