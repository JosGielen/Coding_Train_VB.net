Imports System.Windows.Media.Media3D
Imports GlmNet

Public MustInherit Class GLCamera
    Protected my_Type As CameraType
    Protected my_Position As Point3D
    Protected my_TargetPosition As Point3D
    Protected my_ViewDirection As Vector3D
    Protected my_UpDirection As Vector3D
    Protected my_Yaw As Double
    Protected my_Pitch As Double
    Protected my_Distance As Double
    Protected my_MoveSpeed As Double

#Region "Properties"

    Public ReadOnly Property Type As CameraType
        Get
            Return my_Type
        End Get
    End Property

    Public Property Position As Vector3D
        Get
            Return my_Position
        End Get
        Set(value As Vector3D)
            my_Position = value
            UpdateDirection()
        End Set
    End Property

    Public Property TargetPosition As Point3D
        Get
            Return my_TargetPosition
        End Get
        Set(value As Point3D)
            my_TargetPosition = value
            UpdateDirection()
        End Set
    End Property

    Public Property UpDirection As Vector3D
        Get
            Return my_UpDirection
        End Get
        Set(value As Vector3D)
            my_UpDirection = value
            UpdateDirection()
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

#End Region

    Public Function GetViewMatrix() As mat4
        Dim result As mat4 = mat4.identity()
        Dim X, Y, Z As Vector3D
        Z = -my_ViewDirection
        Z.Normalize()
        Y = UpDirection
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

    Public MustOverride Sub Vertical(Amount As Double)

    Public MustOverride Sub Horizontal(Amount As Double)

    Public MustOverride Sub Forward(Amount As Double)

    Public MustOverride Sub UpdatePosition()

    Public MustOverride Sub UpdateDirection()

End Class

Public Enum CameraType
    Fixed = 0
    ParentControlled = 1
    FreeFlying = 2
    Trackball = 3
End Enum
