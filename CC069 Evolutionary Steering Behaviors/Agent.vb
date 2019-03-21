Public Class Agent
    Private Shared Rnd As Random = New Random
    Private my_Canvas As Canvas
    Private my_Poly As Polygon = New Polygon()
    Private my_FoodVisual As Ellipse
    Private my_PoisonVisual As Ellipse
    Private my_RT As RotateTransform = New RotateTransform()
    Private my_palette As ColorPalette
    Private my_colors As List(Of Brush)
    Private EdgeDistance As Double = 30
    Public Location As Vector
    Public Velocity As Vector
    Public Acceleration As Vector
    Public Force As Vector
    Public Mass As Double
    Public MaxSpeed As Double
    Public MaxForce As Double
    Public DNA(3) As Double
    Public Health As Double
    Public Age As Integer
    Public EatDistance As Double = 4.0

    Public Sub New(locX As Double, locY As Double, mass As Double, maxSpeed As Double, maxForce As Double)
        Location = New Vector(locX, locY)
        Velocity = New Vector(Rnd.NextDouble(), Rnd.NextDouble())
        Acceleration = New Vector()
        Me.Mass = mass
        Me.MaxSpeed = maxSpeed
        Me.MaxForce = maxForce
        Health = 100
        Age = 0
        my_RT.Angle = 0
        DNA(0) = Rnd.NextDouble() - 0.5 'Food attraction
        DNA(1) = Rnd.NextDouble() - 0.5 'Poison atrtaction
        DNA(2) = 130 * Rnd.NextDouble() + 20 'Food perception range
        DNA(3) = 130 * Rnd.NextDouble() + 20 'Poison perception range
        my_palette = New ColorPalette(Environment.CurrentDirectory & "\AgentLife.cpl")
        my_colors = my_palette.GetColorBrushes(200)
        my_Poly = New Polygon()
        my_Poly.Points.Add(New Point(0, 0))
        my_Poly.Points.Add(New Point(-20, -5))
        my_Poly.Points.Add(New Point(-20, 5))
        my_Poly.Stroke = Brushes.Black
        my_Poly.StrokeThickness = 1
        my_Poly.SetValue(Canvas.LeftProperty, Location.X)
        my_Poly.SetValue(Canvas.TopProperty, Location.Y)
        my_Poly.RenderTransform = my_RT
        my_FoodVisual = New Ellipse With
        {
            .Width = DNA(2),
            .Height = DNA(2),
            .Stroke = Brushes.Green,
            .StrokeThickness = 1
        }
        my_FoodVisual.SetValue(Canvas.LeftProperty, Location.X - DNA(2) / 2)
        my_FoodVisual.SetValue(Canvas.TopProperty, Location.Y - DNA(2) / 2)
        my_PoisonVisual = New Ellipse With
        {
            .Width = DNA(3),
            .Height = DNA(3),
            .Stroke = Brushes.Red,
            .StrokeThickness = 1
        }
        my_PoisonVisual.SetValue(Canvas.LeftProperty, Location.X - DNA(3) / 2)
        my_PoisonVisual.SetValue(Canvas.TopProperty, Location.Y - DNA(3) / 2)
    End Sub

    Public Sub New(original As Agent)
        Location = New Vector(original.Location.X, original.Location.Y)
        Velocity = New Vector(Rnd.NextDouble(), Rnd.NextDouble())
        Acceleration = New Vector(0, 0)
        Mass = original.Mass
        MaxSpeed = original.MaxSpeed
        MaxForce = original.MaxForce
        Health = 100
        Age = original.Age
        my_RT.Angle = 0
        DNA(0) = original.DNA(0)
        DNA(1) = original.DNA(1)
        DNA(2) = original.DNA(2)
        DNA(3) = original.DNA(3)
        my_palette = New ColorPalette(Environment.CurrentDirectory & "\AgentLife.cpl")
        my_colors = my_palette.GetColorBrushes(200)
        my_Poly = New Polygon()
        my_Poly.Points.Add(New Point(0, 0))
        my_Poly.Points.Add(New Point(-20, -5))
        my_Poly.Points.Add(New Point(-20, 5))
        my_Poly.Stroke = Brushes.Black
        my_Poly.StrokeThickness = 1
        my_Poly.SetValue(Canvas.LeftProperty, Location.X)
        my_Poly.SetValue(Canvas.TopProperty, Location.Y)
        my_Poly.RenderTransform = my_RT
        my_FoodVisual = New Ellipse With
        {
            .Width = DNA(2),
            .Height = DNA(2),
            .Stroke = Brushes.Green,
            .StrokeThickness = 1
        }
        my_FoodVisual.SetValue(Canvas.LeftProperty, Location.X - DNA(2) / 2)
        my_FoodVisual.SetValue(Canvas.TopProperty, Location.Y - DNA(2) / 2)
        my_PoisonVisual = New Ellipse With
        {
            .Width = DNA(3),
            .Height = DNA(3),
            .Stroke = Brushes.Red,
            .StrokeThickness = 1
        }
        my_PoisonVisual.SetValue(Canvas.LeftProperty, Location.X - DNA(3) / 2)
        my_PoisonVisual.SetValue(Canvas.TopProperty, Location.Y - DNA(3) / 2)
    End Sub

    Public Sub Draw(c As Canvas)
        my_Canvas = c
        my_Poly.SetValue(Canvas.LeftProperty, Location.X)
        my_Poly.SetValue(Canvas.TopProperty, Location.Y)
        my_FoodVisual.Width = DNA(2)
        my_FoodVisual.Height = DNA(2)
        my_FoodVisual.SetValue(Canvas.LeftProperty, Location.X - DNA(2) / 2)
        my_FoodVisual.SetValue(Canvas.TopProperty, Location.Y - DNA(2) / 2)
        my_PoisonVisual.Width = DNA(3)
        my_PoisonVisual.Height = DNA(3)
        my_PoisonVisual.SetValue(Canvas.LeftProperty, Location.X - DNA(3) / 2)
        my_PoisonVisual.SetValue(Canvas.TopProperty, Location.Y - DNA(3) / 2)
        c.Children.Add(my_Poly)
        c.Children.Add(my_FoodVisual)
        c.Children.Add(my_PoisonVisual)
    End Sub

    ''' <summary>
    ''' Get the direction of the force towards food and poison
    ''' </summary>
    Private Function GetSteeringForce(target As Vector) As Vector
        Dim DesiredVelocity As Vector
        Dim Steering As Vector
        DesiredVelocity = target - Location
        Steering = DesiredVelocity - Velocity
        Steering.Normalize()
        Return Steering
    End Function

    ''' <summary>
    ''' Get a force away from the edge
    ''' </summary>
    Private Function GetEdgeSteeringForce() As Vector
        If my_Canvas Is Nothing Then Return New Vector(0, 0)
        Dim center As Vector = New Vector(my_Canvas.ActualWidth / 2, my_Canvas.ActualHeight / 2)
        Dim DesiredVelocity As Vector = New Vector(0, 0)
        Dim EdgeSteeringForce As Vector = New Vector(0, 0)
        If Location.X < EdgeDistance Then
            DesiredVelocity = center - Location
        ElseIf Location.X > my_Canvas.ActualWidth - EdgeDistance Then
            DesiredVelocity = center - Location
        ElseIf Location.Y < EdgeDistance Then
            DesiredVelocity = center - Location
        ElseIf Location.Y > my_Canvas.ActualHeight - EdgeDistance Then
            DesiredVelocity = center - Location
        End If
        If DesiredVelocity.Length > 0 Then
            DesiredVelocity.Normalize()
            DesiredVelocity = MaxSpeed * DesiredVelocity
            EdgeSteeringForce = DesiredVelocity - Velocity
        End If
        Return EdgeSteeringForce
    End Function

    ''' <summary>
    ''' Get the sum of all forces on the Agent
    ''' </summary>
    Private Function GetForce(closestFood As Point, closestPoison As Point) As Vector
        Dim totalForce As Vector = New Vector(0, 0)
        totalForce += GetEdgeSteeringForce()
        If closestFood.X + closestFood.Y > -1 Then
            totalForce += DNA(0) * GetSteeringForce(closestFood)
        End If
        If closestPoison.X + closestPoison.Y > -1 Then
            totalForce += DNA(1) * GetSteeringForce(closestPoison)
        End If
        If totalForce.Length > MaxForce Then
            totalForce.Normalize()
            totalForce = MaxForce * totalForce
        End If
        Return totalForce
    End Function

    Public Sub Update(food As List(Of Point), poison As List(Of Point), foodvalue As Integer, poisonvalue As Integer)
        Dim mindist As Double
        Dim dist As Double
        Dim closestFood As Point
        Dim closestPoison As Point
        'Adjust time dependant values
        Health -= 0.2
        Age += 1
        'Find closest Food in visual range
        mindist = Double.MaxValue
        closestFood = New Point(-1, -1)
        For I As Integer = food.Count - 1 To 0 Step -1
            dist = Math.Sqrt((Location.X - food(I).X) ^ 2 + (Location.Y - food(I).Y) ^ 2)
            If dist < EatDistance Then
                'Eat the food
                food.Remove(food(I))
                Health += foodvalue
            ElseIf dist < DNA(2) / 2 Then
                If dist < mindist Then
                    mindist = dist
                    closestFood = food(I)
                End If
            End If
        Next
        'Find closest Poison in visual range
        mindist = Double.MaxValue
        closestPoison = New Point(-1, -1)
        For I As Integer = poison.Count - 1 To 0 Step -1
            dist = Math.Sqrt((Location.X - poison(I).X) ^ 2 + (Location.Y - poison(I).Y) ^ 2)
            If dist < EatDistance Then
                'Eat the poison
                poison.Remove(poison(I))
                Health -= poisonvalue
            ElseIf dist < DNA(3) / 2 Then
                If dist < mindist Then
                    mindist = dist
                    closestPoison = poison(I)
                End If
            End If
        Next
        'Apply the forces
        Force = GetForce(closestFood, closestPoison)
        Acceleration = Force / Mass
        Velocity += Acceleration
        If Velocity.Length > 0 Then 'Always move at maxspeed
            Velocity.Normalize()
            Velocity = MaxSpeed * Velocity
        End If
        Location += Velocity
        'Adjust the orientation of the Agent
        Dim angle As Double = 180 * Math.Atan2(Velocity.Y, Velocity.X) / Math.PI
        my_RT.Angle = angle
        'Adjust the color to show the Agent health
        Dim col As Integer = CInt(Health)
        If col > 199 Then col = 199
        If col < 0 Then col = 0
        my_Poly.Fill = my_colors(col)
    End Sub

    Public Sub Mutate(mutationrate As Double, StepSize As Double)
        Dim r As Double
        If 100 * Rnd.NextDouble < mutationrate Then 'Food attraction mutation
            r = Rnd.NextDouble()
            If r > 0.66 Then
                DNA(0) = DNA(0) + StepSize
            ElseIf r > 0.33 Then
                DNA(0) = DNA(0) - StepSize
            Else
                DNA(0) = Rnd.NextDouble() - 0.5
            End If
        End If
        If 100 * Rnd.NextDouble < mutationrate Then 'Poison attraction mutation
            r = Rnd.NextDouble()
            If r > 0.66 Then
                DNA(1) = DNA(1) + StepSize
            ElseIf r > 0.33 Then
                DNA(1) = DNA(1) - StepSize
            Else
                DNA(1) = Rnd.NextDouble() - 0.5
            End If
        End If
        If 100 * Rnd.NextDouble < mutationrate Then 'Food Perception Range mutation
            r = Rnd.NextDouble()
            If r > 0.66 Then
                If DNA(2) <= 140 Then DNA(2) = DNA(2) + 10
            ElseIf r > 0.33 Then
                If DNA(2) >= 30 Then DNA(2) = DNA(2) - 10
            Else
                DNA(2) = 130 * Rnd.NextDouble() + 20
            End If
        End If
        If 100 * Rnd.NextDouble < mutationrate Then 'Poison Perception Range mutation
            r = Rnd.NextDouble()
            If r > 0.66 Then
                If DNA(3) <= 140 Then DNA(3) = DNA(3) + 10
            ElseIf r > 0.33 Then
                If DNA(3) >= 30 Then DNA(3) = DNA(3) - 10
            Else
                DNA(3) = 130 * Rnd.NextDouble() + 20
            End If
        End If
    End Sub

    Public Function Status() As String
        Dim result As String = ""
        result &= "  Age = " & Age.ToString() & vbTab
        result &= "  FoodAtt = " & DNA(0).ToString("F2") & vbTab
        result &= "  PoisonAtt = " & DNA(1).ToString("F2") & vbTab
        result &= "  FoodRange = " & DNA(2).ToString("F2") & vbTab
        result &= "  PoisonRange = " & DNA(3).ToString("F2")
        Return result
    End Function

End Class

