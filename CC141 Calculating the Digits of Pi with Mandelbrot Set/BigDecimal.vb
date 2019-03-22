
Imports System.Numerics

Public Structure BigDecimal
    Implements IComparable(Of BigDecimal)

    'Specifies whether the significant digits should be truncated to the given precision after each operation.
    Public Shared AlwaysTruncate As Boolean = True    'If AlwaysTruncate Is set to true all operations are affected.
    Private Shared MyPrecision As Integer = 50         'Sets the maximum precision of division operations.
    Public Shared Notation As NotationType = NotationType.Normal

    Public Property Mantissa As BigInteger
    Public Property Exponent As Integer


    Public Sub New(mantissa As Integer, exponent As Integer)
        Me.Mantissa = mantissa
        Me.Exponent = exponent
        Normalize()
        If AlwaysTruncate Then Truncate()
    End Sub

    Public Sub New(mantissa As Long, exponent As Integer)
        Me.Mantissa = mantissa
        Me.Exponent = exponent
        Normalize()
        If AlwaysTruncate Then Truncate()
    End Sub

    Public Sub New(mantissa As BigInteger, exponent As Integer)
        Me.Mantissa = mantissa
        Me.Exponent = exponent
        Normalize()
        If AlwaysTruncate Then Truncate()
    End Sub

    Public Shared Property Precision As Integer
        Get
            Return MyPrecision
        End Get
        Set(value As Integer)
            MyPrecision = value
            If MyPrecision < 1 Then MyPrecision = 1
        End Set
    End Property

    Public Function CompareTo(other As BigDecimal) As Integer Implements IComparable(Of BigDecimal).CompareTo
        If Me < other Then
            Return -1
        ElseIf Me > other Then
            Return 1
        Else
            Return 0
        End If
    End Function

    'Removes trailing zeros on the mantissa
    Public Sub Normalize()
        Dim remainder As BigInteger = 0
        Dim shortened As BigInteger = 0
        If (Mantissa.IsZero) Then
            Exponent = 0
        Else
            While (remainder = 0)
                shortened = BigInteger.DivRem(Mantissa, 10, remainder)
                If (remainder = 0) Then
                    Mantissa = shortened
                    Exponent += 1
                End If
            End While
        End If
    End Sub

    'Truncate the number to the given precision by removing the least significant digits.
    Public Sub Truncate(digits As Integer)
        If digits < 1 Then digits = 1
        'remove the least significant digits, as long as the number of digits Is higher than the given Precision
        While (NumberOfDigits(Mantissa) > digits)
            Mantissa /= 10
            Exponent += 1
        End While
    End Sub

    Public Sub Truncate()
        Truncate(Precision)
    End Sub

    Public Shared Function NumberOfDigits(value As BigInteger) As Integer
        'do Not count the sign
        Return (value * value.Sign).ToString().Length
        'faster version
        'Return CInt(Math.Ceiling(BigInteger.Log10(value * value.Sign)))
    End Function


