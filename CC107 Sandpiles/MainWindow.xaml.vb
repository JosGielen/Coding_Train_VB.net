Imports System.IO
Imports Microsoft.Win32

Class MainWindow
    Private settingForm As Settings
    Private W As Integer = 0
    Private H As Integer = 0
    Private MaxSand As Integer
    Private InitialSand As Integer
    Private DistributionNr As Integer
    Private pf As PixelFormat = PixelFormats.Rgb24
    Private bitmap As BitmapSource
    Private Stride As Integer = 0
    Private pixelData As Byte()
    Private colorList As List(Of Color)
    Private App_Loaded As Boolean = False
    Private Started As Boolean = False
    Private Pause As Boolean = False
    Private SandPile(,) As Integer


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Init()
        App_Loaded = True
        MnuShowSettings.IsChecked = True
        MnuShowSettings_Click(sender, e)
    End Sub

    Private Sub Init()
        W = CInt(Canvas1.ActualWidth)
        H = CInt(Canvas1.ActualHeight)
        Image1.Width = W
        Image1.Height = H
        Stride = CInt((W * pf.BitsPerPixel + 7) / 8)
        'Resize de arrays
        ReDim SandPile(W, H)
        ReDim pixelData(Stride * H)
        'Get all system colors
        colorList = New List(Of Color)
        For Each propinfo As System.Reflection.PropertyInfo In GetType(Colors).GetProperties
            If propinfo.PropertyType = GetType(Color) Then
                colorList.Add(CType(ColorConverter.ConvertFromString(propinfo.Name), Color))
            End If
        Next
        'Modify some colors
        colorList(0) = Colors.White
        colorList(1) = Colors.Yellow
        colorList(2) = Colors.Orange
        colorList(3) = Colors.Red
        colorList(4) = Colors.DarkRed
        'Set the initial parameters
        MaxSand = 3
        InitialSand = CInt(1.5 * W * H)
        DistributionNr = 4
        'Fill the InitPixelData with white pixels
        For X = 0 To W - 1
            For Y = 0 To H - 1
                SetPixel(X, Y, Color.FromRgb(255, 255, 255), pixelData, Stride)
            Next
        Next
        bitmap = BitmapSource.Create(W, H, 96, 96, pf, Nothing, pixelData, Stride)
        Image1.Source = bitmap
    End Sub

    Public Sub Start()
        Init()
        GetParameters()
        'Poor the sand in the middle
        SandPile(CInt(W / 2), CInt(H / 2)) = InitialSand
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Public Sub Continu()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Public Sub Halt()
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Public Sub GetParameters()
        If settingForm IsNot Nothing Then
            MaxSand = settingForm.MaxSand
            InitialSand = settingForm.InitialSand
            DistributionNr = settingForm.DistributionNr
            colorList = settingForm.ColorList
        End If
    End Sub

    Public Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Dim spill As Integer = 0
        Dim count As Integer = 0
        'Distribute the sand
        For I As Integer = 0 To 10
            count = 0
            For X = 1 To W - 2
                For Y = 1 To H - 2
                    If SandPile(X, Y) > MaxSand Then
                        spill = CInt(Math.Floor(SandPile(X, Y) / (DistributionNr)))
                        SandPile(X, Y) -= DistributionNr * spill
                        SandPile(X - 1, Y) += spill
                        SandPile(X + 1, Y) += spill
                        SandPile(X, Y - 1) += spill
                        SandPile(X, Y + 1) += spill
                        If DistributionNr = 8 Then
                            SandPile(X - 1, Y - 1) += spill
                            SandPile(X - 1, Y + 1) += spill
                            SandPile(X + 1, Y - 1) += spill
                            SandPile(X + 1, Y + 1) += spill
                        End If
                        count += 1
                    End If
                Next
            Next
            'Update the pixeldata colors
            For X = 0 To W - 1
                For Y = 0 To H - 1
                    If SandPile(X, Y) <= MaxSand Then
                        SetPixel(X, Y, colorList(SandPile(X, Y)), pixelData, Stride)
                    Else
                        SetPixel(X, Y, colorList(MaxSand + 1), pixelData, Stride)
                    End If
                Next
            Next
            If count = 0 Then
                Halt()
                settingForm.BtnStart.Content = "Start"
            End If
        Next
        bitmap = BitmapSource.Create(W, H, 96, 96, pf, Nothing, pixelData, Stride)
        Image1.Source = bitmap
    End Sub

    Private Sub SetPixel(ByVal X As Integer, ByVal Y As Integer, ByVal c As Color, ByVal buffer As Byte(), ByVal PixStride As Integer)
        Dim xIndex As Integer = X * 3
        Dim yIndex As Integer = Y * PixStride
        buffer(xIndex + yIndex) = c.R
        buffer(xIndex + yIndex + 1) = c.G
        buffer(xIndex + yIndex + 2) = c.B
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        If settingForm IsNot Nothing Then settingForm.Close()
        End
    End Sub

    Private Sub Window_LocationChanged(sender As Object, e As EventArgs)
        If settingForm IsNot Nothing Then
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
        End If
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If App_Loaded Then
            If settingForm IsNot Nothing Then
                settingForm.Left = Me.Left + Me.Width
                settingForm.Top = Me.Top
            End If
            Init()
        End If
    End Sub

    Private Sub MnuSaveImage_Click(sender As Object, e As RoutedEventArgs)
        Dim saveFileDialog1 As New SaveFileDialog
        Dim FileSaveFilterIndex As Integer = 1
        Dim MyEncoder As BitmapEncoder = New BmpBitmapEncoder()
        saveFileDialog1.InitialDirectory = Environment.CurrentDirectory
        saveFileDialog1.Filter = "Windows Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|TIFF (*.tiff)|*.tiff|PNG (*.png)|*.png"
        saveFileDialog1.FilterIndex = FileSaveFilterIndex
        saveFileDialog1.RestoreDirectory = True
        If saveFileDialog1.ShowDialog() Then
            FileSaveFilterIndex = saveFileDialog1.FilterIndex
            Try
                Select Case FileSaveFilterIndex
                    Case 1
                        MyEncoder = New BmpBitmapEncoder()
                    Case 2
                        MyEncoder = New JpegBitmapEncoder()
                    Case 3
                        MyEncoder = New GifBitmapEncoder()
                    Case 4
                        MyEncoder = New TiffBitmapEncoder()
                    Case 5
                        MyEncoder = New PngBitmapEncoder()
                    Case Else
                        'Should not occur
                        Exit Sub
                End Select
                MyEncoder.Frames.Add(BitmapFrame.Create(bitmap))
                ' Create an instance of StreamWriter to write the histogram to the file.
                Using sw As FileStream = New FileStream(saveFileDialog1.FileName, FileMode.Create)
                    MyEncoder.Save(sw)
                End Using
            Catch ex As Exception
                MessageBox.Show("The Image could not be saved.", "AreaPixelcount error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Private Sub MnuExit_Click(sender As Object, e As RoutedEventArgs)
        If settingForm IsNot Nothing Then settingForm.Close()
        End
    End Sub

    Private Sub MnuShowSettings_Click(sender As Object, e As RoutedEventArgs)
        If MnuShowSettings.IsChecked Then
            If settingForm Is Nothing Then
                settingForm = New Settings(Me, colorList)
                settingForm.Show()
                settingForm.Left = Me.Left + Me.Width
                settingForm.Top = Me.Top
                settingForm.Type = "Default"
                settingForm.MaxSand = MaxSand
                settingForm.InitialSand = InitialSand
                settingForm.DistributionNr = DistributionNr
            Else
                settingForm.Show()
            End If
        Else
            settingForm.Hide()
        End If
    End Sub
End Class
