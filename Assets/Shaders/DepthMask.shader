Shader "Custom/DepthMask"
{
    SubShader
    {
        // Make sure it renders before transparent water
        Tags { "Queue"="Geometry-10" }

        Pass
        {
            ZWrite On      // Write to depth buffer
            ColorMask 0    // Don’t draw any color, just depth
        }
    }
}
