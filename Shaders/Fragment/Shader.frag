#version 330 core
out vec4 FragColor;
in vec3 TexCoord;
in vec3 Normal;
uniform sampler2D texture0;
uniform sampler2D texture1;
uniform sampler2D texture2;
uniform vec3 lightPos;
uniform vec3 viewPos;
uniform int useTexture;
uniform int useNormalMap;
uniform int useSpecularMap;
uniform int useShading;
in vec3 FragPos;
in vec4 VertColor;
uniform mat4 model;
uniform float time;
in mat3 TBN;
in vec3 TangentLightPos;
in vec3 TangentViewPos;
in vec3 TangentFragPos;

const vec3 lightColor = vec3(1.0, 1.0, 1.0);
const float lightPower = 1.0;
const float shininess = 32.0;  // Increased shininess for more visible reflections
vec4 diffuseColor = useTexture==1?texture(texture0,TexCoord.xy):vec4(0.6,0.6,0.6,1.0);
vec4 ambientColor = diffuseColor*0.1;
vec3 specColor = diffuseColor.xyz;

const float screenGamma = 2.2;

// Debug visualization modes
const int DEBUG_NONE = 0;
const int DEBUG_NORMAL = 1;
const int DEBUG_SPECULAR_MAP = 2;
const int DEBUG_VIEW_DIR = 3;
const int DEBUG_LIGHT_DIR = 4;
const int DEBUG_HALF_DIR = 5;
const int DEBUG_SPECULAR_TERM = 6;
const int DEBUG_SPECULAR_MAP_BLUE = 8;

uniform int debugMode = DEBUG_NONE;

void main() {
    vec3 sunPos = vec3(sin(time/10.0f),1.0f,cos(time/10.0f));
    if (diffuseColor.a==0.0) discard;
    
    if (useShading==1){
        vec3 normal;
        vec3 lightDir;
        vec3 viewDir;
        vec3 halfDir;
        float lambertian;
        float specular = 0.0;
        vec4 specularMap = vec4(0.0);
        float specAngle = 0.0;

        // Calculate base surface normal (for specular)
        vec3 baseNormal = normalize(transpose(inverse(mat3(model)))*Normal);

        if (useNormalMap==0){
            normal = baseNormal;
            lightDir = normalize(sunPos);
            lambertian = max(dot(lightDir, normal), 0.0);

            if (lambertian > 0.0) {
                viewDir = normalize(viewPos-FragPos);
                halfDir = normalize(lightDir + viewDir);
                specAngle = max(dot(halfDir, normal), 0.0);
                specular = pow(specAngle, shininess);
                
                if (useSpecularMap == 1) {
                    specularMap = texture(texture2, TexCoord.xy);
                    specular *= specularMap.b;  // Use only blue channel
                }
            }
        }
        else{
            // Get normal from normal map for diffuse lighting only
            normal = texture(texture1, TexCoord.xy).rgb;
            normal = normalize(normal * 2.0 - 1.0);
            normal.y = normal.y;  // Flip Y component for correct orientation

            // Calculate light direction in tangent space for diffuse
            lightDir = normalize(-TangentLightPos);  // Negate the light direction
            lambertian = max(dot(lightDir, normal), 0.0);

            if (lambertian > 0.0) {
                // Use world space vectors for specular calculation
                viewDir = normalize(viewPos-FragPos);
                lightDir = normalize(sunPos);  // Use world space light direction for specular
                halfDir = normalize(lightDir + viewDir);
                specAngle = max(dot(halfDir, baseNormal), 0.0);  // Use base normal for specular
                specular = pow(specAngle, shininess);
                
                if (useSpecularMap == 1) {
                    specularMap = texture(texture2, TexCoord.xy);
                    specular *= specularMap.b;  // Use only blue channel
                }
            }
        }

        // Debug visualization
        if (debugMode != DEBUG_NONE) {
            switch(debugMode) {
                case DEBUG_NORMAL:
                    FragColor = vec4(normal * 0.5 + 0.5, 1.0);
                    break;
                case DEBUG_SPECULAR_MAP:
                    FragColor = useSpecularMap == 1 ? specularMap : vec4(0.0);
                    break;
                case DEBUG_VIEW_DIR:
                    FragColor = vec4(viewDir * 0.5 + 0.5, 1.0);
                    break;
                case DEBUG_LIGHT_DIR:
                    FragColor = vec4(lightDir * 0.5 + 0.5, 1.0);
                    break;
                case DEBUG_HALF_DIR:
                    FragColor = vec4(halfDir * 0.5 + 0.5, 1.0);
                    break;
                case DEBUG_SPECULAR_TERM:
                    FragColor = vec4(vec3(specular), 1.0);
                    break;
                case DEBUG_SPECULAR_MAP_BLUE:
                    FragColor = useSpecularMap == 1 ? vec4(vec3(specularMap.b), 1.0) : vec4(0.0);
                    break;
            }
            return;
        }

        vec3 colorLinear = ambientColor.rgb +
        diffuseColor.rgb * lambertian * lightColor * lightPower +
        specColor * specular * lightColor * lightPower;
        vec3 colorGammaCorrected = pow(colorLinear, vec3(1.0 / screenGamma));
        FragColor = vec4(colorLinear, 1.0);
    }
    else{
        FragColor = diffuseColor;
    }
}
