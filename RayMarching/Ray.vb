Public Class Ray
    Private my_Pos As Point
    Private currentPos As Point
    Private my_Angle As Double
    Private my_Dir As Vector
    Private my_Line As Line
    Private maxWidth As Double
    Private maxHeight As Double

    Public Sub New(pos As Point, angle As Double)
        my_Pos = pos
        currentPos = pos
        my_Angle = angle
        my_Dir = New Vector(Math.Cos(my_Angle * Math.PI / 180), Math.Sin(my_Angle * Math.PI / 180))
    End Sub

    Public Property Pos As Point
        Get
            Return my_Pos
        End Get
        Set(value As Point)
            my_Pos = value
        End Set
    End Property

    Public Property Dir As Vector
        Get
            Return my_Dir
        End Get
        Set(value As Vector)
            my_Dir = value
        End Set
    End Property

    Public ReadOnly Property X1 As Double
        Get
            Return my_Pos.X
        End Get
    End Property

    Public ReadOnly Property Y1 As Double
        Get
            Return my_Pos.Y
        End Get
    End Property

    Public Property X2 As Double
        Get
            Return (my_Pos + my_Dir).X
        End Get
        Set(value As Double)
            my_Line.X2 = value
        End Set
    End Property

    Public Property Y2 As Double
        Get
            Return (my_Pos + my_Dir).Y
        End Get
        Set(value As Double)
            my_Line.Y2 = value
        End Set
    End Property

    Public Sub Show(c As Canvas)
        my_Line = New Line() With
        {
            .Stroke = Brushes.White,
            .StrokeThickness = 1.0,
            .X1 = my_Pos.X,
            .Y1 = my_Pos.Y,
            .X2 = (my_Pos + my_Dir).X,
            .Y2 = (my_Pos + my_Dir).Y
        }
        c.Children.Add(my_Line)
        maxWidth = c.ActualWidth
        maxHeight = c.ActualHeight
    End Sub

    Public Sub Rotate(angle As Double)
        my_Angle += angle
        my_Dir = New Vector(Math.Cos(my_Angle * Math.PI / 180), Math.Sin(my_Angle * Math.PI / 180))
    End Sub

End Class
