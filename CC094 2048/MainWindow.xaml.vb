Class MainWindow
    Private RowNum As Integer = 4
    Private ColNum As Integer = 4
    Private CelWidth As Double = 0.0
    Private CelHeight As Double = 0.0
    Private CellColors(10) As Brush
    Private LineThickness As Double = 10.0
    Private Cells(,) As Cell
    Private GameStarted As Boolean
    Private Score As Integer = 0
    Private Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim gridLine As Line
        Dim FieldWidth As Double = Canvas1.ActualWidth
        Dim FieldHeight As Double = Canvas1.ActualHeight
        Dim LineColor As Brush = Brushes.BurlyWood
        ReDim Cells(RowNum - 1, ColNum - 1)
        Score = 0
        CelWidth = (FieldWidth - (ColNum + 1) * LineThickness) / ColNum
        CelHeight = (FieldHeight - (RowNum + 1) * LineThickness) / RowNum
        Canvas1.Children.Clear()
        'Draw border of the canvas
        'Top
        gridLine = New Line With {
            .Stroke = LineColor,
            .StrokeThickness = LineThickness,
            .X1 = 0,
            .Y1 = LineThickness / 2,
            .X2 = FieldWidth,
            .Y2 = LineThickness / 2
        }
        Canvas1.Children.Add(gridLine)
        'Right
        gridLine = New Line With {
            .Stroke = LineColor,
            .StrokeThickness = LineThickness,
            .X1 = FieldWidth - LineThickness / 2,
            .Y1 = LineThickness / 2,
            .X2 = FieldWidth - LineThickness / 2,
            .Y2 = FieldHeight - LineThickness / 2
        }
        Canvas1.Children.Add(gridLine)
        'Left
        gridLine = New Line With {
            .Stroke = LineColor,
            .StrokeThickness = LineThickness,
            .X1 = LineThickness / 2,
            .Y1 = LineThickness / 2,
            .X2 = LineThickness / 2,
            .Y2 = FieldHeight - LineThickness / 2
        }
        Canvas1.Children.Add(gridLine)
        'Bottom
        gridLine = New Line With {
            .Stroke = LineColor,
            .StrokeThickness = LineThickness,
            .X1 = 0,
            .Y1 = FieldHeight - LineThickness / 2,
            .X2 = FieldWidth,
            .Y2 = FieldHeight - LineThickness / 2
        }
        Canvas1.Children.Add(gridLine)
        'Draw Vertical gridlines
        For I As Integer = 1 To ColNum - 1
            gridLine = New Line With {
                .Stroke = LineColor,
                .StrokeThickness = LineThickness,
                .X1 = (CelWidth + LineThickness) * I + LineThickness / 2,
                .Y1 = LineThickness / 2,
                .X2 = (CelWidth + LineThickness) * I + LineThickness / 2,
                .Y2 = FieldHeight - LineThickness / 2
            }
            Canvas1.Children.Add(gridLine)
        Next
        'Draw Horizontal gridlines
        For I As Integer = 1 To RowNum - 1
            gridLine = New Line With {
                .Stroke = LineColor,
                .StrokeThickness = LineThickness,
                .X1 = LineThickness / 2,
                .Y1 = (CelHeight + LineThickness) * I + LineThickness / 2,
                .X2 = FieldWidth - LineThickness / 2,
                .Y2 = (CelHeight + LineThickness) * I + LineThickness / 2
            }
            Canvas1.Children.Add(gridLine)
        Next
        'Make all the cells
        For I As Integer = 0 To RowNum - 1
            For J As Integer = 0 To ColNum - 1
                Cells(I, J) = CreateCell(I, J, 0, False)
            Next
        Next
        'Show the initial 2 cells
        GetEmptyCell().Visible = True
        GetEmptyCell().Visible = True
        For I As Integer = 0 To RowNum - 1
            For J As Integer = 0 To ColNum - 1
                Cells(I, J).Draw(Canvas1)
            Next
        Next
        GameStarted = True
    End Sub

    Private Sub Window_KeyUp(sender As Object, e As KeyEventArgs)
        Dim movescore As Integer
        Dim moveCount As Integer
        If GameStarted Then
            movescore = 0
            moveCount = 0
            Select Case e.Key
                Case Key.Up
                    'Shift all Cells to the Top and collapse equal cells
                    moveCount += MoveUp()
                    movescore += MergeUp()
                    moveCount += MoveUp()
                Case Key.Down
                    'Shift all Cells to the Bottom and collapse equal cells
                    moveCount += MoveDown()
                    movescore += MergeDown()
                    moveCount += MoveDown()
                Case Key.Left
                    'Shift all Cells to the Left and collapse equal cells
                    moveCount += MoveLeft()
                    movescore += MergeLeft()
                    moveCount += MoveLeft()
                Case Key.Right
                    'Shift all Cells to the Right and collapse equal cells
                    moveCount += MoveRight()
                    movescore += MergeRight()
                    moveCount += MoveRight()
                Case Else
                    e.Handled = True
                    Exit Sub
            End Select
            'Check for 2048 Game Won
            For I As Integer = 0 To RowNum - 1
                For J As Integer = 0 To ColNum - 1
                    If Cells(I, J).CellValue = 2048 Then
                        MessageBox.Show(Me, "CONGRATULATIONS! YOU WON." & vbCrLf & "Your Score = " & Score.ToString(), "2048", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                        Exit Sub
                    End If
                Next
            Next
            'Show a new Cell and give it the Focus
            If moveCount + movescore > 0 Then
                For I As Integer = 0 To RowNum - 1
                    For J As Integer = 0 To ColNum - 1
                        Cells(I, J).SetFocus(False)
                    Next
                Next
                If EmptyCellCount() > 0 Then
                    Dim c As Cell = GetEmptyCell()
                    c.Visible = True
                    c.SetFocus(True)
                End If
            End If
            'Check possible moves
            If PossibleMoves() = False Then
                MessageBox.Show(Me, "  GAME OVER!" & vbCrLf & "Your Score = " & Score.ToString(), "2048", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Exit Sub
            End If
            Score += movescore
            Me.Title = "Score = " & Score.ToString()
            For I As Integer = 0 To RowNum - 1
                For J As Integer = 0 To ColNum - 1
                    Cells(I, J).Draw(Canvas1)
                Next
            Next
        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        GameStarted = False
        End
    End Sub


#Region "Cell Moves"

    Private Function MoveUp() As Integer
        Dim result As Integer = 0
        Dim swapcount As Integer = 0
        Do
            swapcount = 0
            For J As Integer = 0 To ColNum - 1
                For I As Integer = RowNum - 1 To 1 Step -1
                    If Cells(I, J).Visible = True Then
                        'check the cell above
                        If Cells(I - 1, J).Visible = False Then 'Empty cell above so move the cell up
                            SwapCells(Cells(I, J), Cells(I - 1, J))
                            swapcount += 1
                            result += 1
                        End If
                    End If
                Next
            Next
        Loop While swapcount > 0
        Return result
    End Function

    Private Function MergeUp() As Integer
        Dim result As Integer = 0
        For J As Integer = 0 To ColNum - 1
            For I As Integer = 0 To RowNum - 2
                If Cells(I, J).Visible = True Then
                    'check the cell below
                    If Cells(I + 1, J).Visible = True Then
                        If Cells(I, J).CellValue = Cells(I + 1, J).CellValue Then
                            MergeCells(Cells(I + 1, J), Cells(I, J))
                            result += Cells(I, J).CellValue
                        End If
                    End If
                End If
            Next
        Next
        Return result
    End Function

    Private Function MoveDown() As Integer
        Dim result As Integer = 0
        Dim swapcount As Integer = 0
        Do
            swapcount = 0
            For J As Integer = 0 To ColNum - 1
                For I As Integer = 0 To RowNum - 2
                    If Cells(I, J).Visible = True Then
                        'check the cell below
                        If Cells(I + 1, J).Visible = False Then 'Empty cell below so move the cell down
                            SwapCells(Cells(I, J), Cells(I + 1, J))
                            swapcount += 1
                            result += 1
                        End If
                    End If
                Next
            Next
        Loop While swapcount > 0
        Return result
    End Function

    Private Function MergeDown() As Integer
        Dim result As Integer = 0
        For J As Integer = 0 To ColNum - 1
            For I As Integer = RowNum - 1 To 1 Step -1
                If Cells(I, J).Visible = True Then
                    'check the cell above
                    If Cells(I - 1, J).Visible = True Then
                        If Cells(I, J).CellValue = Cells(I - 1, J).CellValue Then
                            MergeCells(Cells(I - 1, J), Cells(I, J))
                            result += Cells(I, J).CellValue
                        End If
                    End If
                End If
            Next
        Next
        Return result
    End Function

    Private Function MoveLeft() As Integer
        Dim result As Integer = 0
        Dim swapcount As Integer = 0
        Do
            swapcount = 0
            For I As Integer = 0 To RowNum - 1
                For J As Integer = ColNum - 1 To 1 Step -1
                    If Cells(I, J).Visible = True Then
                        'check the cell to the left
                        If Cells(I, J - 1).Visible = False Then 'Empty cell below so move the cell down
                            SwapCells(Cells(I, J), Cells(I, J - 1))
                            swapcount += 1
                            result += 1
                        End If
                    End If
                Next
            Next
        Loop While swapcount > 0
        Return result
    End Function

    Private Function MergeLeft() As Integer
        Dim result As Integer = 0
        For I As Integer = 0 To RowNum - 1
            For J As Integer = 0 To ColNum - 2
                If Cells(I, J).Visible = True Then
                    'check the cell to the right
                    If Cells(I, J + 1).Visible = True Then
                        If Cells(I, J).CellValue = Cells(I, J + 1).CellValue Then
                            MergeCells(Cells(I, J + 1), Cells(I, J))
                            result += Cells(I, J).CellValue
                        End If
                    End If
                End If
            Next
        Next
        Return result
    End Function

    Private Function MoveRight() As Integer
        Dim result As Integer = 0
        Dim swapcount As Integer = 0
        Do
            swapcount = 0
            For I As Integer = 0 To RowNum - 1
                For J As Integer = 0 To ColNum - 2
                    If Cells(I, J).Visible = True Then
                        'check the cell to the right
                        If Cells(I, J + 1).Visible = False Then 'Empty cell below so move the cell down
                            SwapCells(Cells(I, J), Cells(I, J + 1))
                            swapcount += 1
                            result += 1
                        End If
                    End If
                Next
            Next
        Loop While swapcount > 0
        Return result
    End Function

    Private Function MergeRight() As Integer
        Dim result As Integer = 0
        For I As Integer = 0 To RowNum - 1
            For J As Integer = ColNum - 1 To 1 Step -1
                If Cells(I, J).Visible = True Then
                    'check the cell to the left
                    If Cells(I, J - 1).Visible = True Then
                        If Cells(I, J).CellValue = Cells(I, J - 1).CellValue Then
                            MergeCells(Cells(I, J - 1), Cells(I, J))
                            result += Cells(I, J).CellValue
                        End If
                    End If
                End If
            Next
        Next
        Return result
    End Function

    Private Sub SwapCells(A As Cell, B As Cell)
        Dim value As Integer = A.CellValue
        Dim vis As Boolean = A.Visible
        Dim celColor As Brush = A.BackColor
        Dim TxtSize As Double = A.TextSize
        A.CellValue = B.CellValue
        A.Visible = B.Visible
        A.BackColor = B.BackColor
        A.TextSize = B.TextSize
        B.CellValue = value
        B.Visible = vis
        B.BackColor = celColor
        B.TextSize = TxtSize
    End Sub

    Private Sub MergeCells(MovingCel As Cell, TargetCel As Cell)
        Dim value As Integer = TargetCel.CellValue
        TargetCel.CellValue = 2 * value
        TargetCel.BackColor = GetCellFormat(2 * value).Color
        TargetCel.TextSize = GetCellFormat(2 * value).Size
        If Rnd.NextDouble() < 0.9 Then
            value = 2
        Else
            value = 4
        End If
        MovingCel.CellValue = value
        MovingCel.BackColor = GetCellFormat(value).Color
        MovingCel.TextSize = GetCellFormat(value).Size
        MovingCel.Visible = False
    End Sub

#End Region

    Private Function CreateCell(row As Integer, col As Integer, value As Integer, visible As Boolean) As Cell
        Dim c As Cell
        c = New Cell(row, col, CelWidth, CelHeight)
        c.Top = LineThickness + (CelHeight + LineThickness) * row
        c.Left = LineThickness + (CelWidth + LineThickness) * col
        If value = 0 Then
            If Rnd.NextDouble() < 0.9 Then
                value = 2
            Else
                value = 4
            End If
        End If
        c.CellValue = value
        c.Visible = visible
        c.BackColor = GetCellFormat(value).Color
        c.TextSize = GetCellFormat(value).Size
        Return c
    End Function

    Private Function PossibleMoves() As Boolean
        Dim result As Boolean = False
        If EmptyCellCount() > 0 Then result = True
        'Check for 2 identical cells side by side
        For I As Integer = 0 To RowNum - 1
            For J As Integer = 0 To ColNum - 2
                If Cells(I, J).CellValue = Cells(I, J + 1).CellValue Then result = True
            Next
        Next
        'Check for 2 identical cells above each other
        For J As Integer = 0 To ColNum - 1
            For I As Integer = 0 To RowNum - 2
                If Cells(I, J).CellValue = Cells(I + 1, J).CellValue Then result = True
            Next
        Next
        Return result
    End Function

    Private Function GetEmptyCell() As Cell
        Dim empty As List(Of Cell) = New List(Of Cell)
        Dim index As Integer = 0
        For I As Integer = 0 To RowNum - 1
            For J As Integer = 0 To ColNum - 1
                If Cells(I, J).Visible = False Then
                    empty.Add(Cells(I, J))
                End If
            Next
        Next
        index = Rnd.Next(empty.Count)
        Return empty(index)
    End Function

    Private Function EmptyCellCount() As Integer
        Dim count As Integer = 0
        For I As Integer = 0 To RowNum - 1
            For J As Integer = 0 To ColNum - 1
                If Cells(I, J).Visible = False Then
                    count += 1
                End If
            Next
        Next
        Return count
    End Function

    Private Function GetCellFormat(value As Integer) As CellFormat
        Dim br As CellFormat
        Select Case value
            Case 2
                br.Color = Brushes.LightYellow
                br.Size = 72
            Case 4
                br.Color = Brushes.LightCyan
                br.Size = 72
            Case 8
                br.Color = Brushes.LightGreen
                br.Size = 72
            Case 16
                br.Color = Brushes.Bisque
                br.Size = 72
            Case 32
                br.Color = Brushes.Pink
                br.Size = 72
            Case 64
                br.Color = Brushes.PaleTurquoise
                br.Size = 72
            Case 128
                br.Color = Brushes.Cyan
                br.Size = 64
            Case 256
                br.Color = Brushes.GreenYellow
                br.Size = 64
            Case 512
                br.Color = Brushes.Orange
                br.Size = 64
            Case 1024
                br.Color = Brushes.Red
                br.Size = 52
            Case 2048
                br.Color = Brushes.Purple
                br.Size = 52
            Case Else
                br.Color = Brushes.Black
                br.Size = 8
        End Select
        Return br
    End Function

End Class

Structure CellFormat
    Public Size As Integer
    Public Color As Brush
End Structure
