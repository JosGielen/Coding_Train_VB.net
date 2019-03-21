
Imports Microsoft.Win32
Imports System.IO

Class MainWindow
    Private Delegate Sub RenderLineDelegate(l As Line)
    Private settingForm As Settings
    Private MyString As String = ""
    'Start settings
    Private FractalType As String = "Initial Settings"
    Private InitialString As String = "X"
    Private InitialLength As Double = 0.38
    Private InitialAngle As Integer = 270
    Private ShowLeaves As Boolean = False
    Private LeavesSize As Integer = 6
    Private MaxIter As Integer = 7
    Private StartPosX As Double = 0.5
    Private StartPosY As Double = 1
    Private BranchColor As Brush = Brushes.Brown
    Private LeavesColor As Brush = Brushes.Green
    Private LengthScaling As Double = 0.5
    Private BranchVariation As Boolean = False
    Private BranchStartThickness As Integer = 1
    Private DeflectionAngle As Double = 35
    Private AllowRandom As Boolean = False
    Private RandomPercentage As Integer = 20
    Private A_rule As String = "AA"
    Private B_rule As String = "B"
    Private C_rule As String = "C"
    Private X_rule As String = "A[+X][-X]AX"
    Private Y_rule As String = "Y"
    Private Z_rule As String = "Z"

#Region "Window Events"

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        txtType.Text = FractalType
    End Sub

    Private Sub Window_LocationChanged(sender As Object, e As EventArgs)
        If settingForm IsNot Nothing Then
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
        End If
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If settingForm IsNot Nothing Then
            settingForm.Left = Me.Left + Me.Width
            settingForm.Top = Me.Top
        End If
        Generate()
    End Sub

#End Region

