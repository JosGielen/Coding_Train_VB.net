Class CoordinateDialog
    Private pt As Point = New Point(0, 0)

    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        TxtX.Focus()
    End Sub

    Private Sub TxtX_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles TxtX.KeyDown
        If e.Key < Key.D0 Then e.Handled = True
        If e.Key > Key.D9 And e.Key < Key.NumPad0 Then e.Handled = True
        If e.Key > Key.NumPad9 Then e.Handled = True
    End Sub

    Private Sub TxtY_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles TxtY.KeyDown
        If e.Key < Key.D0 Then e.Handled = True
        If e.Key > Key.D9 And e.Key < Key.NumPad0 Then e.Handled = True
        If e.Key > Key.NumPad9 Then e.Handled = True
    End Sub

    Private Sub BtnOK_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnOK.Click
        Try
            pt.X = Integer.Parse(TxtX.Text)
            pt.Y = Integer.Parse(TxtY.Text)
        Catch ex As Exception
            MessageBox.Show("Invalid Coordinates!", "Coordinate Dialog Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK)
            Me.DialogResult = False
            Me.Close()
            Exit Sub
        End Try
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles BtnCancel.Click
        Me.DialogResult = False
        Me.Close()
    End Sub

    Public Function Getcoordinate() As Point
        Return pt
    End Function

End Class
