// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:True,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:1,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33420,y:32510,varname:node_2865,prsc:2|diff-6343-OUT,spec-4458-OUT,gloss-5871-OUT,emission-5391-OUT,clip-2456-A;n:type:ShaderForge.SFN_Multiply,id:6343,x:33022,y:32446,varname:node_6343,prsc:2|A-2456-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:32955,y:32262,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_TexCoord,id:8455,x:31039,y:33250,cmnt:Z is tile index,varname:node_8455,prsc:2,uv:0,uaff:True;n:type:ShaderForge.SFN_Tex2d,id:2456,x:33115,y:32880,varname:node_2456,prsc:2,ntxv:0,isnm:False|UVIN-5144-OUT,TEX-7195-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:7195,x:33022,y:32640,ptovrint:False,ptlb:TileMap,ptin:_TileMap,varname:node_7195,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:1269,x:31643,y:33497,ptovrint:False,ptlb:X Tiles,ptin:_XTiles,varname:node_1269,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:6;n:type:ShaderForge.SFN_ValueProperty,id:9554,x:31643,y:33578,ptovrint:False,ptlb:Y Tiles,ptin:_YTiles,varname:node_9554,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:6;n:type:ShaderForge.SFN_Divide,id:5222,x:31930,y:33228,varname:node_5222,prsc:2|A-4552-OUT,B-1269-OUT;n:type:ShaderForge.SFN_Floor,id:7610,x:32102,y:33239,varname:node_7610,prsc:2|IN-5222-OUT;n:type:ShaderForge.SFN_Fmod,id:519,x:31930,y:33372,varname:node_519,prsc:2|A-4552-OUT,B-9554-OUT;n:type:ShaderForge.SFN_Append,id:5144,x:32783,y:32819,varname:node_5144,prsc:2|A-499-OUT,B-3878-OUT;n:type:ShaderForge.SFN_Vector1,id:6513,x:32102,y:33487,varname:node_6513,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:8832,x:32095,y:33410,varname:node_8832,prsc:2,v1:1;n:type:ShaderForge.SFN_Divide,id:7752,x:32303,y:33431,varname:node_7752,prsc:2|A-6513-OUT,B-1269-OUT;n:type:ShaderForge.SFN_Divide,id:7533,x:32303,y:33562,varname:node_7533,prsc:2|A-8832-OUT,B-9554-OUT;n:type:ShaderForge.SFN_Multiply,id:9822,x:32478,y:33431,varname:node_9822,prsc:2|A-7752-OUT,B-519-OUT;n:type:ShaderForge.SFN_Multiply,id:7043,x:32478,y:33561,varname:node_7043,prsc:2|A-7533-OUT,B-7610-OUT;n:type:ShaderForge.SFN_TexCoord,id:9540,x:30921,y:32317,cmnt:Tile Width Tile Height XOffset YOffset,varname:node_9540,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_ValueProperty,id:1440,x:31500,y:31916,ptovrint:False,ptlb:TileWidth,ptin:_TileWidth,varname:node_1440,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:256;n:type:ShaderForge.SFN_ValueProperty,id:7902,x:31387,y:32031,ptovrint:False,ptlb:TileHeight,ptin:_TileHeight,varname:node_7902,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:256;n:type:ShaderForge.SFN_Multiply,id:9353,x:31728,y:32155,varname:node_9353,prsc:2|A-7752-OUT,B-2334-OUT;n:type:ShaderForge.SFN_Multiply,id:6515,x:31711,y:32301,varname:node_6515,prsc:2|A-7533-OUT,B-6528-OUT;n:type:ShaderForge.SFN_Divide,id:2334,x:31702,y:31796,varname:node_2334,prsc:2|A-9540-U,B-1440-OUT;n:type:ShaderForge.SFN_Divide,id:6528,x:31728,y:32003,varname:node_6528,prsc:2|A-9540-V,B-7902-OUT;n:type:ShaderForge.SFN_Subtract,id:3481,x:32204,y:31874,varname:node_3481,prsc:2|A-8803-OUT,B-6731-OUT;n:type:ShaderForge.SFN_Fmod,id:3843,x:32379,y:31923,varname:node_3843,prsc:2|A-3481-OUT,B-5998-OUT;n:type:ShaderForge.SFN_Subtract,id:5998,x:32204,y:31997,varname:node_5998,prsc:2|A-9353-OUT,B-6731-OUT;n:type:ShaderForge.SFN_Add,id:2117,x:32567,y:31908,varname:node_2117,prsc:2|A-3843-OUT,B-6731-OUT;n:type:ShaderForge.SFN_Subtract,id:9535,x:32204,y:32135,varname:node_9535,prsc:2|A-4486-OUT,B-6731-OUT;n:type:ShaderForge.SFN_Subtract,id:7097,x:32204,y:32280,varname:node_7097,prsc:2|A-6515-OUT,B-6731-OUT;n:type:ShaderForge.SFN_Fmod,id:3058,x:32375,y:32190,varname:node_3058,prsc:2|A-9535-OUT,B-7097-OUT;n:type:ShaderForge.SFN_Add,id:6944,x:32573,y:32172,varname:node_6944,prsc:2|A-3058-OUT,B-6731-OUT;n:type:ShaderForge.SFN_Vector1,id:6731,x:31830,y:31617,varname:node_6731,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:499,x:32479,y:32789,varname:node_499,prsc:2|A-2117-OUT,B-9822-OUT,C-7478-OUT;n:type:ShaderForge.SFN_Add,id:3878,x:32479,y:32925,varname:node_3878,prsc:2|A-939-OUT,B-7043-OUT;n:type:ShaderForge.SFN_Divide,id:7863,x:31722,y:32947,varname:node_7863,prsc:2|A-8455-U,B-1269-OUT;n:type:ShaderForge.SFN_Divide,id:7772,x:31722,y:33074,varname:node_7772,prsc:2|A-8455-V,B-9554-OUT;n:type:ShaderForge.SFN_Multiply,id:1001,x:31959,y:32947,varname:node_1001,prsc:2|A-7863-OUT,B-2334-OUT;n:type:ShaderForge.SFN_Multiply,id:9984,x:31959,y:33074,varname:node_9984,prsc:2|A-7772-OUT,B-6528-OUT;n:type:ShaderForge.SFN_Round,id:4552,x:31367,y:33454,varname:node_4552,prsc:2|IN-8455-Z;n:type:ShaderForge.SFN_FragmentPosition,id:3106,x:30445,y:31431,varname:node_3106,prsc:2;n:type:ShaderForge.SFN_ObjectPosition,id:8354,x:30445,y:31573,varname:node_8354,prsc:2;n:type:ShaderForge.SFN_Subtract,id:1847,x:30633,y:31537,varname:node_1847,prsc:2|A-3106-XYZ,B-8354-XYZ;n:type:ShaderForge.SFN_Transform,id:1447,x:30549,y:31709,varname:node_1447,prsc:2,tffrom:0,tfto:1|IN-1847-OUT;n:type:ShaderForge.SFN_ComponentMask,id:8582,x:30733,y:31716,varname:node_8582,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-1447-XYZ;n:type:ShaderForge.SFN_Abs,id:2212,x:30932,y:31716,varname:node_2212,prsc:2|IN-8582-G;n:type:ShaderForge.SFN_Divide,id:9970,x:31113,y:31716,varname:node_9970,prsc:2|A-2212-OUT,B-9554-OUT;n:type:ShaderForge.SFN_Divide,id:7714,x:30997,y:32110,varname:node_7714,prsc:2|A-7902-OUT,B-9540-V;n:type:ShaderForge.SFN_Divide,id:8578,x:31186,y:31915,varname:node_8578,prsc:2|A-9970-OUT,B-7714-OUT;n:type:ShaderForge.SFN_Vector1,id:8714,x:30784,y:32800,varname:node_8714,prsc:2,v1:64;n:type:ShaderForge.SFN_Divide,id:984,x:31097,y:32713,varname:node_984,prsc:2|A-9540-U,B-8714-OUT;n:type:ShaderForge.SFN_Divide,id:8803,x:32187,y:32435,varname:node_8803,prsc:2|A-1001-OUT,B-984-OUT;n:type:ShaderForge.SFN_Divide,id:5484,x:31015,y:32965,varname:node_5484,prsc:2|A-9540-V,B-2226-OUT;n:type:ShaderForge.SFN_Vector1,id:2226,x:30752,y:33033,varname:node_2226,prsc:2,v1:64;n:type:ShaderForge.SFN_Vector1,id:5103,x:30921,y:32458,varname:node_5103,prsc:2,v1:0;n:type:ShaderForge.SFN_Subtract,id:939,x:32525,y:32470,varname:node_939,prsc:2|A-6515-OUT,B-6944-OUT;n:type:ShaderForge.SFN_Slider,id:4458,x:33184,y:32223,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_4458,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:5871,x:33184,y:32324,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_5871,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_ValueProperty,id:2367,x:32962,y:31969,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_2367,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:5391,x:33161,y:32000,varname:node_5391,prsc:2|A-2367-OUT,B-2456-RGB;n:type:ShaderForge.SFN_Divide,id:5127,x:33085,y:33371,varname:node_5127,prsc:2|A-8832-OUT,B-8807-OUT;n:type:ShaderForge.SFN_Multiply,id:8807,x:32817,y:33268,varname:node_8807,prsc:2|A-1269-OUT,B-1440-OUT;n:type:ShaderForge.SFN_Multiply,id:7478,x:33085,y:33235,varname:node_7478,prsc:2|A-9540-Z,B-5127-OUT;n:type:ShaderForge.SFN_Divide,id:5632,x:33162,y:33756,varname:node_5632,prsc:2|A-6513-OUT,B-6781-OUT;n:type:ShaderForge.SFN_Multiply,id:6781,x:32940,y:33613,varname:node_6781,prsc:2|A-7902-OUT,B-9554-OUT;n:type:ShaderForge.SFN_Multiply,id:8680,x:33380,y:33513,varname:node_8680,prsc:2|A-9540-W,B-5954-OUT;n:type:ShaderForge.SFN_Divide,id:1403,x:31773,y:32673,varname:node_1403,prsc:2|A-9984-OUT,B-5484-OUT;n:type:ShaderForge.SFN_Add,id:4486,x:31992,y:32530,varname:node_4486,prsc:2|A-1403-OUT,B-5103-OUT;n:type:ShaderForge.SFN_Divide,id:5954,x:33415,y:33672,varname:node_5954,prsc:2|A-5632-OUT,B-5484-OUT;proporder:6665-7195-1269-9554-1440-7902-4458-5871-2367-3730;pass:END;sub:END;*/

