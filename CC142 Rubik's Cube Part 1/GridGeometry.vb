Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D
Imports GlmNet

Public Class GridGeometry
    Inherits GLGeometry

    Private my_MaxSize As Double
    Private my_Interval As Double
    Private my_XYColor As Media.Color
    Private my_XZColor As Media.Color
    Private my_YZColor As Media.Color
    Private my_ShowXY_XDirection As Boolean
    Private my_ShowXY_YDirection As Boolean
    Private my_ShowXZ_XDirection As Boolean
    Private my_ShowXZ_ZDirection As Boolean
    Private my_ShowYZ_YDirection As Boolean
    Private my_ShowYZ_Zdirection As Boolean
    Private LinesPerGrid As Integer

    Public Sub New()
        my_MaxSize = 5.0
        my_Interval = 1.0
        my_XYColor = Media.Colors.DarkGray
        my_XZColor = Media.Colors.DarkGray
        my_YZColor = Media.Colors.DarkGray
        my_ShowXY_XDirection = True
        my_ShowXY_YDirection = True
        my_ShowXZ_XDirection = True
        my_ShowXZ_ZDirection = True
        my_ShowYZ_YDirection = True
        my_ShowYZ_Zdirection = True
    End Sub

    Public Sub New(maxSize As Double, interval As Double)
        my_MaxSize = maxSize
        my_Interval = interval
        my_XYColor = Media.Colors.DarkGray
        my_XZColor = Media.Colors.DarkGray
        my_YZColor = Media.Colors.DarkGray
        my_ShowXY_XDirection = True
        my_ShowXY_YDirection = True
        my_ShowXZ_XDirection = True
        my_ShowXZ_ZDirection = True
        my_ShowYZ_YDirection = True
        my_ShowYZ_Zdirection = True
    End Sub

    Public Sub New(maxSize As Double, interval As Double, showXY As Boolean, showXZ As Boolean, showYZ As Boolean)
        my_MaxSize = maxSize
        my_Interval = interval
        my_XYColor = Media.Colors.DarkGray
        my_XZColor = Media.Colors.DarkGray
        my_YZColor = Media.Colors.DarkGray
        my_ShowXY_XDirection = showXY
        my_ShowXY_YDirection = showXY
        my_ShowXZ_XDirection = showXZ
        my_ShowXZ_ZDirection = showXZ
        my_ShowYZ_YDirection = showYZ
        my_ShowYZ_Zdirection = showYZ
    End Sub

#Region "Properties"

    Public Property MaxSize As Double
        Get
            Return my_MaxSize
        End Get
        Set(value As Double)
            my_MaxSize = value
        End Set
    End Property

    Public Property Interval As Double
        Get
            Return my_Interval
        End Get
        Set(value As Double)
            If value > 0 Then my_Interval = value
        End Set
    End Property

    Public Property XYColor As Color
        Get
            Return my_XYColor
        End Get
        Set(value As Color)
            my_XYColor = value
        End Set
    End Property

    Public Property XZColor As Color
        Get
            Return my_XZColor
        End Get
        Set(value As Color)
            my_XZColor = value
        End Set
    End Property

    Public Property YZColor As Color
        Get
            Return my_YZColor
        End Get
        Set(value As Color)
            my_YZColor = value
        End Set
    End Property

    Public Property ShowXY_Xdirection As Boolean
        Get
            Return my_ShowXY_XDirection
        End Get
        Set(value As Boolean)
            my_ShowXY_XDirection = value
        End Set
    End Property

    Public Property ShowXY_YDirection As Boolean
        Get
            Return my_ShowXY_YDirection
        End Get
        Set(value As Boolean)
            my_ShowXY_YDirection = value
        End Set
    End Property

    Public Property ShowXZ_XDirection As Boolean
        Get
            Return my_ShowXZ_XDirection
        End Get
        Set(value As Boolean)
            my_ShowXZ_XDirection = value
        End Set
    End Property

    Public Property ShowXZ_ZDirection As Boolean
        Get
            Return my_ShowXZ_ZDirection
        End Get
        Set(value As Boolean)
            my_ShowXZ_ZDirection = value
        End Set
    End Property

    Public Property ShowYZ_YDirection As Boolean
        Get
            Return my_ShowYZ_YDirection
        End Get
        Set(value As Boolean)
            my_ShowYZ_YDirection = value
        End Set
    End Property

    Public Property ShowYZ_ZDirection As Boolean
        Get
            Return my_ShowYZ_Zdirection
        End Get
        Set(value As Boolean)
            my_ShowYZ_Zdirection = value
        End Set
    End Property

