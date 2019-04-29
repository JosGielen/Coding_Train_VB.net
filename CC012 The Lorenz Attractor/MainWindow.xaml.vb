Imports System.Windows.Media.Media3D
Imports My_GL

Class MainWindow

    Private App_Loaded As Boolean = False
    Private my_scene As GLScene
    Private my_Polyline As PolyLineGeometry
    Private palette As ColorPalette
    'Lorenz Attractor data
    Private Sigma As Double = 10.0
    Private Rho As Double = 28.0
    Private Beta As Double = 8.0 / 3.0
    Private X As Double = -3.0
    Private Y As Double = 1.0
    Private Z As Double = 20.0
    Private dX As Double = 0.0
    Private dY As Double = 0.0
    Private dZ As Double = 0.0
    Private dt As Double = 0.01
    Private points As List(Of Point3D) = New List(Of Point3D)
    'Camera positioning
    Private CamStartPos As Vector3D = New Vector3D(0.0, 0.0, 5.0)
    Private CamStartDir As Vector3D = New Vector3D(0.0, 0.0, -1.0)
    'Camera Mouse control
    Private MousebuttonDown As Boolean = False
    Private MouseSensitivity As Double = 0.5
    Private MouseStartPos As Point
    Private MouseEndPos As Point
    'FPS data
    Private LastRenderTime As Date
    Private Framecounter As Integer

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        my_scene = New GLScene(OpenGLC1)
        my_scene.Geometry.Clear()
        palette = New ColorPalette(Environment.CurrentDirectory & "\Rainbow continuous.cpl")
        my_Polyline = New PolyLineGeometry(3.0, 3.0, 3.0) With
        {
            .DrawMode = DrawMode.Fill,
            .PointSize = 5.0,
            .LineWidth = 5.0,
            .RotationAxis = New Vector3D(0, 1, 0),
            .RotationSpeed = 0.005
        }
        my_scene.Initialise(Environment.CurrentDirectory & "\PerPixel.vert", Environment.CurrentDirectory & "\PerPixel.frag")
        my_scene.Camera.Position = New Vector3D(0, 0, 90)
        For I As Integer = 0 To 4
            GetNextLorenzPoint()
        Next
        my_Polyline.Points = points
        my_Polyline.SetVertexColors(palette.GetColors(512))
        my_scene.AddGeometry(my_Polyline)
        'Initialize FPS counter
        LastRenderTime = Now()
        Framecounter = 0
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        App_Loaded = True
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        If Not App_Loaded Then Exit Sub
        'Show FPS
        Framecounter += 1
        If Framecounter = 25 Then
            Dim fps As Double = CInt(25000 / (Now - LastRenderTime).TotalMilliseconds)
            Me.Title = "FPS = " & fps.ToString
            LastRenderTime = Now()
            Framecounter = 0
        End If
        'Create the Lorenz Attractor
        For I As Integer = 0 To 4
            GetNextLorenzPoint()
        Next
        my_Polyline.Points = points
        my_Polyline.GenerateGeometry(my_scene)
        'Update the scene
        For Each geo As GLGeometry In my_scene.Geometry
            geo.Update()
        Next
        'Render the scene.
        my_scene.Render()
    End Sub

    Private Sub GetNextLorenzPoint()
        dX = (Sigma * (Y - X)) * dt
        dY = (X * (Rho - Z) - Y) * dt
        dZ = (X * Y - Beta * Z) * dt
        X = X + dX
        Y = Y + dY
        Z = Z + dZ
        points.Add(New Point3D(X + 3, Y, Z - 30))
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
        End Select
    End Sub

End Class
