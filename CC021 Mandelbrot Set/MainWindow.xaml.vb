Imports Microsoft.Win32
Imports System.IO
Imports System.Windows.Threading

Class MainWindow
    Private Delegate Sub RenderDelegate(ByVal Y As Integer)
    Private Delegate Sub StatusDelegate(ByVal v As Double)
    Private pf As PixelFormat = PixelFormats.Rgb24
    Private Stride As Integer = 0
    Private pixelData As Byte()
    Private imgbrush As New ImageBrush()
    Private my_PaletteFile As String = ""
    Private Colors As New List(Of Color)
    Private Iters As Double(,)
    Private Xmin As Double = 0D
    Private Xmax As Double = 0D
    Private Ymin As Double = 0D
    Private Ymax As Double = 0D
    Private X1 As Double = 0D
    Private Y1 As Double = 0D
    Private X2 As Double = 0D
    Private Y2 As Double = 0D
    Private CalcRatio As Double = 0.0
    Private Zmax As Integer = 100     'Default Bail-out value
    Private Nmax As Integer = 500     'Default Max number of iterations
    Private Colormultiplier As Double = 1
    Private ColorStartIndex As Double = 0
    Private VeldWidth As Integer = 0
    Private VeldHeight As Integer = 0
    Private MaxColIndex As Integer = 0
    Private Started As Boolean = False
    Private CanResize As Boolean = False

#Region "Window Events"

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        TxtBailout.Text = Zmax.ToString()
        TxtMaxIter.Text = Nmax.ToString()
        LblColorMag.Content = "Color Magnifier : " & Colormultiplier.ToString()
        LblColorScroll.Content = "Color Scroll : " & ColorStartIndex.ToString()
        'Set the initial Canvas Size
        VeldWidth = 700
        VeldHeight = 600
        ResizeWindow(VeldWidth, VeldHeight)
        ResetFractalData(VeldWidth, VeldHeight)
        CalcRatio = VeldWidth / VeldHeight
        'Initial window = (-2.5,-1.5) - (1,1.5) 
        Xmin = -2.5
        Ymin = -1.5
        Xmax = 1
        Ymax = 1.5
        X1 = Xmin
        X2 = Xmax
        Y1 = Ymin
        Y2 = Ymax
        'Set the initial color palette
        my_PaletteFile = Environment.CurrentDirectory & "\default.cpl"
        OpenPalette(my_PaletteFile)
        imgbrush.ImageSource = BitmapSource.Create(VeldWidth, VeldHeight, 96, 96, pf, Nothing, pixelData, Stride)
        imgbrush.Stretch = Stretch.UniformToFill
        Canvas1.Background = imgbrush
        'Set the rubberband properties
        RBand.AspectRatio = VeldWidth / VeldHeight
        RBand.Stroke = Brushes.Yellow
        RBand.BoxFillOpacity = 0.3
        RBand.BoxFillColor = Brushes.LightGray
        RBand.CornerSize = 5
        RBand.IsEnabled = False
        CanResize = True
        Me.Title = "MandelBrot"
    End Sub

    Private Sub Window_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        RBand.Mouse_Down(e.GetPosition(Canvas1))
    End Sub

    Private Sub Window_MouseMove(sender As Object, e As MouseEventArgs)
        RBand.Mouse_Move(e.GetPosition(Canvas1))
    End Sub

    Private Sub Window_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        RBand.Mouse_Up(e.GetPosition(Canvas1))
    End Sub

    Private Sub Window_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)
        Dim W As Double = Canvas1.ActualWidth
        Dim H As Double = Canvas1.ActualHeight
        Dim X1N As Double = 0D
        Dim X2N As Double = 0D
        Dim Y1N As Double = 0D
        Dim Y2N As Double = 0D
        If RBand.IsEnabled And RBand.IsDrawn Then
            X1N = X1 + RBand.TopLeftCorner.X * (X2 - X1) / W
            X2N = X1 + RBand.BottomRightCorner.X * (X2 - X1) / W
            Y1N = Y1 + RBand.TopLeftCorner.Y * (Y2 - Y1) / H
            Y2N = Y1 + RBand.BottomRightCorner.Y * (Y2 - Y1) / H
            If 3.5 / (X2N - X1N) > 10000000000000.0 Then
                If MessageBox.Show("The requested zoom exceeds the calculation accuracy." & vbCrLf & "Do you wish to proceed?", "Fractally Information", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) = MessageBoxResult.No Then Exit Sub
            End If
            X1 = X1N
            X2 = X2N
            Y1 = Y1N
            Y2 = Y2N
            RBand.Clear()
            KeepRatio()
            BtnCalc_Click(Me, New RoutedEventArgs())
        End If
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If CanResize Then
            'Fit the Canvas into the new window size
            Canvas1.Width = e.NewSize.Width - 168
            Canvas1.Height = e.NewSize.Height - 94
            VeldWidth = CInt(e.NewSize.Width - 168)
            VeldHeight = CInt(e.NewSize.Height - 94)
            KeepRatio()
            RBand.AspectRatio = VeldWidth / VeldHeight
        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

