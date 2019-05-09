Public Class Wall
    Private my_Pt1 As Point
    Private my_Pt2 As Point
    Private my_Line As Line
    Private my_Color As Brush

    Public Sub New(end1 As Point, end2 As Point, color As Brush)
        my_Pt1 = end1
        my_Pt2 = end2
        my_Color = color
    End Sub

    Public ReadOnly Property WallColor As Brush
        Get
            Return my_Color
        End Get
    End Property

    Public Sub Show(c As Canvas)
        my_Line = New Line() With
        {
            .Stroke = my_Color,
            .StrokeThickness = 3.0,
            .X1 = my_Pt1.X,
            .Y1 = my_Pt1.Y,
            .X2 = my_Pt2.X,
            .Y2 = my_Pt2.Y
        }
        c.Children.Add(my_Line)
    End Sub

    Public Function Intersect(r As Ray) As Point
        Dim result As Point = New Point()
        Dim nom As Double = (my_Pt1.X - my_Pt2.X) * (r.Y1 - r.Y2) - (my_Pt1.Y - my_Pt2.Y) * (r.X1 - r.X2)
        Dim t As Double = ((my_Pt1.X - r.X1) * (r.Y1 - r.Y2) - (my_Pt1.Y - r.Y1) * (r.X1 - r.X2)) / nom
        Dim u As Double = -((my_Pt1.X - my_Pt2.X) * (my_Pt1.Y - r.Y1) - (my_Pt1.Y - my_Pt2.Y) * (my_Pt1.X - r.X1)) / nom
        If t > 0 And t < 1 Then
            If u > 0 Then
                result.X = my_Pt1.X + t * (my_Pt2.X - my_Pt1.X)
                result.Y = my_Pt1.Y + t * (my_Pt2.Y - my_Pt1.Y)
                Return result
            End If
        End If
        Return New Point(-1, -1)
    End Function

End Class
