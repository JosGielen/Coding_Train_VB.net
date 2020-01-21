Class MainWindow
    Private N As Integer = 0
    Private D As Integer = 0
    Private P1 As Polygon
    Private P2 As Polygon


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        P1 = New Polygon() With
        {
            .Stroke = Brushes.White,
            .StrokeThickness = 1.0
        }
        canvas1.Children.Add(P1)
        P2 = New Polygon() With
        {
            .Stroke = Brushes.Magenta,
            .StrokeThickness = 3.0
        }
        canvas1.Children.Add(P2)
        Draw()
    End Sub

    Private Sub Draw()
        If Not IsLoaded Then Exit Sub
        Dim MaxR As Double = 0.0
        If canvas1.ActualWidth > canvas1.ActualHeight Then
            MaxR = canvas1.ActualHeight / 2
        Else
            MaxR = canvas1.ActualWidth / 2
        End If
        P1.Points.Clear()
        P2.Points.Clear()
        N = CInt(SliderN.Value)
        D = CInt(SliderD.Value)
        Dim center As Point = New Point(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2)
        Dim K As Double = 0.0
        Dim R As Double = 0.0
        Dim X As Double = 0.0
        Dim Y As Double = 0.0
        For I As Integer = 0 To 360
            K = I * D * Math.PI / 180
            R = Math.Sin(N * K) * MaxR
            X = R * Math.Cos(K)
            Y = R * Math.Sin(K)
            P1.Points.Add(New Point(center.X + X, center.Y + Y))
        Next
        For I As Double = 0 To 360
            K = I * Math.PI / 180
            R = Math.Sin(N * K) * MaxR
            X = R * Math.Cos(K)
            Y = R * Math.Sin(K)
            P2.Points.Add(New Point(center.X + X, center.Y + Y))
        Next

    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        Draw()
    End Sub
End Class
