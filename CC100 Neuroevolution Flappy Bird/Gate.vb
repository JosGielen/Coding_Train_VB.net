Public Class Gate
    Private my_X As Double
    Private my_W As Double
    Private my_Space As Double
    Private my_UpperY As Double
    Private my_LowerY As Double
    Private my_Rect1 As Rectangle
    Private my_Rect2 As Rectangle
    Private my_Canvas As Canvas
    Shared Rnd As Random = New Random

    Public Sub New(X As Double, W As Double, space As Double, can As Canvas)
        my_X = X
        my_W = W
        my_Space = space
        my_Canvas = can
        my_UpperY = (0.8 * can.ActualHeight - space) * Rnd.NextDouble() + 0.1 * can.ActualHeight 'between 0.1*Height and (0.9*Height - space)
        my_LowerY = my_UpperY + space
        my_Rect1 = New Rectangle() With {
            .Width = my_W,
            .Height = my_UpperY,
            .Fill = Brushes.Yellow}
        my_Rect2 = New Rectangle() With {
            .Width = my_W,
            .Height = my_Canvas.ActualHeight - my_LowerY,
            .Fill = Brushes.Yellow}
        my_Rect1.SetValue(Canvas.LeftProperty, my_X)
        my_Rect1.SetValue(Canvas.TopProperty, 0.0)
        my_Rect2.SetValue(Canvas.LeftProperty, my_X)
        my_Rect2.SetValue(Canvas.TopProperty, my_LowerY)
    End Sub

    Public ReadOnly Property X As Double
        Get
            Return my_X
        End Get
    End Property

    Public ReadOnly Property GateTop As Double
        Get
            Return my_UpperY
        End Get
    End Property

    Public ReadOnly Property GateBottom As Double
        Get
            Return my_LowerY
        End Get
    End Property

    Public Sub Draw()
        my_Canvas.Children.Add(my_Rect1)
        my_Canvas.Children.Add(my_Rect2)
    End Sub

    Public Sub Update(speed As Double)
        my_X -= speed
        my_Rect1.SetValue(Canvas.LeftProperty, my_X)
        my_Rect2.SetValue(Canvas.LeftProperty, my_X)

    End Sub

    Public Sub Reset()
        my_X = my_Canvas.ActualWidth
        my_UpperY = (0.8 * my_Canvas.ActualHeight - my_Space) * Rnd.NextDouble() + 0.1 * my_Canvas.ActualHeight 'between 0.1*Height and (0.9*Height - space)
        my_LowerY = my_UpperY + my_Space
        my_Rect1.Height = my_UpperY
        my_Rect2.Height = my_Canvas.ActualHeight - my_LowerY
        my_Rect1.SetValue(Canvas.LeftProperty, my_X)
        my_Rect2.SetValue(Canvas.LeftProperty, my_X)
        my_Rect2.SetValue(Canvas.TopProperty, my_LowerY)
    End Sub
End Class
