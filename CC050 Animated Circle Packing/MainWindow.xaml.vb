Imports System.Threading

Class MainWindow
    Private LastRenderTime As Date
    Private RenderPeriod As Double = 1  '25 frames per second
    Private Rnd As Random = New Random()
    Private c As Circle
    Private Circles As List(Of Circle)
    Private LoopCounter As Integer
    Private ZeroLoopCounter As Integer
    Private ApplyColor As Boolean = False
    Private Rainbow As List(Of Color)
    Private sharpness As Double = 5.0  'Higher values give more distinct color bands
    Private colorCount As Integer = 256


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        'Some initial code
        Circles = New List(Of Circle)
        LoopCounter = 1
        ZeroLoopCounter = 0
        MakeRainbow()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Do While (Now - LastRenderTime).TotalMilliseconds < RenderPeriod
            Thread.Sleep(1)
        Loop
        Dim X As Double
        Dim Y As Double
        Dim newCircleOK As Boolean
        Dim CircleOverlap As Boolean
        Dim newCircleCounter As Integer

        'Add new circles
        newCircleCounter = 0
        For N As Integer = 0 To LoopCounter
            X = canvas1.ActualWidth * Rnd.NextDouble()
            Y = canvas1.ActualHeight * Rnd.NextDouble()
            newCircleOK = True
            c = New Circle(X, Y)
            'Check if the new circle overlaps existing circles
            For I As Integer = 0 To Circles.Count - 1
                If c.Overlap(Circles(I), 1) Then newCircleOK = False
            Next
            If newCircleOK Then
                c.Draw(canvas1)
                Circles.Add(c)
                newCircleCounter += 1
            End If
        Next
        If newCircleCounter = 0 Then 'No new circles added this render pass
            If LoopCounter < 30 Then LoopCounter += 1 'try adding more circles each render pass
            ZeroLoopCounter += 1
        Else
            ZeroLoopCounter = 0
        End If
        If ZeroLoopCounter = 10 Then 'No more room available
            RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
            If ApplyColor Then ColorCircles()
            Exit Sub
        End If
        'Grow the circles
        CircleOverlap = True
        For I As Integer = 0 To Circles.Count - 1
            CircleOverlap = False
            For J As Integer = 0 To Circles.Count - 1
                If I <> J Then
                    If Circles(I).Overlap(Circles(J), 0) Then CircleOverlap = True
                End If
            Next
            If Not CircleOverlap Then
                If Circles(I).CanGrow() Then Circles(I).Grow()
            End If
        Next
        LastRenderTime = Now()
    End Sub

    Private Sub ColorCircles()
        canvas1.Background = Brushes.Black
        Dim colorindex As Integer = 0
        Dim maxRadius As Double = 0.0
        For I As Integer = 0 To Circles.Count - 1
            If Circles(I).Radius > maxRadius Then maxRadius = Circles(I).Radius
        Next
        For I As Integer = 0 To Circles.Count - 1
            Circles(I).Shape.Stroke = New SolidColorBrush(Rainbow(255 - CInt(colorCount * Circles(I).Radius / maxRadius) Mod 255))
            Circles(I).Shape.Fill = New SolidColorBrush(Rainbow(255 - CInt(colorCount * Circles(I).Radius / maxRadius) Mod 255))
        Next
    End Sub

    Private Sub MakeRainbow()
        Dim r As Byte
        Dim g As Byte
        Dim b As Byte
        Rainbow = New List(Of Color)
        'Fill the Rainbow list
        Rainbow.Clear()
        If sharpness = 0 Then sharpness = 1 / colorCount
        For I As Integer = 0 To colorCount
            If I < colorCount / 5 Then         'Purple To Blue
                r = CByte(155 * (1 - Smooth(0, colorCount / 5, I, sharpness)))
                g = 0
                b = 255
            ElseIf I < 2 * colorCount / 5 Then 'Blue to Cyan
                r = 0
                g = CByte(255 * (Smooth(colorCount / 5, 2 * colorCount / 5, I, sharpness)))
                b = 255
            ElseIf I < 3 * colorCount / 5 Then 'Cyan to Green
                r = 0
                g = 255
                b = CByte(255 * (1 - Smooth(2 * colorCount / 5, 3 * colorCount / 5, I, sharpness)))
            ElseIf I < 4 * colorCount / 5 Then 'Green to Yellow
                r = CByte(255 * (Smooth(3 * colorCount / 5, 4 * colorCount / 5, I, sharpness)))
                g = 255
                b = 0
            Else                               'Yellow to Red
                r = 255
                g = CByte(255 * (1 - Smooth(4 * colorCount / 5, colorCount, I, sharpness)))
                b = 0
            End If
            Rainbow.Add(Color.FromRgb(r, g, b))
        Next
    End Sub

    Private Function Normalize(min As Double, max As Double, X As Double) As Double
        Return (X - min) / (max - min)
    End Function

    Private Function Sigmoid(X As Double) As Double
        Return 1 / (1 + Math.Exp(-1 * X))
    End Function

    Private Function Smooth(min As Double, max As Double, X As Double, Sharpness As Double) As Double
        Dim Xn As Double = (2 * Normalize(min, max, X) - 1) * Sharpness
        Dim Xmin As Double = Sigmoid(-1 * Sharpness)
        Dim Xmax As Double = Sigmoid(Sharpness)
        Return Normalize(Xmin, Xmax, Sigmoid(Xn))
    End Function

End Class
