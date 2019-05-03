Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 10
    Private pf As PixelFormat = PixelFormats.Rgb24
    Private VeldWidth As Integer = 0
    Private VeldHeight As Integer = 0
    Private ImageWidth As Integer = 0
    Private ImageHeight As Integer = 0
    Private Stride As Integer = 0
    Private pixelData As Byte()
    Private Rainbow As List(Of Color)
    Private AppLoaded As Boolean = False
    Private AppRunning As Boolean = False
    Private balls() As ball
    Private BallCount As Integer = 5
    Private BallSize As Double = 10
    Private fuzzy As Boolean = False
    Private sharpness As Double = 1.4
    Private framecounter As Integer = 0

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        VeldWidth = canvas1.ActualWidth
        VeldHeight = canvas1.ActualHeight
        Image1.Width = canvas1.ActualWidth
        Image1.Height = canvas1.ActualHeight
        AppLoaded = True
        Init()
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If AppLoaded Then
            VeldWidth = CInt(Me.Width - 10)
            VeldHeight = CInt(Me.Height - 54)
            Init()
        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        AppLoaded = False
        AppRunning = False
        End
    End Sub

    Private Sub Init()
        Dim r As Byte
        Dim g As Byte
        Dim b As Byte
        Dim bal As Ball
        Dim rnd As Random = New Random()
        ImageWidth = VeldWidth
        ImageHeight = VeldHeight
        Stride = CInt((ImageWidth * pf.BitsPerPixel + 7) / 8)
        Image1.Width = ImageWidth
        Image1.Height = ImageHeight
        'Resize de array
        ReDim pixelData(Stride * ImageHeight)
        'Load the Rainbow colors
        Rainbow = New List(Of Color)
        Dim pal As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Rainbow.cpl")
        Rainbow = pal.GetColors(512)
        'Make the balls
        ReDim balls(BallCount)
        For I As Integer = 0 To BallCount
            bal = New Ball(BallSize, New Point((VeldWidth - BallSize) * rnd.NextDouble(), (VeldHeight - BallSize) * rnd.NextDouble()), 5 * rnd.NextDouble() - 2.5, 5 * rnd.NextDouble() - 2.5)
            balls(I) = bal
        Next
        Me.Title = "MetaBalls"
        Start()
    End Sub

    Public Sub Start()
        AppRunning = True
        Do While AppRunning
            For I As Integer = 0 To BallCount
                balls(I).Update(VeldWidth, VeldHeight)
            Next
            Render()
            Me.Dispatcher.Invoke(DispatcherPriority.Background, New WaitDelegate(AddressOf Wait), WaitTime)
            framecounter += 1
            Me.Title = framecounter.ToString()
        Loop
    End Sub

    Private Sub SetPixel(ByVal x As Integer, ByVal y As Integer, ByVal c As Color, ByVal buffer As Byte(), ByVal PixStride As Integer)
        Dim xIndex As Integer = x * 3
        Dim yIndex As Integer = y * PixStride
        buffer(xIndex + yIndex) = c.R
        buffer(xIndex + yIndex + 1) = c.G
        buffer(xIndex + yIndex + 2) = c.B
    End Sub

    Private Sub Render()
        Dim dist As Double
        Dim index As Double = 0
        For X As Integer = 0 To VeldWidth - 1
            For Y As Integer = 0 To VeldHeight - 1
                dist = 0.0
                For I As Integer = 0 To BallCount
                    dist += BallSize / Math.Sqrt((X - balls(I).X) ^ 2 + (Y - balls(I).Y) ^ 2)
                Next
                index = 1.5 * (1 - dist) * (Rainbow.Count - 1)
                If index < 0 Then
                    SetPixel(X, Y, Rainbow.First, pixelData, Stride)
                ElseIf index < Rainbow.Count - 1 Then
                    SetPixel(X, Y, Rainbow(CInt(index)), pixelData, Stride)
                Else
                    SetPixel(X, Y, Rainbow.Last, pixelData, Stride)
                End If
            Next
        Next
        Dim bitmap As BitmapSource = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, pf, Nothing, pixelData, Stride)
        Image1.Source = bitmap
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(WaitTime)
    End Sub

End Class
