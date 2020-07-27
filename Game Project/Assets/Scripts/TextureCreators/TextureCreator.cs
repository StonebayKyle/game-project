using System;
using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class TextureCreator : MonoBehaviour
{

	[Header("Noise")]

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

	[Header("Scroll")]
	[SerializeField]
	private bool scroll = false;
	[SerializeField]
	private Vector3 scrollOffset;
	[SerializeField]
	private Vector3 scrollRotation;

	private static int OFFSET_RANGE = 25000; // beyond this range, noise gets jagged
	private static int ROTATION_RANGE = 360; // 360 degrees

	[Header("Coloring")]

	public Gradient gradient;

	private GradientGenerator gradientGenerator;

	public bool lerpGradient = false;
	[Range(0, 1)]
	public float gradientStepSize = .01f;
	[ReadOnly]
	[SerializeField]
	private float gradientLerpTime = 0f;

	private Gradient nextGradient;
	private Gradient startGradient;

	[System.NonSerialized]
	public Texture2D texture;
	
	private void OnEnable () 
	{
		if (texture == null)
		{
			texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
			texture.name = "Procedural Texture";
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Trilinear;
			texture.anisoLevel = 9;

			AttachTextureToComponents();
		}

		if (gameObject.GetComponent(typeof(GradientGenerator)))
        {
			gradientGenerator = GetComponent<GradientGenerator>();
			MakeGradient();
        }

		Refresh();
	}

	/// <summary>
	/// Attach texture to whichever component needs to render it.
	/// For example, GetComponent<T>().material.mainTexture = texture;
	/// Remember to require the component at class level:
	/// [RequireComponent(typeof(T))]
	/// </summary>
	public abstract void AttachTextureToComponents();


	public void Refresh()
    {
		FillTexture();
	}

	private void MakeGradient()
    {
		gradient = gradientGenerator.RandomGradient();
    }

    public void Update()
    {
		if (lerpGradient)
        {
			LerpGradient();
        }

		if (scroll)
        {
			Scroll();
        }

		if (scroll || lerpGradient) // refresh any visual update
        {
			Refresh();
		}

    }

	private void LerpGradient()
    {
		if (startGradient == null)
        {
			startGradient = gradient;
        }

		if (nextGradient == null)
		{
			nextGradient = gradientGenerator.RandomGradient();
		}

		gradient = Util.Gradient.Lerp(startGradient, nextGradient, gradientLerpTime);

		gradientLerpTime += gradientStepSize * Time.deltaTime;

		// reset lerp and start towards new gradient
		if (gradientLerpTime >= 1f)
        {
			gradientLerpTime = 0f;

			startGradient = nextGradient;
			nextGradient = gradientGenerator.RandomGradient();
        }
    }

	private void Scroll()
    {

		offset += scrollOffset * Time.deltaTime;
		rotation += scrollRotation * Time.deltaTime;

		WrapNoise();
    }

    private void FillTexture ()
	{
		if (texture.width != resolution)
		{
			texture.Resize(resolution, resolution);
		}

		Quaternion q = Quaternion.Euler(rotation);

		Vector3 point00 = q * new Vector3(-0.5f,-0.5f) + offset;
		Vector3 point10 = q * new Vector3( 0.5f,-0.5f) + offset;
		Vector3 point01 = q * new Vector3(-0.5f, 0.5f) + offset;
		Vector3 point11 = q * new Vector3( 0.5f, 0.5f) + offset;

		NoiseMethod method = Noise.methods[(int)type][dimensions - 1];
		float stepSize = 1f / resolution;
		for (int y = 0; y < resolution; y++)
		{
			Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
			Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
			for (int x = 0; x < resolution; x++)
			{
				Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
				float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
				if (type != NoiseMethodType.Value)
				{
					sample = sample * 0.5f + 0.5f;
				}
				texture.SetPixel(x, y, gradient.Evaluate(sample));
			}
		}
		texture.Apply();
	}


	private void WrapNoise()
    {
		// loops through xyz in the offset and rotation vector3's
		for (int i = 0; i < 3; i++)
        {
			if (offset[i] > OFFSET_RANGE)
            {
				offset[i] = -OFFSET_RANGE;
            } else if (offset[i] < -OFFSET_RANGE)
            {
				offset[i] = OFFSET_RANGE;
            }

			if (Math.Abs(rotation[i]) > ROTATION_RANGE)
            {
				rotation[i] = 0;
            }
        }
    }


	// ---INSPECTOR--- //

	public void RandomizeOffsets()
    {
		offset.x = Random.Range(-OFFSET_RANGE, OFFSET_RANGE);
		offset.y = Random.Range(-OFFSET_RANGE, OFFSET_RANGE);
		offset.z = Random.Range(-OFFSET_RANGE, OFFSET_RANGE);
	}

	public void RandomizeRotation()
    {
		rotation.x = Random.Range(0f, ROTATION_RANGE);
		rotation.y = Random.Range(0f, ROTATION_RANGE);
		rotation.z = Random.Range(0f, ROTATION_RANGE);
	}


}