#Region "Conversions"

    Public Shared Widening Operator CType(ByVal value As Integer) As BigDecimal
        Return New BigDecimal(value, 0)
    End Operator

    Public Shared Widening Operator CType(ByVal value As Long) As BigDecimal
        Return New BigDecimal(value, 0)
    End Operator

    Public Shared Widening Operator CType(ByVal value As Decimal) As BigDecimal
        Dim mantissa As BigInteger = New BigInteger(value)
        Dim exponent As Integer = 0
        Dim scaleFactor As Double = 1
        While (CDec(mantissa) <> value * scaleFactor)
            exponent -= 1
            scaleFactor *= 10
            mantissa = New BigInteger(value * scaleFactor)
        End While
        Return New BigDecimal(mantissa, exponent)
    End Operator

    Public Shared Widening Operator CType(ByVal value As Single) As BigDecimal
        Dim mantstring As String = ""
        Dim expString As String = ""
        Dim mant As BigInteger = 0
        Dim exp As Integer = 0
        Dim valueString As String = value.ToString()
        If valueString.Contains("E"c) Then
            Dim Epos As Integer = valueString.IndexOf("E"c)
            mantstring = valueString.Substring(0, Epos)
            expString = valueString.Substring(Epos + 1, valueString.Length - Epos - 1)
            exp = Integer.Parse(expString)
        Else
            mantstring = valueString
        End If
        If mantstring.Contains("."c) Then
            Dim DotPos As Integer = mantstring.IndexOf("."c)
            exp -= mantstring.Length - DotPos - 1
            mantstring = mantstring.Substring(0, DotPos) & mantstring.Substring(DotPos + 1, mantstring.Length - DotPos - 1)
        End If
        mant = BigInteger.Parse(mantstring)
        Return New BigDecimal(mant, exp)
    End Operator

    Public Shared Widening Operator CType(ByVal value As Double) As BigDecimal
        Dim mantstring As String = ""
        Dim expString As String = ""
        Dim mant As BigInteger = 0
        Dim exp As Integer = 0
        Dim valueString As String = value.ToString()
        If valueString.Contains("E"c) Then
            Dim Epos As Integer = valueString.IndexOf("E"c)
            mantstring = valueString.Substring(0, Epos)
            expString = valueString.Substring(Epos + 1, valueString.Length - Epos - 1)
            exp = Integer.Parse(expString)
        Else
            mantstring = valueString
        End If
        If mantstring.Contains("."c) Then
            Dim DotPos As Integer = mantstring.IndexOf("."c)
            exp -= mantstring.Length - DotPos - 1
            mantstring = mantstring.Substring(0, DotPos) & mantstring.Substring(DotPos + 1, mantstring.Length - DotPos - 1)
        End If
        mant = BigInteger.Parse(mantstring)
        Return New BigDecimal(mant, exp)
    End Operator

    Public Shared Narrowing Operator CType(ByVal value As BigDecimal) As Double
        Dim Max As BigDecimal = Double.MaxValue
        Dim Min As BigDecimal = Double.MinValue
        If value > Max Or value < Min Then Throw New OverflowException()
        Return CDbl(value.Mantissa) * Math.Pow(10, value.Exponent)
    End Operator

    Public Shared Narrowing Operator CType(ByVal value As BigDecimal) As Single
        Dim Max As BigDecimal = Single.MaxValue
        Dim Min As BigDecimal = Single.MinValue
        If value > Max Or value < Min Then Throw New OverflowException()
        Return CSng(CDbl(value.Mantissa) * Math.Pow(10, value.Exponent))
    End Operator

    Public Shared Narrowing Operator CType(ByVal value As BigDecimal) As Decimal
        Return CDec(value.Mantissa) * CDec(Math.Pow(10, value.Exponent))
    End Operator

    Public Shared Narrowing Operator CType(ByVal value As BigDecimal) As Long
        Return CLng(CDbl(value.Mantissa) * 10 ^ value.Exponent)
    End Operator

    Public Shared Narrowing Operator CType(ByVal value As BigDecimal) As Integer
        Return CInt(CDbl(value.Mantissa) * 10 ^ value.Exponent)
    End Operator

#End Region

