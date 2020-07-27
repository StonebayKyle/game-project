using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientGenerator : MonoBehaviour
{
    [Header("Gradient Generator")]
    public GradientMode mode;

    [Header("Color")]

    public bool grayscale = false;

    [Range(2, 10)]
    public int minColorKeyAmt = 2;
    [Range(2, 10)]
    public int maxColorKeyAmt = 5;

    [Range(0, 1)]
    public float colorMaxTimeDeviation = .05f;

    [Header("Alpha")]

    public bool noTransparency = true;

    [Range(2, 10)]
    public int minAlphaKeyAmt = 2;
    [Range(2, 10)]
    public int maxAlphaKeyAmt = 2;

    [Range(0, 1)]
    public float alphaMaxTimeDeviation = 0f;

    public Gradient RandomGradient()
    {
        int colorKeyAmt = Random.Range(minColorKeyAmt, maxColorKeyAmt + 1);
        int alphaKeyAmt = Random.Range(minAlphaKeyAmt, maxAlphaKeyAmt + 1);

        return Gradient(colorKeyAmt, alphaKeyAmt);
    }

    private Gradient Gradient(int colorKeyAmt, int alphaKeyAmt)
    {
        GradientColorKey[] colorKeys = GenerateColorKeys(colorKeyAmt);
        GradientAlphaKey[] alphaKeys = GenerateAlphaKeys(alphaKeyAmt);

        Gradient g = new Gradient();

        g.SetKeys(colorKeys, alphaKeys);

        return g;
    }

    private GradientColorKey[] GenerateColorKeys(int colorKeyAmt)
    {
        GradientColorKey[] colorKeys;
        if (grayscale)
        {
            colorKeys = new GradientColorKey[2];

            colorKeys[0].color = Color.black;
            colorKeys[0].time = 0f;

            colorKeys[1].color = Color.white;
            colorKeys[1].time = 1f;
            return colorKeys;
        }

        colorKeys = new GradientColorKey[colorKeyAmt];

        float colorTimeSpacing = 1f / (colorKeyAmt - 1); // - 1 because first key is always placed at 0

        for (int i = 0; i < colorKeys.Length; i++)
        {
            Color color = new Color(Random.value, Random.value, Random.value);

            colorKeys[i].color = color;
            colorKeys[i].time = CalculateKeyTime(i, colorKeys.Length, colorTimeSpacing, colorMaxTimeDeviation);
        }

        return colorKeys;
    }

    private GradientAlphaKey[] GenerateAlphaKeys(int alphaKeyAmt)
    {
        GradientAlphaKey[] alphaKeys;
        if (noTransparency)
        {
            alphaKeys = new GradientAlphaKey[2];

            alphaKeys[0].alpha = 1f;
            alphaKeys[0].time = 0f;

            alphaKeys[1].alpha = 1f;
            alphaKeys[1].time = 1f;
            return alphaKeys;
        }

        alphaKeys = new GradientAlphaKey[alphaKeyAmt];

        float alphaTimeSpacing = 1f / (alphaKeyAmt - 1); // - 1 because first key is always placed at 0

        for (int i = 0; i < alphaKeys.Length; i++)
        {
            float alpha = Random.value;

            alphaKeys[i].alpha = alpha;
            alphaKeys[i].time = CalculateKeyTime(i, alphaKeys.Length, alphaTimeSpacing, alphaMaxTimeDeviation);
        }

        return alphaKeys;
    }

    private static float CalculateKeyTime(int index, int arrayLength, float spacing, float maxTimeDeviation)
    {
        if (index == 0) // first key
        {
            return 0f;
        }
        else if (index == arrayLength - 1) // last key
        {
            return 1f;
        }
        else // middle key(s)
        {
            return Mathf.Clamp01(index * spacing + Random.Range(-maxTimeDeviation, maxTimeDeviation));
        }
    }

}
