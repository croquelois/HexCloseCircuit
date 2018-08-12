Shader "Custom/liquid" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _Glossiness ("Smoothness", Range(0,1)) = 0.5
    _Specular ("Specular", Color) = (0.2, 0.2, 0.2)
  }
  SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
    LOD 200
    
    CGPROGRAM
    
    #pragma surface surf StandardSpecular alpha
    #pragma target 3.0

    sampler2D _MainTex;

    struct Input {
      float2 uv_MainTex;
      float3 worldPos;
    };

    half _Glossiness;
    half _Specular;
    fixed4 _Color;

    UNITY_INSTANCING_CBUFFER_START(Props)
    UNITY_INSTANCING_CBUFFER_END

    float liquid(float2 inUV, sampler2D perlin){
      float2 uv = inUV*0.1;
      uv.y += _Time.y*0.1;
      float4 noise = tex2D(perlin, uv);
      
      float2 uv2 = inUV*0.1;
      uv2.y -= _Time.y*0.1;
      float4 noise2 = tex2D(perlin, uv2);
      
      float2 uv3 = inUV*0.1;
      uv3.x -= _Time.y*0.1;
      float4 noise3 = tex2D(perlin, uv3);
      
      return noise.x * noise2.y * noise3.z;
    }
    
    void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
      float river = liquid(IN.worldPos.xz, _MainTex);
      
      fixed4 c = saturate(river + _Color);
      o.Albedo = c.rgb;
      o.Specular = _Specular;
      o.Smoothness = _Glossiness;
      o.Occlusion = 1.0;
      o.Alpha = 0.5*c.a;
    }
    
    ENDCG
  }
  FallBack "Diffuse"
}
