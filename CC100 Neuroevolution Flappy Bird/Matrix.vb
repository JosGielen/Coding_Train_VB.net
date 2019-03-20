Public Class Matrix
    Implements ICloneable

    Private my_cols As Integer
    Private my_rows As Integer
    Private my_values(,) As Double
    Private currentX As Integer
    Private currentY As Integer
    Private Shared Rnd As Random = New Random()
    Public Delegate Function FunctionDelegate(ByVal value As Double) As Double

#Region "Constructors"

    Public Sub New(ByVal rows As Integer, ByVal columns As Integer)
        my_cols = columns
        my_rows = rows
        ReDim my_values(my_rows - 1, my_cols - 1)
        currentX = 0
        currentY = 0
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                my_values(I, J) = 0.0
            Next
        Next
    End Sub

    Public Sub New(ByVal size As Integer)
        my_cols = size
        my_rows = size
        ReDim my_values(my_rows - 1, my_cols - 1)
        currentX = 0
        currentY = 0
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                my_values(I, J) = 0.0
            Next
        Next
    End Sub

    Public Shared Function Unity(size As Integer) As Matrix
        Dim result As Matrix = New Matrix(size)
        For I As Integer = 0 To size - 1
            result.Value(I, I) = 1.0
        Next
        Return result
    End Function

    Public Shared Function FromArray(values() As Double) As Matrix 'Make a single column Matrix (= Vector)
        If Not values.IsFixedSize Then
            Throw New Exception("FromArray: The values array must have a fixed size.")
        End If
        Dim result As Matrix = New Matrix(values.Length, 1)
        For I As Integer = 0 To values.Length - 1
            result.Value(I, 0) = values(I)
        Next
        Return result
    End Function

    Public Shared Function FromArray(values(,) As Double) As Matrix 'Make a 2D MAtrix
        If Not values.IsFixedSize Then
            Throw New Exception("FromArray: The values array must have a fixed size.")
        End If
        Dim result As Matrix = New Matrix(values.GetLength(0), values.GetLength(1))
        For I As Integer = 0 To values.GetLength(0) - 1
            For J As Integer = 0 To values.GetLength(1) - 1
                result.Value(I, J) = values(I, J)
            Next
        Next
        Return result
    End Function

    Public Shared Function FromString(s As String) As Matrix 'Format = [[x;x;x;..][x;x;x;...]....]
        Dim result As Matrix
        Dim FormatOK As Boolean = True
        Dim rowbegins As Integer = 0
        Dim rowstrings As List(Of String) = New List(Of String)
        Dim rowstring As String = ""
        Dim cols As Integer = 0
        Dim colData() As String

        If s(0) <> "["c Or s(s.Length - 1) <> "]"c Then FormatOK = False
        For I As Integer = 1 To s.Length - 2
            If s(I) = "[" Then
                rowbegins += 1
                rowstring = ""
            ElseIf s(I) = "]" Then
                rowstrings.Add(rowstring)
            Else
                rowstring &= s(I)
            End If
        Next
        If rowbegins <> rowstrings.Count Then FormatOK = False
        Try
            If FormatOK Then
                cols = rowstrings.First.Split(";"c).Length
                ReDim colData(cols)
                result = New Matrix(rowbegins, cols)
                For I As Integer = 0 To rowstrings.Count - 1
                    colData = rowstrings(I).Split(";"c)
                    If colData.Length <> cols Then
                        FormatOK = False
                        Exit For
                    End If
                    For J As Integer = 0 To colData.Length - 1
                        result.Value(I, J) = Double.Parse(colData(J))
                    Next
                Next
            Else
                Throw New Exception("FromString: The string has an invalid format." & vbCrLf & "Correct Format = [[x;x;x;..][x;x;x;...]....]")
            End If
        Catch fex As FormatException
            Throw New Exception("FromString: The Values in the string can not be parsed into Double numbers")
        Catch ex As Exception
            Throw New Exception("FromString: The string has an invalid format." & vbCrLf & "Correct Format = [[x;x;x;..][x;x;x;...]....]")
        End Try
        Return result
    End Function

#End Region

