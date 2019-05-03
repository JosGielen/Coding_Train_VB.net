Imports System.Windows.Media.Media3D

Public Class EllipsoidGeometry
    Inherits GLGeometry
    Private my_Size As Double
    Private my_Steps As Integer

    'Supershape parameters
    Public a1 As Double = 1.0
    Public b1 As Double = 1.0
    Public m1 As Double = 7.0
    Public n11 As Double = 0.2
    Public n21 As Double = 1.7
    Public n31 As Double = 1.7
    Public a2 As Double = 1.0
    Public b2 As Double = 1.0
    Public m2 As Double = 7.0
    Public n12 As Double = 0.2
    Public n22 As Double = 1.7
    Public n32 As Double = 1.7

    Public Sub New(size As Double, steps As Integer)
        my_Size = size
        my_Steps = steps
        my_VertexCount = (steps + 1) * (steps + 1)
    End Sub

    Public Property Size As Double
        Get
            Return my_Size
        End Get
        Set(value As Double)
            my_Size = value
        End Set
    End Property

    Protected Overrides Sub CreateVertices()
        my_VertexCount = (my_Steps + 1) * (my_Steps + 1)
        Dim rm As Matrix3D = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z)
        ReDim my_vertices(my_VertexCount - 1)
        Dim count As Integer = 0
        Dim r1 As Double
        Dim r2 As Double
        Dim theta As Double = 0.0
        Dim phi As Double = 0.0
        Dim x As Double = 0.0
        Dim y As Double = 0.0
        Dim z As Double = 0.0
        'Calculate the vertex positions
        For I As Integer = 0 To my_Steps
            phi = I * Math.PI / my_Steps - Math.PI / 2          '-PI/2 to PI/2
            r2 = SuperShapeRadius(phi, a2, b2, m2, n12, n22, n32)
            For J As Integer = 0 To my_Steps
                theta = 2 * J * Math.PI / my_Steps - Math.PI    '-PI to PI
                r1 = SuperShapeRadius(theta, a1, b1, m1, n11, n21, n31)
                x = my_Size * r1 * Math.Cos(theta) * r2 * Math.Cos(phi)
                y = my_Size * r1 * Math.Sin(theta) * r2 * Math.Cos(phi)
                z = my_Size * r2 * Math.Sin(phi)
                my_vertices(count) = New Vector3D(x, y, z)
                count += 1
            Next
        Next
        'Apply the initial rotation
        For I As Integer = 0 To my_vertices.Count - 1
            my_vertices(I) = rm.Transform(my_vertices(I))
        Next
    End Sub

    Private Function SuperShapeRadius(angle As Double, a As Double, b As Double, m As Double, n1 As Double, n2 As Double, n3 As Double) As Double
        Dim T1 As Double = Math.Abs(Math.Cos(m * angle / 4) / a)
        T1 = Math.Pow(T1, n2)
        Dim T2 As Double = Math.Abs(Math.Sin(m * angle / 4) / b)
        T2 = Math.Pow(T2, n3)
        Return Math.Pow((T1 + T2), (-1 / n1))
    End Function

    Protected Overrides Sub CreateNormals()
        Dim rm As Matrix3D = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z)
        ReDim my_normals(my_VertexCount - 1)
        Dim count As Integer = 0
        Const E As Double = 0.01
        Dim r1 As Double
        Dim r2 As Double
        Dim theta As Double = 0.0
        Dim phi As Double = 0.0
        Dim x As Double = 0.0
        Dim y As Double = 0.0
        Dim z As Double = 0.0
        'Calculate the normals for each vertex position
        For I As Integer = 0 To my_Steps
            For J As Integer = 0 To my_Steps
                phi = I * Math.PI / my_Steps - Math.PI / 2
                r2 = SuperShapeRadius(phi, a2, b2, m2, n12, n22, n32)
                theta = 2 * J * Math.PI / my_Steps - Math.PI
                r1 = SuperShapeRadius(theta, a1, b1, m1, n11, n21, n31)
                x = my_Size * r1 * Math.Cos(theta) * r2 * Math.Cos(phi)
                y = my_Size * r1 * Math.Sin(theta) * r2 * Math.Cos(phi)
                z = my_Size * r2 * Math.Sin(phi)
                Dim p As Vector3D = New Vector3D(x, y, z)

                phi = (I + E) * Math.PI / my_Steps - Math.PI / 2
                r2 = SuperShapeRadius(phi, a2, b2, m2, n12, n22, n32)
                theta = 2 * J * Math.PI / my_Steps - Math.PI
                r1 = SuperShapeRadius(theta, a1, b1, m1, n11, n21, n31)
                x = my_Size * r1 * Math.Cos(theta) * r2 * Math.Cos(phi)
                y = my_Size * r1 * Math.Sin(theta) * r2 * Math.Cos(phi)
                z = my_Size * r2 * Math.Sin(phi)
                Dim u As Vector3D = New Vector3D(x, y, z) - p

                phi = I * Math.PI / my_Steps - Math.PI / 2
                r2 = SuperShapeRadius(phi, a2, b2, m2, n12, n22, n32)
                theta = 2 * (J + E) * Math.PI / my_Steps - Math.PI
                r1 = SuperShapeRadius(theta, a1, b1, m1, n11, n21, n31)
                x = my_Size * r1 * Math.Cos(theta) * r2 * Math.Cos(phi)
                y = my_Size * r1 * Math.Sin(theta) * r2 * Math.Cos(phi)
                z = my_Size * r2 * Math.Sin(phi)
                Dim v As Vector3D = New Vector3D(x, y, z) - p
                my_normals(count) = Vector3D.CrossProduct(v, u)
                my_normals(count).Normalize()
                count += 1
            Next
        Next
        'Apply the initial rotation
        For I As Integer = 0 To my_normals.Count - 1
            my_normals(I) = rm.Transform(my_normals(I))
        Next
    End Sub

    Protected Overrides Sub CreateIndices()
        Dim indexCount As Integer = 6 * (my_Steps * (my_Steps + 1)) - 1
        ReDim my_indices(indexCount)
        Dim count As Integer = 0
        Dim K1 As Integer
        Dim K2 As Integer
        K1 = 0
        K2 = my_Steps + 1
        For I As Integer = 0 To my_Steps - 2
            For J As Integer = 1 To my_Steps
                my_indices(count) = K1
                count += 1
                my_indices(count) = K2
                count += 1
                my_indices(count) = K1 + 1
                count += 1
                my_indices(count) = K1 + 1
                count += 1
                my_indices(count) = K2
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
        Dim ds As Double = 1 / (my_Steps - 1)
        Dim dt As Double = 1 / my_Steps
        Dim dist As Double
        Dim min As Double = Double.MaxValue
        Dim max As Double = Double.MinValue
        'X Texture coordinate = fixed as 0.5
        'Y Texture coordinate depends on the distance to (0,0,0)
        For I As Integer = 0 To my_vertices.Count - 1
            dist = Math.Sqrt(my_vertices(I).X ^ 2 + my_vertices(I).Y ^ 2 + my_vertices(I).Z ^ 2)
            If dist < min Then min = dist
            If dist > max Then max = dist
        Next
        For I As Integer = 0 To my_vertices.Count - 1
            dist = Math.Sqrt(my_vertices(I).X ^ 2 + my_vertices(I).Y ^ 2 + my_vertices(I).Z ^ 2)
            my_textureCoords(I) = New Vector(0.5, (dist - min) / (max - min))
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
