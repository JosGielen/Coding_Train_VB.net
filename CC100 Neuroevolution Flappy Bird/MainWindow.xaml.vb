Imports System.Windows.Threading
Imports System.Threading
Imports Microsoft.Win32

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 10
    Private App_Started As Boolean = False
    Private g As Gate
    Private Gates As List(Of Gate)
    Private Gatenumber As Integer = 4
    Private GateWidth As Double = 40.0
    Private Gatespace As Double = 150.0
    Private GateSpeed As Double = 0.8
    Private upSpeed As Double = 2.0
    Private downSpeed As Double = 0.02
    Private Birdsnumber As Integer = 300  'Number of birds in each generation
    Private Birds(Birdsnumber) As Bird
    Private BirdSize As Double = 25.0
    Private BirdMutationRate As Double = 5 'Percent chance that a weight in the Bird NeuralNet changes
    Private BirdMutationFactor As Double = 5 'Percentage that a weight in the Bird NeuralNet can change (+ or -)
    Private BirdX As Double
    Private BestBird As Bird
    Private IncreaseTime As Integer = 1000 'Number of frames between each speed increase
    Private SpeedIncrease As Double = 5    'Percentage of speed increase
    Private Rnd As Random = New Random

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Init()
    End Sub

    Private Sub Init()
        Dim Y As Double = 0.0
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
        'Make the gates on the right side of the screen
        For I As Integer = 0 To Gatenumber - 1
            g = New Gate((Canvas1.ActualWidth + GateWidth) * (0.5 + I / Gatenumber), GateWidth, Gatespace, Canvas1)
            g.Draw()
            Gates.Add(g)
        Next
    End Sub

    Private Sub MakeInitialGeneration()
        'Make the initial Generation
        For I As Integer = 0 To Birdsnumber
            Birds(I) = New Bird(BirdX, Canvas1.ActualHeight / 2, BirdSize, Canvas1)
            Birds(I).UpSpeed = upSpeed
            Birds(I).SetBrain(New NeuralNet(5, 8, 2, 0.3, False))
            Birds(I).Draw()
        Next
    End Sub

    Private Sub Start()
        Dim Index As Integer = 0
        Dim Mindistance As Double = 0.0
        Dim LiveBirdCounter As Integer = 0
        App_Started = True
        Do While App_Started
            Render()
            'Check bird collision when a gate reaches the bird(s) or when the birds are inside the gate)
            Index = -1
            For I As Integer = 0 To Gatenumber - 1
                If Gates(I).X <= BirdX + BirdSize / 2 And Gates(I).X + GateWidth > BirdX - BirdSize / 2 Then
                    Index = I
                    Exit For
                End If
            Next
            If Index >= 0 Then 'A gate has reached the Birds
                For I As Integer = 0 To Birdsnumber
                    Birds(I).CheckCollision(Gates(Index))
                Next
            End If
            'Get the closest Gate in front of the Birds
            Index = -1
            Mindistance = Double.MaxValue
            For J As Integer = 0 To Gatenumber - 1
                If Gates(J).X + GateWidth > BirdX + BirdSize / 2 Then 'Gate is in front of the birds or at their location 
                    If Math.Abs(Gates(J).X - BirdX) < Mindistance Then
                        Mindistance = Math.Abs(Gates(J).X - BirdX)
                        Index = J
                    End If
                End If
            Next
            'All Live Birds can think about their next move
            LiveBirdCounter = 0
            For I As Integer = 0 To Birdsnumber
                If Birds(I).Alive Then
                    Birds(I).Think(Gates(Index))
                    LiveBirdCounter += 1
                End If
            Next
            Me.Title = LiveBirdCounter.ToString() & " Birds still alive."
            If LiveBirdCounter = 0 Then 'All birds died: Make the next Generation
                Init()
                'Get the fitness of each bird as the normalized score
                Dim sumScore As Double = 0.0
                Dim OldBirds(Birdsnumber) As Bird
                For I As Integer = 0 To Birdsnumber
                    sumScore += (Birds(I).Score)
                Next
                For I As Integer = 0 To Birdsnumber
                    Birds(I).Fitness = (Birds(I).Score) / sumScore
                Next
                'Get the best bird
                Dim maxScore As Double = 0.0
                For I As Integer = 0 To Birdsnumber
                    If Birds(I).Score > maxScore Then
                        BestBird = Birds(I).copy()
                    End If
                Next
                'Copy all the birds into a selection pool (= Oldbirds)
                For I As Integer = 0 To Birdsnumber
                    OldBirds(I) = Birds(I).copy
                    OldBirds(I).Fitness = Birds(I).Fitness
                Next
                ''Pick random Oldbirds to form the next Generation  (Generic Algorithm where the pickchance = fitness)
                Dim r As Double = 0.0
                For I As Integer = 0 To Birdsnumber
                    r = Rnd.NextDouble()
                    Index = 0
                    Do While r > 0
                        r = r - OldBirds(Index).Fitness
                        Index += 1
                    Loop
                    Index -= 1
                    Birds(I) = OldBirds(Index).copy()
                    If Rnd.Next(100) > BirdMutationRate Then Birds(I).Mutate(BirdMutationRate, BirdMutationFactor)
                    Birds(I).Draw()
                Next
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
        For I As Integer = 0 To Birdsnumber
            If Birds(I).Alive Then
                Birds(I).Update(downSpeed)
                Birds(I).Score += 1
            Else
                Birds(I).remove()
            End If
        Next
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(WaitTime)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        App_Started = False
        End
    End Sub

    Private Sub MnuLoad_Click(sender As Object, e As RoutedEventArgs)
        Dim OFD As New OpenFileDialog()
        'Show an OpenFile dialog
        OFD.InitialDirectory = Environment.CurrentDirectory
        OFD.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        OFD.FilterIndex = 1
        OFD.RestoreDirectory = True
        If OFD.ShowDialog() = True Then
            BestBird = New Bird(BirdX, Canvas1.ActualHeight / 2, BirdSize, Canvas1)
            BestBird.UpSpeed = upSpeed
            BestBird.LoadNN(OFD.FileName)
        End If
        For I As Integer = 0 To Birdsnumber
            Birds(I) = BestBird.copy()
            If Rnd.Next(100) > BirdMutationRate Then Birds(I).Mutate(BirdMutationRate, BirdMutationFactor)
            Birds(I).Draw()
        Next
        Start()
    End Sub

    Private Sub MnuSave_Click(sender As Object, e As RoutedEventArgs)
        Dim SFD As New SaveFileDialog()
        SFD.InitialDirectory = Environment.CurrentDirectory
        SFD.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        SFD.FilterIndex = 1
        SFD.RestoreDirectory = True
        If SFD.ShowDialog() Then
            BestBird.SaveNN(SFD.FileName)
        End If
    End Sub

    Private Sub MnuExit_Click(sender As Object, e As RoutedEventArgs)
        App_Started = False
        End
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not App_Started Then
            BtnStart.Content = "STOP"
            Init()
            MakeInitialGeneration()
            Start()
        Else
            BtnStart.Content = "START"
            Halt()
        End If
    End Sub

    Private Sub MnuSaveCurrent_Click(sender As Object, e As RoutedEventArgs)
        Dim SFD As New SaveFileDialog()
        SFD.InitialDirectory = Environment.CurrentDirectory
        SFD.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        SFD.FilterIndex = 1
        SFD.RestoreDirectory = True
        If SFD.ShowDialog() Then
            For I As Integer = 0 To Birdsnumber
                If Birds(I).Alive Then
                    Birds(I).SaveNN(SFD.FileName)
                    Exit For
                End If
            Next
        End If
    End Sub
End Class

