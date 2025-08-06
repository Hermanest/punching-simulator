Shader "Custom/Mannequin Limb"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _BloodColor("Blood Color", Color) = (0.6, 0, 0, 1)
        _HitCount("Hit Count", Int) = 0
        _HitLifetime("Hit Lifetime", Float) = 10
        _HitDisposeTime("Hit Dispose Time", Float) = 2
    }

    SubShader
    {
        Tags
        {
            "LightMode"="ForwardBase"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            sampler2D _MainTex;
            float4 _BaseColor;
            float4 _BloodColor;

            #define MAX_HITS 20
            float4 _HitPositions[MAX_HITS];
            float _HitTimes[MAX_HITS];
            float _HitLifetime;
            float _HitDisposeTime;
            int _HitCount;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 localPos : TEXCOORD1;
                float4 diff : COLOR0;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v) {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.localPos = v.vertex.xyz;

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;

                return o;
            }

            float4 frag(v2f i) : SV_Target {
                float3 baseCol = tex2D(_MainTex, i.uv).rgb * _BaseColor.rgb;
                float3 bloodBlend = baseCol * i.diff;

                for (int j = 0; j < _HitCount; j++) {
                    float3 hitPos = _HitPositions[j].xyz;
                    float radius = _HitPositions[j].w;

                    float elapsed = _Time.y - _HitTimes[j] - _HitLifetime;
                    float t = smoothstep(1, 0, elapsed / _HitDisposeTime);
                    float alpha = clamp(0, 1, t);

                    float dist = distance(i.localPos, hitPos);
                    if (dist < radius * alpha) {
                        float mask = smoothstep(radius, 0.0, dist);
                        bloodBlend = lerp(bloodBlend, _BloodColor.rgb, 1 - mask);
                    }
                }

                return float4(bloodBlend, 1.0);
            }
            ENDHLSL
        }
    }
}