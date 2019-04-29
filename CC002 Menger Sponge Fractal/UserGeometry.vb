Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D

Public Class UserGeometry
    Inherits GLGeometry

    Private my_UserVertices As List(Of Vector3D)
    Private my_UserNormals As List(Of Vector3D)
    Private my_UserColors As List(Of Color)
    Private my_UserIndices As List(Of Integer)
    Private my_UserTexCoordinates As List(Of Vector)

    ''' <summary>
    ''' Creates a new UserGeometry.  Useage: Add the Vertices one by one or as a list(of Vector3D)
    ''' <para>Normals and texture coordinates should be added for each Vertex.</para>
    ''' <para>If no Indices are added they will be used per Vertex.</para>
    ''' <para>If no normals are added they are set by default as Vector(0,0,1).</para>
    ''' <para>If no texture coordinates are set they are calculated over XY, XZ and YZ planes.</para>
    ''' </summary>
    Public Sub New()
        my_UserVertices = New List(Of Vector3D)
        my_UserNormals = New List(Of Vector3D)
        my_UserColors = New List(Of Color)
        my_UserIndices = New List(Of Integer)
        my_UserTexCoordinates = New List(Of Vector)
    End Sub

    Public Property UserVertices As List(Of Vector3D)
        Get
            Return my_UserVertices
        End Get
        Set(value As List(Of Vector3D))
            my_UserVertices = value
        End Set
    End Property

    Public Property UserNormals As List(Of Vector3D)
        Get
            Return my_UserNormals
        End Get
        Set(value As List(Of Vector3D))
            my_UserNormals = value
        End Set
    End Property

    Public Property UserIndices As List(Of Integer)
        Get
            Return my_UserIndices
        End Get
        Set(value As List(Of Integer))
            my_UserIndices = value
        End Set
    End Property

    Public Property UserTexCoordinates As List(Of Vector)
        Get
            Return my_UserTexCoordinates
        End Get
        Set(value As List(Of Vector))
            my_UserTexCoordinates = value
        End Set
    End Property

    Public Property UserColors As List(Of Color)
        Get
            Return my_UserColors
        End Get
        Set(value As List(Of Color))
            my_UserColors = value
        End Set
    End Property

    Public Sub AddVertex(vertex As Vector3D)
        my_UserVertices.Add(vertex)
        my_VertexCount = my_UserVertices.Count
    End Sub

    Public Sub AddNormal(normal As Vector3D)
        my_UserNormals.Add(normal)
    End Sub

    Public Sub AddColor(color As Color)
        my_UserColors.Add(color)
    End Sub

    Public Sub AddIndex(index As Integer)
        my_UserIndices.Add(index)
    End Sub

    Public Sub AddTexCoordinate(coordinate As Vector)
        my_UserTexCoordinates.Add(coordinate)
    End Sub

    ''' <summary>
    ''' Add a Triangle by specifiying Vertices V1-V2-V3 is counter-clockwise direction.
    ''' Calculates the normal and adds that normal for each Vertex.
    ''' </summary>
    Public Sub AddTriangle(V1 As Vector3D, V2 As Vector3D, V3 As Vector3D)
        Dim D1 As Vector3D = V2 - V1
        Dim D2 As Vector3D = V3 - V1
        Dim N As Vector3D = Vector3D.CrossProduct(D1, D2)
        my_UserVertices.Add(V1)
        my_UserVertices.Add(V2)
        my_UserVertices.Add(V3)
        my_UserNormals.Add(N)
        my_UserNormals.Add(N)
        my_UserNormals.Add(N)
    End Sub

    Protected Overrides Sub CreateVertices()
        ReDim my_vertices(my_VertexCount - 1)
        my_vertices = my_UserVertices.ToArray()
    End Sub

    Protected Overrides Sub CreateNormals()
        If my_UserNormals.Count = 0 Then
            For I As Integer = 0 To my_UserVertices.Count
                my_UserNormals.Add(New Vector3D(0, 0, 1))
            Next
        End If
        ReDim my_normals(my_UserNormals.Count - 1)
        my_normals = my_UserNormals.ToArray()
    End Sub

    Protected Overrides Sub CreateIndices()
        If my_UserIndices.Count = 0 Then
            For I As Integer = 0 To my_UserVertices.Count - 1
                my_UserIndices.Add(I)
            Next
        End If
        ReDim my_indices(my_UserIndices.Count - 1)
        my_indices = my_UserIndices.ToArray()
    End Sub

    Protected Overrides Sub CreateTexCoordinates()
        If my_UserTexCoordinates.Count = 0 Then
            Dim my_XYIndices As List(Of Integer) = New List(Of Integer)
            Dim my_XZIndices As List(Of Integer) = New List(Of Integer)
            Dim my_YZIndices As List(Of Integer) = New List(Of Integer)
            Dim Xmin As Double
            Dim Xmax As Double
            Dim Ymin As Double
            Dim Ymax As Double
            Dim Zmin As Double
            Dim Zmax As Double
            'Calculate the Texture Coordinates for each vertex position by dividing the vertices into 3 planes of orientation
            For I As Integer = 0 To my_vertices.Count - 1
                If Math.Abs(my_normals(I).X) > Math.Abs(my_normals(I).Y) And Math.Abs(my_normals(I).X) > Math.Abs(my_normals(I).Z) Then 'YZ oriented
                    my_YZIndices.Add(I)
                ElseIf Math.Abs(my_normals(I).Y) > Math.Abs(my_normals(I).X) And Math.Abs(my_normals(I).Y) > Math.Abs(my_normals(I).Z) Then 'XZ oriented 
                    my_XZIndices.Add(I)
                Else  'XY oriented
                    my_XYIndices.Add(I)
                End If
            Next
            Ymin = Double.MaxValue
            Ymax = Double.MinValue
            Zmin = Double.MaxValue
            Zmax = Double.MinValue
            For Each I As Integer In my_YZIndices
                If my_vertices(I).Y < Ymin Then Ymin = my_vertices(I).Y
                If my_vertices(I).Y > Ymax Then Ymax = my_vertices(I).Y
                If my_vertices(I).Z < Zmin Then Zmin = my_vertices(I).Z
                If my_vertices(I).Z > Zmax Then Zmax = my_vertices(I).Z
            Next
            For Each I As Integer In my_YZIndices
                my_UserTexCoordinates.Add(New Vector(my_TextureScaleX * (my_vertices(I).Z - Zmin) / (Zmax - Zmin), my_TextureScaleY * (my_vertices(I).Y - Ymin) / (Ymax - Ymin)))
            Next
            Xmin = Double.MaxValue
            Xmax = Double.MinValue
            Zmin = Double.MaxValue
            Zmax = Double.MinValue
            For Each I As Integer In my_XZIndices
                If my_vertices(I).X < Xmin Then Xmin = my_vertices(I).X
                If my_vertices(I).X > Xmax Then Xmax = my_vertices(I).X
                If my_vertices(I).Z < Zmin Then Zmin = my_vertices(I).Z
                If my_vertices(I).Z > Zmax Then Zmax = my_vertices(I).Z
            Next
            For Each I As Integer In my_XZIndices
                my_UserTexCoordinates.Add(New Vector(my_TextureScaleX * (my_vertices(I).X - Xmin) / (Xmax - Xmin), my_TextureScaleY * (my_vertices(I).Z - Zmin) / (Zmax - Zmin)))
            Next
            Xmin = Double.MaxValue
            Xmax = Double.MinValue
            Ymin = Double.MaxValue
            Ymax = Double.MinValue
            For Each I As Integer In my_XYIndices
                If my_vertices(I).X < Xmin Then Xmin = my_vertices(I).X
                If my_vertices(I).X > Xmax Then Xmax = my_vertices(I).X
                If my_vertices(I).Y < Ymin Then Ymin = my_vertices(I).Y
                If my_vertices(I).Y > Ymax Then Ymax = my_vertices(I).Y
            Next
            For Each I As Integer In my_XYIndices
                my_UserTexCoordinates.Add(New Vector(my_TextureScaleX * (my_vertices(I).X - Xmin) / (Xmax - Xmin), my_TextureScaleY * (my_vertices(I).Y - Ymin) / (Ymax - Ymin)))
            Next
        End If
        ReDim my_textureCoords(my_UserTexCoordinates.Count - 1)
        my_textureCoords = my_UserTexCoordinates.ToArray()
    End Sub

    Public Overrides Function GetVertexLayout() As Vector3D
        Return New Vector3D(0, 0, 0)
    End Function

End Class
