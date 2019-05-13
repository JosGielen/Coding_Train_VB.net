Imports System.Windows.Media.Media3D


Public Class TrackballCamera
    Inherits GLCamera

    ''' <summary>
    ''' A Trackball Camera rotates around a targetposition and always looks at that position
    ''' </summary>
    Public Sub New()
        my_Type = CameraType.Trackball
        my_Position = New Point3D(0, 0, 1)
        my_TargetPosition = New Point3D(0, 0, 0)
        my_ViewDirection = my_TargetPosition - my_Position
        my_Distance = 1.0
        my_UpDirection = New Vector3D(0, 1, 0)
        my_Yaw = Math.PI
        my_Pitch = 0.0
        my_MoveSpeed = 1.0
    End Sub

    ''' <summary>
    ''' A Trackball Camera rotates around a targetposition and always looks at that position
    ''' </summary>
    Public Sub New(position As Point3D, targetPosition As Point3D, up As Vector3D)
        my_Type = CameraType.Trackball
        my_Position = position
        my_TargetPosition = targetPosition
        my_UpDirection = up
        my_MoveSpeed = 1.0
        UpdateDirection()
    End Sub

    Protected Property DistanceToTarget As Double
        Get
            Return my_Distance
        End Get
        Set(value As Double)
            my_Distance = value
            UpdatePosition()
        End Set
    End Property

    Public Overrides Sub Vertical(amount As Double)
        my_Pitch -= amount * my_MoveSpeed * Math.PI / 180
        If my_Pitch < -0.49 * Math.PI Then my_Pitch = -0.49 * Math.PI
        If my_Pitch > 0.49 * Math.PI Then my_Pitch = 0.49 * Math.PI
        UpdatePosition()
    End Sub

    Public Overrides Sub Horizontal(amount As Double)
        my_Yaw += amount * my_MoveSpeed * Math.PI / 180
        UpdatePosition()
    End Sub

    Public Overrides Sub Forward(amount As Double)
        my_Distance += amount * my_MoveSpeed
        UpdatePosition()
    End Sub

    Public Overrides Sub UpdateDirection()
        my_ViewDirection = my_TargetPosition - my_Position
        my_Distance = my_ViewDirection.Length
        my_ViewDirection.Normalize()
        my_Yaw = Vector3D.AngleBetween(New Vector3D(my_ViewDirection.X, 0, my_ViewDirection.Z), New Vector3D(0, 0, -1)) * Math.PI / 180
        my_Pitch = Vector3D.AngleBetween(my_ViewDirection, New Vector3D(my_ViewDirection.X, 0, my_ViewDirection.Z)) * Math.PI / 180
    End Sub

    Public Overrides Sub UpdatePosition()
        my_Position = New Point3D() With
        {
            .X = -my_Distance * Math.Sin(my_Yaw) * Math.Cos(my_Pitch) + my_TargetPosition.X,
            .Y = my_Distance * Math.Sin(my_Pitch) + my_TargetPosition.Y,
            .Z = my_Distance * Math.Cos(my_Yaw) * Math.Cos(my_Pitch) + my_TargetPosition.Z
        }
        my_ViewDirection = my_TargetPosition - my_Position
    End Sub

End Class
