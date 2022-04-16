// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Terrain"
{
    Properties
    {
    }
    SubShader
    {
        //Tags { "DisableBatching" = "true"}
        Tags { "RenderType" = "Opaque" }
        LOD 200

        // Inspired from (Sebastian Lague, 2016): https://www.youtube.com/watch?v=XdahmaohYvI
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        const static float epsilon = 1E-4;


        
        // Height
        float minHeight;
        float maxHeight;
        // Ring
        float3 ring_centre;
        float3 ring_right;
        float ring_radius;
        float ring_width;
        float4x4 localToWorldMatrix;
        // Biomes
        const static int maxBiomeCount = 10;
        int biomeCount;
        UNITY_DECLARE_TEX2DARRAY(biome_textures);
        float biome_textureScales[maxBiomeCount];
        float3 biome_tints[maxBiomeCount];
        float biome_tintStrengths[maxBiomeCount];
        float biome_startHeights[maxBiomeCount];
        float biome_blendStrengths[maxBiomeCount];

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        float InverseLerp(float a, float b, float value)
        {
            return saturate((value - a) / (b - a));
        }

        float3 triplanar(float3 pos, float scale, float3 blendAxes, int textureIndex)
        {
            float3 scaledPos = pos / scale;
            float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(biome_textures, float3(scaledPos.y, scaledPos.y, textureIndex)) * blendAxes.x;
            float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(biome_textures, float3(scaledPos.x, scaledPos.z, textureIndex)) * blendAxes.y;
            float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(biome_textures, float3(scaledPos.x, scaledPos.y, textureIndex)) * blendAxes.z;
            return xProjection + yProjection + zProjection;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Determine the height based on right position.
            float3 difference = ring_centre - IN.worldPos;
            float projectionLength = dot(ring_right, difference);
            float3 localRingCentre = ring_centre - ring_right * projectionLength;
            float3 dirToLocalCentre = normalize(localRingCentre - IN.worldPos);
            float3 lowestPoint = localRingCentre + -dirToLocalCentre * ring_radius;
            // Calculating the height and height percentage.
            float height = length(IN.worldPos - lowestPoint);
            float heightPercent = InverseLerp(minHeight, maxHeight, height);

            //float3 screenPos = UnityObjectToClipPos(IN.worldPos);
            float3 localPos = mul(unity_WorldToObject, IN.worldPos);
            float3 localNormal = mul(unity_WorldToObject, float4(IN.worldNormal, 0.0)).xyz;
            float3 localBlendAxes = abs(localNormal);

            // Calculating the right colour.
            for (int i = 0; i < biomeCount; i++)
            {
                float drawStrength = InverseLerp(-biome_blendStrengths[i] / 2 - epsilon, biome_blendStrengths[i] / 2, heightPercent - biome_startHeights[i]);

                // Calculating the color.
                float3 biomeTint = biome_tints[i] * biome_tintStrengths[i];
                float3 textureColour = triplanar(localPos, biome_textureScales[i], localBlendAxes, i) * (1 - biome_tintStrengths[i]);

                // Applying the colour.
                o.Albedo = o.Albedo * (1 - drawStrength) + (biomeTint + textureColour) * drawStrength;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
