Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Media3D
Imports SharpGL
Imports GlmNet
Imports SharpGL.Shaders
Imports SharpGL.VertexBuffers
Imports SharpGL.Enumerations
Imports SharpGL.SceneGraph.Assets
Imports System.Runtime.InteropServices

Public MustInherit Class GLGeometry

    Protected my_openGL As OpenGL
    Protected my_VertexCount As Integer = 0
    Protected my_vertices() As Vector3D
    Protected my_normals() As Vector3D
    Protected my_colors() As Color
    Protected my_indices() As Integer
    Protected my_textureCoords() As Vector
    Protected my_textureFile As String = ""
    Protected my_texture As Texture
    Protected my_vertexBufferArray As VertexBufferArray
    Private vertexBuffer As VertexBuffer
    Private normalBuffer As VertexBuffer
    Private colorBuffer As VertexBuffer
    Private textureBuffer As VertexBuffer
    Private indexBuffer As IndexBuffer
    Protected my_modelMatrix As mat4 = mat4.identity()
    Protected my_viewMatrix As mat4 = mat4.identity()
    Protected my_normalMatrix As mat3 = mat3.identity()
    Private my_Position As vec3 = New vec3(0, 0, 0)
    Private my_Direction As vec3 = New vec3(0, 0, 0)
    Private my_Speed As Double = 0.0
    Private my_RotationAxis As vec3 = New vec3(0, 1, 0)
    Private my_RotationAngle As Single = 0
    Private my_RotationSpeed As Single = 0
    Private my_InitialRotationAxis As Vector3D = New Vector3D(0, 0, 0)
    Private my_DiffuseMaterial As Color = New Color()
    Private my_AmbientMaterial As Color = New Color()
    Private my_SpecularMaterial As Color = New Color()
    Private my_Shininess As Single = 100.0!
    Private my_UseVertexColors As Boolean = False
    Private my_VertexColorIntensity As Double = 0.8
    Private my_DrawMode As DrawMode = DrawMode.Fill
    Protected GLBeginMode As BeginMode = BeginMode.Triangles
    Private my_LineWidth As Single = 1.0!
    Private my_PointSize As Single = 1.0!
    Protected my_TextureScaleX As Double = 1.0
    Protected my_TextureScaleY As Double = 1.0

