// Copyright (C) Itseez3D, Inc. - All Rights Reserved
// You may not use this file except in compliance with an authorized license
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
// See the License for the specific language governing permissions and limitations under the License.
// Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017

Shader "Avatar SDK/Lit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _BumpMap("Bumpmap", 2D) = "bump" {}
        _Cull("Culling: 0-Off, 1-Front, 2-Back", Int) = 0
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
        Cull[_Cull]
		CGPROGRAM
		#pragma surface surf Lambert// SimpleLambert

		float _DiffuseLight;
		half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten)
		{
			half NdotL = dot (s.Normal, lightDir);
			half4 c;
            c.rgb = s.Albedo *_LightColor0.rgb * (NdotL * atten);
			c.a = s.Alpha;

			return c;
		}

		struct Input
		{
			float2 uv_MainTex;
            float2 uv_BumpMap;
		};

		sampler2D _MainTex;
        sampler2D _BumpMap;

		void surf (Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
	Fallback "Diffuse"
}
