Imports System.Windows.Threading

Class MainWindow
    'Program members
    Private Delegate Sub MoveDelegate()
    Private AppLoaded As Boolean = False
    Private GameStarted As Boolean = False
    Private RowNum As Integer
    Private ColNum As Integer
    Private CelWidth As Double = 0.0
    Private CelHeight As Double = 0.0
    Private Markers() As Marker
    Private PlayerCount As Integer
    Private CurrentPlayer As Integer
    Private StartingPlayer As Integer
    Private Player1 As String
    Private Player2 As String
    Private myCol As Integer = 0
    Private myGameMode As GameMode
    Private myDepth As Integer
    Private Rnd As Random = New Random()

#Region "Window Events"
    Private Sub Window1_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        'Default start with 1 player
        PlayerCount = 1
        Player1 = "Player"
        Player2 = "Computer"
        'Default start with 6 rows and 7 columns in normal difficulty mode
        RowNum = 6
        ColNum = 7
        myGameMode = GameMode.Normal
        myDepth = 3
        ReDim Markers(RowNum * ColNum - 1)
        For I As Integer = 0 To ColNum * RowNum - 1
            Markers(I) = New Marker()
        Next
        Width = 60 * ColNum
        Height = 60 * RowNum + 75
        DrawGame()
        'Show a settings dialog to allow modifications
        ShowSettings()
        'Determine who does the first move.
        If Rnd.Next(100) < 50 Then
            StartingPlayer = 1
        Else
            StartingPlayer = 2
        End If
        CurrentPlayer = StartingPlayer
        GameStarted = True
        AppLoaded = True
    End Sub

    Private Sub Window1_SizeChanged(ByVal sender As System.Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles MyBase.SizeChanged
        If AppLoaded Then
            DrawGame()
        End If
    End Sub

    Private Sub Window1_Closed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        End
    End Sub

#End Region

#Region "Menu Events"

    Private Sub MenuSave_Click(sender As Object, e As RoutedEventArgs)
        'TODO Save the current game state
    End Sub

    Private Sub MenuLoad_Click(sender As Object, e As RoutedEventArgs)
        'TODO Load a saved game
    End Sub

    Private Sub MenuExit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        End
    End Sub

    Private Sub MnuSettings_Click(sender As Object, e As RoutedEventArgs)
        ShowSettings()
    End Sub

    Private Sub ShowSettings()
        Dim gs As GameSettings
        Dim gameData As Settings = New Settings() With
        {
            .PlayerCount = PlayerCount,
            .PlayerName1 = Player1,
            .PlayerName2 = Player2,
            .GameSize = New Size(ColNum, RowNum),
            .GameMode = myGameMode
        }
        gs = New GameSettings(gameData, Me)
        gs.Left = Left + ActualWidth
        gs.Top = Top
        gs.ShowDialog()
        If gs.DialogResult = True Then
            PlayerCount = gs.Playeraantal
            Player1 = gs.Player1
            Player2 = gs.Player2
            RowNum = CInt(gs.GameSize.Height)
            ColNum = CInt(gs.GameSize.Width)
            myGameMode = gs.GameMode
            If myGameMode = GameMode.Easy Then myDepth = 2
            If myGameMode = GameMode.Normal Then myDepth = 3
            If myGameMode = GameMode.Hard Then myDepth = 5
            ReDim Markers(RowNum * ColNum - 1)
            For I As Integer = 0 To ColNum * RowNum - 1
                Markers(I) = New Marker()
            Next
            Width = 60 * ColNum
            Height = 60 * RowNum + 75
            GameStarted = True
        End If
        If PlayerCount = 1 Then
            If CurrentPlayer = 2 Then 'The Computer starts in a random column
                myCol = Rnd.Next(ColNum)
                Markers(GetFreeIndex(myCol)).PlayerNr = 2
                CurrentPlayer = 1
            End If
            TxtStatus.Text = Player1 & " Please make your move."
        Else
            If CurrentPlayer = 1 Then
                TxtStatus.Text = Player1 & " please make your move."
            Else
                TxtStatus.Text = Player2 & " please make your move."
            End If
        End If
        DrawGame()
    End Sub

    Private Sub MenuHint_Click(sender As Object, e As RoutedEventArgs)
        'TODO Display a hint.
    End Sub

    Private Sub MenuAnalyse_Click(sender As Object, e As RoutedEventArgs)
        'TODO Analyse the current game state.
    End Sub

    Private Sub MenuStartNew_Click(sender As Object, e As RoutedEventArgs)
        'Clear all markers
        For I As Integer = 0 To ColNum * RowNum - 1
            Markers(I) = New Marker()
        Next
        DrawGame()
        'The other player will now start the game
        If StartingPlayer = 1 Then
            StartingPlayer = 2
        Else
            StartingPlayer = 1
        End If
        CurrentPlayer = StartingPlayer
        If PlayerCount = 1 Then
            If CurrentPlayer = 2 Then 'The Computer starts in a random column
                myCol = Rnd.Next(ColNum)
                Markers(GetFreeIndex(myCol)).PlayerNr = 2
                CurrentPlayer = 1
            End If
            TxtStatus.Text = Player1 & " Please make your move."
        Else
            If CurrentPlayer = 1 Then
                TxtStatus.Text = Player1 & " please make your move."
            Else
                TxtStatus.Text = Player2 & " please make your move."
            End If
        End If
        GameStarted = True
    End Sub

#End Region

#Region "Business Methods"

    Private Sub DrawGame()
        Dim gridLine As Line
        Dim VeldWidth As Double
        Dim VeldHeight As Double
        VeldWidth = Canvas1.ActualWidth
        VeldHeight = Canvas1.ActualHeight
        CelWidth = VeldWidth / ColNum
        CelHeight = VeldHeight / RowNum
        'Adjust the Markers
        Canvas1.Children.Clear()
        For I As Integer = 0 To ColNum * RowNum - 1
            Markers(I).CelWidth = 0.9 * Me.CelWidth
            Markers(I).CelHeight = 0.9 * Me.CelHeight
            Markers(I).Row = CInt(Math.Floor(I / ColNum))
            Markers(I).Col = I Mod ColNum
        Next
        'Draw border of the canvas
        gridLine = New Line With
        {
            .Stroke = Brushes.LightGray,
            .X1 = 1,
            .Y1 = 0,
            .X2 = VeldWidth,
            .Y2 = 0
        }
        Canvas1.Children.Add(gridLine)
        gridLine = New Line With
        {
            .Stroke = Brushes.LightGray,
            .X1 = VeldWidth,
            .Y1 = 0,
            .X2 = VeldWidth,
            .Y2 = VeldHeight
        }
        Canvas1.Children.Add(gridLine)
        gridLine = New Line With
        {
            .Stroke = Brushes.LightGray,
            .X1 = 1,
            .Y1 = 0,
            .X2 = 1,
            .Y2 = VeldHeight
        }
        Canvas1.Children.Add(gridLine)
        gridLine = New Line With
        {
            .Stroke = Brushes.LightGray,
            .X1 = 1,
            .Y1 = VeldHeight,
            .X2 = VeldWidth,
            .Y2 = VeldHeight
        }
        Canvas1.Children.Add(gridLine)
        'Draw Vertical gridlines
        For I As Integer = 0 To ColNum - 2
            gridLine = New Line With
            {
                .Stroke = Brushes.LightGray,
                .X1 = CelWidth * (I + 1),
                .Y1 = 0,
                .X2 = CelWidth * (I + 1),
                .Y2 = VeldHeight
            }
            Canvas1.Children.Add(gridLine)
        Next
        'Draw Horizontal gridlines
        For I As Integer = 0 To RowNum - 2
            gridLine = New Line With
            {
                .Stroke = Brushes.LightGray,
                .X1 = 0,
                .Y1 = CelHeight * (I + 1),
                .X2 = VeldWidth,
                .Y2 = CelHeight * (I + 1)
            }
            Canvas1.Children.Add(gridLine)
        Next
        'Draw all markers
        For I As Integer = 0 To ColNum * RowNum - 1
            Dim X As Double = I Mod ColNum
            Dim Y As Double = Math.Truncate(I / ColNum)
            Markers(I).Draw(Canvas1, (X + 0.05) * CelWidth, (Y + 0.05) * CelHeight)
        Next
    End Sub

    Private Sub Canvas1_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        Dim Col As Integer = 0
        Dim FreeIndex As Integer = -1
        'Check if the game is started
        If Not GameStarted Then
            e.Handled = True
            Exit Sub
        End If
        'Determine the column where the player clicked
        For I As Integer = 0 To Markers.Length - 1
            If Markers(I).Shape.IsMouseOver Then
                Col = Markers(I).Col
                FreeIndex = GetFreeIndex(Col)
            End If
        Next
        If FreeIndex >= 0 Then
            If PlayerCount = 1 Then
                'Set the player marker
                Markers(FreeIndex).PlayerNr = 1
                If CheckVictory() Then Exit Sub
                'Get the computer move
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New MoveDelegate(AddressOf GetBestMove))
                FreeIndex = GetFreeIndex(myCol)
                If FreeIndex >= 0 Then
                    Markers(FreeIndex).PlayerNr = 2
                Else
                    Beep() 'The computer made an invalid move!!!
                End If
                If CheckVictory() Then Exit Sub
            ElseIf PlayerCount = 2 Then
                'Set the player marker
                Markers(FreeIndex).PlayerNr = CurrentPlayer
                If CheckVictory() Then Exit Sub
                'Switch to the next player
                If CurrentPlayer = 1 Then
                    TxtStatus.Text = Player2 & " please make your move."
                    CurrentPlayer = 2
                Else
                    TxtStatus.Text = Player1 & " please make your move."
                    CurrentPlayer = 1
                End If
            End If
        Else
            Beep() 'The player made an invalid move!!!
        End If
    End Sub

    Private Sub GetBestMove()
        Dim FreeIndex As Integer
        Dim score As Integer
        Dim bestscore As Integer = Integer.MinValue
        Dim bestcol As Integer = -1
        Cursor = Cursors.Wait
        For I As Integer = 0 To ColNum - 1
            FreeIndex = GetFreeIndex(I)
            If FreeIndex > 0 Then
                Markers(FreeIndex).PlayerNr = 2
                score = Minimax(myDepth, False)
                If score > bestscore Then
                    bestscore = score
                    bestcol = I
                End If
                Markers(FreeIndex).PlayerNr = 0
            End If
        Next
        myCol = bestcol
        Cursor = Cursors.Arrow
    End Sub

    ''' <summary>
    ''' Use the Minimax algorithm to estimate the best move
    ''' </summary>
    Private Function Minimax(depth As Integer, maximizing As Boolean) As Integer
        Dim FreeIndex As Integer
        Dim Score As Integer
        Dim Bestscore As Integer
        Dim validmoves As Integer = 0
        Dim result As Integer = 0
        If depth = 0 Or Check4(1) > 0 Or Check4(2) > 0 Then 'return the heuristic value of the game
            result = 5000 * Check4(2) + 100 * Check3(2) + 10 * Check2(2) - 3000 * Check4(1) - 100 * Check3(1) - 10 * Check2(1)
            Return result
        End If
        If maximizing Then
            Bestscore = Integer.MinValue
            For I As Integer = 0 To ColNum - 1
                FreeIndex = GetFreeIndex(I)
                If FreeIndex >= 0 Then
                    Markers(FreeIndex).PlayerNr = 2 'Computer = maximizing
                    Score = Minimax(depth - 1, False)
                    If Score > Bestscore Then
                        Bestscore = Score
                    End If
                    validmoves += 1
                    Markers(FreeIndex).PlayerNr = 0
                End If
            Next
            If validmoves > 0 Then
                result = Bestscore
            End If
            Return result
        Else
            Bestscore = Integer.MaxValue
            For I As Integer = 0 To ColNum - 1
                FreeIndex = GetFreeIndex(I)
                If FreeIndex >= 0 Then
                    Markers(FreeIndex).PlayerNr = 1 'Player = minimizing
                    Score = Minimax(depth - 1, True)
                    If Score < Bestscore Then
                        Bestscore = Score
                    End If
                    validmoves += 1
                    Markers(FreeIndex).PlayerNr = 0
                End If
            Next
            If validmoves > 0 Then
                result = Bestscore
            End If
            Return result
        End If
    End Function

    Private Function GetFreeIndex(Col As Integer) As Integer
        Dim index As Integer
        If Col < 0 Or Col >= ColNum Then Return -1
        For I As Integer = RowNum - 1 To 0 Step -1
            index = ColNum * I + Col
            If Markers(index).PlayerNr = 0 Then Return index
        Next
        Return -1
    End Function

    Private Function GetRow(ByVal Index As Integer) As Integer
        Return CInt(Math.Floor(Index / ColNum))
    End Function

    Private Function GetCol(ByVal Index As Integer) As Integer
        Return Index Mod ColNum
    End Function

    Private Function GetIndex(ByVal row As Integer, ByVal col As Integer) As Integer
        Return row * ColNum + col
    End Function

    ''' <summary>
    ''' Check if one of the players made 4 in a Row
    ''' </summary>
    ''' <returns></returns>
    Private Function CheckVictory() As Boolean
        Dim FreeCells As Integer = 0
        For I As Integer = 0 To Markers.Length - 1
            If Markers(I).PlayerNr = 0 Then FreeCells += 1
        Next
        If FreeCells = 0 Then
            TxtStatus.Text = "This game is a draw."
            GameStarted = False
            Return True
        End If
        If PlayerCount = 1 Then
            If Check4(1) > 0 Then
                TxtStatus.Text = "Congratulations, You won!"
                GameStarted = False
                Return True
            End If
            If Check4(2) > 0 Then
                TxtStatus.Text = "Sorry, You lost!"
                GameStarted = False
                Return True
            End If
        End If
        If PlayerCount = 2 Then
            If Check4(1) > 0 Then
                TxtStatus.Text = "Congratulations " & Player1.ToString() & ", You won!"
                GameStarted = False
                Return True
            End If
            If Check4(2) > 0 Then
                TxtStatus.Text = "Congratulations " & Player2.ToString() & ", You won!"
                GameStarted = False
                Return True
            End If
        End If
        Return False
    End Function

    ''' <summary>
    ''' Check for 4 in a Row
    ''' </summary>
    ''' <param name="player">The player number (1 or 2)</param>
    ''' <returns>The number of times 4 in a row was found</returns>
    Private Function Check4(ByVal player As Integer) As Integer
        Dim row As Integer
        Dim col As Integer
        Dim aantal As Integer = 0
        For I As Integer = 0 To Markers.Length - 1
            If Markers(I).PlayerNr <> player Then Continue For
            row = GetRow(I)
            col = GetCol(I)
            If col > 2 Then
                'Check LEFT 
                If Markers(GetIndex(row, col - 1)).PlayerNr = player And Markers(GetIndex(row, col - 2)).PlayerNr = player And Markers(GetIndex(row, col - 3)).PlayerNr = player Then aantal += 1
                If row > 2 Then
                    'Check LEFT UP
                    If Markers(GetIndex(row - 1, col - 1)).PlayerNr = player And Markers(GetIndex(row - 2, col - 2)).PlayerNr = player And Markers(GetIndex(row - 3, col - 3)).PlayerNr = player Then aantal += 1
                End If
            End If
            If row > 2 Then
                'Check UP
                If Markers(GetIndex(row - 1, col)).PlayerNr = player And Markers(GetIndex(row - 2, col)).PlayerNr = player And Markers(GetIndex(row - 3, col)).PlayerNr = player Then aantal += 1
            End If
            If col < ColNum - 3 Then
                'Check RIGHT
                If Markers(GetIndex(row, col + 1)).PlayerNr = player And Markers(GetIndex(row, col + 2)).PlayerNr = player And Markers(GetIndex(row, col + 3)).PlayerNr = player Then aantal += 1
                If row > 2 Then
                    'Check RIGHT UP
                    If Markers(GetIndex(row - 1, col + 1)).PlayerNr = player And Markers(GetIndex(row - 2, col + 2)).PlayerNr = player And Markers(GetIndex(row - 3, col + 3)).PlayerNr = player Then aantal += 1
                End If
            End If
        Next I
        Return aantal
    End Function

    ''' <summary>
    ''' Check for 3 in a Row with the 4th place empty
    ''' </summary>
    ''' <param name="player">The player number (1 or 2)</param>
    ''' <returns>The number of times 3 in a row was found</returns>
    Private Function Check3(ByVal player As Integer) As Integer
        Dim row As Integer
        Dim col As Integer
        Dim aantal As Integer = 0
        For I As Integer = 0 To Markers.Length - 1
            If Markers(I).PlayerNr <> player Then Continue For
            row = GetRow(I)
            col = GetCol(I)
            If col > 2 Then
                'Check LEFT 
                If Markers(GetIndex(row, col - 1)).PlayerNr = player And Markers(GetIndex(row, col - 2)).PlayerNr = player And Markers(GetIndex(row, col - 3)).PlayerNr = 0 Then aantal += 1
                If row > 2 Then
                    'Check LEFT UP
                    If Markers(GetIndex(row - 1, col - 1)).PlayerNr = player And Markers(GetIndex(row - 2, col - 2)).PlayerNr = player And Markers(GetIndex(row - 3, col - 3)).PlayerNr = 0 Then aantal += 1
                End If
            End If
            If row > 2 Then
                'Check UP
                If Markers(GetIndex(row - 1, col)).PlayerNr = player And Markers(GetIndex(row - 2, col)).PlayerNr = player And Markers(GetIndex(row - 3, col)).PlayerNr = 0 Then aantal += 1
            End If
            If col < ColNum - 3 Then
                'Check RIGHT
                If Markers(GetIndex(row, col + 1)).PlayerNr = player And Markers(GetIndex(row, col + 2)).PlayerNr = player And Markers(GetIndex(row, col + 3)).PlayerNr = 0 Then aantal += 1
                If row > 2 Then
                    'Check RIGHT UP
                    If Markers(GetIndex(row - 1, col + 1)).PlayerNr = player And Markers(GetIndex(row - 2, col + 2)).PlayerNr = player And Markers(GetIndex(row - 3, col + 3)).PlayerNr = 0 Then aantal += 1
                End If
            End If
        Next I
        Return aantal
    End Function

    ''' <summary>
    ''' Check for 2 in a Row with the 3rd and 4th place empty
    ''' </summary>
    ''' <param name="player">The player number (1 or 2)</param>
    ''' <returns>The number of times 2 in a row was found</returns>
    Private Function Check2(ByVal player As Integer) As Integer
        Dim row As Integer
        Dim col As Integer
        Dim aantal As Integer = 0
        For I As Integer = 0 To Markers.Length - 1
            If Markers(I).PlayerNr <> player Then Continue For
            row = GetRow(I)
            col = GetCol(I)
            If col > 2 Then
                'Check LEFT
                If Markers(GetIndex(row, col - 1)).PlayerNr = player And Markers(GetIndex(row, col - 2)).PlayerNr = 0 And Markers(GetIndex(row, col - 3)).PlayerNr = 0 Then aantal += 1
                If row > 2 Then
                    'Check LEFT UP
                    If Markers(GetIndex(row - 1, col - 1)).PlayerNr = player And Markers(GetIndex(row - 2, col - 2)).PlayerNr = 0 And Markers(GetIndex(row - 3, col - 3)).PlayerNr = 0 Then aantal += 1
                End If
            End If
            If row > 2 Then
                'Check UP
                If Markers(GetIndex(row - 1, col)).PlayerNr = player And Markers(GetIndex(row - 2, col)).PlayerNr = 0 And Markers(GetIndex(row - 3, col)).PlayerNr = 0 Then aantal += 1
            End If
            If col < ColNum - 3 Then
                'Check RIGHT
                If Markers(GetIndex(row, col + 1)).PlayerNr = player And Markers(GetIndex(row, col + 2)).PlayerNr = 0 And Markers(GetIndex(row, col + 3)).PlayerNr = 0 Then aantal += 1
                If row > 2 Then
                    'Check RIGHT UP
                    If Markers(GetIndex(row - 1, col + 1)).PlayerNr = player And Markers(GetIndex(row - 2, col + 2)).PlayerNr = 0 And Markers(GetIndex(row - 3, col + 3)).PlayerNr = 0 Then aantal += 1
                End If
            End If
        Next I
        Return aantal
    End Function

#End Region

End Class

Public Structure Move
    Public col As Integer
    Public score As Integer
End Structure

Public Enum GameMode
    Easy = 2
    Normal = 3
    Hard = 5
End Enum

Public Structure Settings
    Public PlayerCount As Integer
    Public PlayerName1 As String
    Public PlayerName2 As String
    Public GameSize As Size
    Public GameMode As GameMode
End Structure

