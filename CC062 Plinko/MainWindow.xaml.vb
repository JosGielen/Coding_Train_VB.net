Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 10
    Private rnd As Random = New Random()
    Private PinsPerRow As Integer = 10
    Private balls As List(Of PlinkoBall)
    Private pins As List(Of PlinkoPin)
    Private slots As List(Of PlinkoSlot)
    Private pinDiameter As Double = 13
    Private ballDiameter As Double = 25.0
    Private ballPos As Vector
    Private pinPos As Vector
    Private vel As Vector
    Private gravity As Vector = New Vector(0.0, 0.1)
    Private elasticity As Double = 50.0 '% of kinetic energy remaining after collision
    Private frameCounter As Integer = 0


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim pin As PlinkoPin
        Dim slotdivider As Line
        Dim slot As PlinkoSlot
        Dim pos As Vector
        Dim Xdist As Double = canvas1.ActualWidth / (PinsPerRow + 1)
        pins = New List(Of PlinkoPin)
        balls = New List(Of PlinkoBall)
        slots = New List(Of PlinkoSlot)
        'Make the pins
        For row As Integer = 1 To 10
            pos = New Vector()
            pos.Y = row * (canvas1.ActualHeight - 180) / 10 + 50
            If row Mod 2 = 0 Then
                pos.X = 0
                For col As Integer = 1 To PinsPerRow
                    pos.X += Xdist
                    pin = New PlinkoPin(pinDiameter, pos)
                    pins.Add(pin)
                    pin.Draw(canvas1)
                Next
            Else
                pos.X = Xdist / 2
                For col As Integer = 1 To (PinsPerRow - 1)
                    pos.X += Xdist
                    pin = New PlinkoPin(pinDiameter, pos)
                    pins.Add(pin)
                    pin.Draw(canvas1)
                Next
            End If
        Next
        'Make the slots at the bottom
        For I As Integer = 0 To PinsPerRow
            slot = New PlinkoSlot(I * Xdist, (I + 1) * Xdist)
            slots.Add(slot)
            slot.Draw(canvas1)
            If I < PinsPerRow Then
                slotdivider = New Line() With
                {
                .Stroke = Brushes.Black,
                .StrokeThickness = 8.0,
                .X1 = slot.Right,
                .Y1 = canvas1.ActualHeight,
                .X2 = slot.Right,
                .Y2 = pos.Y + 20
                }
                canvas1.Children.Add(slotdivider)
            End If
        Next
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        NewBall()
        Render()
    End Sub

    Private Sub NewBall()
        Dim ball As PlinkoBall = New PlinkoBall(ballDiameter, New Vector(canvas1.ActualWidth / 2 + (pinDiameter + ballDiameter - 1) * (rnd.NextDouble() - 0.5), 20.0))
        balls.Add(ball)
        ball.Draw(canvas1)
    End Sub

    Private Sub Render()
        Do
            For I As Integer = balls.Count - 1 To 0 Step -1
                balls(I).Update(gravity, elasticity, pins)
                If balls(I).Position.Y >= canvas1.ActualHeight - 100 Then
                    balls(I).Lock()
                End If
                If balls(I).Position.Y >= canvas1.ActualHeight - ballDiameter / 2 Then
                    For J As Integer = 0 To slots.Count - 1
                        If balls(I).Position.X > slots(J).Left And balls(I).Position.X < slots(J).Right Then
                            slots(J).Count += 1
                        End If
                    Next
                    balls(I).Remove(canvas1)
                    balls.RemoveAt(I)
                End If
            Next
            Dim maxCount As Integer = 0
            For I As Integer = 0 To slots.Count - 1
                If slots(I).Count > maxCount Then maxCount = slots(I).Count
            Next
            For I As Integer = 0 To slots.Count - 1
                If slots(I).Count > 0 Then slots(I).SetHeight(100, maxCount)
            Next
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
            frameCounter += 1
            If frameCounter = 150 Then
                frameCounter = 0
                NewBall()
            End If
        Loop
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
