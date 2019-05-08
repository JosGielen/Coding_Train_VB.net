Public Class Drop
    Private my_X As Double
    Private my_Y As Double
    Private depth As Double
    Private speed As Double
    Private maxSpeed As Double = 12.0
    Private length As Double
    Private maxWidth As Double
    Private maxHeight As Double
    Private dot As Line
    Private Shared rnd As Random = New Random()

    Public Sub New(x As Double, y As Double)
        my_X = x
        my_Y = y
        depth = CircleStep(20 * rnd.NextDouble())
        speed = Map(depth, 0, 20, maxSpeed, 5)
        length = Map(depth, 0, 20, 20, 3)
    End Sub

    Public Sub Show(c As Canvas)
        dot = New Line() With
            {
            .Stroke = New SolidColorBrush(Color.FromRgb(138, 43, 226)),
            .StrokeThickness = Map(depth, 0, 20, 4, 1),
            .X1 = my_X,
            .Y1 = my_Y,
            .X2 = my_X,
            .Y2 = my_Y + length
            }
        maxWidth = c.ActualWidth
        maxHeight = c.ActualHeight
        c.Children.Add(dot)
    End Sub

    Public Sub Update()
        my_Y = my_Y + speed
        If my_Y > maxHeight Then
            depth = CircleStep(20 * rnd.NextDouble())
            speed = Map(depth, 0, 20, maxSpeed, 5)
            length = Map(depth, 0, 20, 20, 3)
            my_X = maxWidth * rnd.NextDouble()
            my_Y = -length
            dot.X1 = my_X
            dot.X2 = my_X
            dot.StrokeThickness = Map(depth, 0, 20, 4, 1)
        End If
        dot.Y1 = my_Y
        dot.Y2 = my_Y + length
    End Sub

    Public Function Map(Value As Double, low As Double, high As Double, min As Double, max As Double) As Double
        Dim result As Double = (Value - low) / (high - low)
        Return result * (max - min) + min
    End Function

    Public Function CircleStep(value As Double) As Double
        Return Math.Sqrt(40 * value - value * value)
    End Function

End Class
