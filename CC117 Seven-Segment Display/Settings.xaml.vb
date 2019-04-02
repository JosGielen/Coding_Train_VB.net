Public Class Settings
    Private my_Parent As MainWindow
    Public Border As Double
    Public Thickness As Double
    Public Campher As Double
    Public Space As Double
    Public HasCampher As Boolean
    Public IsTilted As Boolean
    Public BackColor As Brush
    Public SegmentColor As Brush
    Private My_Brushes As List(Of Brush)

    Public Sub New(parent As MainWindow)
        InitializeComponent()
        my_Parent = parent
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        'Load the WPF default colors in the Color comboboxes.
        My_Brushes = New List(Of Brush)
        Dim BrushesType As Type = GetType(Brushes)
        Dim bc As BrushConverter = New BrushConverter()
        For Each propinfo As System.Reflection.PropertyInfo In BrushesType.GetProperties
            If propinfo.PropertyType = GetType(SolidColorBrush) Then
                CmbSegmentColor.Items.Add(propinfo.Name)
                CmbBackgroundColor.Items.Add(propinfo.Name)
                My_Brushes.Add(CType(bc.ConvertFromString(propinfo.Name), Brush))
            End If
        Next
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        End
    End Sub

    Private Sub BtnApply_Click(sender As Object, e As RoutedEventArgs)
        Border = Double.Parse(TxtBorder.Text)
        Thickness = Double.Parse(TxtThickness.Text)
        Campher = Double.Parse(TxtCampher.Text)
        Space = Double.Parse(TxtSpace.Text)
        HasCampher = CbHasCampher.IsChecked.Value
        IsTilted = CbIsTilted.IsChecked.Value
        BackColor = My_Brushes(CmbBackgroundColor.SelectedIndex)
        SegmentColor = My_Brushes(CmbSegmentColor.SelectedIndex)
        my_Parent.UpdateDisplays()
    End Sub

    Private Sub BtnDefault_Click(sender As Object, e As RoutedEventArgs)
        my_Parent.SetDefault()
    End Sub
End Class
