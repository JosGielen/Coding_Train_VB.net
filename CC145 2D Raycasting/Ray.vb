Public Class Ray
    Private my_Pos As Point
    Private my_Dir As Vector
    Private my_Line As Line

    'DEBUG CODE
    Private LineLength As Double = 800

    Public Sub New(pos As Point, angle As Double)
        my_Pos = pos
        my_Dir = New Vector(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180))
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
            .Stroke = New SolidColorBrush(Color.FromArgb(30, 255, 255, 255)), 'Brushes.White,
            .StrokeThickness = 1.0,
            .X1 = my_Pos.X,
            .Y1 = my_Pos.Y,
            .X2 = (my_Pos + 800 * my_Dir).X,
            .Y2 = (my_Pos + 800 * my_Dir).Y
        }
        c.Children.Add(my_Line)
    End Sub

    Public Sub Update(Pos As Point, angle As Double)
        my_Pos = Pos
        my_Line.X1 = my_Pos.X
        my_Line.Y1 = my_Pos.Y
        my_Dir = New Vector(Math.Cos(angle * Math.PI / 180), Math.Sin(angle * Math.PI / 180))
        my_Line.X2 = (my_Pos + LineLength * my_Dir).X
        my_Line.Y2 = (my_Pos + LineLength * my_Dir).Y
    End Sub

End Class
