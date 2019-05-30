Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private settingForm As Settings
    Private App_Started As Boolean = False
    Private CellSize As Double = 15
    Private ForceMag As Double = 0.3
    Private XYChange As Double = 0.1
    Private ZChange As Double = 0.001
    Private ParticleCount As Integer = 10000
    Private maxSpeed As Double = 1.0
    Private TrailLength As Byte = 75
    Private RandomSpawn As Boolean = True
    Private UseColor As Boolean = False
    Private cols As Integer
    Private rows As Integer
    Private FlowField(,) As Vector
    Private Particles As List(Of Particle)
    Private Writebitmap As WriteableBitmap
    Private PixelData As Byte()
    Private Stride As Integer
    Private colorCount As Integer = 360
    Private my_Colors As List(Of Color)
    Private ZOff As Double = 0.0
    Private Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim pal As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Rainbow continuous.cpl")
        my_Colors = pal.GetColors(colorCount)
        Init()
        ShowSettingForm()
    End Sub

    Private Sub Init()
        'Resize the Flowfield
        cols = CInt(canvas1.ActualWidth / CellSize)
        rows = CInt(canvas1.ActualHeight / CellSize)
        ReDim FlowField(rows, cols)
        'Create the Particles
        Particles = New List(Of Particle)
        Dim p As Particle
        For I As Integer = 0 To ParticleCount - 1
            p = New Particle(New Vector((canvas1.ActualWidth) * Rnd.NextDouble(), (canvas1.ActualHeight) * Rnd.NextDouble()), New Vector(), maxSpeed)
            Particles.Add(p)
        Next
        'Resize the Image Control
        Dim w As Integer = CInt(canvas1.ActualWidth) + 2
        Dim h As Integer = CInt(canvas1.ActualHeight) + 2
        Image1.Width = w
        Image1.Height = h
        Image1.Stretch = Stretch.Fill
        'Make a writeable bitmap the size of the Image control
        Writebitmap = New WriteableBitmap(w + 1, h + 1, 96, 96, PixelFormats.Bgra32, Nothing)
        Stride = CInt(Writebitmap.PixelWidth * Writebitmap.Format.BitsPerPixel / 8)
        ReDim PixelData(Stride * Writebitmap.PixelHeight)
    End Sub

    Private Sub ShowSettingForm()
        If settingForm Is Nothing Then
            settingForm = New Settings(Me)
            settingForm.Show()
            settingForm.Left = Me.Left + Me.Width - 15
            settingForm.Top = Me.Top
            settingForm.CellSize = CellSize
            settingForm.MaxForce = ForceMag
            settingForm.XYChange = XYChange
            settingForm.ZChange = ZChange
            settingForm.ParticleCount = ParticleCount
            settingForm.Speed = maxSpeed
            settingForm.TrailLength = TrailLength
            settingForm.RandomSpawn = RandomSpawn
            settingForm.UseColor = UseColor
        Else
            settingForm.Show()
        End If
        settingForm.Update()
    End Sub

    Public Sub GetParameters()
        If settingForm IsNot Nothing Then
            CellSize = settingForm.CellSize
            ForceMag = settingForm.MaxForce
            XYChange = settingForm.XYChange
            ZChange = settingForm.ZChange
            ParticleCount = settingForm.ParticleCount
            maxSpeed = settingForm.Speed
            TrailLength = settingForm.TrailLength
            RandomSpawn = settingForm.RandomSpawn
            UseColor = settingForm.UseColor
        End If
    End Sub

    Public Sub Start()
        GetParameters()
        Init()
        If Not App_Started Then
            App_Started = True
            Do While App_Started
                ZOff += ZChange
                DrawParticles()
                Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), 10)
            Loop
        End If
    End Sub

    Private Sub DrawParticles()
        Dim xOff As Double = 0
        Dim yOff As Double = 0
        Dim row As Integer
        Dim col As Integer
        Dim angle As Double
        Dim V As Vector
        Dim XN As Double
        Dim YN As Double
        Dim pos As Vector
        Dim vel As Vector
        'Calculate the FlowField
        For row = 0 To rows
            xOff = 0.0
            For col = 0 To cols
                angle = 4 * Math.PI * PerlinNoise.Noise3D(xOff, yOff, ZOff)
                V = New Vector(Math.Cos(angle), Math.Sin(angle))
                V = ForceMag * V
                FlowField(row, col) = V
                xOff += XYChange
            Next
            yOff += XYChange
        Next
        'Fade the pixels back to white
        Dim Fadestep As Byte = 1
        If UseColor Then
            Fadestep = CByte(255 / TrailLength)
        End If
        For X As Integer = 0 To Writebitmap.PixelWidth - 1
            For Y As Integer = 0 To Writebitmap.PixelHeight - 1
                FadePixel(X, Y, PixelData, Stride, Fadestep)
            Next
        Next
        'Draw the particles
        For I As Integer = 0 To Particles.Count - 1
            row = CInt(Particles(I).Position.Y / CellSize)
            col = CInt(Particles(I).Position.X / CellSize)
            V = FlowField(row, col)
            Particles(I).ApplyForce(V)
            Particles(I).Update()
            'When the particles leave the field:
            If RandomSpawn Then 'Reset at random position 
                XN = canvas1.ActualWidth * Rnd.NextDouble()
                YN = canvas1.ActualHeight * Rnd.NextDouble()
                row = CInt(YN / CellSize)
                col = CInt(XN / CellSize)
                pos = New Vector(XN, YN)
                vel = FlowField(row, col)
                If Particles(I).Position.X < 0 Then
                    Particles(I).Position = pos
                    Particles(I).Velocity = vel
                ElseIf Particles(I).Position.X > canvas1.ActualWidth Then
                    Particles(I).Position = pos
                    Particles(I).Velocity = vel
                End If
                If Particles(I).Position.Y < 0 Then
                    Particles(I).Position = pos
                    Particles(I).Velocity = vel
                ElseIf Particles(I).Position.Y > canvas1.ActualHeight Then
                    Particles(I).Position = pos
                    Particles(I).Velocity = vel
                End If
            Else 'Wrap to the opposite side 
                If Particles(I).Position.X < 0 Then
                    Particles(I).Position = New Vector(canvas1.ActualWidth, Particles(I).Position.Y)
                ElseIf Particles(I).Position.X > canvas1.ActualWidth Then
                    Particles(I).Position = New Vector(0, Particles(I).Position.Y)
                End If
                If Particles(I).Position.Y < 0 Then
                    Particles(I).Position = New Vector(Particles(I).Position.X, canvas1.ActualHeight)
                ElseIf Particles(I).Position.Y > canvas1.ActualHeight Then
                    Particles(I).Position = New Vector(Particles(I).Position.X, 0)
                End If
            End If
            If UseColor Then
                Dim index As Integer = CInt(Vector.AngleBetween(Particles(I).Velocity, New Vector(1, 0)) + 180) Mod colorCount
                SetPixel(CInt(Particles(I).Position.X), CInt(Particles(I).Position.Y), my_Colors(index), PixelData, Stride)
            Else
                SetPixelBlend(CInt(Particles(I).Position.X), CInt(Particles(I).Position.Y), Color.FromArgb(TrailLength, 0, 0, 0), PixelData, Stride)
            End If
        Next
        'Update the Image
        Dim Intrect = New Int32Rect(0, 0, Writebitmap.PixelWidth - 1, Writebitmap.PixelHeight - 1)
        Writebitmap.WritePixels(Intrect, PixelData, Stride, 0)
        Image1.Source = Writebitmap
    End Sub

    Private Sub SetPixel(ByVal X As Integer, ByVal Y As Integer, ByVal c As Color, ByVal buffer As Byte(), ByVal PixStride As Integer)
        'SetPixel allowing for Alpha and color blending
        Dim xIndex As Integer = X * 4
        Dim yIndex As Integer = Y * PixStride
        buffer(xIndex + yIndex) = c.B
        buffer(xIndex + yIndex + 1) = c.G
        buffer(xIndex + yIndex + 2) = c.R
        buffer(xIndex + yIndex + 3) = c.A
    End Sub

    Private Sub SetPixelBlend(ByVal X As Integer, ByVal Y As Integer, ByVal c As Color, ByVal buffer As Byte(), ByVal PixStride As Integer)
        'SetPixel allowing for Alpha and color blending
        Dim xIndex As Integer = X * 4
        Dim yIndex As Integer = Y * PixStride
        Dim B As Integer = CInt(buffer(xIndex + yIndex)) + c.B
        Dim G As Integer = CInt(buffer(xIndex + yIndex + 1)) + c.G
        Dim R As Integer = CInt(buffer(xIndex + yIndex + 2)) + c.R
        Dim A As Integer = CInt(buffer(xIndex + yIndex + 3)) + c.A
        If B > 255 Then B = 255
        If G > 255 Then G = 255
        If R > 255 Then R = 255
        If A > 255 Then A = 255
        buffer(xIndex + yIndex) = CByte(B)
        buffer(xIndex + yIndex + 1) = CByte(G)
        buffer(xIndex + yIndex + 2) = CByte(R)
        buffer(xIndex + yIndex + 3) = CByte(A)
    End Sub

    Private Sub FadePixel(ByVal X As Integer, ByVal Y As Integer, ByVal buffer As Byte(), ByVal PixStride As Integer, fadeStep As Byte)
        Dim xIndex As Integer = X * 4
        Dim yIndex As Integer = Y * PixStride
        Dim A As Double = buffer(xIndex + yIndex + 3)
        A -= fadeStep
        If A < 0 Then A = 0
        buffer(xIndex + yIndex + 3) = CByte(A)
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If Not IsLoaded Then Exit Sub
        Init()
        settingForm.Left = Me.Left + Me.ActualWidth - 15
        settingForm.Top = Me.Top
    End Sub

    Private Sub Window_LocationChanged(sender As Object, e As EventArgs)
        If Not IsLoaded Then Exit Sub
        settingForm.Left = Me.Left + Me.ActualWidth - 15
        settingForm.Top = Me.Top
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

End Class
