Public Class Segment
    Private my_StartPt As Vector
    Private my_EndPt As Vector
    Private my_Length As Double
    Private my_Angle As Double
    Private my_TotalAngle As Double
    Private my_Line As Line
    Private my_LineThickness As Double
    Private my_Color As Brush

    Public Sub New(x As Double, y As Double, length As Double, angle As Double)
        my_StartPt = New Vector(x, y)
        my_Length = length
        my_Angle = angle
        TotalAngle = angle
        my_LineThickness = 1.0
        my_Color = Brushes.Red
        my_EndPt = my_StartPt + New Vector(my_Length * Math.Cos(my_Angle * Math.PI / 180), my_Length * Math.Sin(my_Angle * Math.PI / 180))
    End Sub

    Public Property StartPt As Vector
        Get
            Return my_StartPt
        End Get
        Set(value As Vector)
            my_StartPt = value
        End Set
    End Property

    Public ReadOnly Property EndPt As Vector
        Get
            Return my_EndPt
        End Get
    End Property

    Public Property Length As Double
        Get
            Return my_Length
        End Get
        Set(value As Double)
            my_Length = value
        End Set
    End Property

    Public Property Angle As Double
        Get
            Return my_Angle
        End Get
        Set(value As Double)
            my_Angle = value
        End Set
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

    Public Property TotalAngle As Double
        Get
            Return my_TotalAngle
        End Get
        Set(value As Double)
            my_TotalAngle = value
        End Set
    End Property

    Public Sub Update()
        my_TotalAngle = my_Angle
        Dim a As Double = my_TotalAngle * Math.PI / 180
        my_EndPt = my_StartPt + New Vector(my_Length * Math.Cos(a), my_Length * Math.Sin(a))
        my_Line.X1 = my_StartPt.X
        my_Line.Y1 = my_StartPt.Y
        my_Line.X2 = my_EndPt.X
        my_Line.Y2 = my_EndPt.Y
    End Sub

    Public Sub Update(parent As Segment)
        my_StartPt = parent.EndPt
        my_TotalAngle = parent.TotalAngle + my_Angle
        Dim a As Double = my_TotalAngle * Math.PI / 180
        my_EndPt = my_StartPt + New Vector(my_Length * Math.Cos(a), my_Length * Math.Sin(a))
        my_Line.X1 = my_StartPt.X
        my_Line.Y1 = my_StartPt.Y
        my_Line.X2 = my_EndPt.X
        my_Line.Y2 = my_EndPt.Y
    End Sub

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

End Class
