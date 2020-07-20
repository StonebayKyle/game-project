using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{

    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;

    [Header("Terrain")]
    [ReadOnly]
    [SerializeField]
    private float xSize;
    [ReadOnly]
    [SerializeField]
    private float zSize;

    public int xVerticesCount = 20;
    public int zVerticesCount = 20;

    public float spaceBetweenVertices = 1f;

    [Header("Scrolling")]
    public bool scroll = false;
    [SerializeField]
    private float scrollWaitSeconds = 0.25f;
    [SerializeField]
    private float xMove = .1f;
    [SerializeField]
    private float zMove = 0f;
    private bool scrollRunning = false;

    [Header("Noise")]
    public float yMultiplier = 1f;

    public float posScale = .3f;

    public float xOffset = 0f;
    public float zOffset = 0f;

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
        if (scroll && scrollRunning == false)
        {
            StartCoroutine(Scroll());
        }
    }

    private IEnumerator Scroll()
    {
        scrollRunning = true;

        xOffset += xMove;
        zOffset += zMove;

        DoTerrainGeneration();

        yield return new WaitForSeconds(scrollWaitSeconds);

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
        vertices = new Vector3[(xVerticesCount + 1) * (zVerticesCount + 1)];

        for (int i = 0, z = 0; z <= zVerticesCount; z++)
        {
            for (int x = 0; x <= xVerticesCount; x++, i++)
            {
                float y = Mathf.PerlinNoise(x * posScale + xOffset, z * posScale + zOffset) * yMultiplier;
                vertices[i] = new Vector3(x * spaceBetweenVertices, y, z * spaceBetweenVertices);
            }
        }
    }

    public void CreateTriangles()
    {
        triangles = new int[xVerticesCount * zVerticesCount * 6];
        int vertex = 0, triangle = 0;
        for (int z = 0; z < zVerticesCount; z++, vertex++)
        {
            // make two sets of mesh triangles each iteration
            for (int x = 0; x < xVerticesCount; x++, vertex++, triangle += 6)
            {
                triangles[triangle] = vertex;
                triangles[triangle + 1] = vertex + xVerticesCount + 1;
                triangles[triangle + 2] = vertex + 1;
                triangles[triangle + 3] = vertex + 1;
                triangles[triangle + 4] = vertex + xVerticesCount + 1;
                triangles[triangle + 5] = vertex + xVerticesCount + 2;
            }
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        mesh.RecalculateBounds();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }






    private void OnDrawGizmos()
    {
        if (vertices != null && vertices.Length > 0)
        {
            DrawVertices();
        }
    }
    private void DrawVertices()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }

    //----- INSPECTOR -----//
    public void EditorUpdate()
    {
        DoTerrainGeneration();
        UpdateMesh();

    }

    public void UpdateReadOnlyValues()
    {
        xSize = xVerticesCount * spaceBetweenVertices;
        zSize = zVerticesCount * spaceBetweenVertices;
    }

    public void RandomizeOffsets()
    {
        float range = 1000000;
        xOffset = Random.Range(-range, range);
        zOffset = Random.Range(-range, range);
    }

}