#Region "Properties"

    ''' <summary>
    ''' Initial position of the center of the geometry.
    ''' </summary>
    ''' <returns></returns>
    Public Property Position As Vector3D
        Get
            Return New Vector3D(my_Position.x, my_Position.y, my_Position.z)
        End Get
        Set(value As Vector3D)
            my_Position = New vec3(CSng(value.X), CSng(value.Y), CSng(value.Z))
        End Set
    End Property

    ''' <summary>
    ''' Initial Rotation of the geometry. Specify in degrees around axis X, Y and Z.
    ''' </summary>
    ''' <returns></returns>
    Public Property InitialRotationAxis As Vector3D
        Get
            Return my_InitialRotationAxis
        End Get
        Set(value As Vector3D)
            my_InitialRotationAxis = value
        End Set
    End Property

    ''' <summary>
    ''' Angle of rotation animation around the RotationAxis
    ''' </summary>
    ''' <returns></returns>
    Public Property RotationAngle As Double
        Get
            Return my_RotationAngle
        End Get
        Set(value As Double)
            my_RotationAngle = CSng(value)
        End Set
    End Property

    ''' <summary>
    ''' Variation of the Rotation angle per frame.
    ''' </summary>
    ''' <returns></returns>
    Public Property RotationSpeed As Double
        Get
            Return my_RotationSpeed
        End Get
        Set(value As Double)
            my_RotationSpeed = CSng(value)
        End Set
    End Property

    ''' <summary>
    ''' Axis of Rotation specify in degrees around axis X, Y and Z.
    ''' </summary>
    ''' <returns></returns>
    Public Property RotationAxis As Vector3D
        Get
            Return New Vector3D(my_RotationAxis.x, my_RotationAxis.y, my_RotationAxis.z)
        End Get
        Set(value As Vector3D)
            my_RotationAxis = New vec3(CSng(value.X), CSng(value.Y), CSng(value.Z))
        End Set
    End Property

    Public ReadOnly Property Vertices As Vector3D()
        Get
            Return my_vertices
        End Get
    End Property

    Public ReadOnly Property Normals As Vector3D()
        Get
            Return my_normals
        End Get
    End Property

    Public ReadOnly Property Colors As Color()
        Get
            Return my_colors
        End Get
    End Property

    Public ReadOnly Property Indices As Integer()
        Get
            Return my_indices
        End Get
    End Property

    Public ReadOnly Property TextureCoordinates As Vector()
        Get
            Return my_textureCoords
        End Get
    End Property

    Public Property DiffuseMaterial As Color
        Get
            Return my_DiffuseMaterial
        End Get
        Set(value As Color)
            my_DiffuseMaterial = value
        End Set
    End Property

    Public Property AmbientMaterial As Color
        Get
            Return my_AmbientMaterial
        End Get
        Set(value As Color)
            my_AmbientMaterial = value
        End Set
    End Property

    Public Property SpecularMaterial As Color
        Get
            Return my_SpecularMaterial
        End Get
        Set(value As Color)
            my_SpecularMaterial = value
        End Set
    End Property

    Public Property Shininess As Double
        Get
            Return my_Shininess
        End Get
        Set(value As Double)
            my_Shininess = CSng(value)
        End Set
    End Property

    Public Property TextureFile As String
        Get
            Return my_textureFile
        End Get
        Set(value As String)
            my_textureFile = value
        End Set
    End Property

    Public Property DrawMode As DrawMode
        Get
            Return my_DrawMode
        End Get
        Set(value As DrawMode)
            my_DrawMode = value
        End Set
    End Property

    Public Property LineWidth As Double
        Get
            Return my_LineWidth
        End Get
        Set(value As Double)
            my_LineWidth = CSng(value)
        End Set
    End Property

    Public Property PointSize As Double
        Get
            Return my_PointSize
        End Get
        Set(value As Double)
            my_PointSize = CSng(value)
        End Set
    End Property

    Public Property TextureScaleX As Double
        Get
            Return my_TextureScaleX
        End Get
        Set(value As Double)
            my_TextureScaleX = value
        End Set
    End Property

    Public Property TextureScaleY As Double
        Get
            Return my_TextureScaleY
        End Get
        Set(value As Double)
            my_TextureScaleY = value
        End Set
    End Property

    Public ReadOnly Property VertexCount As Integer
        Get
            Return my_VertexCount
        End Get
    End Property

    Public Property Direction As Vector3D
        Get
            Return New Vector3D(my_Direction.x, my_Direction.y, my_Direction.z)
        End Get
        Set(value As Vector3D)
            my_Direction = New vec3(CSng(value.X), CSng(value.Y), CSng(value.Z))
        End Set
    End Property

    Public Property Speed As Double
        Get
            Return my_Speed
        End Get
        Set(value As Double)
            my_Speed = value
        End Set
    End Property

    Public Property VertexColorIntensity As Double
        Get
            Return my_VertexColorIntensity
        End Get
        Set(value As Double)
            my_VertexColorIntensity = value
        End Set
    End Property

