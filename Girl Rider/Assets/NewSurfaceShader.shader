Shader "Custom/InvisibleMask" {
	SubShader{
		// draw after all opaque objects (queue = 2001):"Queue" = "Geometry+1" "IgnoreProjector" = "false" "RenderType" = "Transparent" 

		//Tags { "Queue" = "Geometry+1" "RenderType" = "TransparentCutout"}


		//Tags { "Queue" = "Geometry+1" "IgnoreProjector" = "false" "RenderType" = "Transparent" }


		Tags { "Queue" = "Geometry-10" }


		ColorMask 0
		ZWrite On
		ZTest Always


		Pass {

			Blend Zero One // keep the image behind it

		}
	}

}