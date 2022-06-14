// Copyright (C) Itseez3D, Inc. - All Rights Reserved
// You may not use this file except in compliance with an authorized license
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
// See the License for the specific language governing permissions and limitations under the License.
// Written by Itseez3D, Inc. <support@avatarsdk.com>, September 2021

Shader "Avatar SDK/Recoloring/Skin"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _SkinMask("Skin Mask", 2D) = "black" {}
        _SkinColor("Skin Color", Color) = (0, 0, 0, 1)
        _TargetSkinColor("Target Skin Color", Color) = (0, 0, 0, 1)
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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 rgbToHSV(float3 RGB)
            {
                float3 HSV;

                float minChannel, maxChannel;
                if (RGB.x > RGB.y)
                {
                    maxChannel = RGB.x;
                    minChannel = RGB.y;
                }
                else
                {
                    maxChannel = RGB.y;
                    minChannel = RGB.x;
                }

                if (RGB.z > maxChannel) maxChannel = RGB.z;
                if (RGB.z < minChannel) minChannel = RGB.z;

                HSV.xy = 0;
                HSV.z = maxChannel;
                float delta = maxChannel - minChannel;
                if (delta != 0)
                {
                    HSV.y = delta / HSV.z;
                    float3 delRGB;
                    delRGB = (HSV.zzz - RGB + 3 * delta) / (6.0*delta);
                    if (RGB.x == HSV.z) HSV.x = delRGB.z - delRGB.y;
                    else if (RGB.y == HSV.z) HSV.x = (1.0 / 3.0) + delRGB.x - delRGB.z;
                    else if (RGB.z == HSV.z) HSV.x = (2.0 / 3.0) + delRGB.y - delRGB.x;
                }
                return (HSV);
            }

            float3 hsvToRGB(float3 HSV)
            {
                float3 RGB = HSV.z;

                float var_h = HSV.x * 6;
                float var_i = floor(var_h);
                float var_1 = HSV.z * (1.0 - HSV.y);
                float var_2 = HSV.z * (1.0 - HSV.y * (var_h - var_i));
                float var_3 = HSV.z * (1.0 - HSV.y * (1 - (var_h - var_i)));
                if (var_i == 0) { RGB = float3(HSV.z, var_3, var_1); }
                else if (var_i == 1) { RGB = float3(var_2, HSV.z, var_1); }
                else if (var_i == 2) { RGB = float3(var_1, HSV.z, var_3); }
                else if (var_i == 3) { RGB = float3(var_1, var_2, HSV.z); }
                else if (var_i == 4) { RGB = float3(var_3, var_1, HSV.z); }
                else { RGB = float3(HSV.z, var_1, var_2); }

                return (RGB);
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
            sampler2D _SkinMask;
            fixed4 _SkinColor;
            fixed4 _TargetSkinColor;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed4 skinMask = tex2D(_SkinMask, i.uv);
                if (skinMask.r > 0)
                {
#if !UNITY_COLORSPACE_GAMMA
                    col = linearToGamma(col);
                    _SkinColor = linearToGamma(_SkinColor);
                    _TargetSkinColor = linearToGamma(_TargetSkinColor);
#endif

                    float3 skinColorHSV = rgbToHSV(_SkinColor.rgb);
                    float3 targetSkinColorHSV = rgbToHSV(_TargetSkinColor.rgb);

                    float hueShift = targetSkinColorHSV.x - skinColorHSV.x;
                    float saturationScale = targetSkinColorHSV.y / skinColorHSV.y;
                    float valueShift = targetSkinColorHSV.z - skinColorHSV.z;
                    float valueDistanceThreshold = 200.0f / 255.0f;

                    fixed3 colHSV = rgbToHSV(col);
                    {
                        //Hue
                        float hue = colHSV.x + hueShift;
                        if (hue < 0)
                        {
                            hue += 1.0f;
                        }
                        if (hue > 1.0f)
                        {
                            hue -= 1.0f;
                        }
                        colHSV.x = hue;
                    }
                    {
                        //Saturation
                        colHSV.y = max(0, min(1.0f, colHSV.y * saturationScale));
                    }
                    {
                        //Value
                        float coeff = max(0.0f, 1.0f - abs(colHSV.z - skinColorHSV.z) / valueDistanceThreshold);
                        colHSV.z = max(0, min(1.0f, colHSV.z + coeff * valueShift));
                    }

                    col.rgb = hsvToRGB(colHSV);
#if !UNITY_COLORSPACE_GAMMA
                    col = gammaToLinear(col);
#endif
                }
                return col;
            }
            ENDCG
        }
    }
}
