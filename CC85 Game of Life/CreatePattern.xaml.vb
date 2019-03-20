Imports System.IO
Imports Microsoft.Win32
Imports System.Text

Class CreatePattern
    Private VeldWidth As Integer
    Private VeldHeight As Integer
    Private CelWidth As Double
    Private CelHeight As Double
    Private ColNum As Integer = 20
    Private RowNum As Integer = 20
    Private Modified As Boolean = False
    Private PatternName As String = ""
    Private myGen As Generation
    Private StartGen As Generation

#Region "Initialisation"
    Private Sub init()
        VeldWidth = CInt(Canvas1.ActualWidth)
        VeldHeight = CInt(Canvas1.ActualHeight)
        CelWidth = VeldWidth / ColNum
        CelHeight = VeldHeight / RowNum
        Canvas1.Children.Clear()
        'Create new generations 
        myGen = New Generation(ColNum, RowNum)
        StartGen = New Generation(ColNum, RowNum)
        DrawGrid()
    End Sub

    Private Sub DrawGrid()
        Dim gridLine As Line
        'Draw border of the canvas
        gridLine = New Line()
        gridLine.Stroke = Brushes.LightGray
        gridLine.X1 = 0
        gridLine.Y1 = 0
        gridLine.X2 = VeldWidth
        gridLine.Y2 = 0
        Canvas1.Children.Add(gridLine)
        gridLine = New Line()
        gridLine.Stroke = Brushes.LightGray
        gridLine.X1 = VeldWidth
        gridLine.Y1 = 0
        gridLine.X2 = VeldWidth
        gridLine.Y2 = VeldHeight
        Canvas1.Children.Add(gridLine)
        gridLine = New Line()
        gridLine.Stroke = Brushes.LightGray
        gridLine.X1 = 0
        gridLine.Y1 = 0
        gridLine.X2 = 0
        gridLine.Y2 = VeldHeight
        Canvas1.Children.Add(gridLine)
        gridLine = New Line()
        gridLine.Stroke = Brushes.LightGray
        gridLine.X1 = 0
        gridLine.Y1 = VeldHeight
        gridLine.X2 = VeldWidth
        gridLine.Y2 = VeldHeight
        Canvas1.Children.Add(gridLine)
        'Draw Vertical gridlines
        For I As Integer = 0 To ColNum - 2
            gridLine = New Line()
            gridLine.Stroke = Brushes.LightGray
            gridLine.X1 = CelWidth * (I + 1)
            gridLine.Y1 = 0
            gridLine.X2 = CelWidth * (I + 1)
            gridLine.Y2 = VeldHeight
            Canvas1.Children.Add(gridLine)
        Next
        'Draw Horizontal gridlines
        For I As Integer = 0 To RowNum - 2
            gridLine = New Line()
            gridLine.Stroke = Brushes.LightGray
            gridLine.X1 = 0
            gridLine.Y1 = CelHeight * (I + 1)
            gridLine.X2 = VeldWidth
            gridLine.Y2 = CelHeight * (I + 1)
            Canvas1.Children.Add(gridLine)
        Next
    End Sub
#End Region

#Region "Window Events"
    Private Sub CreatePattern_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        Dim X As Integer
        Dim Y As Integer
        X = CInt(Math.Floor(e.GetPosition(Canvas1).X / CelWidth))
        Y = CInt(Math.Floor(e.GetPosition(Canvas1).Y / CelHeight))
        If X >= 0 And X < myGen.Breedte And Y >= 0 And Y < myGen.Hoogte Then
            myGen.Cell(X, Y) = True
            Modified = True
            Render()
        End If
    End Sub

    Private Sub CreatePattern_MouseRightButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseRightButtonUp
        Dim X As Integer
        Dim Y As Integer
        X = CInt(Math.Floor(e.GetPosition(Canvas1).X / CelWidth))
        Y = CInt(Math.Floor(e.GetPosition(Canvas1).Y / CelHeight))
        If X >= 0 And X < myGen.Breedte And Y >= 0 And Y < myGen.Hoogte Then
            myGen.Cell(X, Y) = False
            Modified = True
            Render()
        End If
    End Sub

    Private Sub CreatePattern_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles Me.SizeChanged
        init()
    End Sub
#End Region

