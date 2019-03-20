Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Private Delegate Sub RenderDelegate(pt As Point)
    Private PixelData As Byte()
    Private NewPixelData(2) As Byte
    Private Writebitmap As WriteableBitmap
    Private Stride As Integer = 0
    Private colorList As List(Of Color)
    Private palet As BitmapPalette
    Private PiEstimate As Double
    Private Total As Long = 0
    Private inCircle As Long = 0
    Private started As Boolean = False

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Image1.Width = MyCanvas.ActualWidth
        Image1.Height = MyCanvas.ActualHeight
        Stride = CInt(Image1.Width * PixelFormats.Rgb24.BitsPerPixel / 8)
        ReDim PixelData(Stride * Image1.Height)
        colorList = New List(Of Color) From {
            Colors.Red,
            Colors.Green,
            Colors.Blue
        }
        palet = New BitmapPalette(colorList)
        Writebitmap = New WriteableBitmap(BitmapSource.Create(Image1.Width, Image1.Height, 96, 96, PixelFormats.Rgb24, palet, PixelData, Stride))
        Image1.Source = Writebitmap
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        Dim rand As Random = New Random(0)
        Dim W As Double = MyCanvas.ActualWidth
        Dim H As Double = MyCanvas.ActualHeight
        Dim X As Double = 0.0
        Dim Y As Double = 0.0
        Dim pt As Point

        If Not started Then
            Total = 0
            inCircle = 0
            PiEstimate = 0.0
            BtnStart.Content = "STOP"
            Writebitmap = New WriteableBitmap(BitmapSource.Create(Image1.Width, Image1.Height, 96, 96, PixelFormats.Rgb24, palet, PixelData, Stride))
            Image1.Source = Writebitmap
            started = True
        Else
            BtnStart.Content = "START"
            started = False
        End If
        Do While started
            For I As Integer = 0 To 10000
                X = MyCanvas.ActualWidth * rand.NextDouble()
                Y = MyCanvas.ActualHeight * rand.NextDouble()
                If (X - W / 2) ^ 2 + (Y - H / 2) ^ 2 < (W / 2) ^ 2 Then
                    inCircle += 1
                End If
                Total += 1
            Next
            pt = New Point(X, Y)
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New RenderDelegate(AddressOf Draw), pt)
        Loop

    End Sub

    Private Sub Draw(pt As Point)
        Dim rect As Int32Rect = New Int32Rect(CInt(pt.X), CInt(pt.Y), 1, 1)
        If LocationsDistance(pt, New Point(MyCanvas.ActualWidth / 2, MyCanvas.ActualHeight / 2)) <= MyCanvas.ActualWidth / 2 Then
            NewPixelData(0) = 0
            NewPixelData(1) = 255
            NewPixelData(2) = 0
        Else
            NewPixelData(0) = 255
            NewPixelData(1) = 0
            NewPixelData(2) = 0
        End If
        TxtNumber.Text = Total.ToString()
        TxtPiEstimate.Text = (4 * inCircle / Total)
        If rect.X < Writebitmap.PixelWidth And rect.Y < Writebitmap.PixelHeight Then
            Writebitmap.WritePixels(rect, NewPixelData, Stride, 0)
        End If

    End Sub

    Public Function LocationsDistance(ByVal p1 As Point, ByVal p2 As Point) As Double
        Return Math.Sqrt((p2.X - p1.X) ^ 2 + (p2.Y - p1.Y) ^ 2)
    End Function

End Class
