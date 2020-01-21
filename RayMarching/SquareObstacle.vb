Public Class SquareObstacle
    Private my_Pos As Point
    Private my_Size As Double
    Private my_Rect As Rectangle

    Public Sub New(pos As Point, size As Double)
        my_Pos = pos
        my_Size = size
    End Sub

    Public Property Pos As Point
        Get
            Return my_Pos
        End Get
        Set(value As Point)
            my_Pos = value
        End Set
    End Property

    Public Property Size As Double
        Get
            Return my_Size
        End Get
        Set(value As Double)
            my_Size = value
        End Set
    End Property

    Public Sub Show(c As Canvas)
        my_Rect = New Rectangle() With
        {
            .Width = my_Size,
            .Height = my_Size,
            .Stroke = Brushes.White,
            .StrokeThickness = 1.0,
            .Fill = Brushes.White
        }
        my_Rect.SetValue(Canvas.LeftProperty, my_Pos.X - my_Size / 2)
        my_Rect.SetValue(Canvas.TopProperty, my_Pos.Y - my_Size / 2)
        c.Children.Add(my_Rect)
    End Sub

    Public Sub HighLite(status As Boolean)
        If status Then
            my_Rect.Fill = New SolidColorBrush(Color.FromArgb(100, 150, 255, 0))
        Else
            my_Rect.Fill = Brushes.White
        End If
    End Sub

    Public Function Signeddistance(pt As Point) As Double
        Dim dx As Double
        Dim dy As Double
        dx = Math.Abs(my_Pos.X - pt.X) - my_Size / 2
        dy = Math.Abs(my_Pos.Y - pt.Y) - my_Size / 2
        If dx < 0 Then dx = 0
        If dy < 0 Then dy = 0
        Return Math.Sqrt(dx ^ 2 + dy ^ 2)
    End Function

End Class
