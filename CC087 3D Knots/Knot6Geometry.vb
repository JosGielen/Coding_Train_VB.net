﻿Imports System.Windows.Media.Media3D

'where 0 < mu < (4 * A3 + 2) * pi

'x = cos(mu) * (A1 - cos(A2 * mu / (2 * A3 + 1)))
'y = sin(mu) * (B1 - cos(B2 * mu / (2 * A3 + 1)))
'z = -sin(C1 * mu / (2 * A3 + 1)))

'A1 = 2      B1 = 2     C1 = 2
'A2 = 2      B2 = 2 
'A3 = 2
'====================================================================

Public Class Knot6Geometry
    Inherits GLGeometry

    Private my_Size As Double
    Private my_Steps As Integer
    Private my_Diameter As Double = 1.0
    Private my_Slices As Integer
    Private centerPoints() As Vector3D
    'Knot parameters
    Public a1 As Double = 2.0
    Public a2 As Double = 2.0
    Public a3 As Double = 2.0
    Public a4 As Double = 0.0
    Public a5 As Double = 0.0
    Public a6 As Double = 0.0
    Public a7 As Double = 0.0
    Public a8 As Double = 0.0
    Public b1 As Double = 2.0
    Public b2 As Double = 2.0
    Public b3 As Double = 0.0
    Public b4 As Double = 0.0
    Public b5 As Double = 0.0
    Public b6 As Double = 0.0
    Public b7 As Double = 0.0
    Public b8 As Double = 0.0
    Public c1 As Double = 2.0
    Public c2 As Double = 0.0
    Public c3 As Double = 0.0
    Public c4 As Double = 0.0
    Public c5 As Double = 0.0
    Public c6 As Double = 0.0
    Public c7 As Double = 0.0
    Public c8 As Double = 0.0

    Public Sub New(size As Double, steps As Integer, diameter As Double, slices As Integer)
        my_Size = size
        my_Steps = steps
        my_Diameter = diameter
        my_Slices = slices
        my_VertexCount = (my_Steps + 1) * (my_Slices + 1)
    End Sub

    Public Property Size As Double
        Get
            Return my_Size
        End Get
        Set(value As Double)
            my_Size = value
        End Set
    End Property

    Public Property Steps As Integer
        Get
            Return my_Steps
        End Get
        Set(value As Integer)
            my_Steps = value
        End Set
    End Property

    Public Property Diameter As Double
        Get
            Return my_Diameter
        End Get
        Set(value As Double)
            my_Diameter = value
        End Set
    End Property

    Public Property Slices As Integer
        Get
            Return my_Slices
        End Get
        Set(value As Integer)
            my_Slices = value
        End Set
    End Property

    Public Sub SetParameters(settingForm As Settings)
        a1 = settingForm.A1
        a2 = settingForm.A2
        a3 = settingForm.A3
        a4 = settingForm.A4
        a5 = settingForm.A5
        a6 = settingForm.A6
        a7 = settingForm.A7
        a8 = settingForm.A8
        b1 = settingForm.B1
        b2 = settingForm.B2
        b3 = settingForm.B3
        b4 = settingForm.B4
        b5 = settingForm.B5
        b6 = settingForm.B6
        b7 = settingForm.B7
        b8 = settingForm.B8
        c1 = settingForm.C1
        c2 = settingForm.C2
        c3 = settingForm.C3
        c4 = settingForm.C4
        c5 = settingForm.C5
        c6 = settingForm.C6
        c7 = settingForm.C7
        c8 = settingForm.C8
    End Sub

    Protected Overrides Sub CreateVertices()
        my_VertexCount = (my_Steps + 1) * (my_Slices + 1)
        Dim rm As Matrix3D = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z)
        ReDim my_vertices(my_VertexCount - 1)
        ReDim centerPoints(my_Steps)
        Dim count As Integer = 0
        Dim mu As Double = 0.0
        Dim x As Double = 0.0
        Dim y As Double = 0.0
        Dim z As Double = 0.0
        Dim V As Vector3D
        Dim V1 As Vector3D
        Dim Aar As AxisAngleRotation3D = New AxisAngleRotation3D(V, 360 / my_Slices)
        Dim rotT As RotateTransform3D = New RotateTransform3D(Aar)
        'Calculate the knot center coordinates
        For I As Integer = 0 To my_Steps
            mu = I * (4 * a3 + 2) * Math.PI / my_Steps
            x = my_Size * Math.Cos(mu) * (a1 - Math.Cos(a2 * mu / (2 * a3 + 1)))
            y = my_Size * Math.Sin(mu) * (b1 - Math.Cos(b2 * mu / (2 * a3 + 1)))
            z = -1 * my_Size * Math.Sin(c1 * mu / (2 * a3 + 1))
            centerPoints(I) = New Vector3D(x, y, z)
        Next
        'Calculate the vertex positions at my_Diameter / 2 around the knot center coordinates 
        For I As Integer = 0 To my_Steps - 1
            V = centerPoints(I + 1) - centerPoints(I)
            Aar = New AxisAngleRotation3D(V, 360 / my_Slices)
            rotT = New RotateTransform3D(Aar)
            V1 = Vector3D.CrossProduct(V, New Vector3D(0, 0, 1))
            V1.Normalize()
            V1 = (my_Diameter / 2) * V1
            For J As Integer = 0 To my_Slices
                my_vertices(count) = V1 + centerPoints(I)
                V1 = rotT.Transform(V1)
                count += 1
            Next
        Next
        'Add vertices around the first knot coordinate again to close the knot.
        V = centerPoints(1) - centerPoints(0)
        Aar = New AxisAngleRotation3D(V, 360 / my_Slices)
        rotT = New RotateTransform3D(Aar)
        If V.X <> 0 Or V.Z <> 0 Then
            V1 = Vector3D.CrossProduct(V, New Vector3D(0, 1, 0))
        ElseIf V.Y <> 0 Then
            V1 = Vector3D.CrossProduct(V, New Vector3D(0, 0, 1))
        End If
        V1.Normalize()
        V1 = (my_Diameter / 2) * V1
        For J As Integer = 0 To my_Slices
            my_vertices(count) = V1 + centerPoints(0)
            V1 = rotT.Transform(V1)
            count += 1
        Next
        'Apply the initial rotation
        For I As Integer = 0 To my_vertices.Count - 1
            my_vertices(I) = rm.Transform(my_vertices(I))
        Next
    End Sub

    Protected Overrides Sub CreateNormals()
        Dim rm As Matrix3D = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z)
        ReDim my_normals(my_VertexCount - 1)
        Dim count As Integer = 0
        'Calculate the normals as vector3D from the knot coordinates towards the vertices 
        For I As Integer = 0 To my_Steps
            For J As Integer = 0 To my_Slices
                my_normals(count) = my_vertices(count) - rm.Transform(centerPoints(I))
                my_normals(count).Normalize()
                count += 1
            Next
        Next
    End Sub

    Protected Overrides Sub CreateIndices()
        Dim indexCount As Integer = 6 * my_Steps * my_Slices - 1
        ReDim my_indices(indexCount)
        Dim count As Integer = 0
        Dim K1 As Integer
        Dim K2 As Integer
        'Triangles = (K1, K+1, K2) - (K2, K1+1, K2+1)
        K1 = 0
        K2 = my_Slices + 1
        For I As Integer = 0 To my_Steps - 1
            For J As Integer = 0 To my_Slices - 1
                my_indices(count) = K1
                count += 1
                my_indices(count) = K1 + 1
                count += 1
                my_indices(count) = K2
                count += 1
                my_indices(count) = K2
                count += 1
                my_indices(count) = K1 + 1
                count += 1
                my_indices(count) = K2 + 1
                count += 1
                K1 = K1 + 1
                K2 = K2 + 1
            Next
            K1 = K1 + 1
            K2 = K2 + 1
        Next
    End Sub

    Protected Overrides Sub CreateTexCoordinates()
        ReDim my_textureCoords(my_VertexCount - 1)
        Dim count As Integer = 0
        'Calculate the texture coordinates for each vertex position
        For I As Integer = 0 To my_Steps
            For J As Integer = 0 To my_Slices
                my_textureCoords(count) = New Vector(my_TextureScaleX * J / my_Slices, my_TextureScaleY * I / my_Steps)
                count += 1
            Next
        Next
    End Sub

    ''' <summary>
    ''' X = number of vertices per stack, Y = number of stacks, Z = 0
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function GetVertexLayout() As Vector3D
        Return New Vector3D(my_Steps + 1, my_Steps, 0)
    End Function

End Class
