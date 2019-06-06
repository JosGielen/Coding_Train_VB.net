Class MainWindow
    Private grid(,) As Cell
    Private cols As Integer
    Private rows As Integer
    Private CellSize As Double
    Private BombCount As Integer
    Private Rnd As Random = New Random

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        'Start the game at 9 x 9 size
        cols = 9
        rows = 9
        CellSize = 25
        BombCount = 10
        Canvas1.Width = CellSize * cols
        Canvas1.Height = CellSize * rows
        Init()
    End Sub

    Private Sub Init()
        'Make all the cells
        Canvas1.Children.Clear()
        Dim c As Cell
        ReDim grid(cols, rows)
        For I As Integer = 0 To cols - 1
            For J As Integer = 0 To rows - 1
                c = New Cell(I, J, False, CellSize, Me)
                grid(I, J) = c
                c.Show(Canvas1)
            Next
        Next
        Canvas1.UpdateLayout()
        'Add the bombs
        Dim X As Integer = 0
        Dim Y As Integer = 0
        For I As Integer = 0 To BombCount - 1
            Do
                X = Rnd.Next(cols)
                Y = Rnd.Next(rows)
            Loop While grid(X, Y).GotBomb
            grid(X, Y).GotBomb = True
        Next
        'Get the number of Neighbours with bombs
        Dim N As Integer = 0
        For I As Integer = 0 To cols - 1
            For J As Integer = 0 To rows - 1
                N = 0
                If I > 0 Then
                    If J > 0 Then
                        If grid(I - 1, J - 1).GotBomb Then N += 1
                    End If
                    If grid(I - 1, J).GotBomb Then N += 1
                    If J < rows - 1 Then
                        If grid(I - 1, J + 1).GotBomb Then N += 1
                    End If
                End If
                If J > 0 Then
                    If grid(I, J - 1).GotBomb Then N += 1
                End If
                If J < rows - 1 Then
                    If grid(I, J + 1).GotBomb Then N += 1
                End If
                If I < cols - 1 Then
                    If J > 0 Then
                        If grid(I + 1, J - 1).GotBomb Then N += 1
                    End If
                    If grid(I + 1, J).GotBomb Then N += 1
                    If J < rows - 1 Then
                        If grid(I + 1, J + 1).GotBomb Then N += 1
                    End If
                End If
                grid(I, J).BombNeighbours = N
            Next
        Next
    End Sub

    Public Sub RevealNeighbours(X As Integer, Y As Integer)
        If X > 0 Then
            If Y > 0 Then
                If Not grid(X - 1, Y - 1).GotBomb And Not grid(X - 1, Y - 1).Revealed Then
                    grid(X - 1, Y - 1).Reveal()
                    If grid(X - 1, Y - 1).BombNeighbours = 0 Then RevealNeighbours(X - 1, Y - 1)
                End If
            End If
            If Not grid(X - 1, Y).GotBomb And Not grid(X - 1, Y).Revealed Then
                grid(X - 1, Y).Reveal()
                If grid(X - 1, Y).BombNeighbours = 0 Then RevealNeighbours(X - 1, Y)
            End If
            If Y < rows - 1 Then
                If Not grid(X - 1, Y + 1).GotBomb And Not grid(X - 1, Y + 1).Revealed Then
                    grid(X - 1, Y + 1).Reveal()
                    If grid(X - 1, Y + 1).BombNeighbours = 0 Then RevealNeighbours(X - 1, Y + 1)
                End If
            End If
        End If
        If Y > 0 Then
            If Not grid(X, Y - 1).GotBomb And Not grid(X, Y - 1).Revealed Then
                grid(X, Y - 1).Reveal()
                If grid(X, Y - 1).BombNeighbours = 0 Then RevealNeighbours(X, Y - 1)
            End If
        End If
        If Y < rows - 1 Then
            If Not grid(X, Y + 1).GotBomb And Not grid(X, Y + 1).Revealed Then
                grid(X, Y + 1).Reveal()
                If grid(X, Y + 1).BombNeighbours = 0 Then RevealNeighbours(X, Y + 1)
            End If
        End If
        If X < cols - 1 Then
            If Y > 0 Then
                If Not grid(X + 1, Y - 1).GotBomb And Not grid(X + 1, Y - 1).Revealed Then
                    grid(X + 1, Y - 1).Reveal()
                    If grid(X + 1, Y - 1).BombNeighbours = 0 Then RevealNeighbours(X + 1, Y - 1)
                End If
            End If
            If Not grid(X + 1, Y).GotBomb And Not grid(X + 1, Y).Revealed Then
                grid(X + 1, Y).Reveal()
                If grid(X + 1, Y).BombNeighbours = 0 Then RevealNeighbours(X + 1, Y)
            End If
            If Y < rows - 1 Then
                If Not grid(X + 1, Y + 1).GotBomb And Not grid(X + 1, Y + 1).Revealed Then
                    grid(X + 1, Y + 1).Reveal()
                    If grid(X + 1, Y + 1).BombNeighbours = 0 Then RevealNeighbours(X + 1, Y + 1)
                End If
            End If
        End If
    End Sub

    Public Sub RevealAll()
        For I As Integer = 0 To cols - 1
            For J As Integer = 0 To rows - 1
                grid(I, J).Reveal()
            Next
        Next
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

    Private Sub MnuNewGame_Click(sender As Object, e As RoutedEventArgs)
        Init()
    End Sub

    Private Sub MnuExit_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub Mnu9x9_Click(sender As Object, e As RoutedEventArgs)
        cols = 9
        rows = 9
        BombCount = 10
        Canvas1.Width = CellSize * cols
        Canvas1.Height = CellSize * rows
        Init()
    End Sub

    Private Sub Mnu15x15_Click(sender As Object, e As RoutedEventArgs)
        cols = 15
        rows = 15
        BombCount = 25
        Canvas1.Width = CellSize * cols
        Canvas1.Height = CellSize * rows
        Init()
    End Sub

    Private Sub Mnu20x20_Click(sender As Object, e As RoutedEventArgs)
        cols = 20
        rows = 20
        BombCount = 50
        Canvas1.Width = CellSize * cols
        Canvas1.Height = CellSize * rows
        Init()
    End Sub
End Class
