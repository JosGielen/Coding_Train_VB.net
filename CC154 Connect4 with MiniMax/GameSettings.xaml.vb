
Public Class GameSettings

    Private my_Playeraantal As Integer
    Private my_Player1 As String
    Private my_Player2 As String
    Private my_Size As Size
    Private my_GameMode As GameMode


    Public Sub New(setting As Settings, parent As MainWindow)
        ' This call is required by the designer.
        InitializeComponent()

        my_Playeraantal = setting.PlayerCount
        my_Player1 = setting.PlayerName1
        my_Player2 = setting.PlayerName2
        my_Size = setting.GameSize
        my_GameMode = setting.GameMode
        If my_Playeraantal = 1 Then RbSinglePlayer.IsChecked = True
        If my_Playeraantal = 2 Then RbTwoPlayers.IsChecked = True
        TxtPlayer1Name.Text = my_Player1
        TxtPlayer2Name.Text = my_Player2
        If my_Size.Width = 7 And my_Size.Height = 6 Then CmbSize.SelectedIndex = 0
        If my_Size.Width = 8 And my_Size.Height = 8 Then CmbSize.SelectedIndex = 1
        If my_Size.Width = 10 And my_Size.Height = 10 Then CmbSize.SelectedIndex = 2
        If my_Size.Width = 12 And my_Size.Height = 12 Then CmbSize.SelectedIndex = 3
        If my_GameMode = GameMode.Easy Then CmbDifficulty.SelectedIndex = 0
        If my_GameMode = GameMode.Normal Then CmbDifficulty.SelectedIndex = 1
        If my_GameMode = GameMode.Hard Then CmbDifficulty.SelectedIndex = 2
    End Sub

    Public Property Playeraantal As Integer
        Get
            Return my_Playeraantal
        End Get
        Set(value As Integer)
            my_Playeraantal = value
        End Set
    End Property

    Public Property Player1 As String
        Get
            Return my_Player1
        End Get
        Set(value As String)
            my_Player1 = value
        End Set
    End Property

    Public Property Player2 As String
        Get
            Return my_Player2
        End Get
        Set(value As String)
            my_Player2 = value
        End Set
    End Property

    Public Property GameMode As GameMode
        Get
            Return my_GameMode
        End Get
        Set(value As GameMode)
            my_GameMode = value
        End Set
    End Property

    Public Property GameSize As Size
        Get
            Return my_Size
        End Get
        Set(value As Size)
            my_Size = value
        End Set
    End Property

    Private Sub RbSinglePlayer_Click(sender As Object, e As RoutedEventArgs)
        TxtPlayer2Name.Text = "Computer"
        TxtPlayer2Name.IsReadOnly = True
    End Sub

    Private Sub RbTwoPlayers_Click(sender As Object, e As RoutedEventArgs)
        If TxtPlayer2Name.Text.Equals("Computer") Then TxtPlayer2Name.Text = ""
        TxtPlayer2Name.IsReadOnly = False
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As RoutedEventArgs)
        If RbSinglePlayer.IsChecked = True Then my_Playeraantal = 1
        If RbTwoPlayers.IsChecked = True Then my_Playeraantal = 2
        my_Player1 = TxtPlayer1Name.Text
        my_Player2 = TxtPlayer2Name.Text
        If my_Player1 = "Player" Then my_Player1 = "" 'Dont call the player Player.
        Select Case CmbDifficulty.SelectedIndex
            Case 0
                my_GameMode = GameMode.Easy
            Case 1
                my_GameMode = GameMode.Normal
            Case 2
                my_GameMode = GameMode.Hard
        End Select
        Select Case CmbSize.SelectedIndex
            Case 0
                my_Size = New Size(7, 6)
            Case 1
                my_Size = New Size(8, 8)
            Case 2
                my_Size = New Size(10, 10)
            Case 3
                my_Size = New Size(12, 12)
        End Select
        DialogResult = True
        Close()
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As RoutedEventArgs)
        DialogResult = False
        Close()
    End Sub
End Class
