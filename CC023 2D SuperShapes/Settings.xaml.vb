Imports Barnsley_Ferns

Public Class Settings
    Private MyMain As Window
    Public A As Double = 0.0
    Public B As Double = 0.0
    Public M As Double = 0.0
    Public N1 As Double = 0.0
    Public N2 As Double = 0.0
    Public N3 As Double = 0.0

    Public Sub New(main As Window)
        InitializeComponent()
        MyMain = main
    End Sub

    Public Sub Update()
        TxtA.Text = A.ToString()
        TxtB.Text = B.ToString()
        TxtM.Text = M.ToString()
        TxtN1.Text = N1.ToString()
        TxtN2.Text = N2.ToString()
        TxtN3.Text = N3.ToString()
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        Try
            A = Double.Parse(TxtA.Text)
            B = Double.Parse(TxtB.Text)
            M = Double.Parse(TxtM.Text)
            N1 = Double.Parse(TxtN1.Text)
            N2 = Double.Parse(TxtN2.Text)
            N3 = Double.Parse(TxtN3.Text)
            CType(MyMain, MainWindow).Start()
        Catch ex As Exception
            MessageBox.Show("The Parameters are not valid.", "3D SuperShape settings error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As RoutedEventArgs)
        End
    End Sub

    Private Sub BtnAUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtA.Text)
        dummy += 0.1
        TxtA.Text = dummy.ToString()
    End Sub

    Private Sub BtnADown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtA.Text)
        dummy -= 0.1
        TxtA.Text = dummy.ToString()
    End Sub

    Private Sub BtnBUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtB.Text)
        dummy += 0.1
        TxtB.Text = dummy.ToString()
    End Sub

    Private Sub BtnBDown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtB.Text)
        dummy -= 0.1
        TxtB.Text = dummy.ToString()
    End Sub

    Private Sub BtnMUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtM.Text)
        dummy += 1
        TxtM.Text = dummy.ToString()
    End Sub

    Private Sub BtnMDown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtM.Text)
        dummy -= 1
        TxtM.Text = dummy.ToString()
    End Sub

    Private Sub BtnN1UP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtN1.Text)
        dummy += 0.1
        TxtN1.Text = dummy.ToString()
    End Sub

    Private Sub BtnN1Down_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtN1.Text)
        dummy -= 0.1
        TxtN1.Text = dummy.ToString()
    End Sub

    Private Sub BtnN2UP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtN2.Text)
        dummy += 0.1
        TxtN2.Text = dummy.ToString()
    End Sub

    Private Sub BtnN2Down_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtN2.Text)
        dummy -= 0.1
        TxtN2.Text = dummy.ToString()
    End Sub

    Private Sub BtnN3UP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtN3.Text)
        dummy += 0.1
        TxtN3.Text = dummy.ToString()
    End Sub

    Private Sub BtnN3Down_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtN3.Text)
        dummy -= 0.1
        TxtN3.Text = dummy.ToString()
    End Sub
End Class
