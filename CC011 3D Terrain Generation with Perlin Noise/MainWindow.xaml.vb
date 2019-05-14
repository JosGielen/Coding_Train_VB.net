Imports My_GL
Imports System.Windows.Media.Media3D

Class MainWindow
    Private App_Loaded As Boolean = False
    Private Rendering As Boolean = False
    Private Mesh As MeshGeometry
    Private XSize As Integer = 250
    Private ZSize As Integer = 250
    Private CellSize As Double = 0.3
    Private Scale As Double = 0.15
    Private PeakHeight As Double = 8.0
    Private Roughness As Integer = 4
    Private Elevations(XSize - 1, ZSize - 1) As Double
    Private UseOpenSimplex As Boolean = True
    Private ZOff As Double = 0.0

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Mesh = New MeshGeometry(XSize, ZSize, CellSize) With
        {
            .Position = New Vector3D(0, 0, 0),
            .InitialRotationAxis = New Vector3D(0, 0, 0),
            .RotationAxis = New Vector3D(0, 0, 0),
            .RotationSpeed = 0.0,
            .AmbientMaterial = Colors.Black,
            .DiffuseMaterial = Color.FromRgb(150, 150, 150),
            .SpecularMaterial = Colors.Black,
            .Shininess = 30,
            .DrawMode = DrawMode.Fill,
            .LineWidth = 1.0,
            .PointSize = 1.0,
            .UseMaterial = True,
            .UseVertexColors = False,
            .UseTexture = False,
            .TextureScaleX = 1.0,
            .TextureScaleY = 1.0,
            .VertexColorIntensity = 1.0
        }
        Scene1.Lights(0).Direction = New Vector3D(-3, -2, -1)
        'Scene1.Camera.Position = New Vector3D(0, 12.0, 42.0)
        Scene1.Camera = New FixedCamera(New Vector3D(0, 1.5 * PeakHeight, ZSize * CellSize / 2), New Vector3D(0, 2, 0), New Vector3D(0, 1, 0))
        Scene1.AddGeometry(Mesh)
        App_Loaded = True
    End Sub

    Private Sub Init()
        UseOpenSimplex = RbSimplex.IsChecked.Value
        Dim h As Double = 0.0
        For I As Integer = 0 To XSize - 1
            For J As Integer = 0 To ZSize - 2
                If UseOpenSimplex Then
                    h = PeakHeight * OpenSimplexNoise.WideSimplex2D(I * Scale * CellSize, J * Scale * CellSize + ZOff, Roughness, 0.5, 1) - 0.3
                Else
                    h = PeakHeight * PerlinNoise.WideNoise2D(I * Scale * CellSize, J * Scale * CellSize + ZOff, Roughness, 0.5, 1) - 0.3
                End If
                If h < 0 Then h = 0.05
                Elevations(I, J) = h
            Next
            Elevations(I, ZSize - 1) = 0.8 * PeakHeight
        Next
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        If Not Rendering Then Exit Sub
        Dim h As Double = 0.0
        For I As Integer = 0 To XSize - 1
            If UseOpenSimplex Then
                h = PeakHeight * OpenSimplexNoise.WideSimplex2D(I * Scale * CellSize, ZOff, Roughness, 0.5, 1) - 0.3
            Else
                h = PeakHeight * PerlinNoise.WideNoise2D(I * Scale * CellSize, ZOff, Roughness, 0.5, 1) - 0.3
            End If
            If h < 0 Then h = 0.05
            Elevations(I, 0) = h
            For J As Integer = ZSize - 1 To 1 Step -1
                Elevations(I, J) = Elevations(I, J - 1)
            Next
        Next
        ZOff -= Scale * CellSize
        Mesh.Heights = Elevations
        GC.Collect()
        Mesh.GenerateGeometry(Scene1)
        'Render the scene.
        Scene1.Render()
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            Scale = SldScale.Value
            Roughness = CInt(SldRoughness.Value)
            SldScale.IsEnabled = False
            SldRoughness.IsEnabled = False
            Init()
            Mesh.TextureFile = Environment.CurrentDirectory & "\Terrain.jpg"
            Mesh.UseTexture = True
            AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
            BtnStart.Content = "STOP"
            Rendering = True
        Else
            RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
            SldScale.IsEnabled = True
            SldRoughness.IsEnabled = True
            BtnStart.Content = "START"
            Rendering = False
        End If
    End Sub

End Class
