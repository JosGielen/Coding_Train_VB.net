Imports System.Windows.Media.Media3D

Public Class FixedCamera
    Inherits GLCamera

    Private my_StartPosition As Point3D
    Private my_StartTargetPosition As Point3D
    Private my_startUpDirection As Vector3D

    ''' <summary>
    ''' A Fixed camera can not be changed at all.
    ''' </summary>
    ''' <param name="position">The camera will always stay at this position.</param>
    ''' <param name="targetPosition">The camera will always look at this target position.</param>
    ''' <param name="up">The Up direction determines the roll angle of the camera. It can not be changed.</param>
    Public Sub New(position As Point3D, targetPosition As Point3D, up As Vector3D)
        my_Type = CameraType.Fixed
        my_StartPosition = position
        my_StartTargetPosition = targetPosition
        my_startUpDirection = up
        my_MoveSpeed = 0.0
        my_Position = my_StartPosition
        my_TargetPosition = my_StartTargetPosition
        my_UpDirection = my_startUpDirection
        my_ViewDirection = my_StartTargetPosition - my_StartPosition
        my_Distance = my_ViewDirection.Length
        my_ViewDirection.Normalize()
    End Sub

    ''' <summary>
    ''' Not implemented for a Fixed Camera
    ''' </summary>
    Public Overrides Sub Vertical(Amount As Double)
        Throw New NotImplementedException("A fixed Camera can not be modified.")
    End Sub

    ''' <summary>
    ''' Not implemented for a Fixed Camera
    ''' </summary>
    Public Overrides Sub Horizontal(Amount As Double)
        Throw New NotImplementedException("A fixed Camera can not be modified.")
    End Sub

    ''' <summary>
    ''' Not implemented for a Fixed Camera
    ''' </summary>
    Public Overrides Sub Forward(Amount As Double)
        Throw New NotImplementedException("A fixed Camera can not be modified.")
    End Sub

    Public Overrides Sub UpdateDirection()
        my_MoveSpeed = 0.0
        my_Position = my_StartPosition
        my_TargetPosition = my_StartTargetPosition
        my_UpDirection = my_startUpDirection
        my_ViewDirection = my_StartTargetPosition - my_StartPosition
        my_Distance = my_ViewDirection.Length
        my_ViewDirection.Normalize()
    End Sub

    Public Overrides Sub UpdatePosition()
        my_MoveSpeed = 0.0
        my_Position = my_StartPosition
        my_TargetPosition = my_StartTargetPosition
        my_UpDirection = my_startUpDirection
        my_ViewDirection = my_StartTargetPosition - my_StartPosition
        my_Distance = my_ViewDirection.Length
        my_ViewDirection.Normalize()
    End Sub

End Class