#Region "Operators"

    Public Shared Operator +(value As BigDecimal) As BigDecimal
        Return value
    End Operator

    Public Shared Operator -(value As BigDecimal) As BigDecimal
        value.Mantissa *= -1
        Return value
    End Operator

    Public Shared Operator +(left As BigDecimal, right As BigDecimal) As BigDecimal
        If left.Exponent > right.Exponent Then
            Return New BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent)
        Else
            Return New BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent)
        End If
    End Operator

    Public Shared Operator -(left As BigDecimal, right As BigDecimal) As BigDecimal
        If left.Exponent > right.Exponent Then
            Return New BigDecimal(AlignExponent(left, right) - right.Mantissa, right.Exponent)
        Else
            Return New BigDecimal(left.Mantissa - AlignExponent(right, left), left.Exponent)
        End If
    End Operator

    Public Shared Operator *(left As BigDecimal, right As BigDecimal) As BigDecimal
        Return New BigDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent)
    End Operator

    'dividend / divisor
    Public Shared Operator /(dividend As BigDecimal, divisor As BigDecimal) As BigDecimal
        Dim exponentChange As Integer = Precision - (NumberOfDigits(dividend.Mantissa) - NumberOfDigits(divisor.Mantissa))
        If exponentChange < 0 Then exponentChange = 0
        Dim result As New BigDecimal(dividend.Mantissa * BigInteger.Pow(10, exponentChange) / divisor.Mantissa, dividend.Exponent - divisor.Exponent - exponentChange)
        result.Truncate()
        Return result
    End Operator

    Public Shared Operator Mod(left As BigDecimal, right As BigDecimal) As BigDecimal
        Return left - right * ((left / right).Floor())
    End Operator

    Public Shared Operator =(left As BigDecimal, right As BigDecimal) As Boolean
        Return left.Mantissa = right.Mantissa And left.Exponent = right.Exponent
    End Operator

    Public Shared Operator <>(left As BigDecimal, right As BigDecimal) As Boolean
        Return left.Mantissa <> right.Mantissa Or left.Exponent <> right.Exponent
    End Operator

    Public Shared Operator <(left As BigDecimal, right As BigDecimal) As Boolean
        If left.Exponent > right.Exponent Then
            Return AlignExponent(left, right) < right.Mantissa
        Else
            Return left.Mantissa < AlignExponent(right, left)
        End If
    End Operator

    Public Shared Operator >(left As BigDecimal, right As BigDecimal) As Boolean

        If left.Exponent > right.Exponent Then
            Return AlignExponent(left, right) > right.Mantissa
        Else
            Return left.Mantissa > AlignExponent(right, left)
        End If
    End Operator

    Public Shared Operator <=(left As BigDecimal, right As BigDecimal) As Boolean
        If left.Exponent > right.Exponent Then
            Return AlignExponent(left, right) <= right.Mantissa
        Else
            Return left.Mantissa <= AlignExponent(right, left)
        End If
    End Operator

    Public Shared Operator >=(left As BigDecimal, right As BigDecimal) As Boolean

        If left.Exponent > right.Exponent Then
            Return AlignExponent(left, right) >= right.Mantissa
        Else
            Return left.Mantissa >= AlignExponent(right, left)
        End If
    End Operator

    'Returns the mantissa of value, aligned to the exponent of reference.
    'Assumes the exponent of value Is larger than of reference.
    Private Shared Function AlignExponent(value As BigDecimal, reference As BigDecimal) As BigInteger
        Return value.Mantissa * BigInteger.Pow(10, value.Exponent - reference.Exponent)
    End Function

#End Region

