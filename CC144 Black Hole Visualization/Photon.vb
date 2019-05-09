Public Class Photon
    Private my_Pos As Vector
    Private my_Velocity As Vector
    Private my_Trajectory As List(Of Line)
    Private my_Escaped As Boolean
    Private dot As Ellipse
    Private my_Alive As Boolean
    Private my_Theta As Double
    Private maxWidth As Double
    Private maxHeight As Double

    Public Sub New(x As Double, y As Double, v As Vector)
        my_Pos = New Vector(x, y)
        my_Velocity = v
        my_Alive = True
        my_Escaped = False
        my_Theta = Math.PI
        my_Trajectory = New List(Of Line)
    End Sub

    Public Property Pos As Vector
        Get
            Return my_Pos
        End Get
        Set(value As Vector)
            my_Pos = value
        End Set
    End Property

    Public Property Velocity As Vector
        Get
            Return my_Velocity
        End Get
        Set(value As Vector)
            my_Velocity = value
        End Set
    End Property

    Public Property Theta As Double
        Get
            Return my_Theta
        End Get
        Set(value As Double)
            my_Theta = value
        End Set
    End Property

    Public Property Alive As Boolean
        Get
            Return my_Alive
        End Get
        Set(value As Boolean)
            my_Alive = value
        End Set
    End Property

    Public ReadOnly Property Trajectory As List(Of Line)
        Get
            Return my_Trajectory
        End Get
    End Property

    Public ReadOnly Property Escaped As Boolean
        Get
            Return my_Escaped
        End Get
    End Property

    Public Sub Dead()
        my_Alive = False
        dot.Visibility = Visibility.Hidden
    End Sub

    Public Sub Update(c As Canvas)
        Dim l As Line
        If my_Alive Then
            l = New Line() With
            {
                .Stroke = New SolidColorBrush(Color.FromArgb(150, 255, 255, 255)),
                .StrokeThickness = 1.0,
                .X1 = my_Pos.X,
                .Y1 = my_Pos.Y
            }
            Dim dV As Vector = dt * New Vector(my_Velocity.X, my_Velocity.Y)
            my_Pos = my_Pos + dV
            l.X2 = my_Pos.X
            l.Y2 = my_Pos.Y
            my_Trajectory.Add(l)
            c.Children.Add(l)
            dot.SetValue(Canvas.LeftProperty, my_Pos.X - 2)
            dot.SetValue(Canvas.TopProperty, my_Pos.Y - 2)
            If my_Pos.X < 0 Or my_Pos.X > maxWidth Or my_Pos.Y < 0 Or my_Pos.Y > maxHeight Then
                my_Escaped = True
                Dead()
            End If
        End If
    End Sub

    Public Sub Show(c As Canvas)
        maxWidth = c.ActualWidth
        maxHeight = c.ActualHeight
        dot = New Ellipse() With
        {
            .Width = 4,
            .Height = 4,
            .Stroke = Brushes.White,
            .StrokeThickness = 1.0,
            .Fill = Brushes.White
        }
        dot.SetValue(Canvas.LeftProperty, my_Pos.X - 2)
        dot.SetValue(Canvas.TopProperty, my_Pos.Y - 2)
        c.Children.Add(dot)
    End Sub

End Class
