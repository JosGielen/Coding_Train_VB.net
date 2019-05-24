
Public Class PerlinNoise
    Private Shared Rnd As Random = New Random()
    Private Shared XOff As Double
    Private Shared YOff As Double
    Private Shared ZOff As Double
    Private Shared ReadOnly permutation() As Integer = {151, 160, 137, 91, 90, 15,                           'Hash lookup table As defined by Ken Perlin.  This Is a randomly
        131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,   'arranged array of all numbers from 0-255 inclusive.
        190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
        88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
        77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
        102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
        135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
        5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
        223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
        129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
        251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
        49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
        138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
    }
    Private Shared ReadOnly p(512) As Integer

    Shared Sub New()
        For I As Integer = 0 To 255
            p(I) = permutation(I)
            p(I + 256) = permutation(I)
        Next
        Randomize()
    End Sub

    ''' <summary>
    ''' Calculates a widened 1D Perlin Noise value between 0 and 1 for input value x using 4 octaves and 0.5 persistence.
    ''' </summary>
    ''' <param name="x">The input of the 1D Perlin Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideNoise(x As Double, factor As Integer) As Double
        Return SmoothStep(Noise(x, 4, 0.5), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a widened 1D Perlin Noise value between 0 and 1 for input value x with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The input of the 1D Perlin Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 1D Perlin Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 1D Perlin Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideNoise(x As Double, octaves As Integer, persistence As Double, factor As Integer) As Double
        Return SmoothStep(Noise(x, octaves, persistence), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a 1D Perlin Noise value between 0 and 1 for input value x using 4 octaves and 0.5 persistence.
    ''' </summary>
    ''' <param name="x">The input of the 1D Perlin Noise function.</param>
    Public Shared Function Noise(x As Double) As Double
        Return Noise(x, 4, 0.5)
    End Function

    ''' <summary>
    ''' Calculates a 1D Perlin Noise value between 0 and 1 for input value x with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The input of the 1D Perlin Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 1D Perlin Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 1D Perlin Noise function.</param>
    Public Shared Function Noise(x As Double, octaves As Integer, persistence As Double) As Double
        Dim total As Double = 0
        Dim frequency As Double = 1
        Dim amplitude As Double = 1
        Dim maxValue As Double = 0  'Used for normalizing result to 0.0 - 1.0
        For I As Integer = 0 To octaves - 1
            total += Perlin(x * frequency + XOff) * amplitude
            maxValue += amplitude
            amplitude *= persistence
            frequency *= 2
        Next
        Return total / maxValue
    End Function

    ''' <summary>
    ''' Calculates a widened 2D Perlin Noise value between 0 and 1 for input values x,y using 4 octaves and 0.5 persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 2D Perlin Noise function.</param>
    ''' <param name="y">The y input of the 2D Perlin Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideNoise2D(x As Double, y As Double, factor As Integer) As Double
        Return SmoothStep(Noise2D(x, y, 4, 0.5), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a widened 2D Perlin Noise value between 0 and 1 for input values x,y with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 2D Perlin Noise function.</param>
    ''' <param name="y">The y input of the 2D Perlin Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 2D Perlin Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 2D Perlin Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideNoise2D(x As Double, y As Double, octaves As Integer, persistence As Double, factor As Integer) As Double
        Return SmoothStep(Noise2D(x, y, octaves, persistence), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a 2D Perlin Noise value between 0 and 1 for input values x,y using 4 octaves and 0.5 persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 2D Perlin Noise function.</param>
    ''' <param name="y">The y input of the 2D Perlin Noise function.</param>
    Public Shared Function Noise2D(x As Double, y As Double) As Double
        Return Noise2D(x, y, 4, 0.5)
    End Function

    ''' <summary>
    ''' Calculates a 2D Perlin Noise value between 0 and 1 for input values x,y with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 2D Perlin Noise function.</param>
    ''' <param name="y">The y input of the 2D Perlin Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 2D Perlin Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 2D Perlin Noise function.</param>
    Public Shared Function Noise2D(x As Double, y As Double, octaves As Integer, persistence As Double) As Double
        Dim total As Double = 0
        Dim frequency As Double = 1
        Dim amplitude As Double = 1
        Dim maxValue As Double = 0  'Used for normalizing result to 0.0 - 1.0
        For I As Integer = 0 To octaves - 1
            total += Perlin2D(x * frequency + XOff, y * frequency + YOff) * amplitude
            maxValue += amplitude
            amplitude *= persistence
            frequency *= 2
        Next
        Return total / maxValue
    End Function

    ''' <summary>
    ''' Calculates a widened 3D Perlin Noise value between 0 and 1 for input values x,y,z using 4 octaves and 0.5 persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 3D Perlin Noise function.</param>
    ''' <param name="y">The y input of the 3D Perlin Noise function.</param>
    ''' <param name="z">The z input of the 3D Perlin Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideNoise3D(x As Double, y As Double, z As Double, factor As Integer) As Double
        Return SmoothStep(Noise3D(x, y, z, 4, 0.5), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a widened 3D Perlin Noise value between 0 and 1 for input values x,y,z with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 3D Perlin Noise function.</param>
    ''' <param name="y">The y input of the 3D Perlin Noise function.</param>
    ''' <param name="z">The z input of the 3D Perlin Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 3D Perlin Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 3D Perlin Noise function.</param>
    ''' <param name="factor">The number of times the widening function is applied</param>
    Public Shared Function WideNoise3D(x As Double, y As Double, z As Double, octaves As Integer, persistence As Double, factor As Integer) As Double
        Return SmoothStep(Noise3D(x, y, z, octaves, persistence), 0, 1, factor)
    End Function

    ''' <summary>
    ''' Calculates a 3D Perlin Noise value between 0 and 1 for input values x,y,z using 4 octaves and 0.5 persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 3D Perlin Noise function.</param>
    ''' <param name="y">The y input of the 3D Perlin Noise function.</param>
    ''' <param name="z">The z input of the 3D Perlin Noise function.</param>
    Public Shared Function Noise3D(x As Double, y As Double, z As Double) As Double
        Return Noise3D(x, y, z, 4, 0.5)
    End Function

    ''' <summary>
    ''' Calculates a 3D Perlin Noise value between 0 and 1 for input values x,y,z with given octaves and persistence.
    ''' </summary>
    ''' <param name="x">The x input of the 3D Perlin Noise function.</param>
    ''' <param name="y">The y input of the 3D Perlin Noise function.</param>
    ''' <param name="z">The z input of the 3D Perlin Noise function.</param>
    ''' <param name="octaves">Number of octaves used to calculate the 3D Perlin Noise.</param>
    ''' <param name="persistence">Relative strength of higher octaves in the 3D Perlin Noise function.</param>
    Public Shared Function Noise3D(x As Double, y As Double, z As Double, octaves As Integer, persistence As Double) As Double
        Dim total As Double = 0
        Dim frequency As Double = 1
        Dim amplitude As Double = 1
        Dim maxValue As Double = 0  'Used for normalizing result to 0.0 - 1.0
        For I As Integer = 0 To octaves - 1
            total += Perlin3D(x * frequency + XOff, y * frequency + YOff, z * frequency + ZOff) * amplitude
            maxValue += amplitude
            amplitude *= persistence
            frequency *= 2
        Next
        Return total / maxValue
    End Function

    ''' <summary>
    ''' Calculates the standard 1D Perlin Noise value between 0 and 1 for input value x.
    ''' </summary>
    ''' <param name="x">The x input of the 1D Perlin Noise function.</param>
    Public Shared Function Perlin(x As Double) As Double
        Dim xi As Integer = CInt(Math.Floor(x)) And 255
        Dim xf As Double = x - Math.Floor(x)
        Dim u As Double = Fade(xf)
        Dim g1 As Integer = p(xi)
        Dim g2 As Integer = p(xi + 1)
        Dim d1 As Double = Grad(g1, xf)
        Dim d2 As Double = Grad(g2, xf - 1)
        Dim x1 As Double = Lerp(d1, d2, u)   'Interpolate X values
        Return (x1 + 1) / 2  'Return value between 0 and 1
    End Function

    ''' <summary>
    ''' Calculates the standard 2D Perlin Noise value between 0 and 1 for input values x,y.
    ''' </summary>
    ''' <param name="x">The x input of the 2D Perlin Noise function.</param>
    ''' <param name="y">The y input of the 2D Perlin Noise function.</param>
    Public Shared Function Perlin2D(x As Double, y As Double) As Double
        Dim xi As Integer = CInt(Math.Floor(x)) And 255
        Dim yi As Integer = CInt(Math.Floor(y)) And 255
        Dim g1 As Integer = p(p(xi) + yi)
        Dim g2 As Integer = p(p(xi + 1) + yi)
        Dim g3 As Integer = p(p(xi) + yi + 1)
        Dim g4 As Integer = p(p(xi + 1) + yi + 1)
        Dim xf As Double = x - Math.Floor(x)
        Dim yf As Double = y - Math.Floor(y)
        Dim d1 As Double = Grad2D(g1, xf, yf)
        Dim d2 As Double = Grad2D(g2, xf - 1, yf)
        Dim d3 As Double = Grad2D(g3, xf, yf - 1)
        Dim d4 As Double = Grad2D(g4, xf - 1, yf - 1)
        Dim u As Double = Fade(xf)
        Dim v As Double = Fade(yf)
        Dim x1 As Double = Lerp(d1, d2, u)   'Interpolate X at Y
        Dim x2 As Double = Lerp(d3, d4, u)   'Interpolate X at Y-1
        Dim y1 As Double = Lerp(x1, x2, v)   'Interpolate Y between both X interpolated values
        Return (y1 + 1) / 2  'Return value between 0 and 1
    End Function

    ''' <summary>
    ''' Calculates the standard 3D Perlin Noise value between 0 and 1 for input values x,y,z.
    ''' </summary>
    ''' <param name="x">The x input of the 3D Perlin Noise function.</param>
    ''' <param name="y">The y input of the 3D Perlin Noise function.</param>
    ''' <param name="z">The z input of the 3D Perlin Noise function.</param>
    Public Shared Function Perlin3D(x As Double, y As Double, z As Double) As Double
        Dim xi As Integer = CInt(Math.Floor(x)) And 255
        Dim yi As Integer = CInt(Math.Floor(y)) And 255
        Dim zi As Integer = CInt(Math.Floor(z)) And 255
        Dim g1 As Integer = p(p(p(xi) + yi) + zi)
        Dim g2 As Integer = p(p(p(xi) + yi + 1) + zi)
        Dim g3 As Integer = p(p(p(xi) + yi) + zi + 1)
        Dim g4 As Integer = p(p(p(xi) + yi + 1) + zi + 1)
        Dim g5 As Integer = p(p(p(xi + 1) + yi) + zi)
        Dim g6 As Integer = p(p(p(xi + 1) + yi + 1) + zi)
        Dim g7 As Integer = p(p(p(xi + 1) + yi) + zi + 1)
        Dim g8 As Integer = p(p(p(xi + 1) + yi + 1) + zi + 1)
        Dim xf As Double = x - Math.Floor(x)
        Dim yf As Double = y - Math.Floor(y)
        Dim zf As Double = z - Math.Floor(z)
        Dim d1 As Double = Grad3D(g1, xf, yf, zf)
        Dim d2 As Double = Grad3D(g2, xf, yf - 1, zf)
        Dim d3 As Double = Grad3D(g3, xf, yf, zf - 1)
        Dim d4 As Double = Grad3D(g4, xf, yf - 1, zf - 1)
        Dim d5 As Double = Grad3D(g5, xf - 1, yf, zf)
        Dim d6 As Double = Grad3D(g6, xf - 1, yf - 1, zf)
        Dim d7 As Double = Grad3D(g7, xf - 1, yf, zf - 1)
        Dim d8 As Double = Grad3D(g8, xf - 1, yf - 1, zf - 1)
        Dim u As Double = Fade(xf)
        Dim v As Double = Fade(yf)
        Dim w As Double = Fade(zf)
        Dim X1 As Double = Lerp(d1, d5, u)  'Interpolate X at (Y,Z)
        Dim X2 As Double = Lerp(d2, d6, u)  'Interpolate X at (Y-1,Z)
        Dim X3 As Double = Lerp(d3, d7, u)  'Interpolate X at (Y,Z-1)
        Dim X4 As Double = Lerp(d4, d8, u)  'Interpolate X (Y-1,Z-1)
        Dim y1 As Double = Lerp(X1, X2, v)  'Interpolate Y between both X interpolated values at Z
        Dim y2 As Double = Lerp(X3, X4, v)  'Interpolate Y between both X interpolated values at Z-1
        Dim z1 As Double = Lerp(y1, y2, w)  'Interpolate Z between both Y interpolated values.
        Return (z1 + 1) / 2  'Return value between 0 and 1
    End Function

#Region "Private"

    Private Shared Function Lerp(a As Double, b As Double, x As Double) As Double
        Return a + x * (b - a)
    End Function

    'Fade function as defined by Ken Perlin.  This eases coordinate values
    'so that they will "ease" towards integral values.  
    'This ends up smoothing the final output.
    Private Shared Function Fade(t As Double) As Double
        Return t * t * t * (t * (t * 6 - 15) + 10)   '6t^5 - 15t^4 + 10t^3
    End Function

    Private Shared Function Grad(hash As Integer, x As Double) As Double
        Select Case hash And 1 'Take the first bit Of the hash value
            Case 0
                Return x
            Case 1
                Return -x
            Case Else
                Return 0  'never happens
        End Select
    End Function

    Private Shared Function Grad2D(hash As Integer, x As Double, y As Double) As Double
        Select Case hash And 3 'Take the first 2 bits Of the hash value
            Case 0
                Return x + y
            Case 1
                Return -x + y
            Case 2
                Return x - y
            Case 3
                Return -x - y
            Case Else
                Return 0  'never happens
        End Select
    End Function

    Private Shared Function Grad3D(hash As Integer, x As Double, y As Double, z As Double) As Double
        Dim h As Integer = hash And &HF 'Take the first 4 bits Of the hash value
        Select Case h
            Case 0
                Return x + y
            Case 1
                Return -x + y
            Case 2
                Return x - y
            Case 3
                Return -x - y
            Case 4
                Return x + z
            Case 5
                Return -x + z
            Case 6
                Return x - z
            Case 7
                Return -x - z
            Case 8
                Return y + z
            Case 9
                Return -y + z
            Case 10
                Return y - z
            Case 11
                Return -y - z
            Case 12
                Return y + x
            Case 13
                Return -y + z
            Case 14
                Return y - x
            Case 15
                Return -y - z
            Case Else
                Return 0  'never happens
        End Select
    End Function

    Private Shared Sub Randomize()
        'Make random seed values for the Perlin Noise
        Dim N As Integer = Now.Millisecond
        If N < 10 Then N = N + 100
        For I As Integer = 0 To N
            XOff = 10 * Rnd.NextDouble()
            YOff = 10 * Rnd.NextDouble()
            ZOff = 10 * Rnd.NextDouble()
        Next
    End Sub

    Private Shared Function SmoothStep(x As Double, lower As Double, upper As Double, Strength As Integer) As Double
        'Make sure lower <= upper
        If lower > upper Then
            Dim dummy As Double
            dummy = lower
            lower = upper
            upper = dummy
        End If
        'Lock x between lower and upper
        If x < lower Then x = lower
        If x > upper Then x = upper
        'Scale x between 0 and 1
        x = (x - lower) / (upper - lower)
        'Widen x
        For I As Integer = 0 To Strength - 1
            x = x * x * x * (x * (x * 6 - 15) + 10)
        Next
        'Scale x back between lower and upper
        Return (upper - lower) * x + lower
    End Function
#End Region

End Class
