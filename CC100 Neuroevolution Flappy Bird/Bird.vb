Public Class Bird
    Private my_X As Double
    Private my_Y As Double
    Private my_Speed As Double
    Private my_Upspeed As Double
    Private my_Size As Double
    Private my_Alive As Boolean
    Private my_Score As Integer
    Private my_Fitness As Double
    Private my_ellipse As Ellipse
    Private my_Canvas As Canvas
    Private my_Brain As NeuralNet

    Public Sub New(X As Double, Y As Double, Size As Double, Can As Canvas)
        my_X = X
        my_Y = Y
        my_Size = Size
        my_Canvas = Can
        my_Speed = 0.0
        my_Score = 0
        my_Fitness = 0.0
        my_Alive = True
        my_ellipse = New Ellipse() With {
            .Width = my_Size,
            .Height = my_Size,
            .Fill = Brushes.Red}
        my_ellipse.SetValue(Canvas.LeftProperty, my_X - my_Size / 2)
        my_ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2)
    End Sub

    Public Sub SetBrain(NN As NeuralNet)
        my_Brain = NN.Copy
    End Sub

    Public Sub Mutate(mutateRate As Double, mutateFactor As Double)
        my_Brain.Mutate(mutateRate, mutateFactor)
    End Sub

    Public Property Y As Double
        Get
            Return my_Y
        End Get
        Set(value As Double)
            my_Y = value
        End Set
    End Property

    Public Property Alive As Boolean
        Get
            Return my_Alive
        End Get
        Set(value As Boolean)
            my_Alive = value
        End Set
    End Property

    Public Property UpSpeed As Double
        Get
            Return my_Upspeed
        End Get
        Set(value As Double)
            my_Upspeed = value
        End Set
    End Property

    Public Property Speed As Double
        Get
            Return my_Speed
        End Get
        Set(value As Double)
            my_Speed = value
        End Set
    End Property

    Public ReadOnly Property Brain As NeuralNet
        Get
            Return my_Brain
        End Get
    End Property

    Public Property Score As Integer
        Get
            Return my_Score
        End Get
        Set(value As Integer)
            my_Score = value
        End Set
    End Property

    Public Property Fitness As Double
        Get
            Return my_Fitness
        End Get
        Set(value As Double)
            my_Fitness = value
        End Set
    End Property

    Public Sub Draw()
        my_Canvas.Children.Add(my_ellipse)
    End Sub

    Public Sub Remove()
        my_Canvas.Children.Remove(my_ellipse)
    End Sub

    Public Sub Think(g As Gate)
        Dim inputs(4) As Double
        Dim output(1) As Double
        inputs(0) = my_Y / my_Canvas.ActualHeight
        inputs(1) = g.GateTop / my_Canvas.ActualHeight
        inputs(2) = g.GateBottom / my_Canvas.ActualHeight
        inputs(3) = 0.5 * Math.Abs(g.X - my_X) / my_Canvas.ActualWidth
        inputs(4) = my_Speed / 10
        output = my_Brain.Query(inputs)
        If output(0) > output(1) Then Flap()
    End Sub

    Public Sub Update(downspeed)
        my_Speed += downspeed
        my_Y += my_Speed
        If my_Y > my_Canvas.ActualHeight - my_Size Then
            my_Y = my_Canvas.ActualHeight - my_Size
            my_Alive = False     'Die when hit the bottom?
        End If
        If my_Y < my_Size Then
            my_Y = my_Size
            my_Alive = False    'Die when hit the top?
        End If
        my_ellipse.SetValue(Canvas.TopProperty, my_Y - my_Size / 2)
    End Sub

    Public Sub Flap()
        my_Speed -= my_Upspeed
    End Sub

    Public Sub CheckCollision(g As Gate)
        If my_Y < g.GateTop + my_Size / 2 Or my_Y > g.GateBottom - my_Size / 2 Then
            my_Alive = False     'Die when hit a gate
            'Improve the score for dying close to the gate
            my_Score += CInt(my_Canvas.ActualHeight / Math.Abs((g.GateTop + g.GateBottom) / 2 - my_Y))
        End If
    End Sub

    Public Function copy() As Bird
        Dim result As Bird = New Bird(my_X, my_Canvas.ActualHeight / 2, my_Size, my_Canvas)
        result.SetBrain(my_Brain.Copy())
        result.UpSpeed = my_Upspeed
        Return result
    End Function

    Public Sub SaveNN(file As String)
        my_Brain.Save(file)
    End Sub

    Public Sub LoadNN(file As String)
        my_Brain = NeuralNet.FromFile(file)
    End Sub

End Class
