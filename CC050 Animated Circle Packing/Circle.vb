Public Class Circle
    Private my_Center As Point
    Private my_Radius As Double
    Private my_Color As Brush
    Private my_Ellipse As Ellipse
    Private my_Border As Rect

    Public Sub New(X As Double, Y As Double)
        my_Center = New Point(X, Y)
        my_Radius = 1.0
        my_Color = Brushes.Black
        my_Ellipse = New Ellipse() With {
            .Width = 2 * my_Radius,
            .Height = 2 * my_Radius,
            .Stroke = my_Color,
            .StrokeThickness = 1
            }
        my_Ellipse.SetValue(Canvas.LeftProperty, X - my_Radius)
        my_Ellipse.SetValue(Canvas.TopProperty, Y - my_Radius)
        my_Border = New Rect()
    End Sub

    Public ReadOnly Property Center As Point
        Get
            Return my_Center
        End Get
    End Property

    Public ReadOnly Property Radius As Double
        Get
            Return my_Radius
        End Get
    End Property

    Public ReadOnly Property Shape As Ellipse
        Get
            Return my_Ellipse
        End Get
    End Property

    Public Sub Draw(can As Canvas)
        my_Border.X = 0
        my_Border.Y = 0
        my_Border.Width = can.ActualWidth
        my_Border.Height = can.ActualHeight
        can.Children.Add(my_Ellipse)
    End Sub

    Public Sub Grow()
        my_Radius += 0.5
        my_Ellipse.Width = 2 * my_Radius
        my_Ellipse.Height = 2 * my_Radius
        my_Ellipse.SetValue(Canvas.LeftProperty, my_Center.X - my_Radius)
        my_Ellipse.SetValue(Canvas.TopProperty, my_Center.Y - my_Radius)
    End Sub

    Public Function Overlap(other As Circle, spacing As Double)
        Dim dist As Double = Math.Sqrt((other.Center.X - my_Center.X) ^ 2 + (other.Center.Y - my_Center.Y) ^ 2)
        Return dist < my_Radius + other.Radius + spacing
    End Function

    Public Function CanGrow() As Boolean
        Dim result As Boolean = True
        If my_Radius >= my_Center.X Or my_Radius + my_Center.X >= my_Border.X + my_Border.Width Then result = False
        If my_Radius >= my_Center.Y Or my_Radius + my_Center.Y >= my_Border.Y + my_Border.Height Then result = False
        Return result
    End Function
End Class
