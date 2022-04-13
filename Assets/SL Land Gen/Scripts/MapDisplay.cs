using UnityEngine;

// Mainly inspired from (Sebastian Lague, 2016): https://www.youtube.com/watch?v=MRNFcywkUSA
public class MapDisplay : MonoBehaviour
{
    [Header("Texture Rendering")]
    public Renderer textureRenderer = null;

    [Header("Mesh Rendering")]
    public MeshFilter meshFilter = null;
    public MeshRenderer meshRenderer = null;

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
