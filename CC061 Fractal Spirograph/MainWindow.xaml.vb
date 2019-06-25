Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private W As Integer = 0
    Private H As Integer = 0
    Private pixformat As PixelFormat
    Private Writebitmap As WriteableBitmap
    Private Stride As Integer = 0
    Private pixelData As Byte()
    Private pixelCounts As Integer(,)
    Private buffer(2) As Byte
    Private colorList As List(Of Color)
    Private palet As BitmapPalette
    Private Rendering As Boolean = False
    Private settingForm As Settings
    Private EpicycleCount As Integer = 6
    Private speedStep As Integer = -4
    Private RadiusFactor As Integer = 33
    Private innerCircles As Boolean = False
    Private Epicycles As List(Of Epicycle)
    Private Time As Double
    Private TimeStep As Double = 0.0005
    Private previousPt As Point

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        settingForm = New Settings(Me)
        settingForm.Left = Me.Left + Me.Width
        settingForm.Top = Me.Top
        settingForm.Show()
        settingForm.CircleCount = EpicycleCount
        settingForm.RadiusFactor = RadiusFactor
        settingForm.SpeedFactor = speedStep
        settingForm.TimeStep = TimeStep
    End Sub

    Private Sub Init()
        Dim epi As Epicycle
        Dim Radius As Double = canvas1.ActualWidth / 6
        Epicycles = New List(Of Epicycle)
        Time = 0.0
        'Get the settings
        EpicycleCount = settingForm.CircleCount
        RadiusFactor = settingForm.RadiusFactor
        speedStep = settingForm.SpeedFactor
        innerCircles = settingForm.InnerCircles
        TimeStep = settingForm.TimeStep
        'Create the Bitmap
        Image1.Width = canvas1.ActualWidth
        Image1.Height = canvas1.ActualHeight
        W = CInt(Image1.Width)
        H = CInt(Image1.Height)
        Stride = CInt(W * PixelFormats.Rgb24.BitsPerPixel / 8)
        ReDim pixelData(Stride * H)
        ReDim pixelCounts(W, H)
        colorList = New List(Of Color) From {
            Colors.Red,
            Colors.Green,
            Colors.Blue
        }
        palet = New BitmapPalette(colorList)
        'Show the start-up image
        Writebitmap = New WriteableBitmap(BitmapSource.Create(W, H, 96, 96, PixelFormats.Rgb24, palet, pixelData, Stride))
        Image1.Source = Writebitmap

        'Create the Epicycles
        For I As Integer = 0 To EpicycleCount - 1
            epi = New Epicycle(Math.Pow(speedStep, I), Radius)
            Epicycles.Add(epi)
            Radius = RadiusFactor * Radius / 100
        Next
    End Sub

    Private Sub Render()
        Dim pt As Point
        Do While Rendering
            pt = SumEpicycles()
            SetPixel(CInt(pt.X), CInt(pt.Y), Stride)
            Time += TimeStep
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), 1)
            If Time >= 2 * Math.PI Then 'Reset the drawing
                settingForm.BtnStart.Content = "Start"
                Halt()
            End If
        Loop

    End Sub

    Private Function SumEpicycles() As Point
        Dim X As Double
        Dim Y As Double
        'Draw the Epicycles
        X = canvas1.ActualWidth / 2
        Y = canvas1.ActualHeight / 2
        For I As Integer = 0 To Epicycles.Count - 2
            If innerCircles Then
                X += (Epicycles(I).Radius - Epicycles(I + 1).Radius) * Math.Cos(Epicycles(I).Speed * Time - Math.PI / 2)
                Y += (Epicycles(I).Radius - Epicycles(I + 1).Radius) * Math.Sin(Epicycles(I).Speed * Time - Math.PI / 2)
            Else
                X += (Epicycles(I).Radius + Epicycles(I + 1).Radius) * Math.Cos(Epicycles(I).Speed * Time - Math.PI / 2)
                Y += (Epicycles(I).Radius + Epicycles(I + 1).Radius) * Math.Sin(Epicycles(I).Speed * Time - Math.PI / 2)
            End If
        Next
        Return New Point(X, Y)
    End Function

    Private Sub SetPixel(ByVal X As Integer, ByVal Y As Integer, ByVal PixStride As Integer)
        Dim xIndex As Integer = X * 3
        Dim yIndex As Integer = Y * PixStride
        Dim colorIndex As Integer = pixelData(xIndex + yIndex)
        pixelCounts(X, Y) += 1
        If pixelCounts(X, Y) > 24 Then pixelCounts(X, Y) = 24
        'Make a rectangle with Width=1 and Height=1
        Dim rect As Int32Rect = New Int32Rect(X, Y, 1, 1)
        buffer(0) = 255
        buffer(1) = 0
        buffer(2) = 0
        If rect.X < Writebitmap.PixelWidth And rect.Y < Writebitmap.PixelHeight Then
            Writebitmap.WritePixels(rect, buffer, PixStride, 0)
        End If
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Public Sub Start()
        Init()
        Rendering = True
        Render()
    End Sub

    Public Sub Halt()
        Rendering = False
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If IsLoaded Then
            Init()
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
        End If
    End Sub

    Private Sub Window_LocationChanged(sender As Object, e As EventArgs)
        If IsLoaded Then
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
        End If
    End Sub

End Class



