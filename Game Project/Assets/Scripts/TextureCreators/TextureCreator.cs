using UnityEngine;
using UnityEngine.UI;

public abstract class TextureCreator : MonoBehaviour {

	[Range(2, 512)]
	public int resolution = 256;

	public Vector3 offset;
	public Vector3 rotation;

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

	[System.NonSerialized]
	public Texture2D texture;
	
	private void OnEnable () {
		if (texture == null)
		{
			texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
			texture.name = "Procedural Texture";
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Trilinear;
			texture.anisoLevel = 9;

			AttachTextureToComponents();
		}

		FillTexture();
	}

	/// <summary>
	/// Attach texture to whichever component needs to render it.
	/// For example, GetComponent<T>().material.mainTexture = texture;
	/// Remember to require the component at class level:
	/// [RequireComponent(typeof(T))]
	/// </summary>
	public abstract void AttachTextureToComponents();

	private void Update () {
		if (transform.hasChanged) {
			transform.hasChanged = false;
			FillTexture();
		}
	}
	
	public void FillTexture () {
		if (texture.width != resolution) {
			texture.Resize(resolution, resolution);
		}

		Quaternion q = Quaternion.Euler(rotation);

		Vector3 point00 = q * new Vector3(-0.5f,-0.5f) + offset;
		Vector3 point10 = q * new Vector3( 0.5f,-0.5f) + offset;
		Vector3 point01 = q * new Vector3(-0.5f, 0.5f) + offset;
		Vector3 point11 = q * new Vector3( 0.5f, 0.5f) + offset;

		NoiseMethod method = Noise.methods[(int)type][dimensions - 1];
		float stepSize = 1f / resolution;
		for (int y = 0; y < resolution; y++) {
			Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
			Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
			for (int x = 0; x < resolution; x++) {
				Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
				float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
				if (type != NoiseMethodType.Value) {
					sample = sample * 0.5f + 0.5f;
				}
				texture.SetPixel(x, y, coloring.Evaluate(sample));
			}
		}
		texture.Apply();
	}
}