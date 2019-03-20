Imports Barnsley_Ferns

Public Class Settings
    Private MyMain As Window
    Private my_p1 As Double = 0.0
    Private my_p2 As Double = 0.0
    Private my_p3 As Double = 0.0
    Private my_p4 As Double = 0.0
    Private my_F1 As Matrix
    Private my_F2 As Matrix
    Private my_F3 As Matrix
    Private my_F4 As Matrix
    Private my_C1 As Matrix
    Private my_C2 As Matrix
    Private my_C3 As Matrix
    Private my_C4 As Matrix
    Private my_Type As String

    Public Sub New(main As Window, name As String)
        ' This call is required by the designer.
        InitializeComponent()
        MyMain = main
        my_F1 = New Matrix(2, 2)
        my_F2 = New Matrix(2, 2)
        my_F3 = New Matrix(2, 2)
        my_F4 = New Matrix(2, 2)
        my_C1 = New Matrix(2, 1)
        my_C2 = New Matrix(2, 1)
        my_C3 = New Matrix(2, 1)
        my_C4 = New Matrix(2, 1)
        Type = name
        TxtFernType.Text = my_Type
    End Sub

    Public Property p1 As Double
        Get
            my_p1 = Double.Parse(TxtP1.Text)
            Return my_p1
        End Get
        Set(value As Double)
            my_p1 = value
            TxtP1.Text = my_p1.ToString()
        End Set
    End Property

    Public Property p2 As Double
        Get
            my_p2 = Double.Parse(TxtP2.Text)
            Return my_p2
        End Get
        Set(value As Double)
            my_p2 = value
            TxtP2.Text = my_p2.ToString()
        End Set
    End Property

    Public Property p3 As Double
        Get
            my_p3 = Double.Parse(TxtP3.Text)
            Return my_p3
        End Get
        Set(value As Double)
            my_p3 = value
            TxtP3.Text = my_p3.ToString()
        End Set
    End Property

    Public Property p4 As Double
        Get
            my_p4 = Double.Parse(TxtP4.Text)
            Return my_p4
        End Get
        Set(value As Double)
            my_p4 = value
            TxtP4.Text = my_p4.ToString()
        End Set
    End Property

    Public Property F1 As Matrix
        Get
            my_F1.Value(0, 0) = Double.Parse(TxtF1X1.Text)
            my_F1.Value(0, 1) = Double.Parse(TxtF1Y1.Text)
            my_F1.Value(1, 0) = Double.Parse(TxtF1X2.Text)
            my_F1.Value(1, 1) = Double.Parse(TxtF1Y2.Text)
            Return my_F1
        End Get
        Set(value As Matrix)
            my_F1 = CType(value.Clone(), Matrix)
            TxtF1X1.Text = my_F1.Value(0, 0).ToString()
            TxtF1Y1.Text = my_F1.Value(0, 1).ToString()
            TxtF1X2.Text = my_F1.Value(1, 0).ToString()
            TxtF1Y2.Text = my_F1.Value(1, 1).ToString()
        End Set
    End Property

    Public Property F2 As Matrix
        Get
            my_F2.Value(0, 0) = Double.Parse(TxtF2X1.Text)
            my_F2.Value(0, 1) = Double.Parse(TxtF2Y1.Text)
            my_F2.Value(1, 0) = Double.Parse(TxtF2X2.Text)
            my_F2.Value(1, 1) = Double.Parse(TxtF2Y2.Text)
            Return my_F2
        End Get
        Set(value As Matrix)
            my_F2 = CType(value.Clone(), Matrix)
            TxtF2X1.Text = my_F2.Value(0, 0).ToString()
            TxtF2Y1.Text = my_F2.Value(0, 1).ToString()
            TxtF2X2.Text = my_F2.Value(1, 0).ToString()
            TxtF2Y2.Text = my_F2.Value(1, 1).ToString()
        End Set
    End Property

    Public Property F3 As Matrix
        Get
            my_F3.Value(0, 0) = Double.Parse(TxtF3X1.Text)
            my_F3.Value(0, 1) = Double.Parse(TxtF3Y1.Text)
            my_F3.Value(1, 0) = Double.Parse(TxtF3X2.Text)
            my_F3.Value(1, 1) = Double.Parse(TxtF3Y2.Text)
            Return my_F3
        End Get
        Set(value As Matrix)
            my_F3 = CType(value.Clone(), Matrix)
            TxtF3X1.Text = my_F3.Value(0, 0).ToString()
            TxtF3Y1.Text = my_F3.Value(0, 1).ToString()
            TxtF3X2.Text = my_F3.Value(1, 0).ToString()
            TxtF3Y2.Text = my_F3.Value(1, 1).ToString()
        End Set
    End Property

    Public Property F4 As Matrix
        Get
            my_F4.Value(0, 0) = Double.Parse(TxtF4X1.Text)
            my_F4.Value(0, 1) = Double.Parse(TxtF4Y1.Text)
            my_F4.Value(1, 0) = Double.Parse(TxtF4X2.Text)
            my_F4.Value(1, 1) = Double.Parse(TxtF4Y2.Text)
            Return my_F4
        End Get
        Set(value As Matrix)
            my_F4 = CType(value.Clone(), Matrix)
            TxtF4X1.Text = my_F4.Value(0, 0).ToString()
            TxtF4Y1.Text = my_F4.Value(0, 1).ToString()
            TxtF4X2.Text = my_F4.Value(1, 0).ToString()
            TxtF4Y2.Text = my_F4.Value(1, 1).ToString()
        End Set
    End Property

    Public Property C1 As Matrix
        Get
            my_C1.Value(0, 0) = Double.Parse(TxtC1X.Text)
            my_C1.Value(1, 0) = Double.Parse(TxtC1Y.Text)
            Return my_C1
        End Get
        Set(value As Matrix)
            my_C1 = CType(value.Clone(), Matrix)
            TxtC1X.Text = my_C1.Value(0, 0).ToString()
            TxtC1Y.Text = my_C1.Value(1, 0).ToString()
        End Set
    End Property

    Public Property C2 As Matrix
        Get
            my_C2.Value(0, 0) = Double.Parse(TxtC2X.Text)
            my_C2.Value(1, 0) = Double.Parse(TxtC2Y.Text)
            Return my_C2
        End Get
        Set(value As Matrix)
            my_C2 = CType(value.Clone(), Matrix)
            TxtC2X.Text = my_C2.Value(0, 0).ToString()
            TxtC2Y.Text = my_C2.Value(1, 0).ToString()
        End Set
    End Property

    Public Property C3 As Matrix
        Get
            my_C3.Value(0, 0) = Double.Parse(TxtC3X.Text)
            my_C3.Value(1, 0) = Double.Parse(TxtC3Y.Text)
            Return my_C3
        End Get
        Set(value As Matrix)
            my_C3 = CType(value.Clone(), Matrix)
            TxtC3X.Text = my_C3.Value(0, 0).ToString()
            TxtC3Y.Text = my_C3.Value(1, 0).ToString()
        End Set
    End Property

    Public Property C4 As Matrix
        Get
            my_C4.Value(0, 0) = Double.Parse(TxtC4X.Text)
            my_C4.Value(1, 0) = Double.Parse(TxtC4Y.Text)
            Return my_C4
        End Get
        Set(value As Matrix)
            my_C4 = CType(value.Clone(), Matrix)
            TxtC4X.Text = my_C4.Value(0, 0).ToString()
            TxtC4Y.Text = my_C4.Value(1, 0).ToString()
        End Set
    End Property

    Public Property Type As String
        Get
            my_Type = TxtFernType.Text
            Return my_Type
        End Get
        Set(value As String)
            my_Type = value
            TxtFernType.Text = my_Type
        End Set
    End Property

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        Try
            If CType(BtnStart.Content, String) = "Start" Then
                CType(MyMain, MainWindow).Start()
                BtnStart.Content = "Stop"
            Else
                CType(MyMain, MainWindow).Halt()
                BtnStart.Content = "Start"
            End If
        Catch ex As Exception
            MessageBox.Show("The Parameters are not valid.", "Barnsley Fern settings error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As RoutedEventArgs)
        CType(MyMain, MainWindow).MnuShowSettings.IsChecked = False
        Me.Hide()
    End Sub

End Class