#Region "Additional mathematical functions"

    Public Shared Function Exp(exponent As BigDecimal) As BigDecimal
        'Newton's Series Expansion for e^(x) converges fast for small x
        Dim Mant As BigInteger
        Dim Expo As Integer
        Dim A As BigDecimal
        Dim APow As BigDecimal
        Dim eA As BigDecimal
        Dim Iter As BigDecimal
        Dim result As BigDecimal
        Dim Accur As BigDecimal = New BigDecimal(1, -MyPrecision)
        Dim I As Integer = 0
        If exponent > 10 Or exponent < -10 Then
            'Convert exponent to scientific notation A x 10^(B) where A < 10.
            Mant = exponent.Mantissa * exponent.Mantissa.Sign
            Expo = exponent.Exponent
            Do While Mant >= 10
                Mant = Mant / 10
                Expo = Expo + 1
            Loop
            'Step 1: calculate e^(A)
            A = exponent / Math.Pow(10, Expo)
            APow = New BigDecimal(1, 0)
            Iter = New BigDecimal(1, 0)
            eA = New BigDecimal(0, 0)
            Do While BigDecimal.Abs(Iter) > Accur
                Iter = APow / BigDecimal.Fact(I)
                eA = eA + Iter
                APow = APow * A
                I += 1
            Loop
            eA = eA.Round(MyPrecision)
            'Step 2: Multiply e^(A) * e^(A) 10^(B) times
            result = New BigDecimal(1, 0)
            For J As Double = 1 To Math.Pow(10, Expo) Step 1.0
                result = result * eA
            Next
        Else
            'Calculate e^(exponent) directly
            APow = New BigDecimal(1, 0)
            Iter = New BigDecimal(1, 0)
            result = New BigDecimal(0, 0)
            Do While BigDecimal.Abs(Iter) > Accur
                Iter = APow / BigDecimal.Fact(I)
                result = result + Iter
                APow = APow * exponent
                I += 1
            Loop
        End If
        Return result.Round(MyPrecision)
    End Function

    Public Shared Function Abs(number As BigDecimal) As BigDecimal
        Return New BigDecimal(number.Mantissa * number.Mantissa.Sign, number.Exponent)
    End Function

    Public Shared Function Logn(number As BigDecimal) As BigDecimal
        'Use a Taylor series to calculate logn(Z) where Z < 2 
        'Convert number to scientific notation A x 10^(B) where 1 <= A < 10.
        'logn(A x 10^(B)) = logn(A) + B*logn(10)
        'Sqrt(Sqrt(A)) <= 2 -> logn(A) = 4*logn(Sqrt(Sqrt(A)))
        'Sqrt(Sqrt(10)) < 2 -> logn(10) = 4*logn(Sqrt(Sqrt(10)))
        '--------------------
        'Logn(number) = 4*logn(Sqrt(Sqrt(A))) + 4*B*logn(Sqrt(Sqrt(10))) 
        Dim Mant As BigInteger
        Dim Expo As Integer
        Dim A As BigDecimal
        Dim B As BigDecimal
        Dim Z As BigDecimal
        Dim ZFactor As BigDecimal
        Dim ZPow As BigDecimal
        Dim LnA As BigDecimal
        Dim Ln10 As BigDecimal
        Dim Iter As BigDecimal
        Dim result As BigDecimal
        Dim Accur As BigDecimal = New BigDecimal(1, -MyPrecision)
        Dim I As BigDecimal
        If number.Mantissa < 0 Then
            Throw New ArithmeticException("Logarithm of negative numbers do not exist.")
            Exit Function
        ElseIf number.Mantissa = 0 Then
            Throw New NotFiniteNumberException("Logarithm of 0 is infinite.")
            Exit Function
        Else
            'Convert number to scientific notation A x 10^(B) where A < 10.
            Mant = number.Mantissa * number.Mantissa.Sign
            Expo = number.Exponent
            Do While Mant >= 10
                Mant = Mant / 10
                Expo = Expo + 1
            Loop
            A = number / Math.Pow(10, Expo)
            B = Expo
            'Calculate logn(A)
            Z = BigDecimal.SQRT(BigDecimal.SQRT(A))
            ZFactor = (Z - 1) / (Z + 1)
            I = New BigDecimal(0, 0)
            ZPow = ZFactor
            Iter = New BigDecimal(1, 0)
            LnA = New BigDecimal(0, 0)
            Do While BigDecimal.Abs(Iter) > Accur
                Iter = ZPow / (2 * I + 1)
                LnA = LnA + Iter
                ZPow = ZPow * ZFactor * ZFactor
                I = I + 1
            Loop
            If B <> 0 Then
                'Calculate logn(10)
                Z = BigDecimal.SQRT(BigDecimal.SQRT(10))
                ZFactor = (Z - 1) / (Z + 1)
                I = New BigDecimal(0, 0)
                ZPow = ZFactor
                Iter = New BigDecimal(1, 0)
                Ln10 = New BigDecimal(0, 0)
                Do While BigDecimal.Abs(Iter) > Accur
                    Iter = ZPow / (2 * I + 1)
                    Ln10 = Ln10 + Iter
                    ZPow = ZPow * ZFactor * ZFactor
                    I = I + 1
                Loop
            Else
                Ln10 = New BigDecimal(0, 0)
            End If

            'Calculate logn(number)
            result = 8 * LnA + 8 * B * Ln10
        End If
        Return result.Round(MyPrecision)
    End Function

    Public Shared Function Pow(basis As BigDecimal, exponent As BigDecimal) As BigDecimal
        'A^(B) = Exp(logn(A^(B))) = Exp(B*logn(A))
        Try
            Return Exp(exponent * Logn(basis))
        Catch ex As Exception
            Throw New ArithmeticException(basis.ToString() & " to the power of " & exponent.ToString() & " can not be calculated.")
        End Try
        Return 0
    End Function

    Public Shared Function Fact(number As Integer) As BigDecimal
        Dim temp As BigDecimal = New BigDecimal(1, 0)
        For I As Integer = 1 To number
            temp = temp * CType(I, BigDecimal)
        Next
        Return temp
    End Function

    Public Shared Function SQRT(number As BigDecimal) As BigDecimal
        Dim Mant As BigInteger
        Dim Expo As Integer
        Dim result As BigDecimal
        If number < 0 Then
            Throw New ArithmeticException("Square Root of negative numbers can not be calculated.")
            Exit Function
        ElseIf number.Mantissa = 0 Then
            Return New BigDecimal(0, 0)
        End If
        'Get a start value
        Mant = number.Mantissa
        Expo = number.Exponent
        Do While Mant >= 10
            Mant = Mant / 10
            Expo = Expo + 1
        Loop
        result = Math.Sqrt(CDbl(number / Math.Pow(10, Expo))) * Math.Pow(10, Expo / 2)
        'Use Newton method for iterative calculation of square root
        For I As Integer = 0 To Precision
            result = 0.5 * (result + number / result)
        Next
        result.Truncate(Precision)
        Return result
    End Function

    'Rounds the number to the given precision.
    Public Function Round(precision As Integer) As BigDecimal
        Dim result As BigDecimal = New BigDecimal(Mantissa, Exponent)
        Dim remainder As BigInteger = 0
        If precision < 1 Then precision = 1
        'remove the least significant digits, as long as the number of digits Is higher than the given Precision
        While (NumberOfDigits(result.Mantissa) > precision)
            result.Mantissa = BigInteger.DivRem(result.Mantissa, 10, remainder)
            result.Exponent += 1
        End While
        If remainder >= 5 Then result.Mantissa += 1
        Return result
    End Function

    Public Function Floor() As BigDecimal
        Dim result As BigDecimal = New BigDecimal(Mantissa, Exponent)
        result.Truncate(BigDecimal.NumberOfDigits(Mantissa) + Exponent)
        Return result
    End Function

