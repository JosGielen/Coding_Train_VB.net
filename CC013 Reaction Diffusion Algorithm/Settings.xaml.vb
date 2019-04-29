Public Class Settings
    Private MyMain As Window
    Private MyDiffA As Double = 0.0
    Private MyDiffB As Double = 0.0
    Private MyFeed As Double = 0.0
    Private MyKill As Double = 0.0

    Public Sub New(main As Window)
        ' This call is required by the designer.
        InitializeComponent()
        MyMain = main
    End Sub

    Public Property DiffA As Double
        Get
            Return MyDiffA
        End Get
        Set(value As Double)
            MyDiffA = value
            TxtDiffA.Text = MyDiffA.ToString()
        End Set
    End Property

    Public Property DiffB As Double
        Get
            Return MyDiffB
        End Get
        Set(value As Double)
            MyDiffB = value
            TxtDiffB.Text = MyDiffB.ToString()
        End Set
    End Property

    Public Property Feed As Double
        Get
            Return MyFeed
        End Get
        Set(value As Double)
            MyFeed = value
            TxtFeed.Text = MyFeed.ToString()
        End Set
    End Property

    Public Property Kill As Double
        Get
            Return MyKill
        End Get
        Set(value As Double)
            MyKill = value
            TxtKill.Text = MyKill.ToString()
        End Set
    End Property


    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If BtnStart.Content.Equals("Start") Then
            BtnStart.Content = "Stop"
            Try
                MyDiffA = Double.Parse(TxtDiffA.Text)
                MyDiffB = Double.Parse(TxtDiffB.Text)
                MyFeed = Double.Parse(TxtFeed.Text)
                MyKill = Double.Parse(TxtKill.Text)
                CType(MyMain, MainWindow).Start()
            Catch ex As Exception
                MessageBox.Show("The Parameters are not valid.", "L-System settings error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        Else
            CType(MyMain, MainWindow).Halt()
            BtnStart.Content = "Start"
        End If
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As RoutedEventArgs)
        MyMain.Close()
        Me.Hide()
    End Sub
End Class