#End Region

#Region "Menu"

    Private Sub MnuOpenFractal_Click(sender As Object, e As RoutedEventArgs)
        Dim OFD As New OpenFileDialog()
        Dim myStream As StreamReader = Nothing
        Dim newX1 As Double = 0
        Dim newX2 As Double = 0
        Dim newY1 As Double = 0
        Dim newY2 As Double = 0
        Dim check As Boolean = False
        OFD.InitialDirectory = Environment.CurrentDirectory
        OFD.Filter = "Fractals (*.frc)|*.frc"
        OFD.FilterIndex = 1
        OFD.RestoreDirectory = True
        If OFD.ShowDialog() Then
            Try
                myStream = New StreamReader(OFD.FileName)
                If (myStream IsNot Nothing) Then
                    ' Lees de Instelling data uit de file
                    VeldWidth = Integer.Parse(myStream.ReadLine())
                    VeldHeight = Integer.Parse(myStream.ReadLine())
                    my_PaletteFile = myStream.ReadLine()
                    newX1 = Double.Parse(myStream.ReadLine())
                    newX2 = Double.Parse(myStream.ReadLine())
                    newY1 = Double.Parse(myStream.ReadLine())
                    newY2 = Double.Parse(myStream.ReadLine())
                    Colormultiplier = Double.Parse(myStream.ReadLine())
                    ColorStartIndex = Double.Parse(myStream.ReadLine())
                    Zmax = Integer.Parse(myStream.ReadLine())
                    Nmax = Integer.Parse(myStream.ReadLine())
                    check = Boolean.Parse(myStream.ReadLine())
                    'Pas de Window en array afmetingen aan
                    ResizeWindow(VeldWidth, VeldHeight)
                    ResetFractalData(VeldWidth, VeldHeight)
                    'Lees de Fractal data
                    For I As Integer = 0 To VeldWidth - 1
                        For J As Integer = 0 To VeldHeight - 1
                            Iters(I, J) = Double.Parse(myStream.ReadLine())
                        Next
                    Next
                End If
            Catch Ex As Exception
                MessageBox.Show("Cannot read the Fractal data. Original error: " & Ex.Message, "Fractally error", MessageBoxButton.OK, MessageBoxImage.Error)
                Exit Sub
            Finally
                'Check of myStream wel open is want er kan een exception geweest zijn.
                If (myStream IsNot Nothing) Then
                    myStream.Close()
                End If
            End Try
            'Laad de palette kleuren
            OpenPalette(my_PaletteFile)
            'Zet de controls op de nieuwe waarden
            TxtBailout.Text = Zmax.ToString()
            TxtMaxIter.Text = Nmax.ToString()
            SldColorMag.Value = Colormultiplier
            SldColorScroll.Value = ColorStartIndex
            CBSmooth.IsChecked = check
            X1 = newX1
            X2 = newX2
            Y1 = newY1
            Y2 = newY2
            'Onthoud de berekende waarden
            CalcRatio = VeldWidth / VeldHeight
            'Teken de fractal
            UpdateColors()
            'Update the progressbar
            PBStatus.Value = 100
        End If
        RBand.IsEnabled = True
    End Sub

    Private Sub MnuSavefractal_Click(sender As Object, e As RoutedEventArgs)
        Dim SFD As New SaveFileDialog()
        Dim myStream As StreamWriter = Nothing
        SFD.InitialDirectory = Environment.CurrentDirectory
        SFD.Filter = "Fractals (*.frc)|*.frc"
        SFD.FilterIndex = 1
        SFD.RestoreDirectory = True
        If SFD.ShowDialog() Then
            Try
                myStream = New StreamWriter(SFD.FileName)
                If (myStream IsNot Nothing) Then
                    'Write the fractal data to the file
                    myStream.WriteLine(VeldWidth)
                    myStream.WriteLine(VeldHeight)
                    myStream.WriteLine(my_PaletteFile)
                    myStream.WriteLine(X1)
                    myStream.WriteLine(X2)
                    myStream.WriteLine(Y1)
                    myStream.WriteLine(Y2)
                    myStream.WriteLine(Colormultiplier)
                    myStream.WriteLine(ColorStartIndex)
                    myStream.WriteLine(Zmax)
                    myStream.WriteLine(Nmax)
                    myStream.WriteLine(CBSmooth.IsChecked)
                    For I As Integer = 0 To VeldWidth - 1
                        For J As Integer = 0 To VeldHeight - 1
                            myStream.WriteLine(Iters(I, J))
                        Next
                    Next
                    'Update the progressbar
                    PBStatus.Value = 100
                End If
            Catch Ex As Exception
                'Update the progressbar
                PBStatus.Value = 0
                MessageBox.Show("Cannot save the Fractal data. Original error: " & Ex.Message, "Fractally error", MessageBoxButton.OK, MessageBoxImage.Error)
            Finally
                ' Check this again, since we need to make sure we didn't throw an exception on open.
                If (myStream IsNot Nothing) Then
                    myStream.Close()
                End If
            End Try
        End If
    End Sub

    Private Sub MnuExit_Click(sender As Object, e As RoutedEventArgs)
        End
    End Sub

    Private Sub MnuReset_Click(sender As Object, e As RoutedEventArgs)
        Xmin = -2.5D
        Xmax = 1D
        Ymin = -1.5D
        Ymax = 1.5D
        X1 = Xmin
        X2 = Xmax
        Y1 = Ymin
        Y2 = Ymax
        KeepRatio()
        BtnCalc_Click(Me, New RoutedEventArgs())
    End Sub

    Private Sub MnuOpenPalette_Click(sender As Object, e As RoutedEventArgs)
        Dim openFileDialog1 As New OpenFileDialog()
        Dim ColIndex As Integer = 0
        openFileDialog1.InitialDirectory = Environment.CurrentDirectory
        openFileDialog1.Filter = "Color Palettes (*.cpl)|*.cpl"
        openFileDialog1.FilterIndex = 1
        openFileDialog1.RestoreDirectory = True
        If openFileDialog1.ShowDialog() Then
            my_PaletteFile = openFileDialog1.FileName
            OpenPalette(my_PaletteFile)
            UpdateColors()
        End If
    End Sub

