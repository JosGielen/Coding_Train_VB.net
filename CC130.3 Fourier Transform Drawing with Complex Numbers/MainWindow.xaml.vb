Imports System.IO

Class MainWindow
    Private Rendering As Boolean = False
    Private Signal As List(Of Point)
    Private Epicycles As List(Of Epicycle)
    Private Circles As List(Of Ellipse)
    Private Radia As List(Of Line)
    Private dot As Ellipse
    Private newPt As Point
    Private p As Polyline
    Private Time As Double
    Private TimeStep As Double
    Private my_MouseDown As Boolean
    Private previousPt As Point

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        my_MouseDown = False
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        If my_MouseDown Or Not Rendering Then Exit Sub
        'Draw the Epicycles
        DrawEpicycles()
        'Add the new polyline point to the polyline
        p.Points.Add(newPt)
        Time += TimeStep
        If Time >= 2 * Math.PI Then 'Reset the drawing
            Time = 0
            p.Points.Clear()
        End If
    End Sub

    Private Sub DrawEpicycles()
        Dim X As Double
        Dim Y As Double
        Dim PreviousX As Double
        Dim PreviousY As Double
        Dim Freq As Double
        Dim Amp As Double
        Dim Phase As Double
        'Draw the Epicycles
        X = canvas1.ActualWidth / 2
        Y = canvas1.ActualHeight / 2
        PreviousX = X
        PreviousY = Y
        For I As Integer = 0 To Epicycles.Count - 1
            Freq = Epicycles(I).Freqency
            Amp = Epicycles(I).Amplitude
            Phase = Epicycles(I).Phase
            X += Amp * Math.Cos(Freq * Time + Phase)
            Y += Amp * Math.Sin(Freq * Time + Phase)
            Circles(I).Width = 2 * Amp
            Circles(I).Height = 2 * Amp
            Circles(I).SetValue(Canvas.LeftProperty, PreviousX - Amp)
            Circles(I).SetValue(Canvas.TopProperty, PreviousY - Amp)
            Radia(I).X1 = PreviousX
            Radia(I).Y1 = PreviousY
            Radia(I).X2 = X
            Radia(I).Y2 = Y
            PreviousX = X
            PreviousY = Y
        Next
        'Final point of Epicycles.
        dot.SetValue(Canvas.LeftProperty, X - 4)
        dot.SetValue(Canvas.TopProperty, Y - 4)
        newPt = New Point(X, Y)
    End Sub

    Private Sub StartRender()
        BtnStart.Content = "STOP"
        Rendering = True
    End Sub

    Private Sub StopRender()
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        BtnStart.Content = "START"
        Rendering = False
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            StartRender()
        Else
            StopRender()
        End If
    End Sub

    Private Sub Window_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If Not Rendering Then Exit Sub
        my_MouseDown = True
        Time = 0
        Circles = New List(Of Ellipse)
        Radia = New List(Of Line)
        canvas1.Children.Clear()
        Signal = New List(Of Point)
        previousPt = e.GetPosition(canvas1)
    End Sub

    Private Sub Window_MouseMove(sender As Object, e As MouseEventArgs)
        If Not Rendering Then Exit Sub
        Dim l As Line
        Dim X As Double
        Dim Y As Double
        If my_MouseDown Then
            X = e.GetPosition(canvas1).X
            Y = e.GetPosition(canvas1).Y
            Signal.Add(New Point(X - canvas1.ActualWidth / 2, Y - canvas1.ActualHeight / 2))
            l = New Line() With
            {
                .Stroke = Brushes.Gray,
                .StrokeThickness = 1.0,
                .X1 = previousPt.X,
                .Y1 = previousPt.Y,
                .X2 = X,
                .Y2 = Y
            }
            canvas1.Children.Add(l)
            previousPt = New Point(X, Y)
        End If
    End Sub

    Private Sub Window_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If Not Rendering Then Exit Sub
        my_MouseDown = False
        'Skip 9 out of 10 points
        Debug.Print(Signal.Count.ToString())

        Dim ReducedSignal As List(Of Point) = New List(Of Point)
        For I As Integer = 0 To Signal.Count - 1
            If I Mod 10 = 0 Then ReducedSignal.Add(Signal(I))
        Next
        Debug.Print(ReducedSignal.Count.ToString())

        'Calculate the Epicycle data from the Signals with the Discrete Fourier Transform
        Epicycles = DFT.GetSortedEpicycles(ReducedSignal, False)
        'Create the Epicycle visualisation (Circles with a Radius line)
        Dim Ex As Ellipse
        Dim Lx As Line
        For I As Integer = 0 To Epicycles.Count - 1
            Ex = New Ellipse() With
            {
                .Stroke = New SolidColorBrush(Color.FromRgb(80, 80, 80)),
                .StrokeThickness = 1
            }
            Circles.Add(Ex)
            canvas1.Children.Add(Ex)
            Lx = New Line With
            {
                .Stroke = Brushes.Yellow,
                .StrokeThickness = 1
            }
            Radia.Add(Lx)
            canvas1.Children.Add(Lx)
        Next
        'Create the endpoint indicator dot
        dot = New Ellipse With
        {
            .Stroke = Brushes.Red,
            .StrokeThickness = 1,
            .Fill = Brushes.Red,
            .Width = 6,
            .Height = 6
        }
        canvas1.Children.Add(dot)
        'Create the resulting drawing
        p = New Polyline() With
        {
            .Stroke = Brushes.White,
            .StrokeThickness = 1
        }
        canvas1.Children.Add(p)
        TimeStep = 2 * Math.PI / Epicycles.Count
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub
End Class



