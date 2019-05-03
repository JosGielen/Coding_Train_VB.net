Imports System.Windows.Media.Media3D

Public Class Leaf
    Private my_Location As Vector3D
    Private my_Alive As Boolean

    Public Sub New(loc As Vector3D)
        my_Location = loc
        my_Alive = True
    End Sub

    Public Property Location As Vector3D
        Get
            Return my_Location
        End Get
        Set(value As Vector3D)
            my_Location = value
        End Set
    End Property

    Public ReadOnly Property Alive As Boolean
        Get
            Return my_Alive
        End Get
    End Property

    Public Function ClosestBranch(branches As List(Of Branch), killDist As Double, viewDist As Double) As Branch
        Dim dist As Double = 0.0
        Dim mindist As Double = Double.MaxValue
        Dim closest As Branch = Nothing
        For I As Integer = 0 To branches.Count - 1
            dist = Math.Sqrt((my_Location.X - branches(I).Location.X) ^ 2 + (my_Location.Y - branches(I).Location.Y) ^ 2 + (my_Location.Z - branches(I).Location.Z) ^ 2)
            If dist < killDist Then
                my_Alive = False
                Return Nothing
            End If
            If dist < viewDist Then
                If dist < mindist Then
                    mindist = dist
                    closest = branches(I)
                End If
            End If
        Next
        Return closest
    End Function

End Class
