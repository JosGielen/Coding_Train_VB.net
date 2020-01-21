Imports System.Windows.Threading

Class MainWindow
    Private Delegate Sub MoveDelegate()
    Private AppLoaded As Boolean = False
    Private GameStarted As Boolean = False
    Private RowNum As Integer
    Private ColNum As Integer
    Private StartIndex As Integer = 0
    Private CelWidth As Double = 0.0
    Private CelHeight As Double = 0.0
    Private Markers() As Marker
    Private Playeraantal As Integer
    Private CurrentPlayer As Integer
    Private Player1 As String = "Player1Name"
    Private Player2 As String = "Player2Name"
    Private myCol As Integer = 0
    Private myDepth As Integer

#Region "Window Events"
    Private Sub Window1_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        'Default start with 1 player
        Playeraantal = 1
        CurrentPlayer = 1 'If 1 player then Player = 1 ; Computer = 2
        'Default start with 6 rows and 7 columns
        RowNum = 6
        ColNum = 7
        ReDim Markers(RowNum * ColNum - 1)
        For I As Integer = 0 To ColNum * RowNum - 1
            Markers(I) = New Marker()
        Next
        Me.Width = 60 * ColNum
        Me.Height = 60 * RowNum + 35
        'Default start in normal difficulty mode
        myDepth = 3
        'Draw the empty start game
        DrawGame()
        AppLoaded = True
        GameStarted = True
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

#Region "Menu Events"

    Private Sub MenuExit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        End
    End Sub

    Private Sub MenuEasy_Click(sender As Object, e As RoutedEventArgs)
        MenuEasy.IsChecked = True
        MenuNormal.IsChecked = False
        MenuHard.IsChecked = False
        myDepth = 2
    End Sub

    Private Sub MenuNormal_Click(sender As Object, e As RoutedEventArgs)
        MenuEasy.IsChecked = False
        MenuNormal.IsChecked = True
        MenuHard.IsChecked = False
        myDepth = 3
    End Sub

    Private Sub MenuHard_Click(sender As Object, e As RoutedEventArgs)
        MenuEasy.IsChecked = False
        MenuNormal.IsChecked = False
        MenuHard.IsChecked = True
        myDepth = 5
    End Sub

    Private Sub Menu6x7_Click(sender As Object, e As RoutedEventArgs)
        ColNum = 7
        RowNum = 6
        ReDim Markers(RowNum * ColNum - 1)
        For I As Integer = 0 To ColNum * RowNum - 1
            Markers(I) = New Marker()
        Next
        Me.Width = 60 * ColNum
        Me.Height = 60 * RowNum + 35
        Menu6x7.IsChecked = True
        Menu8x8.IsChecked = False
        Menu10x10.IsChecked = False
        Menu12x12.IsChecked = False
        DrawGame()
    End Sub

    Private Sub Menu8x8_Click(sender As Object, e As RoutedEventArgs)
        ColNum = 8
        RowNum = 8
        ReDim Markers(RowNum * ColNum - 1)
        For I As Integer = 0 To ColNum * RowNum - 1
            Markers(I) = New Marker()
        Next
        Me.Width = 60 * ColNum
        Me.Height = 60 * RowNum + 35
        Menu6x7.IsChecked = False
        Menu8x8.IsChecked = True
        Menu10x10.IsChecked = False
        Menu12x12.IsChecked = False
        DrawGame()
    End Sub

    Private Sub Menu10x10_Click(sender As Object, e As RoutedEventArgs)
        ColNum = 10
        RowNum = 10
        ReDim Markers(RowNum * ColNum - 1)
        For I As Integer = 0 To ColNum * RowNum - 1
            Markers(I) = New Marker()
        Next
        Me.Width = 60 * ColNum
        Me.Height = 60 * RowNum + 35
        Menu6x7.IsChecked = False
        Menu8x8.IsChecked = False
        Menu10x10.IsChecked = True
        Menu12x12.IsChecked = False
        DrawGame()
    End Sub

    Private Sub Menu12x12_Click(sender As Object, e As RoutedEventArgs)
        ColNum = 12
        RowNum = 12
        ReDim Markers(RowNum * ColNum - 1)
        For I As Integer = 0 To ColNum * RowNum - 1
            Markers(I) = New Marker()
        Next
        Me.Width = 60 * ColNum
        Me.Height = 60 * RowNum + 35
        Menu6x7.IsChecked = False
        Menu8x8.IsChecked = False
        Menu10x10.IsChecked = False
        Menu12x12.IsChecked = True
        DrawGame()
    End Sub

    Private Sub MenuStartNew_Click(sender As Object, e As RoutedEventArgs)
        For I As Integer = 0 To ColNum * RowNum - 1
            Markers(I) = New Marker()
        Next
        DrawGame()
        GameStarted = True
    End Sub

