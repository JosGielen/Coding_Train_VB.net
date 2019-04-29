#version 330

in vec3 aPosition;
in vec3 aNormal;
in vec3 aColor;
in vec2 aTexCoord;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;
uniform mat3 NormalMatrix;

out vec3 FragPos;
out vec3 Normal;
out vec3 VertColor;
out vec2 TexCoord;

void main()
{
    FragPos = vec3(Model * vec4(aPosition, 1.0));
    Normal = NormalMatrix * aNormal;
    gl_Position = Projection * View * vec4(FragPos, 1.0);
    VertColor = aColor;
    TexCoord = aTexCoord;
}