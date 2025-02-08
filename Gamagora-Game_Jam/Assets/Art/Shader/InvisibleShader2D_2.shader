Shader "Custom/2D Light Cone Reveal"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint Color", Color) = (1,1,1,1)
        _LightPos("Light Position", Vector) = (0,0,0,0)
        _LightDir("Light Direction", Vector) = (0,1,0,0)
        _InnerAngle("Inner Angle", Range(0,180)) = 30
        _OuterAngle("Outer Angle", Range(0,180)) = 60
        _LightRadius("Light Radius", Float) = 5
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _LightPos;
            float4 _LightDir;
            float _InnerAngle;
            float _OuterAngle;
            float _LightRadius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Distance entre la lumière et le pixel
                float2 toPixel = i.worldPos.xy - _LightPos.xy;
                float distance = length(toPixel);
                
                // Normalisation de la direction
                float2 dirToPixel = normalize(toPixel);
                float2 lightDirection = normalize(_LightDir.xy);

                // Vérifie si le pixel est hors de la portée
                if (distance > _LightRadius) discard;

                // Calcul de l'angle entre la direction de la lumière et le pixel
                float angleToPixel = degrees(acos(dot(dirToPixel, lightDirection)));

                // Vérifie si l'angle est hors du cône
                if (angleToPixel > _OuterAngle * 0.5) discard;

                // Lissage de la transition entre Inner et Outer Angle
                float visibility = 1.0;
                if (angleToPixel > _InnerAngle * 0.5)
                {
                    float t = (angleToPixel - (_InnerAngle * 0.5)) / ((_OuterAngle * 0.5) - (_InnerAngle * 0.5));
                    visibility = 1.0 - t;
                }

                // Applique la visibilité au sprite
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.a *= visibility;

                return col;
            }
            ENDCG
        }
    }
}