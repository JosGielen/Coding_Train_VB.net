Imports System.Windows
Imports System.Windows.Media.Media3D

Public Class EllipsoidGeometry
    Inherits GLGeometry
    Private my_X_Size As Double
    Private my_Y_Size As Double
    Private my_Z_Size As Double
    Private my_stacks As Integer
    Private my_slices As Integer

    Public Sub New(X_Size As Double, Y_Size As Double, Z_Size As Double)
        my_X_Size = X_Size
        my_Y_Size = Y_Size
        my_Z_Size = Z_Size
        my_stacks = 16
        my_slices = 32
        my_VertexCount = my_stacks * (my_slices + 1)
    End Sub

    ''' <summary>
    ''' Create a new Ellipsoid
    ''' </summary>
    ''' <param name="stacks">The geometry is divided into horizontal stacks</param>
    ''' <param name="slices">Each stack is divides into slices</param>
    Public Sub New(X_Size As Double, Y_Size As Double, Z_Size As Double, stacks As Integer, slices As Integer)
        my_X_Size = X_Size
        my_Y_Size = Y_Size
        my_Z_Size = Z_Size
        my_stacks = stacks
        my_slices = slices
        my_VertexCount = my_stacks * (my_slices + 1)
    End Sub

    ''' <summary>
    ''' Create a new Ellipsoid with size 1 along all axes (= sphere)
    ''' </summary>
    ''' <param name="stacks">The geometry is divided into horizontal stacks</param>
    ''' <param name="slices">Each stack is divides into slices</param>
    Public Sub New(stacks As Integer, slices As Integer)
        my_X_Size = 1.0
        my_Y_Size = 1.0
        my_Z_Size = 1.0
        my_stacks = stacks
        my_slices = slices
        my_VertexCount = my_stacks * (my_slices + 1)
    End Sub

    Public Property X_Size As Double
        Get
            Return my_X_Size
        End Get
        Set(value As Double)
            my_X_Size = value
        End Set
    End Property

    Public Property Y_Size As Double
        Get
            Return my_Y_Size
        End Get
        Set(value As Double)
            my_Y_Size = value
        End Set
    End Property

    Public Property Z_Size As Double
        Get
            Return my_Z_Size
        End Get
        Set(value As Double)
            my_Z_Size = value
        End Set
    End Property

    Public Property Stacks As Integer
        Get
            Return my_stacks
        End Get
        Set(value As Integer)
            my_stacks = value
            If my_stacks < 2 Then my_stacks = 2
        End Set
    End Property

    Public Property Slices As Integer
        Get
            Return my_slices
        End Get
        Set(value As Integer)
            my_slices = value
            If my_slices < 1 Then my_slices = 1
        End Set
    End Property

    Protected Overrides Sub CreateVertices(percent As Integer)
        my_VertexCount = my_stacks * (my_slices + 1)
        Dim rm As Matrix3D = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z)
        ReDim my_vertices(my_VertexCount - 1)
        Dim count As Integer = 0
        Dim ds As Double = 1 / (my_stacks - 1)
        Dim dt As Double = 1 / my_slices
        'Calculate the vertex positions

        Dim XOff As Double = 0.0
        Dim YOff As Double = 0.0
        Dim ZOff As Double = 0.0

        Dim Offx As Double = 0.0
        Dim Offy As Double = 10.0
        Dim F As Double = 4.0 'Determines how fast the Noise changes
        Dim R As Double = 0.0
        Dim Angle As Double = 0.0
        Offx = Math.Cos(2 * percent * Math.PI / 100)
        Offy = Math.Sin(2 * percent * Math.PI / 100)

        Dim theta As Double = 0.0
        Dim phi As Double = 0.0
        Dim x As Double = 0.0
        Dim y As Double = 0.0
        Dim z As Double = 0.0
        R = my_Y_Size * OpenSimplexNoise.Simplex3D(Offx, F + Offy, 0)
        R = R / 2 + 0.3
        For I As Integer = 0 To my_slices
            my_vertices(count) = New Vector3D(0, R, 0) 'Top verteces
            count += 1
        Next
        For s As Double = ds To 1 - ds / 2 Step ds
            For t As Double = 0 To 1 + dt / 2 Step dt
                theta = t * 2 * Math.PI 'tracks
                phi = s * Math.PI  'slices
                XOff = F * Math.Sin(theta) * Math.Sin(phi)
                YOff = F * Math.Cos(phi)
                ZOff = F * Math.Cos(theta) * Math.Sin(phi)
                R = my_Y_Size * OpenSimplexNoise.Simplex3D(XOff + Offx, YOff + Offy, ZOff)
                R = R / 2 + 0.3
                x = R * Math.Sin(theta) * Math.Sin(phi)
                y = R * Math.Cos(phi)
                z = -R * Math.Cos(theta) * Math.Sin(phi)
                my_vertices(count) = New Vector3D(x, y, z)
                count += 1
            Next
        Next
        For I As Integer = 0 To my_slices
            my_vertices(count) = New Vector3D(0, -R, 0) 'Bottom verteces
            count += 1
        Next
        'Apply the initial rotation
        For I As Integer = 0 To my_vertices.Count - 1
            my_vertices(I) = rm.Transform(my_vertices(I))
        Next
    End Sub

    Protected Overrides Sub CreateNormals(percent As Integer)
        Dim rm As Matrix3D = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z)
        ReDim my_normals(my_VertexCount - 1)
        Dim count As Integer = 0
        Const E As Double = 0.01
        Dim ds As Double = 1 / (my_stacks - 1)
        Dim dt As Double = 1 / my_slices
        Dim XOff As Double = 0.0
        Dim YOff As Double = 0.0
        Dim ZOff As Double = 0.0

        Dim Offx As Double = 0.0
        Dim Offy As Double = 10.0
        Dim F As Double = 4.0 'Determines how fast the Noise changes
        Dim R As Double = 0.0
        Dim Angle As Double = 0.0
        Offx = Math.Cos(2 * percent * Math.PI / 100)
        Offy = Math.Sin(2 * percent * Math.PI / 100)

        Dim theta As Double = 0.0
        Dim phi As Double = 0.0
        Dim x As Double = 0.0
        Dim y As Double = 0.0
        Dim z As Double = 0.0
        'Calculate the normals for each vertex position
        For I As Integer = 0 To my_slices
            my_normals(count) = New Vector3D(0, 1, 0) 'Normal of the top verteces
            count += 1
        Next
        For s As Double = ds To 1 - ds / 2 Step ds
            For t As Double = 0 To 1 + dt / 2 Step dt
                theta = t * 2 * Math.PI 'tracks
                phi = s * Math.PI  'slices
                XOff = F * Math.Sin(theta) * Math.Sin(phi)
                YOff = F * Math.Cos(phi)
                ZOff = F * Math.Cos(theta) * Math.Sin(phi)
                R = my_Y_Size * OpenSimplexNoise.Simplex3D(XOff + Offx, YOff + Offy, ZOff)
                R = R / 2 + 0.3
                x = R * Math.Sin(theta) * Math.Sin(phi)
                y = R * Math.Cos(phi)
                z = -R * Math.Cos(theta) * Math.Sin(phi)
                Dim p As Vector3D = New Vector3D(x, y, z)

                theta = t * 2 * Math.PI 'tracks
                phi = (s + E) * Math.PI  'slices
                XOff = F * Math.Sin(theta) * Math.Sin(phi)
                YOff = F * Math.Cos(phi)
                ZOff = F * Math.Cos(theta) * Math.Sin(phi)
                R = my_Y_Size * OpenSimplexNoise.Simplex3D(XOff + Offx, YOff + Offy, ZOff)
                R = R / 2 + 0.3
                x = R * Math.Sin(theta) * Math.Sin(phi)
                y = R * Math.Cos(phi)
                z = -R * Math.Cos(theta) * Math.Sin(phi)
                Dim u As Vector3D = New Vector3D(x, y, z) - p

                theta = (t + E) * 2 * Math.PI 'tracks
                phi = s * Math.PI  'slices
                XOff = F * Math.Sin(theta) * Math.Sin(phi)
                YOff = F * Math.Cos(phi)
                ZOff = F * Math.Cos(theta) * Math.Sin(phi)
                R = my_Y_Size * OpenSimplexNoise.Simplex3D(XOff + Offx, YOff + Offy, ZOff)
                R = R / 2 + 0.3
                x = R * Math.Sin(theta) * Math.Sin(phi)
                y = R * Math.Cos(phi)
                z = -R * Math.Cos(theta) * Math.Sin(phi)
                Dim v As Vector3D = New Vector3D(x, y, z) - p
                my_normals(count) = Vector3D.CrossProduct(v, u)
                my_normals(count).Normalize()
                count += 1
            Next
        Next
        For I As Integer = 0 To my_slices
            my_normals(count) = New Vector3D(0, -1, 0) 'Normal of the bottom verteces
            count += 1
        Next
        'Apply the initial rotation
        For I As Integer = 0 To my_normals.Count - 1
            my_normals(I) = rm.Transform(my_normals(I))
        Next
    End Sub

    Protected Overrides Sub CreateIndices()
        Dim indexCount As Integer = 6 * ((my_stacks - 1) * my_slices) - 1
        ReDim my_indices(indexCount)
        Dim count As Integer = 0
        Dim K1 As Integer
        Dim K2 As Integer
        K1 = 0
        K2 = my_slices + 1
        For I As Integer = 0 To my_stacks - 2
            For J As Integer = 1 To my_slices
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
        Dim ds As Double = 1 / (my_stacks - 1)
        Dim dt As Double = 1 / my_slices
        Dim dist As Double
        Dim min As Double = Double.MaxValue
        Dim max As Double = Double.MinValue
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
        Return New Vector3D(my_slices + 1, my_stacks, 0)
    End Function
End Class
