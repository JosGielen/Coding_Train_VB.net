Imports System.Windows.Threading
Imports System.Threading
Imports System.IO
Imports Microsoft.Win32
Imports System.Text

Class Window1
    'Program members
    Private Delegate Sub RenderDelegate()
    Private pf As PixelFormat = PixelFormats.Rgb24
    Private VeldWidth As Integer = 0
    Private VeldHeight As Integer = 0
    Private ImageWidth As Integer = 0
    Private ImageHeight As Integer = 0
    Private Stride As Integer = 0
    Private pixelData As Byte()
    Private whiteArray As Byte()
    Private AppRunning As Boolean = False
    Private AppLoaded As Boolean = False
    Private EditMode As Boolean = False
    Private myGen As Generation
    Private StartGen As Generation
    Private Modified As Boolean
    Private GenName As String 'Path + Filename + extension

#Region "Initialisation"
    Private Sub Init()
        'Image is 2x Veld size (we draw 4 pixels per cell)
        ImageWidth = 2 * VeldWidth
        ImageHeight = 2 * VeldHeight
        Stride = CInt((ImageWidth * pf.BitsPerPixel + 7) / 8)
        Image1.Width = ImageWidth
        Image1.Height = ImageHeight
        'Resize de arrays
        ReDim pixelData(Stride * ImageHeight)
        ReDim whiteArray(Stride * ImageHeight)
        'Vul de white Array met Witte pixels
        For x = 0 To ImageWidth - 1
            For y = 0 To ImageHeight - 1
                SetPixel(x, y, Color.FromRgb(255, 255, 255), whiteArray, Stride)
            Next
        Next
        'Create new generations 
        myGen = New Generation(VeldWidth, VeldHeight)
        StartGen = New Generation(VeldWidth, VeldHeight)
        AppRunning = False
        Modified = False
        EditMode = False
        GenName = ""
        Me.Title = "Game of Life: Initialized"
        Render()
    End Sub
#End Region

#Region "Window Events"
    Private Sub Window1_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        VeldWidth = 200
        VeldHeight = 200
        Me.Width = 410  '2 * VeldWidth + 2* Window Borderthickness
        Me.Height = 454 '2 * VeldHeight + 2 * Window Borderthickenss + Menu Height + Window Titlebar Height
        AppLoaded = True
        Init()
    End Sub

    Private Sub Window1_SizeChanged(ByVal sender As System.Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles MyBase.SizeChanged
        If AppLoaded Then
            VeldWidth = CInt((Me.Width - 10) / 2)
            VeldHeight = CInt((Me.Height - 54) / 2)
            Init()
        End If
    End Sub

    Private Sub Window1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseMove
        Dim X As Integer
        Dim Y As Integer
        If EditMode = True Then
            X = CInt(e.GetPosition(Image1).X / 2)
            Y = CInt(e.GetPosition(Image1).Y / 2)
            If X >= 0 And X < myGen.Breedte And Y >= 0 And Y < myGen.Hoogte Then
                Me.Title = "Game of Life: Edit mode (" & X.ToString() & " , " & Y.ToString() & ")"
            End If
        End If
    End Sub

    Private Sub Window1_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        Dim X As Integer
        Dim Y As Integer
        If EditMode = True Then
            X = CInt(e.GetPosition(Image1).X / 2)
            Y = CInt(e.GetPosition(Image1).Y / 2)
            If X >= 0 And X < myGen.Breedte And Y >= 0 And Y < myGen.Hoogte Then
                myGen.Cell(X, Y) = True
                Modified = True
                Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New RenderDelegate(AddressOf Render))
            End If
        End If
    End Sub

    Private Sub Window1_MouseRightButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseRightButtonUp
        Dim X As Integer
        Dim Y As Integer
        If EditMode = True Then
            X = CInt(e.GetPosition(Image1).X / 2)
            Y = CInt(e.GetPosition(Image1).Y / 2)
            If X >= 0 And X < myGen.Breedte And Y >= 0 And Y < myGen.Hoogte Then
                myGen.Cell(X, Y) = False
                Modified = True
                Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New RenderDelegate(AddressOf Render))
            End If
        End If
    End Sub

    Private Sub Window1_Closed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        AppLoaded = False
        End
    End Sub
#End Region

