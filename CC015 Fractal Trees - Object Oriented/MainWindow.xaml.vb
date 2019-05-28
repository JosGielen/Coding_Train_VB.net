Imports System.IO

Class MainWindow
    Private Tree As List(Of Branch)
    Private leafs As List(Of Ellipse)
    Private Len As Double = 160
    Private Angle As Double
    Private App_Loaded As Boolean = False
    Private ResultFileName As String = "FractalTree.gif"
    Private frameNumber As Integer = 0
    Private Recording As Boolean = False

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        App_Loaded = True
        Init()
    End Sub

    Private Sub Init()
        canvas1.Children.Clear()
        Angle = SldAngle.Value
        Tree = New List(Of Branch)
        leafs = New List(Of Ellipse)
        Dim StartPt As Point = New Point(canvas1.ActualWidth / 2, canvas1.ActualHeight)
        Dim EndPt As Point = New Point(canvas1.ActualWidth / 2, canvas1.ActualHeight - Len)
        Dim Root As Branch = New Branch(StartPt, EndPt)
        Root.Show(canvas1)
        Tree.Add(Root)
        frameNumber = 0
        canvas1.UpdateLayout()
        'Save the window as a jpeg image
        If Recording Then SaveImage(Me)
    End Sub

    Private Sub SldAngle_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Not App_Loaded Then Exit Sub
        Angle = SldAngle.Value
        Init()
    End Sub

    Private Sub BtnGrow_Click(sender As Object, e As RoutedEventArgs)
        Dim newbranches As List(Of Branch) = New List(Of Branch)
        Dim leafSize As Double
        Dim leaf As Ellipse
        'Branch the existing branches
        For I As Integer = 0 To Tree.Count - 1
            newbranches.AddRange(Tree(I).Branch(Angle))
        Next
        If newbranches.Count > 0 Then
            'Remove the previous leaves
            For I As Integer = 0 To leafs.Count - 1
                canvas1.Children.Remove(leafs(I))
            Next
            leafs = New List(Of Ellipse)
            'Draw the new branches
            For I As Integer = 0 To newbranches.Count - 1
                newbranches(I).Show(canvas1)
                Tree.Add(newbranches(I))
                'Draw leaves at the end of the new branches
                leafSize = newbranches(I).Length
                If leafSize < 6 Then leafSize = 6
                leaf = New Ellipse() With
                {
                    .Stroke = Brushes.Green,
                    .StrokeThickness = 1.0,
                    .Fill = Brushes.Green,
                    .Width = leafSize,
                    .Height = leafSize
                }
                leaf.SetValue(Canvas.LeftProperty, newbranches(I).EndPt.X - leafSize / 2)
                leaf.SetValue(Canvas.TopProperty, newbranches(I).EndPt.Y - leafSize / 2)
                canvas1.Children.Add(leaf)
                leafs.Add(leaf)
            Next
            canvas1.UpdateLayout()
            'Save the window as a png image
            If Recording Then SaveImage(Me)
        Else 'The Tree is fully grown
            If Recording Then
                'Convert the png images into an animated Gif.
                MakeGif(1)
                Recording = False
            End If
        End If
    End Sub

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
        For Each f As String In Directory.GetFiles(Environment.CurrentDirectory & "\output")
            If Path.GetExtension(f) = ".png" Then
                File.Delete(f)
            End If
        Next
    End Sub


End Class