#End Region

#Region "Business Methods"

    Private Sub Canvas1_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        'A player clicked in the gamearea to select a column for his move
        Dim Col As Integer = 0
        Dim FreeIndex As Integer = 0
        'Check if the game is started
        If Not GameStarted Then
            e.Handled = True
            Exit Sub
        End If
        For I As Integer = 0 To Markers.Length - 1
            'Determine the column
            If Markers(I).Shape.IsMouseOver Then
                Col = Markers(I).Col
                FreeIndex = GetFreeIndex(Col)
                If FreeIndex >= 0 Then
                    If Playeraantal = 1 Then
                        Markers(FreeIndex).PlayerNr = 1
                        If CheckVictory() Then Exit Sub
                        'Get the computer move
                        Me.Dispatcher.Invoke(DispatcherPriority.SystemIdle, New MoveDelegate(AddressOf GetBestMove))
                        'GetBestMove()
                        FreeIndex = GetFreeIndex(myCol)
                        If FreeIndex >= 0 Then
                            Markers(FreeIndex).PlayerNr = 2
                        Else
                            Beep()
                            'The computer made an invalid move!!!
                            'DEBUG CODE
                            MessageBox.Show("The computer made an invalid move!!!", "4 in a Row", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                            'END DEBUG CODE
                        End If
                        If CheckVictory() Then Exit Sub
                    ElseIf Playeraantal = 2 Then
                        Markers(FreeIndex).PlayerNr = CurrentPlayer
                        If CheckVictory() Then Exit Sub
                        If CurrentPlayer = 1 Then
                            CurrentPlayer = 2
                        Else
                            CurrentPlayer = 1
                        End If
                    End If
                Else
                    Beep()
                    'The player made an invalid move!!!
                End If
            End If
        Next
    End Sub

    Private Sub GetBestMove()
        Dim FreeIndex As Integer = 0
        Dim score As Integer
        Dim bestscore As Integer = Integer.MinValue
        Dim bestcol As Integer = -1
        Me.Cursor = Cursors.Wait
        For I As Integer = 0 To ColNum - 1
            FreeIndex = GetFreeIndex(I)
            If FreeIndex > 0 Then
                Markers(FreeIndex).PlayerNr = 2
                score = Minimax(myDepth, I, False)
                If score > bestscore Then
                    bestscore = score
                    bestcol = I
                End If
                Markers(FreeIndex).PlayerNr = 0
            End If
        Next
        myCol = bestcol
        Me.Cursor = Cursors.Arrow
    End Sub

    ''' <summary>
    ''' Use the Minimax algorithm to estimate the best move
    ''' </summary>
    Private Function Minimax(depth As Integer, col As Integer, maximizing As Boolean) As Integer
        Dim FreeIndex As Integer = 0
        Dim Score As Integer = 0
        Dim Bestscore As Integer = 0
        Dim validmoves As Integer = 0
        Dim result As Integer = 0
        If depth = 0 Or Check4(1) > 0 Or Check4(2) > 0 Then 'return the heuristic value of the game
            result = 1000 * Check4(2) + 100 * Check3(2) + 10 * Check2(2) - 1000 * Check4(1) - 100 * Check3(1) - 10 * Check2(1)
            Return result
        End If
        If maximizing Then
            Bestscore = Integer.MinValue
            For I As Integer = 0 To ColNum - 1
                FreeIndex = GetFreeIndex(I)
                If FreeIndex >= 0 Then
                    Markers(FreeIndex).PlayerNr = 2 'Computer = maximizing
                    Score = Minimax(depth - 1, I, False)
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
                    Score = Minimax(depth - 1, I, True)
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
            MessageBox.Show("Sorry, This game is a draw.", "4 in a Row", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            GameStarted = False
            Return True
        End If

        If Playeraantal = 1 Then
            If Check4(1) > 0 Then
                MessageBox.Show("Congratulations, You won!", "4 in a Row", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                GameStarted = False
                Return True
            End If
            If Check4(2) > 0 Then
                MessageBox.Show("Sorry, You lost!", "4 in a Row", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                GameStarted = False
                Return True
            End If
        End If
        If Playeraantal = 2 Then
            If Check4(1) > 0 Then
                MessageBox.Show("Congratulations " & Player1.ToString() & ", You won!", "4 in a Row", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                GameStarted = False
                Return True
            End If
            If Check4(2) > 0 Then
                MessageBox.Show("Congratulations " & Player2.ToString() & ", You won!", "4 in a Row", MessageBoxButton.OK, MessageBoxImage.Exclamation)
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
        Dim row As Integer = 0
        Dim col As Integer = 0
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
        Dim row As Integer = 0
        Dim col As Integer = 0
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
        Dim row As Integer = 0
        Dim col As Integer = 0
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

