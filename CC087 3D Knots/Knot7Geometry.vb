Imports System.Windows.Media.Media3D

'X = A1 * sin(A2 * mu) * cos(A3 * mu) + A4 * cos(A5 * mu) * sin(A6 * mu) + A7 * sin(A8 * mu)
'Y = B1 * sin(B2 * mu) * sin(B3 * mu) + B4 * cos(B5 * mu) * cos(B6 * mu) + B7 * cos(B8 * mu)
'Z = C1 * cos(C2 * mu)


'A1 = -0.45     B1 = -0.45      C1 = 0.75 
'A2 = 1.5       B2 = 1.5        C2 = 1.5
'A3 = 1.0       B3 = 1.0 
'A4 = -0.3      B4 = 0.3
'A5 = 1.5       B5 = 1.5
'A6 = 1.0       B6 = 1.0
'A7 = -0.5      B7 = 0.5
'A8 = 1.0       B8 = 1.0
'====================================================================================

Public Class Knot7Geometry
    Inherits GLGeometry

    Private my_steps As Integer
    Private my_slices As Integer
    Private my_Diameter As Double
    Private my_Size As Double

    'Knot parameters
    Public a1 As Double = -0.45
    Public a2 As Double = 1.5
    Public a3 As Double = 1.0
    Public a4 As Double = -0.3
    Public a5 As Double = 1.5
    Public a6 As Double = 1.0
    Public a7 As Double = -0.5
    Public a8 As Double = 1.0
    Public b1 As Double = -0.45
    Public b2 As Double = 1.5
    Public b3 As Double = 1.0
    Public b4 As Double = 0.3
    Public b5 As Double = 1.5
    Public b6 As Double = 1.0
    Public b7 As Double = 0.5
    Public b8 As Double = 1.0
    Public c1 As Double = 0.75
    Public c2 As Double = 1.5
    Public c3 As Double = 0.0
    Public c4 As Double = 0.0
    Public c5 As Double = 0.0
    Public c6 As Double = 0.0
    Public c7 As Double = 0.0
    Public c8 As Double = 0.0

    Public Sub New(size As Double, steps As Integer, Diameter As Double, slices As Integer)
        my_Diameter = Diameter
        my_Size = size
        my_steps = steps
        my_slices = slices
        my_VertexCount = (my_steps + 1) * (my_slices + 1)
    End Sub

    Public Property Stacks As Integer
        Get
            Return my_steps
        End Get
        Set(value As Integer)
            my_steps = value
            If my_steps < 1 Then my_steps = 1
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

    Public Property Diameter As Double
        Get
            Return my_Diameter
        End Get
        Set(value As Double)
            my_Diameter = value
        End Set
    End Property

    Public Property Size As Double
        Get
            Return my_Size
        End Get
        Set(value As Double)
            my_Size = value
        End Set
    End Property

    ''' <summary>
    ''' X = number of vertices per stack, Y = number of stacks, Z = 0
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function GetVertexLayout() As Vector3D
        Return New Vector3D(my_slices + 1, my_steps + 1, 0)
    End Function

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
        my_VertexCount = (my_steps + 1) * (my_slices + 1)
        Dim rm As Matrix3D = CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z)
        ReDim my_vertices(my_VertexCount - 1)
        Dim count As Integer = 0
        Dim ds As Double = 1 / my_steps
        Dim dt As Double = 1 / my_slices
        'Calculate the vertex positions
        For s As Double = 0 To 1 + ds / 2 Step ds
            For t As Double = 0 To 1 + dt / 2 Step dt
                my_vertices(count) = EvaluateTrefoil(s, t)
                count += 1
            Next
        Next
        'Apply the initial rotation
        For I As Integer = 0 To my_vertices.Count - 1
            my_vertices(I) = rm.Transform(my_vertices(I))
        Next
    End Sub

    Protected Overrides Sub CreateNormals()
        Dim rm As MatrixTransform3D = New MatrixTransform3D(CalculateRotationMatrix(InitialRotationAxis.X, InitialRotationAxis.Y, InitialRotationAxis.Z))
        ReDim my_normals(my_VertexCount - 1)
        Dim count As Integer = 0
        Const E As Double = 0.01
        Dim ds As Double = 1 / my_steps
        Dim dt As Double = 1 / my_slices
        'Calculate the normals for each vertex position
        For s As Double = 0 To 1 + ds / 2 Step ds
            For t As Double = 0 To 1 + dt / 2 Step dt
                Dim p As Vector3D = EvaluateTrefoil(s, t)
                Dim u As Vector3D = EvaluateTrefoil(s + E, t) - p
                Dim v As Vector3D = EvaluateTrefoil(s, t + E) - p
                my_normals(count) = Vector3D.CrossProduct(u, v)
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
        Dim indexCount As Integer = 6 * (my_steps + 1) * (my_slices + 1) - 1
        ReDim my_indices(indexCount)
        Dim count As Integer = 0
        Dim K1 As Integer = 0
        Dim K2 As Integer = my_slices + 1
        For I As Integer = 0 To my_steps - 1
            For J As Integer = 0 To my_slices - 1
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
                K1 += 1
                K2 += 1
            Next
            K1 += 1
            K2 += 1
        Next
    End Sub

    Protected Overrides Sub CreateTexCoordinates()
        ReDim my_textureCoords(my_VertexCount - 1)
        Dim count As Integer = 0
        Dim ds As Double = 1 / my_steps
        Dim dt As Double = 1 / my_slices
        'Calculate the texture coordinates for each vertex position
        For s As Double = 0 To 1 + ds / 2 Step ds
            For t As Double = 0 To 1 + dt / 2 Step dt
                my_textureCoords(count) = New Vector(my_TextureScaleX * t, my_TextureScaleY * s)
                count += 1
            Next
        Next
    End Sub

    Private Function EvaluateTrefoil(s As Double, t As Double) As Vector3D
        Dim a As Double = b7
        Dim b As Double = b4
        Dim c As Double = c1 / 1.5
        Dim d As Double = my_Diameter
        Dim mu As Double = (1 - s) * 4 * Math.PI
        Dim v As Double = t * Math.PI * 2
        Dim r As Double = a + b * Math.Cos(1.5 * mu)
        Dim x As Double = r * Math.Cos(mu)
        Dim y As Double = r * Math.Sin(mu)
        Dim z As Double = c * Math.Sin(1.5 * mu)
        Dim dv As Vector3D
        dv.X = a1 * Math.Sin(a2 * mu) * Math.Cos(a3 * mu) + a4 * Math.Cos(a5 * mu) * Math.Sin(a6 * mu) + a7 * Math.Sin(a8 * mu)
        dv.Y = b1 * Math.Sin(b2 * mu) * Math.Sin(b3 * mu) + b4 * Math.Cos(b5 * mu) * Math.Cos(b6 * mu) + b7 * Math.Cos(b8 * mu)
        dv.Z = c1 * Math.Cos(c2 * mu)
        dv.Normalize()
        Dim qvn As Vector3D = New Vector3D(dv.Y, -dv.X, 0.0)
        qvn.Normalize()
        Dim ww As Vector3D = Vector3D.CrossProduct(dv, qvn)
        Dim range As Vector3D = New Vector3D With
        {
            .X = my_Size * (x + d * (qvn.X * Math.Cos(v) + ww.X * Math.Sin(v))),
            .Y = my_Size * (y + d * (qvn.Y * Math.Cos(v) + ww.Y * Math.Sin(v))),
            .Z = my_Size * (z + d * ww.Z * Math.Sin(v))
        }
        Return range
    End Function

End Class
