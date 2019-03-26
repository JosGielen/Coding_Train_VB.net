Public Class PlinkoSlot
    Private my_Left As Double
    Private my_Right As Double
    Private my_Count As Integer
    Private my_Height As Double
    Private my_Bottom As Double
    Private my_Rectangle As Rectangle

    Public Sub New(left As Double, right As Double)
        my_Left = left
        my_Right = right
        my_Count = 0
        my_Height = 3.0
        my_Rectangle = New Rectangle() With
        {
        .Stroke = Brushes.Black,
        .StrokeThickness = 1.0,
        .Fill = Brushes.Blue,
        .Width = right - left - 3,
        .Height = 0
        }
    End Sub

    Public Property Left As Double
        Get
            Return my_Left
        End Get
        Set(value As Double)
            my_Left = value
        End Set
    End Property

    Public Property Right As Double
        Get
            Return my_Right
        End Get
        Set(value As Double)
            my_Right = value
        End Set
    End Property

    Public Property Count As Integer
        Get
            Return my_Count
        End Get
        Set(value As Integer)
            my_Count = value
        End Set
    End Property

    Public Sub Draw(c As Canvas)
        my_Bottom = c.ActualHeight
        my_Rectangle.SetValue(Canvas.LeftProperty, my_Left + 3)
        my_Rectangle.SetValue(Canvas.TopProperty, my_Bottom - my_Height)
        c.Children.Add(my_Rectangle)
    End Sub

    Public Sub SetHeight(maxHeight As Double, maxCount As Integer)
        If maxCount > maxHeight / 3 Then
            my_Height = my_Count * maxHeight / maxCount
        Else
            my_Height = 3 * my_Count
        End If
        my_Rectangle.Height = my_Height
        my_Rectangle.SetValue(Canvas.TopProperty, my_Bottom - my_Height)
    End Sub
End Class
