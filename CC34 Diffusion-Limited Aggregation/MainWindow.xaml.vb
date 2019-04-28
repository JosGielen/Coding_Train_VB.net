Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private WaitTime As Integer = 1
    Private PixelData() As Byte
    Private NewPixelData(2) As Byte
    Private Writebitmap As WriteableBitmap
    Private Stride As Integer = 0
    Private colorList As List(Of Color)
    Private palet As BitmapPalette
    Private App_Started As Boolean = False
    Private Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        'Set the Image size
        image1.Width = canvas1.ActualWidth
        image1.Height = canvas1.ActualHeight
        'Make a WriteableBitmap
        Stride = CInt(image1.Width * PixelFormats.Rgb24.BitsPerPixel / 8)
        ReDim PixelData(CInt(Stride * image1.Height))
        colorList = New List(Of Color) From {
            Colors.Red,
            Colors.Green,
            Colors.Blue
        }
        palet = New BitmapPalette(colorList)
        Writebitmap = New WriteableBitmap(BitmapSource.Create(CInt(image1.Width), CInt(image1.Height), 96, 96, PixelFormats.Rgb24, palet, PixelData, Stride))
        image1.Source = Writebitmap
        Init()
    End Sub

    Private Sub Init()
        'Make all pixels white
        For I As Integer = 0 To PixelData.Length - 1
            PixelData(I) = 255
        Next
        'Set a seed in the middle
        Dim index As Integer
        For I As Integer = CInt(Writebitmap.PixelWidth / 2) - 3 To CInt(Writebitmap.PixelWidth / 2) + 3
            For J As Integer = CInt(Writebitmap.PixelHeight / 2) - 3 To CInt(Writebitmap.PixelHeight / 2) + 3
                index = CInt(3 * (J * Writebitmap.PixelWidth + I))
                PixelData(index) = 0
                PixelData(index + 1) = 0
                PixelData(index + 2) = 0
            Next
        Next
        Dim Intrect As Int32Rect
        Intrect = New Int32Rect(0, 0, Writebitmap.PixelWidth - 1, Writebitmap.PixelHeight - 1)
        Writebitmap.WritePixels(Intrect, PixelData, Stride, 0)
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not App_Started Then
            App_Started = True
            BtnStart.Content = "STOP"
            Render()
        Else
            App_Started = False
            BtnStart.Content = "START"
        End If
    End Sub

    Private Sub Render()
        Dim X As Integer
        Dim Y As Integer
        Dim stuck As Boolean
        Dim dir As Integer
        Do While App_Started
            X = CInt(Writebitmap.PixelWidth * Rnd.NextDouble())
            Y = CInt(Writebitmap.PixelHeight * Rnd.NextDouble())
            Do
                If X < 2 Then X = 2
                If X > Writebitmap.PixelWidth - 2 Then X = Writebitmap.PixelWidth - 2
                If Y < 2 Then Y = 2
                If Y > Writebitmap.PixelHeight - 2 Then Y = Writebitmap.PixelHeight - 2
                stuck = False
                If IsFixed(X - 1, Y - 1) Then stuck = True
                If IsFixed(X - 1, Y) Then stuck = True
                If IsFixed(X - 1, Y + 1) Then stuck = True
                If IsFixed(X, Y - 1) Then stuck = True
                If IsFixed(X, Y + 1) Then stuck = True
                If IsFixed(X + 1, Y - 1) Then stuck = True
                If IsFixed(X + 1, Y) Then stuck = True
                If IsFixed(X + 1, Y + 1) Then stuck = True
                If Not App_Started Or stuck Then
                    Exit Do
                Else
                    dir = Rnd.Next(0, 8)
                    Select Case dir
                        Case 0
                            X = X - 1
                            Y = Y - 1
                        Case 1
                            X = X - 1
                        Case 2
                            X = X - 1
                            Y = Y + 1
                        Case 3
                            Y = Y - 1
                        Case 4
                            Y = Y + 1
                        Case 5
                            X = X + 1
                            Y = Y - 1
                        Case 6
                            X = X + 1
                        Case 7
                            X = X + 1
                            Y = Y + 1
                    End Select
                End If
            Loop
            DrawPixel(X, Y)
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
        Loop
    End Sub

    Private Function IsFixed(X As Integer, Y As Integer) As Boolean
        Return PixelData(3 * (Y * Writebitmap.PixelWidth + X)) = 0
    End Function

    Private Sub DrawPixel(X As Integer, Y As Integer)
        'Set the Pixeldata at position X,Y to 0
        Dim index As Integer = 3 * (Y * Writebitmap.PixelWidth + X)
        PixelData(index) = 0
        PixelData(index + 1) = 0
        PixelData(index + 2) = 0
        'Make a rectangle of size 1,1
        Dim rect As Int32Rect = New Int32Rect(X, Y, 1, 1)
        'Set the pixel at X, Y to black
        NewPixelData(0) = 0
        NewPixelData(1) = 0
        NewPixelData(2) = 0
        If rect.X < Writebitmap.PixelWidth And rect.Y < Writebitmap.PixelHeight Then
            Writebitmap.WritePixels(rect, NewPixelData, Stride, 0)
        End If
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub
End Class
