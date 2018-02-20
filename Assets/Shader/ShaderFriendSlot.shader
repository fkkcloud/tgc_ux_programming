// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha-blended shader.
 // - no lighting
 // - no lightmap support
 // - no per-material color
 
 Shader "Custom/FriendSlot" {
 Properties {
     _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
     _UIOpacity ("UI Opacity", Float) = 0.0
     _ColorMult ("ColorMult", Float) = 1.0
 }
 
 SubShader {
     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     LOD 100
     
     ZWrite Off
     Blend SrcAlpha OneMinusSrcAlpha 
     
     Pass {  
         CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_fog
             
             #include "UnityCG.cginc"
 
             struct appdata_t {
                 float4 vertex : POSITION;
                 float2 texcoord : TEXCOORD0;
                 float4 color: COLOR;
             };
 
             struct v2f {
                 float4 vertex : SV_POSITION;
                 half2 texcoord : TEXCOORD0;
                 float4 color: COLOR;
                 UNITY_FOG_COORDS(1)
             };
 
             sampler2D _MainTex;
             float4 _MainTex_ST;
             float _Opacity;
             float _UIOpacity;
             float _ColorMult;
             
             v2f vert (appdata_t v)
             {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                 o.color = v.color;
                 UNITY_TRANSFER_FOG(o,o.vertex);
                 return o;
             }
             
             fixed4 frag (v2f i) : SV_Target
             {
                 fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                 UNITY_APPLY_FOG(i.fogCoord, col);
                 col.a *=_UIOpacity;
                 col.r *= _ColorMult;
                 col.g *= _ColorMult;
                 col.b *= _ColorMult;
                    
                 return col;
             }
         ENDCG
     }
 }
 
 }