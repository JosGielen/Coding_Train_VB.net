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
    Private my_Colors As List(Of Color)
    Private palet As BitmapPalette
    Private Rendering As Boolean = False
    Private Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Init()
    End Sub

    Private Sub Init()
        Dim cp As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\Rainbow.cpl")
        my_Colors = cp.GetColors(25)
        Image1.Width = Canvas1.ActualWidth
        Image1.Height = Canvas1.ActualHeight
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
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            BtnStart.Content = "Stop"
            Rendering = True
            Render()
        Else
            BtnStart.Content = "Start"
            Rendering = False
        End If
    End Sub

    Public Sub Render()
        Dim X As Integer = CInt(Canvas1.ActualWidth / 2)
        Dim Y As Integer = CInt(Canvas1.ActualHeight / 2)
        Do While Rendering
            SetPixel(X, Y, Stride)
            Select Case Rnd.Next(4)
                Case 0
                    X = X + 1
                Case 1
                    X = X - 1
                Case 2
                    Y = Y + 1
                Case 3
                    Y = Y - 1
            End Select
            If X < 0 Then X = 0
            If X > W - 1 Then X = W - 1
            If Y < 0 Then Y = 0
            If Y > H - 1 Then Y = H - 1
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), 1)
        Loop
    End Sub


    Private Sub SetPixel(ByVal X As Integer, ByVal Y As Integer, ByVal PixStride As Integer)
        Dim xIndex As Integer = X * 3
        Dim yIndex As Integer = Y * PixStride
        Dim colorIndex As Integer = pixelData(xIndex + yIndex)
        pixelCounts(X, Y) += 1
        If pixelCounts(X, Y) > 24 Then pixelCounts(X, Y) = 24
        'Make a rectangle with Width=1 and Height=1
        Dim rect As Int32Rect = New Int32Rect(X, Y, 1, 1)
        buffer(0) = my_Colors(pixelCounts(X, Y)).R
        buffer(1) = my_Colors(pixelCounts(X, Y)).G
        buffer(2) = my_Colors(pixelCounts(X, Y)).B
        If rect.X < Writebitmap.PixelWidth And rect.Y < Writebitmap.PixelHeight Then
            Writebitmap.WritePixels(rect, buffer, PixStride, 0)
        End If
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

End Class
