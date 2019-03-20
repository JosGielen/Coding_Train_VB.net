Imports System.IO
Imports Microsoft.Win32

Class MainWindow
    Private Rendering As Boolean = False
    Private ImageFileName As String
    Private SettingsFileName As String
    Private FilePath As String
    Private FileSaveFilterIndex As Integer = 1
    Private W As Integer = 0
    Private H As Integer = 0
    Private bitmap As BitmapSource
    Private PixelData As Byte()
    Private Stride As Integer = 0
    Private my_Color As Color
    Private App_Started As Boolean = False
    Private settingForm As Settings
    Private p1 As Double = 0.01
    Private p2 As Double = 0.85
    Private p3 As Double = 0.07
    Private p4 As Double = 0.07
    Private F1 As Matrix
    Private F2 As Matrix
    Private F3 As Matrix
    Private F4 As Matrix
    Private C1 As Matrix
    Private C2 As Matrix
    Private C3 As Matrix
    Private C4 As Matrix
    Private point As Matrix
    Private Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        W = CInt(canvas1.ActualWidth)
        H = CInt(canvas1.ActualHeight)
        Image1.Width = W
        Image1.Height = H
        point = Matrix.FromArray({0, 0})
        F1 = Matrix.FromArray({{0, 0}, {0, 0.16}})
        F2 = Matrix.FromArray({{0.85, 0.04}, {-0.04, 0.85}})
        F3 = Matrix.FromArray({{0.2, -0.26}, {0.23, 0.22}})
        F4 = Matrix.FromArray({{-0.15, 0.28}, {0.26, 0.24}})
        C1 = Matrix.FromArray({0, 0})
        C2 = Matrix.FromArray({0, 1.6})
        C3 = Matrix.FromArray({0, 1.6})
        C4 = Matrix.FromArray({0, 0.44})
        my_Color = Color.FromRgb(0, 255, 0)
        Init()
        MnuShowSettings.IsChecked = True
        ShowSettingForm()
        App_Started = True
    End Sub

    Private Sub Init()
        W = CInt(canvas1.ActualWidth)
        H = CInt(canvas1.ActualHeight)
        Image1.Width = W
        Image1.Height = H
        Stride = CInt(Image1.Width * PixelFormats.Rgb24.BitsPerPixel / 8)
        ReDim PixelData(CInt(Stride * Image1.Height))
        bitmap = BitmapSource.Create(W, H, 96, 96, PixelFormats.Rgb24, Nothing, PixelData, Stride)
        Image1.Source = bitmap
    End Sub

    Private Sub ShowSettingForm()
        If settingForm Is Nothing Then
            settingForm = New Settings(Me, "None")
            settingForm.Show()
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
            settingForm.p1 = p1
            settingForm.p2 = p2
            settingForm.p3 = p3
            settingForm.p4 = p4
            settingForm.F1 = F1
            settingForm.F2 = F2
            settingForm.F3 = F3
            settingForm.F4 = F4
            settingForm.C1 = C1
            settingForm.C2 = C2
            settingForm.C3 = C3
            settingForm.C4 = C4
        Else
            settingForm.Show()
        End If
    End Sub

    Public Sub Start()
        Init()
        GetParameters()
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        Rendering = True
    End Sub

    Public Sub Halt()
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        Rendering = False
    End Sub

    Private Sub SetPixel(ByVal X As Integer, ByVal Y As Integer, ByVal c As Color, ByVal buffer As Byte(), ByVal PixStride As Integer)
        Dim xIndex As Integer = X * 3
        Dim yIndex As Integer = Y * PixStride
        If xIndex + yIndex + 2 >= 0 And xIndex + yIndex + 2 < buffer.Length Then
            buffer(xIndex + yIndex) = c.R
            buffer(xIndex + yIndex + 1) = c.G
            buffer(xIndex + yIndex + 2) = c.B
        End If
    End Sub

    Public Sub GetParameters()
        If settingForm IsNot Nothing Then
            p1 = settingForm.p1
            p2 = settingForm.p2
            p3 = settingForm.p3
            p4 = settingForm.p4
            F1 = settingForm.F1
            F2 = settingForm.F2
            F3 = settingForm.F3
            F4 = settingForm.F4
            C1 = settingForm.C1
            C2 = settingForm.C2
            C3 = settingForm.C3
            C4 = settingForm.C4
        End If
    End Sub

    Public Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        Dim test As Double
        Dim X As Integer = 0
        Dim Y As Integer = 0
        'Draw a new point
        For I As Integer = 0 To 500
            'range: −2.1820 < x < 2.6558 and 0 ≤ y < 9.9983.
            X = CInt((point.Value(0, 0) + 3) * canvas1.ActualWidth / 6)
            Y = CInt((10.5 - point.Value(1, 0)) * canvas1.ActualHeight / 10.5)
            SetPixel(X, Y, my_Color, PixelData, Stride)
            'Appy point transformation
            test = Rnd.NextDouble()
            If test < p1 Then
                point = F1 * point + C1
            ElseIf test < p1 + p2 Then
                point = F2 * point + C2
            ElseIf test < p1 + p2 + p3 Then
                point = F3 * point + C3
            Else
                point = F4 * point + C4
            End If
        Next
        bitmap = BitmapSource.Create(W, H, 96, 96, PixelFormats.Rgb24, Nothing, PixelData, Stride)
        Image1.Source = bitmap
    End Sub

    Private Sub Window_LocationChanged(sender As Object, e As EventArgs)
        If settingForm IsNot Nothing Then
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
        End If
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If App_Started Then
            If settingForm IsNot Nothing Then
                settingForm.Left = Me.Left + Me.Width
                settingForm.Top = Me.Top
            End If
            Init()
            If Rendering Then Start()
        End If
    End Sub

    Private Sub MnuFileOpen_Click(sender As Object, e As RoutedEventArgs)
        Dim openFileDialog1 As New OpenFileDialog()
        Dim sr As StreamReader
        'Show an OpenFile dialog
        If FilePath = "" Then
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory
        Else
            openFileDialog1.InitialDirectory = FilePath
        End If
        openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        openFileDialog1.FilterIndex = 1
        openFileDialog1.RestoreDirectory = True
        If openFileDialog1.ShowDialog() = True Then
            FilePath = Path.GetDirectoryName(openFileDialog1.FileName)
            SettingsFileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName)
            sr = New StreamReader(openFileDialog1.FileName)
            'Read the setting data from the file
            p1 = Double.Parse(sr.ReadLine())
            p2 = Double.Parse(sr.ReadLine())
            p3 = Double.Parse(sr.ReadLine())
            p4 = Double.Parse(sr.ReadLine())
            F1 = Matrix.FromString(sr.ReadLine())
            F2 = Matrix.FromString(sr.ReadLine())
            F3 = Matrix.FromString(sr.ReadLine())
            F4 = Matrix.FromString(sr.ReadLine())
            C1 = Matrix.FromString(sr.ReadLine())
            C2 = Matrix.FromString(sr.ReadLine())
            C3 = Matrix.FromString(sr.ReadLine())
            C4 = Matrix.FromString(sr.ReadLine())
        End If
    End Sub

    Private Sub MnuFileSave_Click(sender As Object, e As RoutedEventArgs)
        Dim saveFileDialog1 As New SaveFileDialog
        Dim MyEncoder As BitmapEncoder = New BmpBitmapEncoder()
        If FilePath = "" Then
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory
        Else
            saveFileDialog1.InitialDirectory = FilePath
        End If
        saveFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(SettingsFileName)
        saveFileDialog1.FilterIndex = 1
        saveFileDialog1.RestoreDirectory = True
        If saveFileDialog1.ShowDialog() Then
            SettingsFileName = Path.GetFileName(saveFileDialog1.FileName)
            FilePath = Path.GetDirectoryName(saveFileDialog1.FileName)
            SaveSettings(SettingsFileName)
        End If
    End Sub

    Private Sub SaveSettings(fileName As String)
        Dim myStream As StreamWriter = Nothing
        GetParameters()
        Try
            myStream = New StreamWriter(fileName)
            If (myStream IsNot Nothing) Then
                'Write the setting data to the file
                myStream.WriteLine(p1.ToString())
                myStream.WriteLine(p2.ToString())
                myStream.WriteLine(p3.ToString())
                myStream.WriteLine(p4.ToString())
                myStream.WriteLine(F1.ToString())
                myStream.WriteLine(F2.ToString())
                myStream.WriteLine(F3.ToString())
                myStream.WriteLine(F4.ToString())
                myStream.WriteLine(C1.ToString())
                myStream.WriteLine(C2.ToString())
                myStream.WriteLine(C3.ToString())
                myStream.WriteLine(C4.ToString())
            End If
        Catch Ex As Exception
            MessageBox.Show("The Settings could not be saved. Original error: " & Ex.Message, "PROGRAM NAME error", MessageBoxButton.OK, MessageBoxImage.Error)
        Finally
            If (myStream IsNot Nothing) Then
                myStream.Close()
            End If
        End Try
    End Sub

    Private Sub MnuImageSave_Click(sender As Object, e As RoutedEventArgs)
        Dim saveFileDialog1 As New SaveFileDialog
        Dim MyEncoder As BitmapEncoder = New BmpBitmapEncoder()
        If FilePath = "" Then
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory
        Else
            saveFileDialog1.InitialDirectory = FilePath
        End If
        saveFileDialog1.Filter = "Windows Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|TIFF (*.tiff)|*.tiff|PNG (*.png)|*.png"
        saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(ImageFileName)
        saveFileDialog1.FilterIndex = FileSaveFilterIndex
        saveFileDialog1.RestoreDirectory = True
        If saveFileDialog1.ShowDialog() Then
            FileSaveFilterIndex = saveFileDialog1.FilterIndex
            ImageFileName = Path.GetFileName(saveFileDialog1.FileName)
            FilePath = Path.GetDirectoryName(saveFileDialog1.FileName)
            SaveImage(ImageFileName)
        End If
    End Sub

    Private Sub SaveImage(FileName As String)
        Dim MyEncoder As BitmapEncoder = New BmpBitmapEncoder()
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
            Using sw As FileStream = New FileStream(FileName, FileMode.Create)
                MyEncoder.Save(sw)
            End Using
        Catch ex As Exception
            MessageBox.Show("The Image could not be saved. Original error: " & ex.Message, "Barnslet Ferns error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub MnuExit_Click(sender As Object, e As RoutedEventArgs)
        If settingForm IsNot Nothing Then settingForm.Close()
        End
    End Sub

    Private Sub MnuShowSettings_Click(sender As Object, e As RoutedEventArgs)
        If MnuShowSettings.IsChecked Then
            ShowSettingForm()
        Else
            settingForm.Hide()
        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        If settingForm IsNot Nothing Then settingForm.Close()
        End
    End Sub
End Class
