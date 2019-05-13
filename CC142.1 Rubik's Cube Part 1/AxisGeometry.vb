Imports System.Windows
Imports System.Windows.Media.Media3D
Imports GlmNet

Public Class AxisGeometry
    Inherits GLGeometry

    Private my_X_Axis As Axis
    Private my_Y_Axis As Axis
    Private my_Z_Axis As Axis
    Private my_ArrowSize As Double
    Private my_LabelSize As Double

    Public Sub New()
        my_ArrowSize = 0.1
        my_LabelSize = 0.15
        my_X_Axis = New Axis With
        {
            .Visible = True,
            .Color = Media.Colors.Lime,
            .Length = 5.0,
            .ShowLabel = True,
            .ShowArrow = True
        }
        my_Y_Axis = New Axis With
        {
            .Visible = True,
            .Color = Media.Colors.Red,
            .Length = 5.0,
            .ShowLabel = True,
            .ShowArrow = True
        }
        my_Z_Axis = New Axis With
        {
            .Visible = True,
            .Color = Media.Colors.Blue,
            .Length = 5.0,
            .ShowLabel = True,
            .ShowArrow = True
        }
    End Sub

    Public Sub New(length As Double, showArrow As Boolean, showLabels As Boolean)
        my_ArrowSize = 0.1
        my_LabelSize = 0.15
        my_X_Axis = New Axis With
        {
            .Visible = True,
            .Color = Media.Colors.Lime,
            .Length = length,
            .ShowLabel = showLabels,
            .ShowArrow = showArrow
        }
        my_Y_Axis = New Axis With
        {
            .Visible = True,
            .Color = Media.Colors.Red,
            .Length = length,
            .ShowLabel = showLabels,
            .ShowArrow = showArrow
        }
        my_Z_Axis = New Axis With
        {
            .Visible = True,
            .Color = Media.Colors.Blue,
            .Length = length,
            .ShowLabel = showLabels,
            .ShowArrow = showArrow
        }
    End Sub

    Public Property X_Axis As Axis
        Get
            Return my_X_Axis
        End Get
        Set(value As Axis)
            my_X_Axis = value
        End Set
    End Property

    Public Property Y_Axis As Axis
        Get
            Return my_Y_Axis
        End Get
        Set(value As Axis)
            my_Y_Axis = value
        End Set
    End Property

    Public Property Z_Axis As Axis
        Get
            Return my_Z_Axis
        End Get
        Set(value As Axis)
            my_Z_Axis = value
        End Set
    End Property

    Public Property ArrowSize As Double
        Get
            Return my_ArrowSize
        End Get
        Set(value As Double)
            my_ArrowSize = value
        End Set
    End Property

    Public Property LabelSize As Double
        Get
            Return my_LabelSize
        End Get
        Set(value As Double)
            my_LabelSize = value
        End Set
    End Property

    Protected Overrides Sub CreateVertices()
        my_VertexCount = GetVertexCount()
        ReDim my_vertices(my_VertexCount - 1)
        Dim count As Integer = 0
        If my_X_Axis.Visible Then
            my_vertices(count) = New Vector3D(0, 0, 0)
            my_vertices(count + 1) = New Vector3D(my_X_Axis.Length, 0, 0)
            count += 2
            If my_X_Axis.ShowArrow Then
                my_vertices(count) = New Vector3D(my_X_Axis.Length - my_ArrowSize, my_ArrowSize, 0)
                my_vertices(count + 1) = New Vector3D(my_X_Axis.Length, 0, 0)
                my_vertices(count + 2) = New Vector3D(my_X_Axis.Length - my_ArrowSize, -my_ArrowSize, 0)
                my_vertices(count + 3) = New Vector3D(my_X_Axis.Length, 0, 0)
                my_vertices(count + 4) = New Vector3D(my_X_Axis.Length - my_ArrowSize, 0, -my_ArrowSize)
                my_vertices(count + 5) = New Vector3D(my_X_Axis.Length, 0, 0)
                my_vertices(count + 6) = New Vector3D(my_X_Axis.Length - my_ArrowSize, 0, my_ArrowSize)
                my_vertices(count + 7) = New Vector3D(my_X_Axis.Length, 0, 0)
                count += 8
            End If
            If my_X_Axis.ShowLabel Then
                my_vertices(count) = New Vector3D(my_X_Axis.Length + my_LabelSize, my_LabelSize, 0)
                my_vertices(count + 1) = New Vector3D(my_X_Axis.Length + 3 * my_LabelSize, -my_LabelSize, 0)
                my_vertices(count + 2) = New Vector3D(my_X_Axis.Length + 3 * my_LabelSize, my_LabelSize, 0)
                my_vertices(count + 3) = New Vector3D(my_X_Axis.Length + my_LabelSize, -my_LabelSize, 0)
                count += 4
            End If
        End If
        If my_Y_Axis.Visible Then
            my_vertices(count) = New Vector3D(0, 0, 0)
            my_vertices(count + 1) = New Vector3D(0, my_Y_Axis.Length, 0)
            count += 2
            If my_Y_Axis.ShowArrow Then
                my_vertices(count) = New Vector3D(-my_ArrowSize, my_Y_Axis.Length - my_ArrowSize, 0)
                my_vertices(count + 1) = New Vector3D(0, my_Y_Axis.Length, 0)
                my_vertices(count + 2) = New Vector3D(my_ArrowSize, my_Y_Axis.Length - my_ArrowSize, 0)
                my_vertices(count + 3) = New Vector3D(0, my_Y_Axis.Length, 0)
                my_vertices(count + 4) = New Vector3D(0, my_Y_Axis.Length - my_ArrowSize, my_ArrowSize)
                my_vertices(count + 5) = New Vector3D(0, my_Y_Axis.Length, 0)
                my_vertices(count + 6) = New Vector3D(0, my_Y_Axis.Length - my_ArrowSize, -my_ArrowSize)
                my_vertices(count + 7) = New Vector3D(0, my_Y_Axis.Length, 0)
                count += 8
            End If
            If my_Y_Axis.ShowLabel Then
                my_vertices(count) = New Vector3D(-my_LabelSize, my_Y_Axis.Length + 3 * my_LabelSize, 0)
                my_vertices(count + 1) = New Vector3D(0, my_Y_Axis.Length + 2 * my_LabelSize, 0)
                my_vertices(count + 2) = New Vector3D(-my_LabelSize, my_Y_Axis.Length + my_LabelSize, 0)
                my_vertices(count + 3) = New Vector3D(my_LabelSize, my_Y_Axis.Length + 3 * my_LabelSize, 0)
                count += 4
            End If
        End If
        If my_Z_Axis.Visible Then
            my_vertices(count) = New Vector3D(0, 0, 0)
            my_vertices(count + 1) = New Vector3D(0, 0, my_Z_Axis.Length)
            count += 2
            If my_Z_Axis.ShowArrow Then
                my_vertices(count) = New Vector3D(-my_ArrowSize, 0, my_Z_Axis.Length - my_ArrowSize)
                my_vertices(count + 1) = New Vector3D(0, 0, my_Z_Axis.Length)
                my_vertices(count + 2) = New Vector3D(my_ArrowSize, 0, my_Z_Axis.Length - my_ArrowSize)
                my_vertices(count + 3) = New Vector3D(0, 0, my_Z_Axis.Length)
                my_vertices(count + 4) = New Vector3D(0, my_ArrowSize, my_Z_Axis.Length - my_ArrowSize)
                my_vertices(count + 5) = New Vector3D(0, 0, my_Z_Axis.Length)
                my_vertices(count + 6) = New Vector3D(0, -my_ArrowSize, my_Z_Axis.Length - my_ArrowSize)
                my_vertices(count + 7) = New Vector3D(0, 0, my_Z_Axis.Length)
                count += 8
            End If
            If my_Z_Axis.ShowLabel Then
                my_vertices(count) = New Vector3D(-my_LabelSize, my_LabelSize, my_Z_Axis.Length + my_LabelSize)
                my_vertices(count + 1) = New Vector3D(my_LabelSize, my_LabelSize, my_Z_Axis.Length + my_LabelSize)
                my_vertices(count + 2) = New Vector3D(my_LabelSize, my_LabelSize, my_Z_Axis.Length + my_LabelSize)
                my_vertices(count + 3) = New Vector3D(-my_LabelSize, -my_LabelSize, my_Z_Axis.Length + my_LabelSize)
                my_vertices(count + 4) = New Vector3D(-my_LabelSize, -my_LabelSize, my_Z_Axis.Length + my_LabelSize)
                my_vertices(count + 5) = New Vector3D(my_LabelSize, -my_LabelSize, my_Z_Axis.Length + my_LabelSize)
            End If
        End If
        'Force  lines mode
        GLBeginMode = SharpGL.Enumerations.BeginMode.Lines
    End Sub

    Protected Overrides Sub CreateNormals()
        ReDim my_normals(my_VertexCount - 1)
        For I As Integer = 0 To my_VertexCount - 1
            my_normals(I) = New Vector3D(0, 0, 1)
        Next
    End Sub

    Protected Overrides Function GetNormalMatrix() As mat3
        Return mat3.identity() 'Prevent shading effects
    End Function

    Protected Overrides Sub CreateIndices()
        ReDim my_indices(my_VertexCount - 1)
        For I As Integer = 0 To my_VertexCount - 1
            my_indices(I) = I
        Next
    End Sub

    Protected Overrides Sub CreateColors()
        ReDim my_colors(my_VertexCount - 1)
        Dim count As Integer = 0
        If my_X_Axis.Visible Then
            For I As Integer = 0 To 1
                my_colors(count + I) = my_X_Axis.Color
            Next
            count += 2
            If my_X_Axis.ShowArrow Then
                For I As Integer = 0 To 7
                    my_colors(count + I) = my_X_Axis.Color
                Next
                count = count + 8
            End If
            If my_X_Axis.ShowLabel Then
                For I As Integer = 0 To 3
                    my_colors(count + I) = my_X_Axis.Color
                Next
                count = count + 4
            End If
        End If
        If my_Y_Axis.Visible Then
            For I As Integer = 0 To 1
                my_colors(count + I) = my_Y_Axis.Color
            Next
            count += 2
            If my_Y_Axis.ShowArrow Then
                For I As Integer = 0 To 7
                    my_colors(count + I) = my_Y_Axis.Color
                Next
                count = count + 8
            End If
            If my_Y_Axis.ShowLabel Then
                For I As Integer = 0 To 3
                    my_colors(count + I) = my_Y_Axis.Color
                Next
                count = count + 4
            End If
        End If
        If my_Z_Axis.Visible Then
            For I As Integer = 0 To 1
                my_colors(count + I) = my_Z_Axis.Color
            Next
            count += 2
            If my_Z_Axis.ShowArrow Then
                For I As Integer = 0 To 7
                    my_colors(count + I) = my_Z_Axis.Color
                Next
                count += 8
            End If
            If my_Z_Axis.ShowLabel Then
                For I As Integer = 0 To 5
                    my_colors(count + I) = my_Z_Axis.Color
                Next
            End If
        End If

    End Sub

    Protected Overrides Sub CreateTexCoordinates()
        ReDim my_textureCoords(my_VertexCount - 1)
        For I As Integer = 0 To my_VertexCount - 1
            my_textureCoords(I) = New Vector(0, 0)
        Next
    End Sub

    ''' <summary>
    ''' X = 2 vertices, Y = 2 vertices, Z = 2 vertices
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function GetVertexLayout() As Vector3D
        Return New Vector3D(2, 2, 2)
    End Function


    Private Function GetVertexCount() As Integer
        Dim count As Integer = 0
        If my_X_Axis.Visible Then
            count += 2
            If my_X_Axis.ShowArrow Then count += 8
            If my_X_Axis.ShowLabel Then count += 4
        End If
        If my_Y_Axis.Visible Then
            count += 2
            If my_Y_Axis.ShowArrow Then count += 8
            If my_Y_Axis.ShowLabel Then count += 4
        End If
        If my_Z_Axis.Visible Then
            count += 2
            If my_Z_Axis.ShowArrow Then count += 8
            If my_Z_Axis.ShowLabel Then count += 6
        End If
        Return count
    End Function

End Class

Public Class Axis
    Public Visible As Boolean
    Public Length As Double
    Public Color As Media.Color
    Public ShowLabel As Boolean
    Public ShowArrow As Boolean
End Class
