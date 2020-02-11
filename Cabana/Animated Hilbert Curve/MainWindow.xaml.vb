Imports System.Windows.Threading
Imports System.IO
Imports System.Threading

Class MainWindow
    Private Delegate Sub RenderDelegate()
    Private Delegate Sub UpdateDelegate(ByVal I As Integer)
    Private Rendering As Boolean = False
    Private Order As Integer
    Private Total As Integer
    Private Len As Double
    Private Linear() As Vector
    Private Path() As Vector
    Private Interpolated() As Vector
    Private Lines() As Line
    Private My_Brushes() = {Brushes.Red, Brushes.DeepSkyBlue, Brushes.Lime, Brushes.Yellow}
    Private Recording As Boolean
    Private DeleteImages As Boolean = True
    Private WaitTime As Integer
    Private UseColors As Boolean
    Private frameNumber As Integer = 0
    Private ResultFileName As String = "HilbertMorph.gif"

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If Height > Width + 67 Then
            Width = Height - 67
        Else
            Height = Width + 67
        End If
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            Rendering = True
            BtnStart.Content = "STOP"
            Recording = CbRecording.IsChecked.Value
            UseColors = CbUseColor.IsChecked.Value
            Dim PrevPath() As Vector
            'Zero order starts as a point in the center and morphs out to order 1.
            Order = 1
            Len = canvas1.ActualWidth / (Math.Pow(2, Order))
            Total = (Math.Pow(2, Order)) ^ 2
            Dim L As Line
            ReDim Linear(Total - 1)
            ReDim Path(Total - 1)
            ReDim Interpolated(Total - 1)
            ReDim Lines(Total - 1)
            If Recording Then
                WaitTime = 1000
            Else
                WaitTime = 100
            End If
            Dim maxOrder As Integer = Integer.Parse(TxtMaxOrder.Text)
            'Calculate the points
            For I As Integer = 0 To Total - 1
                Linear(I) = New Vector(canvas1.ActualWidth / 2, canvas1.ActualHeight / 2)
                Path(I) = Hilbert(I)
                Interpolated(I) = Linear(I)
            Next
            'Draw the lines
            canvas1.Children.Clear()
            For I As Integer = 0 To Interpolated.Length - 2
                L = New Line() With
                    {
                        .StrokeThickness = 2.0,
                        .X1 = Interpolated(I).X,
                        .Y1 = Interpolated(I).Y,
                        .X2 = Interpolated(I + 1).X,
                        .Y2 = Interpolated(I + 1).Y
                    }
                If UseColors Then
                    L.Stroke = My_Brushes(I And 3)
                Else
                    L.Stroke = Brushes.DeepSkyBlue
                End If
                Lines(I) = L
                canvas1.Children.Add(L)
            Next
            'Morph the lines from Linear to Path
            For p As Double = 0 To 1 Step 0.04
                If Not Rendering Then Exit Sub
                For I As Integer = 0 To Total - 1
                    Interpolated(I) = Lerp(Linear(I), Path(I), p)
                Next
                Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New RenderDelegate(AddressOf Render))
                Thread.Sleep(WaitTime)
                If Recording Then SaveImage(canvas1)
            Next
            'Higher orders are formed by linear interpolating 4 points in each line 
            '(except in the connection lines)
            For Order = 2 To maxOrder
                Len = canvas1.ActualWidth / (Math.Pow(2, Order))
                Total = (Math.Pow(2, Order)) ^ 2
                PrevPath = Path
                ReDim Linear(Total - 1)
                ReDim Path(Total - 1)
                ReDim Interpolated(Total - 1)
                ReDim Lines(Total - 1)
                'Calculate 4 points between the points of the Previous Path 
                'But not between the 3rd and 4th point (= connecting line)
                Dim index As Integer
                Dim counter As Integer = 0
                For I As Integer = 0 To PrevPath.Length - 2
                    index = I And 3
                    If index < 3 Then
                        For Perc As Double = 0 To 0.8 Step 0.2
                            Linear(counter) = Lerp(PrevPath(I), PrevPath(I + 1), Perc)
                            counter += 1
                        Next
                    ElseIf index = 3 Then
                        Linear(counter) = PrevPath(I)
                        counter += 1
                    End If
                Next
                Linear(counter) = PrevPath.Last
                For I As Integer = 0 To Total - 1
                    Path(I) = Hilbert(I)
                    Interpolated(I) = Linear(I)
                Next
                canvas1.Children.Clear()
                'Draw the initial lines
                For I As Integer = 0 To Interpolated.Length - 2
                    L = New Line() With
                        {
                            .StrokeThickness = 2.0,
                            .X1 = Interpolated(I).X,
                            .Y1 = Interpolated(I).Y,
                            .X2 = Interpolated(I + 1).X,
                            .Y2 = Interpolated(I + 1).Y
                        }
                    If UseColors Then
                        L.Stroke = My_Brushes(I And 3)
                    Else
                        L.Stroke = Brushes.DeepSkyBlue
                    End If
                    Lines(I) = L
                    canvas1.Children.Add(L)
                Next
                'Morph the lines from Linear to Path
                For p As Double = 0 To 1 Step 0.02
                    If Not Rendering Then Exit Sub
                    For I As Integer = 0 To Total - 1
                        Interpolated(I) = Lerp(Linear(I), Path(I), p)
                    Next
                    Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New RenderDelegate(AddressOf Render))
                    Thread.Sleep(WaitTime)
                    If Recording Then SaveImage(canvas1)
                Next
            Next
            'Wait to show the last order curve
            If Recording Then
                For I As Integer = 0 To 40
                    Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New RenderDelegate(AddressOf Render))
                    Thread.Sleep(WaitTime)
                    SaveImage(canvas1)
                Next
                'Convert the png images into an animated Gif.
                MakeGif(15)
                Recording = False
            End If
            Rendering = False
            BtnStart.Content = "START"
        Else
            Rendering = False
            BtnStart.Content = "START"
        End If
    End Sub

    Private Function Lerp(V1 As Vector, V2 As Vector, percent As Double) As Vector
        Dim X As Double = V1.X + percent * (V2.X - V1.X)
        Dim Y As Double = V1.Y + percent * (V2.Y - V1.Y)
        Return New Vector(X, Y)
    End Function

    Private Sub Render()
        For I As Integer = 0 To Interpolated.Length - 2
            Lines(I).X1 = Interpolated(I).X
            Lines(I).Y1 = Interpolated(I).Y
            Lines(I).X2 = Interpolated(I + 1).X
            Lines(I).Y2 = Interpolated(I + 1).Y
        Next
    End Sub

    Private Function Hilbert(Nr As Integer) As Vector
        Dim Index As Integer
        Dim V(3) As Vector
        Dim Result As Vector
        Dim Offset As Integer
        Dim Dummy As Integer
        V(0) = New Vector(0, 0)
        V(1) = New Vector(0, 1)
        V(2) = New Vector(1, 1)
        V(3) = New Vector(1, 0)
        Index = Nr And 3
        Result = V(Index)
        For I As Integer = 1 To Order - 1
            Offset = CInt(Math.Pow(2, I))
            Nr >>= 2
            Index = Nr And 3
            If Index = 0 Then
                Dummy = Result.X
                Result.X = Result.Y
                Result.Y = Dummy
            End If
            If Index = 1 Then
                Result.Y += Offset
            End If
            If Index = 2 Then
                Result.X += Offset
                Result.Y += Offset
            End If
            If Index = 3 Then
                Dummy = Offset - 1 - Result.X
                Result.X = Offset - 1 - Result.Y
                Result.Y = Dummy
                Result.X += Offset
            End If
        Next
        Result.X = Len / 2 + Len * Result.X
        Result.Y = Len / 2 + Len * Result.Y
        Return Result
    End Function

    Private Sub SaveImage(Element As FrameworkElement)
        Dim dirInfo As DirectoryInfo = Directory.CreateDirectory(Environment.CurrentDirectory & "\output")
        Dim dir As String = dirInfo.FullName
        Dim fileName As String = dir & "\Image-" & frameNumber.ToString("0000") & ".png"
        Dim MyEncoder As BitmapEncoder = New PngBitmapEncoder()
        Dim renderbmp As RenderTargetBitmap = New RenderTargetBitmap(CInt(Element.ActualWidth), CInt(Element.ActualHeight), 96, 96, PixelFormats.Default)
        renderbmp.Render(Element)
        Try
            MyEncoder.Frames.Add(BitmapFrame.Create(renderbmp))
            ' Create a FileStream to write the image to the file.
            Using sw As FileStream = New FileStream(fileName, FileMode.Create)
                MyEncoder.Save(sw)
            End Using
            frameNumber += 1
        Catch ex As Exception
            MessageBox.Show("The Image could not be saved.", "NoiseGifLoop error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MakeGif(frameRate As Integer)
        'Create an animated Gif with ffmpeg.exe
        Dim prog As String = Environment.CurrentDirectory & "\ffmpeg.exe"
        Dim args As String = " -framerate " & frameRate.ToString() & " -i output\Image-%4d.png " & ResultFileName
        Dim p As Process = Process.Start(prog, args)
        p.WaitForExit()
        'Delete the image files
        If DeleteImages Then
            For Each f As String In Directory.GetFiles(Environment.CurrentDirectory & "\output")
                If System.IO.Path.GetExtension(f) = ".png" Then
                    File.Delete(f)
                End If
            Next
        End If
    End Sub

End Class
