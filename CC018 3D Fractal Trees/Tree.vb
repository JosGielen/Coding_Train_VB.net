Imports System.Windows.Media.Media3D

Public Class Tree
    Private my_Leafs As List(Of Leaf)
    Private my_Branches As List(Of Branch)
    Private my_Killdist As Double
    Private my_Viewdist As Double
    Private my_RootGrow As Boolean

    Public Sub New(killdist As Double, viewdist As Double)
        my_Killdist = killdist
        my_Viewdist = viewdist
        my_RootGrow = True
        my_Leafs = New List(Of Leaf)
        my_Branches = New List(Of Branch)
    End Sub

    Public ReadOnly Property Leafs As List(Of Leaf)
        Get
            Return my_Leafs
        End Get
    End Property

    Public ReadOnly Property Branches As List(Of Branch)
        Get
            Return my_Branches
        End Get
    End Property

    Public Sub SetRoot(location As Vector3D, Startdirection As Vector3D, branchlength As Double)
        Dim b As Branch = New Branch(Nothing, location, Startdirection, branchlength)
        my_Branches.Add(b)
    End Sub

    Public Sub AddLeaf(l As Leaf)
        my_Leafs.Add(l)
    End Sub

    Public Function Grow() As List(Of Branch)
        Dim result As List(Of Branch) = New List(Of Branch)
        Dim closestBranch As Branch
        Dim newBranch As Branch
        Dim V As Vector3D
        If my_RootGrow = True Then
            'Step1: Grow the root of the tree untill leaves are reached
            newBranch = my_Branches.Last
            Dim d As Double = 0.0
            For I As Integer = 0 To my_Leafs.Count - 1
                d = Math.Sqrt((newBranch.Location.X - my_Leafs(I).Location.X) ^ 2 + (newBranch.Location.Y - my_Leafs(I).Location.Y) ^ 2 + (newBranch.Location.Z - my_Leafs(I).Location.Z) ^ 2)
                If d < my_Viewdist Then
                    my_RootGrow = False
                    Exit For
                End If
            Next
            If my_RootGrow Then
                my_Branches.Add(newBranch.Spawn())
                result.Add(my_Branches.Last())
            End If
        Else
            'Step2: Make the branches of the tree
            For I As Integer = 0 To my_Leafs.Count - 1
                closestBranch = my_Leafs(I).ClosestBranch(my_Branches, my_Killdist, my_Viewdist)
                If closestBranch IsNot Nothing Then
                    V = my_Leafs(I).Location - closestBranch.Location
                    closestBranch.AddForce(V)
                End If
            Next
            'Add branches
            For I As Integer = my_Branches.Count - 1 To 0 Step -1
                If my_Branches(I).ForceCount > 0 Then
                    newBranch = my_Branches(I).Spawn()
                    my_Branches.Add(newBranch)
                    result.Add(newBranch)
                    my_Branches(I).Reset()
                End If
            Next
            'Remove dead leaves
            For I As Integer = my_Leafs.Count - 1 To 0 Step -1
                If Not my_Leafs(I).Alive Then my_Leafs.RemoveAt(I)
            Next
        End If
        Return result
    End Function

End Class
