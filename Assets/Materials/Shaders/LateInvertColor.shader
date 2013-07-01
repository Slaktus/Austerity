Shader "VOL/LateInvertColor" {
Properties {
_Color ("Main Color", Color) = (1,1,1,1)
  
}

Category {
 Tags { "Queue"="Transparent+10" "IgnoreProjector"="True" "RenderType"="Transparent" }
 	Blend OneMinusDstColor OneMinusSrcColor  

 	Cull Back 
 	Lighting Off 
 	ZWrite Off 
  
  
  
  
 SubShader {
 	Pass {
 	 ZTest LEqual
 	 Color [_Color]
 	 } 
 }
}
}