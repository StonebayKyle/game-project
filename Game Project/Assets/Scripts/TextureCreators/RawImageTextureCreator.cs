using UnityEngine;
using UnityEngine.UI;

public class RawImageTextureCreator : TextureCreator
{
    public override void AttachTextureToComponents()
    {
        GetComponent<RawImage>().material.mainTexture = texture;
    }
}
