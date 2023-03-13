Shader "A/glow"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white"{}
        _MainColor("MainColor", Color) = (1, 1, 1, 1)
        _Emiss("Emiss", Float) = 1.0
        _RimPower("RimPower", Float) = 1.0
    }
        SubShader
        {
            Tags{"Queue" = "Transparent"}
            Pass
            {
                ZWrite off
                Blend SrcAlpha One
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex: POSITION;
                    float2 uv: TEXCOORD0;
                    float3 normal: NORMAL;
                };

                struct v2f
                {
                    float4 pos: SV_POSITION;
                    float2 uv: TEXCOORD0;
                    float3 normal_world: TEXCOORD1;
                    float3 view_dir: TEXCOORD2;
                };

                sampler2D _MainTex;
                float4 _MainColor;
                float _Emiss;
                float _RimPower;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.normal_world = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                    float3 pos_world = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.view_dir = normalize(_WorldSpaceCameraPos.xyz - pos_world);
                    o.uv = v.uv;
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float3 normal = normalize(i.normal_world);
                    float3 view_dir = normalize(i.view_dir);
                    float NdotV = saturate(dot(normal, view_dir));
                    float3 col = _MainColor.xyz * _Emiss;
                    float fresnel = pow(1.0 - NdotV, _RimPower);
                    float alpha = saturate(fresnel * _Emiss);
                    return float4(col, alpha);
                }

                ENDCG
            }
        }
}