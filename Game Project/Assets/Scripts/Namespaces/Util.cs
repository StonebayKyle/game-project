using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    /// <summary> Adds Gradient lerping</summary>
    ///
    /// <authors>RkSanders, modified by StonebayKyle</author>
    public static class Gradient
    {
        public static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return Lerp(a, b, t, false, false);
        }

        public static UnityEngine.Gradient LerpNoAlpha(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return Lerp(a, b, t, true, false);
        }

        public static UnityEngine.Gradient LerpNoColor(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return Lerp(a, b, t, false, true);
        }

        static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t, bool noAlpha, bool noColor)
        {
            //lists of each of the unique key times
            var colorKeysTimes = new List<float>();
            var alphaKeysTimes = new List<float>();

            if (!noColor)
            {
                for (int i = 0; i < a.colorKeys.Length; i++)
                {
                    float k = a.colorKeys[i].time;
                    if (!colorKeysTimes.Contains(k))
                        colorKeysTimes.Add(k);
                }

                for (int i = 0; i < b.colorKeys.Length; i++)
                {
                    float k = b.colorKeys[i].time;
                    if (!colorKeysTimes.Contains(k))
                        colorKeysTimes.Add(k);
                }
            }

            if (!noAlpha)
            {
                for (int i = 0; i < a.alphaKeys.Length; i++)
                {
                    float k = a.alphaKeys[i].time;
                    if (!alphaKeysTimes.Contains(k))
                        alphaKeysTimes.Add(k);
                }

                for (int i = 0; i < b.alphaKeys.Length; i++)
                {
                    float k = b.alphaKeys[i].time;
                    if (!alphaKeysTimes.Contains(k))
                        alphaKeysTimes.Add(k);
                }
            }

            GradientColorKey[] clrs = new GradientColorKey[colorKeysTimes.Count];
            GradientAlphaKey[] alphas = new GradientAlphaKey[alphaKeysTimes.Count];

            //Pick colors of color gradients at key times and lerp them
            for (int i = 0; i < colorKeysTimes.Count; i++)
            {
                float key = colorKeysTimes[i];
                var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
                clrs[i] = new GradientColorKey(clr, key);
            }

            //Pick colors of alpha gradients at key times and lerp them
            for (int i = 0; i < alphaKeysTimes.Count; i++)
            {
                float key = alphaKeysTimes[i];
                var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
                alphas[i] = new GradientAlphaKey(clr.a, key);
            }

            var g = new UnityEngine.Gradient();
            g.SetKeys(clrs, alphas);

            return g;
        }
    }
}