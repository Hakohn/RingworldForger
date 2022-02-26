using System;
using UnityEngine;

[Serializable]
public class RingLayer
{
    public int widthSubdivisions = 2;
    public int lengthSubdivisions = 100;
    public float textureScale = 0.1f;
    [HideInInspector]
    public Mesh mesh = null;
    [HideInInspector]
    public MeshFilter filter = null;
    [HideInInspector]
    public MeshRenderer renderer = null;
    [HideInInspector]
    public Vector3[] verts = null;
    [HideInInspector]
    public int[] tris = null;
    [HideInInspector]
    public Vector2[] uv = null;

    public void RecalculateUVs()
    {
        uv = new Vector2[widthSubdivisions * lengthSubdivisions];

        float lenToWidthScale = lengthSubdivisions / widthSubdivisions;
        float fTextureS = widthSubdivisions * textureScale;
        float fTextureT = lengthSubdivisions * textureScale * lenToWidthScale;
        for (int z = 0; z < lengthSubdivisions; z++)
        {
            for (int x = 0; x < widthSubdivisions; x++)
            {
                int i = z * widthSubdivisions + x;
                float fScaleC = (float)x / (widthSubdivisions - 1);
                float fScaleR = (float)z / (lengthSubdivisions - 1);
                uv[i] = new Vector2(fTextureS * fScaleC, fTextureT * fScaleR);
            }
        }
    }
}