#Region "Menu Events"
    Private Sub Menu10x10_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu10x10.Click
        ColNum = 10
        RowNum = 10
        Menu20x20.IsChecked = False
        Menu40x40.IsChecked = False
        Menu60x60.IsChecked = False
        Me.Width = 12 * ColNum + 8
        Me.Height = 12 * RowNum + 99
    End Sub

    Private Sub Menu20x20_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu20x20.Click
        ColNum = 20
        RowNum = 20
        Menu10x10.IsChecked = False
        Menu40x40.IsChecked = False
        Menu60x60.IsChecked = False
        Me.Width = 12 * ColNum + 8
        Me.Height = 12 * RowNum + 99
    End Sub

    Private Sub Menu40x40_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu40x40.Click
        ColNum = 40
        RowNum = 40
        Menu10x10.IsChecked = False
        Menu20x20.IsChecked = False
        Menu60x60.IsChecked = False
        Me.Width = 12 * ColNum + 8
        Me.Height = 12 * RowNum + 99
    End Sub

    Private Sub Menu60x60_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Menu60x60.Click
        ColNum = 60
        RowNum = 60
        Menu10x10.IsChecked = False
        Menu20x20.IsChecked = False
        Menu40x40.IsChecked = False
        Me.Width = 12 * ColNum + 8
        Me.Height = 12 * RowNum + 99
    End Sub

    Private Sub MenuVert_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuVert.Click
        Dim b As Integer = myGen.Breedte - 1
        Dim h As Integer = myGen.Hoogte - 1
        Dim TestGen As Generation = New Generation(myGen.Breedte, myGen.Hoogte)
        'flip myGen into TestGen
        For Y As Integer = 0 To h
            For X As Integer = 0 To b
                TestGen.Cell(b - X, Y) = myGen.Cell(X, Y)
            Next
        Next
        'Copy TestGen back to myGen
        For Y As Integer = 0 To h
            For X As Integer = 0 To b
                myGen.Cell(X, Y) = TestGen.Cell(X, Y)
            Next
        Next
        Render()
    End Sub

    Private Sub MenuHori_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuHori.Click
        Dim b As Integer = myGen.Breedte - 1
        Dim h As Integer = myGen.Hoogte - 1
        Dim TestGen As Generation = New Generation(myGen.Breedte, myGen.Hoogte)
        'flip myGen into TestGen
        For X As Integer = 0 To b
            For Y As Integer = 0 To h
                TestGen.Cell(X, h - Y) = myGen.Cell(X, Y)
            Next
        Next
        'Copy TestGen back to myGen
        For Y As Integer = 0 To h
            For X As Integer = 0 To b
                myGen.Cell(X, Y) = TestGen.Cell(X, Y)
            Next
        Next
        Render()
    End Sub

    Private Sub MenuOpen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuOpen.Click
        Dim myStream As StreamReader = Nothing
        Dim myRow As String
        Dim c() As Char
        Dim X, Y As Integer
        Dim MinX As Integer = 0
        Dim MinY As Integer = 0
        Dim MaxX As Integer = 0
        Dim MaxY As Integer = 0
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
                    PatternName = openFileDialog1.FileName
                    myGen.Clear()
                    Y = 0
                    'Lees de afmetingen van de Game of Life in de file
                    ColNum = Integer.Parse(myStream.ReadLine())
                    RowNum = Integer.Parse(myStream.ReadLine())
                    MinX = Integer.Parse(myStream.ReadLine())
                    MinY = Integer.Parse(myStream.ReadLine())
                    MaxX = Integer.Parse(myStream.ReadLine())
                    MaxY = Integer.Parse(myStream.ReadLine())
                    'Pas het veld aan voor deze afmetingen
                    Me.Width = 12 * ColNum + 8
                    Me.Height = 12 * RowNum + 99
                    'Lees de Pattern Data
                    'Iedere levende Cell werd weggeschreven als een 1 of een X
                    While Not myStream.EndOfStream
                        myRow = myStream.ReadLine()
                        If MinY + Y < myGen.Hoogte Then
                            'parse the row into chars and fill the Generation
                            c = myRow.ToCharArray()
                            For X = 0 To c.GetLength(0) - 1
                                If MinX + X < myGen.Breedte Then
                                    If c(X) = "1" Or c(X) = "X" Then
                                        myGen.Cell(MinX + X, MinY + Y) = True
                                    Else
                                        myGen.Cell(MinX + X, MinY + Y) = False
                                    End If
                                End If
                            Next
                            Y = Y + 1
                        End If
                    End While
                    Render()
                    Modified = False
                    Me.Title = "Pattern: " & Path.GetFileName(PatternName)
                    Menu10x10.IsChecked = False
                    Menu20x20.IsChecked = False
                    Menu40x40.IsChecked = False
                    Menu60x60.IsChecked = False
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
        If PatternName = "" Then
            'Request filename via SaveAs
            MenuSaveAs_Click(sender, e)
        Else
            Dim sb As New StringBuilder()
            Dim MinX As Integer = ColNum
            Dim MinY As Integer = RowNum
            Dim MaxX As Integer = 0
            Dim MaxY As Integer = 0
            'Determine the pattern origin and size
            For X As Integer = 0 To ColNum - 1
                For Y As Integer = 0 To RowNum - 1
                    If myGen.Cell(X, Y) Then
                        If X < MinX Then MinX = X
                        If Y < MinY Then MinY = Y
                        If X > MaxX Then MaxX = X
                        If Y > MaxY Then MaxY = Y
                    End If
                Next
            Next
            'fill a StringBuilder with the Cell data 
            For Y As Integer = MinY To MaxY
                For X As Integer = MinX To MaxX
                    If myGen.Cell(X, Y) Then
                        sb.Append("X")
                    Else
                        sb.Append(".")
                    End If
                Next X
                sb.AppendLine()
            Next Y
            'Write the data to the File 
            Using outfile As New StreamWriter(PatternName)
                'Schrijf de Veld afmetingen weg
                outfile.WriteLine(ColNum)
                outfile.WriteLine(RowNum)
                outfile.WriteLine(MinX)
                outfile.WriteLine(MinY)
                outfile.WriteLine(MaxX)
                outfile.WriteLine(MaxY)
                outfile.Write(sb.ToString())
            End Using
            Modified = False
            Me.Title = "Pattern: " & Path.GetFileName(PatternName)
        End If
    End Sub

    Private Sub MenuSaveAs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuSaveAs.Click
        Dim saveFileDialog1 As New SaveFileDialog()
        saveFileDialog1.InitialDirectory = Environment.CurrentDirectory
        saveFileDialog1.DefaultExt = ".pat"
        saveFileDialog1.Filter = "Pattern files (*.pat)|*.pat|All files (*.*)|*.*"
        saveFileDialog1.FilterIndex = 1
        saveFileDialog1.RestoreDirectory = True
        If saveFileDialog1.ShowDialog() Then
            PatternName = saveFileDialog1.FileName
            Dim sb As New StringBuilder()
            Dim MinX As Integer = ColNum
            Dim MinY As Integer = RowNum
            Dim MaxX As Integer = 0
            Dim MaxY As Integer = 0
            'Determine the pattern origin and size
            For X As Integer = 0 To ColNum - 1
                For Y As Integer = 0 To RowNum - 1
                    If myGen.Cell(X, Y) Then
                        If X < MinX Then MinX = X
                        If Y < MinY Then MinY = Y
                        If X > MaxX Then MaxX = X
                        If Y > MaxY Then MaxY = Y
                    End If
                Next
            Next
            'fill a StringBuilder with the Cell data
            For Y As Integer = MinY To MaxY
                For X As Integer = MinX To MaxX
                    If myGen.Cell(X, Y) Then
                        sb.Append("X")
                    Else
                        sb.Append(".")
                    End If
                Next X
                sb.AppendLine()
            Next Y
            'Write the Data to the File 
            Using outfile As New StreamWriter(PatternName)
                outfile.WriteLine(ColNum)
                outfile.WriteLine(RowNum)
                outfile.WriteLine(MinX)
                outfile.WriteLine(MinY)
                outfile.WriteLine(MaxX)
                outfile.WriteLine(MaxY)
                outfile.Write(sb.ToString())
            End Using
            Modified = False
            Me.Title = "Pattern: " & Path.GetFileName(PatternName)
        End If
    End Sub

    Private Sub MenuExit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuExit.Click
        Me.Close()
        'TODO : Ask for save pattern?
    End Sub

    Private Sub MenuClear_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuClear.Click
        Modified = False
        myGen.Clear()
        StartGen.Clear()
        Render()
    End Sub

    Private Sub MenuReset_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuReset.Click
        'Copy StartGen back to MyGen
        For X As Integer = 0 To StartGen.Breedte - 1
            For Y As Integer = 0 To StartGen.Hoogte - 1
                myGen.Cell(X, Y) = StartGen.Cell(X, Y)
            Next Y
        Next X
        Modified = False
        Render()
    End Sub
#End Region

#Region "Business Code"
    Private Sub BtnStep_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnStep.Click
        If Modified Then
            'Copy MyGen to StartGen
            For X As Integer = 0 To myGen.Breedte - 1
                For Y As Integer = 0 To myGen.Hoogte - 1
                    StartGen.Cell(X, Y) = myGen.Cell(X, Y)
                Next Y
            Next X
        End If
        Modified = False
        myGen.update()
        Render()
    End Sub

    Private Sub Render()
        Dim rect As Rectangle
        Canvas1.Children.Clear()
        DrawGrid()
        For X As Integer = 0 To ColNum - 1
            For Y As Integer = 0 To RowNum - 1
                If myGen.Cell(X, Y) Then
                    rect = New Rectangle()
                    rect.Width = CelWidth - 2
                    rect.Height = CelHeight - 2
                    rect.Stroke = Brushes.Black
                    rect.Fill = Brushes.Black
                    rect.SetValue(Canvas.TopProperty, Y * CelHeight + 1)
                    rect.SetValue(Canvas.LeftProperty, X * CelWidth + 1)
                    Canvas1.Children.Add(rect)
                End If
            Next
        Next
    End Sub
#End Region

End Class
