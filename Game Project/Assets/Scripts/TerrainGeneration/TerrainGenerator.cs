using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{

    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    [SerializeField]
    private float generationDelaySeconds = .1f;
    [SerializeField]
    private float noiseScale = .3f;
    [SerializeField]
    private float xOffset;
    [SerializeField]
    private float zOffset;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        StartCoroutine(CreateShape());
    }

    private void Update()
    {
        UpdateMesh();
    }

    public void EditorUpdate()
    {
        StopAllCoroutines();
        StartCoroutine(CreateShape());
    }

    private IEnumerator CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                float y = Mathf.PerlinNoise(x * noiseScale + xOffset, z * noiseScale + zOffset) * 2f;
                vertices[i] = new Vector3(x, y, z);
            }
        }
         
        triangles = new int[xSize * zSize * 6];
        int vertex = 0, triangle = 0;
        for (int z = 0; z < zSize; z++, vertex++)
        {
            // make two sets of mesh triangles each iteration
            for (int x = 0; x < xSize; x++, vertex++, triangle += 6)
            {
                triangles[triangle] = vertex;
                triangles[triangle + 1] = vertex + xSize + 1;
                triangles[triangle + 2] = vertex + 1;
                triangles[triangle + 3] = vertex + 1;
                triangles[triangle + 4] = vertex + xSize + 1;
                triangles[triangle + 5] = vertex + xSize + 2;

                yield return new WaitForSeconds(generationDelaySeconds);
            }
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
}
