Imports System.Windows.Media.Media3D
Imports GlmNet

Public Class Cubelet
    Inherits GLGeometry

    Private my_Size As Double
    Private my_Pos As Vector3D
    Private ReadOnly c(5) As Color   'The color of each Cubelet face.
    Private my_FaceNumbers(2) As Integer 'The index numbers of the cubeletfaces that determine the colors of the cubelet.
    Private my_ColorNumbers As String
    Private sideColors As List(Of Color) 'The color of each vertex.
    Private hiddenColor As Color
    Private ReadOnly Centerdistance As Double

    Public Sub New(position As Vector3D, size As Double, spacing As Double, cubeletfacenumbers As Integer())
        my_Pos = position
        my_Size = size
        hiddenColor = Color.FromRgb(130, 130, 130)
        my_FaceNumbers(0) = cubeletfacenumbers(0)
        my_FaceNumbers(1) = cubeletfacenumbers(1)
        my_FaceNumbers(2) = cubeletfacenumbers(2)
        sideColors = New List(Of Color)
        Centerdistance = size + spacing
        my_VertexCount = 24
        'Set the position of the cubelet
        my_Position = New vec3(Centerdistance * my_Pos.X, Centerdistance * my_Pos.Y, Centerdistance * my_Pos.Z)
        'Set the colors of the cubelet according to the cubeletfaces colors
        my_AmbientMaterial = Color.FromRgb(0, 0, 0)
        my_DiffuseMaterial = Color.FromRgb(0, 0, 0)
        my_SpecularMaterial = Color.FromRgb(255, 255, 255)
        my_Shininess = 60.0
        my_VertexColorIntensity = 0.8
        'Set the default color of each Cubelet face
        c(0) = hiddenColor  'Up
        c(1) = hiddenColor  'Down
        c(2) = hiddenColor  'Front
        c(3) = hiddenColor  'Back
        c(4) = hiddenColor  'Left
        c(5) = hiddenColor  'Right
    End Sub

    Public ReadOnly Property ColorNumbers As String
        Get
            Return my_ColorNumbers
        End Get
    End Property

    Public ReadOnly Property CubeletPosition As Vector3D
        Get
            Return my_Pos
        End Get
    End Property

    Public Sub ResetPosition()
        my_Position = New vec3(Centerdistance * my_Pos.X, Centerdistance * my_Pos.Y, Centerdistance * my_Pos.Z)
        my_Rotation = New vec3(0, 0, 0)
    End Sub

    Public Sub SetColors(cubeletfaces As List(Of CubeletFace))
        my_ColorNumbers = ""
        If my_Pos.Y = 1 Then
            c(0) = cubeletfaces(my_FaceNumbers(1)).FaceColor
            my_ColorNumbers &= cubeletfaces(my_FaceNumbers(1)).FaceColorNumber.ToString()
        Else
            c(0) = hiddenColor
        End If
        If my_Pos.Y = -1 Then
            c(1) = cubeletfaces(my_FaceNumbers(1)).FaceColor
            my_ColorNumbers &= cubeletfaces(my_FaceNumbers(1)).FaceColorNumber.ToString()
        Else
            c(1) = hiddenColor
        End If
        If my_Pos.Z = 1 Then
            c(2) = cubeletfaces(my_FaceNumbers(2)).FaceColor
            my_ColorNumbers &= cubeletfaces(my_FaceNumbers(2)).FaceColorNumber.ToString()
        Else
            c(2) = hiddenColor
        End If
        If my_Pos.Z = -1 Then
            c(3) = cubeletfaces(my_FaceNumbers(2)).FaceColor
            my_ColorNumbers &= cubeletfaces(my_FaceNumbers(2)).FaceColorNumber.ToString()
        Else
            c(3) = hiddenColor
        End If
        If my_Pos.X = 1 Then
            c(5) = cubeletfaces(my_FaceNumbers(0)).FaceColor
            my_ColorNumbers &= cubeletfaces(my_FaceNumbers(0)).FaceColorNumber.ToString()
        Else
            c(5) = hiddenColor
        End If
        If my_Pos.X = -1 Then
            c(4) = cubeletfaces(my_FaceNumbers(0)).FaceColor
            my_ColorNumbers &= cubeletfaces(my_FaceNumbers(0)).FaceColorNumber.ToString()
        Else
            c(4) = hiddenColor
        End If
        'Set the color of each vertex of the cubelet
        sideColors.Clear()
        For I As Integer = 0 To 5 '6 faces
            For J As Integer = 0 To 3 '4 vertices per face
                sideColors.Add(c(I))
            Next
        Next
        SetVertexColors(sideColors)
    End Sub

