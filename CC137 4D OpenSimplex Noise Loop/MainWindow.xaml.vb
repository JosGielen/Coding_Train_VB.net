Imports System.IO
Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 10
    Private myEllipsoid As EllipsoidGeometry
    Private percent As Integer
    Private recording As Boolean = False
    Private fileName As String = "NoiseLoopFrame-"
    Private ResultFileName As String = "OpenSimplexNoiseGifLoop.gif"
    Private frameNumber As Integer = 0
    Private started As Boolean = False

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        If recording Then
            Width = 380
            Height = 350
        Else
            Width = 680
            Height = 650
        End If
    End Sub

    Private Sub Render()
        Do
            'Render the scene.
            myScene.Render(percent)
            percent += 1
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
            If recording Then
                SaveImage(Environment.CurrentDirectory & "\output\" & fileName & frameNumber.ToString("000") & ".png")
                frameNumber += 1
                Thread.Sleep(100)
            End If
            Title = "Percent = " & percent.ToString()
            If percent > 100 Then
                percent = 0
                If recording Then
                    MakeGif()
                    End
                End If
            End If
        Loop
    End Sub

    Private Sub SaveImage(filename As String)
        Dim MyEncoder As BitmapEncoder = New PngBitmapEncoder()
        Dim renderbmp As RenderTargetBitmap = New RenderTargetBitmap(CInt(myScene.ActualWidth), CInt(myScene.ActualHeight), 96, 96, PixelFormats.Default)
        renderbmp.Render(myScene)
        Try
            MyEncoder.Frames.Add(BitmapFrame.Create(renderbmp))
            ' Create a FileStream to write the image to the file.
            Using sw As FileStream = New FileStream(filename, FileMode.Create)
                MyEncoder.Save(sw)
            End Using
        Catch ex As Exception
            MessageBox.Show("The Image could not be saved.", "NoiseGifLoop3D error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MakeGif()
        Dim prog As String = Environment.CurrentDirectory & "\ffmpeg.exe"
        Dim args As String = "-i output\" & fileName & "%3d.png -r 20 output\" & ResultFileName
        Process.Start(prog, args)
        For Each f As String In Directory.GetFiles(Environment.CurrentDirectory & "\output")
            If Path.GetExtension(f) = ".png" Then
                File.Delete(f)
            End If
        Next
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        If started Then Exit Sub
        myEllipsoid = New EllipsoidGeometry(7, 7, 7, 100, 100) With
        {
            .InitialRotationAxis = New Media3D.Vector3D(0, 0, 0),
            .Position = New Media3D.Vector3D(0, 0, 0),
            .AmbientMaterial = Color.FromRgb(0, 0, 0),
            .DiffuseMaterial = Color.FromRgb(0, 0, 0),
            .SpecularMaterial = Color.FromRgb(150, 150, 150),
            .Shininess = 20,
            .UseTexture = True,
            .TextureFile = Environment.CurrentDirectory & "\rainbow3.jpg"
        }
        myScene.AddGeometry(myEllipsoid)
        started = True
        Render()
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
