Class MainWindow
    Private Rendering As Boolean = False
    Private flock As List(Of Agent)
    Private population As Integer = 500
    Private speed As Double = 3.0
    Private viewdistance As Double = 40.0
    Private maxForce As Double = 0.1
    Private maxSpeed As Double = 1.5
    Private constantSpeed As Boolean = True
    Private WrapEdges As Boolean = True
    Private separationStrength As Double = 12.0
    Private alignmentStrength As Double = 0.5
    Private cohesionStrength As Double = 0.015
    Private cursorStrength As Double = 0.01
    Private rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim ag As Agent
        Dim x As Double = 0.0
        Dim y As Double = 0.0
        flock = New List(Of Agent)
        For I As Integer = 0 To population
            x = (0.8 * rnd.NextDouble() + 0.1) * canvas1.ActualWidth
            y = (0.8 * rnd.NextDouble() + 0.1) * canvas1.ActualHeight
            ag = New Agent(New Point(x, y)) With {
                .CanRotate = True,
                .MaxSpeed = maxSpeed,
                .MaxForce = maxForce,
                .MoveToTarget = False,
                .Velocity = New Vector(4 * rnd.NextDouble() - 2, 4 * rnd.NextDouble() - 2),
                .UseConstantSpeed = constantSpeed,
                .Size = New Size(10.0, 5.0)
            }
            ag.Draw(canvas1)
            flock.Add(ag)
        Next
        StartRender()
    End Sub

    Private Sub DetaultValues()
        speed = 3.0
        viewdistance = 50.0
        maxForce = 0.1
        maxSpeed = 1.5
        constantSpeed = True
        WrapEdges = True
        separationStrength = 10.0
        alignmentStrength = 0.5
        cohesionStrength = 0.01
        cursorStrength = 0.01
    End Sub

    Private Sub StartRender()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        Rendering = True
    End Sub

    Private Sub StopRender()
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        Rendering = False
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Dim mloc As Vector = CType(Mouse.GetPosition(canvas1), Vector)
        Dim x As Double = 0.0
        Dim y As Double = 0.0
        Dim dist As Double = 0.0
        Dim Steering As Vector
        Dim strength As Double = 0.0
        Dim total As Integer
        Dim mid As Vector = New Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2)
        'Calculate the Steerings on each agent
        For I As Integer = 0 To population
            x = flock(I).Location.X
            y = flock(I).Location.Y
            'Move away from the cursor
            dist = Distance(flock(I).Location, mloc)
            If dist < 3 * viewdistance Then
                flock(I).ApplyForce(cursorStrength * (flock(I).Location - mloc))
            End If
            'Apply flocking behaviour
            Steering = New Vector()
            total = 0
            For J As Integer = 0 To population
                If J <> I Then
                    dist = Distance(flock(J).Location, flock(I).Location)
                    If dist < viewdistance Then
                        'Separation
                        Steering = Steering + separationStrength * (flock(I).Location - flock(J).Location) / (dist * dist)
                        'Alignment
                        Steering = Steering + alignmentStrength * (flock(J).Velocity - flock(I).Velocity)
                        'Cohesion
                        Steering = Steering + cohesionStrength * (flock(J).Location - flock(I).Location)
                        total += 1
                    End If
                End If
            Next
            If total > 0 Then
                Steering = Steering / total
                flock(I).ApplyForce(Steering)
            End If
        Next
        'Update the agents
        For I As Integer = 0 To population
            If WrapEdges Then flock(I).Edges(canvas1)
            flock(I).Update()
        Next
    End Sub

    Private Function Distance(v1 As Vector, v2 As Vector) As Double
        Return (v2 - v1).Length
    End Function


End Class
