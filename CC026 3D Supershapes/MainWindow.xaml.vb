Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private myEllipsoid As EllipsoidGeometry
    Private frameNumber As Integer = 170
    Private MaxFrameNumber As Integer = 250
    Private Slices As Integer = 150
    Private started As Boolean = False

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        If started Then Exit Sub
        myEllipsoid = New EllipsoidGeometry(3.5, Slices) With
        {
            .InitialRotationAxis = New Media3D.Vector3D(-30, 0, 0),
            .Position = New Media3D.Vector3D(0, 0, 0),
            .AmbientMaterial = Color.FromRgb(0, 0, 0),
            .DiffuseMaterial = Color.FromRgb(0, 0, 0),
            .SpecularMaterial = Color.FromRgb(150, 150, 150),
            .Shininess = 20,
            .UseTexture = True,
            .TextureFile = Environment.CurrentDirectory & "\rainbow3.jpg"
        }
        myScene.AddGeometry(myEllipsoid)
        Title = "3D Supershape"
        started = True
        Render()
    End Sub

    Private Sub Render()
        Dim imageName As String = ""
        Do
            myEllipsoid.m2 = 5 * (Math.Sin(2 * Math.PI * frameNumber / MaxFrameNumber) + 1.3)
            myEllipsoid.m1 = 5 * (Math.Cos(2 * Math.PI * frameNumber / MaxFrameNumber) + 1.3)
            myScene.Render()
            frameNumber += 1
            Title = "3D Supershape " & frameNumber.ToString()
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), 10)
            If frameNumber >= MaxFrameNumber Then
                frameNumber = 0
            End If
        Loop
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

End Class
