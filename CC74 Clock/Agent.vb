Public Class Agent
    Private my_Location As Vector
    Private my_Velocity As Vector
    Private my_Acceleration As Vector
    Private my_Mass As Double
    Private my_MaxSpeed As Double
    Private my_MaxForce As Double
    Private my_Target As Vector
    Private my_Size As Double
    Private my_Color As Brush
    Private my_Breakingdistance As Double
    Private my_Force As Vector
    Private my_Shape As Ellipse

    Public Sub New(location As Point, mass As Double, maxSpeed As Double, maxForce As Double)
        my_Location = New Vector(location.X, location.Y)
        my_Velocity = New Vector()
        my_Acceleration = New Vector()
        my_Mass = mass
        my_MaxSpeed = maxSpeed
        my_MaxForce = maxForce
        my_Target = my_Location
        my_Size = 2.0
        my_Color = Brushes.Black
        my_Shape = New Ellipse() With {
            .Stroke = my_Color,
            .StrokeThickness = 1.0,
            .Fill = my_Color,
            .Width = my_Size,
            .Height = my_Size
        }
        my_Shape.SetValue(Canvas.LeftProperty, Me.Location.X)
        my_Shape.SetValue(Canvas.TopProperty, Me.Location.Y)
    End Sub

    Public Property Location As Vector
        Get
            Return my_Location
        End Get
        Set(value As Vector)
            my_Location = value
        End Set
    End Property

    Public Property Velocity As Vector
        Get
            Return my_Velocity
        End Get
        Set(value As Vector)
            my_Velocity = value
        End Set
    End Property

    Public Property Breakingdistance As Double
        Get
            Return my_Breakingdistance
        End Get
        Set(value As Double)
            my_Breakingdistance = value
        End Set
    End Property

    Public Property Size As Double
        Get
            Return my_Size
        End Get
        Set(value As Double)
            my_Size = value
            my_Shape.Width = my_Size
            my_Shape.Height = my_Size
        End Set
    End Property

    Public Property Target As Vector
        Get
            Return my_Target
        End Get
        Set(value As Vector)
            my_Target = value
        End Set
    End Property

    Public Property Color As Brush
        Get
            Return my_Color
        End Get
        Set(value As Brush)
            my_Color = value
            my_Shape.Fill = value
            my_Shape.Stroke = value
        End Set
    End Property

    Public Sub SetTarget(target As Point)
        my_Target = New Vector(target.X, target.Y)
    End Sub

    Public Sub Draw(c As Canvas)
        c.Children.Add(my_Shape)
    End Sub

    Public Sub ApplyForce(force As Vector)
        my_Force = my_Force + force
        If my_Force.Length > my_MaxForce Then
            my_Force.Normalize()
            my_Force = my_MaxForce * my_Force
        End If
    End Sub

    Private Function GetSteeringForce(target As Vector) As Vector
        Dim DesiredVelocity As Vector
        Dim Steering As Vector
        Dim maxspeed As Double
        Dim dist As Double
        DesiredVelocity = target - my_Location
        dist = DesiredVelocity.Length
        If dist > my_Breakingdistance Then
            maxspeed = my_MaxSpeed
        ElseIf dist < 0.2 Then
            maxspeed = 0
        Else
            maxspeed = (my_MaxSpeed * dist / my_Breakingdistance)
        End If
        DesiredVelocity.Normalize()
        DesiredVelocity = maxspeed * DesiredVelocity
        Steering = DesiredVelocity - my_Velocity
        Return Steering
    End Function

    Public Sub Update()
        ApplyForce(GetSteeringForce(my_Target))
        my_Acceleration = my_Force / my_Mass
        my_Velocity += my_Acceleration
        my_Location += my_Velocity
        my_Force = 0 * my_Force
        my_Shape.SetValue(Canvas.LeftProperty, Location.X)
        my_Shape.SetValue(Canvas.TopProperty, Location.Y)
    End Sub

End Class
