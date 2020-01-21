Public Class Marker
    Private my_Shape As Ellipse
    Private my_PlayerNr As Integer
    Private my_Row As Integer
    Private my_Col As Integer

    Public Sub New()
        my_Shape = New Ellipse()
        my_Shape.Stroke = Brushes.Black
        my_Shape.StrokeThickness = 1
        my_Shape.Fill = Brushes.White
    End Sub

    Public Property CelWidth() As Double
        Get
            Return my_Shape.Width
        End Get
        Set(ByVal value As Double)
            my_Shape.Width = value
        End Set
    End Property

    Public Property CelHeight() As Double
        Get
            Return my_Shape.Height
        End Get
        Set(ByVal value As Double)
            my_Shape.Height = value
        End Set
    End Property

    Public Property Shape As Ellipse
        Get
            Return my_Shape
        End Get
        Set(value As Ellipse)
            my_Shape = value
        End Set
    End Property

    Public Property Row As Integer
        Get
            Return my_Row
        End Get
        Set(value As Integer)
            my_Row = value
        End Set
    End Property

    Public Property Col As Integer
        Get
            Return my_Col
        End Get
        Set(value As Integer)
            my_Col = value
        End Set
    End Property

    Public Property PlayerNr As Integer
        Get
            Return my_PlayerNr
        End Get
        Set(value As Integer)
            my_PlayerNr = value
            If value = 0 Then
                my_Shape.Fill = Brushes.White
            ElseIf value = 1 Then
                my_Shape.Fill = Brushes.Yellow
            ElseIf value = 2 Then
                my_Shape.Fill = Brushes.Red
            Else
                Throw New Exception("Invalid player number.")
            End If
        End Set
    End Property

    Public Sub Draw(ByVal c As Canvas, ByVal Xorig As Double, ByVal Yorig As Double)
        my_Shape.SetValue(Canvas.LeftProperty, Xorig)
        my_Shape.SetValue(Canvas.TopProperty, Yorig)
        c.Children.Add(my_Shape)
    End Sub
End Class
