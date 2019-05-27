Class MainWindow
    Private len As Double = 180.0
    Private Angle As Double = 30.0
    Private RT As RotateTransform

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        Init()
        DrawBranch(len, New Point(canvas1.ActualWidth / 2, canvas1.ActualHeight), RT)
    End Sub

    Private Sub SldAngle_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        Angle = SldAngle.Value
        Init()
        DrawBranch(len, New Point(canvas1.ActualWidth / 2, canvas1.ActualHeight), RT)
    End Sub

    Private Sub Init()
        canvas1.Children.Clear()
        RT = New RotateTransform() With
        {
            .Angle = 0.0,
            .CenterX = canvas1.ActualWidth / 2,
            .CenterY = canvas1.ActualHeight
        }
    End Sub

    Private Sub DrawBranch(length As Double, Start As Point, rt As RotateTransform)
        If length > 3 Then
            Dim endPt As Point = rt.Transform(New Point(Start.X, Start.Y - length))
            Dim r As RotateTransform = New RotateTransform(rt.Angle)
            Dim l As Line = New Line() With
            {
                .Stroke = Brushes.White,
                .StrokeThickness = 2.0,
                .X1 = Start.X,
                .Y1 = Start.Y
            }
            l.X2 = endPt.X
            l.Y2 = endPt.Y
            canvas1.Children.Add(l)
            r.CenterX = endPt.X
            r.CenterY = endPt.Y
            r.Angle += Angle
            DrawBranch(0.67 * length, endPt, r)
            r.Angle -= 2 * Angle
            DrawBranch(0.67 * length, endPt, r)
        End If
    End Sub
End Class
