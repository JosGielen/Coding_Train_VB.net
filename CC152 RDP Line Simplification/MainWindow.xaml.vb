Class MainWindow
    Private points As List(Of Vector)
    Private RDPpoints As List(Of Vector)
    Private My_Width As Integer = 0
    Private My_Height As Integer = 0
    Private Epsilon As Double = 5.0

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        My_Width = CInt(canvas1.ActualWidth)
        My_Height = CInt(canvas1.ActualHeight)
        points = New List(Of Vector)
        RDPpoints = New List(Of Vector)
        Dim X As Double = 0.0
        Dim Y As Double = 0.0
        For I As Integer = 0 To My_Width
            X = I * 5.0 / My_Width
            Y = My_Height - (Math.Exp(-X) * Math.Cos(2 * Math.PI * X) + 1) * My_Height / 2
            points.Add(New Vector(I, Y))
            RDPpoints.Add(New Vector(I, Y))
        Next
        Draw()
    End Sub

    Private Sub EpSlider_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Not IsLoaded Then Exit Sub
        Epsilon = EpSlider.Value
        TxtEpsilon.Text = Epsilon.ToString("F1")
        RDPpoints.Clear()
        RDPpoints.Add(points.First)
        RDP(1, points.Count - 2)
        RDPpoints.Add(points.Last)
        Draw()
    End Sub

    Private Sub RDP(firstIndex As Integer, lastIndex As Integer)
        Dim nextIndex As Integer = FindFurthest(firstIndex, lastIndex)
        If nextIndex > 0 Then
            RDP(firstIndex, nextIndex)
            RDPpoints.Add(points(nextIndex))
            RDP(nextIndex + 1, lastIndex)
        End If
    End Sub

    Private Function FindFurthest(firstIndex As Integer, lastIndex As Integer) As Integer
        Dim recordDistance As Double = -1.0
        Dim recordIndex As Integer = 0
        Dim dist As Double = -1.0
        If firstIndex >= lastIndex Then Return -1
        For I As Integer = firstIndex To lastIndex
            dist = LineDistance(points(firstIndex), points(lastIndex), points(I))
            If dist > recordDistance Then
                recordDistance = dist
                recordIndex = I
            End If
        Next
        If recordDistance >= Epsilon Then
            Return recordIndex
        Else
            Return -1
        End If
    End Function

    Private Function LineDistance(a As Vector, b As Vector, c As Vector) As Double
        Dim ac As Vector = c - a
        Dim ab As Vector = b - a
        Dim normal As Vector
        ab.Normalize()
        ab = (ac * ab) * ab
        normal = a + ab
        Return (c - normal).Length
    End Function

    Private Sub Draw()
        Dim L As Line
        canvas1.Children.Clear()
        For I As Integer = 0 To points.Count - 2
            L = New Line() With
                {
                    .X1 = points(I).X,
                    .Y1 = points(I).Y,
                    .X2 = points(I + 1).X,
                    .Y2 = points(I + 1).Y,
                    .Stroke = Brushes.White,
                    .StrokeThickness = 1.0
                }
            canvas1.Children.Add(L)
        Next
        For I As Integer = 0 To RDPpoints.Count - 2
            L = New Line() With
                {
                    .X1 = RDPpoints(I).X,
                    .Y1 = RDPpoints(I).Y,
                    .X2 = RDPpoints(I + 1).X,
                    .Y2 = RDPpoints(I + 1).Y,
                    .Stroke = Brushes.Red,
                    .StrokeThickness = 1.0
                }
            canvas1.Children.Add(L)
        Next
        TxtRDPpoints.Text = RDPpoints.Count.ToString()
    End Sub


End Class
