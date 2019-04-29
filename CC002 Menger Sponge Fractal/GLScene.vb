Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D
Imports GlmNet
Imports SharpGL
Imports SharpGL.SceneGraph
Imports SharpGL.Shaders
Imports System.IO

Public Class GLScene
    Public Const positionAttribute As UInteger = 0
    Public Const normalAttribute As UInteger = 1
    Public Const colorAttribute As UInteger = 2
    Public Const textureAttribute As UInteger = 3
    Private my_gl As OpenGL
    Private my_attributeLocations As Dictionary(Of UInteger, String)
    Private Shader As ShaderProgram
    Private viewMatrix As mat4
    Private projectionMatrix As mat4
    'Lighting data
    Private my_Lights As List(Of GLLight)
    Private my_LightCount As Single = 0.0F
    'Camera movement data
    Private my_Camera As GLCamera
    'Scene data
    Private my_Background As Color
    Private geometries As List(Of GLGeometry)

    ''' <summary>
    ''' Creates a new OpenGL scene with a default Camera , a Directional Light and a BoxGeometry.
    ''' <para>Tries to set OpenGL version 4.4 with FBO rendering and 60 FPS.</para>
    ''' </summary>
    ''' <param name="GLControl">The SharpGL.WPF.OpenGLControl used to display the scene</param>
    Public Sub New(GLControl As WPF.OpenGLControl)
        my_gl = GLControl.OpenGL
        'Set Modern GL mode.
        Try
            If Double.Parse(GLControl.OpenGL.Version.Substring(0, 3)) < 4.0 Then
                GLControl.OpenGLVersion = Version.OpenGLVersion.OpenGL4_0
            End If
            If GLControl.RenderContextType <> RenderContextType.FBO Then
                GLControl.RenderContextType = RenderContextType.FBO
            End If
            GLControl.FrameRate = 60
            GLControl.OnApplyTemplate()
            'Set the default OpenGLControl handlers to prevent error messages.
            AddHandler GLControl.OpenGLInitialized, AddressOf GLControl_OpenGLInitialized
            AddHandler GLControl.OpenGLDraw, AddressOf GLControl_OpenGLDraw
            AddHandler GLControl.Resized, AddressOf GLControl_Resized
        Catch ex As Exception
            MessageBox.Show("WPF.OpenGLControl settings caused exception: " & ex.Message, "GLScene Error.", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
        'Specify the attribute locations
        AttributeLocations = New Dictionary(Of UInteger, String) From
        {
            {positionAttribute, "Position"},
            {normalAttribute, "Normal"},
            {colorAttribute, "Color"},
            {textureAttribute, "Texture"}
        }
        'Set default Camera and Light
        CreateProjectionMatrix(0.45F, GLControl.ActualWidth, GLControl.ActualHeight, 1, 300)
        my_Lights = New List(Of GLLight)
        Dim l As GLLight = New GLLight(LightType.DirectionalLight) With
        {
            .Direction = New Vector3D(1.0, 2.0, 2.0),
            .Ambient = Color.FromRgb(255, 255, 255),
            .Diffuse = Color.FromRgb(255, 255, 255),
            .Specular = Color.FromRgb(255, 255, 255)
        }
        AddLight(l)
        my_Camera = New GLCamera(New Vector3D(0, 0, 6), New Vector3D(0, 0, -1), New Vector3D(0, 1, 0)) With
        {
            .MoveSpeed = 0.1
        }
        my_Background = Colors.Black
        geometries = New List(Of GLGeometry)
    End Sub

    ''' <summary>
    ''' Creates a Blank OpenGL Scene without setting any parameters or content.
    ''' </summary>
    Public Sub New()
        geometries = New List(Of GLGeometry)
    End Sub

