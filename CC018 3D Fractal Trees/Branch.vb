Imports System.Windows.Media.Media3D

Public Class Branch
    Private my_Parent As Branch
    Private my_Location As Vector3D
    Private my_Direction As Vector3D
    Private my_ForceDir As Vector3D
    Private my_ForceCount As Integer
    Private my_Length As Double

    Public Sub New(parent As Branch, loc As Vector3D, dir As Vector3D, length As Double)
        my_Parent = parent
        my_Location = loc
        my_ForceDir = dir
        my_Direction = dir
        my_Length = length
        my_ForceCount = 0
        my_Direction.Normalize()
        my_Direction = length * my_Direction
    End Sub

    Public ReadOnly Property Parent As Branch
        Get
            Return my_Parent
        End Get
    End Property

    Public ReadOnly Property Location As Vector3D
        Get
            Return my_Location
        End Get
    End Property

    Public ReadOnly Property ForceCount As Integer
        Get
            Return my_ForceCount
        End Get
    End Property

    Public Sub Reset()
        my_ForceCount = 0
        my_ForceDir = my_Direction
    End Sub

    Public Sub AddForce(force As Vector3D)
        my_ForceDir = my_ForceDir + force
        my_ForceCount += 1
    End Sub

    Public Function Spawn() As Branch
        Dim result As Branch = Nothing
        If my_ForceCount > 0 Then
            my_ForceDir = my_ForceDir / my_ForceCount
            my_ForceDir.Normalize()
            my_ForceDir = my_Length * my_ForceDir
            result = New Branch(Me, my_Location + my_ForceDir, my_ForceDir, my_Length)
        Else
            result = New Branch(Me, my_Location + my_Direction, my_Direction, my_Length)
        End If
        Return result
    End Function

End Class
