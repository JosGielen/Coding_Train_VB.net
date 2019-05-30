Public Class Particle
    Private my_Pos As Vector
    Private my_Vel As Vector
    Private my_Acc As Vector
    Private my_MaxSpeed As Double

    Public Sub New(position As Vector, velocity As Vector, maxSpeed As Double)
        my_Pos = position
        my_Vel = velocity
        my_MaxSpeed = maxSpeed
        my_Acc = New Vector()
    End Sub

    Public Property Position As Vector
        Get
            Return my_Pos
        End Get
        Set(value As Vector)
            my_Pos = value
        End Set
    End Property

    Public Property Velocity As Vector
        Get
            Return my_Vel
        End Get
        Set(value As Vector)
            my_Vel = value
        End Set
    End Property

    Public Sub Update()
        my_Vel = my_Vel + my_Acc
        my_Vel.Normalize()
        my_Vel = my_MaxSpeed * my_Vel
        my_Pos = my_Pos + my_Vel
        my_Acc = New Vector()
    End Sub

    Public Sub ApplyForce(force As Vector)
        my_Acc = my_Acc + force
    End Sub

End Class
