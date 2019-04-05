Imports System.Windows.Threading
Imports System.Threading

Class MainWindow
    Public Delegate Sub WaitDelegate(ByVal t As Integer)
    Private Rendering As Boolean = False
    Private WaitTime As Integer = 25
    Private display(5) As SevenSegmentDisplay
    Private Number As Integer = 0
    Private setform As Settings

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim displayleft As Double = 0.02 * Canvas1.ActualWidth
        Dim displaytop As Double = 0.1 * Canvas1.ActualHeight
        Dim displayheight As Double = 0.8 * Canvas1.ActualHeight
        Dim displayWidth As Double = (Canvas1.ActualWidth - 50) / 6
        For I As Integer = 0 To 5
            display(I) = New SevenSegmentDisplay(displayWidth, displayheight, displayleft, displaytop)
            displayleft += display(I).Width + 5
            display(I).Value = 0
            display(I).Draw(Canvas1)
        Next
        display(2).ShowDot = True
        setform = New Settings(Me)
        setform.Show()
        setform.Left = Me.Left + Me.Width
        setform.Top = Me.Top
        setform.CmbBackgroundColor.SelectedItem = "Black"
        setform.CmbSegmentColor.SelectedItem = "Red"
        setform.TxtBorder.Text = display(0).Border.ToString()
        setform.TxtCampher.Text = display(0).Campher.ToString()
        setform.TxtSpace.Text = display(0).SegmentSpace.ToString()
        setform.TxtThickness.Text = display(0).SegmentThickness.ToString()
        setform.CbHasCampher.IsChecked = display(0).HasCampher
        setform.CbIsTilted.IsChecked = display(0).IsTilted
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        If Not Rendering Then
            BtnStart.Content = "STOP"
            Rendering = True
            Render()
        Else
            BtnStart.Content = "START"
            Rendering = False
        End If
    End Sub

    Private Sub Render()
        Do While Rendering
            Number += 1
            If Number > 999999 Then Number = 0
            For I As Integer = 0 To 5
                display(I).Value = CInt(Math.Floor(Number / Math.Pow(10, (5 - I)))) Mod 10
            Next
            Me.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, New WaitDelegate(AddressOf Wait), WaitTime)
        Loop
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If Me.IsLoaded Then
            Dim displayleft As Double = 0.02 * Canvas1.ActualWidth
            Dim displaytop As Double = 0.1 * Canvas1.ActualHeight
            Dim displayheight As Double = 0.8 * Canvas1.ActualHeight
            Dim displayWidth As Double = (Canvas1.ActualWidth - 50) / 6
            For I As Integer = 0 To 5
                display(I).Left = displayleft
                display(I).Top = displaytop
                display(I).Height = displayheight
                display(I).Width = displayWidth
                displayleft += display(I).Width + 5
                display(I).Value = CInt(Math.Floor(Number / Math.Pow(10, (5 - I)))) Mod 10
            Next
            setform.Left = Me.Left + Me.Width
            setform.Top = Me.Top
        End If
    End Sub

    Public Sub UpdateDisplays()
        For I As Integer = 0 To 5
            display(I).Backcolor = setform.BackColor
            display(I).Border = setform.Border
            display(I).HasCampher = setform.HasCampher
            If setform.HasCampher Then
                display(I).Campher = setform.Campher
            Else
                display(I).Campher = 0.0
            End If
            display(I).IsTilted = setform.IsTilted
            display(I).SegmentSpace = setform.Space
            display(I).SegmentThickness = setform.Thickness
            display(I).SegmentColor = setform.SegmentColor
        Next
    End Sub

    Public Sub SetDefault()
        For I As Integer = 0 To 5
            display(I).setDefault()
        Next
        setform.CmbBackgroundColor.SelectedItem = "Black"
        setform.CmbSegmentColor.SelectedItem = "Red"
        setform.TxtBorder.Text = display(0).Border.ToString()
        setform.TxtCampher.Text = display(0).Campher.ToString()
        setform.TxtSpace.Text = display(0).SegmentSpace.ToString()
        setform.TxtThickness.Text = display(0).SegmentThickness.ToString()
        setform.CbHasCampher.IsChecked = display(0).HasCampher
        setform.CbIsTilted.IsChecked = display(0).IsTilted
    End Sub

    Private Sub Wait(ByVal t As Integer)
        Thread.Sleep(t)
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

    Private Sub Window_LocationChanged(sender As Object, e As EventArgs)
        setform.Left = Me.Left + Me.Width
        setform.Top = Me.Top
    End Sub
End Class
