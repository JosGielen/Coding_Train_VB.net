Public Class Settings
    Private MyMain As Window
    Private MyBrushes As List(Of Brush)
    Private MyBrushStrings As List(Of String)
    Private MyFractalType As String = ""
    Private MyInitialString As String = ""
    Private MyInitialLength As Double = 0.0
    Private MyInitialAngle As Integer = 0
    Private MyShowLeaves As Boolean = False
    Private MyLeavesSize As Integer = 6
    Private MyMaxIter As Integer = 0
    Private MyStartPosX As Double = 0.0
    Private MyStartPosY As Double = 0.0
    Private MyBranchColor As Brush = Brushes.Brown
    Private MyLeavesColor As Brush = Brushes.Green
    Private MyLengthScaling As Double = 0.0
    Private MyBranchVariation As Boolean = False
    Private MyBranchStartThickness As Integer = 12
    Private MyDeflectionAngle As Double = 0
    Private MyAllowRandom As Boolean = False
    Private MyRandomPercentage As Integer = 20
    Private MyArule As String = ""
    Private MyBrule As String = ""
    Private MyCrule As String = ""
    Private MyXrule As String = ""
    Private MyYrule As String = ""
    Private MyZrule As String = ""

    Public Sub New(main As Window)
        ' This call is required by the designer.
        InitializeComponent()
        MyMain = main
        'Fill the combobox with all standard WPF brushes
        MyBrushes = New List(Of Brush)
        MyBrushStrings = New List(Of String)
        Dim brushType As Type = GetType(Brushes)
        Dim bc As BrushConverter = New BrushConverter()
        For Each propinfo As System.Reflection.PropertyInfo In brushType.GetProperties
            If propinfo.PropertyType = GetType(SolidColorBrush) Then
                CmbBranchColor.Items.Add(propinfo.Name)
                CmbLeavesColor.Items.Add(propinfo.Name)
                MyBrushes.Add(CType(bc.ConvertFromString(propinfo.Name), Brush))
                MyBrushStrings.Add(CType(bc.ConvertFromString(propinfo.Name), Brush).ToString())
            End If
        Next
    End Sub

    Public Property FractalType As String
        Get
            Return MyFractalType
        End Get
        Set(value As String)
            MyFractalType = value
            TxtFractalType.Text = MyFractalType
        End Set
    End Property

    Public Property InitialString As String
        Get
            Return MyInitialString
        End Get
        Set(value As String)
            MyInitialString = value
            TxtInitialString.Text = MyInitialString
        End Set
    End Property

    Public Property InitialLength As Double
        Get
            Return MyInitialLength
        End Get
        Set(value As Double)
            MyInitialLength = value
            TxtInitialLength.Text = MyInitialLength.ToString()
        End Set
    End Property

    Public Property InitialAngle As Integer
        Get
            Return MyInitialAngle
        End Get
        Set(value As Integer)
            MyInitialAngle = value
            TxtInitialAngle.Text = MyInitialAngle.ToString()
        End Set
    End Property

    Public Property ShowLeaves As Boolean
        Get
            Return MyShowLeaves
        End Get
        Set(value As Boolean)
            MyShowLeaves = value
            cbShowLeaves.IsChecked = MyShowLeaves
        End Set
    End Property

    Public Property LeavesSize As Integer
        Get
            Return MyLeavesSize
        End Get
        Set(value As Integer)
            MyLeavesSize = value
            TxtLeaveSize.Text = MyLeavesSize.ToString()
        End Set
    End Property

    Public Property MaxIter As Integer
        Get
            Return MyMaxIter
        End Get
        Set(value As Integer)
            MyMaxIter = value
            TxtMaxIterations.Text = MyMaxIter.ToString()
        End Set
    End Property

    Public Property StartPosX As Double
        Get
            Return MyStartPosX
        End Get
        Set(value As Double)
            MyStartPosX = value
            TxtStartPosX.Text = MyStartPosX.ToString()
        End Set
    End Property

    Public Property StartPosY As Double
        Get
            Return MyStartPosY
        End Get
        Set(value As Double)
            MyStartPosY = value
            TxtStartPosY.Text = MyStartPosY.ToString()
        End Set
    End Property

    Public Property BranchColor As Brush
        Get
            Return MyBranchColor
        End Get
        Set(value As Brush)
            MyBranchColor = value
            Dim index As Integer = MyBrushStrings.IndexOf(MyBranchColor.ToString())
            CmbBranchColor.SelectedIndex = index
        End Set
    End Property

    Public Property LeavesColor As Brush
        Get
            Return MyLeavesColor
        End Get
        Set(value As Brush)
            MyLeavesColor = value
            Dim index As Integer = MyBrushStrings.IndexOf(MyLeavesColor.ToString())
            CmbLeavesColor.SelectedIndex = index
        End Set
    End Property

    Public Property LengthScaling As Double
        Get
            Return MyLengthScaling
        End Get
        Set(value As Double)
            MyLengthScaling = value
            TxtLengthScale.Text = MyLengthScaling.ToString()
        End Set
    End Property

    Public Property BranchVariation As Boolean
        Get
            Return MyBranchVariation
        End Get
        Set(value As Boolean)
            MyBranchVariation = value
            cbBranchVariation.IsChecked = MyBranchVariation
        End Set
    End Property

    Public Property BranchStartThickness As Integer
        Get
            Return MyBranchStartThickness
        End Get
        Set(value As Integer)
            MyBranchStartThickness = value
            TxtBranchStartSize.Text = MyBranchStartThickness.ToString()
        End Set
    End Property

    Public Property DeflectionAngle As Double
        Get
            Return MyDeflectionAngle
        End Get
        Set(value As Double)
            MyDeflectionAngle = value
            TxtDeflectionAngle.Text = MyDeflectionAngle.ToString()
        End Set
    End Property

    Public Property AllowRandom As Boolean
        Get
            Return MyAllowRandom
        End Get
        Set(value As Boolean)
            MyAllowRandom = value
            cbAllowRandom.IsChecked = MyAllowRandom
        End Set
    End Property

    Public Property RandomPercentage As Integer
        Get
            Return MyRandomPercentage
        End Get
        Set(value As Integer)
            MyRandomPercentage = value
            TxtRandomPercentage.Text = MyRandomPercentage.ToString()
        End Set
    End Property

    Public Property A_rule As String
        Get
            Return MyArule
        End Get
        Set(value As String)
            MyArule = value
            TxtArule.Text = MyArule
        End Set
    End Property

    Public Property B_rule As String
        Get
            Return MyBrule
        End Get
        Set(value As String)
            MyBrule = value
            TxtBrule.Text = MyBrule
        End Set
    End Property

    Public Property C_rule As String
        Get
            Return MyCrule
        End Get
        Set(value As String)
            MyCrule = value
            TxtCrule.Text = MyCrule
        End Set
    End Property

    Public Property X_rule As String
        Get
            Return MyXrule
        End Get
        Set(value As String)
            MyXrule = value
            TxtXrule.Text = MyXrule
        End Set
    End Property

    Public Property Y_rule As String
        Get
            Return MyYrule
        End Get
        Set(value As String)
            MyYrule = value
            TxtYrule.Text = MyYrule
        End Set
    End Property

    Public Property Z_rule As String
        Get
            Return MyZrule
        End Get
        Set(value As String)
            MyZrule = value
            TxtZrule.Text = MyZrule
        End Set
    End Property

    Private Sub BtnStart_Click(sender As Object, e As RoutedEventArgs)
        Try
            MyFractalType = TxtFractalType.Text
            MyInitialString = TxtInitialString.Text
            MyInitialLength = Double.Parse(TxtInitialLength.Text)
            MyInitialAngle = Integer.Parse(TxtInitialAngle.Text)
            MyShowLeaves = CType(cbShowLeaves.IsChecked, Boolean)
            MyLeavesSize = Integer.Parse(TxtLeaveSize.Text)
            MyMaxIter = Integer.Parse(TxtMaxIterations.Text)
            MyStartPosX = Double.Parse(TxtStartPosX.Text)
            MyStartPosY = Double.Parse(TxtStartPosY.Text)
            MyBranchColor = MyBrushes.Item(CmbBranchColor.SelectedIndex)
            MyLeavesColor = MyBrushes.Item(CmbLeavesColor.SelectedIndex)
            MyLengthScaling = Double.Parse(TxtLengthScale.Text)
            MyBranchVariation = CType(cbBranchVariation.IsChecked, Boolean)
            MyBranchStartThickness = Integer.Parse(TxtBranchStartSize.Text)
            MyDeflectionAngle = Integer.Parse(TxtDeflectionAngle.Text)
            MyAllowRandom = CType(cbAllowRandom.IsChecked, Boolean)
            MyRandomPercentage = Integer.Parse(TxtRandomPercentage.Text)
            MyArule = TxtArule.Text
            MyBrule = TxtBrule.Text
            MyCrule = TxtCrule.Text
            MyXrule = TxtXrule.Text
            MyYrule = TxtYrule.Text
            MyZrule = TxtZrule.Text
            CType(MyMain, MainWindow).GetParameters()
        Catch ex As Exception
            MessageBox.Show("The Parameters are not valid.", "L-System settings error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As RoutedEventArgs)
        CType(MyMain, MainWindow).MnuShowSettings.IsChecked = False
        Me.Hide()
    End Sub
End Class
