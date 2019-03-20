Class MainWindow
    Private EndPtCount As Integer = 8
    Private StepPercentage As Double = 0.7
    Private PixelData As Byte()
    Private Intrect As Int32Rect
    Private Stride As Integer = 0
    Private Writebitmap As WriteableBitmap
    Private NewPixelData(2) As Byte
    Private colorList As List(Of Color)
    Private palet As BitmapPalette
    Private Points As List(Of Vector)
    Private currentPt As Vector
    Private nextPt As Vector
    Private Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Init()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub Init()
        Dim p As Vector
        Dim angle As Double = 0.0
        Dim radius As Double = 0.9 * Canvas1.ActualWidth / 2
        Dim midPt As Vector = New Vector(Canvas1.ActualWidth / 2, Canvas1.ActualHeight / 2)
        Image1.Width = Canvas1.ActualWidth
        Image1.Height = Canvas1.ActualHeight
        Points = New List(Of Vector)
        If 0.9 * Canvas1.ActualHeight / 2 < radius Then radius = 0.9 * Canvas1.ActualHeight / 2
        Stride = CInt(Canvas1.ActualWidth * PixelFormats.Rgb24.BitsPerPixel / 8)
        ReDim PixelData(Stride * CInt(Canvas1.ActualHeight))
        colorList = New List(Of Color) From
        {
            Colors.Red,
            Colors.Green,
            Colors.Blue
        }
        palet = New BitmapPalette(colorList)
        Writebitmap = New WriteableBitmap(BitmapSource.Create(CInt(Canvas1.ActualWidth), CInt(Canvas1.ActualHeight), 96, 96, PixelFormats.Rgb24, palet, PixelData, Stride))
        Image1.Source = Writebitmap
        'Create the endpoints on a circle
        For I As Integer = 0 To EndPtCount - 1
            angle = 2 * Math.PI * I / EndPtCount
            p = New Vector(radius * Math.Cos(angle), radius * Math.Sin(angle)) + midPt
            Points.Add(p)
            SetPixel(p.X, p.Y, Colors.White, PixelData, Stride)
        Next
        currentPt = midPt  'New Vector(Canvas1.ActualWidth * Rnd.NextDouble(), Canvas1.ActualHeight * Rnd.NextDouble())
    End Sub


    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        'Do the actual rendering
        Dim index As Integer = 0
        For I As Integer = 0 To 200
            index = Rnd.Next(Points.Count)
            nextPt = Points(index)
            currentPt = Lerp(currentPt, nextPt, StepPercentage)
            SetPixel(currentPt.X, currentPt.Y, Colors.White, PixelData, Stride)
        Next
        Intrect = New Int32Rect(0, 0, Writebitmap.PixelWidth - 1, Writebitmap.PixelHeight - 1)
        Writebitmap.WritePixels(Intrect, PixelData, Stride, 0)
    End Sub

    Private Function Lerp(V1 As Vector, V2 As Vector, percentage As Double) As Vector
        Dim result As Vector
        result.X = V1.X + percentage * (V2.X - V1.X)
        result.Y = V1.Y + percentage * (V2.Y - V1.Y)
        Return result
    End Function

    Private Sub SetPixel(ByVal X As Double , ByVal Y As Double , ByVal c As Color, ByVal buffer As Byte(), ByVal PixStride As Integer)
        If X < 0 Or X > Writebitmap.PixelWidth Or Y < 0 Or Y > Writebitmap.PixelHeight Then Exit Sub
        Dim xIndex As Integer = CInt(X) * 3
        Dim yIndex As Integer = CInt(Y) * PixStride
        buffer(xIndex + yIndex) = c.R
        buffer(xIndex + yIndex + 1) = c.G
        buffer(xIndex + yIndex + 2) = c.B
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        Init()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub
End Class
