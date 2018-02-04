#version 330

uniform vec2 iResolution; //[xResolution, yResolution] of display
uniform float iGlobalTime; //global time in seconds as float
	
void main()
{
	//create uv to be in the range [0..1]x[0..1]
	vec2 uv = gl_FragCoord.xy / iResolution;
	vec2 mouse = iMouse.xy / iResolution;

	//4 component color red, green, blue, alpha
	vec4 color = vec4(uv, 0, 1);
	color.b = sin(iGlobalTime);

	gl_FragColor = color;
}