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

    public float spaceBetweenVertices = 1f;

    public float yMultiplier = 1f;

    public float posScale = .3f;

    public float xOffset = 0f;
    public float zOffset = 0f;

    public float waitSeconds = 0.25f;


    public bool scroll = false;
    private bool scrollRunning = false;



    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        DoTerrainGeneration();
    }

    private void Update()
    {
        UpdateMesh();
        if (shouldScroll && scrollRunning == false)
        {
            StartCoroutine(Scroll());
        }
    }

    private IEnumerator Scroll()
    {
        scrollRunning = true;
        DoTerrainGeneration();
        xOffset+= .1f;

        yield return new WaitForSeconds(waitSeconds);

        scrollRunning = false;
        yield return null;
    }


    private void DoTerrainGeneration()
    {
        CreateVertices();
        CreateTriangles();
    }

    private void CreateVertices()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                float y = Mathf.PerlinNoise(x * posScale + xOffset, z * posScale + zOffset) * yMultiplier;
                vertices[i] = new Vector3(x * spaceBetweenVertices, y, z * spaceBetweenVertices);
            }
        }
    }

    public void CreateTriangles()
    {
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


    private void DrawVertices()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
    private void OnDrawGizmos()
    {
        if (vertices != null && vertices.Length > 0)
        {
            DrawVertices();
        }
    }

    //----- INSPECTOR -----//
    public void EditorUpdate()
    {
        DoTerrainGeneration();
        UpdateMesh();
    }

    public void RandomizeOffsets()
    {
        float range = 100000;
        xOffset = Random.Range(-range, range);
        zOffset = Random.Range(-range, range);
    }

}
