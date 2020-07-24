using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    public float stepSize;

    public int xSize, zSize;

    public Vector3 offset;
    public Vector3 rotation;

    [Range(0f, 1f)]
    public float strength = 1f;

    public bool damping;

    public float frequency = 1f;

    [Range(1, 8)]
    public int octaves = 1;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    [Range(1, 3)]
    public int dimensions = 3;

    public NoiseMethodType type;

    public Gradient coloring;

    public bool coloringForStrength;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Color[] colors;

    private float currentStepSize;
    private int currentRows, currentCols;

    private void OnEnable()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Surface Mesh";
            GetComponent<MeshFilter>().mesh = mesh;
        }
        Refresh();
    }

    public void Refresh()
    {
        if (stepSize != currentStepSize || xSize != currentRows || zSize != currentCols)
        {
            CreateGrid();
        }
        Quaternion q = Quaternion.Euler(rotation);

        float xLength = xSize * stepSize;
        float zLength = zSize * stepSize;

        Vector3 point00 = q * new Vector3(-xLength, -zLength) + offset; // left bottom
        Vector3 point10 = q * new Vector3(xLength, -zLength) + offset; // right bottom
        Vector3 point01 = q * new Vector3(-xLength, zLength) + offset; // left top
        Vector3 point11 = q * new Vector3(xLength, zLength) + offset; // right top

        NoiseMethod method = Noise.methods[(int)type][dimensions - 1];

        float amplitude = damping ? strength / frequency : strength;

        float inverseXSize = 1f / xSize;
        float inverseZSize = 1f / zSize;

        for (int v = 0, y = 0; y <= zSize; y++)
        {
            Vector3 point0 = Vector3.Lerp(point00, point01, y * inverseZSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, y * inverseZSize);
            for (int x = 0; x <= xSize; x++, v++)
            {
                Vector3 point = Vector3.Lerp(point0, point1, x * inverseXSize);
                float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
                sample = type == NoiseMethodType.Value ? (sample - 0.5f) : (sample * 0.5f);
                if (coloringForStrength)
                {
                    colors[v] = coloring.Evaluate(sample + 0.5f);
                    sample *= amplitude;
                }
                else
                {
                    sample *= amplitude;
                    colors[v] = coloring.Evaluate(sample + 0.5f);
                }
                vertices[v].y = sample;
            }
        }
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    private void CreateGrid()
    {
        currentStepSize = stepSize;
        currentRows = xSize;
        currentCols = zSize;

        mesh.Clear();

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        colors = new Color[vertices.Length];
        normals = new Vector3[vertices.Length];
        Vector2[] uv = new Vector2[vertices.Length];

        for (int v = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, v++)
            {
                vertices[v] = new Vector3(x * stepSize - 0.5f, 0f, z * stepSize - 0.5f);
                colors[v] = Color.black;
                normals[v] = Vector3.up;
                uv[v] = new Vector2(x * stepSize, z * stepSize);
            }
        }
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.normals = normals;
        mesh.uv = uv;

        int[] triangles = new int[xSize * zSize * 6];
        for (int t = 0, v = 0, z = 0; z < zSize; z++, v++)
        {
            for (int x = 0; x < xSize; x++, v++, t += 6)
            {
                triangles[t] = v;
                triangles[t + 1] = v + xSize + 1;
                triangles[t + 2] = v + 1;
                triangles[t + 3] = v + 1;
                triangles[t + 4] = v + xSize + 1;
                triangles[t + 5] = v + xSize + 2;
            }
        }
        mesh.triangles = triangles;
    }

    public void RandomizeOffsets()
    {
        int range = 120000; // beyond this range, Noise seems to break down and become jagged.
        offset.x = Random.Range(-range, range);
        offset.y = Random.Range(-range, range);
    }
}