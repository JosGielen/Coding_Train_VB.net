Public Class FavoriteNameForm

    Private my_Name As String = ""

    Public Property FavoriteName As String
        Get
            Return my_Name
        End Get
        Set(value As String)
            my_Name = value
        End Set
    End Property

    Private Sub BtnOK_Click(sender As Object, e As RoutedEventArgs)
        my_Name = TxtName.Text
        DialogResult = True
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As RoutedEventArgs)
        my_Name = ""
        DialogResult = False
    End Sub
End Class
