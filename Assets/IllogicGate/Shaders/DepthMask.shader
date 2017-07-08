Shader "Custom/Depth Mask" 
{
	SubShader
	{
		// draw after all transparent objects (3001)
		Tags{ "Queue" = "Transparent+1" }
		Pass
		{
			Blend Zero One // keep the image behind it
		}
	}
}