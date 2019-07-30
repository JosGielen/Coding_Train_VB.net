Class MainWindow
    Private N As Integer = 0
    Private D As Integer = 0
    Private P As Polygon

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        P = New Polygon() With
        {
            .Stroke = Brushes.White,
            .StrokeThickness = 1.0
        }
        canvas1.Children.Add(P)
        Draw()
    End Sub

    Private Sub Draw()
        If Not IsLoaded Then Exit Sub
        P.Points.Clear()
        N = CInt(SliderN.Value)
        D = CInt(SliderD.Value)
        Dim center As Point = New Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2)
        Dim K As Double = N / D
        Dim R As Double = 0
        For A As Double = 0 To 2 * D * Math.PI Step 0.01
            R = 0.9 * canvas1.ActualWidth / 2 * Math.Cos(N * A / D)
            P.Points.Add(New Point(center.X + R * Math.Cos(A), center.Y + R * Math.Sin(A)))
        Next
    End Sub

End Class