#End Region

#Region "Controls"

    Private Sub BtnBailoutUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtBailout.Text)
        dummy += 50
        TxtBailout.Text = dummy.ToString()
    End Sub

    Private Sub BtnBailoutDown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtBailout.Text)
        If dummy > 50 Then
            dummy -= 50
            TxtBailout.Text = dummy.ToString()
        End If
    End Sub

    Private Sub BtnMaxIterUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtMaxIter.Text)
        dummy += 100
        TxtMaxIter.Text = dummy.ToString()
    End Sub

    Private Sub BtnMaxIterDown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtMaxIter.Text)
        If dummy > 100 Then
            dummy -= 100
            TxtMaxIter.Text = dummy.ToString()
        End If
    End Sub

    Private Sub SldColorMag_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If SldColorMag.Value <> Math.Round(SldColorMag.Value) Then
            SldColorMag.Value = Math.Round(SldColorMag.Value)
        End If
        If SldColorMag.Value <> Colormultiplier Then
            If SldColorMag.Value = 0 Then
                Colormultiplier = 1
            ElseIf SldColorMag.Value < 0 Then
                Colormultiplier = 1 + SldColorMag.Value * 0.9 / 50
            Else
                Colormultiplier = SldColorMag.Value
            End If
            LblColorMag.Content = "Color Magnifier : " & Colormultiplier.ToString()
            UpdateColors()
        End If
    End Sub

    Private Sub SldColorScroll_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If SldColorScroll.Value <> Math.Round(SldColorScroll.Value) Then
            SldColorScroll.Value = Math.Round(SldColorScroll.Value)
        End If
        If SldColorScroll.Value <> ColorStartIndex Then
            ColorStartIndex = SldColorScroll.Value
            LblColorScroll.Content = "Color Scroll : " & ColorStartIndex.ToString()
            UpdateColors()
        End If
    End Sub

