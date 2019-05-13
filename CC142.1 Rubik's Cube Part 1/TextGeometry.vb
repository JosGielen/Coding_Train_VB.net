Imports System.Drawing
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports SharpGL.SceneGraph.Assets
Imports GlmNet

Public Class TextGeometry
    Inherits GLGeometry

    Private my_Text As String
    Private my_FontStyle As Windows.FontStyle
    Private my_FontWeight As FontWeight
    Private my_FontFamily As Media.FontFamily
    Private my_ForeColor As Media.Color
    Private my_Bitmap As Bitmap
    Private my_Width As Single
    Private my_Height As Single

    Public Sub New(text As String)
        my_Text = text
        my_FontFamily = New Media.FontFamily("Times New Roman")
        my_Width = 1.0F
        my_FontStyle = FontStyles.Normal
        my_FontWeight = FontWeights.Normal
        my_ForeColor = Media.Colors.Lime
        my_Bitmap = Text_to_Image(my_Text)
        my_texture = New Texture()
        my_textureFile = ""
        my_VertexCount = 4
    End Sub

    Public Sub New(text As String, width As Double)
        my_Text = text
        my_FontFamily = New Media.FontFamily("Times New Roman")
        my_Width = CSng(width)
        my_FontStyle = FontStyles.Normal
        my_FontWeight = FontWeights.Normal
        my_ForeColor = Media.Colors.Lime
        my_Bitmap = Text_to_Image(my_Text)
        my_texture = New Texture()
        my_textureFile = ""
        my_VertexCount = 4
    End Sub

    Public Sub New(text As String, fontFamily As Media.FontFamily, width As Double, fontStyle As Windows.FontStyle, fontWeight As FontWeight, color As Media.Color)
        my_Text = text
        my_FontFamily = fontFamily
        my_Width = CSng(width)
        my_FontStyle = fontStyle
        my_FontWeight = fontWeight
        my_ForeColor = color
        my_Bitmap = Text_to_Image(my_Text)
        my_texture = New Texture()
        my_textureFile = ""
        my_VertexCount = 4
    End Sub

    Public Function Text_to_Image(txt As String) As Bitmap
        'creating bitmap image
        Dim bmp As Bitmap = New Bitmap(1, 1)
        Try
            'FromImage method creates a New Graphics from the specified Image.
            Dim g As Graphics = Graphics.FromImage(bmp)
            'Create the Font object for the image text drawing.
            Dim f As Font
            If my_FontStyle = Windows.FontStyles.Italic Then
                f = New Font(my_FontFamily.ToString(), 140.0F, System.Drawing.FontStyle.Italic)
            ElseIf my_FontWeight = FontWeights.Bold Or my_FontWeight = FontWeights.ExtraBold Or my_FontWeight = FontWeights.Black Or my_FontWeight = FontWeights.Heavy Then
                f = New Font(my_FontFamily.ToString(), 140.0F, System.Drawing.FontStyle.Bold)
            Else
                f = New Font(my_FontFamily.ToString(), 140.0F, System.Drawing.FontStyle.Regular)
            End If
            'Instantiating object of Bitmap image again with the correct size for the text And font.
            Dim stringSize As SizeF = g.MeasureString(txt, f)
            my_Height = my_Width * stringSize.Height / stringSize.Width
            bmp = New Bitmap(bmp, stringSize.Width, stringSize.Height)
            g = Graphics.FromImage(bmp)
            'Draw Specified text with specified format
            Dim col As System.Drawing.Color = System.Drawing.Color.FromArgb(255, my_ForeColor.R, my_ForeColor.G, my_ForeColor.B)
            Dim br As System.Drawing.Brush = New System.Drawing.SolidBrush(col)
            g.DrawString(txt, f, br, 0, 0)
            bmp.RotateFlip(RotateFlipType.Rotate180FlipX)
            f.Dispose()
            g.Flush()
            g.Dispose()
        Catch ex As Exception
            'Leave the bitmap empty
        End Try
        Return bmp
    End Function

    Public Property FontFamily As Media.FontFamily
        Get
            Return my_FontFamily
        End Get
        Set(value As Media.FontFamily)
            my_FontFamily = value
        End Set
    End Property

    Public Property Width As Double
        Get
            Return my_Width
        End Get
        Set(value As Double)
            my_Width = value
        End Set
    End Property

    Public Property ForeColor As Media.Color
        Get
            Return my_ForeColor
        End Get
        Set(value As Media.Color)
            my_ForeColor = value
            my_Bitmap = Text_to_Image(my_Text)
        End Set
    End Property

    Public Property Text As String
        Get
            Return my_Text
        End Get
        Set(value As String)
            my_Text = value
            'Set the text as a Texture
            my_textureFile = ""
            my_Bitmap = Text_to_Image(my_Text)
            my_texture.Create(my_openGL, my_Bitmap)
            my_UseTexture = False 'Prevent using a blank texture
        End Set
    End Property

    Public Property FontStyle As Windows.FontStyle
        Get
            Return my_FontStyle
        End Get
        Set(value As Windows.FontStyle)
            my_FontStyle = value
        End Set
    End Property

    Public Property FontWeight As FontWeight
        Get
            Return my_FontWeight
        End Get
        Set(value As FontWeight)
            my_FontWeight = value
        End Set
    End Property

    Protected Overrides Function GetNormalMatrix() As mat3
        Return mat3.identity() 'Prevent shading effects
    End Function

    Protected Overrides Sub CreateVertices()
        my_textureFile = ""
        my_Bitmap = Text_to_Image(my_Text) 'Updates my_Height
        ReDim my_vertices(my_VertexCount - 1)
        my_vertices(0) = New Vector3D(-my_Width / 2, -my_Height / 2, 0)
        my_vertices(1) = New Vector3D(my_Width / 2, -my_Height / 2, 0)
        my_vertices(2) = New Vector3D(my_Width / 2, my_Height / 2, 0)
        my_vertices(3) = New Vector3D(-my_Width / 2, my_Height / 2, 0)
        'Force Fill mode
        GLBeginMode = SharpGL.Enumerations.BeginMode.Triangles
        'Set the text as a Texture
        my_texture.Create(my_openGL, my_Bitmap)
        my_UseTexture = False 'Prevent using a blank texture
    End Sub

    Protected Overrides Sub CreateNormals()
        ReDim my_normals(my_VertexCount - 1)
        For I As Integer = 0 To my_VertexCount - 1
            my_normals(I) = New Vector3D(0, 0, 1)
        Next
    End Sub

    Protected Overrides Sub CreateIndices()
        ReDim my_indices(5)
        my_indices(0) = 0
        my_indices(1) = 1
        my_indices(2) = 2
        my_indices(3) = 2
        my_indices(4) = 3
        my_indices(5) = 0
    End Sub

    Protected Overrides Sub CreateTexCoordinates()
        ReDim my_textureCoords(my_VertexCount - 1)
        my_textureCoords(0) = New Vector(0, 0)
        my_textureCoords(1) = New Vector(1, 0)
        my_textureCoords(2) = New Vector(1, 1)
        my_textureCoords(3) = New Vector(0, 1)
    End Sub

    ''' <summary>
    ''' The text is rendered on a rectangle: X = 2 vertices; Y = 2 vertices
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function GetVertexLayout() As Vector3D
        Return New Vector3D(2, 2, 0)
    End Function
End Class
