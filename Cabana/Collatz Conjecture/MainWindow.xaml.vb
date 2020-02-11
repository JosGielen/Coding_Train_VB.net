Imports System.IO
Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 10
    Private Rendering As Boolean = False
    Private angleOff As Double = 15.0
    Private length As Double = 10.0
    Private colorList As List(Of Brush)
    Private series As List(Of Integer)
    Private ImageFileName As String = "Collatz-"
    Private frameNumber As Integer = 0
    Private ResultFileName As String = "Collatz.gif"
    Private recording As Boolean = False
    Private Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim cp As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Reds.cpl")
        colorList = cp.GetColorBrushes(2000)
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            BtnStart.Content = "STOP"
            Rendering = True
            Render()
        Else
            BtnStart.Content = "START"
            Rendering = False
        End If
    End Sub

    Private Sub Render()
        Dim N As Integer = 0
        Dim imageName As String = ""
        series = New List(Of Integer)
        For I = 1 To 1500
            If Not Rendering Then Exit Sub
            series.Clear()
            N = I
            While N > 1
                series.Add(N)
                N = Collatz(N)
            End While
            series.Add(1)
            DrawSeries()
            Me.Title = I.ToString()
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
            If recording And I Mod 8 = 0 Then
                frameNumber += 1
                imageName = Environment.CurrentDirectory & "\output\" & ImageFileName & frameNumber.ToString("000") & ".png"
                SaveImage(imageName)
                Thread.Sleep(50)
            End If
        Next
        If recording Then
            MakeGif()
            End
        End If
    End Sub

    Private Function Collatz(n As Integer) As Integer
        If n Mod 2 = 0 Then
            Return CInt(n / 2)
        Else
            Return CInt((3 * n + 1) / 2)
        End If
    End Function

    Private Sub DrawSeries()
        If series.Count > 50 Then Exit Sub
        Dim L As Line
        Dim X As Double = canvas1.ActualWidth / 6
        Dim Y As Double = canvas1.ActualHeight
        Dim angle As Double = Math.PI / 4
        Dim Xoff As Double = 0.0
        Dim Yoff As Double = 0.0
        Dim index As Integer = Rnd.Next(colorList.Count)
        series.Reverse()
        For I As Integer = 0 To series.Count - 2
            If series(I + 1) Mod 2 = 0 Then
                angle += angleOff * Math.PI / 180
            Else
                angle -= angleOff * Math.PI / 180
            End If
            Xoff = -length * Math.Cos(angle)
            Yoff = -length * Math.Sin(angle)
            L = New Line() With
                {
                .X1 = X,
                .Y1 = Y,
                .X2 = X + Xoff,
                .Y2 = Y + Yoff,
                .Stroke = colorList(index),
                .StrokeThickness = 5
                }
            canvas1.Children.Add(L)
            X += Xoff
            Y += Yoff
        Next
    End Sub

    Private Sub SaveImage(filename As String)
        Dim MyEncoder As BitmapEncoder = New PngBitmapEncoder()
        Dim renderbmp As RenderTargetBitmap = New RenderTargetBitmap(CInt(canvas1.ActualWidth), CInt(canvas1.ActualHeight), 96, 96, PixelFormats.Default)
        renderbmp.Render(canvas1)
        Try
            MyEncoder.Frames.Add(BitmapFrame.Create(renderbmp))
            ' Create a FileStream to write the image to the file.
            Using sw As FileStream = New FileStream(filename, FileMode.Create)
                MyEncoder.Save(sw)
            End Using
        Catch ex As Exception
            MessageBox.Show("The Image could not be saved.", "NoiseGifLoop error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MakeGif()
        Dim prog As String = Environment.CurrentDirectory & "\ffmpeg.exe"
        Dim args As String = "-i output\" & ImageFileName & "%3d.png -r 20 " & ResultFileName
        Process.Start(prog, args)
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