#Region "Menus"

    Private Sub MnuShowSettings_Click(sender As Object, e As RoutedEventArgs)
        If MnuShowSettings.IsChecked Then
            If settingForm Is Nothing Then
                settingForm = New Settings(Me)
                settingForm.FractalType = FractalType
                settingForm.InitialString = InitialString
                settingForm.InitialLength = InitialLength
                settingForm.InitialAngle = InitialAngle
                settingForm.ShowLeaves = ShowLeaves
                settingForm.LeavesSize = LeavesSize
                settingForm.MaxIter = MaxIter
                settingForm.StartPosX = StartPosX
                settingForm.StartPosY = StartPosY
                settingForm.BranchColor = BranchColor
                settingForm.LeavesColor = LeavesColor
                settingForm.LengthScaling = LengthScaling
                settingForm.BranchVariation = BranchVariation
                settingForm.BranchStartThickness = BranchStartThickness
                settingForm.DeflectionAngle = DeflectionAngle
                settingForm.AllowRandom = AllowRandom
                settingForm.RandomPercentage = RandomPercentage
                settingForm.A_rule = A_rule
                settingForm.B_rule = B_rule
                settingForm.C_rule = C_rule
                settingForm.X_rule = X_rule
                settingForm.Y_rule = Y_rule
                settingForm.Z_rule = Z_rule
                settingForm.Left = Me.Left + Me.Width
                settingForm.Top = Me.Top
            End If
            settingForm.Show()
        Else
            settingForm.Hide()
        End If
    End Sub

    Private Sub MnuFileOpen_Click(sender As Object, e As RoutedEventArgs)
        Dim bc As BrushConverter = New BrushConverter()
        Dim openFileDialog1 As New OpenFileDialog()
        openFileDialog1.InitialDirectory = Environment.CurrentDirectory
        openFileDialog1.Filter = "L-System File (*.txt)|*.txt"
        openFileDialog1.RestoreDirectory = True
        If openFileDialog1.ShowDialog() = True Then
            Try
                Using sr As StreamReader = New StreamReader(openFileDialog1.FileName)
                    ' Read the data from the file.
                    FractalType = Path.GetFileName(openFileDialog1.FileName)
                    InitialString = sr.ReadLine()
                    InitialLength = Double.Parse(sr.ReadLine())
                    InitialAngle = Integer.Parse(sr.ReadLine())
                    ShowLeaves = Boolean.Parse(sr.ReadLine())
                    LeavesSize = Integer.Parse(sr.ReadLine())
                    MaxIter = Integer.Parse(sr.ReadLine())
                    StartPosX = Double.Parse(sr.ReadLine())
                    StartPosY = Double.Parse(sr.ReadLine())
                    BranchColor = CType(bc.ConvertFromString(sr.ReadLine()), Brush)
                    LeavesColor = CType(bc.ConvertFromString(sr.ReadLine()), Brush)
                    LengthScaling = Double.Parse(sr.ReadLine())
                    BranchVariation = Boolean.Parse(sr.ReadLine())
                    BranchStartThickness = Integer.Parse(sr.ReadLine())
                    DeflectionAngle = Integer.Parse(sr.ReadLine())
                    AllowRandom = Boolean.Parse(sr.ReadLine())
                    RandomPercentage = Integer.Parse(sr.ReadLine())
                    A_rule = sr.ReadLine()
                    B_rule = sr.ReadLine()
                    C_rule = sr.ReadLine()
                    X_rule = sr.ReadLine()
                    Y_rule = sr.ReadLine()
                    Z_rule = sr.ReadLine()
                End Using
                If settingForm IsNot Nothing Then
                    settingForm.FractalType = FractalType
                    settingForm.InitialString = InitialString
                    settingForm.InitialLength = InitialLength
                    settingForm.InitialAngle = InitialAngle
                    settingForm.ShowLeaves = ShowLeaves
                    settingForm.LeavesSize = LeavesSize
                    settingForm.MaxIter = MaxIter
                    settingForm.StartPosX = StartPosX
                    settingForm.StartPosY = StartPosY
                    settingForm.BranchColor = BranchColor
                    settingForm.LeavesColor = LeavesColor
                    settingForm.LengthScaling = LengthScaling
                    settingForm.BranchVariation = BranchVariation
                    settingForm.BranchStartThickness = BranchStartThickness
                    settingForm.DeflectionAngle = DeflectionAngle
                    settingForm.AllowRandom = AllowRandom
                    settingForm.RandomPercentage = RandomPercentage
                    settingForm.A_rule = A_rule
                    settingForm.B_rule = B_rule
                    settingForm.C_rule = C_rule
                    settingForm.X_rule = X_rule
                    settingForm.Y_rule = Y_rule
                    settingForm.Z_rule = Z_rule
                    settingForm.Title = "L-Systems Settings : " & Path.GetFileName(openFileDialog1.FileName)
                End If
                txtType.Text = FractalType
            Catch ex As Exception
                MessageBox.Show("The Parameters in File " & Path.GetFileName(openFileDialog1.FileName) & " are not valid.", "L-System error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Private Sub MnuFileSave_Click(sender As Object, e As RoutedEventArgs)
        Dim saveFileDialog1 As New SaveFileDialog
        saveFileDialog1.InitialDirectory = Environment.CurrentDirectory
        saveFileDialog1.Filter = "L-System File (*.txt)|*.txt"
        saveFileDialog1.RestoreDirectory = True
        If saveFileDialog1.ShowDialog() Then
            Using outfile As New StreamWriter(saveFileDialog1.FileName)
                outfile.WriteLine(InitialString)
                outfile.WriteLine(InitialLength.ToString())
                outfile.WriteLine(InitialAngle.ToString())
                outfile.WriteLine(ShowLeaves.ToString())
                outfile.WriteLine(LeavesSize.ToString())
                outfile.WriteLine(MaxIter.ToString())
                outfile.WriteLine(StartPosX.ToString())
                outfile.WriteLine(StartPosY.ToString())
                outfile.WriteLine(BranchColor.ToString())
                outfile.WriteLine(LeavesColor.ToString())
                outfile.WriteLine(LengthScaling.ToString())
                outfile.WriteLine(BranchVariation.ToString())
                outfile.WriteLine(BranchStartThickness.ToString())
                outfile.WriteLine(DeflectionAngle.ToString())
                outfile.WriteLine(AllowRandom.ToString())
                outfile.WriteLine(RandomPercentage.ToString())
                outfile.WriteLine(A_rule)
                outfile.WriteLine(B_rule)
                outfile.WriteLine(C_rule)
                outfile.WriteLine(X_rule)
                outfile.WriteLine(Y_rule)
                outfile.WriteLine(Z_rule)
            End Using
            FractalType = Path.GetFileName(saveFileDialog1.FileName)
            txtType.Text = FractalType
        End If
    End Sub

    Private Sub MnuImageSave_Click(sender As Object, e As RoutedEventArgs)
        Dim Rect As New Rect(MyCanvas.Margin.Left, MyCanvas.Margin.Top, MyCanvas.ActualWidth, MyCanvas.ActualHeight)
        Dim rtb As New RenderTargetBitmap(CInt(Rect.Right), CInt(Rect.Bottom), 96.0, 96.0, System.Windows.Media.PixelFormats.Default)
        rtb.Render(MyCanvas)
        Dim saveFileDialog1 As New SaveFileDialog
        Dim MyEncoder As BitmapEncoder = New BmpBitmapEncoder()
        saveFileDialog1.InitialDirectory = Environment.CurrentDirectory
        saveFileDialog1.Filter = "Windows Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|TIFF (*.tiff)|*.tiff|PNG (*.png)|*.png"
        saveFileDialog1.FilterIndex = 1
        saveFileDialog1.RestoreDirectory = True
        If saveFileDialog1.ShowDialog() Then
            Try
                Select Case saveFileDialog1.FilterIndex
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
                MyEncoder.Frames.Add(BitmapFrame.Create(rtb))
                ' Create an instance of StreamWriter to write the Image to the file.
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

#End Region

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        Generate()
    End Sub

    Public Sub GetParameters()
        FractalType = "Custom Type"
        InitialString = settingForm.InitialString
        InitialLength = settingForm.InitialLength
        InitialAngle = settingForm.InitialAngle
        ShowLeaves = settingForm.ShowLeaves
        LeavesSize = settingForm.LeavesSize
        MaxIter = settingForm.MaxIter
        StartPosX = settingForm.StartPosX
        StartPosY = settingForm.StartPosY
        BranchColor = settingForm.BranchColor
        LeavesColor = settingForm.LeavesColor
        LengthScaling = settingForm.LengthScaling
        BranchVariation = settingForm.BranchVariation
        BranchStartThickness = settingForm.BranchStartThickness
        DeflectionAngle = settingForm.DeflectionAngle
        AllowRandom = settingForm.AllowRandom
        RandomPercentage = settingForm.RandomPercentage
        A_rule = settingForm.A_rule
        B_rule = settingForm.B_rule
        C_rule = settingForm.C_rule
        X_rule = settingForm.X_rule
        Y_rule = settingForm.Y_rule
        Z_rule = settingForm.Z_rule
        txtType.Text = FractalType
        Generate()
    End Sub

#Region "L-system routines"

    Private Sub Generate()
        Dim newString As String = ""
        Dim rnd As Random = New Random()
        MyString = InitialString
        Dim Length As Double = InitialLength * MyCanvas.ActualWidth
        Dim Angle As Integer = InitialAngle
        Me.Cursor = Cursors.Wait
        Do
            MyString = InitialString
            Length = InitialLength * MyCanvas.ActualWidth
            Angle = InitialAngle
            For I As Integer = 1 To MaxIter
                Length = LengthScaling * Length
                newString = ""
                For J As Integer = 0 To MyString.Length - 1
                    If MyString(J) = "A"c Then
                        newString &= A_rule
                    ElseIf MyString(J) = "B"c Then
                        newString &= B_rule
                    ElseIf MyString(J) = "C"c Then
                        newString &= C_rule
                    ElseIf MyString(J) = "X"c Then
                        If AllowRandom Then
                            If rnd.NextDouble() > RandomPercentage / 200 Then newString &= X_rule
                        Else
                            newString &= X_rule
                        End If
                    ElseIf MyString(J) = "Y"c Then
                            If AllowRandom Then
                                If rnd.NextDouble() > RandomPercentage / 200 Then newString &= Y_rule
                            Else
                                newString &= Y_rule
                            End If
                        ElseIf MyString(J) = "Z"c Then
                            If AllowRandom Then
                                If rnd.NextDouble() > RandomPercentage / 200 Then newString &= Z_rule
                            Else
                                newString &= Z_rule
                            End If
                        Else
                            newString &= MyString(J)
                    End If
                Next
                MyString = newString
            Next
        Loop While MyString.Length < 1
        Draw(MyString, Length, Angle)
        Me.Cursor = Cursors.Arrow
    End Sub

    Private Sub Draw(s As String, length As Double, angle As Double)
        Dim l As Line
        Dim e As Ellipse
        Dim rnd As Random = New Random()
        Dim randomlengthfactor As Double = 1.0
        Dim positions As New List(Of Point)
        Dim angles As New List(Of Double)
        Dim distance As Double = 0.0
        Dim thickness As Double = 1.0
        Dim nodecounter As Integer = 1
        Dim leaves As New List(Of Ellipse)
        Dim startPt As New Point()
        Dim endPt As New Point With {
            .X = StartPosX * MyCanvas.ActualWidth,
            .Y = StartPosY * MyCanvas.ActualHeight}
        MyCanvas.Children.Clear()
        For I As Integer = 0 To s.Length() - 1
            If s(I) = "A"c Or s(I) = "B"c Or s(I) = "C"c Then
                startPt = endPt
                If AllowRandom Then 'Random branch length
                    randomlengthfactor = 0.2 + 1.6 * rnd.NextDouble()
                Else
                    randomlengthfactor = 1
                End If
                endPt.X = startPt.X + length * randomlengthfactor * Math.Cos(angle * Math.PI / 180)
                endPt.Y = startPt.Y + length * randomlengthfactor * Math.Sin(angle * Math.PI / 180)
                If BranchVariation Then 'Branch thicknes varies with distance from startpoint and node
                    distance = Math.Sqrt((StartPosX * MyCanvas.ActualWidth - endPt.X) ^ 2 + (StartPosY * MyCanvas.ActualHeight - endPt.Y) ^ 2)
                    thickness = BranchStartThickness * (1 - (nodecounter + 2) / MaxIter) * (1 - distance / MyCanvas.ActualHeight())
                Else
                    thickness = BranchStartThickness
                End If
                If thickness < 1.0 Then thickness = 1.0
                l = New Line With {
                .X1 = startPt.X,
                .Y1 = startPt.Y,
                .X2 = endPt.X,
                .Y2 = endPt.Y,
                .Stroke = BranchColor,
                .StrokeThickness = thickness
                }
                MyCanvas.Children.Add(l)
            ElseIf s(I) = "X"c Or s(I) = "Y"c Or s(I) = "Z"c Then
                If ShowLeaves Then 'Collect all leaves in a list
                    e = New Ellipse()
                    e.SetValue(Canvas.LeftProperty, endPt.X)
                    e.SetValue(Canvas.TopProperty, endPt.Y)
                    e.Height = LeavesSize
                    e.Width = LeavesSize
                    e.Fill = LeavesColor
                    leaves.Add(e)
                End If
            ElseIf s(I) = "-"c Then
                angle = (angle - DeflectionAngle) Mod 360
            ElseIf s(I) = "+"c Then
                angle = (angle + DeflectionAngle) Mod 360
            ElseIf s(I) = "["c Then
                positions.Add(endPt)
                If AllowRandom And rnd.NextDouble < RandomPercentage / 50 Then
                    angles.Add(angle + (0.35 - 0.7 * rnd.NextDouble()) * DeflectionAngle)
                Else
                    angles.Add(angle)
                End If
                nodecounter += 1
            ElseIf s(I) = "]"c Then
                endPt = positions.Last
                angle = angles.Last
                positions.RemoveAt(positions.Count - 1)
                angles.RemoveAt(angles.Count - 1)
                nodecounter -= 1
            End If
        Next
        If ShowLeaves Then
            For I As Integer = 0 To leaves.Count - 1 'Show the leaves last
                MyCanvas.Children.Add(leaves(I))
            Next
        End If
    End Sub

    Private Sub Window_Closed(sender As Object, e As EventArgs)
        If settingForm IsNot Nothing Then settingForm.Close()
        End
    End Sub

#End Region

End Class