#Region "Properties"

    Public ReadOnly Property Columns As Integer
        Get
            Return my_cols
        End Get
    End Property

    Public ReadOnly Property Rows As Integer
        Get
            Return my_rows
        End Get
    End Property

    Public Property Value(row As Integer, col As Integer) As Double
        Get
            Return my_values(row, col)
        End Get
        Set(value As Double)
            my_values(row, col) = value
        End Set
    End Property

#End Region

#Region "Self modification operations"

    Public Sub Randomize()
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                my_values(I, J) = 2.0 * Rnd.NextDouble() - 1.0
            Next
        Next
    End Sub

    Public Sub RandomizeNormal(mean As Double, stdDev As Double)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                my_values(I, J) = NormalDist(mean, stdDev)
            Next
        Next
    End Sub

    Public Sub Randomize(lowLimit As Double, highLimit As Double)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                my_values(I, J) = lowLimit + (highLimit - lowLimit) * Rnd.NextDouble()
            Next
        Next
    End Sub

    Public Sub AddScalar(scalar As Double)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                my_values(I, J) += scalar
            Next
        Next
    End Sub

    Public Sub MultiplyScalar(scalar As Double)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                my_values(I, J) = my_values(I, J) * scalar
            Next
        Next
    End Sub

    Public Sub AddMatrix(C As Matrix)
        If my_rows <> C.Rows Or my_cols <> C.Columns Then
            Throw New Exception("AddMatrix: Matrices size Mismatch.")
        End If
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                my_values(I, J) = my_values(I, J) + C.Value(I, J)
            Next
        Next
    End Sub

    Public Sub MultiplyHadamard(ByVal C As Matrix)
        If my_rows <> C.Rows Or my_cols <> C.Columns Then
            Throw New Exception("MatrixAdd: Matrices size Mismatch.")
        End If
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                my_values(I, J) = my_values(I, J) * C.Value(I, J)
            Next
        Next
    End Sub

    Public Sub Transpose()
        Dim result As Matrix = New Matrix(my_cols, my_rows)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                result.Value(J, I) = my_values(I, J)
            Next
        Next
        my_rows = result.Rows
        my_cols = result.Columns
        ReDim my_values(my_rows, my_cols)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                my_values(I, J) = result.Value(I, J)
            Next
        Next
    End Sub

    Public Sub Map(ByVal mapFunction As FunctionDelegate)
        Dim val As Double = 0.0
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                val = my_values(I, J)
                my_values(I, J) = mapFunction(val)
            Next
        Next
    End Sub

#End Region

#Region "New Matrix Operations"

    Public Function ScalarAdd(scalar As Double) As Matrix
        Dim result As Matrix = New Matrix(my_rows, my_cols)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                result.Value(I, J) = my_values(I, J) + scalar
            Next
        Next
        Return result
    End Function

    Public Function ScalarMultiply(scalar As Double) As Matrix
        Dim result As Matrix = New Matrix(my_rows, my_cols)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                result.Value(I, J) = my_values(I, J) * scalar
            Next
        Next
        Return result
    End Function

    Public Function MatrixAdd(ByVal C As Matrix) As Matrix
        If my_rows <> C.Rows Or my_cols <> C.Columns Then
            Throw New Exception("MatrixAdd: Matrices size Mismatch.")
        End If
        Dim result As Matrix = New Matrix(my_rows, my_cols)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                result.Value(I, J) = my_values(I, J) + C.Value(I, J)
            Next
        Next
        Return result
    End Function

    Public Function HadamardMultiply(ByVal C As Matrix) As Matrix
        If my_rows <> C.Rows Or my_cols <> C.Columns Then
            Throw New Exception("MatrixAdd: Matrices size Mismatch.")
        End If
        Dim B As Matrix = New Matrix(my_rows, my_cols)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                B.Value(I, J) = my_values(I, J) * C.Value(I, J)
            Next
        Next
        Return B
    End Function

    Public Function MatrixMultiply(ByVal C As Matrix) As Matrix
        If my_cols <> C.Rows Or my_cols <> C.Rows Then
            Throw New Exception("MatrixMultiply: Matrices size Mismatch.")
        End If
        Dim B As Matrix = New Matrix(my_rows, C.Columns)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To C.Columns - 1
                For K As Integer = 0 To my_cols - 1 ' or 0 to C.rows -1
                    B.Value(I, J) += my_values(I, K) * C.Value(K, J)
                Next
            Next
        Next
        Return B
    End Function

    Public Function MapTo(ByVal mapFunction As FunctionDelegate) As Matrix
        Dim result As Matrix = New Matrix(my_rows, my_cols)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                result.Value(I, J) = mapFunction(my_values(I, J))
            Next
        Next
        Return result
    End Function

