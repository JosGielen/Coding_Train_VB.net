Imports System.Windows.Media.Media3D
Imports GlmNet

Public Class GLCamera
    Private my_Position As Vector3D
    Private my_Direction As Vector3D
    Private my_Up As Vector3D
    Private my_Yaw As Double
    Private my_Pitch As Double
    Private my_MoveSpeed As Double

    Public Sub New()
        my_Position = New Vector3D(0, 0, 1)
        my_Direction = New Vector3D(0, 0, -1)
        my_Up = New Vector3D(0, 1, 0)
        my_Yaw = 0.0
        my_Pitch = 0.0
        my_MoveSpeed = 1.0
    End Sub

    Public Sub New(position As Vector3D, direction As Vector3D, up As Vector3D)
        my_Position = position
        my_Direction = direction
        my_Up = up
        my_Yaw = Math.Atan2(direction.X, direction.Z) - Math.Sign(direction.X) * Math.PI
        my_Pitch = Math.Atan2(direction.Y, Math.Sqrt(direction.X ^ 2 + direction.Z ^ 2))
        my_MoveSpeed = 1.0
    End Sub

    Public Property Position As Vector3D
        Get
            Return my_Position
        End Get
        Set(value As Vector3D)
            my_Position = value
        End Set
    End Property

    Public Property Direction As Vector3D
        Get
            Return my_Direction
        End Get
        Set(value As Vector3D)
            my_Direction = value
            my_Yaw = Math.Atan2(Direction.X, Direction.Z) - Math.Sign(Direction.X) * Math.PI
            my_Pitch = Math.Atan2(Direction.Y, Math.Sqrt(Direction.X ^ 2 + Direction.Z ^ 2))
        End Set
    End Property

    Public Property Up As Vector3D
        Get
            Return my_Up
        End Get
        Set(value As Vector3D)
            my_Up = value
        End Set
    End Property

    Public Property Yaw As Double
        Get
            Return my_Yaw
        End Get
        Set(value As Double)
            my_Yaw = value
            my_Direction = New Vector3D() With
            {
                .X = Math.Cos(my_Pitch * Math.PI / 180) * Math.Sin(my_Yaw * Math.PI / 180),
                .Y = Math.Sin(my_Pitch * Math.PI / 180),
                .Z = -1 * Math.Cos(my_Pitch * Math.PI / 180) * Math.Cos(my_Yaw * Math.PI / 180)
            }
        End Set
    End Property

    Public Property Pitch As Double
        Get
            Return my_Pitch
        End Get
        Set(value As Double)
            my_Pitch = value
            my_Direction = New Vector3D() With
            {
                .X = Math.Cos(my_Pitch * Math.PI / 180) * Math.Sin(my_Yaw * Math.PI / 180),
                .Y = Math.Sin(my_Pitch * Math.PI / 180),
                .Z = -1 * Math.Cos(my_Pitch * Math.PI / 180) * Math.Cos(my_Yaw * Math.PI / 180)
            }
        End Set
    End Property

    Public ReadOnly Property X As Single
        Get
            Return CSng(my_Position.X)
        End Get
    End Property

    Public ReadOnly Property Y As Single
        Get
            Return CSng(my_Position.Y)
        End Get
    End Property

    Public ReadOnly Property Z As Single
        Get
            Return CSng(my_Position.Z)
        End Get
    End Property

    Public Property MoveSpeed As Double
        Get
            Return my_MoveSpeed
        End Get
        Set(value As Double)
            my_MoveSpeed = value
        End Set
    End Property

    Public Sub Forward(amount As Double)
        my_Position += amount * my_MoveSpeed * my_Direction
    End Sub

    Public Function GetViewMatrix() As mat4
        Dim result As mat4 = mat4.identity()
        Dim X, Y, Z As Vector3D
        Z = -my_Direction
        Z.Normalize()
        Y = Up
        X = Vector3D.CrossProduct(Y, Z)
        Y = Vector3D.CrossProduct(Z, X)
        X.Normalize()
        Y.Normalize()
        result(0, 0) = CSng(X.X)
        result(1, 0) = CSng(X.Y)
        result(2, 0) = CSng(X.Z)
        result(3, 0) = CSng(Vector3D.DotProduct(-X, Position))
        result(0, 1) = CSng(Y.X)
        result(1, 1) = CSng(Y.Y)
        result(2, 1) = CSng(Y.Z)
        result(3, 1) = CSng(Vector3D.DotProduct(-Y, Position))
        result(0, 2) = CSng(Z.X)
        result(1, 2) = CSng(Z.Y)
        result(2, 2) = CSng(Z.Z)
        result(3, 2) = CSng(Vector3D.DotProduct(-Z, Position))
        result(0, 3) = 0.0F
        result(1, 3) = 0.0F
        result(2, 3) = 0.0F
        result(3, 3) = 1.0F
        Return result
    End Function

End Class
