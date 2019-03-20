
Class MainWindow

    Private Rendering As Boolean = False
    Private Wave As List(Of Double)
    Private radius As Double = 80
    Private center As Point
    Private startX As Double = 5 * radius
    Private X As Double
    Private Y As Double
    Private K As Integer = 5
    Private PtsPerCycle As Integer = 100
    Private DeltaX As Double
    Private Time As Double
    Private TimeStep As Double

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Wave = New List(Of Double)
        radius = 80
        center = New Point(2.5 * radius, canvas1.ActualHeight / 2)
        startX = 5.5 * radius
        TimeStep = 0.02
        sldIterations.Value = K
        TxtIterations.Text = K.ToString()
        DeltaX = (canvas1.ActualWidth - startX) / (6 * Math.PI / TimeStep)
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Dim N As Integer = 0
        X = center.X
        Y = center.Y
        Dim PreviousX As Double = center.X
        Dim PreviousY As Double = center.Y
        Dim R As Double
        Dim El As Ellipse
        Dim cl As Line
        Dim dot As Ellipse
        canvas1.Children.Clear()
        For I As Integer = 0 To K - 1
            N = 2 * I + 1
            R = radius * 4 / (N * Math.PI)
            X += R * Math.Cos(N * Time)
            Y += R * Math.Sin(N * Time)
            El = New Ellipse() With 'Epicycle
            {
                .Stroke = Brushes.Gray,
                .StrokeThickness = 1,
                .Width = 2 * R,
                .Height = 2 * R
            }
            El.SetValue(Canvas.LeftProperty, PreviousX - R)
            El.SetValue(Canvas.TopProperty, PreviousY - R)
            canvas1.Children.Add(El)
            cl = New Line() With 'Radius line of the Epicycle
            {
                .Stroke = Brushes.Yellow,
                .StrokeThickness = 1,
                .X1 = PreviousX,
                .Y1 = PreviousY,
                .X2 = X,
                .Y2 = Y
            }
            canvas1.Children.Add(cl)
            PreviousX = X
            PreviousY = Y
        Next
        Wave.Insert(0, Y)
        'Final point that draws the wave.
        dot = New Ellipse() With
        {
            .Stroke = Brushes.Yellow,
            .StrokeThickness = 1,
            .Fill = Brushes.Yellow,
            .Width = 8,
            .Height = 8
        }
        dot.SetValue(Canvas.LeftProperty, X - 4)
        dot.SetValue(Canvas.TopProperty, Y - 4)
        canvas1.Children.Add(dot)
        Dim p As Polyline = New Polyline() With
        {
            .Stroke = Brushes.White,
            .StrokeThickness = 1
        }
        For I As Integer = Wave.Count - 1 To 0 Step -1
            p.Points.Add(New Point(startX + I * DeltaX, Wave(I)))
        Next
        canvas1.Children.Add(p)
        Dim l2 As Line = New Line() With
        {
            .Stroke = Brushes.Red,
            .StrokeThickness = 1,
            .X1 = X,
            .Y1 = Y,
            .X2 = startX,
            .Y2 = Y
        }
        canvas1.Children.Add(l2)
        If Wave.Count > 6 * Math.PI / TimeStep Then
            Wave.Remove(Wave.Last)
        End If
        Time += TimeStep
    End Sub

    Private Sub StartRender()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        BtnStart.Content = "Stop"
        Rendering = True
    End Sub

    Private Sub StopRender()
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        Rendering = False
    End Sub

    Private Sub sldIterations_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Not IsLoaded Then Exit Sub
        K = sldIterations.Value
        TxtIterations.Text = K.ToString()
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            StartRender()
        Else
            StopRender()
        End If
    End Sub
End Class
