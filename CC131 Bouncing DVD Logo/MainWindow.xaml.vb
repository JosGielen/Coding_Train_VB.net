
Class MainWindow

    Private Rnd As Random = New Random()
    Private cb(2) As Byte
    Private X As Double
    Private Y As Double
    Private dX As Double
    Private dY As Double

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim bitmap As BitmapImage
        Dim convertBitmap As FormatConvertedBitmap
        Dim Writebitmap As WriteableBitmap
        bitmap = New BitmapImage(New System.Uri(Environment.CurrentDirectory & "\DVD-Logo.png"))
        'Change to 32 bpp if needed
        If bitmap.Format.BitsPerPixel <> 32 Then
            convertBitmap = New FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, Nothing, 0)
            Writebitmap = New WriteableBitmap(convertBitmap)
        Else
            Writebitmap = New WriteableBitmap(bitmap)
        End If
        'Show the picture in a WPF Image control
        image1.Source = Writebitmap
        X = (canvas1.ActualWidth - ImgCanvas.ActualWidth) * Rnd.NextDouble()
        Y = (canvas1.ActualHeight - ImgCanvas.ActualHeight) * Rnd.NextDouble()
        dX = 2.0
        dY = 2.0
        ImgCanvas.Width = Writebitmap.PixelWidth
        ImgCanvas.Height = Writebitmap.PixelHeight
        ImgCanvas.SetValue(Canvas.LeftProperty, X)
        ImgCanvas.SetValue(Canvas.TopProperty, Y)
        Rnd.NextBytes(cb)
        ImgCanvas.Background = New SolidColorBrush(Color.FromRgb(cb(0), cb(1), cb(2)))
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Dim colorchange As Boolean = False
        X += dX
        Y += dY
        If X < 0 Then
            X = 0
            dX = -dX
            colorchange = True
        End If
        If X > canvas1.ActualWidth - ImgCanvas.ActualWidth Then
            X = canvas1.ActualWidth - ImgCanvas.ActualWidth
            dX = -dX
            colorchange = True
        End If
        If Y < 0 Then
            Y = 0
            dY = -dY
            colorchange = True
        End If
        If Y > canvas1.ActualHeight - ImgCanvas.ActualHeight Then
            Y = canvas1.ActualHeight - ImgCanvas.ActualHeight
            dY = -dY
            colorchange = True
        End If
        ImgCanvas.SetValue(Canvas.LeftProperty, X)
        ImgCanvas.SetValue(Canvas.TopProperty, Y)
        If colorchange Then
            Rnd.NextBytes(cb)
            ImgCanvas.Background = New SolidColorBrush(Color.FromRgb(cb(0), cb(1), cb(2)))
        End If
    End Sub

End Class
