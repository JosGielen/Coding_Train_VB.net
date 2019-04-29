Imports System.Windows.Threading
Imports System.Threading
Imports System.IO
Imports Microsoft.Win32
Imports System.Text

Class MainWindow
    Private settingForm As Settings
    Private Delegate Sub RenderDelegate()
    Private pf As PixelFormat = PixelFormats.Rgb24
    Private VeldWidth As Integer = 0
    Private VeldHeight As Integer = 0
    Private ImageWidth As Integer = 0
    Private ImageHeight As Integer = 0
    Private Stride As Integer = 0
    Private pixelData As Byte()
    Private AppRunning As Boolean = False
    Private AppLoaded As Boolean = False
    Private myColors As List(Of Color)
    Private myGen As Generation
    Private StartGen As Generation

#Region "Initialisation"
    Private Sub Init()
        ImageWidth = VeldWidth
        ImageHeight = VeldHeight
        Stride = CInt((ImageWidth * pf.BitsPerPixel + 7) / 8)
        Image1.Width = ImageWidth
        Image1.Height = ImageHeight
        'Resize de array
        ReDim pixelData(Stride * ImageHeight)
        'Create new generations 
        myGen = New Generation(VeldWidth, VeldHeight)
        myGen.DiffA = 1.0     '1.0
        myGen.DiffB = 0.5     '0.5
        myGen.Feed = 0.055    '0.055
        myGen.Kill = 0.062    '0.062
        StartGen = New Generation(VeldWidth, VeldHeight)
        StartGen.DiffA = 1      '1.0
        StartGen.DiffB = 0.5    '0.5
        StartGen.Feed = 0.055   '0.055
        StartGen.Kill = 0.062   '0.062
        'Create a Settings Form
        settingForm = New Settings(Me)
        settingForm.Left = Me.Left + Me.Width
        settingForm.Top = Me.Top
        settingForm.Show()
        settingForm.DiffA = myGen.DiffA
        settingForm.DiffB = myGen.DiffB
        settingForm.Feed = myGen.Feed
        settingForm.Kill = myGen.Kill
        AppRunning = False
        Me.Title = "Reaction-Diffusion: Initialized"
        Render()
    End Sub
#End Region

#Region "Window Events"

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        VeldWidth = 200
        VeldHeight = 200
        Me.Width = 210  'VeldWidth + 2* Window Borderthickness
        Me.Height = 254 'VeldHeight + 2 * Window Borderthickenss + Menu Height + Window Titlebar Height
        Dim pal As ColorPalette = New ColorPalette(Environment.CurrentDirectory & "\ThermalBlack.cpl")
        myColors = pal.GetColors(256)
        AppLoaded = True
        Init()
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If AppLoaded Then
            VeldWidth = CInt(Me.Width - 10)
            VeldHeight = CInt(Me.Height - 54)
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
            Init()
        End If
    End Sub

    Private Sub Window_Closed(sender As Object, e As EventArgs)
        AppLoaded = False
        End
    End Sub

    Public Sub Start()
        myGen = New Generation(VeldWidth, VeldHeight)
        StartGen = New Generation(VeldWidth, VeldHeight)
        myGen.DiffA = settingForm.DiffA
        myGen.DiffB = settingForm.DiffB
        myGen.Feed = settingForm.Feed
        myGen.Kill = settingForm.Kill
        'Copy MyGen to StartGen
        For X As Integer = 0 To myGen.Breedte - 1
            For Y As Integer = 0 To myGen.Hoogte - 1
                StartGen.CellA(X, Y) = myGen.CellA(X, Y)
                StartGen.CellB(X, Y) = myGen.CellB(X, Y)
            Next
        Next
        'Start the game and render while the application is Idle
        'This allows other events to stop the Do loop
        AppRunning = True
        Do While AppRunning
            myGen.update()
            Me.Title = "Reaction-Diffusion: Generation " & myGen.Volgnummer.ToString
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New RenderDelegate(AddressOf Render))
        Loop
    End Sub

    Public Sub Halt()
        AppRunning = False
    End Sub

#End Region

