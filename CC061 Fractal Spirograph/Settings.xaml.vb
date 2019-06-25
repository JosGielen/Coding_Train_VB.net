Public Class Settings
    Private MyMain As Window
    Private My_CircleCount As Integer = 0
    Private My_RadiusFactor As Integer = 0
    Private my_SpeedFactor As Integer = 0
    Private my_InnerCircles As Boolean = False
    Private my_TimeStep As Double = 0.0

    Public Sub New(main As Window)
        ' This call is required by the designer.
        InitializeComponent()
        MyMain = main
    End Sub

    Public Property CircleCount As Integer
        Get
            Return My_CircleCount
        End Get
        Set(value As Integer)
            My_CircleCount = value
            TxtAantal.Text = My_CircleCount.ToString()
        End Set
    End Property

    Public Property RadiusFactor As Integer
        Get
            Return My_RadiusFactor
        End Get
        Set(value As Integer)
            My_RadiusFactor = value
            TxtRadius.Text = My_RadiusFactor.ToString() & "%"
        End Set
    End Property

    Public Property InnerCircles As Boolean
        Get
            Return RbInner.IsChecked.Value
        End Get
        Set(value As Boolean)
            my_InnerCircles = value
            RbInner.IsChecked = value
            RbOuter.IsChecked = Not value
        End Set
    End Property

    Public Property SpeedFactor As Integer
        Get
            Return my_SpeedFactor
        End Get
        Set(value As Integer)
            my_SpeedFactor = value
            TxtSpeed.Text = my_SpeedFactor.ToString()
        End Set
    End Property

    Public Property TimeStep As Double
        Get
            Return my_TimeStep
        End Get
        Set(value As Double)
            my_TimeStep = value
            TxtStep.Text = my_TimeStep.ToString()
        End Set
    End Property

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If BtnStart.Content.Equals("Start") Then
            BtnStart.Content = "Stop"
            Try
                My_CircleCount = Integer.Parse(TxtAantal.Text)
                My_RadiusFactor = Integer.Parse(TxtRadius.Text.Substring(0, 2))
                my_SpeedFactor = Integer.Parse(TxtSpeed.Text)
                my_InnerCircles = RbInner.IsChecked.Value
                my_TimeStep = Double.Parse(TxtStep.Text)
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

    Private Sub BtnAantalUP_Click(sender As Object, e As RoutedEventArgs)
        Try
            My_CircleCount = Integer.Parse(TxtAantal.Text)
            My_CircleCount += 1
            TxtAantal.Text = My_CircleCount.ToString()
        Catch ex As Exception
            'Do nothing
        End Try
    End Sub

    Private Sub BtnAantalDown_Click(sender As Object, e As RoutedEventArgs)
        Try
            My_CircleCount = Integer.Parse(TxtAantal.Text)
            My_CircleCount -= 1
            If My_CircleCount < 2 Then My_CircleCount = 2
            TxtAantal.Text = My_CircleCount.ToString()
        Catch ex As Exception
            'Do nothing
        End Try
    End Sub

    Private Sub BtnRadiusUP_Click(sender As Object, e As RoutedEventArgs)
        Try
            My_RadiusFactor = Integer.Parse(TxtRadius.Text.Substring(0, 2))
            My_RadiusFactor += 5
            TxtRadius.Text = My_RadiusFactor.ToString() & "%"
        Catch ex As Exception
            'Do nothing
        End Try
    End Sub

    Private Sub BtnRadiusDown_Click(sender As Object, e As RoutedEventArgs)
        Try
            My_RadiusFactor = Integer.Parse(TxtRadius.Text.Substring(0, 2))
            My_RadiusFactor -= 5
            If My_RadiusFactor < 10 Then My_RadiusFactor = 10
            TxtRadius.Text = My_RadiusFactor.ToString() & "%"
        Catch ex As Exception
            'Do nothing
        End Try
    End Sub

    Private Sub BtnSpeedUP_Click(sender As Object, e As RoutedEventArgs)
        Try
            my_SpeedFactor = Integer.Parse(TxtSpeed.Text)
            my_SpeedFactor += 1
            TxtSpeed.Text = my_SpeedFactor.ToString()
        Catch ex As Exception
            'Do nothing
        End Try
    End Sub

    Private Sub BtnSpeedDown_Click(sender As Object, e As RoutedEventArgs)
        Try
            my_SpeedFactor = Integer.Parse(TxtSpeed.Text)
            my_SpeedFactor -= 1
            TxtSpeed.Text = my_SpeedFactor.ToString()
        Catch ex As Exception
            'Do nothing
        End Try
    End Sub
End Class
