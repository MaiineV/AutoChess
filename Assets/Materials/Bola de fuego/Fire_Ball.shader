// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fire_Ball"
{
	Properties
	{
		_Texture0("Texture 0", 2D) = "white" {}
		_Texture1("Texture 1", 2D) = "bump" {}
		_DistortionAmount("Distortion Amount", Range( 0 , 1)) = 1
		_ScrollSpeed("Scroll Speed", Range( 0 , 1)) = 1
		_Float0("Float 0", Float) = 6
		_DivideAmount("Divide Amount", Float) = 1.05
		_Burn("Burn", Range( 0 , 1)) = 1
		_Heatwave("Heat wave", Range( 0 , 1)) = 1
		_DissolveAmount("Dissolve Amount", Float) = 0
		_WiggleAmount("Wiggle Amount", Float) = 0
		_TextureSample4("Texture Sample 4", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		AlphaToMask On
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Texture1;
		uniform float _ScrollSpeed;
		uniform sampler2D _Texture0;
		uniform float _Heatwave;
		uniform float _Burn;
		uniform float _WiggleAmount;
		uniform sampler2D _TextureSample4;
		uniform float4 _TextureSample4_ST;
		uniform float4 _Texture1_ST;
		uniform float _DistortionAmount;
		uniform float _Float0;
		uniform float _DivideAmount;
		uniform float _DissolveAmount;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float temp_output_13_0 = ( _Time.y * _ScrollSpeed );
			float2 panner34 = ( temp_output_13_0 * float2( 0,-1 ) + v.texcoord.xy);
			float3 tex2DNode32 = UnpackNormal( tex2Dlod( _Texture1, float4( panner34, 0, 0.0) ) );
			float4 tex2DNode22 = tex2Dlod( _Texture0, float4( ( ( (tex2DNode32).xy * _Heatwave ) + v.texcoord.xy ), 0, 0.0) );
			float temp_output_23_0 = step( tex2DNode22.r , _Burn );
			v.vertex.xyz += ( ( ( ase_vertex3Pos * ase_vertex3Pos ) * tex2DNode32 * temp_output_23_0 ) * _WiggleAmount );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TextureSample4 = i.uv_texcoord * _TextureSample4_ST.xy + _TextureSample4_ST.zw;
			o.Albedo = tex2D( _TextureSample4, uv_TextureSample4 ).rgb;
			float4 color15 = IsGammaSpace() ? float4(0.945098,0.5668061,0,0) : float4(0.8796223,0.2811682,0,0);
			float4 color16 = IsGammaSpace() ? float4(1,0.9607843,0.04313726,0) : float4(1,0.9130987,0.003346536,0);
			float2 uv_Texture1 = i.uv_texcoord * _Texture1_ST.xy + _Texture1_ST.zw;
			float temp_output_13_0 = ( _Time.y * _ScrollSpeed );
			float2 panner11 = ( temp_output_13_0 * float2( 0,-1 ) + float2( 0,0 ));
			float2 uv_TexCoord9 = i.uv_texcoord + panner11;
			float4 lerpResult17 = lerp( color15 , color16 , tex2D( _Texture0, ( ( (UnpackNormal( tex2D( _Texture1, uv_Texture1 ) )).xy * _DistortionAmount ) + uv_TexCoord9 ) ));
			float4 temp_cast_1 = (_Float0).xxxx;
			float2 panner34 = ( temp_output_13_0 * float2( 0,-1 ) + i.uv_texcoord);
			float3 tex2DNode32 = UnpackNormal( tex2D( _Texture1, panner34 ) );
			float4 tex2DNode22 = tex2D( _Texture0, ( ( (tex2DNode32).xy * _Heatwave ) + i.uv_texcoord ) );
			float temp_output_23_0 = step( tex2DNode22.r , _Burn );
			float temp_output_48_0 = ( _DissolveAmount / _DivideAmount );
			float temp_output_44_0 = step( tex2DNode22.r , ( 1.0 - temp_output_48_0 ) );
			float temp_output_45_0 = ( temp_output_44_0 - step( tex2DNode22.r , ( 1.0 - temp_output_48_0 ) ) );
			float4 temp_cast_2 = (temp_output_45_0).xxxx;
			float4 temp_cast_3 = (temp_output_45_0).xxxx;
			o.Emission = saturate( ( ( ( ( pow( lerpResult17 , temp_cast_1 ) * 1.0 ) * ( temp_output_23_0 + ( temp_output_23_0 - step( tex2DNode22.r , ( _Burn / _DivideAmount ) ) ) ) ) - temp_cast_2 ) - temp_cast_3 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			AlphaToMask Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
2427;73;887;617;-781.8331;309.1647;1.589825;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;12;-2661.965,273.3466;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-2734.012,422.4311;Inherit;False;Property;_ScrollSpeed;Scroll Speed;4;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;35;-2854.203,743.3837;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-2354.108,318.2389;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;4;-2396.556,-399.3398;Inherit;True;Property;_Texture1;Texture 1;1;0;Create;True;0;0;0;False;0;False;455f0d38f02799846b98adf7002a81dc;455f0d38f02799846b98adf7002a81dc;False;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.PannerNode;34;-2473.357,802.6817;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;5;-2076.863,-172.5303;Inherit;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;32;-2093.954,771.4681;Inherit;True;Property;_TextureSample3;Texture Sample 3;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-1745.779,107.6784;Inherit;False;Property;_DistortionAmount;Distortion Amount;2;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;11;-1747.166,336.9796;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;36;-1733.059,795.2373;Inherit;True;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1720.769,1058.753;Inherit;False;Property;_Heatwave;Heat wave;8;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;6;-1708.543,-177.2005;Inherit;True;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1418.466,140.7999;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1391.894,-125.1549;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1374.099,875.6302;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;39;-1378.471,1203.117;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-1036.318,1034.8;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-963.4429,-36.99249;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-532.9573,550.8126;Inherit;False;Property;_DivideAmount;Divide Amount;6;0;Create;True;0;0;0;False;0;False;1.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-971.188,-552.7451;Inherit;True;Property;_Texture0;Texture 0;0;0;Create;True;0;0;0;False;0;False;3a13284c4203af746a27bf1d499ce834;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;24;-568.4454,339.273;Inherit;False;Property;_Burn;Burn;7;0;Create;True;0;0;0;False;0;False;1;0.5966471;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;16;-626.5898,-583.5939;Inherit;False;Constant;_Hot;Hot;5;0;Create;True;0;0;0;False;0;False;1,0.9607843,0.04313726,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;49;-631.6805,785.2931;Inherit;True;Property;_DissolveAmount;Dissolve Amount;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-640.694,-336.3268;Inherit;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;15;-604.3203,-777.6559;Inherit;False;Constant;_Warm;Warm;5;0;Create;True;0;0;0;False;0;False;0.945098,0.5668061,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;22;-672.3963,48.03872;Inherit;True;Property;_TextureSample2;Texture Sample 2;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;27;-186.7829,391.4439;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;48;-297.1303,835.9518;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;29;77.88016,259.6216;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;202.2281,-483.8652;Inherit;True;Property;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;17;-173.5645,-629.7147;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;23;-220.2596,8.579823;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;42;93.77266,685.6441;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;30;314.7818,83.98918;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;41;-46.5206,934.2318;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;18;265.3371,-839.4014;Inherit;True;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;21;423.075,-433.6835;Inherit;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;681.1111,-881.0708;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;43;161.7083,962.0593;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;573.0021,-154.8614;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;44;342.1583,521.4934;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;51;1050.452,-346.4037;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;50;1075.044,-617.4702;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;45;609.983,724.5217;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;860.1371,-267.3425;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;1324.427,-503.7773;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;46;882.3654,139.5126;Inherit;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;1559.157,-362.6137;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;54;1382.506,-56.87976;Inherit;False;Property;_WiggleAmount;Wiggle Amount;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;47;1206.96,359.5616;Inherit;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;1669.263,-152.4657;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;57;1625.557,-663.5049;Inherit;True;Property;_TextureSample4;Texture Sample 4;11;0;Create;True;0;0;0;False;0;False;-1;4af1892cea9dcd94e902a0774b6a060a;4af1892cea9dcd94e902a0774b6a060a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;58;1551.71,181.995;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1904.12,-210.6857;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Fire_Ball;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;34;0;35;0
WireConnection;34;1;13;0
WireConnection;5;0;4;0
WireConnection;32;0;4;0
WireConnection;32;1;34;0
WireConnection;11;1;13;0
WireConnection;36;0;32;0
WireConnection;6;0;5;0
WireConnection;9;1;11;0
WireConnection;7;0;6;0
WireConnection;7;1;8;0
WireConnection;37;0;36;0
WireConnection;37;1;38;0
WireConnection;40;0;37;0
WireConnection;40;1;39;0
WireConnection;10;0;7;0
WireConnection;10;1;9;0
WireConnection;2;0;1;0
WireConnection;2;1;10;0
WireConnection;22;0;1;0
WireConnection;22;1;40;0
WireConnection;27;0;24;0
WireConnection;27;1;28;0
WireConnection;48;0;49;0
WireConnection;48;1;28;0
WireConnection;29;0;22;1
WireConnection;29;1;27;0
WireConnection;17;0;15;0
WireConnection;17;1;16;0
WireConnection;17;2;2;0
WireConnection;23;0;22;1
WireConnection;23;1;24;0
WireConnection;42;0;48;0
WireConnection;30;0;23;0
WireConnection;30;1;29;0
WireConnection;41;0;48;0
WireConnection;18;0;17;0
WireConnection;18;1;20;0
WireConnection;19;0;18;0
WireConnection;19;1;21;0
WireConnection;43;0;22;1
WireConnection;43;1;41;0
WireConnection;31;0;23;0
WireConnection;31;1;30;0
WireConnection;44;0;22;1
WireConnection;44;1;42;0
WireConnection;45;0;44;0
WireConnection;45;1;43;0
WireConnection;25;0;19;0
WireConnection;25;1;31;0
WireConnection;52;0;50;0
WireConnection;52;1;51;0
WireConnection;46;0;25;0
WireConnection;46;1;45;0
WireConnection;53;0;52;0
WireConnection;53;1;32;0
WireConnection;53;2;23;0
WireConnection;47;0;46;0
WireConnection;47;1;45;0
WireConnection;55;0;53;0
WireConnection;55;1;54;0
WireConnection;58;0;47;0
WireConnection;0;0;57;0
WireConnection;0;2;58;0
WireConnection;0;9;44;0
WireConnection;0;11;55;0
ASEEND*/
//CHKSM=53C44CE07A411169FF43299526C6B158A3EB191D