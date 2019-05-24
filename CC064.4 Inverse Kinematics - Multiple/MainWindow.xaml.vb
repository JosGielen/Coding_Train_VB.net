Class MainWindow
    Private flags As List(Of Flagellum)
    Private SegmentCount As Integer = 20
    Private FlagellumLength As Double = 300
    Private FlagellaCount As Integer = 2
    Private MousePos As Vector
    Private BallPos As Vector
    Private BallDir As Vector
    Private BallSpeed As Double
    Private BallSize As Double
    Private Gravity As Vector
    Private Ball As Ellipse
    Private Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        flags = New List(Of Flagellum)
        Dim fl As Flagellum
        Dim loc As Vector
        For I As Integer = 0 To FlagellaCount - 1
            loc = New Vector(I / (FlagellaCount - 1) * (canvas1.ActualWidth - 40) + 20, canvas1.ActualHeight - 5)
            fl = New Flagellum(loc, SegmentCount, FlagellumLength, Brushes.Yellow)
            fl.Show(canvas1)
            flags.Add(fl)
        Next
        BallPos = New Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 3)
        BallDir = New Vector(2, Rnd.NextDouble())
        BallSpeed = 3.0
        BallSize = 30.0
        Gravity = New Vector(0, 0.03)
        Ball = New Ellipse() With
        {
            .Width = BallSize,
            .Height = BallSize,
            .Stroke = Brushes.Red,
            .StrokeThickness = 1.0,
            .Fill = Brushes.Red
        }
        canvas1.Children.Add(Ball)
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        For I As Integer = 0 To flags.Count - 1
            flags(I).Follow(BallPos)
        Next
        Ball.SetValue(Canvas.LeftProperty, BallPos.X - BallSize / 2)
        Ball.SetValue(Canvas.TopProperty, BallPos.Y - BallSize / 2)
        BallDir += Gravity
        BallPos += BallSpeed * BallDir
        If BallPos.X < 0 Then
            BallPos.X = 0
            BallDir.X = -1 * BallDir.X
        End If
        If BallPos.X > canvas1.ActualWidth Then
            BallPos.X = canvas1.ActualWidth
            BallDir.X = -1 * BallDir.X
        End If
        If BallPos.Y < 0 Then
            BallPos.Y = 0
            BallDir.Y = -1 * BallDir.Y
        End If
        If BallPos.Y > canvas1.ActualHeight Then
            BallPos.Y = canvas1.ActualHeight
            BallDir.Y = -1 * BallDir.Y
        End If
    End Sub

End Class
