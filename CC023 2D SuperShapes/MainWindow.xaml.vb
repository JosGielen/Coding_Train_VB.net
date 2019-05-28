Class MainWindow
    Private settingForm As Settings
    Private Scale As Double = 1.0
    Private PointCount As Integer = 200
    Private points As PointCollection
    Private A As Double = 1.0
    Private B As Double = 1.0
    Private M As Double = 0.0
    Private N1 As Double = 0.2
    Private N2 As Double = 1.7
    Private N3 As Double = 1.7

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        ShowSettingForm()
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        Start()
    End Sub

    Public Sub Start()
        canvas1.Children.Clear()
        points = New PointCollection(PointCount)
        GetParameters()
        Dim poly As Polygon = New Polygon() With
        {
            .Stroke = Brushes.White,
            .StrokeThickness = 2.0
        }
        Dim R As Double = 0.0
        Dim X As Double = 0.0
        Dim Y As Double = 0.0
        Dim maxX As Double = 0.0
        Dim minX As Double = Double.MaxValue
        Dim maxY As Double = 0.0
        Dim minY As Double = Double.MaxValue
        For Angle As Double = 0 To 2 * Math.PI Step 2 * Math.PI / PointCount
            R = SuperShape(Angle)
            X = R * Math.Cos(Angle)
            If X > maxX Then maxX = X
            If X < minX Then minX = X
            Y = R * Math.Sin(Angle)
            If Y > maxY Then maxY = Y
            If Y < minY Then minY = Y
            points.Add(New Point(X, Y))
        Next
        'Determine the Scale to fit the Polygon inside the canvas
        If maxX - minX > maxY - minY Then
            Scale = (canvas1.ActualWidth - 100) / (maxX - minX)
        Else
            Scale = (canvas1.ActualHeight - 100) / (maxY - minY)
        End If
        minX = Scale * minX
        maxX = Scale * maxX
        minY = Scale * minY
        maxY = Scale * maxY
        For I As Integer = 0 To points.Count - 1
            points(I) = New Point(Scale * points(I).X + (canvas1.ActualWidth - minX - maxX) / 2, Scale * points(I).Y + (canvas1.ActualHeight - minY - maxY) / 2)
        Next
        poly.Points = points
        canvas1.Children.Add(poly)
    End Sub

    Private Function SuperShape(angle As Double) As Double
        Dim part1 As Double = Math.Pow(Math.Abs(Math.Cos(angle * M / 4) / A), N2)
        Dim part2 As Double = Math.Pow(Math.Abs(Math.Sin(angle * M / 4) / B), N3)
        Return 1 / (Math.Pow((part1 + part2), 1 / N1))
    End Function

    Private Sub ShowSettingForm()
        If settingForm Is Nothing Then
            settingForm = New Settings(Me)
            settingForm.Show()
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
            settingForm.A = A
            settingForm.B = B
            settingForm.M = M
            settingForm.N1 = N1
            settingForm.N2 = N2
            settingForm.N3 = N3
        Else
            settingForm.Show()
        End If
        settingForm.Update()
    End Sub

    Public Sub GetParameters()
        If settingForm IsNot Nothing Then
            A = settingForm.A
            B = settingForm.B
            M = settingForm.M
            N1 = settingForm.N1
            N2 = settingForm.N2
            N3 = settingForm.N3
        End If
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If Not IsLoaded Then Exit Sub
        settingForm.Left = Me.Left + Me.ActualWidth
        settingForm.Top = Me.Top
    End Sub

    Private Sub Window_LocationChanged(sender As Object, e As EventArgs)
        If Not IsLoaded Then Exit Sub
        settingForm.Left = Me.Left + Me.ActualWidth
        settingForm.Top = Me.Top
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
