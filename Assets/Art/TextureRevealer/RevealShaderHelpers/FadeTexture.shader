Shader "TNTC/FadeTexture" {
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FadeAmount ("Fade Amount", Range(0,1)) = 0.05
        _TargetAlpha ("Target Alpha", Range(0,1)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Cull Off ZWrite Off ZTest Always
        Blend One Zero // Force overwrite of source, not additive blending

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _FadeAmount;
            float _TargetAlpha;

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a > _TargetAlpha + 0.001)
                    col.a = lerp(col.a, _TargetAlpha, _FadeAmount);
                return col;
            }
            ENDCG
        }
    }
}