Imports System.Windows.Media.Media3D

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private settingForm As Settings
    Private knotType As Integer
    Private rotationSpeed As Double
    Private ShowTexture As Boolean
    Private MyDrawMode As DrawMode
    Private myKnot As GLGeometry
    Private frameNumber As Integer = 170
    Private MaxFrameNumber As Integer = 250
    Private started As Boolean = False

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        settingForm = New Settings(Me)
        settingForm.Show()
        settingForm.Left = Me.Left + Me.Width
        settingForm.Top = Me.Top
        settingForm.CmbKnotType.SelectedIndex = 0
        rotationSpeed = 0.003
        ShowTexture = True
        MyDrawMode = DrawMode.Fill
        SetScene()
        CreateKnot1(DrawMode.Fill)
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub SetScene()
        'Set the scene lights
        myScene.Lights.Clear()
        Dim l1 As GLLight = New GLLight(LightType.DirectionalLight) With
        {
        .Direction = New Vector3D(2.0, 2.0, 2.0),
        .Ambient = Color.FromRgb(150, 150, 150),
        .Diffuse = Color.FromRgb(255, 255, 255),
        .Specular = Color.FromRgb(255, 255, 255)
        }
        myScene.AddLight(l1)
        Dim l2 As GLLight = New GLLight(LightType.PointLight) With
        {
        .Position = New Vector3D(-3.0, 0.0, 1.0),
        .Ambient = Color.FromRgb(150, 150, 150),
        .Diffuse = Color.FromRgb(200, 200, 200),
        .Specular = Color.FromRgb(255, 255, 255),
        .Linear = 0.5,
        .Quadratic = 0,
        .Constant = 0.3
        }
        myScene.AddLight(l2)
        Dim l3 As GLLight = New GLLight(LightType.SpotLight) With
        {
        .Position = New Vector3D(2.0, 2.0, -2.0),
        .Direction = New Vector3D(-2.0, -2.0, 2.0),
        .Ambient = Color.FromRgb(150, 150, 150),
        .Diffuse = Color.FromRgb(255, 255, 255),
        .Specular = Color.FromRgb(255, 255, 255),
        .CutOff = 8.0,
        .OuterCutOff = 12.0,
        .Linear = 0.09,
        .Quadratic = 0.03,
        .Constant = 0.3
        }
        myScene.AddLight(l3)
        'Set the camera position
        myScene.Camera.Position = New Vector3D(0, 0, -300)
        myScene.CamDefaultPos = myScene.Camera.Position
    End Sub

    Public Sub GetParameters()
        knotType = settingForm.KnotType
        Select Case knotType
            Case 1
                CreateKnot1(MyDrawMode)
            Case 2
                CreateKnot2(MyDrawMode)
            Case 3
                CreateKnot3(MyDrawMode)
            Case 4
                CreateKnot4(MyDrawMode)
            Case 5
                CreateKnot5(MyDrawMode)
            Case 6
                CreateKnot6(MyDrawMode)
            Case 7
                CreateKnot7(MyDrawMode)
            Case 8
                CreateKnot8(MyDrawMode)
        End Select
    End Sub

    Public Sub SetRotation(rotSpeed As Double)
        rotationSpeed = rotSpeed
        myKnot.RotationSpeed = rotationSpeed
    End Sub

    Public Sub SetShowTexture(showText As Boolean)
        ShowTexture = showText
        GetParameters()
    End Sub

    Public Sub SetDrawMode(mode As Integer)
        Select Case mode
            Case 0
                MyDrawMode = DrawMode.Points
            Case 1
                MyDrawMode = DrawMode.Lines
            Case 2
                MyDrawMode = DrawMode.Fill
        End Select
        myKnot.DrawMode = MyDrawMode
    End Sub

    Private Sub SetKnotColor(useTex As Boolean)
        If useTex Then
            myKnot.DiffuseMaterial = Color.FromRgb(0, 0, 0)
            myKnot.AmbientMaterial = Color.FromRgb(0, 0, 0)
            myKnot.SpecularMaterial = Color.FromRgb(150, 150, 150)
            myKnot.Shininess = 20.0
            myKnot.UseTexture = True
            myKnot.TextureFile = Environment.CurrentDirectory & "\Rope.jpg"
        Else
            myKnot.DiffuseMaterial = Color.FromRgb(0, 200, 150)
            myKnot.AmbientMaterial = Color.FromRgb(0, 200, 150)
            myKnot.SpecularMaterial = Color.FromRgb(250, 250, 250)
            myKnot.Shininess = 50.0
            myKnot.UseTexture = False
        End If
    End Sub

    Private Sub CreateKnot1(dm As DrawMode)
        myScene.ClearGeometries()
        myKnot = New Knot1Geometry(5, 480, 12, 32) With
        {
            .Position = New Vector3D(0, 0, 0),
            .InitialRotationAxis = New Vector3D(0, 0, 0),
            .RotationAxis = New Vector3D(0, 1, 0),
            .RotationSpeed = rotationSpeed,
            .DrawMode = dm,
            .TextureScaleX = 1,
            .TextureScaleY = 20
        }
        CType(myKnot, Knot1Geometry).SetParameters(settingForm)
        SetKnotColor(ShowTexture)
        myScene.AddGeometry(myKnot)
        Title = "3D Knot #1"
    End Sub

    Private Sub CreateKnot2(dm As DrawMode)
        myScene.ClearGeometries()
        myKnot = New Knot2Geometry(32, 240, 12, 32) With
        {
            .Position = New Vector3D(0, 0, 0),
            .InitialRotationAxis = New Vector3D(0, 0, 0),
            .RotationAxis = New Vector3D(0, 1, 0),
            .RotationSpeed = rotationSpeed,
            .DrawMode = dm,
            .TextureScaleX = 1,
            .TextureScaleY = 30
        }
        CType(myKnot, Knot2Geometry).SetParameters(settingForm)
        SetKnotColor(ShowTexture)
        myScene.AddGeometry(myKnot)
        Title = "3D Knot #2"
    End Sub

    Private Sub CreateKnot3(dm As DrawMode)
        myScene.ClearGeometries()
        myKnot = New Knot3Geometry(70, 480, 8, 32) With
        {
            .Position = New Vector3D(0, 0, 0),
            .InitialRotationAxis = New Vector3D(0, 0, 0),
            .RotationAxis = New Vector3D(0, 1, 0),
            .RotationSpeed = rotationSpeed,
            .DrawMode = dm,
            .TextureScaleX = 1,
            .TextureScaleY = 50
        }
        CType(myKnot, Knot3Geometry).SetParameters(settingForm)
        SetKnotColor(ShowTexture)
        myScene.AddGeometry(myKnot)
        Title = "3D Knot #3"
    End Sub

    Private Sub CreateKnot4(dm As DrawMode)
        myScene.ClearGeometries()
        myKnot = New Knot4Geometry(45, 720, 6, 32) With
        {
            .Position = New Vector3D(0, 0, 0),
            .InitialRotationAxis = New Vector3D(0, 0, 0),
            .RotationAxis = New Vector3D(0, 1, 0),
            .RotationSpeed = rotationSpeed,
            .DrawMode = dm,
            .TextureScaleX = 1,
            .TextureScaleY = 70
        }
        CType(myKnot, Knot4Geometry).SetParameters(settingForm)
        SetKnotColor(ShowTexture)
        myScene.AddGeometry(myKnot)
        Title = "3D Knot #4"
    End Sub

    Private Sub CreateKnot5(dm As DrawMode)
        myScene.ClearGeometries()
        myKnot = New Knot5Geometry(120, 480, 8, 32) With
        {
            .Position = New Vector3D(0, 0, 0),
            .InitialRotationAxis = New Vector3D(0, 0, 0),
            .RotationAxis = New Vector3D(0, 1, 0),
            .RotationSpeed = rotationSpeed,
            .DrawMode = dm,
            .TextureScaleX = 1,
            .TextureScaleY = 30
        }
        CType(myKnot, Knot5Geometry).SetParameters(settingForm)
        SetKnotColor(ShowTexture)
        myScene.AddGeometry(myKnot)
        Title = "3D Knot #5"
    End Sub

    Private Sub CreateKnot6(dm As DrawMode)
        myScene.ClearGeometries()
        myKnot = New Knot6Geometry(30, 480, 8, 32) With
        {
            .Position = New Vector3D(0, 0, 0),
            .InitialRotationAxis = New Vector3D(0, 0, 0),
            .RotationAxis = New Vector3D(0, 1, 0),
            .RotationSpeed = rotationSpeed,
            .DrawMode = dm,
            .TextureScaleX = 1,
            .TextureScaleY = 50
        }
        CType(myKnot, Knot6Geometry).SetParameters(settingForm)
        SetKnotColor(ShowTexture)
        myScene.AddGeometry(myKnot)
        Title = "3D Knot #6"
    End Sub

    Private Sub CreateKnot7(dm As DrawMode)
        myScene.ClearGeometries()
        myKnot = New Knot7Geometry(100, 120, 0.07, 32) With
        {
            .Position = New Vector3D(0, 0, 0),
            .InitialRotationAxis = New Vector3D(0, 0, 0),
            .RotationAxis = New Vector3D(0, 1, 0),
            .RotationSpeed = rotationSpeed,
            .DrawMode = dm,
            .TextureScaleX = 1,
            .TextureScaleY = 30
        }
        CType(myKnot, Knot7Geometry).SetParameters(settingForm)
        SetKnotColor(ShowTexture)
        myScene.AddGeometry(myKnot)
        Title = "3D Knot #7"
    End Sub

    Private Sub CreateKnot8(dm As DrawMode)
        myScene.ClearGeometries()
        myKnot = New Knot8Geometry(60, 480, 8, 32) With
        {
            .Position = New Vector3D(0, 0, 0),
            .InitialRotationAxis = New Vector3D(0, 0, 0),
            .RotationAxis = New Vector3D(0, 1, 0),
            .RotationSpeed = rotationSpeed,
            .DrawMode = dm,
            .TextureScaleX = 1,
            .TextureScaleY = 30
        }
        CType(myKnot, Knot8Geometry).SetParameters(settingForm)
        SetKnotColor(ShowTexture)
        myScene.AddGeometry(myKnot)
        Title = "3D Knot #8"
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        'Update the scene
        For Each geo As GLGeometry In myScene.Geometries
            geo.Update()
        Next
        'Render the scene.
        myScene.Render()
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

    Private Sub Window_LocationChanged(sender As Object, e As EventArgs)
        If settingForm IsNot Nothing Then
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
        End If
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If settingForm IsNot Nothing Then
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
        End If
    End Sub
End Class
