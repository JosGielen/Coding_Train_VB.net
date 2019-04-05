Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D
Imports GlmNet
Imports SharpGL
Imports SharpGL.SceneGraph
Imports SharpGL.Shaders
Imports System.IO
Imports System.Windows.Input
Imports System.Reflection

Public Class GLScene
    Public Const positionAttribute As UInteger = 0
    Public Const normalAttribute As UInteger = 1
    Public Const colorAttribute As UInteger = 2
    Public Const textureAttribute As UInteger = 3
    'OpenGL data
    Private my_gl As OpenGL
    Private my_attributeLocations As Dictionary(Of UInteger, String)
    Private my_VertexShader As String
    Private my_FragmentShader As String
    Private my_Shader As ShaderProgram
    Private my_VertexShaderFile As String
    Private my_FragmentShaderFile As String
    Private my_UpdateShader As Boolean
    Private viewMatrix As mat4
    Private projectionMatrix As mat4
    'Scene data
    Public SceneIsLoaded As Boolean
    Private my_Background As Color
    Private my_Geometries As List(Of GLGeometry)
    Private my_GenerateGeometries As Boolean
    Private my_Lights As List(Of GLLight)
    Private my_UpdateLights As Boolean
    Private my_LightCount As Single
    Private my_Camera As GLCamera
    Private my_ShowAxes As Boolean
    Private my_axes As AxisGeometry
    Private my_ShowGrid As Boolean
    Private my_Grid As GridGeometry
    'Camera Mouse control
    Private MousebuttonDown As Boolean
    Private MouseSensitivity As Double
    Private MouseStartPos As Point
    Private MouseEndPos As Point
    'Camera positioning
    Private CamStartPos As Point3D
    Private CamStartTarget As Point3D
    Private CamStartUpDir As Vector3D

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        my_UpdateShader = False
        my_GenerateGeometries = False
        my_Geometries = New List(Of GLGeometry)
        my_LightCount = 0
        my_UpdateLights = False
        my_Lights = New List(Of GLLight)
        'Set some Lights around the cube 
        Dim l1 As GLLight = New GLLight(LightType.DirectionalLight) With
        {
            .Direction = New Vector3D(50.0, 0.0, 50.0),
            .Ambient = Color.FromRgb(255, 255, 255),
            .Diffuse = Color.FromRgb(255, 255, 255),
            .Specular = Color.FromRgb(200, 200, 200)
        }
        AddLight(l1)
        Dim l2 As GLLight = New GLLight(LightType.DirectionalLight) With
        {
            .Direction = New Vector3D(50.0, 0.0, -50.0),
            .Ambient = Color.FromRgb(255, 255, 255),
            .Diffuse = Color.FromRgb(255, 255, 255),
            .Specular = Color.FromRgb(200, 200, 200)
        }
        AddLight(l2)
        Dim l3 As GLLight = New GLLight(LightType.DirectionalLight) With
        {
            .Direction = New Vector3D(-50.0, 0.0, 50.0),
            .Ambient = Color.FromRgb(255, 255, 255),
            .Diffuse = Color.FromRgb(255, 255, 255),
            .Specular = Color.FromRgb(200, 200, 200)
        }
        AddLight(l3)
        Dim l4 As GLLight = New GLLight(LightType.DirectionalLight) With
        {
            .Direction = New Vector3D(-50.0, 0.0, -50.0),
            .Ambient = Color.FromRgb(255, 255, 255),
            .Diffuse = Color.FromRgb(255, 255, 255),
            .Specular = Color.FromRgb(200, 200, 200)
        }
        AddLight(l4)
        my_Background = Colors.Black
        'Set the default camera
        CamStartPos = New Point3D(0.0, 0.0, 250.0)
        CamStartTarget = New Point3D(0.0, 0.0, 0.0)
        CamStartUpDir = New Vector3D(0.0, 50.0, 0.0)
        my_Camera = New TrackballCamera() With
        {
            .Position = CamStartPos,
            .TargetPosition = CamStartTarget,
            .UpDirection = CamStartUpDir
        }
        ' my_Camera = New FixedCamera(CamStartPos, CamStartTarget, CamStartUpDir)
        my_ShowAxes = False
        my_ShowGrid = False
        my_axes = New AxisGeometry()
        my_Grid = New GridGeometry()
        MousebuttonDown = False
        MouseSensitivity = 0.5
        SceneIsLoaded = False
    End Sub

    Private Sub UserControl_Loaded(sender As Object, e As Windows.RoutedEventArgs)
        'Set Modern GL mode.
        Try
            If Double.Parse(OpenGLCtrl.OpenGL.Version.Substring(0, 3)) < 4.0 Then
                OpenGLCtrl.OpenGLVersion = Version.OpenGLVersion.OpenGL4_0
            End If
            If OpenGLCtrl.RenderContextType <> RenderContextType.FBO Then
                OpenGLCtrl.RenderContextType = RenderContextType.FBO
            End If
            OpenGLCtrl.FrameRate = 60
            OpenGLCtrl.OnApplyTemplate()
            'Set the default OpenGLControl handlers to prevent SharpGL error messages.
            AddHandler OpenGLCtrl.OpenGLInitialized, AddressOf GLControl_OpenGLInitialized
            AddHandler OpenGLCtrl.OpenGLDraw, AddressOf GLControl_OpenGLDraw
            AddHandler OpenGLCtrl.Resized, AddressOf GLControl_Resized
        Catch ex As Exception
            MessageBox.Show("WPF.OpenGLControl settings caused exception: " & ex.Message, "GLScene Error.", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
        my_gl = OpenGLCtrl.OpenGL
        'Specify the attribute locations
        my_attributeLocations = New Dictionary(Of UInteger, String) From
        {
            {positionAttribute, "Position"},
            {normalAttribute, "Normal"},
            {colorAttribute, "Color"},
            {textureAttribute, "Texture"}
        }
        'Check version before creating shaders.
        If Double.Parse(my_gl.Version.Substring(0, 3)) < 2.0 Then
            Throw New Exception("OpenGL version " & Double.Parse(my_gl.Version.Substring(0, 3)) & " does not allow use of shaders.")
            Exit Sub
        End If
        'Create the shaderProgram.
        my_Shader = New ShaderProgram()
        If Not my_UpdateShader Then  'No Shaders were defined so use the default shaders
            LoadDefaultShaders()
            UpdateShaders()
        End If
        CreateProjectionMatrix(0.45F, ActualWidth, ActualHeight, 1, 300)
        If my_UpdateLights Then
            UpdateLights()
            my_UpdateLights = False
        End If
        'Set the scene background color.
        If Background IsNot Nothing Then
            my_Background = ColorConverter.ConvertFromString(Background.ToString())
            my_gl.ClearColor(my_Background.ScR, my_Background.ScG, my_Background.ScB, my_Background.ScA)
        End If
        'Show the Axes and Grid if needed
        If my_ShowAxes Then
            AddGeometry(my_axes)
        End If
        If my_ShowGrid Then
            AddGeometry(my_Grid)
        End If
        'Create a Splash Screen if no other geometries are visible.
        If my_Geometries.Count = 0 Then
            Dim b1 As TextGeometry = New TextGeometry("OpenGL WPF control.") With
            {
                .Position = New Vector3D(0, 0, 0),
                .Width = 6,
                .ForeColor = Colors.Lime
            }
            Dim b2 As TextGeometry = New TextGeometry("Based on SharpGL by D. Kerr.") With
            {
                .Position = New Vector3D(0, -2, 0),
                .Width = 4,
                .ForeColor = Colors.White
            }
            AddGeometry(b1)
            AddGeometry(b2)
        End If
        SceneIsLoaded = True
    End Sub

#Region "Properties"

    Public ReadOnly Property Geometries As List(Of GLGeometry)
        Get
            Return my_Geometries
        End Get
    End Property

    Public ReadOnly Property Lights As List(Of GLLight)
        Get
            Return my_Lights
        End Get
    End Property

    Public ReadOnly Property AttributeLocations As Dictionary(Of UInteger, String)
        Get
            Return my_attributeLocations
        End Get
    End Property

    Public Property Camera As GLCamera
        Get
            Return my_Camera
        End Get
        Set(value As GLCamera)
            my_Camera = value
            CamStartPos = value.Position
            CamStartTarget = value.TargetPosition
            CamStartUpDir = value.UpDirection
        End Set
    End Property

    Public ReadOnly Property GL As OpenGL
        Get
            Return my_gl
        End Get
    End Property

    Public ReadOnly Property VertexShader As String
        Get
            Return my_VertexShader
        End Get
    End Property

    Public ReadOnly Property FragmentShader As String
        Get
            Return my_FragmentShader
        End Get
    End Property

    Public Property ShowAxes As Boolean
        Get
            Return my_ShowAxes
        End Get
        Set(value As Boolean)
            my_ShowAxes = value
        End Set
    End Property

    Public ReadOnly Property Axes As AxisGeometry
        Get
            Return my_axes
        End Get
    End Property

    Public Property ShowGrid As Boolean
        Get
            Return my_ShowGrid
        End Get
        Set(value As Boolean)
            my_ShowGrid = value
        End Set
    End Property

    Public ReadOnly Property Grid As GridGeometry
        Get
            Return my_Grid
        End Get
    End Property

#End Region

#Region "Camera Control"

    Private Sub UserControl_MouseDown(sender As Object, e As Windows.Input.MouseButtonEventArgs)
        MouseStartPos = e.GetPosition(OpenGLCtrl)
        MousebuttonDown = True
    End Sub

    Private Sub UserControl_MouseMove(sender As Object, e As Windows.Input.MouseEventArgs)
        If MousebuttonDown Then
            MouseEndPos = e.GetPosition(OpenGLCtrl)
            Select Case Camera.Type
                Case CameraType.Fixed
                    'Do nothing
                Case CameraType.ParentControlled
                    'Must be implemented in the Parent window.
                Case CameraType.FreeFlying
                    Camera.Horizontal(0.5 * MouseSensitivity * (MouseStartPos - MouseEndPos).X)
                    Camera.Vertical(0.5 * MouseSensitivity * (MouseStartPos - MouseEndPos).Y)
                Case CameraType.Trackball
                    Camera.Horizontal(MouseSensitivity * (MouseEndPos - MouseStartPos).X)
                    Camera.Vertical(MouseSensitivity * (MouseStartPos - MouseEndPos).Y)
            End Select
            MouseStartPos = MouseEndPos
        End If
    End Sub

    Private Sub UserControl_MouseUp(sender As Object, e As Windows.Input.MouseButtonEventArgs)
        MousebuttonDown = False
    End Sub

    Private Sub UserControl_MouseWheel(sender As Object, e As Windows.Input.MouseWheelEventArgs)
        Dim amount As Double = 0.02 * MouseSensitivity * e.Delta
        Select Case Camera.Type
            Case CameraType.Fixed
                    'Do nothing
            Case CameraType.ParentControlled
                    'Must be implemented in the Parent window.
            Case CameraType.FreeFlying
                Camera.Forward(amount)
            Case CameraType.Trackball
                Camera.Forward(amount)
        End Select
    End Sub

    Private Sub UserControl_KeyDown(sender As Object, e As Windows.Input.KeyEventArgs)
        Select Case Camera.Type
            Case CameraType.Fixed
                    'Do nothing
            Case CameraType.ParentControlled
                    'Must be implemented in the Parent window.
            Case CameraType.FreeFlying
                Select Case e.Key
                    Case Key.Up
                        Camera.Forward(Camera.MoveSpeed)
                    Case Key.Down
                        Camera.Forward(-1 * Camera.MoveSpeed)
                    Case Key.Right
                        Camera.Horizontal(-1 * Camera.MoveSpeed)
                    Case Key.Left
                        Camera.Horizontal(Camera.MoveSpeed)
                End Select
            Case CameraType.Trackball
                Select Case e.Key
                    Case Key.Up
                        Camera.Vertical(Camera.MoveSpeed)
                    Case Key.Down
                        Camera.Vertical(-1 * Camera.MoveSpeed)
                    Case Key.Right
                        Camera.Horizontal(Camera.MoveSpeed)
                    Case Key.Left
                        Camera.Horizontal(-1 * Camera.MoveSpeed)
                End Select
        End Select
    End Sub

    Private Sub UserControl_KeyUp(sender As Object, e As Windows.Input.KeyEventArgs)
        Select Case e.Key
            Case Key.Escape 'Reset the camera.
                Camera.Position = CamStartPos
                Camera.TargetPosition = CamStartTarget
                Camera.UpDirection = CamStartUpDir
        End Select
    End Sub

#End Region

#Region "Scene modification"

    ''' <summary>
    ''' Adds a geometry description to the Scene. 
    ''' <para>All the geometries in the scene are re-created at the next render pass.</para>
    ''' </summary>
    ''' <param name="geo">A geometry to be added to the scene</param>
    Public Sub AddGeometry(geo As GLGeometry)
        my_Geometries.Add(geo)
        my_GenerateGeometries = True
    End Sub

    ''' <summary>
    ''' Creates the actual geometry objects that are in the scene.
    ''' <para>This is called automatic at the next render pass after adding a geometry to the scene.</para>
    ''' </summary>
    Public Sub GenerateGeometries()
        'Set the scene background color.
        If Background IsNot Nothing Then
            my_Background = ColorConverter.ConvertFromString(Background.ToString())
        End If
        my_gl.ClearColor(my_Background.ScR, my_Background.ScG, my_Background.ScB, my_Background.ScA)
        Dim parentWindow As Window = Window.GetWindow(Me)
        AddHandler parentWindow.PreviewKeyDown, AddressOf UserControl_KeyDown
        AddHandler parentWindow.PreviewKeyUp, AddressOf UserControl_KeyUp
        If my_ShowAxes And Not my_Geometries.Contains(my_axes) Then
            AddGeometry(my_axes)
        End If
        If my_Geometries.Contains(my_axes) And Not my_ShowAxes Then
            Geometries.Remove(my_axes)
        End If
        If my_ShowGrid And Not my_Geometries.Contains(my_Grid) Then
            AddGeometry(my_Grid)
        End If
        If my_Geometries.Contains(my_Grid) And Not my_ShowGrid Then
            Geometries.Remove(my_Grid)
        End If
        For Each geo As GLGeometry In my_Geometries
            geo.GenerateGeometry(Me)
        Next
    End Sub

    ''' <summary>
    ''' Removes all Geometries from the scene (except the axes if they are set).
    ''' </summary>
    Public Sub ClearGeometries()
        my_Geometries.Clear()
        If my_ShowAxes Then
            AddGeometry(my_axes)
        End If
        If my_ShowGrid Then
            AddGeometry(my_Grid)
        End If
    End Sub

    ''' <summary>
    ''' Adds a Light to the scene.
    ''' <para>All the Lights parameters are set on the fragment shader at the next render pass.</para>
    ''' </summary>
    ''' <param name="light"></param>
    Public Sub AddLight(light As GLLight)
        light.Index = my_LightCount
        my_Lights.Add(light)
        my_LightCount += 1
        my_UpdateLights = True
    End Sub

    ''' <summary>
    ''' Sets all the Lights parameters on the fragment shader.
    ''' <para>This is called automatic at the next render pass after adding a Light to the scene.</para>
    ''' </summary>
    Public Sub UpdateLights()
        'Set the lighting parameters in the Schader
        my_Shader.SetUniform1(my_gl, "lightCount", my_LightCount)
        For Each l As GLLight In my_Lights
            l.SetLightData(my_gl, Me, my_Shader)
        Next
    End Sub

    ''' <summary>
    ''' Loads the Shader files into memmory.
    ''' <para>The ProgramShader is created at the next render pass.</para>
    ''' <para>This replaces the default Shaders used by the GLScene.</para>
    ''' </summary>
    ''' <param name="VertexShaderFile">Path of the Vertex Shader file.</param>
    ''' <param name="FragmentShaderFile">Path of the Fragment Shader file.</param>
    Public Sub SetShaders(VertexShaderFile As String, FragmentShaderFile As String)
        Try
            my_VertexShader = LoadTextFile(VertexShaderFile)
            my_FragmentShader = LoadTextFile(FragmentShaderFile)
            my_UpdateShader = True
        Catch ex As Exception
            MessageBox.Show("SetShaders was unable to load the shaders : " & ex.Message, "GLScene Error.", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Creates the ShaderProgram
    ''' <para>This is called automatic at the next render pass after setting Shaders.</para>
    ''' </summary>
    Public Sub UpdateShaders()
        If Double.Parse(my_gl.Version.Substring(0, 3)) < 2.0 Then
            Throw New Exception("OpenGL version " & Double.Parse(my_gl.Version.Substring(0, 3)) & " does not allow use of shaders.")
        End If
        'Create the shaderProgram.
        Try
            my_Shader = New ShaderProgram()
            my_Shader.Create(my_gl, my_VertexShader, my_FragmentShader, AttributeLocations)
            my_Shader.Bind(my_gl)
            my_UpdateShader = False
        Catch ex As Exception
            MessageBox.Show("Unable to update the shaders : " & ex.Message, "GLScene Error.", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Renders the scene. This must be called from the Parent window in a draw loop.
    ''' </summary>
    Public Sub Render()
        'Clear the buffers
        my_gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT Or OpenGL.GL_DEPTH_BUFFER_BIT Or OpenGL.GL_STENCIL_BUFFER_BIT)
        If my_UpdateLights Then
            UpdateLights()
            my_UpdateLights = False
        End If
        If my_GenerateGeometries Then
            GenerateGeometries()
            my_GenerateGeometries = False
        End If
        If my_UpdateShader Then
            UpdateShaders()
            my_UpdateShader = False
        End If
        'Create the view matrix each render pass to allow camera movement.
        viewMatrix = my_Camera.GetViewMatrix()
        my_Shader.SetUniform3(my_gl, "viewPos", my_Camera.X, my_Camera.Y, my_Camera.Z)
        my_Shader.SetUniformMatrix4(my_gl, "View", viewMatrix.to_array())
        my_Shader.SetUniformMatrix4(my_gl, "Projection", projectionMatrix.to_array())
        'Draw the Geometry
        For Each geo As GLGeometry In my_Geometries
            geo.Draw(my_Shader)
        Next
    End Sub

#End Region

#Region "Utilities"

    Private Sub CreateProjectionMatrix(fov As Double, width As Double, height As Double, near As Double, far As Double)
        Dim H As Single = fov * height / width
        projectionMatrix = glm.pfrustum(-fov, fov, -H, H, near, far)
    End Sub

    Private Sub LoadDefaultShaders()
        Try
            Dim _assembly As [Assembly] = [Assembly].GetExecutingAssembly()
            Dim reader As StreamReader = New StreamReader(_assembly.GetManifestResourceStream("RubiksCube.PerPixel.vert"))
            Using reader
                my_VertexShader = reader.ReadToEnd()
            End Using
            reader = New StreamReader(_assembly.GetManifestResourceStream("RubiksCube.PerPixel.frag"))
            Using reader
                my_FragmentShader = reader.ReadToEnd()
            End Using
            my_UpdateShader = True
        Catch ex As Exception
            MessageBox.Show("Unable to load the Default Shaders : " & ex.Message, "GLScene Error.", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Shared Function LoadTextFile(textFileName As String) As String
        Dim result As String = ""
        Dim reader As StreamReader = New StreamReader(textFileName)
        Using reader
            result = reader.ReadToEnd()
        End Using
        Return result
    End Function

    Private Sub GLControl_OpenGLInitialized(sender As Object, args As OpenGLEventArgs)
        'Does nothing. Used to prevent SharpGL error messages.
    End Sub

    Private Sub GLControl_OpenGLDraw(sender As Object, args As OpenGLEventArgs)
        'Does nothing. Used to prevent SharpGL error messages.
    End Sub

    Private Sub GLControl_Resized(sender As Object, args As OpenGLEventArgs)
        CreateProjectionMatrix(0.45F, ActualWidth, ActualHeight, 1, 300)
    End Sub

#End Region

End Class
