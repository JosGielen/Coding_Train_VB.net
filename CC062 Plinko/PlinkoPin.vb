Public Class PlinkoPin
    Private my_Diameter As Double
    Private my_Pos As Vector
    Private my_Ellipse As Ellipse

    Public Sub New(diameter As Double, position As Vector)
        my_Diameter = diameter
        my_Pos = position
        my_Ellipse = New Ellipse() With
        {
            .Width = my_Diameter + 2.0,
            .Height = my_Diameter + 2.0,
            .Stroke = Brushes.Black,
            .StrokeThickness = 1.0,
            .Fill = Brushes.Red
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

    Public Property Diameter As Double
        Get
            Return my_Diameter
        End Get
        Set(value As Double)
            my_Diameter = value
        End Set
    End Property

    Public Sub Draw(c As Canvas)
        c.Children.Add(my_Ellipse)
    End Sub



End Class
