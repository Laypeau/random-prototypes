using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Gen_Tile : MonoBehaviour
{
	Mesh mesh;
	Vector3[] vertices; //Local space
	int[] triangles;
	Color[] colours;

	[Header("Mesh Settings")]
	public Vector2Int segments = Vector2Int.one * 10;
	public Vector2 length = Vector2.one * 10;

	[Header("Noise")]
	[Range(0f, 2f)]public float noiseScale = 0.3f;
	[Range(0f, 10f)] public float noiseHeight = 2f;
	public Vector2 noiseOffset;

	[Header("Vertex Colours")]
	public Gradient gradient = new Gradient();

	void Awake()
	{
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
	}

	public void Remesh()
	{
		GenerateMesh();
		UpdateMesh();
	}

	void GenerateMesh()
	{
		//Loop through all points and create vertices
		vertices = new Vector3[(segments.x + 1) * (segments.y + 1)]; //1 unit line has 2 verts, 2 unit long line has 3 vertices, et cetera
		colours = new Color[(segments.x + 1) * (segments.y + 1)];
		for(int i = 0, z = 0; z <= segments.y; z++)
		{
			for(int x = 0; x <= segments.x; x++)
			{				
				float y = Mathf.PerlinNoise(noiseScale * (69 + transform.position.x + (x * length.x)/segments.x), noiseScale * (420 + transform.position.z + (z * length.y)/segments.y)) * noiseHeight; //apparently unity's perlin noise loops
				vertices[i] = new Vector3(x * length.x/segments.x, y, z * length.y/segments.y);

				colours[i] = gradient.Evaluate(Mathf.PerlinNoise(noiseScale * (69 + transform.position.x + (x * length.x) / segments.x), noiseScale * (420 + transform.position.z + (z * length.y) / segments.y)));

				i++; 
			}
		}

		//Loops through each point (Except the last ones) and creates a square
		triangles = new int[segments.x * segments.y * 6]; //number of quads * number of vertices per quad
		int vert = 0;
		int tris = 0;
		for(int z = 0; z < segments.y; z++)
		{
			for (int x = 0; x < segments.x; x++)
			{
				triangles[tris + 0] = vert + 0;
				triangles[tris + 1] = vert + segments.x + 1;
				triangles[tris + 2] = vert + 1;

				triangles[tris + 3] = vert + 1;
				triangles[tris + 4] = vert + segments.x + 1;
				triangles[tris + 5] = vert + segments.x + 2;

				vert++;
				tris += 6;
			}
			vert++;
		}
	}

	void UpdateMesh()
	{
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.colors = colours;
		mesh.RecalculateNormals();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine(transform.position, transform.position + (Vector3.forward * length.y));
		Gizmos.DrawLine(transform.position + (Vector3.forward * length.y), transform.position + (Vector3.right *length.y) + (Vector3.forward * length.y));
		Gizmos.DrawLine(transform.position + (Vector3.right *length.y) + (Vector3.forward * length.y), transform.position + Vector3.right *length.y);
		Gizmos.DrawLine(transform.position + Vector3.right *length.y, transform.position);

		// if(vertices != null)
		// {
		// 	for(int i = 0; i < vertices.Length; i++)
		// 	{
		// 		Gizmos.color = Color.Lerp(Color.red, Color.green, (float)i / vertices.Length);
		// 		Gizmos.DrawSphere(transform.position + vertices[i], 0.05f);
		// 	}
		// }
	}
}