#End Region

    ''' <summary>
    ''' This creates a VertexBufferArray object That holds the state of all of the
    ''' vertex buffer operations.
    ''' </summary>
    Public Sub GenerateGeometry(scene As GLScene)
        my_openGL = scene.GL
        my_vertexBufferArray = New VertexBufferArray()
        my_vertexBufferArray.Create(my_openGL)
        my_vertexBufferArray.Bind(my_openGL)
        CreateVertexBuffer(GLScene.positionAttribute)
        CreateNormalBuffer(GLScene.normalAttribute)
        CreateColorBuffer(GLScene.colorAttribute)
        CreateTextureCoordBuffer(GLScene.textureAttribute)
        CreateIndexBuffer()
        my_vertexBufferArray.Unbind(my_openGL)
        my_texture = New Texture() With {.Name = ""}
        If my_textureFile <> "" Then
            Try
                my_texture.Create(my_openGL, my_textureFile)
                my_texture.Name = my_textureFile
            Catch ex As Exception
                MessageBox.Show("Unable to create Texture from File:" & my_textureFile, "GeometryGL error", MessageBoxButton.OK, MessageBoxImage.Error)
                my_texture = New Texture() With {.Name = ""}
            End Try
        End If
    End Sub

    Public Sub Update()
        'Move the geometry object.
        my_Position = my_Position + CSng(my_Speed) * my_Direction
        'Rotate the geometry object.
        my_RotationAngle += my_RotationSpeed
    End Sub

    ''' <summary>
    ''' Draws the geometry by using Vertex and Fragment Shaders
    ''' </summary>
    ''' <param name="shader">A compiled and linked ShaderProgram</param>
    Public Sub Draw(shader As ShaderProgram)
        my_openGL.LineWidth(my_LineWidth)
        my_openGL.PointSize(my_PointSize)
        shader.SetUniform3(my_openGL, "material.diffuse", my_DiffuseMaterial.ScR, my_DiffuseMaterial.ScG, my_DiffuseMaterial.ScB)
        shader.SetUniform3(my_openGL, "material.ambient", my_AmbientMaterial.ScR, my_AmbientMaterial.ScG, my_AmbientMaterial.ScB)
        shader.SetUniform3(my_openGL, "material.specular", my_SpecularMaterial.ScR, my_SpecularMaterial.ScG, my_SpecularMaterial.ScB)
        shader.SetUniform1(my_openGL, "material.shininess", my_Shininess)
        shader.SetUniformMatrix4(my_openGL, "Model", GetModelMatrix.to_array())
        shader.SetUniformMatrix3(my_openGL, "NormalMatrix", GetNormalMatrix.to_array())
        my_texture.Bind(my_openGL)
        my_vertexBufferArray.Bind(my_openGL)
        Select Case my_DrawMode
            Case DrawMode.Fill
                my_openGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled)
            Case DrawMode.Lines
                my_openGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines)
            Case DrawMode.Points
                my_openGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Points)
        End Select
        Select Case GLBeginMode
            Case BeginMode.Triangles
                my_openGL.DrawElements(OpenGL.GL_TRIANGLES, my_indices.Length, OpenGL.GL_UNSIGNED_INT, IntPtr.Zero)
            Case BeginMode.Lines
                my_openGL.DrawElements(OpenGL.GL_LINES, my_indices.Length, OpenGL.GL_UNSIGNED_INT, IntPtr.Zero)
            Case BeginMode.Points
                my_openGL.DrawElements(OpenGL.GL_POINTS, my_indices.Length, OpenGL.GL_UNSIGNED_INT, IntPtr.Zero)
        End Select
        my_vertexBufferArray.Unbind(my_openGL)
    End Sub

    ''' <summary>
    ''' Calculate a Matrix that performs the rotation around the X, Y and Z axes.
    ''' </summary>
    ''' <param name="x">Rotation around the X-axis in degrees</param>
    ''' <param name="y">Rotation around the Y-axis in degrees></param>
    ''' <param name="z">>Rotation around the Z-axis in degrees</param>
    ''' <returns></returns>
    Public Shared Function CalculateRotationMatrix(x As Double, y As Double, z As Double) As Matrix3D
        Dim matrix As Matrix3D = New Matrix3D()
        matrix.Rotate(New Quaternion(New Vector3D(1, 0, 0), x))
        matrix.Rotate(New Quaternion(New Vector3D(0, 1, 0) * matrix, y))
        matrix.Rotate(New Quaternion(New Vector3D(0, 0, 1) * matrix, z))
        Return matrix
    End Function

    Private Sub CreateVertexBuffer(vertexAttributeLocation As UInteger)
        CreateVertices()
        vertexBuffer = New VertexBuffer()
        vertexBuffer.Create(my_openGL)
        vertexBuffer.Bind(my_openGL)
        Dim vData As List(Of Single) = New List(Of Single)
        For Each v As Vector3D In my_vertices
            vData.Add(CSng(v.X))
            vData.Add(CSng(v.Y))
            vData.Add(CSng(v.Z))
        Next
        vertexBuffer.SetData(my_openGL, vertexAttributeLocation, vData.ToArray(), False, 3)
    End Sub

    Private Sub CreateNormalBuffer(normalAttributeLocation As UInteger)
        CreateNormals()
        normalBuffer = New VertexBuffer()
        normalBuffer.Create(my_openGL)
        normalBuffer.Bind(my_openGL)
        Dim nData As List(Of Single) = New List(Of Single)
        For Each n As Vector3D In my_normals
            nData.Add(CSng(n.X))
            nData.Add(CSng(n.Y))
            nData.Add(CSng(n.Z))
        Next
        normalBuffer.SetData(my_openGL, normalAttributeLocation, nData.ToArray(), False, 3)
    End Sub

    Private Sub CreateColorBuffer(ColorAttributeLocation As UInteger)
        CreateColors()
        colorBuffer = New VertexBuffer()
        colorBuffer.Create(my_openGL)
        colorBuffer.Bind(my_openGL)
        Dim cData As List(Of Single) = New List(Of Single)
        For Each col As Color In my_colors
            cData.Add(col.ScR)
            cData.Add(col.ScG)
            cData.Add(col.ScB)
        Next
        colorBuffer.SetData(my_openGL, ColorAttributeLocation, cData.ToArray(), True, 3)
    End Sub

    Private Sub CreateTextureCoordBuffer(TextureAttributeLocation As UInteger)
        CreateTexCoordinates()
        textureBuffer = New VertexBuffer()
        textureBuffer.Create(my_openGL)
        textureBuffer.Bind(my_openGL)
        Dim tData As List(Of Single) = New List(Of Single)
        For Each t As Vector In my_textureCoords
            tData.Add(CSng(t.X))
            tData.Add(CSng(t.Y))
        Next
        textureBuffer.SetData(my_openGL, TextureAttributeLocation, tData.ToArray(), True, 2)
    End Sub

    Private Sub CreateIndexBuffer()
        CreateIndices()
        indexBuffer = New IndexBuffer()
        indexBuffer.Create(my_openGL)
        indexBuffer.Bind(my_openGL)
        Dim indis(my_indices.Count - 1) As UInteger
        For i As Integer = 0 To my_indices.Count - 1
            indis(i) = CType(my_indices(i), UInteger)
        Next
        'SetData(indexBuffer, my_openGL, indis)

        indexBuffer.SetData(my_openGL, indis)

    End Sub

    Public Sub SetData(buffer As IndexBuffer, gl As OpenGL, rawData() As UInteger)
        'Get handle to raw data.
        Dim hData As GCHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned)
        Dim Size As Integer = Marshal.SizeOf(GetType(UInteger)) * rawData.Length
        gl.BufferData(OpenGL.GL_ELEMENT_ARRAY_BUFFER, Size, hData.AddrOfPinnedObject(), OpenGL.GL_STATIC_DRAW)
        'Free handle.
        hData.Free()
    End Sub

    Protected Overridable Function GetModelMatrix() As mat4
        Dim rotation As mat4 = glm.rotate(mat4.identity(), my_RotationAngle, my_RotationAxis)
        Dim translation As mat4 = glm.translate(mat4.identity(), my_Position)
        my_modelMatrix = rotation * translation
        Return my_modelMatrix
    End Function

    Protected Overridable Function GetNormalMatrix() As mat3
        Return GetModelMatrix.to_mat3()
    End Function

    ''' <summary>
    ''' Specify the color of each Vertex.
    ''' If number of colors in ColorList is less than the number of Vertices the colors will wrap.
    ''' </summary>
    ''' <param name="ColorList"></param>
    Public Sub SetVertexColors(ColorList As List(Of Color))
        If ColorList.Count > 0 Then
            my_UseVertexColors = True
            ReDim my_colors(VertexCount - 1)
            Dim index As Integer = 0
            For I As Integer = 0 To VertexCount - 1
                index = I Mod ColorList.Count
                my_colors(I) = ColorList(index)
            Next
        End If
    End Sub

    Protected Overridable Sub CreateColors()
        If my_UseVertexColors Then
            For I As Integer = 0 To my_colors.Count - 1
                my_colors(I) = my_colors(I) * CSng(my_VertexColorIntensity)
                my_colors(I).A = 255
            Next
        Else
            ReDim my_colors(my_vertices.Count - 1)
            For I As Integer = 0 To my_vertices.Count - 1
                my_colors(I) = Color.FromRgb(0, 0, 0)
            Next
        End If
    End Sub

    ''' <summary>
    ''' Returns the number of vertices in X, Y and Z directions.
    ''' This allows to set the color per vertex or per vertex group in X, Y and Z axis.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetVertexLayout() As Vector3D

    Protected MustOverride Sub CreateVertices()

    Protected MustOverride Sub CreateNormals()

    Protected MustOverride Sub CreateIndices()

    Protected MustOverride Sub CreateTexCoordinates()

End Class

Public Enum DrawMode As Integer
    Points = 0
    Lines = 1
    Fill = 2
End Enum

Public Enum GLDrawMode As Integer
    Points = 0
    Lines = 1
    Triangles = 2
End Enum

