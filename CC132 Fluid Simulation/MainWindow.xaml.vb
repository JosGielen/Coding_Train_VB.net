Class MainWindow
    Private Rnd As Random = New Random()
    Private N As Integer = 96
    Private scale As Double = 0.0
    Private F As Fluid
    Private iter As Integer = 8
    Private TimeStep As Double = 0.03
    Private PerlinX As Double = 0.0
    Private colorCount As Integer = 650
    Private center As Point
    Private DyeRate As Double = 512
    'Render data
    Private my_Colors As List(Of Color)
    Private colorList As List(Of Color)
    Private palet As BitmapPalette
    Private PixFormat As PixelFormat
    Private BytesPerPixel As Integer
    Private Writebitmap As WriteableBitmap
    Private PixelData() As Byte
    Private NewPixelData() As Byte
    Private Stride As Integer = 0
    Private CellSize As Integer = 2
    Private Intrect As Int32Rect
    'FPS data
    Private LastRenderTime As Date
    Private Framecounter As Integer

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        'Load a color palette
        Dim pal As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Thermal.cpl")
        my_Colors = pal.GetColors(colorCount)
        If canvas1.ActualWidth > canvas1.ActualHeight Then
            N = CInt(canvas1.ActualHeight / CellSize)
        Else
            N = CInt(canvas1.ActualWidth / CellSize)
        End If
        center = New Point(N / 2, N / 2)
        'Create a Fluid
        F = New Fluid(N, TimeStep, 0, 0.1)
        'Create a writeable bitmap to render the Fluid
        scale = canvas1.ActualWidth / N
        image1.Width = N * CellSize   'canvas1.ActualWidth
        image1.Height = N * CellSize  'canvas1.ActualHeight
        image1.Stretch = Stretch.None

        PixFormat = PixelFormats.Rgb24
        BytesPerPixel = PixFormat.BitsPerPixel / 8
        Stride = N * CellSize * BytesPerPixel
        ReDim PixelData(Stride * image1.Height)
        ReDim NewPixelData(CellSize * CellSize * BytesPerPixel)
        colorList = New List(Of Color) From
            {
                Colors.Red,
                Colors.Green,
                Colors.Blue
            }
        palet = New BitmapPalette(colorList)
        Writebitmap = New WriteableBitmap(BitmapSource.Create(image1.Width, image1.Height, 96, 96, PixelFormats.Rgb24, palet, PixelData, Stride))
        image1.Source = Writebitmap
        'Initialize FPS counter
        LastRenderTime = Now()
        Framecounter = 0
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Dim angle As Double
        'Show FPS
        Framecounter += 1
        If Framecounter = 10 Then
            Dim fps As Double = CInt(10000 / (Now - LastRenderTime).TotalMilliseconds)
            Me.Title = "FPS = " & fps.ToString
            LastRenderTime = Now()
            Framecounter = 0
        End If
        'Add some dye to the Fluid
        angle = 2 * Math.PI * PerlinNoise.WideNoise(PerlinX, 2)
        PerlinX += 0.01
        For I As Integer = -1 To 1
            For J As Integer = -1 To 1
                F.AddDensity(center.X + I, center.Y + J, DyeRate)
            Next
        Next
        F.AddVelocity(center.X, center.Y, 5 * Math.Cos(angle), 5 * Math.Sin(angle))
        F.FadeDensity(0.7 * DyeRate / (N * N))
        'Update the Fluid
        F.TimeStep(iter)
        RenderFluid()
    End Sub

    Private Sub RenderFluid()
        Dim ColorIndex As Integer
        For J As Integer = 0 To N - 1
            For I As Integer = 0 To N - 1
                ColorIndex = CInt(F.density(I, J))
                If ColorIndex >= colorCount Then ColorIndex = colorCount - 1
                Intrect = New Int32Rect(I * CellSize, J * CellSize, CellSize, CellSize)
                For K As Integer = 0 To CellSize * CellSize - 1
                    NewPixelData(BytesPerPixel * K) = my_Colors(ColorIndex).R
                    NewPixelData(BytesPerPixel * K + 1) = my_Colors(ColorIndex).G
                    NewPixelData(BytesPerPixel * K + 2) = my_Colors(ColorIndex).B
                Next
                If Intrect.X < Writebitmap.PixelWidth And Intrect.Y < Writebitmap.PixelHeight Then
                    Writebitmap.WritePixels(Intrect, NewPixelData, CellSize * BytesPerPixel, 0)
                End If
            Next
        Next
    End Sub

End Class