#End Region


#Region "Shared Operators"

    Public Shared Operator +(C As Matrix) As Matrix
        Return C
    End Operator

    Public Shared Operator -(C As Matrix) As Matrix
        Dim result As Matrix = New Matrix(C.Rows, C.Columns)
        For I As Integer = 0 To C.Rows - 1
            For J As Integer = 0 To C.Columns - 1
                result.Value(I, J) = -1 * C.Value(I, J)
            Next
        Next
        Return result
    End Operator

    Public Shared Operator +(left As Matrix, right As Matrix) As Matrix
        If left.Rows <> right.Rows Or left.Columns <> right.Columns Then
            Throw New Exception("Operator + : Matrices size Mismatch.")
        End If
        Dim result As Matrix = New Matrix(left.Rows, left.Columns)
        For I As Integer = 0 To left.Rows - 1
            For J As Integer = 0 To left.Columns - 1
                result.Value(I, J) = left.Value(I, J) + right.Value(I, J)
            Next
        Next
        Return result
    End Operator

    Public Shared Operator -(left As Matrix, right As Matrix) As Matrix
        If left.Rows <> right.Rows Or left.Columns <> right.Columns Then
            Throw New Exception("Operator - : Matrices size Mismatch.")
        End If
        Dim result As Matrix = New Matrix(left.Rows, left.Columns)
        For I As Integer = 0 To left.Rows - 1
            For J As Integer = 0 To left.Columns - 1
                result.Value(I, J) = left.Value(I, J) - right.Value(I, J)
            Next
        Next
        Return result
    End Operator

    Public Shared Operator *(left As Matrix, right As Matrix) As Matrix
        If left.Columns <> right.Rows Then
            Throw New Exception("Operator * : Matrices size Mismatch.")
        End If
        Dim B As Matrix = New Matrix(left.Rows, right.Columns)
        For I As Integer = 0 To left.Rows - 1
            For J As Integer = 0 To right.Columns - 1
                For K As Integer = 0 To left.Columns - 1 ' or 0 to right.rows -1
                    B.Value(I, J) += left.Value(I, K) * right.Value(K, J)
                Next
            Next
        Next
        Return B
    End Operator

    Public Shared Operator =(left As Matrix, right As Matrix) As Boolean
        If left.Rows <> right.Rows Or left.Columns <> right.Columns Then Return False
        For I As Integer = 0 To left.Rows - 1
            For J As Integer = 0 To left.Columns - 1
                If left.Value(I, J) <> right.Value(I, J) Then Return False
            Next
        Next
        Return True
    End Operator

    Public Shared Operator <>(left As Matrix, right As Matrix) As Boolean
        If left.Rows <> right.Rows Or left.Columns <> right.Columns Then Return True
        For I As Integer = 0 To left.Rows - 1
            For J As Integer = 0 To left.Columns - 1
                If left.Value(I, J) <> right.Value(I, J) Then Return True
            Next
        Next
        Return False
    End Operator

    Public Shared Function Transpose(C As Matrix) As Matrix
        Dim result As Matrix = New Matrix(C.Columns, C.Rows)
        For I As Integer = 0 To C.Rows - 1
            For J As Integer = 0 To C.Columns - 1
                result.Value(J, I) = C.Value(I, J)
            Next
        Next
        Return result
    End Function

