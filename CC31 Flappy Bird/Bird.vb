Public Class Bird
    Private my_X As Double
    Private my_Y As Double
    Private my_Speed As Double
    Private my_Size As Double
    Private my_Alive As Boolean
    Private my_ellipse As Ellipse
    Private my_Canvas As Canvas


    Public Sub New(X As Double, Y As Double, Size As Double, Can As Canvas)
        my_X = X
        my_Y = Y
        my_Size = Size
        my_Canvas = Can
        my_Speed = 0
        my_Alive = True
        my_ellipse = New Ellipse() With {
            .Width = my_Size,
            .Height = my_Size,
            .Fill = Brushes.Red}
        my_ellipse.SetValue(Canvas.LeftProperty, my_X - my_Size / 2)
        my_ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2)
    End Sub

    Public ReadOnly Property Alive As Boolean
        Get
            Return my_Alive
        End Get
    End Property

    Public Sub Draw()
        my_Canvas.Children.Add(my_ellipse)
    End Sub

    Public Sub Update(downspeed)
        my_Speed += downspeed
        my_Y += my_Speed
        If my_Y > my_Canvas.ActualHeight - my_Size Then
            my_Y = my_Canvas.ActualHeight - my_Size
            my_Alive = False
        End If
        If my_Y < my_Size Then
            my_Y = my_Size
            'my_Alive = False    'Die when hit the top??
        End If
        my_ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2)
    End Sub

    Public Sub Flap(upspeed As Double)
        my_Speed -= upspeed
    End Sub

    Public Sub CheckCollision(g As Gate)
        If my_Y < g.GateTop + my_Size / 2 Or my_Y > g.GateBottom - my_Size / 2 Then
            my_Alive = False
        End If
    End Sub


End Class
