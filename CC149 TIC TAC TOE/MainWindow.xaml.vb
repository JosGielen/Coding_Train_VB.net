
Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private cells(8) As Cell
    Private rects(8) As Rectangle
    Private startplayer As String = ""
    Private currentPlayer As String = ""
    Private MoveNr As Integer = 0
    Dim Rnd As Random = New Random()

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        If Rnd.NextDouble() < 0.5 Then
            startplayer = "X"
        Else
            startplayer = "O"
        End If
        NewGame()
    End Sub

    Private Sub NewGame()
        For I As Integer = 0 To 8
            cells(I) = New Cell() With
            {
                .content = I.ToString(),
                .isUsed = False
            }
        Next
        MoveNr = 1
        'Switch start player each game
        If startplayer = "X" Then
            startplayer = "O"
        Else
            startplayer = "X"
        End If
        If startplayer = "X" Then
            currentPlayer = "X"
            CalculateMove()
            TxtMessage.Text = "I started. Please make your move."
        ElseIf startplayer = "O" Then
            currentPlayer = "O"
            TxtMessage.Text = "The Player starts. Please make your move."
        End If
        AddHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
    End Sub

    Private Sub CompositionTarget_Rendering(sender As Object, e As EventArgs)
        canvas1.Children.Clear()
        DrawGrid()
        If GetResult().Equals("O") Then
            Showresult("Congratulations, You Won!")
        ElseIf GetResult().Equals("X") Then
            Showresult("Sorry, I Won!")
        ElseIf MoveNr > 9 Then
            Showresult("It ended in a Tie.")
        Else
            If currentPlayer = "X" Then
                CalculateMove()
            End If
        End If
    End Sub

    Private Sub DrawGrid()
        Dim center As Point = New Point()
        Dim size As Double = canvas1.ActualWidth / 5
        Dim l As Line
        Dim r As Rectangle
        l = New Line() With
        {
            .Stroke = Brushes.Black,
            .StrokeThickness = 3.0,
            .X1 = 25,
            .Y1 = 25,
            .X2 = canvas1.ActualWidth - 25,
            .Y2 = 25
        }
        canvas1.Children.Add(l)
        l = New Line() With
        {
            .Stroke = Brushes.Black,
            .StrokeThickness = 3.0,
            .X1 = 25,
            .Y1 = (canvas1.ActualHeight - 50) / 3 + 25,
            .X2 = canvas1.ActualWidth - 25,
            .Y2 = (canvas1.ActualHeight - 50) / 3 + 25
        }
        canvas1.Children.Add(l)
        l = New Line() With
        {
            .Stroke = Brushes.Black,
            .StrokeThickness = 3.0,
            .X1 = 25,
            .Y1 = 2 * (canvas1.ActualHeight - 50) / 3 + 25,
            .X2 = canvas1.ActualWidth - 25,
            .Y2 = 2 * (canvas1.ActualHeight - 50) / 3 + 25
        }
        canvas1.Children.Add(l)
        l = New Line() With
        {
            .Stroke = Brushes.Black,
            .StrokeThickness = 3.0,
            .X1 = 25,
            .Y1 = canvas1.ActualHeight - 25,
            .X2 = canvas1.ActualWidth - 25,
            .Y2 = canvas1.ActualHeight - 25
        }
        canvas1.Children.Add(l)
        l = New Line() With
        {
            .Stroke = Brushes.Black,
            .StrokeThickness = 3.0,
            .X1 = 25,
            .Y1 = 25,
            .X2 = 25,
            .Y2 = canvas1.ActualHeight - 25
        }
        canvas1.Children.Add(l)
        l = New Line() With
        {
            .Stroke = Brushes.Black,
            .StrokeThickness = 3.0,
            .X1 = (canvas1.ActualWidth - 50) / 3 + 25,
            .Y1 = 25,
            .X2 = (canvas1.ActualWidth - 50) / 3 + 25,
            .Y2 = canvas1.ActualHeight - 25
        }
        canvas1.Children.Add(l)
        l = New Line() With
        {
            .Stroke = Brushes.Black,
            .StrokeThickness = 3.0,
            .X1 = 2 * (canvas1.ActualWidth - 50) / 3 + 25,
            .Y1 = 25,
            .X2 = 2 * (canvas1.ActualWidth - 50) / 3 + 25,
            .Y2 = canvas1.ActualHeight - 25
        }
        canvas1.Children.Add(l)
        l = New Line() With
        {
            .Stroke = Brushes.Black,
            .StrokeThickness = 3.0,
            .X1 = canvas1.ActualWidth - 25,
            .Y1 = 25,
            .X2 = canvas1.ActualWidth - 25,
            .Y2 = canvas1.ActualHeight - 25
        }
        canvas1.Children.Add(l)
        For I As Integer = 0 To 8
            center.X = (I Mod 3 + 0.5) * (canvas1.ActualWidth - 50) / 3 + 25
            center.Y = (Math.Floor(I / 3) + 0.5) * (canvas1.ActualHeight - 50) / 3 + 25
            r = New Rectangle() With
            {
                .Width = size,
                .Height = size,
                .StrokeThickness = 0.0,
                .Fill = Brushes.White
            }
            r.SetValue(Canvas.LeftProperty, center.X - size / 2)
            r.SetValue(Canvas.TopProperty, center.Y - size / 2)
            rects(I) = r
            canvas1.Children.Add(r)
            If cells(I).content.Equals("O") Then
                Dim el As Ellipse = New Ellipse() With
                {
                    .Stroke = Brushes.Black,
                    .StrokeThickness = 3.0,
                    .Width = size,
                    .Height = size
                }
                el.SetValue(Canvas.LeftProperty, center.X - size / 2)
                el.SetValue(Canvas.TopProperty, center.Y - size / 2)
                canvas1.Children.Add(el)
            ElseIf cells(I).content.Equals("X") Then
                l = New Line() With
                {
                    .Stroke = Brushes.Black,
                    .StrokeThickness = 3.0,
                    .X1 = center.X - size / 2,
                    .Y1 = center.Y - size / 2,
                    .X2 = center.X + size / 2,
                    .Y2 = center.Y + size / 2
                }
                canvas1.Children.Add(l)
                l = New Line() With
                {
                    .Stroke = Brushes.Black,
                    .StrokeThickness = 3.0,
                    .X1 = center.X - size / 2,
                    .Y1 = center.Y + size / 2,
                    .X2 = center.X + size / 2,
                    .Y2 = center.Y - size / 2
                }
                canvas1.Children.Add(l)
            End If
        Next
    End Sub

    Private Sub CalculateMove()
        If currentPlayer <> "X" Then Exit Sub
        Dim corner As Integer
        Dim selectedIndex As Integer
        If MoveNr = 1 Then 'Always start in a corner
            corner = Rnd.Next(4)
            If corner = 0 Then selectedIndex = 0
            If corner = 1 Then selectedIndex = 2
            If corner = 2 Then selectedIndex = 6
            If corner = 3 Then selectedIndex = 8
        ElseIf MoveNr = 2 Or MoveNr = 3 Then 'Try the center.
            If Not cells(4).isUsed Then
                selectedIndex = 4
            Else   'if the player started in the center, use a corner
                Do
                    corner = Rnd.Next(4)
                    If corner = 0 Then selectedIndex = 0
                    If corner = 1 Then selectedIndex = 2
                    If corner = 2 Then selectedIndex = 6
                    If corner = 3 Then selectedIndex = 8
                Loop While cells(selectedIndex).isUsed
            End If
        Else
            'Try to make 3 in a row
            If cells(0).content.Equals("X") And cells(0).Equals(cells(1)) And Not cells(2).isUsed Then
                selectedIndex = 2
            ElseIf cells(0).content.Equals("X") And cells(0).Equals(cells(2)) And Not cells(1).isUsed Then
                selectedIndex = 1
            ElseIf cells(1).content.Equals("X") And cells(1).Equals(cells(2)) And Not cells(0).isUsed Then
                selectedIndex = 0
            ElseIf cells(3).content.Equals("X") And cells(3).Equals(cells(4)) And Not cells(5).isUsed Then
                selectedIndex = 5
            ElseIf cells(3).content.Equals("X") And cells(3).Equals(cells(5)) And Not cells(4).isUsed Then
                selectedIndex = 4
            ElseIf cells(4).content.Equals("X") And cells(4).Equals(cells(5)) And Not cells(3).isUsed Then
                selectedIndex = 3
            ElseIf cells(6).content.Equals("X") And cells(6).Equals(cells(7)) And Not cells(8).isUsed Then
                selectedIndex = 8
            ElseIf cells(6).content.Equals("X") And cells(6).Equals(cells(8)) And Not cells(7).isUsed Then
                selectedIndex = 7
            ElseIf cells(7).content.Equals("X") And cells(7).Equals(cells(8)) And Not cells(6).isUsed Then
                selectedIndex = 6
            ElseIf cells(0).content.Equals("X") And cells(0).Equals(cells(3)) And Not cells(6).isUsed Then
                selectedIndex = 6
            ElseIf cells(0).content.Equals("X") And cells(0).Equals(cells(6)) And Not cells(3).isUsed Then
                selectedIndex = 3
            ElseIf cells(3).content.Equals("X") And cells(3).Equals(cells(6)) And Not cells(0).isUsed Then
                selectedIndex = 0
            ElseIf cells(1).content.Equals("X") And cells(1).Equals(cells(4)) And Not cells(7).isUsed Then
                selectedIndex = 7
            ElseIf cells(1).content.Equals("X") And cells(1).Equals(cells(7)) And Not cells(4).isUsed Then
                selectedIndex = 4
            ElseIf cells(4).content.Equals("X") And cells(4).Equals(cells(7)) And Not cells(1).isUsed Then
                selectedIndex = 1
            ElseIf cells(2).content.Equals("X") And cells(2).Equals(cells(5)) And Not cells(8).isUsed Then
                selectedIndex = 8
            ElseIf cells(2).content.Equals("X") And cells(2).Equals(cells(8)) And Not cells(5).isUsed Then
                selectedIndex = 5
            ElseIf cells(5).content.Equals("X") And cells(5).Equals(cells(8)) And Not cells(2).isUsed Then
                selectedIndex = 2
            ElseIf cells(0).content.Equals("X") And cells(0).Equals(cells(4)) And Not cells(8).isUsed Then
                selectedIndex = 8
            ElseIf cells(0).content.Equals("X") And cells(0).Equals(cells(8)) And Not cells(4).isUsed Then
                selectedIndex = 4
            ElseIf cells(4).content.Equals("X") And cells(4).Equals(cells(8)) And Not cells(0).isUsed Then
                selectedIndex = 0
            ElseIf cells(2).content.Equals("X") And cells(2).Equals(cells(4)) And Not cells(6).isUsed Then
                selectedIndex = 6
            ElseIf cells(2).content.Equals("X") And cells(2).Equals(cells(6)) And Not cells(4).isUsed Then
                selectedIndex = 4
            ElseIf cells(4).content.Equals("X") And cells(4).Equals(cells(6)) And Not cells(2).isUsed Then
                selectedIndex = 2
                'Prevent the player from making 3 in a row
            ElseIf cells(0).content.Equals("O") And cells(0).Equals(cells(1)) And Not cells(2).isUsed Then
                selectedIndex = 2
            ElseIf cells(0).content.Equals("O") And cells(0).Equals(cells(2)) And Not cells(1).isUsed Then
                selectedIndex = 1
            ElseIf cells(1).content.Equals("O") And cells(1).Equals(cells(2)) And Not cells(0).isUsed Then
                selectedIndex = 0
            ElseIf cells(3).content.Equals("O") And cells(3).Equals(cells(4)) And Not cells(5).isUsed Then
                selectedIndex = 5
            ElseIf cells(3).content.Equals("O") And cells(3).Equals(cells(5)) And Not cells(4).isUsed Then
                selectedIndex = 4
            ElseIf cells(4).content.Equals("O") And cells(4).Equals(cells(5)) And Not cells(3).isUsed Then
                selectedIndex = 3
            ElseIf cells(6).content.Equals("O") And cells(6).Equals(cells(7)) And Not cells(8).isUsed Then
                selectedIndex = 8
            ElseIf cells(6).content.Equals("O") And cells(6).Equals(cells(8)) And Not cells(7).isUsed Then
                selectedIndex = 7
            ElseIf cells(7).content.Equals("O") And cells(7).Equals(cells(8)) And Not cells(6).isUsed Then
                selectedIndex = 6
            ElseIf cells(0).content.Equals("O") And cells(0).Equals(cells(3)) And Not cells(6).isUsed Then
                selectedIndex = 6
            ElseIf cells(0).content.Equals("O") And cells(0).Equals(cells(6)) And Not cells(3).isUsed Then
                selectedIndex = 3
            ElseIf cells(3).content.Equals("O") And cells(3).Equals(cells(6)) And Not cells(0).isUsed Then
                selectedIndex = 0
            ElseIf cells(1).content.Equals("O") And cells(1).Equals(cells(4)) And Not cells(7).isUsed Then
                selectedIndex = 7
            ElseIf cells(1).content.Equals("O") And cells(1).Equals(cells(7)) And Not cells(4).isUsed Then
                selectedIndex = 4
            ElseIf cells(4).content.Equals("O") And cells(4).Equals(cells(7)) And Not cells(1).isUsed Then
                selectedIndex = 1
            ElseIf cells(2).content.Equals("O") And cells(2).Equals(cells(5)) And Not cells(8).isUsed Then
                selectedIndex = 8
            ElseIf cells(2).content.Equals("O") And cells(2).Equals(cells(8)) And Not cells(5).isUsed Then
                selectedIndex = 5
            ElseIf cells(5).content.Equals("O") And cells(5).Equals(cells(8)) And Not cells(2).isUsed Then
                selectedIndex = 2
            ElseIf cells(0).content.Equals("O") And cells(0).Equals(cells(4)) And Not cells(8).isUsed Then
                selectedIndex = 8
            ElseIf cells(0).content.Equals("O") And cells(0).Equals(cells(8)) And Not cells(4).isUsed Then
                selectedIndex = 4
            ElseIf cells(4).content.Equals("O") And cells(4).Equals(cells(8)) And Not cells(0).isUsed Then
                selectedIndex = 0
            ElseIf cells(2).content.Equals("O") And cells(2).Equals(cells(4)) And Not cells(6).isUsed Then
                selectedIndex = 6
            ElseIf cells(2).content.Equals("O") And cells(2).Equals(cells(6)) And Not cells(4).isUsed Then
                selectedIndex = 4
            ElseIf cells(4).content.Equals("O") And cells(4).Equals(cells(6)) And Not cells(2).isUsed Then
                selectedIndex = 2
                'Try to make 2 x 2 in a row
            ElseIf cells(0).content.Equals("X") And Not cells(1).isUsed And Not cells(2).isUsed Then
                selectedIndex = 2
            ElseIf cells(0).content.Equals("X") And Not cells(3).isUsed And Not cells(6).isUsed Then
                selectedIndex = 6
            ElseIf cells(2).content.Equals("X") And Not cells(1).isUsed And Not cells(0).isUsed Then
                selectedIndex = 0
            ElseIf cells(2).content.Equals("X") And Not cells(5).isUsed And Not cells(8).isUsed Then
                selectedIndex = 8
            ElseIf cells(6).content.Equals("X") And Not cells(3).isUsed And Not cells(0).isUsed Then
                selectedIndex = 0
            ElseIf cells(6).content.Equals("X") And Not cells(7).isUsed And Not cells(8).isUsed Then
                selectedIndex = 8
            ElseIf cells(8).content.Equals("X") And Not cells(5).isUsed And Not cells(2).isUsed Then
                selectedIndex = 2
            ElseIf cells(8).content.Equals("X") And Not cells(7).isUsed And Not cells(6).isUsed Then
                selectedIndex = 6
            Else
                Do
                    selectedIndex = Rnd.Next(9)
                Loop While cells(selectedIndex).isUsed
            End If
        End If
        cells(selectedIndex).content = "X"
        cells(selectedIndex).isUsed = True
        MoveNr += 1
        currentPlayer = "O"
    End Sub

    Private Sub Showresult(result As String)
        currentPlayer = ""
        RemoveHandler CompositionTarget.Rendering, AddressOf CompositionTarget_Rendering
        TxtMessage.Text = "Game Over: " & result & vbCrLf & "Do you want to play again?"
        btnOK.Visibility = Visibility.Visible
        btnCANCEL.Visibility = Visibility.Visible
    End Sub

    Private Function GetResult() As String
        If cells(0).Equals(cells(1)) And cells(0).Equals(cells(2)) Then
            rects(0).Fill = Brushes.Red
            rects(1).Fill = Brushes.Red
            rects(2).Fill = Brushes.Red
            Return cells(0).content
        ElseIf cells(3).Equals(cells(4)) And cells(3).Equals(cells(5)) Then
            rects(3).Fill = Brushes.Red
            rects(4).Fill = Brushes.Red
            rects(5).Fill = Brushes.Red
            Return cells(3).content
        ElseIf cells(6).Equals(cells(7)) And cells(6).Equals(cells(8)) Then
            rects(6).Fill = Brushes.Red
            rects(7).Fill = Brushes.Red
            rects(8).Fill = Brushes.Red
            Return cells(6).content
        ElseIf cells(0).Equals(cells(3)) And cells(0).Equals(cells(6)) Then
            rects(0).Fill = Brushes.Red
            rects(3).Fill = Brushes.Red
            rects(6).Fill = Brushes.Red
            Return cells(0).content
        ElseIf cells(1).Equals(cells(4)) And cells(1).Equals(cells(7)) Then
            rects(1).Fill = Brushes.Red
            rects(4).Fill = Brushes.Red
            rects(7).Fill = Brushes.Red
            Return cells(1).content
        ElseIf cells(2).Equals(cells(5)) And cells(2).Equals(cells(8)) Then
            rects(2).Fill = Brushes.Red
            rects(5).Fill = Brushes.Red
            rects(8).Fill = Brushes.Red
            Return cells(2).content
        ElseIf cells(0).Equals(cells(4)) And cells(0).Equals(cells(8)) Then
            rects(0).Fill = Brushes.Red
            rects(4).Fill = Brushes.Red
            rects(8).Fill = Brushes.Red
            Return cells(0).content
        ElseIf cells(2).Equals(cells(4)) And cells(2).Equals(cells(6)) Then
            rects(2).Fill = Brushes.Red
            rects(4).Fill = Brushes.Red
            rects(6).Fill = Brushes.Red
            Return cells(2).content
        End If
        Return "T"
    End Function

    Private Sub Window_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Dim playerSelectedIndex As Integer = 0
        If currentPlayer <> "O" Then Exit Sub
        Dim pt As Point = e.GetPosition(canvas1)
        If pt.X < 25 Or pt.X > canvas1.ActualWidth - 25 Then Exit Sub
        If pt.Y < 25 Or pt.Y > canvas1.ActualHeight - 25 Then Exit Sub
        Dim col As Integer = CInt(Math.Floor((pt.X - 25) / ((canvas1.ActualWidth - 50) / 3)))
        Dim row As Integer = CInt(Math.Floor((pt.Y - 25) / ((canvas1.ActualHeight - 50) / 3)))
        playerSelectedIndex = 3 * row + col
        If Not cells(playerSelectedIndex).isUsed Then
            cells(playerSelectedIndex).content = "O"
            cells(playerSelectedIndex).isUsed = True
            MoveNr += 1
            currentPlayer = "X"
            TxtMessage.Text = "Please make your move."
        Else
            Beep()
        End If
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As RoutedEventArgs)
        btnOK.Visibility = Visibility.Collapsed
        btnCANCEL.Visibility = Visibility.Collapsed
        NewGame()
    End Sub

    Private Sub BtnCANCEL_Click(sender As Object, e As RoutedEventArgs)
        End
    End Sub
End Class

Public Class Cell
    Public content As String = ""
    Public isUsed As Boolean = False

    Public Overrides Function Equals(obj As Object) As Boolean
        If TypeOf obj IsNot Cell Then Return False
        Return content.Equals(CType(obj, Cell).content)
    End Function
End Class
