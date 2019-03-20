Class MainWindow
    Private Rnd As Random = New Random()
    Private QT As QuadTree
    Private my_MouseDown As Boolean = False
    Private my_MouseStart As Point
    Private my_MouseEnd As Point

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        QT = New QuadTree(New Rect(1.0, 1.0, canvas1.ActualWidth - 2, canvas1.ActualHeight - 2), 4)
        For I As Integer = 0 To 500
            Dim pt As Point = New Point(1.0 + (canvas1.ActualWidth - 2.0) * Rnd.NextDouble(), 1.0 + (canvas1.ActualHeight - 2.0) * Rnd.NextDouble())
            QT.Insert(pt)
            canvas1.Children.Clear()
            QT.Draw(canvas1)
        Next
    End Sub

    Private Sub Window_MouseUp(sender As Object, e As MouseButtonEventArgs)
        my_MouseDown = False
        my_MouseEnd = e.GetPosition(canvas1)
        Dim r As Rect = New Rect(my_MouseStart, my_MouseEnd)
        Dim found As List(Of Point) = QT.Query(r)

        Dim rec As Rectangle = New Rectangle() With
        {
        .Width = r.Width,
        .Height = r.Height,
        .Stroke = Brushes.Red,
        .StrokeThickness = 2
        }
        rec.SetValue(Canvas.TopProperty, r.Top)
        rec.SetValue(Canvas.LeftProperty, r.Left)
        canvas1.Children.Add(rec)

        Dim El As Ellipse
        For I As Integer = 0 To found.Count - 1
            El = New Ellipse() With
            {
            .Width = 4,
            .Height = 4,
            .Stroke = Brushes.Red,
            .StrokeThickness = 1
            }
            El.SetValue(Canvas.TopProperty, found(I).Y - 2.0)
            El.SetValue(Canvas.LeftProperty, found(I).X - 2.0)
            canvas1.Children.Add(El)
        Next
    End Sub

    Private Sub Window_MouseMove(sender As Object, e As MouseEventArgs)

    End Sub

    Private Sub Window_MouseDown(sender As Object, e As MouseButtonEventArgs)
        my_MouseDown = True
        my_MouseStart = e.GetPosition(canvas1)
    End Sub
End Class
