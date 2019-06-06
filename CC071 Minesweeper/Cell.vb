Public Class Cell
    Private PosX As Integer
    Private PosY As Integer
    Private mySize As Double
    Private myParent As MainWindow
    Private myGotBomb As Boolean
    Private myRevealed As Boolean
    Private myBombNeighbours As Integer
    Private myBtn As Button

    Public Sub New(X As Integer, Y As Integer, gotbomb As Boolean, size As Double, parent As MainWindow)
        PosX = X
        PosY = Y
        myGotBomb = gotbomb
        mySize = size
        myRevealed = False
        myBombNeighbours = 0
        myParent = parent
        myBtn = New Button() With
        {
            .Width = mySize,
            .Height = mySize,
            .Content = "",
            .FontSize = 18,
            .Padding = New Thickness(0, 0, 0, 0),
            .VerticalContentAlignment = VerticalAlignment.Center,
            .FontWeight = FontWeights.Bold,
            .Background = Brushes.SlateBlue
        }
        AddHandler myBtn.Click, AddressOf Btn_Click
    End Sub

    Public Property GotBomb As Boolean
        Get
            Return myGotBomb
        End Get
        Set(value As Boolean)
            myGotBomb = value
        End Set
    End Property

    Public Property Revealed As Boolean
        Get
            Return myRevealed
        End Get
        Set(value As Boolean)
            myRevealed = value
        End Set
    End Property

    Public Property BombNeighbours As Integer
        Get
            Return myBombNeighbours
        End Get
        Set(value As Integer)
            myBombNeighbours = value
        End Set
    End Property

    Public Property Btn As Button
        Get
            Return myBtn
        End Get
        Set(value As Button)
            myBtn = value
        End Set
    End Property

    Public Sub Show(c As Canvas)
        Btn.SetValue(Canvas.LeftProperty, mySize * PosX)
        Btn.SetValue(Canvas.TopProperty, mySize * PosY)
        c.Children.Add(Btn)
    End Sub

    Public Sub Reveal()
        myBtn.Background = Brushes.LightGray
        If myGotBomb Then 'TODO: Show an image of a bomb
            Dim el As Ellipse = New Ellipse() With
            {
                .Fill = Brushes.OrangeRed,
                .Width = mySize / 2,
                .Height = mySize / 2
            }
            myBtn.Background = Brushes.Yellow
            myBtn.Content = el
        Else
            If BombNeighbours = 0 Then
                myBtn.Content = ""
            Else
                If BombNeighbours = 1 Then
                    myBtn.Foreground = Brushes.Blue
                ElseIf BombNeighbours = 2 Then
                    myBtn.Foreground = Brushes.Green
                Else
                    myBtn.Foreground = Brushes.Red
                End If
                myBtn.Content = BombNeighbours.ToString()
            End If
        End If
        Revealed = True
    End Sub

    Private Sub Btn_Click(sender As Object, e As RoutedEventArgs)
        Reveal()
        If myGotBomb Then
            myParent.RevealAll()
        Else
            If BombNeighbours = 0 Then
                myParent.RevealNeighbours(PosX, PosY)
            End If
        End If
    End Sub

End Class
