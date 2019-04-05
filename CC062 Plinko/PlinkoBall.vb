Public Class PlinkoBall
    Private my_Diameter As Double
    Private my_Pos As Vector
    Private my_Velocity As Vector
    Private my_Ellipse As Ellipse
    Private my_maxX As Double
    Private isLocked As Boolean = False

    Public Sub New(diameter As Double, position As Vector)
        my_Diameter = diameter
        my_Pos = position
        my_Ellipse = New Ellipse() With
        {
            .Width = my_Diameter,
            .Height = my_Diameter,
            .Stroke = Brushes.Black,
            .StrokeThickness = 1.0,
            .Fill = Brushes.Green
        }
        my_Ellipse.SetValue(Canvas.LeftProperty, my_Pos.X - my_Diameter / 2)
        my_Ellipse.SetValue(Canvas.TopProperty, my_Pos.Y - my_Diameter / 2)
    End Sub

    Public Property Position As Vector
        Get
            Return my_Pos
        End Get
        Set(value As Vector)
            my_Pos = value
        End Set
    End Property

    Public Sub Lock()
        isLocked = True
    End Sub

    Public Sub Draw(c As Canvas)
        c.Children.Add(my_Ellipse)
        my_maxX = c.ActualWidth
    End Sub

    Public Sub Remove(c As Canvas)
        c.Children.Remove(my_Ellipse)
    End Sub

    Public Sub Update(gravity As Vector, Elasticity As Double, pins As List(Of PlinkoPin))
        Dim dist As Double = 0.0
        my_Velocity = my_Velocity + gravity
        If isLocked Then my_Velocity.X = 0
        my_Pos = my_Pos + my_Velocity
        For I As Integer = 0 To pins.Count - 1
            dist = Math.Sqrt((pins(I).Position.X - my_Pos.X) ^ 2 + (pins(I).Position.Y - my_Pos.Y) ^ 2)
            If dist < (pins(I).Diameter + my_Diameter) / 2 Then
                Dim norm As Vector = my_Pos - pins(I).Position
                norm.Normalize()
                my_Velocity = Elasticity / 100 * my_Velocity.Length * norm
                my_Pos = pins(I).Position + ((pins(I).Diameter + my_Diameter) / 2 + 1) * norm
            End If
            If my_Pos.X <= my_Diameter Or my_Pos.X >= my_maxX Then my_Velocity.X = -1 * my_Velocity.X
        Next
        my_Ellipse.SetValue(Canvas.LeftProperty, my_Pos.X - my_Diameter / 2)
        my_Ellipse.SetValue(Canvas.TopProperty, my_Pos.Y - my_Diameter / 2)
    End Sub

End Class
