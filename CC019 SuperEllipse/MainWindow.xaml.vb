Class MainWindow
    Private A As Double = 100.0
    Private B As Double = 100.0
    Private N As Double = 2.0


    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        Draw()
    End Sub

    Private Sub Draw()
        canvas1.Children.Clear()
        Dim poly As Polygon = New Polygon() With
        {
            .Stroke = Brushes.White,
            .StrokeThickness = 1.0
        }
        Dim X As Double = 0.0
        Dim Y As Double = 0.0
        For Angle As Double = 0 To 2 * Math.PI Step Math.PI / 50
            X = Math.Pow(Math.Abs(Math.Cos(Angle)), 2 / N) * A * Math.Sign(Math.Cos(Angle))
            Y = Math.Pow(Math.Abs(Math.Sin(Angle)), 2 / N) * B * Math.Sign(Math.Sin(Angle))
            poly.Points.Add(New Point(X + canvas1.ActualWidth / 2, Y + canvas1.ActualHeight / 2))
        Next
        canvas1.Children.Add(poly)
    End Sub

    Private Sub SldN_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Not IsLoaded Then Exit Sub
        N = SldN.Value
        Draw()
    End Sub
End Class
