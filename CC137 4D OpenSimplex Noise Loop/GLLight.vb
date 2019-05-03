Imports System.Windows.Media
Imports System.Windows.Media.Media3D
Imports SharpGL
Imports SharpGL.Shaders

Public Class GLLight
    Private my_Index As Single
    Private my_SwitchedOn As Boolean
    Private ReadOnly my_type As LightType
    Private my_position As Vector3D
    Private my_direction As Vector3D
    Private my_cutOff As Single
    Private my_outerCutOff As Single
    Private my_constant As Single
    Private my_linear As Single
    Private my_quadratic As Single
    Private my_ambient As Color
    Private my_diffuse As Color
    Private my_specular As Color

    Public Sub New(type As LightType)
        my_Index = 0.0F
        my_SwitchedOn = True
        my_type = type
        my_position = New Vector3D(0, 0, 0)
        my_direction = New Vector3D(0, 0, 0)
        my_cutOff = 0.0F
        my_outerCutOff = 0.0F
        my_constant = 1.0F
        my_linear = 1.0F
        my_quadratic = 1.0F
        my_ambient = Colors.Black
        my_diffuse = Colors.Black
        my_specular = Colors.Black
    End Sub

    Public Property Index As Single
        Get
            Return my_Index
        End Get
        Set(value As Single)
            my_Index = value
        End Set
    End Property

    Public ReadOnly Property Type As LightType
        Get
            Return my_type
        End Get
    End Property

    Public Property Position As Vector3D
        Get
            Return my_position
        End Get
        Set(value As Vector3D)
            my_position = value
        End Set
    End Property

    Public Property Direction As Vector3D
        Get
            Return my_direction
        End Get
        Set(value As Vector3D)
            my_direction = value
        End Set
    End Property

    Public Property CutOff As Double
        Get
            Return my_cutOff
        End Get
        Set(value As Double)
            my_cutOff = CSng(value)
        End Set
    End Property

    Public Property OuterCutOff As Double
        Get
            Return my_outerCutOff
        End Get
        Set(value As Double)
            my_outerCutOff = CSng(value)
        End Set
    End Property

    Public Property Constant As Double
        Get
            Return my_constant
        End Get
        Set(value As Double)
            my_constant = CSng(value)
        End Set
    End Property

    Public Property Linear As Double
        Get
            Return my_linear
        End Get
        Set(value As Double)
            my_linear = CSng(value)
        End Set
    End Property

    Public Property Quadratic As Double
        Get
            Return my_quadratic
        End Get
        Set(value As Double)
            my_quadratic = CSng(value)
        End Set
    End Property

    Public Property Ambient As Color
        Get
            Return my_ambient
        End Get
        Set(value As Color)
            my_ambient = value
        End Set
    End Property

    Public Property Diffuse As Color
        Get
            Return my_diffuse
        End Get
        Set(value As Color)
            my_diffuse = value
        End Set
    End Property

    Public Property Specular As Color
        Get
            Return my_specular
        End Get
        Set(value As Color)
            my_specular = value
        End Set
    End Property

    Public Property SwitchedOn As Boolean
        Get
            Return my_SwitchedOn
        End Get
        Set(value As Boolean)
            my_SwitchedOn = value
        End Set
    End Property

    Public Sub SetLightData(gl As OpenGL, scene As GLScene, Shader As ShaderProgram)
        If my_SwitchedOn Then
            Shader.SetUniform1(gl, "lights[" & my_Index.ToString() & "].type", my_type)
            Shader.SetUniform3(gl, "lights[" & my_Index.ToString() & "].position", CSng(my_position.X), CSng(my_position.Y), CSng(my_position.Z))
            Shader.SetUniform3(gl, "lights[" & my_Index.ToString() & "].direction", CSng(my_direction.X), CSng(my_direction.Y), CSng(my_direction.Z))
            Shader.SetUniform1(gl, "lights[" & my_Index.ToString() & "].cutOff", CSng(Math.Cos(my_cutOff * Math.PI / 180)))
            Shader.SetUniform1(gl, "lights[" & my_Index.ToString() & "].outerCutOff", CSng(Math.Cos(my_outerCutOff * Math.PI / 180)))
            Shader.SetUniform1(gl, "lights[" & my_Index.ToString() & "].constant", my_constant)
            Shader.SetUniform1(gl, "lights[" & my_Index.ToString() & "].linear", my_linear)
            Shader.SetUniform1(gl, "lights[" & my_Index.ToString() & "].quadratic", my_quadratic)
            Shader.SetUniform3(gl, "lights[" & my_Index.ToString() & "].ambient", my_ambient.ScR, my_ambient.ScG, my_ambient.ScB)
            Shader.SetUniform3(gl, "lights[" & my_Index.ToString() & "].diffuse", my_diffuse.ScR, my_diffuse.ScG, my_diffuse.ScB)
            Shader.SetUniform3(gl, "lights[" & my_Index.ToString() & "].specular", my_specular.ScR, my_specular.ScG, my_specular.ScB)
        Else
            Shader.SetUniform1(gl, "lights[" & my_Index.ToString() & "].type", 0.0F)
        End If
    End Sub

End Class

Public Enum LightType As Integer
    ''' <summary>
    ''' The socket is Empty
    ''' </summary>
    NoLight = 0
    ''' <summary>
    ''' e.g. A beam of sunlight
    ''' </summary>
    DirectionalLight = 1
    ''' <summary>
    ''' e.g. A light bulb
    ''' </summary>
    PointLight = 2
    ''' <summary>
    ''' e.g. A halogen spot
    ''' </summary>
    SpotLight = 3
End Enum