#Region "Properties"

    Public Property Geometry As List(Of GLGeometry)
        Get
            Return geometries
        End Get
        Set(value As List(Of GLGeometry))
            geometries = value
        End Set
    End Property

    Public Property Background As Color
        Get
            Return my_Background
        End Get
        Set(value As Color)
            my_Background = value
        End Set
    End Property

    Public Property AttributeLocations As Dictionary(Of UInteger, String)
        Get
            Return my_attributeLocations
        End Get
        Set(value As Dictionary(Of UInteger, String))
            my_attributeLocations = value
        End Set
    End Property

    Public Property Camera As GLCamera
        Get
            Return my_Camera
        End Get
        Set(value As GLCamera)
            my_Camera = value
        End Set
    End Property

    Public ReadOnly Property Lights As List(Of GLLight)
        Get
            Return my_Lights
        End Get
    End Property

    Public Property GL As OpenGL
        Get
            Return my_gl
        End Get
        Set(value As OpenGL)
            my_gl = value
        End Set
    End Property

#End Region

    Public Sub Initialise(VertexShaderFile As String, FragmentShaderFile As String)
        If Double.Parse(my_gl.Version.Substring(0, 3)) < 2.0 Then
            Throw New Exception("OpenGL version " & Double.Parse(my_gl.Version.Substring(0, 3)) & " does not allow use of shaders.")
        End If
        'Create the shaderProgram.
        Shader = New ShaderProgram()
        Dim VertShader As String = LoadTextFile(VertexShaderFile)
        Dim FragShader As String = LoadTextFile(FragmentShaderFile)
        Shader.Create(my_gl, VertShader, FragShader, AttributeLocations)
        Shader.Bind(my_gl)
        'Set the scene background color.
        my_gl.ClearColor(my_Background.ScR, my_Background.ScG, my_Background.ScB, my_Background.ScA)
        'Set the lighting parameters in the Schader
        Shader.SetUniform1(my_gl, "lightCount", my_LightCount)
        For Each l As GLLight In my_Lights
            l.SetLightData(my_gl, Me, Shader)
        Next
    End Sub

    Public Sub AddGeometry(geo As GLGeometry)
        geo.GenerateGeometry(Me)
        geometries.Add(geo)
    End Sub

    Public Sub AddLight(light As GLLight)
        light.Index = my_LightCount
        my_Lights.Add(light)
        my_LightCount += 1
    End Sub

    Public Sub SetCamera(cam As GLCamera)
        my_Camera = cam
    End Sub

    Public Sub Render()
        'Clear the buffers
        my_gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT Or OpenGL.GL_DEPTH_BUFFER_BIT Or OpenGL.GL_STENCIL_BUFFER_BIT)
        'Create the view matrix each render pass to allow camera movement.
        viewMatrix = my_Camera.GetViewMatrix()
        Shader.SetUniform3(my_gl, "viewPos", my_Camera.X, my_Camera.Y, my_Camera.Z)
        Shader.SetUniformMatrix4(my_gl, "View", viewMatrix.to_array())
        Shader.SetUniformMatrix4(my_gl, "Projection", projectionMatrix.to_array())
        'Draw the Geometry
        For Each geo As GLGeometry In geometries
            geo.Draw(Shader)
        Next
    End Sub

    Public Sub CreateProjectionMatrix(fov As Double, width As Double, height As Double, near As Double, far As Double)
        Dim H As Single = fov * height / width
        projectionMatrix = glm.pfrustum(-fov, fov, -H, H, near, far)
    End Sub

    Private Shared Function LoadTextFile(textFileName As String) As String
        Dim result As String = ""
        Dim reader As StreamReader = New StreamReader(textFileName)
        Using reader
            result = reader.ReadToEnd()
        End Using
        Return result
    End Function

    Public Sub GLControl_OpenGLInitialized(sender As Object, args As OpenGLEventArgs)
        'Do nothing
    End Sub

    Public Sub GLControl_OpenGLDraw(sender As Object, args As OpenGLEventArgs)
        'Do nothing
    End Sub

    Public Sub GLControl_Resized(sender As Object, args As OpenGLEventArgs)
        'Do nothing
    End Sub

End Class





