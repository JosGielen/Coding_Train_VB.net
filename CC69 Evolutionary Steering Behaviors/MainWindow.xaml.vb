Class MainWindow
    Private Rnd As Random = New Random
    Private my_Log As Log = New Log(True, False)
    Private logging As Boolean
    Private food As List(Of Point)
    Private foodCount As Integer = 200
    Private foodValue As Integer = 10
    Private poison As List(Of Point)
    Private poisonCount As Integer = 60
    Private poisonValue As Integer = 50
    Private agents As List(Of Agent)
    Private PopulationNr As Integer = 0
    Private agentCount As Integer = 15
    Private Maxspeed As Double = 1.5
    Private MaxForce As Double = 3.0
    Private MutationRate As Double = 30.0
    Private stepSize As Double = 0.1
    Private maxAge As Integer = 0
    Private eldestIndex As Integer = -1
    Private best As Agent
    Private Rendering As Boolean = False

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        logging = True
        If logging Then my_Log.AddItem("Start settings:")
        If logging Then my_Log.AddItem("foodCount = " & foodCount.ToString())
        If logging Then my_Log.AddItem("poisonCount = " & poisonCount.ToString())
        If logging Then my_Log.AddItem("AgentCount = " & agentCount.ToString())

        food = New List(Of Point)
        For I As Integer = 0 To foodCount - 1
            food.Add(New Point(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)))
        Next
        poison = New List(Of Point)
        For I As Integer = 0 To poisonCount - 1
            poison.Add(New Point(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)))
        Next
        agents = New List(Of Agent)
        PopulationNr = 1
        If logging Then my_Log.AddItem("Population Nr: " & PopulationNr.ToString())
        For I As Integer = 0 To agentCount - 1
            agents.Add(New Agent(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05), 1.0, Maxspeed, MaxForce))
            If logging Then my_Log.AddItem("Agent " & I.ToString & " = " & agents(I).Status)
        Next
        best = agents(0)
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        Rendering = True
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        If Not Rendering Then Exit Sub
        Dim alive As Integer = 0
        Dim newAgent As Agent
        'Add some food
        If Rnd.NextDouble() < 0.09 Then
            food.Add(New Point(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)))
        End If
        'Add some poison
        If poison.Count < 75 Then
            If Rnd.NextDouble() < 0.01 Then
                poison.Add(New Point(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)))
            End If
        End If
        'Update the agents
        Dim actualhealth As Double = 0.0
        For I As Integer = agents.Count - 1 To 0 Step -1
            actualhealth = agents(I).Health
            If actualhealth > 0 Then
                alive += 1
                agents(I).Update(food, poison, foodValue, poisonValue)
                If agents(I).Health <= 0 Then
                    If logging Then my_Log.AddItem("Agent " & I.ToString() & "  died at age " & agents(I).Age.ToString())
                End If
            End If
        Next
        'Allow procreation
        For I As Integer = 0 To agents.Count - 1
            If agents(I).Health >= 200 Then
                newAgent = New Agent(agents(I))
                newAgent.Mutate(MutationRate, stepSize)
                agents(I).Health = 100
                agents.Add(newAgent)
                If logging Then my_Log.AddItem("Agent " & I.ToString() & "  spawned a new agent:")
                If logging Then my_Log.AddItem("Parent = " & I.ToString() & agents(I).Status)
                If logging Then my_Log.AddItem("Child = " & (agents.Count - 1).ToString() & newAgent.Status)
            End If
        Next
        'Restart if all Agents die
        If alive = 0 Then
            If logging Then my_Log.AddItem("EXTICTION.")
            If logging Then my_Log.AddItem("Best Agent was " & best.Status)
            PopulationNr += 1
            If logging Then my_Log.AddItem("Making New Population (Nr: " & PopulationNr.ToString() & ")")
            food.Clear()
            For I As Integer = 0 To foodCount - 1
                food.Add(New Point(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)))
            Next
            poison.Clear()
            For I As Integer = 0 To poisonCount - 1
                poison.Add(New Point(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)))
            Next
            maxAge = 0
            eldestIndex = 0
            agents.Clear()
            For I As Integer = 0 To agentCount - 1
                If 100 * Rnd.NextDouble() > 100 - MutationRate Then
                    newAgent = New Agent(best)
                    newAgent.Mutate(MutationRate, stepSize)
                    If logging Then my_Log.AddItem("Agent " & I.ToString & " = Mutant of Best : " & newAgent.Status)
                Else
                    newAgent = New Agent(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05), 1.0, Maxspeed, MaxForce)
                    If logging Then my_Log.AddItem("Agent " & I.ToString & " = Random : " & newAgent.Status)
                End If
                agents.Add(newAgent)
            Next
        End If
        'Show the eldest Agent status info
        For I As Integer = 0 To agents.Count - 1
            If agents(I).Health > 0 And agents(I).Age > maxAge Then
                maxAge = agents(I).Age
                eldestIndex = I
                best = agents(I)
            End If
        Next
        If eldestIndex >= 0 Then
            TxtEldest.Text = "Age = " & best.Age.ToString() &
                                "  ( Food = " & best.DNA(0).ToString("F2") &
                                " ; Range = " & best.DNA(2).ToString("F2") &
                                " )  ( Poison = " & best.DNA(1).ToString("F2") &
                                " ; Range = " & best.DNA(3).ToString("F2") & " )"
        End If
        TxtFoodCount.Text = food.Count.ToString()
        TxtPoisonCount.Text = poison.Count.ToString()
        Draw()
    End Sub

    Private Sub Draw()
        canvas1.Children.Clear()
        Dim El As Ellipse
        For I As Integer = 0 To food.Count - 1
            El = New Ellipse() With
            {
            .Width = 4,
            .Height = 4,
            .Stroke = Brushes.Green,
            .StrokeThickness = 1,
            .Fill = Brushes.Green
            }
            El.SetValue(Canvas.LeftProperty, food(I).X)
            El.SetValue(Canvas.TopProperty, food(I).Y)
            canvas1.Children.Add(El)
        Next
        For I As Integer = 0 To poison.Count - 1
            El = New Ellipse() With
            {
            .Width = 4,
            .Height = 4,
            .Stroke = Brushes.Red,
            .StrokeThickness = 1,
            .Fill = Brushes.Red
            }
            El.SetValue(Canvas.LeftProperty, poison(I).X)
            El.SetValue(Canvas.TopProperty, poison(I).Y)
            canvas1.Children.Add(El)
        Next
        For I As Integer = 0 To agents.Count - 1
            If agents(I).Health > 0 Then agents(I).Draw(canvas1)
        Next
    End Sub

    Private Sub Window_KeyUp(sender As Object, e As KeyEventArgs)


    End Sub
End Class