#End Region

    'Displays the BigDecimal in the selected notation
    Public Overrides Function ToString() As String
        Dim Mant As BigInteger = Mantissa * Mantissa.Sign
        Dim MantString As String = Mantissa.ToString()
        Dim Exp As Integer = Exponent
        Dim ExpString As String = ""
        Dim DecimalPosition As Integer = 0
        Dim returnString As String = ""
        If Notation = NotationType.Engineering Then
            Do While Mant >= 10
                Mant = Mant / 10
                Exp = Exp + 1
            Loop
            DecimalPosition = 1
            Do While Exp Mod 3 > 0
                Mant = Mant * 10
                Exp = Exp - 1
                DecimalPosition += 1
            Loop
            If Mantissa.Sign = -1 Then DecimalPosition += 1
        ElseIf Notation = NotationType.Scientific Then
            Do While Mant >= 10
                Mant = Mant / 10
                Exp = Exp + 1
            Loop
            DecimalPosition = 1
            If Mantissa.Sign = -1 Then DecimalPosition += 1
        Else
            DecimalPosition = 0
        End If
        If Exp > 0 Then
            ExpString = "E+" & Exp.ToString()
        ElseIf Exp < 0 Then
            ExpString = "E" & Exp.ToString()
        Else
            ExpString = ""
        End If
        Do While DecimalPosition + 1 > MantString.Length
            MantString &= "0"
        Loop
        If DecimalPosition > 0 Then
            returnString = String.Concat(MantString.Insert(DecimalPosition, "."), ExpString)
        Else
            returnString = String.Concat(MantString, ExpString)
        End If
        Return returnString
    End Function

    Public Overloads Function Equals(other As BigDecimal) As Boolean
        Return other.Mantissa.Equals(Mantissa) And other.Exponent = Exponent
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        If ReferenceEquals(vbNull, obj) Then Return False
        If TypeOf obj Is BigDecimal Then
            Return CType(obj, BigDecimal).Equals(Me)
        Else
            Return False
        End If
    End Function

    Public Overrides Function GetHashCode() As Integer
        'Unchecked
        Return CInt((Mantissa.GetHashCode() * 397) ^ Exponent)
    End Function

End Structure


Public Enum NotationType
        Normal = 1
        Engineering = 2
        Scientific = 3
    End Enum