#End Region

#Region "Fractal Calculation"

    Private Sub BtnCalc_Click(sender As Object, e As RoutedEventArgs)
        'Start/stop the application
        Dim Y As Integer = 0
        Started = Not Started
        If Started Then
            BtnCalc.Content = "STOP"
            Integer.TryParse(TxtBailout.Text, Zmax)
            Integer.TryParse(TxtMaxIter.Text, Nmax)
            'Set the Canvas size to the actual Fractal size
            ResizeWindow(VeldWidth, VeldHeight)
            'Reset the arrays
            ResetFractalData(VeldWidth, VeldHeight)
            CalcRatio = VeldWidth / VeldHeight
        Else
            BtnCalc.Content = "CALCULATE"
        End If
        'Calculate the Fractal Line per Line
        Y = 0
        Do While Started
            Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New RenderDelegate(AddressOf RenderLine), Y)
            Y += 1
            If Y >= VeldHeight Then
                BtnCalc.Content = "CALCULATE"
                Started = False
            End If
        Loop
        RBand.IsEnabled = True
    End Sub

    Private Sub RenderLine(ByVal J As Integer)
        'Fill the buffer with the pixels from line J
        Dim ColIndex As Integer = 0
        Dim X0 As Double = 0
        Dim Y0 As Double = Y1 + J * (Y2 - Y1) / VeldHeight  'Calculate Y value of line J
        For I As Integer = 0 To VeldWidth - 1
            X0 = X1 + I * (X2 - X1) / VeldWidth      'Calculate X value of the pixel
            'Use the selected fractal type
            Iters(I, J) = Mandelbrot(X0, Y0)
            If Iters(I, J) < 0 Then
                SetPixel(I, J, Color.FromRgb(0, 0, 0), pixelData, Stride)
            Else
                ColIndex = CInt(Colormultiplier * Iters(I, J) + ColorStartIndex) Mod MaxColIndex
                SetPixel(I, J, Colors(ColIndex), pixelData, Stride)
            End If
        Next
        'Update the progressbar
        PBStatus.Value = 100 * (J + 1) / VeldHeight
        'Show the partially calculated fractal
        Dim bmp As BitmapSource = BitmapSource.Create(VeldWidth, VeldHeight, 96, 96, pf, Nothing, pixelData, Stride)
        imgbrush.ImageSource = bmp
    End Sub

    Private Sub SetPixel(ByVal x As Integer, ByVal y As Integer, ByVal c As Color, ByVal buffer As Byte(), ByVal PixStride As Integer)
        Dim xIndex As Integer = x * 3
        Dim yIndex As Integer = y * PixStride
        buffer(xIndex + yIndex) = c.R
        buffer(xIndex + yIndex + 1) = c.G
        buffer(xIndex + yIndex + 2) = c.B
    End Sub

    Private Function Mandelbrot(ByVal X0 As Double, ByVal Y0 As Double) As Double
        Dim X As Double = 0     'X coordinate during iterations
        Dim Y As Double = 0     'Y coordinate during iterations
        Dim N As Integer = 0
        Dim modul As Double = 0.0
        Dim Xtemp As Double = 0
        MaxColIndex = Colors.Count - 1
        N = 0
        X = 0
        Y = 0
        While X * X + Y * Y < Zmax And N < Nmax
            Xtemp = X * X - Y * Y + X0
            Y = 2 * X * Y + Y0
            X = Xtemp
            N += 1
        End While
        If N >= Nmax Then
            Return -1
        Else
            'Do 2 more iterations for the color smoothing to work OK
            Xtemp = X * X - Y * Y + X0
            Y = 2 * X * Y + Y0
            X = Xtemp
            N += 1
            Xtemp = X * X - Y * Y + X0
            Y = 2 * X * Y + Y0
            X = Xtemp
            N += 1
            'Return the Color Index
            If CBSmooth.IsChecked Then
                modul = Math.Sqrt(X * X + Y * Y)
                Return N - (Math.Log(Math.Log(modul))) / Math.Log(2)
            Else
                Return N
            End If
        End If
    End Function