#End Region


    Public Function ToArray() As Double(,)
        Dim result(my_rows - 1, my_cols - 1) As Double
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                result(I, J) = my_values(I, J)
            Next
        Next
        Return result
    End Function

    Public Function ColToArray(column As Integer) As Double()
        If column >= my_cols Then
            Throw New Exception("ColToArray: The Matrix does not contain column number " & column.ToString())
        End If
        Dim result(my_rows - 1) As Double
        For I As Integer = 0 To my_rows - 1
            result(I) = my_values(I, column)
        Next
        Return result
    End Function

    Public Function RowToArray(row As Integer) As Double()
        If row >= my_rows Then
            Throw New Exception("RowToArray: The Matrix does not contain row number " & row.ToString())
        End If
        Dim result(my_cols - 1) As Double
        For I As Integer = 0 To my_cols - 1
            result(I) = my_values(row, I)
        Next
        Return result
    End Function

    Public Shared Function NormalDist(mu As Double, sig As Double) As Double
        Dim u As Double
        Dim v As Double
        Dim x As Double
        Dim y As Double
        Dim q As Double
        Do
            u = Rnd.NextDouble()
            v = 1.7156 * (Rnd.NextDouble() - 0.5)
            x = u - 0.449871
            y = Math.Abs(v) + 0.386595
            q = Math.Sqrt(x) + y * (0.196 * y - 0.25472 * x)
        Loop While (q > 0.27597 And (q > 0.27846 Or Math.Sqrt(v) > -4.0 * Math.Log(u) * Math.Sqrt(u)))
        Return mu + sig * v / u
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Dim result As Matrix = New Matrix(my_rows, my_cols)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                result.Value(I, J) = my_values(I, J)
            Next
        Next
        Return result
    End Function

    Public Overrides Function ToString() As String 'Format = [[x;x;x;..][x;x;x;...]....]
        Dim temp As String = "["
        For I As Integer = 0 To my_rows - 1
            temp &= "["
            For J As Integer = 0 To my_cols - 1
                temp &= my_values(I, J)
                If J < my_cols - 1 Then temp &= ";"
            Next
            temp &= "]"
        Next
        temp &= "]"
        Return temp
    End Function

    'EXTRA CODE TO BE CHECKED
#Region "EXTRA"

    Public Sub AddElement(ByVal element As Double)
        If currentX > (my_rows - 1) Then
            currentY += 1
            currentX = 0
        End If
        Try
            my_values(currentX, currentY) = element
        Catch e As Exception
            Throw New Exception("Matrix filled with values.")
        End Try
        currentX += 1
    End Sub

    Public Function SubMatrix(ByVal x As Integer, ByVal y As Integer) As Matrix
        Dim S As Matrix = New Matrix(my_rows - 1, my_cols - 1)
        For I As Integer = 0 To my_rows - 1
            For J As Integer = 0 To my_cols - 1
                If (I <> x And J <> y) Then
                    S.AddElement(my_values(I, J))
                End If
            Next
        Next
        Return S
    End Function

    Public Function Determinant(ByVal DoClone As Boolean) As Double
        Dim result As Double = 0.0
        'TODO implement Gaussian Elimination method for calculating the determinant
        If my_rows = 1 And my_cols = 1 Then
            Return my_values(0, 0)
        End If

        Dim Y As Integer = 0
        Dim k As Integer = 0
        For i As Integer = 0 To my_cols - 1
            If my_values(i, Y) <> 0 Then
                k = i
                Exit For
            End If
        Next

        Dim temp As Double
        Dim NewMatrix As Matrix
        If DoClone Then
            NewMatrix = CType(Clone(), Matrix)
        Else
            NewMatrix = Me
        End If
        Dim f As Double
        For i As Integer = k + 1 To my_cols - 1
            If my_values(i, Y) <> 0 Then
                f = my_values(i, Y) / my_values(k, Y)
                For j As Integer = 0 To my_rows - 1
                    NewMatrix.Value(i, j) = my_values(i, j) - my_values(k, j) * f
                Next
            End If
        Next
        NewMatrix = NewMatrix.SubMatrix(k, Y) 'Save space
        temp += ((-1) ^ (k + Y)) * my_values(k, Y) * NewMatrix.Determinant(False)
        Return temp
        Return result
    End Function

#End Region

End Class
