Public Class Settings
    Private MyMain As Window
    Private MyKnotType As Integer = 1
    Private AllowUpdate As Boolean = False
    Public A1 As Double = 0.0
    Public A2 As Double = 0.0
    Public A3 As Double = 0.0
    Public A4 As Double = 0.0
    Public A5 As Double = 0.0
    Public A6 As Double = 0.0
    Public A7 As Double = 0.0
    Public A8 As Double = 0.0
    Public B1 As Double = 0.0
    Public B2 As Double = 0.0
    Public B3 As Double = 0.0
    Public B4 As Double = 0.0
    Public B5 As Double = 0.0
    Public B6 As Double = 0.0
    Public B7 As Double = 0.0
    Public B8 As Double = 0.0
    Public C1 As Double = 0.0
    Public C2 As Double = 0.0
    Public C3 As Double = 0.0
    Public C4 As Double = 0.0
    Public C5 As Double = 0.0
    Public C6 As Double = 0.0
    Public C7 As Double = 0.0
    Public C8 As Double = 0.0

    Public Sub New(main As Window)
        ' This call is required by the designer.
        InitializeComponent()
        MyMain = main
        AllowUpdate = True
    End Sub

    Public Property KnotType As Integer
        Get
            Return MyKnotType
        End Get
        Set(value As Integer)
            MyKnotType = value
        End Set
    End Property

    Private Sub KnotTypeChange(sender As Object, e As RoutedEventArgs)
        AllowUpdate = False
        MyKnotType = CmbKnotType.SelectedIndex + 1
        ClearParameters()
        Select Case MyKnotType
            Case 1
                TxtFormulas.Text = "X = A1 * cos(A2 * mu) + A3 * cos(A4 * mu) + A5 * cos(A6 * mu) + A7 * cos(A8 * mu)" & vbCrLf
                TxtFormulas.Text &= "Y = B1 * sin(B2 * mu) + B3 * sin(B4 * mu)" & vbCrLf
                TxtFormulas.Text &= "Z = C1 * sin(C2 * mu) * C3 * sin(C4 * mu) + C5 * sin(C6 * mu) + C7 * sin(C8 * mu)"
                A1 = 10.0
                A2 = 1.0
                A3 = 10.0
                A4 = 3.0
                A5 = 1.0
                A6 = 2.0
                A7 = 1.0
                A8 = 4.0
                B1 = 6.0
                B2 = 1.0
                B3 = 10.0
                B4 = 3.0
                C1 = 4.0
                C2 = 3.0
                C3 = 1.0
                C4 = 2.5
                C5 = 4.0
                C6 = 4.0
                C7 = -2.0
                C8 = 6.0
                SetEnabled("A1", True)
                SetEnabled("A2", True)
                SetEnabled("A3", True)
                SetEnabled("A4", True)
                SetEnabled("A5", True)
                SetEnabled("A6", True)
                SetEnabled("A7", True)
                SetEnabled("A8", True)
                SetEnabled("B1", True)
                SetEnabled("B2", True)
                SetEnabled("B3", True)
                SetEnabled("B4", True)
                SetEnabled("B5", False)
                SetEnabled("B6", False)
                SetEnabled("B7", False)
                SetEnabled("B8", False)
                SetEnabled("C1", True)
                SetEnabled("C2", True)
                SetEnabled("C3", True)
                SetEnabled("C4", True)
                SetEnabled("C5", True)
                SetEnabled("C6", True)
                SetEnabled("C7", True)
                SetEnabled("C8", True)
            Case 2
                TxtFormulas.Text = "X = A1 * cos(A2 * mu + A3) + A4 * cos(A5 * mu)" & vbCrLf
                TxtFormulas.Text &= "Y = B1 * sin(B2 * mu) + B3 * sin(B4 * mu)" & vbCrLf
                TxtFormulas.Text &= "Z = C1 * sin(C2 * mu) + C3 * sin(C4 * mu)"
                A1 = 4.0 / 3.0
                A2 = 1.0
                A3 = Math.PI
                A4 = 2.0
                A5 = 3.0
                B1 = 4.0 / 3.0
                B2 = 1.0
                B3 = 2.0
                B4 = 3.0
                C1 = 1.0
                C2 = 4.0
                C3 = 0.5
                C4 = 2.0
                SetEnabled("A1", True)
                SetEnabled("A2", True)
                SetEnabled("A3", True)
                SetEnabled("A4", True)
                SetEnabled("A5", True)
                SetEnabled("A6", False)
                SetEnabled("A7", False)
                SetEnabled("A8", False)
                SetEnabled("B1", True)
                SetEnabled("B2", True)
                SetEnabled("B3", True)
                SetEnabled("B4", True)
                SetEnabled("B5", False)
                SetEnabled("B6", False)
                SetEnabled("B7", False)
                SetEnabled("B8", False)
                SetEnabled("C1", True)
                SetEnabled("C2", True)
                SetEnabled("C3", True)
                SetEnabled("C4", True)
                SetEnabled("C5", False)
                SetEnabled("C6", False)
                SetEnabled("C7", False)
                SetEnabled("C8", False)
            Case 3
                TxtFormulas.Text = "X = cos(mu) * (1 + cos(A1 * mu / A2) / 2.0)" & vbCrLf
                TxtFormulas.Text &= "Y = sin(mu) * (1 + cos(B1 * mu / B2) / 2.0)" & vbCrLf
                TxtFormulas.Text &= "Z = sin(C1 * mu / C2) / 2.0"
                A1 = 7.0
                A2 = 4.0
                B1 = 7.0
                B2 = 4.0
                C1 = 7.0
                C2 = 4.0
                SetEnabled("A1", True)
                SetEnabled("A2", True)
                SetEnabled("A3", False)
                SetEnabled("A4", False)
                SetEnabled("A5", False)
                SetEnabled("A6", False)
                SetEnabled("A7", False)
                SetEnabled("A8", False)
                SetEnabled("B1", True)
                SetEnabled("B2", True)
                SetEnabled("B3", False)
                SetEnabled("B4", False)
                SetEnabled("B5", False)
                SetEnabled("B6", False)
                SetEnabled("B7", False)
                SetEnabled("B8", False)
                SetEnabled("C1", True)
                SetEnabled("C2", True)
                SetEnabled("C3", False)
                SetEnabled("C4", False)
                SetEnabled("C5", False)
                SetEnabled("C6", False)
                SetEnabled("C7", False)
                SetEnabled("C8", False)
            Case 4
                TxtFormulas.Text = "r = A1 + A2 * sin(A3 * beta)" & vbCrLf
                TxtFormulas.Text &= "theta = B1 * beta" & vbCrLf
                TxtFormulas.Text &= "phi = C1 * PI * sin(C2 * beta)"
                A1 = 0.8
                A2 = 1.6
                A3 = 6.0
                B1 = 2.0
                C1 = 0.6
                C2 = 12.0
                SetEnabled("A1", True)
                SetEnabled("A2", True)
                SetEnabled("A3", True)
                SetEnabled("A4", False)
                SetEnabled("A5", False)
                SetEnabled("A6", False)
                SetEnabled("A7", False)
                SetEnabled("A8", False)
                SetEnabled("B1", True)
                SetEnabled("B2", False)
                SetEnabled("B3", False)
                SetEnabled("B4", False)
                SetEnabled("B5", False)
                SetEnabled("B6", False)
                SetEnabled("B7", False)
                SetEnabled("B8", False)
                SetEnabled("C1", True)
                SetEnabled("C2", True)
                SetEnabled("C3", False)
                SetEnabled("C4", False)
                SetEnabled("C5", False)
                SetEnabled("C6", False)
                SetEnabled("C7", False)
                SetEnabled("C8", False)
            Case 5
                TxtFormulas.Text = "r = A1 * sin(A2 * beta + A3 * PI)" & vbCrLf
                TxtFormulas.Text &= "theta = B1 * beta" & vbCrLf
                TxtFormulas.Text &= "phi = C1 * PI * sin(C2 * beta)"
                A1 = 0.72
                A2 = 6.0
                A3 = 0.5
                B1 = 4.0
                C1 = 0.2
                C2 = 6.0
                SetEnabled("A1", True)
                SetEnabled("A2", True)
                SetEnabled("A3", True)
                SetEnabled("A4", False)
                SetEnabled("A5", False)
                SetEnabled("A6", False)
                SetEnabled("A7", False)
                SetEnabled("A8", False)
                SetEnabled("B1", True)
                SetEnabled("B2", False)
                SetEnabled("B3", False)
                SetEnabled("B4", False)
                SetEnabled("B5", False)
                SetEnabled("B6", False)
                SetEnabled("B7", False)
                SetEnabled("B8", False)
                SetEnabled("C1", True)
                SetEnabled("C2", True)
                SetEnabled("C3", False)
                SetEnabled("C4", False)
                SetEnabled("C5", False)
                SetEnabled("C6", False)
                SetEnabled("C7", False)
                SetEnabled("C8", False)
            Case 6
                TxtFormulas.Text = "X = cos(mu) * (A1 - cos(A2 * mu / (2 * A3 + 1)))" & vbCrLf
                TxtFormulas.Text &= "Y = sin(mu) * (B1 - cos(B2 * mu / (2 * A3 + 1)))" & vbCrLf
                TxtFormulas.Text &= "Z = -sin(C1 * mu / (2 * A3 + 1))"
                A1 = 2.0
                A2 = 2.0
                A3 = 2.0
                B1 = 2.0
                B2 = 2.0
                C1 = 2.0
                SetEnabled("A1", True)
                SetEnabled("A2", True)
                SetEnabled("A3", True)
                SetEnabled("A4", False)
                SetEnabled("A5", False)
                SetEnabled("A6", False)
                SetEnabled("A7", False)
                SetEnabled("A8", False)
                SetEnabled("B1", True)
                SetEnabled("B2", True)
                SetEnabled("B3", False)
                SetEnabled("B4", False)
                SetEnabled("B5", False)
                SetEnabled("B6", False)
                SetEnabled("B7", False)
                SetEnabled("B8", False)
                SetEnabled("C1", True)
                SetEnabled("C2", False)
                SetEnabled("C3", False)
                SetEnabled("C4", False)
                SetEnabled("C5", False)
                SetEnabled("C6", False)
                SetEnabled("C7", False)
                SetEnabled("C8", False)
            Case 7
                TxtFormulas.Text = "X = A1 * sin(A2 * mu) * cos(A3 * mu) + A4 * cos(A5 * mu) * sin(A6 * mu) + A7 * sin(A8 * mu)" & vbCrLf
                TxtFormulas.Text &= "Y = B1 * sin(B2 * mu) * sin(B3 * mu) + B4 * cos(B5 * mu) * cos(B6 * mu) + B7 * cos(B8 * mu)" & vbCrLf
                TxtFormulas.Text &= "Z = C1 * cos(C2 * mu)"
                A1 = -0.45
                A2 = 1.5
                A3 = 1.0
                A4 = -0.3
                A5 = 1.5
                A6 = 1.0
                A7 = -0.5
                A8 = 1.0
                B1 = -0.45
                B2 = 1.5
                B3 = 1.0
                B4 = 0.3
                B5 = 1.5
                B6 = 1.0
                B7 = 0.5
                B8 = 1.0
                C1 = 0.75
                C2 = 1.5
                SetEnabled("A1", True)
                SetEnabled("A2", True)
                SetEnabled("A3", True)
                SetEnabled("A4", True)
                SetEnabled("A5", True)
                SetEnabled("A6", True)
                SetEnabled("A7", True)
                SetEnabled("A8", True)
                SetEnabled("B1", True)
                SetEnabled("B2", True)
                SetEnabled("B3", True)
                SetEnabled("B4", True)
                SetEnabled("B5", True)
                SetEnabled("B6", True)
                SetEnabled("B7", True)
                SetEnabled("B8", True)
                SetEnabled("C1", True)
                SetEnabled("C2", True)
                SetEnabled("C3", False)
                SetEnabled("C4", False)
                SetEnabled("C5", False)
                SetEnabled("C6", False)
                SetEnabled("C7", False)
                SetEnabled("C8", False)
            Case 8
                TxtFormulas.Text = "X = A1 * cos(A2 * mu) + A3 * sin(A4 * mu) + A5 * cos(A6 * mu) + A7 * sin(A8 * mu)" & vbCrLf
                TxtFormulas.Text &= "Y = B1 * cos(B2 * mu) + B3 * sin(B4 * mu) + B5 * cos(B6 * mu) + B7 * sin(B8 * mu)" & vbCrLf
                TxtFormulas.Text &= "Z = C1 * cos(C2 * mu) + C3 * sin(C4 * mu)"
                a1 = -0.22
                a2 = 1.0
                a3 = -1.28
                a4 = 1.0
                a5 = -0.44
                a6 = 3.0
                a7 = -0.78
                a8 = 3.0
                b1 = -0.1
                b2 = 2.0
                b3 = -0.27
                b4 = 2.0
                b5 = 0.38
                b6 = 4.0
                b7 = 0.46
                b8 = 4.0
                c1 = 0.7
                c2 = 3.0
                c3 = -0.4
                C4 = 3.0
                SetEnabled("A1", True)
                SetEnabled("A2", True)
                SetEnabled("A3", True)
                SetEnabled("A4", True)
                SetEnabled("A5", True)
                SetEnabled("A6", True)
                SetEnabled("A7", True)
                SetEnabled("A8", True)
                SetEnabled("B1", True)
                SetEnabled("B2", True)
                SetEnabled("B3", True)
                SetEnabled("B4", True)
                SetEnabled("B5", True)
                SetEnabled("B6", True)
                SetEnabled("B7", True)
                SetEnabled("B8", True)
                SetEnabled("C1", True)
                SetEnabled("C2", True)
                SetEnabled("C3", True)
                SetEnabled("C4", True)
                SetEnabled("C5", False)
                SetEnabled("C6", False)
                SetEnabled("C7", False)
                SetEnabled("C8", False)
        End Select
        UpdateParameters()
        AllowUpdate = True
        CType(MyMain, MainWindow).GetParameters()
    End Sub

    Private Sub ClearParameters()
        A1 = 0
        A2 = 0
        A3 = 0
        A4 = 0
        A5 = 0
        A6 = 0
        A7 = 0
        A8 = 0
        B1 = 0
        B2 = 0
        B3 = 0
        B4 = 0
        B5 = 0
        B6 = 0
        B7 = 0
        B8 = 0
        C1 = 0
        C2 = 0
        C3 = 0
        C4 = 0
        C5 = 0
        C6 = 0
        C7 = 0
        C8 = 0
    End Sub

    Private Sub UpdateParameters()
        SldA1.Value = A1
        TxtA1.Text = A1.ToString()
        SldA2.Value = A2
        TxtA2.Text = A2.ToString()
        SldA3.Value = A3
        TxtA3.Text = A3.ToString()
        SldA4.Value = A4
        TxtA4.Text = A4.ToString()
        SldA5.Value = A5
        TxtA5.Text = A5.ToString()
        SldA6.Value = A6
        TxtA6.Text = A6.ToString()
        SldA7.Value = A7
        TxtA7.Text = A7.ToString()
        SldA8.Value = A8
        TxtA8.Text = A8.ToString()
        SldB1.Value = B1
        TxtB1.Text = B1.ToString()
        SldB2.Value = B2
        TxtB2.Text = B2.ToString()
        SldB3.Value = B3
        TxtB3.Text = B3.ToString()
        SldB4.Value = B4
        TxtB4.Text = B4.ToString()
        SldB5.Value = B5
        TxtB5.Text = B5.ToString()
        SldB6.Value = B6
        TxtB6.Text = B6.ToString()
        SldB7.Value = B7
        TxtB7.Text = B7.ToString()
        SldB8.Value = B8
        TxtB8.Text = B8.ToString()
        SldC1.Value = C1
        TxtC1.Text = C1.ToString()
        SldC2.Value = C2
        TxtC2.Text = C2.ToString()
        SldC3.Value = C3
        TxtC3.Text = C3.ToString()
        SldC4.Value = C4
        TxtC4.Text = C4.ToString()
        SldC5.Value = C5
        TxtC5.Text = C5.ToString()
        SldC6.Value = C6
        TxtC6.Text = C6.ToString()
        SldC7.Value = C7
        TxtC7.Text = C7.ToString()
        SldC8.Value = C8
        Txtc8.Text = C8.ToString()
    End Sub

    Private Sub SetEnabled(Parameter As String, Enabled As Boolean)
        Select Case Parameter
            Case "A1"
                SldA1.IsEnabled = Enabled
                TxtA1.IsEnabled = Enabled
            Case "A2"
                SldA2.IsEnabled = Enabled
                TxtA2.IsEnabled = Enabled
            Case "A3"
                SldA3.IsEnabled = Enabled
                TxtA3.IsEnabled = Enabled
            Case "A4"
                SldA4.IsEnabled = Enabled
                TxtA4.IsEnabled = Enabled
            Case "A5"
                SldA5.IsEnabled = Enabled
                TxtA5.IsEnabled = Enabled
            Case "A6"
                SldA6.IsEnabled = Enabled
                TxtA6.IsEnabled = Enabled
            Case "A7"
                SldA7.IsEnabled = Enabled
                TxtA7.IsEnabled = Enabled
            Case "A8"
                SldA8.IsEnabled = Enabled
                TxtA8.IsEnabled = Enabled
            Case "B1"
                SldB1.IsEnabled = Enabled
                TxtB1.IsEnabled = Enabled
            Case "B2"
                SldB2.IsEnabled = Enabled
                TxtB2.IsEnabled = Enabled
            Case "B3"
                SldB3.IsEnabled = Enabled
                TxtB3.IsEnabled = Enabled
            Case "B4"
                SldB4.IsEnabled = Enabled
                TxtB4.IsEnabled = Enabled
            Case "B5"
                SldB5.IsEnabled = Enabled
                TxtB5.IsEnabled = Enabled
            Case "B6"
                SldB6.IsEnabled = Enabled
                TxtB6.IsEnabled = Enabled
            Case "B7"
                SldB7.IsEnabled = Enabled
                TxtB7.IsEnabled = Enabled
            Case "B8"
                SldB8.IsEnabled = Enabled
                TxtB8.IsEnabled = Enabled
            Case "C1"
                SldC1.IsEnabled = Enabled
                TxtC1.IsEnabled = Enabled
            Case "C2"
                SldC2.IsEnabled = Enabled
                TxtC2.IsEnabled = Enabled
            Case "C3"
                SldC3.IsEnabled = Enabled
                TxtC3.IsEnabled = Enabled
            Case "C4"
                SldC4.IsEnabled = Enabled
                TxtC4.IsEnabled = Enabled
            Case "C5"
                SldC5.IsEnabled = Enabled
                TxtC5.IsEnabled = Enabled
            Case "C6"
                SldC6.IsEnabled = Enabled
                TxtC6.IsEnabled = Enabled
            Case "C7"
                SldC7.IsEnabled = Enabled
                TxtC7.IsEnabled = Enabled
            Case "C8"
                SldC8.IsEnabled = Enabled
                Txtc8.IsEnabled = Enabled
        End Select
    End Sub

    Private Sub ParameterChange(sender As Object, e As RoutedEventArgs)
        If AllowUpdate Then
            A1 = SldA1.Value
            TxtA1.Text = A1.ToString()
            A2 = SldA2.Value
            TxtA2.Text = A2.ToString()
            A3 = SldA3.Value
            TxtA3.Text = A3.ToString()
            A4 = SldA4.Value
            TxtA4.Text = A4.ToString()
            A5 = SldA5.Value
            TxtA5.Text = A5.ToString()
            A6 = SldA6.Value
            TxtA6.Text = A6.ToString()
            A7 = SldA7.Value
            TxtA7.Text = A7.ToString()
            A8 = SldA8.Value
            TxtA8.Text = A8.ToString()
            B1 = SldB1.Value
            TxtB1.Text = B1.ToString()
            B2 = SldB2.Value
            TxtB2.Text = B2.ToString()
            B3 = SldB3.Value
            TxtB3.Text = B3.ToString()
            B4 = SldB4.Value
            TxtB4.Text = B4.ToString()
            B5 = SldB5.Value
            TxtB5.Text = B5.ToString()
            B6 = SldB6.Value
            TxtB6.Text = B6.ToString()
            B7 = SldB7.Value
            TxtB7.Text = B7.ToString()
            B8 = SldB8.Value
            TxtB8.Text = B8.ToString()
            C1 = SldC1.Value
            TxtC1.Text = C1.ToString()
            C2 = SldC2.Value
            TxtC2.Text = C2.ToString()
            C3 = SldC3.Value
            TxtC3.Text = C3.ToString()
            C4 = SldC4.Value
            TxtC4.Text = C4.ToString()
            C5 = SldC5.Value
            TxtC5.Text = C5.ToString()
            C6 = SldC6.Value
            TxtC6.Text = C6.ToString()
            C7 = SldC7.Value
            TxtC7.Text = C7.ToString()
            C8 = SldC8.Value
            Txtc8.Text = C8.ToString()
            CType(MyMain, MainWindow).GetParameters()
        End If
    End Sub

    Private Sub TxtRotation_TextChanged(sender As Object, e As TextChangedEventArgs)
        If AllowUpdate Then
            Dim rotspeed As Double = Double.Parse(TxtRotation.Text)
            CType(MyMain, MainWindow).SetRotation(rotspeed)
        End If
    End Sub

    Private Sub CBTexture_Click(sender As Object, e As RoutedEventArgs)
        If AllowUpdate Then CType(MyMain, MainWindow).SetShowTexture(CBTexture.IsChecked.Value)
    End Sub

    Private Sub CmbDrawMode_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If AllowUpdate Then CType(MyMain, MainWindow).SetDrawMode(CmbDrawMode.SelectedIndex)
    End Sub
End Class
