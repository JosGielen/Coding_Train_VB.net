Public Class Star
    Private pos As Point
    Private size As Double
    Private maxWidth As Double
    Private maxHeight As Double
    Private dot As Ellipse
    Private Shared rnd As Random = New Random()

    Public Sub New(x As Double, y As Double, starSize As Double)
        pos = New Point(x, y)
        size = starSize
    End Sub

    Public Sub Show(c As Canvas)
        dot = New Ellipse() With
            {
            .Width = size,
            .Height = size,
            .Stroke = Brushes.White,
            .StrokeThickness = 1.0,
            .Fill = Brushes.White
            }
        dot.SetValue(Canvas.LeftProperty, pos.X)
        dot.SetValue(Canvas.TopProperty, pos.Y)
        maxWidth = c.ActualWidth
        maxHeight = c.ActualHeight
        c.Children.Add(dot)
    End Sub

    Public Sub Update(speed As Double)
        Dim v As Vector = pos - New Point(maxWidth / 2, maxHeight / 2)
        pos = pos + 0.01 * speed * v
        If pos.X < 0 Or pos.Y > maxWidth Or pos.Y < 0 Or pos.Y > maxHeight Then
            pos.X = 0.8 * maxWidth * rnd.NextDouble()
            pos.Y = 0.8 * maxHeight * rnd.NextDouble()
            dot.Width = 0
            dot.Height = 0
        End If
        dot.Width += 0.02 * speed
        dot.Height += 0.02 * speed
        dot.SetValue(Canvas.LeftProperty, pos.X)
        dot.SetValue(Canvas.TopProperty, pos.Y)
    End Sub

End Class
