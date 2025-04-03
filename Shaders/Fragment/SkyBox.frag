#version 330 core

in vec4 fragPos;
out vec4 outColor;

uniform samplerCube texture0;

void main()
{
    outColor = texture(texture0, fragPos.xyz);
}
