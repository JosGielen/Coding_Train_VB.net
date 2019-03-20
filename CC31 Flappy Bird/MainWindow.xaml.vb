Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 10
    Private App_Started As Boolean = False
    Private g As Gate
    Private Gates As List(Of Gate)
    Private Gatenumber As Integer = 4
    Private GateWidth As Double = 40.0
    Private Gatespace As Double = 100.0
    Private GateSpeed As Double = 0.8
    Private upSpeed As Double = 2.0
    Private downSpeed As Double = 0.02
    Private b As Bird
    Private BirdSize As Double = 25.0
    Private BirdX As Double
    Private Framecounter As Integer = 0
    Private IncreaseTime As Integer = 1000 'Number of frames between each speed increase
    Private SpeedIncrease As Double = 5    'Percentage of speed increase
    Private Score As Integer = 0


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Init()
    End Sub

    Private Sub Init()
        Gates = New List(Of Gate)
        Canvas1.Children.Clear()
        BirdX = Canvas1.ActualWidth / 10
        GateSpeed = 0.8
        upSpeed = 1
        downSpeed = 0.02
        'Make the border and background
        Dim r As Rectangle = New Rectangle() With {
            .Width = Canvas1.ActualWidth - 15,
            .Height = Canvas1.ActualHeight - 30,
            .Fill = Brushes.Black}
        r.SetValue(Canvas.TopProperty, 15.0)
        r.SetValue(Canvas.LeftProperty, 0.0)
        Canvas1.Children.Add(r)
        'Make the gates to the right of the screen
        For I As Integer = 0 To Gatenumber - 1
            g = New Gate((Canvas1.ActualWidth + GateWidth) * (0.5 + I / Gatenumber), GateWidth, Gatespace, Canvas1)
            g.Draw()
            Gates.Add(g)
        Next
        'Make the bird
        b = New Bird(BirdX, Canvas1.ActualHeight / 2, BirdSize, Canvas1)
        b.Draw()
        Score = 0
    End Sub


    Private Sub Start()
        Dim Index As Integer = 0
        Init()
        App_Started = True
        Do While App_Started
            Render()
            'Check bird collision when a gate reaches the bird(s) or when the bird(s) is/are inside the gate)
            Index = -1
            For I As Integer = 0 To Gatenumber - 1
                If Gates(I).X <= BirdX + BirdSize / 2 And Gates(I).X + GateWidth > BirdX - BirdSize / 2 Then
                    Index = I
                    Exit For
                End If
            Next
            If Index >= 0 Then b.CheckCollision(Gates(Index))
            If Not b.Alive Then
                App_Started = False
                MessageBox.Show(Me, "   Game Over!   " & vbCrLf & "Your Score = " & Score.ToString(), "Flappy Bird", MessageBoxButton.OK)
            End If
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
        Loop
    End Sub

    Private Sub Halt()
        App_Started = False
    End Sub

    Private Sub Render()
        For I As Integer = 0 To Gates.Count - 1
            Gates(I).Update(GateSpeed)
            If Gates(I).X < -GateWidth Then
                Gates(I).Reset()
            End If
        Next
        b.Update(downSpeed)
        Framecounter += 1
        If Framecounter = IncreaseTime Then
            Framecounter = 0
            GateSpeed *= (100 + SpeedIncrease) / 100
            downSpeed *= (100 + SpeedIncrease) / 100
            upSpeed *= (100 + SpeedIncrease) / 100
        End If
        Score += 1
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(WaitTime)
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        If Not App_Started Then
            Start()
        Else
            Halt()
        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        App_Started = False
        End
    End Sub

    Private Sub Window_KeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Space Then
            b.Flap(upSpeed)
        End If
    End Sub
End Class
