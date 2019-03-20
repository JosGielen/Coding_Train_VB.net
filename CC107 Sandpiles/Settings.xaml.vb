Imports Microsoft.Win32

Public Class Settings
    Private My_Main As Window
    Private my_MaxSand As Integer
    Private my_InitialSand As Integer
    Private my_DistributionNr As Integer
    Private my_Type As String
    Private my_ColorList As List(Of Color)
    Private my_Loaded As Boolean = False

    Public Sub New(main As Window, colors As List(Of Color))
        ' This call is required by the designer.
        InitializeComponent()
        My_Main = main
        my_ColorList = colors
    End Sub

    Public Property MaxSand As Integer
        Get
            my_MaxSand = Integer.Parse(TxtMaxSand.Text)
            Return my_MaxSand
        End Get
        Set(value As Integer)
            my_MaxSand = value
            TxtMaxSand.Text = my_MaxSand.ToString()
            Init()
        End Set
    End Property

    Public Property DistributionNr As Integer
        Get
            Return my_DistributionNr
        End Get
        Set(value As Integer)
            my_DistributionNr = value
            If my_DistributionNr = 4 Then
                RB4way.IsChecked = True
            ElseIf my_DistributionNr = 8 Then
                RB8Way.IsChecked = True
            Else
                MessageBox.Show("Invalid Sand distribution number.", "SandPiles Settings error", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
        End Set
    End Property

    Public Property Type As String
        Get
            my_Type = TxtType.Text
            Return my_Type
        End Get
        Set(value As String)
            my_Type = value
            TxtType.Text = my_Type
        End Set
    End Property

    Public Property ColorList As List(Of Color)
        Get
            Return my_ColorList
        End Get
        Set(value As List(Of Color))
            my_ColorList = value
            Init()
        End Set
    End Property

    Public Property InitialSand As Integer
        Get
            my_InitialSand = Integer.Parse(TxtInitialSand.Text)
            Return my_InitialSand
        End Get
        Set(value As Integer)
            my_InitialSand = value
            TxtInitialSand.Text = my_InitialSand.ToString()
        End Set
    End Property

    Private Sub Init()
        Dim btn As Button
        LBColors.Items.Clear()
        For I As Integer = 0 To my_MaxSand + 1
            btn = New Button() With {
                  .Background = New SolidColorBrush(my_ColorList(I)),
                  .Height = 15,
                  .Width = LBColors.ActualWidth - 15
                  }
            LBColors.Items.Add(btn)
        Next
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        Try
            If CType(BtnStart.Content, String) = "Start" Then
                CType(My_Main, MainWindow).Start()
                BtnStart.Content = "Stop"
            Else
                CType(My_Main, MainWindow).Halt()
                BtnStart.Content = "Start"
            End If
        Catch ex As Exception
            MessageBox.Show("The Parameters are not valid.", "SandPiles Settings error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As RoutedEventArgs)
        CType(My_Main, MainWindow).MnuShowSettings.IsChecked = False
        Me.Hide()
    End Sub

    Private Sub BtnColor_Click(sender As Object, e As RoutedEventArgs)
        'TODO: Show a COLOR Selection Dialog
        CType(sender, Button).Background = Brushes.Black 'Set the background to the selected color
    End Sub

    Private Sub RB4way_Click(sender As Object, e As RoutedEventArgs)
        If RB4way.IsChecked Then my_DistributionNr = 4
    End Sub

    Private Sub RB8Way_Click(sender As Object, e As RoutedEventArgs)
        If RB8Way.IsChecked Then my_DistributionNr = 8
    End Sub

    Private Sub TxtMaxSand_TextChanged(sender As Object, e As TextChangedEventArgs) Handles TxtMaxSand.TextChanged
        If TxtMaxSand.Text = "" Then Exit Sub
        Try
            my_MaxSand = Integer.Parse(TxtMaxSand.Text)
            Init()
        Catch ex As Exception
            MessageBox.Show("Invalid Maximum Sand.", "SandPiles Settings Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
