Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 10
    Private rnd As Random = New Random()
    Dim ball As PlinkoBall
    Private balls As List(Of PlinkoBall)
    Private pins As List(Of PlinkoPin)
    Private pinDiameter As Double = 15.0
    Private ballDiameter As Double = 25.0
    Private ballPos As Vector
    Private pinPos As Vector
    Private vel As Vector
    Private gravity As Vector = New Vector(0.0, 0.1)
    Private elasticity As Double = 60.0 '% of kinetic energy remaining after collision


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim pin As PlinkoPin
        Dim pos As Vector
        Dim Xdist As Double = canvas1.ActualWidth / 13
        pins = New List(Of PlinkoPin)
        balls = New List(Of PlinkoBall)
        For row As Integer = 1 To 10
            pos = New Vector()
            pos.Y = row * (canvas1.ActualHeight - 150) / 10 + 50
            If row Mod 2 = 0 Then
                pos.X = 0
                For col As Integer = 1 To 12
                    pos.X += Xdist
                    pin = New PlinkoPin(pinDiameter, pos)
                    pins.Add(pin)
                    pin.Draw(canvas1)
                Next
            Else
                pos.X = Xdist / 2
                For col As Integer = 1 To 11
                    pos.X += Xdist
                    pin = New PlinkoPin(pinDiameter, pos)
                    pins.Add(pin)
                    pin.Draw(canvas1)
                Next
            End If
        Next
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        NewBall()
    End Sub

    Private Sub NewBall()
        ball = New PlinkoBall(ballDiameter, New Vector(canvas1.ActualWidth / 2 + (pinDiameter + ballDiameter) * (rnd.NextDouble() - 0.5), 20.0))
        ball.Draw(canvas1)
        Render()
    End Sub

    Private Sub Render()
        Do
            ball.Update(gravity, elasticity, pins)
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
            If ball.Position.Y >= canvas1.ActualHeight - ballDiameter / 2 Then
                Exit Do
            End If
        Loop
        NewBall()
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
