Imports System.IO

Public Class Settings
    Private MyMain As Window
    Private my_Favoritesfile As String
    Private FavoriteSettings As List(Of Favorite)
    Public CellSize As Double = 0.0
    Public ParticleCount As Integer = 0
    Public MaxForce As Double = 0.0
    Public Speed As Double = 0.0
    Public XYChange As Double = 0.0
    Public ZChange As Double = 0.0
    Public TrailLength As Byte = 0
    Public RandomSpawn As Boolean = False
    Public UseColor As Boolean = False

    Public Sub New(main As Window)
        InitializeComponent()
        MyMain = main
        'Load Favorites from file
        my_Favoritesfile = Environment.CurrentDirectory & "\Favorites.txt"
        FavoriteSettings = New List(Of Favorite)
        Try
            LoadFavorites(my_Favoritesfile)
        Catch ex As Exception 'No ini file or wrong data format.

        End Try
    End Sub

    Public Sub Update()
        TxtCellSize.Text = CellSize.ToString()
        TxtCount.Text = ParticleCount.ToString()
        TxtForce.Text = MaxForce.ToString()
        TxtSpeed.Text = Speed.ToString()
        TxtXYChange.Text = XYChange.ToString()
        TxtZChange.Text = ZChange.ToString()
        TxtTrailLength.Text = TrailLength.ToString()
        CBRndSpawn.IsChecked = RandomSpawn
        CBUseColor.IsChecked = UseColor
    End Sub

    Private Sub LoadFavorites(filename As String)
        Dim sr As StreamReader = Nothing
        Dim fav As Favorite
        sr = New StreamReader(filename)
        While Not sr.EndOfStream
            fav = New Favorite
            fav.Name = sr.ReadLine()
            fav.CellSize = Double.Parse(sr.ReadLine())
            fav.ParticleCount = Integer.Parse(sr.ReadLine())
            fav.MaxForce = Double.Parse(sr.ReadLine())
            fav.Speed = Double.Parse(sr.ReadLine())
            fav.XYChange = Double.Parse(sr.ReadLine())
            fav.ZChange = Double.Parse(sr.ReadLine())
            fav.TrailLength = Byte.Parse(sr.ReadLine())
            fav.RandomSpawn = Boolean.Parse(sr.ReadLine())
            fav.UseColor = Boolean.Parse(sr.ReadLine())
            FavoriteSettings.Add(fav)
        End While
        If (sr IsNot Nothing) Then
            sr.Close()
        End If
        For I As Integer = 0 To FavoriteSettings.Count - 1
            CmbFavorites.Items.Add(FavoriteSettings(I).Name)
        Next
        If CmbFavorites.Items.Count > 0 Then
            CmbFavorites.SelectedIndex = 0
            SetFavorite(0)
        End If
    End Sub

    Private Sub SetFavorite(index As Integer)
        If FavoriteSettings.Count > index Then
            CellSize = FavoriteSettings(index).CellSize
            ParticleCount = FavoriteSettings(index).ParticleCount
            MaxForce = FavoriteSettings(index).MaxForce
            Speed = FavoriteSettings(index).Speed
            XYChange = FavoriteSettings(index).XYChange
            ZChange = FavoriteSettings(index).ZChange
            TrailLength = FavoriteSettings(index).TrailLength
            RandomSpawn = FavoriteSettings(index).RandomSpawn
            UseColor = FavoriteSettings(index).UseColor
            Update()
        End If
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        Try
            CellSize = Double.Parse(TxtCellSize.Text)
            ParticleCount = Integer.Parse(TxtCount.Text)
            MaxForce = Double.Parse(TxtForce.Text)
            Speed = Double.Parse(TxtSpeed.Text)
            XYChange = Double.Parse(TxtXYChange.Text)
            ZChange = Double.Parse(TxtZChange.Text)
            TrailLength = Byte.Parse(TxtTrailLength.Text)
            RandomSpawn = CBRndSpawn.IsChecked.Value
            UseColor = CBUseColor.IsChecked.Value
            CType(MyMain, MainWindow).Start()
        Catch ex As Exception
            MessageBox.Show("The Parameters are not valid.", "Perlin FlowField settings error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As RoutedEventArgs)
        End
    End Sub

    Private Sub BtnCellSizeUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtCellSize.Text)
        dummy += 1
        TxtCellSize.Text = dummy.ToString()
    End Sub

    Private Sub BtnCellSizeDown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtCellSize.Text)
        dummy -= 1
        If dummy < 5 Then dummy = 5
        TxtCellSize.Text = dummy.ToString()
    End Sub

    Private Sub BtnCountUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Integer = Integer.Parse(TxtCount.Text)
        dummy += 100
        TxtCount.Text = dummy.ToString()
    End Sub

    Private Sub BtnCountDown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Integer = Integer.Parse(TxtCount.Text)
        dummy -= 100
        If dummy < 100 Then dummy = 100
        TxtCount.Text = dummy.ToString()
    End Sub

    Private Sub BtnForceUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtForce.Text)
        dummy += 0.01
        TxtForce.Text = dummy.ToString()
    End Sub

    Private Sub BtnForceDown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtForce.Text)
        dummy -= 0.01
        TxtForce.Text = dummy.ToString()
    End Sub

    Private Sub BtnSpeedUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtSpeed.Text)
        dummy += 0.05
        TxtSpeed.Text = dummy.ToString()
    End Sub

    Private Sub BtnSpeedDown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtSpeed.Text)
        dummy -= 0.05
        TxtSpeed.Text = dummy.ToString()
    End Sub

    Private Sub BtnXYChangeUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtXYChange.Text)
        dummy += 0.01
        TxtXYChange.Text = dummy.ToString()
    End Sub

    Private Sub BtnXYChangeDown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtXYChange.Text)
        dummy -= 0.01
        TxtXYChange.Text = dummy.ToString()
    End Sub

    Private Sub BtnZChangeUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtZChange.Text)
        dummy += 0.0002
        TxtZChange.Text = dummy.ToString()
    End Sub

    Private Sub BtnZChangeDown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Double = Double.Parse(TxtZChange.Text)
        dummy -= 0.0002
        TxtZChange.Text = dummy.ToString()
    End Sub

    Private Sub BtnTrailLengthUP_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Integer = Integer.Parse(TxtTrailLength.Text)
        dummy += 5
        If dummy > 255 Then dummy = 255
        TxtTrailLength.Text = dummy.ToString()
    End Sub

    Private Sub BtnTrailLengthDown_Click(sender As Object, e As RoutedEventArgs)
        Dim dummy As Integer = Integer.Parse(TxtTrailLength.Text)
        dummy -= 5
        If dummy < 5 Then dummy = 5
        TxtTrailLength.Text = dummy.ToString()
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As RoutedEventArgs)
        Dim sw As StreamWriter = Nothing
        Dim frm As FavoriteNameForm = New FavoriteNameForm()
        If frm.ShowDialog() Then
            'Make the new Favorite
            Dim fav As Favorite = New Favorite
            fav.Name = frm.FavoriteName
            fav.CellSize = Double.Parse(TxtCellSize.Text)
            fav.ParticleCount = Integer.Parse(TxtCount.Text)
            fav.MaxForce = Double.Parse(TxtForce.Text)
            fav.Speed = Double.Parse(TxtSpeed.Text)
            fav.XYChange = Double.Parse(TxtXYChange.Text)
            fav.ZChange = Double.Parse(TxtZChange.Text)
            fav.TrailLength = Byte.Parse(TxtTrailLength.Text)
            fav.RandomSpawn = CBRndSpawn.IsChecked.Value
            fav.UseColor = CBUseColor.IsChecked.Value
            FavoriteSettings.Add(fav)
            'Set the new Favorite name in the combobox
            CmbFavorites.Items.Add(fav.Name)
            CmbFavorites.SelectedIndex = CmbFavorites.Items.Count - 1
            'Write all the Favorites to the Favorites file
            sw = New StreamWriter(my_Favoritesfile)
            For I As Integer = 0 To FavoriteSettings.Count - 1
                sw.WriteLine(FavoriteSettings(I).Name)
                sw.WriteLine(FavoriteSettings(I).CellSize.ToString())
                sw.WriteLine(FavoriteSettings(I).ParticleCount.ToString())
                sw.WriteLine(FavoriteSettings(I).MaxForce.ToString())
                sw.WriteLine(FavoriteSettings(I).Speed.ToString())
                sw.WriteLine(FavoriteSettings(I).XYChange.ToString())
                sw.WriteLine(FavoriteSettings(I).ZChange.ToString())
                sw.WriteLine(FavoriteSettings(I).TrailLength.ToString())
                sw.WriteLine(FavoriteSettings(I).RandomSpawn.ToString())
                sw.WriteLine(FavoriteSettings(I).UseColor.ToString())
            Next
            If (sw IsNot Nothing) Then
                sw.Close()
            End If
        End If
    End Sub

    Private Sub CmbFavorites_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not IsLoaded Then Exit Sub
        SetFavorite(CmbFavorites.SelectedIndex)
    End Sub
End Class

Public Structure Favorite
    Public Name As String
    Public CellSize As Double
    Public ParticleCount As Integer
    Public MaxForce As Double
    Public Speed As Double
    Public XYChange As Double
    Public ZChange As Double
    Public TrailLength As Byte
    Public RandomSpawn As Boolean
    Public UseColor As Boolean
End Structure