#Region "GLGeometry Overrides"

    Public Overrides Function GetVertexLayout() As Vector3D
        Return New Vector3D(4, 0, 6)
    End Function

    Protected Overrides Sub CreateVertices()
        ReDim my_vertices(23)
        'Calculate the vertex positions
        Dim x As Double = 0.5 * my_Size
        Dim y As Double = 0.5 * my_Size
        Dim z As Double = 0.5 * my_Size
        'UP surface 
        my_vertices(0) = New Vector3D(x, y, z)
        my_vertices(1) = New Vector3D(x, y, -z)
        my_vertices(2) = New Vector3D(-x, y, -z)
        my_vertices(3) = New Vector3D(-x, y, z)
        'DOWN surface 
        my_vertices(4) = New Vector3D(-x, -y, z)
        my_vertices(5) = New Vector3D(-x, -y, -z)
        my_vertices(6) = New Vector3D(x, -y, -z)
        my_vertices(7) = New Vector3D(x, -y, z)
        'FRONT surface 
        my_vertices(8) = New Vector3D(x, y, z)
        my_vertices(9) = New Vector3D(-x, y, z)
        my_vertices(10) = New Vector3D(-x, -y, z)
        my_vertices(11) = New Vector3D(x, -y, z)
        'BACK surface 
        my_vertices(12) = New Vector3D(x, y, -z)
        my_vertices(13) = New Vector3D(x, -y, -z)
        my_vertices(14) = New Vector3D(-x, -y, -z)
        my_vertices(15) = New Vector3D(-x, y, -z)
        'LEFT surface 
        my_vertices(16) = New Vector3D(-x, y, -z)
        my_vertices(17) = New Vector3D(-x, -y, -z)
        my_vertices(18) = New Vector3D(-x, -y, z)
        my_vertices(19) = New Vector3D(-x, y, z)
        'RIGHT surface
        my_vertices(20) = New Vector3D(x, y, z)
        my_vertices(21) = New Vector3D(x, -y, z)
        my_vertices(22) = New Vector3D(x, -y, -z)
        my_vertices(23) = New Vector3D(x, y, -z)
    End Sub

    Protected Overrides Sub CreateNormals()
        ReDim my_normals(23)
        'Calculate the normals for each vertex position
        'UP surface 
        my_normals(0) = New Vector3D(0, 1, 0)
        my_normals(1) = New Vector3D(0, 1, 0)
        my_normals(2) = New Vector3D(0, 1, 0)
        my_normals(3) = New Vector3D(0, 1, 0)
        'DOWN surface 
        my_normals(4) = New Vector3D(0, -1, 0)
        my_normals(5) = New Vector3D(0, -1, 0)
        my_normals(6) = New Vector3D(0, -1, 0)
        my_normals(7) = New Vector3D(0, -1, 0)
        'FRONT surface 
        my_normals(8) = New Vector3D(0, 0, 1)
        my_normals(9) = New Vector3D(0, 0, 1)
        my_normals(10) = New Vector3D(0, 0, 1)
        my_normals(11) = New Vector3D(0, 0, 1)
        'BACK surface 
        my_normals(12) = New Vector3D(0, 0, -1)
        my_normals(13) = New Vector3D(0, 0, -1)
        my_normals(14) = New Vector3D(0, 0, -1)
        my_normals(15) = New Vector3D(0, 0, -1)
        'LEFT surface 
        my_normals(16) = New Vector3D(-1, 0, 0)
        my_normals(17) = New Vector3D(-1, 0, 0)
        my_normals(18) = New Vector3D(-1, 0, 0)
        my_normals(19) = New Vector3D(-1, 0, 0)
        'RIGHT surface
        my_normals(20) = New Vector3D(1, 0, 0)
        my_normals(21) = New Vector3D(1, 0, 0)
        my_normals(22) = New Vector3D(1, 0, 0)
        my_normals(23) = New Vector3D(1, 0, 0)
    End Sub

    Protected Overrides Sub CreateIndices()
        ReDim my_indices(35)
        'Calculate the Indices for each box surface
        'UP surface 
        my_indices(0) = 0
        my_indices(1) = 1
        my_indices(2) = 2
        my_indices(3) = 2
        my_indices(4) = 3
        my_indices(5) = 0
        'DOWN surface
        my_indices(6) = 4
        my_indices(7) = 5
        my_indices(8) = 6
        my_indices(9) = 6
        my_indices(10) = 7
        my_indices(11) = 4
        'FRONT surface 
        my_indices(12) = 8
        my_indices(13) = 9
        my_indices(14) = 10
        my_indices(15) = 10
        my_indices(16) = 11
        my_indices(17) = 8
        'BACK surface
        my_indices(18) = 12
        my_indices(19) = 13
        my_indices(20) = 14
        my_indices(21) = 14
        my_indices(22) = 15
        my_indices(23) = 12
        'LEFT surface
        my_indices(24) = 16
        my_indices(25) = 17
        my_indices(26) = 18
        my_indices(27) = 18
        my_indices(28) = 19
        my_indices(29) = 16
        'RIGHT surface
        my_indices(30) = 20
        my_indices(31) = 21
        my_indices(32) = 22
        my_indices(33) = 22
        my_indices(34) = 23
        my_indices(35) = 20
    End Sub

    Protected Overrides Sub CreateTexCoordinates()
        ReDim my_textureCoords(23)
        'Calculate the Texture Coordinates for each vertex position
        'UP surface 
        my_textureCoords(0) = New Vector(my_TextureScaleX, 0)
        my_textureCoords(1) = New Vector(my_TextureScaleX, my_TextureScaleY)
        my_textureCoords(2) = New Vector(0, my_TextureScaleY)
        my_textureCoords(3) = New Vector(0, 0)
        'DOWN surface 
        my_textureCoords(4) = New Vector(0, my_TextureScaleY)
        my_textureCoords(5) = New Vector(0, 0)
        my_textureCoords(6) = New Vector(my_TextureScaleX, 0)
        my_textureCoords(7) = New Vector(my_TextureScaleX, my_TextureScaleY)
        'FRONT surface 
        my_textureCoords(8) = New Vector(my_TextureScaleX, my_TextureScaleY)
        my_textureCoords(9) = New Vector(0, my_TextureScaleY)
        my_textureCoords(10) = New Vector(0, 0)
        my_textureCoords(11) = New Vector(my_TextureScaleX, 0)
        'BACK surface 
        my_textureCoords(12) = New Vector(0, my_TextureScaleY)
        my_textureCoords(13) = New Vector(0, 0)
        my_textureCoords(14) = New Vector(my_TextureScaleX, 0)
        my_textureCoords(15) = New Vector(my_TextureScaleX, my_TextureScaleY)
        'LEFT surface 
        my_textureCoords(16) = New Vector(0, my_TextureScaleY)
        my_textureCoords(17) = New Vector(0, 0)
        my_textureCoords(18) = New Vector(my_TextureScaleX, 0)
        my_textureCoords(19) = New Vector(my_TextureScaleX, my_TextureScaleY)
        'RIGHT surface
        my_textureCoords(20) = New Vector(0, my_TextureScaleY)
        my_textureCoords(21) = New Vector(0, 0)
        my_textureCoords(22) = New Vector(my_TextureScaleX, 0)
        my_textureCoords(23) = New Vector(my_TextureScaleX, my_TextureScaleY)
    End Sub

#End Region


End Class