#Region "Menu Events"
    Private Sub MenuNew_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuNew.Click
        If Modified = True Then
            'Request for a save
            If MessageBox.Show("Do you want to save this Game of Life?", "Game of Life", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.None) = MessageBoxResult.Yes Then
                MenuSaveAs_Click(sender, e)
            End If
        End If
        myGen.Clear()
        GenName = ""
        Me.Title = "Game of Life: New"
        Modified = False
        EditMode = False
        Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New RenderDelegate(AddressOf Render))
    End Sub

    Private Sub MenuOpen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuOpen.Click
        Dim myStream As StreamReader = Nothing
        Dim fileWidth As Integer = 0
        Dim fileHeight As Integer = 0
        Dim myRow As String
        Dim c() As Char
        Dim X, Y As Integer
        Dim openFileDialog1 As New OpenFileDialog()
        openFileDialog1.InitialDirectory = Environment.CurrentDirectory
        openFileDialog1.Multiselect = False
        openFileDialog1.DefaultExt = ".gol"
        openFileDialog1.Filter = "Game of Life files (*.gol)|*.gol|All files (*.*)|*.*"
        openFileDialog1.FilterIndex = 1
        openFileDialog1.RestoreDirectory = True
        If openFileDialog1.ShowDialog() Then
            Try
                myStream = New StreamReader(openFileDialog1.OpenFile())
                If (myStream IsNot Nothing) Then
                    GenName = openFileDialog1.FileName
                    myGen.Clear()
                    Y = 0
                    'Lees de afmetingen van de Game of Life in de file
                    fileWidth = Integer.Parse(myStream.ReadLine())
                    fileHeight = Integer.Parse(myStream.ReadLine())
                    'Pas het veld aan voor deze afmetingen
                    Me.Width = 2 * fileWidth + 10
                    Me.Height = 2 * fileHeight + 54
                    'Lees de Game of Life Data
                    'Iedere levende Cell werd weggeschreven als een 1 of een X
                    While Not myStream.EndOfStream
                        myRow = myStream.ReadLine()
                        If Y < myGen.Hoogte Then
                            'parse the row into chars and fill the Generation
                            c = myRow.ToCharArray()
                            For X = 0 To c.GetLength(0) - 1
                                If X < myGen.Breedte Then
                                    If c(X) = "1" Or c(X) = "X" Then
                                        myGen.Cell(X, Y) = True
                                    Else
                                        myGen.Cell(X, Y) = False
                                    End If
                                End If

                            Next
                            Y = Y + 1
                        End If
                    End While
                    Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New RenderDelegate(AddressOf Render))
                    Modified = False
                    EditMode = False
                    Me.Title = "Game of Life: " & Path.GetFileName(GenName)
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

    Private Sub MenuSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuSave.Click
        If GenName = "" Then
            'Request filename via SaveAs
            MenuSaveAs_Click(sender, e)
        Else
            Dim sb As New StringBuilder()
            Dim X, Y As Integer
            'fill a StringBuilder with the Cell data 
            For Y = 0 To myGen.Hoogte - 1
                For X = 0 To myGen.Breedte - 1
                    If myGen.Cell(X, Y) Then
                        sb.Append("X")
                    Else
                        sb.Append(".")
                    End If
                Next X
                sb.AppendLine()
            Next Y
            'Write the data to the File 
            Using outfile As New StreamWriter(GenName)
                'Schrijf de Veld afmetingen weg
                outfile.WriteLine(VeldWidth)
                outfile.WriteLine(VeldHeight)
                outfile.Write(sb.ToString())
            End Using
            Modified = False
            EditMode = False
            Me.Title = "Game of Life: " & Path.GetFileName(GenName)
        End If
    End Sub

    Private Sub MenuSaveAs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuSaveAs.Click
        Dim saveFileDialog1 As New SaveFileDialog()
        saveFileDialog1.InitialDirectory = Environment.CurrentDirectory
        saveFileDialog1.Filter = "Game of Life files (*.gol)|*.gol|All files (*.*)|*.*"
        saveFileDialog1.FilterIndex = 1
        saveFileDialog1.RestoreDirectory = True
        If saveFileDialog1.ShowDialog() Then
            GenName = saveFileDialog1.FileName
            Dim sb As New StringBuilder()
            Dim X, Y As Integer
            'fill a StringBuilder with the Cell data 
            For Y = 0 To myGen.Hoogte - 1
                For X = 0 To myGen.Breedte - 1
                    If myGen.Cell(X, Y) Then
                        sb.Append("X")
                    Else
                        sb.Append(".")
                    End If
                Next X
                sb.AppendLine()
            Next Y
            'Write the Data to the File 
            Using outfile As New StreamWriter(GenName)
                outfile.WriteLine(VeldWidth)
                outfile.WriteLine(VeldHeight)
                outfile.Write(sb.ToString())
            End Using
            Modified = False
            EditMode = False
            Me.Title = "Game of Life: " & Path.GetFileName(GenName)
        End If
    End Sub

    Private Sub MenuExit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuExit.Click
        If Modified = True Then
            'Request for a save
            If MessageBox.Show("Do you want to save this Game of Life?", "Game of Life", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.None) = MessageBoxResult.Yes Then
                MenuSaveAs_Click(sender, e)
            End If
        End If
        End
    End Sub

    Private Sub Menu100_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu100.Click
        Me.Width = 210
        Me.Height = 254
        Menu200.IsChecked = False
        Menu300.IsChecked = False
        Menu400.IsChecked = False
    End Sub

    Private Sub Menu200_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu200.Click
        Me.Width = 410
        Me.Height = 454
        Menu100.IsChecked = False
        Menu300.IsChecked = False
        Menu400.IsChecked = False
    End Sub

    Private Sub Menu300_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu300.Click
        Me.Width = 610
        Me.Height = 654
        Menu100.IsChecked = False
        Menu200.IsChecked = False
        Menu400.IsChecked = False
    End Sub

    Private Sub Menu400_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu400.Click
        Me.Width = 810
        Me.Height = 854
        Menu100.IsChecked = False
        Menu200.IsChecked = False
        Menu300.IsChecked = False
    End Sub

    Private Sub MenuClear_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuClear.Click
        EditMode = False
        Modified = False
        myGen.Clear()
        If GenName <> "" Then
            Me.Title = "Game of Life: Cleared " & Path.GetFileName(GenName)
        Else
            Me.Title = "Game of Life: Cleared"
        End If
        Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New RenderDelegate(AddressOf Render))
    End Sub

    Private Sub MenuEdit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuEdit.Click
        AppRunning = False
        EditMode = True
    End Sub

    Private Sub MenuCreate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuCreate.Click
        Dim CP As CreatePattern = New CreatePattern()
        CP.Show()
    End Sub

    Private Sub MenuInsert_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuInsert.Click
        'Step1: Open a file dialog to select a pattern
        Dim myStream As StreamReader = Nothing
        Dim cd As CoordinateDialog = New CoordinateDialog()
        Dim myRow As String
        Dim TestGen As Generation
        Dim c() As Char
        Dim X As Integer = 0
        Dim Y As Integer = 0
        Dim MinX As Integer = 0
        Dim MinY As Integer = 0
        Dim MaxX As Integer = 0
        Dim MaxY As Integer = 0
        Dim OriginX As Integer = 0
        Dim OriginY As Integer = 0
        Dim openFileDialog1 As New OpenFileDialog()
        openFileDialog1.InitialDirectory = Environment.CurrentDirectory
        openFileDialog1.Multiselect = False
        openFileDialog1.DefaultExt = ".pat"
        openFileDialog1.Filter = "Pattern files (*.pat)|*.pat|All files (*.*)|*.*"
        openFileDialog1.FilterIndex = 1
        openFileDialog1.RestoreDirectory = True
        If openFileDialog1.ShowDialog() Then
            Try
                myStream = New StreamReader(openFileDialog1.OpenFile())
                If (myStream IsNot Nothing) Then
                    Y = 0
                    'Lees de afmetingen van de Game of Life in de file
                    myStream.ReadLine()
                    myStream.ReadLine()
                    MinX = Integer.Parse(myStream.ReadLine())
                    MinY = Integer.Parse(myStream.ReadLine())
                    MaxX = Integer.Parse(myStream.ReadLine())
                    MaxY = Integer.Parse(myStream.ReadLine())
                    TestGen = New Generation(MaxX - MinX + 1, MaxY - MinY + 1)
                    'Lees de Pattern Data in testGen
                    'Iedere levende Cell werd weggeschreven als een 1 of een X
                    While Not myStream.EndOfStream
                        myRow = myStream.ReadLine()
                        If Y < TestGen.Hoogte Then
                            'parse the row into chars and fill TestGen
                            c = myRow.ToCharArray()
                            For X = 0 To c.GetLength(0) - 1
                                If X < TestGen.Breedte Then
                                    If c(X) = "1" Or c(X) = "X" Then
                                        TestGen.Cell(X, Y) = True
                                    Else
                                        TestGen.Cell(X, Y) = False
                                    End If
                                End If
                            Next
                            Y = Y + 1
                        End If
                    End While
                    'Step2: Ask for the coordinates of the origin (=top left corner) of the pattern.
                    If cd.ShowDialog() Then
                        OriginX = CInt(cd.Getcoordinate().X)
                        OriginY = CInt(cd.Getcoordinate().Y)
                        If OriginX + MaxX - MinX > VeldWidth Or OriginY + MaxY - MinY > VeldHeight Then
                            MessageBox.Show("The pattern does not fit into the Field!", "Game of Life Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK)
                        Else
                            'Copy TestGen into myGen at the origin position
                            For X = 0 To TestGen.Breedte - 1
                                For Y = 0 To TestGen.Hoogte - 1
                                    myGen.Cell(OriginX + X, OriginY + Y) = TestGen.Cell(X, Y)
                                Next
                            Next
                            Modified = True
                            Render()
                        End If
                    End If
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

    Private Sub MenuSingle_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuSingle.Click
        EditMode = False
        Modified = False
        myGen.update()
        Me.Title = "Game of Life : " & Path.GetFileName(GenName) & " Generation " & myGen.Volgnummer.ToString
        Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New RenderDelegate(AddressOf Render))
    End Sub

    Private Sub MenuStart_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuStart.Click
        'Set menu item availability
        MenuStart.IsEnabled = False
        MenuReset.IsEnabled = False
        MenuClear.IsEnabled = False
        MenuSize.IsEnabled = False
        MenuCreate.IsEnabled = False
        MenuInsert.IsEnabled = False
        MenuEdit.IsEnabled = False
        MenuSingle.IsEnabled = False
        MenuStop.IsEnabled = True
        MenuFile.IsEnabled = False
        'Copy MyGen to StartGen
        For X As Integer = 0 To myGen.Breedte - 1
            For Y As Integer = 0 To myGen.Hoogte - 1
                StartGen.Cell(X, Y) = myGen.Cell(X, Y)
            Next Y
        Next X
        'Start the game and render while the application is Idle
        'This allows other events to stop the Do loop
        AppRunning = True
        EditMode = False
        Modified = False
        If GenName <> "" Then
            Me.Title = "Game of Life: " & Path.GetFileName(GenName)
        Else
            Me.Title = "Game of Life"
        End If
        Do While AppRunning
            myGen.update()
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New RenderDelegate(AddressOf Render))
        Loop
    End Sub

    Private Sub MenuStop_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuStop.Click
        AppRunning = False
        MenuStart.IsEnabled = True
        MenuReset.IsEnabled = True
        MenuClear.IsEnabled = True
        MenuSize.IsEnabled = True
        MenuCreate.IsEnabled = True
        MenuInsert.IsEnabled = True
        MenuEdit.IsEnabled = True
        MenuSingle.IsEnabled = True
        MenuStop.IsEnabled = False
        MenuFile.IsEnabled = True
    End Sub

    Private Sub MenuReset_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuReset.Click
        'Copy StartGen back to MyGen
        For X As Integer = 0 To StartGen.Breedte - 1
            For Y As Integer = 0 To StartGen.Hoogte - 1
                myGen.Cell(X, Y) = StartGen.Cell(X, Y)
            Next Y
        Next X
        EditMode = False
        Modified = False
        Render()
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
        'Fill the pixelData array with white pixels
        Array.Copy(whiteArray, pixelData, pixelData.Length)
        'Fill the buffer with the pixels that need drawing in black
        For X As Integer = 0 To VeldWidth - 1
            For Y As Integer = 0 To VeldHeight - 1
                If myGen.Cell(X, Y) Then
                    SetPixel(2 * X, 2 * Y, Color.FromRgb(0, 0, 0), pixelData, Stride)
                    SetPixel(2 * X + 1, 2 * Y, Color.FromRgb(0, 0, 0), pixelData, Stride)
                    SetPixel(2 * X, 2 * Y + 1, Color.FromRgb(0, 0, 0), pixelData, Stride)
                    SetPixel(2 * X + 1, 2 * Y + 1, Color.FromRgb(0, 0, 0), pixelData, Stride)
                End If
            Next Y
        Next X
        Dim bitmap As BitmapSource = BitmapSource.Create(ImageWidth, ImageHeight, 96, 96, pf, Nothing, pixelData, Stride)
        Image1.Source = bitmap
        Thread.Sleep(40)
    End Sub
#End Region

End Class
