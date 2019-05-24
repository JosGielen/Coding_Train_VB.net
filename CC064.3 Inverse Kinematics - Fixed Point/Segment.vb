Public Class Segment
    Private my_StartPt As Vector
    Private my_EndPt As Vector
    Private my_Length As Double
    Private my_Angle As Double
    Private my_Line As Line
    Private my_LineThickness As Double
    Private my_Color As Brush
    Private Shared Rnd As Random = New Random()

    Public Sub New(x As Double, y As Double, length As Double)
        my_StartPt = New Vector(x, y)
        my_Length = length
        my_Angle = 0.0
        my_LineThickness = 1.0
        my_Color = Brushes.Red
        my_EndPt = my_StartPt + New Vector(my_Length * Math.Cos(my_Angle * Math.PI / 180), my_Length * Math.Sin(my_Angle * Math.PI / 180))
    End Sub

    Public ReadOnly Property StartPt As Vector
        Get
            Return my_StartPt
        End Get
    End Property

    Public ReadOnly Property EndPt As Vector
        Get
            Return my_EndPt
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
        Dim dir As Vector = target - my_StartPt
        my_Angle = Vector.AngleBetween(New Vector(1, 0), dir)
        dir.Normalize()
        my_EndPt = target
        my_StartPt = target - my_Length * dir
        Update()
    End Sub

    Public Sub SetStart(pos As Vector)
        my_StartPt = pos
        my_EndPt = my_StartPt + New Vector(my_Length * Math.Cos(my_Angle * Math.PI / 180), my_Length * Math.Sin(my_Angle * Math.PI / 180))
        Update()
    End Sub

    Public Sub Update()
        my_Line.X1 = my_StartPt.X
        my_Line.Y1 = my_StartPt.Y
        my_Line.X2 = my_EndPt.X
        my_Line.Y2 = my_EndPt.Y
    End Sub

End Class
