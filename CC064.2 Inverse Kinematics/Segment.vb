Public Class Segment
    Private my_StartPt As Vector
    Private my_EndPt As Vector
    Private my_Length As Double
    Private my_Line As Line
    Private my_LineThickness As Double
    Private my_Color As Brush

    Public Sub New(length As Double)
        my_StartPt = New Vector(0, 0)
        my_Length = length
        my_LineThickness = 1.0
        my_Color = Brushes.Red
        my_EndPt = my_StartPt
    End Sub

    Public ReadOnly Property StartPt As Vector
        Get
            Return my_StartPt
        End Get
    End Property

    Public Property LineThickness As Double
        Get
            Return my_LineThickness
        End Get
        Set(value As Double)
            my_LineThickness = value
        End Set
    End Property

    Public Property LineColor As Brush
        Get
            Return my_Color
        End Get
        Set(value As Brush)
            my_Color = value
        End Set
    End Property

    Public Sub Show(c As Canvas)
        my_Line = New Line() With
        {
            .Stroke = my_Color,
            .StrokeThickness = my_LineThickness,
            .X1 = my_StartPt.X,
            .Y1 = my_StartPt.Y,
            .X2 = my_EndPt.X,
            .Y2 = my_EndPt.Y
        }
        c.Children.Add(my_Line)
    End Sub

    Public Sub Follow(target As Vector)
        If target <> my_StartPt Then
            Dim dir As Vector = target - my_StartPt
            dir.Normalize()
            my_EndPt = target
            my_StartPt = target - my_Length * dir
            my_Line.X1 = my_StartPt.X
            my_Line.Y1 = my_StartPt.Y
            my_Line.X2 = my_EndPt.X
            my_Line.Y2 = my_EndPt.Y
        End If
    End Sub

End Class
