Public Class Particle
    Private my_X As Double
    Private my_Y As Double
    Private my_VX As Double
    Private my_VY As Double
    Private my_Size As Double
    Private my_Alpha As Double
    Private my_Ellipse As Ellipse

    Public Sub New(X As Double, Y As Double, VX As Double, VY As Double, size As Double)
        my_X = X
        my_Y = Y
        my_VX = VX
        my_VY = VY
        my_Size = size
        my_Alpha = 100.0
        my_Ellipse = New Ellipse() With
        {
            .Width = size,
            .Height = size,
            .Fill = New SolidColorBrush(Color.FromArgb(CByte(my_Alpha), 255, 255, 255))
        }
        my_Ellipse.SetValue(Canvas.LeftProperty, my_X - my_Size / 2)
        my_Ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2)
    End Sub

    Public ReadOnly Property Alpha As Double
        Get
            Return my_Alpha
        End Get
    End Property

    Public Sub Update(dVX As Double)
        my_VX += dVX
        my_X += my_VX
        my_Y -= my_VY
        my_Size += 0.06 * my_VY
        my_Ellipse.Width = my_Size
        my_Ellipse.Height = my_Size
        my_Ellipse.SetValue(Canvas.LeftProperty, my_X - my_Size / 2)
        my_Ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2)
        my_Alpha *= 0.99
        If my_Alpha < 1 Then my_Alpha = 0
        my_Ellipse.Fill.Opacity = my_Alpha / 100
    End Sub

    Public Sub Draw(c As Canvas)
        c.Children.Add(my_Ellipse)
    End Sub

End Class
