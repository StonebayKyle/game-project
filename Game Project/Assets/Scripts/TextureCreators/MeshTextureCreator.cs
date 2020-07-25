using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshTextureCreator : TextureCreator
{
    public override void AttachTextureToComponents()
    {
        GetComponent<MeshRenderer>().material.mainTexture = texture;
    }
}
