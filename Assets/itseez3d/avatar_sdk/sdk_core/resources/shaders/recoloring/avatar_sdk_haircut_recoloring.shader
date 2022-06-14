// Copyright (C) Itseez3D, Inc. - All Rights Reserved
// You may not use this file except in compliance with an authorized license
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
// See the License for the specific language governing permissions and limitations under the License.
// Written by Itseez3D, Inc. <support@avatarsdk.com>, September 2021

Shader "Avatar SDK/Recoloring/Haircut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorTarget("ColorTarget", Vector) = (1, 1, 1, 0)
        _ColorTint("ColorTint", Vector) = (0, 0, 0, 0)
        _TintCoeff("TintCoeff", Float) = 0.8
        _MultiplicationTintThreshold("MultiplicationTintThreshold", Float) = 0.2
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 linearToGamma(fixed4 c)
            {
                half gamma = 1 / 2.2;
                return pow(c, gamma);
            }

            fixed4 gammaToLinear(fixed4 c)
            {
                half gamma = 2.2;
                return pow(c, gamma);
            }

            sampler2D _MainTex;
            fixed4 _ColorTarget;
            fixed4 _ColorTint;
            float _TintCoeff;
            float _MultiplicationTintThreshold;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 colorTexture = tex2D(_MainTex, i.uv);
#if !UNITY_COLORSPACE_GAMMA
                colorTexture = linearToGamma(colorTexture);
#endif
                float threshold = _MultiplicationTintThreshold;
                fixed4 tinted = colorTexture + _TintCoeff * _ColorTint;

                float maxTargetChannel = max(_ColorTarget.r, max(_ColorTarget.g, _ColorTarget.b));
                if (maxTargetChannel < threshold)
                {
                    float darkeningCoeff = min(0.85, (threshold - maxTargetChannel) / threshold);
                    tinted = (1.0 - darkeningCoeff) * tinted + darkeningCoeff * (_ColorTarget * colorTexture);
                }

                fixed4 col = fixed4(tinted.rgb, colorTexture.a);
#if !UNITY_COLORSPACE_GAMMA
                col = gammaToLinear(col);
#endif
                return col;
            }
            ENDCG
        }
    }
}
