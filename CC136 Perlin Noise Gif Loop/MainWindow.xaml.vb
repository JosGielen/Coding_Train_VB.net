Imports System.IO
Imports System.Threading

Class MainWindow
    Private lines As List(Of Line)
    Private Rendering As Boolean = False
    Private recording As Boolean = False
    Private fileName As String = "CircleNoise-"
    Private frameNumber As Integer = 0
    Private AngleStep As Double = 1
    Private MaxRadius As Double = 200.0
    Private Percent As Integer = 0
    Private myColors As List(Of Brush)

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim cp As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Rainbow.cpl")
        myColors = cp.GetColorBrushes(CInt(MaxRadius))
        lines = New List(Of Line)
        Dim L As Line
        For Angle As Double = 0 To 360 Step AngleStep
            L = New Line() With
            {
                .StrokeThickness = 2
            }
            lines.Add(L)
            canvas1.Children.Add(L)
        Next
        Rendering = True
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        If Not Rendering Then Exit Sub
        Dim XOff As Double = 0.0
        Dim YOff As Double = 0.0
        Dim Offx As Double = 0.0
        Dim Offy As Double = 0.0
        Dim X As Double = 0.0
        Dim Y As Double = 0.0
        Dim PrevX As Double = 0.0
        Dim PrevY As Double = 0.0
        Dim F As Double = 5.0 'Determines how fast the Noise changes
        Dim R As Double = 0.0
        Dim Angle As Double = 0.0
        Offx = Math.Cos(2 * Percent * Math.PI / 100)
        Offy = Math.Sin(2 * Percent * Math.PI / 100)
        For I As Integer = 0 To lines.Count - 1
            Angle = 2 * Math.PI * I / (lines.Count - 1)
            XOff = F * Math.Cos(Angle)
            YOff = F * Math.Sin(Angle)
            R = MaxRadius * OpenSimplexNoise.Simplex2D(XOff + Offx, YOff + Offy)
            X = R * Math.Cos(Angle) + canvas1.ActualWidth / 2
            Y = R * Math.Sin(Angle) + canvas1.ActualHeight / 2
            If I > 0 Then
                lines(I).Stroke = myColors(CInt(R))
                lines(I).X1 = PrevX
                lines(I).Y1 = PrevY
                lines(I).X2 = X
                lines(I).Y2 = Y
            End If
            PrevX = X
            PrevY = Y
        Next
        Percent += 1
        Thread.Sleep(100)
        If recording Then
            SaveImage(Environment.CurrentDirectory & "\output\CircleNoise-" & frameNumber.ToString("000") & ".png")
            frameNumber += 1
            Thread.Sleep(100)
        End If
        If Percent > 100 Then
            Percent = 0
            If recording Then End
        End If
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

End Class
