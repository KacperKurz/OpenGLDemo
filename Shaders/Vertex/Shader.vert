#version 330 core
layout (location = 0) in vec4 aPos;
layout (location = 1) in vec3 aTexCoord;
layout (location = 2) in vec3 aNormal;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec3 aBitangent;
out vec3 TexCoord;
out vec3 Normal;
uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
uniform vec3 lightPos;
uniform vec3 viewPos;
uniform float time;
out vec3 FragPos;
out mat3 TBN;
out vec3 TangentLightPos;
out vec3 TangentViewPos;
out vec3 TangentFragPos;


void main()
{
    FragPos = vec3(model * aPos);
    TexCoord = aTexCoord;
    Normal = aNormal;
    vec3 sunPos = vec3(sin(time/10.0f),1.0f,cos(time/10.0f));
    // note that we read the multiplication from right to left
    gl_Position = projection*view*model*aPos;
    vec3 T = normalize(vec3(model * vec4(aTangent,   0.0)));
    vec3 B = normalize(vec3(model * vec4(aBitangent, 0.0)));
    vec3 N = normalize(vec3(model * vec4(aNormal,    0.0)));
    TBN = transpose(mat3(T, B, N));
    TangentLightPos = TBN * sunPos;
    TangentViewPos = TBN * viewPos;
    TangentFragPos = TBN * FragPos;
}