#Region "Menu Events"
    Private Sub MenuNew_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuNew.Click
        myGen.Clear()
        Me.Title = "Reaction-Diffusion: New"
        Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New RenderDelegate(AddressOf Render))
    End Sub

    Private Sub MenuOpen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuOpen.Click
        Dim myStream As StreamReader = Nothing
        Dim fileWidth As Integer = 0
        Dim fileHeight As Integer = 0
        Dim openFileDialog1 As New OpenFileDialog()
        openFileDialog1.InitialDirectory = Environment.CurrentDirectory
        openFileDialog1.Multiselect = False
        openFileDialog1.DefaultExt = ".*"
        openFileDialog1.Filter = "All files (*.*)|*.*"
        openFileDialog1.FilterIndex = 1
        openFileDialog1.RestoreDirectory = True
        If openFileDialog1.ShowDialog() Then
            Try
                myStream = New StreamReader(openFileDialog1.OpenFile())
                If (myStream IsNot Nothing) Then
                    'Lees de afmetingen van de Reaction-Diffusion setting in de file
                    fileWidth = Integer.Parse(myStream.ReadLine())
                    fileHeight = Integer.Parse(myStream.ReadLine())
                    'Pas het veld aan voor deze afmetingen
                    Me.Width = fileWidth + 10
                    Me.Height = fileHeight + 54
                    myGen = New Generation(fileWidth, fileHeight)
                    'Lees de Reaction-Diffusion Parameters
                    myGen.DiffA = Double.Parse(myStream.ReadLine())
                    myGen.DiffB = Double.Parse(myStream.ReadLine())
                    myGen.Feed = Double.Parse(myStream.ReadLine())
                    myGen.Kill = Double.Parse(myStream.ReadLine())
                    Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New RenderDelegate(AddressOf Render))
                End If
            Catch Ex As Exception
                MessageBox.Show("Cannot read file from disk. Original error: " & Ex.Message)
            Finally
                ' Check this again, since we need to make sure we didn't throw an exception on open.
                If (myStream IsNot Nothing) Then
                    myStream.Close()
                End If
            End Try
        End If
    End Sub

    Private Sub MenuSaveAs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuSaveAs.Click
        Dim saveFileDialog1 As New SaveFileDialog()
        saveFileDialog1.InitialDirectory = Environment.CurrentDirectory
        saveFileDialog1.Filter = "All files (*.*)|*.*"
        saveFileDialog1.FilterIndex = 1
        saveFileDialog1.RestoreDirectory = True
        If saveFileDialog1.ShowDialog() Then
            'Write the Data to the File 
            Using outfile As New StreamWriter(saveFileDialog1.FileName)
                outfile.WriteLine(VeldWidth)
                outfile.WriteLine(VeldHeight)
                outfile.WriteLine(myGen.DiffA)
                outfile.WriteLine(myGen.DiffB)
                outfile.WriteLine(myGen.Feed)
                outfile.WriteLine(myGen.Kill)
            End Using
            Me.Title = "Reaction - Diffusion: " & Path.GetFileName(saveFileDialog1.FileName)
        End If
    End Sub

    Private Sub MenuExit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuExit.Click
        End
    End Sub


    Private Sub Menu200_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu200.Click
        Me.Width = 210
        Me.Height = 254
        Menu300.IsChecked = False
        Menu400.IsChecked = False
        Menu500.IsChecked = False
    End Sub

    Private Sub Menu300_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu300.Click
        Me.Width = 310
        Me.Height = 354
        Menu200.IsChecked = False
        Menu400.IsChecked = False
        Menu500.IsChecked = False
    End Sub

    Private Sub Menu400_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu400.Click
        Me.Width = 410
        Me.Height = 454
        Menu200.IsChecked = False
        Menu300.IsChecked = False
        Menu500.IsChecked = False
    End Sub

    Private Sub Menu500_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu500.Click
        Me.Width = 510
        Me.Height = 554
        Menu200.IsChecked = False
        Menu300.IsChecked = False
        Menu400.IsChecked = False
    End Sub

#End Region

#Region "Business Methods"
    Private Sub SetPixel(ByVal x As Integer, ByVal y As Integer, ByVal c As Color, ByVal buffer As Byte(), ByVal PixStride As Integer)
        Dim xIndex As Integer = x * 3
        Dim yIndex As Integer = y * PixStride
        buffer(xIndex + yIndex) = c.R
        buffer(xIndex + yIndex + 1) = c.G
        buffer(xIndex + yIndex + 2) = c.B
    End Sub

    Private Sub Render()
        Dim ColIndex As Integer
        For X As Integer = 0 To VeldWidth - 1
            For Y As Integer = 0 To VeldHeight - 1
                If myGen.CellB(X, Y) > 0 Then
                    If myGen.CellB(X, Y) < 1 Then
                        ColIndex = 2 * 255 * (1 - myGen.CellB(X, Y))
                    Else
                        ColIndex = 0
                    End If
                Else
                    ColIndex = 255
                End If
                SetPixel(X, Y, myColors(ColIndex Mod 256), pixelData, Stride)
            Next Y
        Next X
        Dim bitmap As BitmapSource = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, pf, Nothing, pixelData, Stride)
        Image1.Source = bitmap
        Thread.Sleep(40)
    End Sub

    Private Sub Window_LocationChanged(sender As Object, e As EventArgs)
        If AppLoaded Then
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
        End If
    End Sub

#End Region

End Class
