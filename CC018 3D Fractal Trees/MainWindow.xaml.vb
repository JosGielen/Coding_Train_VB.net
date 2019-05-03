Imports My_GL
Imports System.Windows.Media.Media3D
Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Private App_Loaded As Boolean = False
    Private WaitTime As Integer = 5
    Private Delegate Sub WaitDelegate(ByVal t As Integer)
    Private IterationCount As Integer = 0
    Private previousbranches As Integer = 0
    Private Growing As Boolean = True
    Private Rnd As Random
    'Tree data
    Private my_Tree As Tree
    Private LeafCount As Integer = 250
    Private Leafs(LeafCount - 1) As Leaf
    Private TreeRadius As Double = 35.0
    Private TreeCenter As Point3D = New Vector3D(0.0, 0.5 * TreeRadius, 0.0)
    Private RootPosition As Vector3D = New Vector3D(0.0, -2 * TreeRadius, 0.0)
    Private KillDistance As Double = 2.0
    Private ViewDistance As Double = 35.0
    Private BranchLength As Double = 4.0
    Private ShowLeaves As Boolean = False
    Private UseTexture As Boolean = False
    'Camera positioning
    Private Distance As Double = 150 'Camera distance from the center of the scene
    Private rotateAngle As Double = 0


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Init()
        Scene1.Camera.Position = New Vector3D(0, 0, Distance)
        Dim b As BoxGeometry = New BoxGeometry(150, 20, 150) With
        {
            .Position = RootPosition,
            .AmbientMaterial = Colors.Green,
            .DiffuseMaterial = Colors.Green,
            .SpecularMaterial = Colors.Black,
            .Shininess = 50
        }
        If UseTexture Then
            b.TextureFile = Environment.CurrentDirectory & "\grass.jpg"
            b.UseTexture = True
        End If
        Scene1.AddGeometry(b)
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        App_Loaded = True
    End Sub

    Private Sub Window_MouseDown(sender As Object, e As MouseButtonEventArgs)
    End Sub

    Private Sub Init()
        Rnd = New Random()
        Dim pos As Vector3D
        Dim X As Double = 0.0
        Dim Y As Double = 0.0
        Dim Z As Double = 0.0
        'Do the program initialisation
        '  Create a Tree
        my_Tree = New Tree(KillDistance, ViewDistance)
        '  Add leaves to the Tree inside a spherical volume
        For I As Integer = 0 To LeafCount - 1
            X = TreeRadius * (2 * Rnd.NextDouble() - 1)
            Y = TreeRadius * (2 * Rnd.NextDouble() - 1)
            Z = TreeRadius * (2 * Rnd.NextDouble() - 1)
            pos = New Vector3D(X, Y, Z) + TreeCenter
            my_Tree.AddLeaf(New Leaf(pos))
        Next
        my_Tree.Leafs.CopyTo(Leafs)
        '  Set the Tree Root (= First Branch)
        my_Tree.SetRoot(RootPosition, New Vector3D(0.0, 1.0, 0.0), BranchLength)
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        If Not App_Loaded Then Exit Sub
        If Growing Then Grow() 'Stop growing when no new branches during 20 iterations
        'Render the scene.
        Scene1.Render()
    End Sub

    Private Sub Grow()
        Dim newbranches As List(Of Branch) = New List(Of Branch)
        newbranches = my_Tree.Grow()
        If newbranches.Count = previousbranches Then
            IterationCount += 1
            If my_Tree.Branches.Count > 50 And IterationCount = 5 Then
                Growing = False
                If ShowLeaves Then
                    'Draw Ellipsoids that represent the leafs
                    Dim pt As Point3D
                    Dim El As EllipsoidGeometry
                    For I As Integer = 0 To Leafs.Length - 1
                        pt = Leafs(I).Location
                        El = New EllipsoidGeometry(5, 5, 5, 6, 6) With
                            {
                                .Position = Leafs(I).Location,
                                .AmbientMaterial = Color.FromRgb(0, 160, 0),
                                .DiffuseMaterial = Color.FromRgb(0, 160, 0),
                                .SpecularMaterial = Colors.White,
                                .Shininess = 50
                            }
                        If UseTexture Then
                            El.AmbientMaterial = Color.FromRgb(0, 100, 0)
                            El.DiffuseMaterial = Color.FromRgb(0, 100, 0)
                            El.TextureScaleX = 2
                            El.TextureScaleY = 2
                            El.TextureFile = Environment.CurrentDirectory & "\leaf1.jpg"
                            El.UseTexture = True
                        End If
                        Scene1.AddGeometry(El)
                    Next
                End If
            End If
        Else
            IterationCount = 0
            Growing = True
        End If
        previousbranches = newbranches.Count
        'Draw CylinderLines that represent the branches
        Dim pt1 As Vector3D
        Dim pt2 As Vector3D
        Dim Thickness As Double
        Dim cyl As CylinderLineGeometry
        For I As Integer = 0 To newbranches.Count - 1 'First branch has no Parent
            'Thickness goes from 1 at Location.Y = (TreeCenter + TreeRadius) to 8 at Location.Y = RootPosition
            'It is reduced by XZ distance from Location to TreeCenter
            Thickness = 7 * (newbranches(I).Parent.Location.Y - (TreeCenter.Y + TreeRadius)) / (RootPosition.Y - (TreeCenter.Y + TreeRadius)) + 1
            Thickness -= 0.1 * (Math.Sqrt((newbranches(I).Parent.Location.X - TreeCenter.X) ^ 2 + (newbranches(I).Parent.Location.Z - TreeCenter.Z) ^ 2))
            If Thickness < 0.5 Then Thickness = 0.5
            pt1 = newbranches(I).Location
            pt2 = newbranches(I).Parent.Location
            cyl = New CylinderLineGeometry(pt1, pt2, Thickness, 6) With
                {
                    .Position = New Vector3D(0, 0, 0),
                    .InitialRotationAxis = New Vector3D(0, 0, 0),
                    .AmbientMaterial = Color.FromRgb(120, 100, 20),
                    .DiffuseMaterial = Color.FromRgb(120, 100, 20),
                    .SpecularMaterial = Colors.White,
                    .Shininess = 30
                }
            If UseTexture Then
                cyl.AmbientMaterial = Color.FromRgb(100, 70, 20)
                cyl.DiffuseMaterial = Color.FromRgb(100, 70, 20)
                cyl.TextureFile = Environment.CurrentDirectory & "\bark.jpg"
                cyl.UseTexture = True
            End If
            Scene1.AddGeometry(cyl)
        Next
        Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
    End Sub


    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
