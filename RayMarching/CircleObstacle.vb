Public Class CircleObstacle
    Private my_Pos As Point
    Private my_Speed As Vector
    Private my_Radius As Double
    Private maxWidth As Double
    Private maxHeight As Double
    Private my_Ellipse As Ellipse

    Public Sub New(pos As Point, radius As Double, angle As Double, speed As Double)
        my_Pos = pos
        my_Radius = radius
        my_Speed = speed * New Vector(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180))
    End Sub

    Public Property Pos As Point
        Get
            Return my_Pos
        End Get
        Set(value As Point)
            my_Pos = value
        End Set
    End Property

    Public Property Speed As Vector
        Get
            Return my_Speed
        End Get
        Set(value As Vector)
            my_Speed = value
        End Set
    End Property

    Public Property Radius As Double
        Get
            Return my_Radius
        End Get
        Set(value As Double)
            my_Radius = value
        End Set
    End Property

    Public Sub Show(c As Canvas)
        my_Ellipse = New Ellipse() With
        {
            .Width = 2 * my_Radius,
            .Height = 2 * my_Radius,
            .Stroke = Brushes.White,
            .StrokeThickness = 1.0
        }
        my_Ellipse.SetValue(Canvas.LeftProperty, my_Pos.X - my_Radius)
        my_Ellipse.SetValue(Canvas.TopProperty, my_Pos.Y - my_Radius)
        c.Children.Add(my_Ellipse)
        maxWidth = c.ActualWidth
        maxHeight = c.ActualHeight
    End Sub

    Public Sub HighLite(status As Boolean)
        If status Then
            my_Ellipse.Fill = New SolidColorBrush(Color.FromArgb(100, 255, 100, 0))
        Else
            my_Ellipse.Fill = Brushes.Black
        End If
    End Sub

    Public Sub Update()
        my_Pos = my_Pos + my_Speed
        'Check Wall collisions
        If my_Pos.X < my_Radius Then
            my_Pos.X = my_Radius + 2
            my_Speed.X = -1 * my_Speed.X
        ElseIf my_Pos.X > maxWidth - my_Radius Then
            my_Pos.X = maxWidth - my_Radius - 2
            my_Speed.X = -1 * my_Speed.X
        End If
        If my_Pos.Y < my_Radius Then
            my_Pos.Y = my_Radius + 2
            my_Speed.Y = -1 * my_Speed.Y
        ElseIf my_Pos.Y > maxHeight - my_Radius Then
            my_Pos.Y = maxHeight - my_Radius - 2
            my_Speed.Y = -1 * my_Speed.Y
        End If
        'Check Square Collisions
        my_Ellipse.SetValue(Canvas.LeftProperty, my_Pos.X - my_Radius)
        my_Ellipse.SetValue(Canvas.TopProperty, my_Pos.Y - my_Radius)
    End Sub

    Public Sub Collide(other As CircleObstacle)
        If (my_Pos - other.Pos).Length < (my_Radius + other.Radius) Then
            Dim X1 As Vector = New Vector(my_Pos.X, my_Pos.Y)
            Dim X2 As Vector = New Vector(other.Pos.X, other.Pos.Y)
            Dim CP As Double = (my_Speed - other.Speed) * (X1 - X2)
            Dim V1A As Vector = my_Speed
            Dim V2A As Vector = other.Speed
            V1A = my_Speed - CP / ((X1 - X2).Length ^ 2) * (X1 - X2)
            V2A = other.Speed - CP / ((X2 - X1).Length ^ 2) * (X2 - X1)
            my_Pos = other.Pos + 1.01 * (X1 - X2)
            other.Pos = my_Pos + 1.01 * (X2 - X1)
            my_Speed = V1A
            other.Speed = V2A
        End If
    End Sub

    Public Sub Collide(other As SquareObstacle)
        Dim dx As Double
        Dim dy As Double
        Dim dist As Double
        dx = Math.Abs(my_Pos.X - other.Pos.X) - other.Size / 2
        dy = Math.Abs(my_Pos.Y - other.Pos.Y) - other.Size / 2
        If dx < 0 Then dx = 0
        If dy < 0 Then dy = 0
        dist = Math.Sqrt(dx ^ 2 + dy ^ 2)
        If dist < my_Radius Then
            If dx < my_Radius And dy > 0 Then my_Speed.Y = -1 * my_Speed.Y
            If dy < my_Radius And dx > 0 Then my_Speed.X = -1 * my_Speed.X
        End If
    End Sub

    Public Function Signeddistance(pt As Point) As Double
        Return (my_Pos - pt).Length - my_Radius
    End Function

End Class