#End Region

    Protected Overrides Sub CreateVertices()
        my_VertexCount = GetVertexCount()
        Dim count As Integer = 0
        ReDim my_vertices(my_VertexCount - 1)
        If my_ShowXY_XDirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(-my_MaxSize, I * my_Interval, 0)
                my_vertices(count + 1) = New Vector3D(my_MaxSize, I * my_Interval, 0)
                count += 2
            Next
            my_vertices(count) = New Vector3D(0, 0, 0)
            my_vertices(count + 1) = New Vector3D(-my_MaxSize, 0, 0)
            count += 2
            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(-my_MaxSize, -I * my_Interval, 0)
                my_vertices(count + 1) = New Vector3D(my_MaxSize, -I * my_Interval, 0)
                count += 2
            Next
        End If
        If my_ShowXY_YDirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(I * my_Interval, -my_MaxSize, 0)
                my_vertices(count + 1) = New Vector3D(I * my_Interval, my_MaxSize, 0)
                count += 2
            Next
            my_vertices(count) = New Vector3D(0, 0, 0)
            my_vertices(count + 1) = New Vector3D(0, -my_MaxSize, 0)
            count += 2
            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(-I * my_Interval, -my_MaxSize, 0)
                my_vertices(count + 1) = New Vector3D(-I * my_Interval, my_MaxSize, 0)
                count += 2
            Next
        End If
        If my_ShowXZ_XDirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(-my_MaxSize, 0, I * my_Interval)
                my_vertices(count + 1) = New Vector3D(my_MaxSize, 0, I * my_Interval)
                count += 2
            Next
            my_vertices(count) = New Vector3D(0, 0, 0)
            my_vertices(count + 1) = New Vector3D(-my_MaxSize, 0, 0)
            count += 2

            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(-my_MaxSize, 0, -I * my_Interval)
                my_vertices(count + 1) = New Vector3D(my_MaxSize, 0, -I * my_Interval)
                count += 2
            Next
        End If
        If my_ShowXZ_ZDirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(I * my_Interval, 0, -my_MaxSize)
                my_vertices(count + 1) = New Vector3D(I * my_Interval, 0, my_MaxSize)
                count += 2
            Next
            my_vertices(count) = New Vector3D(0, 0, 0)
            my_vertices(count + 1) = New Vector3D(0, 0, -my_MaxSize)
            count += 2
            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(-I * my_Interval, 0, -my_MaxSize)
                my_vertices(count + 1) = New Vector3D(-I * my_Interval, 0, my_MaxSize)
                count += 2
            Next
        End If
        If my_ShowYZ_YDirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(0, -my_MaxSize, I * my_Interval)
                my_vertices(count + 1) = New Vector3D(0, my_MaxSize, I * my_Interval)
                count += 2
            Next
            my_vertices(count) = New Vector3D(0, 0, 0)
            my_vertices(count + 1) = New Vector3D(0, -my_MaxSize, 0)
            count += 2
            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(0, -my_MaxSize, -I * my_Interval)
                my_vertices(count + 1) = New Vector3D(0, my_MaxSize, -I * my_Interval)
                count += 2
            Next
        End If
        If my_ShowYZ_Zdirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(0, I * my_Interval, -my_MaxSize)
                my_vertices(count + 1) = New Vector3D(0, I * my_Interval, my_MaxSize)
                count += 2
            Next
            my_vertices(count) = New Vector3D(0, 0, 0)
            my_vertices(count + 1) = New Vector3D(0, 0, -my_MaxSize)
            count += 2
            For I As Integer = 1 To LinesPerGrid - 1
                my_vertices(count) = New Vector3D(0, -I * my_Interval, -my_MaxSize)
                my_vertices(count + 1) = New Vector3D(0, -I * my_Interval, my_MaxSize)
                count += 2
            Next
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
        If my_ShowXY_XDirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_colors(count) = my_XYColor
                my_colors(count + 1) = my_XYColor
                my_colors(count + 2) = my_XYColor
                my_colors(count + 3) = my_XYColor
                count += 4
            Next
            my_colors(count) = my_XYColor
            my_colors(count + 1) = my_XYColor
            count += 2
        End If
        If my_ShowXY_YDirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_colors(count) = my_XYColor
                my_colors(count + 1) = my_XYColor
                my_colors(count + 2) = my_XYColor
                my_colors(count + 3) = my_XYColor
                count += 4
            Next
            my_colors(count) = my_XYColor
            my_colors(count + 1) = my_XYColor
            count += 2
        End If
        If my_ShowXZ_XDirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_colors(count) = my_XZColor
                my_colors(count + 1) = my_XZColor
                my_colors(count + 2) = my_XZColor
                my_colors(count + 3) = my_XZColor
                count += 4
            Next
            my_colors(count) = my_XZColor
            my_colors(count + 1) = my_XZColor
            count += 2
        End If
        If my_ShowXZ_ZDirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_colors(count) = my_XZColor
                my_colors(count + 1) = my_XZColor
                my_colors(count + 2) = my_XZColor
                my_colors(count + 3) = my_XZColor
                count += 4
            Next
            my_colors(count) = my_XZColor
            my_colors(count + 1) = my_XZColor
            count += 2
        End If
        If my_ShowYZ_YDirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_colors(count) = my_YZColor
                my_colors(count + 1) = my_YZColor
                my_colors(count + 2) = my_YZColor
                my_colors(count + 3) = my_YZColor
                count += 4
            Next
            my_colors(count) = my_YZColor
            my_colors(count + 1) = my_YZColor
            count += 2
        End If
        If my_ShowYZ_Zdirection Then
            For I As Integer = 1 To LinesPerGrid - 1
                my_colors(count) = my_YZColor
                my_colors(count + 1) = my_YZColor
                my_colors(count + 2) = my_YZColor
                my_colors(count + 3) = my_YZColor
                count += 4
            Next
            my_colors(count) = my_YZColor
            my_colors(count + 1) = my_YZColor
            count += 2
        End If
    End Sub

    Protected Overrides Sub CreateTexCoordinates()
        ReDim my_textureCoords(my_VertexCount - 1)
        For I As Integer = 0 To my_VertexCount - 1
            my_textureCoords(I) = New Vector(0, 0)
        Next
    End Sub

    ''' <summary>
    ''' X , Y and Z = 2 vertices per line
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function GetVertexLayout() As Vector3D
        Throw New NotImplementedException()
    End Function

    Private Function GetVertexCount() As Integer
        Dim count As Integer = 0
        LinesPerGrid = CInt(my_MaxSize / my_Interval) + 1
        my_MaxSize = (LinesPerGrid - 1) * my_Interval
        If my_ShowXY_XDirection = True Then count += 4 * LinesPerGrid + 2
        If my_ShowXY_YDirection = True Then count += 4 * LinesPerGrid + 2
        If my_ShowXZ_XDirection = True Then count += 4 * LinesPerGrid + 2
        If my_ShowXZ_ZDirection = True Then count += 4 * LinesPerGrid + 2
        If my_ShowYZ_YDirection = True Then count += 4 * LinesPerGrid + 2
        If my_ShowYZ_Zdirection = True Then count += 4 * LinesPerGrid + 2
        Return count
    End Function
End Class

