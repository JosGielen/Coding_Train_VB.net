Public Class Agent
    Private my_Location As Vector
    Private my_Velocity As Vector
    Private my_DesiredVelocity As Vector
    Private my_Force As Vector
    Private my_Acceleration As Vector
    Private my_Mass As Double
    Private my_MaxSpeed As Double
    Private my_ConstantSpeed As Boolean
    Private my_MaxForce As Double
    Private my_MoveToTarget As Boolean    'If True: Automatic moves to a given target and stops there
    Private my_Breakingdistance As Double 'Reduce speed when distance to target < my_Breakingdistance
    Private my_Accuracy As Double         'Stop moving when distance to target < my_Accuracy
    Private my_Target As Vector
    Private my_Shape As Shape             'Custom shape for the agent
    Private my_Size As Size               'Adjust the size of the agent (also for custom shapes)
    Private my_FillColor As Brush
    Private my_LineColor As Brush
    Private my_LineThickness As Double
    Private my_Rotate As Boolean
    Private my_RT As RotateTransform

    Public Sub New(location As Point)
        my_Location = New Vector(location.X, location.Y)
        my_Velocity = New Vector()
        my_DesiredVelocity = New Vector()
        my_Acceleration = New Vector()
        my_Mass = 1.0
        my_MaxSpeed = Double.MaxValue
        my_MaxForce = Double.MaxValue
        my_ConstantSpeed = False
        my_Breakingdistance = Double.MaxValue
        my_Accuracy = 0.2
        my_Size = New Size(0.0, 0.0)
        my_FillColor = Brushes.White
        my_LineColor = Brushes.Black
        my_LineThickness = 1.0
        my_Rotate = False
        my_MoveToTarget = False
        my_Target = New Vector(location.X, location.Y)
        'Default shape is a triangle
        my_Shape = New Polygon()
        CType(my_Shape, Polygon).Points.Add(New Point(0, 0))
        CType(my_Shape, Polygon).Points.Add(New Point(-20, -5))
        CType(my_Shape, Polygon).Points.Add(New Point(-20, 5))
        my_Shape.Fill = my_FillColor
        my_Shape.Stroke = my_LineColor
        my_Shape.StrokeThickness = my_LineThickness
        my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Size.Width / 2)
        my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Size.Height / 2)
    End Sub

    Public Sub New(location As Point, shape As Shape)
        my_Location = New Vector(location.X, location.Y)
        my_Velocity = New Vector()
        my_DesiredVelocity = New Vector()
        my_Acceleration = New Vector()
        my_Mass = 1.0
        my_MaxSpeed = Double.MaxValue
        my_MaxForce = Double.MaxValue
        my_Breakingdistance = Double.MaxValue
        my_Accuracy = 0.2
        my_Size = New Size(0.0, 0.0)
        my_FillColor = Brushes.White
        my_LineColor = Brushes.Black
        my_LineThickness = 1.0
        my_Rotate = False
        my_MoveToTarget = False
        my_Target = New Vector(location.X, location.Y)
        my_Shape = shape
        my_Shape.Fill = my_FillColor
        my_Shape.Stroke = my_LineColor
        my_Shape.StrokeThickness = my_LineThickness
        my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Size.Width / 2)
        my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Size.Height / 2)
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

    Public Property Size As Size
        Get
            Return my_Size
        End Get
        Set(value As Size)
            my_Size = value
            my_Shape.Stretch = Stretch.Uniform
            my_Shape.Width = my_Size.Width
            my_Shape.Height = my_Size.Height
        End Set
    End Property

    Public Property MoveToTarget As Boolean
        Get
            Return my_MoveToTarget
        End Get
        Set(value As Boolean)
            my_MoveToTarget = value
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

    Public Property Fill As Brush
        Get
            Return my_FillColor
        End Get
        Set(value As Brush)
            my_FillColor = value
            my_Shape.Fill = value
        End Set
    End Property

    Public Property Stroke As Brush
        Get
            Return my_LineColor
        End Get
        Set(value As Brush)
            my_LineColor = value
            my_Shape.Stroke = value
        End Set
    End Property

    Public Property StrokeThickness As Double
        Get
            Return my_LineThickness
        End Get
        Set(value As Double)
            my_LineThickness = value
            my_Shape.StrokeThickness = value
        End Set
    End Property

    Public Property MaxSpeed As Double
        Get
            Return my_MaxSpeed
        End Get
        Set(value As Double)
            my_MaxSpeed = value
        End Set
    End Property

    Public Property MaxForce As Double
        Get
            Return my_MaxForce
        End Get
        Set(value As Double)
            my_MaxForce = value
        End Set
    End Property

    Public Property CanRotate As Boolean
        Get
            Return my_Rotate
        End Get
        Set(value As Boolean)
            my_Rotate = value
            If value = True Then
                my_RT = New RotateTransform()
                my_RT.Angle = 0
                my_Shape.RenderTransform = my_RT
            Else
                my_RT = Nothing
                my_Shape.RenderTransform = Nothing
            End If
        End Set
    End Property

    Public Property Accuracy As Double
        Get
            Return my_Accuracy
        End Get
        Set(value As Double)
            my_Accuracy = value
        End Set
    End Property

    Public Property Mass As Double
        Get
            Return my_Mass
        End Get
        Set(value As Double)
            my_Mass = value
        End Set
    End Property

    Public Property UseConstantSpeed As Boolean
        Get
            Return my_ConstantSpeed
        End Get
        Set(value As Boolean)
            my_ConstantSpeed = value
        End Set
    End Property

    Public Sub SetTarget(target As Point)
        my_Target = New Vector(target.X, target.Y)
    End Sub

    Public Sub Draw(c As Canvas)
        c.Children.Add(my_Shape)
    End Sub

    Public Sub ApplySteeringForce(target As Vector)
        Dim SteeringForce As Vector
        Dim maxspeed As Double
        Dim dist As Double
        my_Target = target
        my_DesiredVelocity = target - my_Location
        dist = my_DesiredVelocity.Length
        If dist > my_Breakingdistance Then
            maxspeed = my_MaxSpeed
        ElseIf dist < my_Accuracy Then
            maxspeed = 0
        Else
            If my_Breakingdistance > 0 Then
                maxspeed = (my_MaxSpeed * dist / my_Breakingdistance)
            Else
                maxspeed = my_MaxSpeed
            End If
        End If
        my_DesiredVelocity.Normalize()
        my_DesiredVelocity = maxspeed * my_DesiredVelocity
        SteeringForce = my_DesiredVelocity - my_Velocity
        ApplyForce(SteeringForce)
    End Sub

    Public Sub ApplyForce(force As Vector)
        my_Force = my_Force + force
    End Sub

    Public Sub Update()
        If my_MoveToTarget = True Then
            ApplySteeringForce(my_Target)
        Else
            my_Accuracy = 0.0
        End If
        If my_Force.Length > my_MaxForce Then
            my_Force.Normalize()
            my_Force = my_MaxForce * my_Force
        End If
        my_Acceleration = my_Force / my_Mass
        my_Velocity += my_Acceleration
        If my_Rotate And my_Velocity.Length > my_Accuracy Then
            If my_Shape.ActualWidth > 0 And my_Shape.ActualHeight > 0 Then
                my_RT.CenterX = my_Shape.Width / 2
                my_RT.CenterY = my_Shape.Height / 2
            End If
            my_RT.Angle = 180 * Math.Atan2(my_Velocity.Y, my_Velocity.X) / Math.PI
        End If
        If my_ConstantSpeed = True Then
            my_Velocity.Normalize()
            my_Velocity = my_MaxSpeed * my_Velocity
        End If
        my_Location += my_Velocity
        my_Shape.SetValue(Canvas.LeftProperty, my_Location.X - my_Size.Width / 2)
        my_Shape.SetValue(Canvas.TopProperty, my_Location.Y - my_Size.Height / 2)
        my_Force = 0 * my_Force
    End Sub

    Public Sub Edges(c As Canvas)
        If my_Location.X < 0 Then
            my_Location.X = c.ActualWidth
        ElseIf my_Location.X > c.ActualWidth Then
            my_Location.X = 0
        End If
        If my_Location.Y < 0 Then
            my_Location.Y = c.ActualHeight
        ElseIf my_Location.Y > c.ActualHeight Then
            my_Location.Y = 0
        End If
    End Sub

End Class