#End Region

#Region "Utilities"

    Private Sub OpenPalette(ByVal pal As String)
        Dim myStream As StreamReader = Nothing
        Dim myLine As String = ""
        Dim ColAantal As Integer = 0
        Dim txtparts(2) As String
        Dim r As Byte = 0
        Dim g As Byte = 0
        Dim b As Byte = 0
        Try
            myStream = New StreamReader(pal)
            If (myStream IsNot Nothing) Then
                Colors.Clear()
                MaxColIndex = Integer.Parse(myStream.ReadLine())
                ' Lees de palette kleuren
                For I As Integer = 0 To MaxColIndex - 1
                    myLine = myStream.ReadLine()
                    txtparts = myLine.Split(";"c)
                    r = Byte.Parse(txtparts(0))
                    g = Byte.Parse(txtparts(1))
                    b = Byte.Parse(txtparts(2))
                    Colors.Add(Color.FromRgb(r, g, b))
                Next
                If ColorStartIndex <= MaxColIndex Then
                    SldColorScroll.Value = ColorStartIndex
                Else
                    SldColorScroll.Value = SldColorScroll.Minimum
                End If
                SldColorScroll.Maximum = MaxColIndex
                SldColorScroll.TickFrequency = CInt(MaxColIndex / 10)
            End If
        Catch Ex As Exception
            MessageBox.Show("Cannot read file. Original error: " & Ex.Message, "Fractally error", MessageBoxButton.OK, MessageBoxImage.Error)
        Finally
            ' Check this again, since we need to make sure we didn't throw an exception on open.
            If (myStream IsNot Nothing) Then
                myStream.Close()
            End If
        End Try
    End Sub

    Private Sub ResizeWindow(ByVal w As Integer, ByVal h As Integer)
        'Resize the entire window when the Fractal-Image size needs to change
        'but do not process the Window.SizeChange Event!
        CanResize = False
        Me.Width = w + 168
        Me.Height = h + 94
        Canvas1.Width = w
        Canvas1.Height = h
        CanResize = True
    End Sub

    Private Sub KeepRatio()
        Dim NewRatio As Double = VeldWidth / VeldHeight
        Dim MidX As Double = (X1 + X2) / 2
        Dim MidY As Double = (Y1 + Y2) / 2
        'Scale X or Y-axis to keep aspect ratio
        If NewRatio > CalcRatio Then 'Adjust Fractal Height
            Y1 = MidY - (X2 - X1) / (2 * NewRatio)
            Y2 = MidY + (X2 - X1) / (2 * NewRatio)
        Else 'Adjust Fractal Width
            X1 = MidX - NewRatio * (Y2 - Y1) / 2
            X2 = MidX + NewRatio * (Y2 - Y1) / 2
        End If
    End Sub

    Private Sub ResetFractalData(ByVal w As Integer, ByVal h As Integer)
        If w > 0 And h > 0 Then
            Stride = CInt((w * pf.BitsPerPixel + 7) / 8)
            'Resize de arrays
            ReDim pixelData(Stride * h)
            ReDim Iters(w, h)
        End If
    End Sub

    Private Sub UpdateColors()
        'Update the fractal colors
        MaxColIndex = Colors.Count()
        SldColorScroll.Maximum = MaxColIndex
        SldColorScroll.TickFrequency = CInt(MaxColIndex / 10)
        Dim ColIndex As Integer = 0
        Cursor = Cursors.Wait
        For I As Integer = 0 To VeldWidth - 1
            For J As Integer = 0 To VeldHeight - 1
                If Iters(I, J) < 0 Then
                    SetPixel(I, J, Color.FromRgb(0, 0, 0), pixelData, Stride)
                Else
                    ColIndex = CInt(Colormultiplier * Iters(I, J) + ColorStartIndex) Mod MaxColIndex
                    SetPixel(I, J, Colors(ColIndex), pixelData, Stride)
                End If
            Next
        Next
        'Show the bitmap
        Dim bmp As BitmapSource = BitmapSource.Create(VeldWidth, VeldHeight, 96, 96, pf, Nothing, pixelData, Stride)
        imgbrush.ImageSource = bmp
        Cursor = Cursors.Arrow
    End Sub

    Private Sub UpdateStatus(ByVal v As Double)
        PBStatus.Value = v
    End Sub

#End Region


End Class