Shader "UnityWAD/DoomWalls" {
    Properties {
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _TileMap ("TileMap", 2D) = "white" {}
        _XTiles ("X Tiles", Float ) = 6
        _YTiles ("Y Tiles", Float ) = 6
        _TileWidth ("TileWidth", Float ) = 256
        _TileHeight ("TileHeight", Float ) = 256
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0
        _Emission ("Emission", Float ) = 0
        _node_3730 ("node_3730", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
            "CanUseSpriteAtlas"="True"
        }
        Pass {
            Name "DEFERRED"
            Tags {
                "LightMode"="Deferred"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_DEFERRED
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile ___ UNITY_HDR_ON
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _TileMap; uniform float4 _TileMap_ST;
            uniform float _XTiles;
            uniform float _YTiles;
            uniform float _TileWidth;
            uniform float _TileHeight;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _Emission;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD7;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            void frag(
                VertexOutput i,
                out half4 outDiffuse : SV_Target0,
                out half4 outSpecSmoothness : SV_Target1,
                out half4 outNormal : SV_Target2,
                out half4 outEmission : SV_Target3 )
            {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float node_2334 = (i.uv1.r/_TileWidth);
                float node_6731 = 0.0;
                float node_6513 = 1.0;
                float node_4552 = round(i.uv0.b);
                float node_8832 = 1.0;
                float node_7533 = (node_8832/_YTiles);
                float node_6528 = (i.uv1.g/_TileHeight);
                float node_6515 = (node_7533*node_6528);
                float node_9984 = ((i.uv0.g/_YTiles)*node_6528);
                float node_5484 = (i.uv1.g/64.0);
                float node_1403 = (node_9984/node_5484);
                float2 node_5144 = float2(((fmod(((((i.uv0.r/_XTiles)*node_2334)/(i.uv1.r/64.0))-node_6731),(((node_6513/_XTiles)*node_2334)-node_6731))+node_6731)+((node_6513/_XTiles)*fmod(node_4552,_YTiles))+(i.uv1.b*(node_8832/(_XTiles*_TileWidth)))),((node_6515-(fmod(((node_1403+0.0)-node_6731),(node_6515-node_6731))+node_6731))+(node_7533*floor((node_4552/_XTiles)))));
                float4 node_2456 = tex2D(_TileMap,TRANSFORM_TEX(node_5144, _TileMap));
                clip(node_2456.a - 0.5);
////// Lighting:
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
/////// GI Data:
                UnityLight light; // Dummy light
                light.color = 0;
                light.dir = half3(0,1,0);
                light.ndotl = max(0,dot(normalDirection,light.dir));
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = 1;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
////// Specular:
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float3 diffuseColor = (node_2456.rgb*_Color.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
/////// Diffuse:
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
////// Emissive:
                float3 emissive = (_Emission*node_2456.rgb);
/// Final Color:
                outDiffuse = half4( diffuseColor, 1 );
                outSpecSmoothness = half4( specularColor, gloss );
                outNormal = half4( normalDirection * 0.5 + 0.5, 1 );
                outEmission = half4( (_Emission*node_2456.rgb), 1 );
                outEmission.rgb += indirectSpecular * 1;
                outEmission.rgb += indirectDiffuse * diffuseColor;
                #ifndef UNITY_HDR_ON
                    outEmission.rgb = exp2(-outEmission.rgb);
                #endif
            }
            ENDCG
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _TileMap; uniform float4 _TileMap_ST;
            uniform float _XTiles;
            uniform float _YTiles;
            uniform float _TileWidth;
            uniform float _TileHeight;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _Emission;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float node_2334 = (i.uv1.r/_TileWidth);
                float node_6731 = 0.0;
                float node_6513 = 1.0;
                float node_4552 = round(i.uv0.b);
                float node_8832 = 1.0;
                float node_7533 = (node_8832/_YTiles);
                float node_6528 = (i.uv1.g/_TileHeight);
                float node_6515 = (node_7533*node_6528);
                float node_9984 = ((i.uv0.g/_YTiles)*node_6528);
                float node_5484 = (i.uv1.g/64.0);
                float node_1403 = (node_9984/node_5484);
                float2 node_5144 = float2(((fmod(((((i.uv0.r/_XTiles)*node_2334)/(i.uv1.r/64.0))-node_6731),(((node_6513/_XTiles)*node_2334)-node_6731))+node_6731)+((node_6513/_XTiles)*fmod(node_4552,_YTiles))+(i.uv1.b*(node_8832/(_XTiles*_TileWidth)))),((node_6515-(fmod(((node_1403+0.0)-node_6731),(node_6515-node_6731))+node_6731))+(node_7533*floor((node_4552/_XTiles)))));
                float4 node_2456 = tex2D(_TileMap,TRANSFORM_TEX(node_5144, _TileMap));
                clip(node_2456.a - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float3 diffuseColor = (node_2456.rgb*_Color.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = (_Emission*node_2456.rgb);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _TileMap; uniform float4 _TileMap_ST;
            uniform float _XTiles;
            uniform float _YTiles;
            uniform float _TileWidth;
            uniform float _TileHeight;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _Emission;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float node_2334 = (i.uv1.r/_TileWidth);
                float node_6731 = 0.0;
                float node_6513 = 1.0;
                float node_4552 = round(i.uv0.b);
                float node_8832 = 1.0;
                float node_7533 = (node_8832/_YTiles);
                float node_6528 = (i.uv1.g/_TileHeight);
                float node_6515 = (node_7533*node_6528);
                float node_9984 = ((i.uv0.g/_YTiles)*node_6528);
                float node_5484 = (i.uv1.g/64.0);
                float node_1403 = (node_9984/node_5484);
                float2 node_5144 = float2(((fmod(((((i.uv0.r/_XTiles)*node_2334)/(i.uv1.r/64.0))-node_6731),(((node_6513/_XTiles)*node_2334)-node_6731))+node_6731)+((node_6513/_XTiles)*fmod(node_4552,_YTiles))+(i.uv1.b*(node_8832/(_XTiles*_TileWidth)))),((node_6515-(fmod(((node_1403+0.0)-node_6731),(node_6515-node_6731))+node_6731))+(node_7533*floor((node_4552/_XTiles)))));
                float4 node_2456 = tex2D(_TileMap,TRANSFORM_TEX(node_5144, _TileMap));
                clip(node_2456.a - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float3 diffuseColor = (node_2456.rgb*_Color.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _TileMap; uniform float4 _TileMap_ST;
            uniform float _XTiles;
            uniform float _YTiles;
            uniform float _TileWidth;
            uniform float _TileHeight;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
                float4 uv1 : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float node_2334 = (i.uv1.r/_TileWidth);
                float node_6731 = 0.0;
                float node_6513 = 1.0;
                float node_4552 = round(i.uv0.b);
                float node_8832 = 1.0;
                float node_7533 = (node_8832/_YTiles);
                float node_6528 = (i.uv1.g/_TileHeight);
                float node_6515 = (node_7533*node_6528);
                float node_9984 = ((i.uv0.g/_YTiles)*node_6528);
                float node_5484 = (i.uv1.g/64.0);
                float node_1403 = (node_9984/node_5484);
                float2 node_5144 = float2(((fmod(((((i.uv0.r/_XTiles)*node_2334)/(i.uv1.r/64.0))-node_6731),(((node_6513/_XTiles)*node_2334)-node_6731))+node_6731)+((node_6513/_XTiles)*fmod(node_4552,_YTiles))+(i.uv1.b*(node_8832/(_XTiles*_TileWidth)))),((node_6515-(fmod(((node_1403+0.0)-node_6731),(node_6515-node_6731))+node_6731))+(node_7533*floor((node_4552/_XTiles)))));
                float4 node_2456 = tex2D(_TileMap,TRANSFORM_TEX(node_5144, _TileMap));
                clip(node_2456.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _TileMap; uniform float4 _TileMap_ST;
            uniform float _XTiles;
            uniform float _YTiles;
            uniform float _TileWidth;
            uniform float _TileHeight;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform float _Emission;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float node_2334 = (i.uv1.r/_TileWidth);
                float node_6731 = 0.0;
                float node_6513 = 1.0;
                float node_4552 = round(i.uv0.b);
                float node_8832 = 1.0;
                float node_7533 = (node_8832/_YTiles);
                float node_6528 = (i.uv1.g/_TileHeight);
                float node_6515 = (node_7533*node_6528);
                float node_9984 = ((i.uv0.g/_YTiles)*node_6528);
                float node_5484 = (i.uv1.g/64.0);
                float node_1403 = (node_9984/node_5484);
                float2 node_5144 = float2(((fmod(((((i.uv0.r/_XTiles)*node_2334)/(i.uv1.r/64.0))-node_6731),(((node_6513/_XTiles)*node_2334)-node_6731))+node_6731)+((node_6513/_XTiles)*fmod(node_4552,_YTiles))+(i.uv1.b*(node_8832/(_XTiles*_TileWidth)))),((node_6515-(fmod(((node_1403+0.0)-node_6731),(node_6515-node_6731))+node_6731))+(node_7533*floor((node_4552/_XTiles)))));
                float4 node_2456 = tex2D(_TileMap,TRANSFORM_TEX(node_5144, _TileMap));
                o.Emission = (_Emission*node_2456.rgb);
                
                float3 diffColor = (node_2456.rgb*_Color.rgb);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic, specColor, specularMonochrome );
                float roughness = 1.0 - _Gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
