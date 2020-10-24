Shader "Custom/Dev/ImageBounds"
{
    Properties
    {
        [Toggle(DEBUG_IS_MIN)] _IsAlphaMasked("Use Min", Float) = 1
    }

    // Helper functions
    CGINCLUDE
    // For a given object-space point, get it's 2d screen uv coordinate
    inline float2 ObjToScreenUV(in float3 obj_pos)
    {
        // 4D Homogeneous clip space coordinate of world pos
        float4 clip_pos4d = UnityObjectToClipPos(obj_pos);
        // 2D clip space (screen edges range from [-1 to 1])
        float2 clip_pos2d = clip_pos4d.xy / clip_pos4d.w;
        // Shift so range [-1, 1] becomes [0, 1], accounting for UV vertical flip as necessary
#if UNITY_UV_STARTS_AT_TOP
        return float2(1 + clip_pos2d.x, 1 - clip_pos2d.y) / 2;
#else
        return (1 + clip_pos2d) / 2;
#endif
    }

    // Get the 2d min/max UV coordinates that a quad takes up on screen, where in
    // object space a quad is 4 vertices centered at 0,0 and width x height = 1x1
    inline float4 GetQuadUVBounds()
    {
        // 1. Get clamped relative screen position of 4 corners of image quad
        float2 bl = ObjToScreenUV(float3(-0.5, -0.5, 0));
        float2 tl = ObjToScreenUV(float3(-0.5, 0.5, 0));
        float2 br = ObjToScreenUV(float3(0.5, -0.5, 0));
        float2 tr = ObjToScreenUV(float3(0.5, 0.5, 0));

        // 2. Get min/max of x and y
        float min_x = min(min(bl.x, tl.x), min(br.x, tr.x));
        float max_x = max(max(bl.x, tl.x), max(br.x, tr.x));
        float min_y = min(min(bl.y, tl.y), min(br.y, tr.y));
        float max_y = max(max(bl.y, tl.y), max(br.y, tr.y));

        return float4(min_x, min_y, max_x, max_y);
    }
    ENDCG

    // SubShaders
    SubShader
    {
        // No culling or depth
        Cull Off 
        ZWrite Off 
        ZTest Always

        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "DisableBatching" = "True"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile __ DEBUG_IS_MIN

            float4 vert(float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }

            half4 frag () : COLOR
            {
                float4 bounds = GetQuadUVBounds();
#if DEBUG_IS_MIN
                return float4(bounds.xy, 0, 1);
#else
                return float4(bounds.zw, 0, 1);
#endif
            }
            ENDCG
        }
    }
}