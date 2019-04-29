Imports SharpGL
Imports System.Windows.Media.Media3D

Class MainWindow
    Private App_Loaded As Boolean = False
    Private my_scene As GLScene
    Private Sponge As List(Of Box)
    Private geo As UserGeometry
    'Camera positioning
    Private CamStartPos As Vector3D = New Vector3D(0.0, 0.0, 40.0)
    Private CamStartDir As Vector3D = New Vector3D(0.0, 0.0, -1.0)
    'Camera Mouse control
    Private MousebuttonDown As Boolean = False
    Private MouseSensitivity As Double = 0.5
    Private MouseStartPos As Point
    Private MouseEndPos As Point
    'FPS data
    Private ShowFPS As Boolean = False
    Private LastRenderTime As Date
    Private Framecounter As Integer

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        my_scene = New GLScene(OpenGLC1)
        my_scene.Camera.Position = New Vector3D(0.0, 0.0, 40.0)
        my_scene.Camera.MoveSpeed = 0.2
        my_scene.Lights.Clear()
        Dim l As GLLight = New GLLight(LightType.PointLight) With
            {
            .Ambient = Colors.LightGray,
            .Diffuse = Colors.LightGray,
            .Specular = Colors.White,
            .Position = New Vector3D(0, 0, 0),
            .Constant = 1,
            .Linear = 0.0,
            .Quadratic = 0.0
            }
        my_scene.AddLight(l)
        l = New GLLight(LightType.DirectionalLight) With
            {
            .Ambient = Colors.LightGray,
            .Diffuse = Colors.White,
            .Specular = Colors.White,
            .Direction = New Vector3D(1, 2, 2),
            .Constant = 0.1,
            .Linear = 0.0,
            .Quadratic = 0.0
            }
        my_scene.AddLight(l)
        my_scene.Initialise(Environment.CurrentDirectory & "\PerPixel.vert", Environment.CurrentDirectory & "\PerPixel.frag")
        'Do the program initialisation
        Sponge = New List(Of Box)
        Sponge.Add(New Box(New Point3D(0.0, 0.0, 0.0), 10.0))
        CreateGeometry()
        'Initialize FPS counter
        LastRenderTime = Now()
        Framecounter = 0
        App_Loaded = True
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        If Not App_Loaded Then Exit Sub
        If ShowFPS Then
            Framecounter += 1
            If Framecounter = 25 Then
                Dim fps As Double = CInt(25000 / (Now - LastRenderTime).TotalMilliseconds)
                Me.Title = "FPS = " & fps.ToString
                LastRenderTime = Now()
                Framecounter = 0
            End If
        Else
            Me.Title = "VertexCount = " & my_scene.Geometry(0).Vertices.Count.ToString()
        End If
        'Render the scene.
        my_scene.Render()
    End Sub

    Private Sub CreateGeometry()
        my_scene.Geometry.Clear()
        geo = New UserGeometry() With
        {
            .AmbientMaterial = Color.FromRgb(100, 90, 25),
            .DiffuseMaterial = Color.FromRgb(200, 180, 50),
            .SpecularMaterial = Color.FromRgb(255, 255, 255),
            .Shininess = 50,
            .Position = New Vector3D(0.0, 0.0, 0.0),
            .DrawMode = DrawMode.Fill
        }
        'Add the vertices and normals of each box to the UserGeometry
        For Each b As Box In Sponge
            geo.UserIndices.AddRange(b.getIndices(geo.UserVertices.Count))
            geo.UserVertices.AddRange(b.GetVertices())
            geo.UserNormals.AddRange(b.GetNormals)
        Next
        my_scene.AddGeometry(geo)
    End Sub

    Private Sub Window_MouseDown(sender As Object, e As MouseButtonEventArgs)
        MouseStartPos = e.GetPosition(OpenGLC1)
        MousebuttonDown = True
    End Sub

    Private Sub Window_MouseMove(sender As Object, e As MouseEventArgs)
        'Camera look (move) direction change
        If MousebuttonDown Then
            MouseEndPos = e.GetPosition(OpenGLC1)
            my_scene.Camera.Yaw += MouseSensitivity * (MouseEndPos - MouseStartPos).X
            my_scene.Camera.Pitch -= MouseSensitivity * (MouseEndPos - MouseStartPos).Y
            MouseStartPos = MouseEndPos
        End If
    End Sub

    Private Sub Window_MouseUp(sender As Object, e As MouseButtonEventArgs)
        MousebuttonDown = False
    End Sub

    Private Sub Window_MouseWheel(sender As Object, e As MouseWheelEventArgs)
        'Mouse wheel Forward/Backward movement throught the scene.
        Dim amount As Double = 0.1 * MouseSensitivity * e.Delta
        my_scene.Camera.Forward(amount)
    End Sub

    Private Sub Window_KeyDown(sender As Object, e As KeyEventArgs)
        'Keyboard movement through the Scene
        Select Case e.Key
            Case Key.Up
                my_scene.Camera.Forward(1)
            Case Key.Down
                my_scene.Camera.Forward(-1)
            Case Key.Right
                my_scene.Camera.Yaw += 1
            Case Key.Left
                my_scene.Camera.Yaw -= 1
        End Select
    End Sub

    Private Sub Window_KeyUp(sender As Object, e As KeyEventArgs)
        Select Case e.Key
            Case Key.Escape
                'Reset the camera and target positions.
                my_scene.Camera.Position = CamStartPos
                my_scene.Camera.Direction = CamStartDir
            Case Key.Space
                Dim newSponge As List(Of Box) = New List(Of Box)
                For I As Integer = 0 To Sponge.Count - 1
                    newSponge.AddRange(Sponge(I).Devide())
                Next
                Sponge = newSponge
                CreateGeometry()
        End Select
    End Sub

End Class
