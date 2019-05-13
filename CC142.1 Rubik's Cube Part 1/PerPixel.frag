#version 330 core

out vec4 FragColor;

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
}; 

struct MyLight {
    float type;
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;
    float constant;
    float linear;
    float quadratic;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

#define NR_OF_LIGHTS 9

in vec3 FragPos;
in vec3 Normal;
in vec3 VertColor;
in vec2 TexCoord;

uniform float lightCount;
uniform vec3 viewPos;
uniform MyLight lights[NR_OF_LIGHTS];
uniform Material material;
uniform sampler2D ourTexture;

// function prototypes
vec3 CalcDirLight(MyLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(MyLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(MyLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
	vec3 norm = normalize(Normal);
	vec3 viewDir = normalize(viewPos - FragPos);
	vec3 result = VertColor;
	for(int i = 0; i < lightCount; i++) {
		if(lights[i].type == 1.0)
			result += CalcDirLight(lights[i], norm, viewDir);
		if(lights[i].type == 2.0)
			result += CalcPointLight(lights[i], norm, FragPos, viewDir); 
		if(lights[i].type == 3.0)
			result += CalcSpotLight(lights[i], norm, FragPos, viewDir);   
	}
	FragColor = vec4(result, 1.0) + texture(ourTexture, TexCoord);
}

// calculates the color when using a directional light.
vec3 CalcDirLight(MyLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // combine results
    vec3 ambient = light.ambient * material.ambient;
    vec3 diffuse = light.diffuse * diff * material.diffuse;
    vec3 specular = light.specular * spec * material.specular;
    return (ambient + diffuse + specular);
}

// calculates the color when using a point light.
vec3 CalcPointLight(MyLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
    // combine results
    vec3 ambient = light.ambient * material.ambient;
    vec3 diffuse = light.diffuse * diff * material.diffuse;
    vec3 specular = light.specular * spec * material.specular;
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}

// calculates the color when using a spot light.
vec3 CalcSpotLight(MyLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
    // spotlight intensity
    float theta = dot(lightDir, normalize(-light.direction)); 
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    // combine results
    vec3 ambient = light.ambient * material.ambient;
    vec3 diffuse = light.diffuse * diff * material.diffuse;
    vec3 specular = light.specular * spec * material.specular;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
}