
Public Class CubeletFace

    Public Size As Double
    Public Rect As Rectangle
    Public Left As Double
    Public Top As Double
    Private ReadOnly fixed As Boolean
    Private my_FaceColor As Color
    Private ReadOnly my_Colors(5) As Color
    Private my_FaceColorNumber As Integer

    Public Sub New(_ColorNumber As Integer, _left As Double, _top As Double, _size As Double, _fixed As Boolean)
        my_FaceColorNumber = _ColorNumber
        Size = _size
        Left = _left
        Top = _top
        fixed = _fixed
        my_Colors(0) = Color.FromRgb(255, 255, 255)
        my_Colors(1) = Color.FromRgb(255, 255, 0)
        my_Colors(2) = Color.FromRgb(0, 255, 0)
        my_Colors(3) = Color.FromRgb(0, 0, 255)
        my_Colors(4) = Color.FromRgb(255, 180, 0)
        my_Colors(5) = Color.FromRgb(255, 0, 0)
        my_FaceColor = my_Colors(_ColorNumber)
        Rect = New Rectangle() With
        {
            .Width = _size,
            .Height = _size,
            .Stroke = Brushes.Black,
            .StrokeThickness = 2.0,
            .Fill = New SolidColorBrush(my_FaceColor)
        }
        Rect.SetValue(Canvas.LeftProperty, _left)
        Rect.SetValue(Canvas.TopProperty, _top)
    End Sub

    Public Property FaceColor As Color
        Get
            Return my_FaceColor
        End Get
        Set(value As Color)
            my_FaceColor = value
            Rect.Fill = New SolidColorBrush(my_FaceColor)
        End Set
    End Property

    Public Property FaceColorNumber As Integer
        Get
            Return my_FaceColorNumber
        End Get
        Set(value As Integer)
            my_FaceColorNumber = value
            my_FaceColor = my_Colors(my_FaceColorNumber)
            Rect.Fill = New SolidColorBrush(my_Colors(my_FaceColorNumber))
        End Set
    End Property

    Public Function Contains(pt As Point) As Boolean
        Return Left < pt.X And Left + Size > pt.X And Top < pt.Y And Top + Size > pt.Y
    End Function

    Public Sub ToggleColor()
        If Not fixed Then
            my_FaceColorNumber += 1
            If my_FaceColorNumber > 5 Then my_FaceColorNumber = 0
            my_FaceColor = my_Colors(my_FaceColorNumber)
            Rect.Fill = New SolidColorBrush(my_FaceColor)
        End If
    End Sub

    Public Sub Draw(c As Canvas)
        c.Children.Add(Rect)
    End Sub

End Class
