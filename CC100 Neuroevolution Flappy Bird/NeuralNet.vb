Imports System.IO

Public Class NeuralNet

    Private my_InputNr As Integer
    Private my_HiddenNr As Integer
    Private my_OutputNr As Integer
    Private my_LearningRate As Double
    Private my_RandomizeNormal As Boolean
    Private WeightsIH As Matrix
    Private WeightsHO As Matrix
    Private BiasH As Matrix
    Private BiasO As Matrix
    Shared Rnd As Random = New Random

    Public Sub New(inputNodes As Integer, hiddenNodes As Integer, outputNodes As Integer, learningRate As Double, NormalWeights As Boolean)
        my_InputNr = inputNodes
        my_HiddenNr = hiddenNodes
        my_OutputNr = outputNodes
        my_LearningRate = learningRate
        my_RandomizeNormal = NormalWeights
        WeightsIH = New Matrix(my_HiddenNr, my_InputNr)
        WeightsHO = New Matrix(my_OutputNr, my_HiddenNr)
        If NormalWeights Then
            WeightsIH.RandomizeNormal(0.0, Math.Pow(my_InputNr, -0.5))
            WeightsHO.RandomizeNormal(0.0, Math.Pow(my_HiddenNr, -0.5))
        Else
            WeightsIH.Randomize()
            WeightsHO.Randomize()
        End If
        BiasH = New Matrix(my_HiddenNr, 1)
        BiasO = New Matrix(my_OutputNr, 1)
        BiasH.Randomize()
        BiasO.Randomize()
    End Sub

    Public Shared Function FromFile(filename As String) As NeuralNet
        Dim result As NeuralNet = Nothing
        Dim sr As StreamReader
        Dim InputNr As Integer = 0
        Dim HiddenNr As Integer = 0
        Dim OutputNr As Integer = 0
        Dim lr As Double = 0.0
        Dim normalRand As Boolean = False
        Try
            sr = New StreamReader(filename)
            InputNr = Integer.Parse(sr.ReadLine())
            HiddenNr = Integer.Parse(sr.ReadLine())
            OutputNr = Integer.Parse(sr.ReadLine())
            lr = Double.Parse(sr.ReadLine())
            normalRand = Boolean.Parse(sr.ReadLine())
            result = New NeuralNet(InputNr, HiddenNr, OutputNr, lr, normalRand)
            result.WeightsIH = Matrix.FromString(sr.ReadLine())
            result.WeightsHO = Matrix.FromString(sr.ReadLine())
            result.BiasH = Matrix.FromString(sr.ReadLine())
            result.BiasO = Matrix.FromString(sr.ReadLine())
        Catch ex As Exception
            MessageBox.Show("Cannot save the NeuralNet data. Original error: " & ex.Message, "NeuralNet error", MessageBoxButton.OK, MessageBoxImage.Error)
            Return Nothing
        End Try
        Return result
    End Function

    Public ReadOnly Property InputNodes As Integer
        Get
            Return my_InputNr
        End Get
    End Property

    Public ReadOnly Property HiddenNodes As Integer
        Get
            Return my_HiddenNr
        End Get
    End Property

    Public ReadOnly Property OutputNodes As Integer
        Get
            Return my_OutputNr
        End Get
    End Property

    Public ReadOnly Property LearningRate As Double
        Get
            Return my_LearningRate
        End Get
    End Property

    Public ReadOnly Property NormalizeWeights As Boolean
        Get
            Return my_RandomizeNormal
        End Get
    End Property

    Public Sub Train(inputs() As Double, targets As Double())
        If inputs.Length <> my_InputNr Then
            Throw New Exception("The number of inputs does not match this Neural Net configuration.")
        End If
        'Convert parameters to Matrices
        Dim my_Inputs As Matrix = Matrix.FromArray(inputs)
        Dim my_targets As Matrix = Matrix.FromArray(targets)
        'Create the signal matrices
        Dim Hidden_In As Matrix = New Matrix(my_HiddenNr, 1) 'Hidden node values without activation
        Dim Hidden_Out As Matrix = New Matrix(my_HiddenNr, 1) 'Hidden node values after activation
        Dim Output_In As Matrix = New Matrix(my_OutputNr, 1) 'Output node values without activation
        Dim Output_Out As Matrix = New Matrix(my_OutputNr, 1) 'Output node values after activation

        'STEP1: Calculate the Output with the FeedForward mechanism
        '  Generating the Hidden output
        Hidden_In = WeightsIH * my_Inputs
        Hidden_In.AddMatrix(BiasH)
        Hidden_Out = Hidden_In.MapTo(AddressOf Activation)
        '  Generating the final output
        Output_In = WeightsHO * Hidden_Out
        Output_In.AddMatrix(BiasO)
        Output_Out = Output_In.MapTo(AddressOf Activation)

        'STEP2: Calculate the Output and Hidden Errors 
        '  Calculate the output errors
        Dim Output_Errors As Matrix = my_targets - Output_Out
        '  Calculate the hidden errors using BackPropagation
        Dim Hidden_Errors As Matrix = Matrix.Transpose(WeightsHO) * Output_Errors

        'STEP3: Change the Weights using simplyfied Gradient Descent mechanism
        '  Change the Weights of Hidden to Output
        Dim Output_Gradients As Matrix = Output_In.MapTo(AddressOf Gradient)
        Output_Gradients.MultiplyHadamard(Output_Errors)
        Output_Gradients.MultiplyScalar(my_LearningRate)
        Dim WeightsHO_Delta As Matrix = Output_Gradients * Matrix.Transpose(Hidden_Out)
        WeightsHO.AddMatrix(WeightsHO_Delta)
        '  Change the Weights of Input to Hidden
        Dim Hidden_Gradients As Matrix = Hidden_In.MapTo(AddressOf Gradient)
        Hidden_Gradients.MultiplyHadamard(Hidden_Errors)
        Hidden_Gradients.MultiplyScalar(my_LearningRate)
        Dim WeightsIH_Delta As Matrix = Hidden_Gradients * Matrix.Transpose(my_Inputs)
        WeightsIH.AddMatrix(WeightsIH_Delta)

        'STEP4: Change the Biases
        BiasO.MatrixAdd(Output_Gradients)
        BiasH.MatrixAdd(Hidden_Gradients)
    End Sub

    Public Function Query(inputs() As Double) As Double()
        If inputs.Length <> my_InputNr Then
            Throw New Exception("The number of inputs does not match this Neural Net configuration.")
        End If
        Dim my_Inputs As Matrix = Matrix.FromArray(inputs)
        Dim Hidden As Matrix = New Matrix(my_HiddenNr, 1)
        Dim Outputs As Matrix = New Matrix(my_OutputNr, 1)
        'Generating the Hidden output
        Hidden = WeightsIH * my_Inputs
        Hidden.AddMatrix(BiasH)
        Hidden.Map(AddressOf Activation)
        'Generating the final output
        Outputs = WeightsHO * Hidden
        Outputs.AddMatrix(BiasO)
        Outputs.Map(AddressOf Activation)
        Return Outputs.ColToArray(0)
    End Function

    Public Function Copy() As NeuralNet
        Dim result As NeuralNet = New NeuralNet(my_InputNr, my_HiddenNr, my_OutputNr, my_LearningRate, my_RandomizeNormal)
        result.WeightsHO = CType(WeightsHO.Clone(), Matrix)
        result.WeightsIH = CType(WeightsIH.Clone(), Matrix)
        result.BiasH = CType(BiasH.Clone(), Matrix)
        result.BiasO = CType(BiasO.Clone(), Matrix)
        Return result
    End Function

    Public Sub Mutate(rate As Double, factor As Double)
        For I As Integer = 0 To WeightsHO.Rows - 1
            For J As Integer = 0 To WeightsHO.Columns - 1
                If 100 * Rnd.NextDouble < rate Then
                    If Rnd.NextDouble() < 0.5 Then
                        WeightsHO.Value(I, J) *= (100 - factor) / 100
                    Else
                        WeightsHO.Value(I, J) *= (100 + factor) / 100
                    End If
                End If
            Next
        Next
        For I As Integer = 0 To WeightsIH.Rows - 1
            For J As Integer = 0 To WeightsIH.Columns - 1
                If 100 * Rnd.NextDouble < rate Then
                    If Rnd.NextDouble() < 0.5 Then
                        WeightsIH.Value(I, J) *= (100 - factor) / 100
                    Else
                        WeightsIH.Value(I, J) *= (100 + factor) / 100
                    End If
                End If
            Next
        Next

        'Mutation of the Bias values??

        'For I As Integer = 0 To BiasH.Rows - 1
        '    For J As Integer = 0 To BiasH.Columns - 1
        '        If 100 * Rnd.NextDouble < rate Then
        '            If Rnd.NextDouble() < 0.5 Then
        '                BiasH.Value(I, J) *= (100 - factor) / 100
        '            Else
        '                BiasH.Value(I, J) *= (100 + factor) / 100
        '            End If
        '        End If
        '    Next
        'Next
        'For I As Integer = 0 To BiasO.Rows - 1
        '    For J As Integer = 0 To BiasO.Columns - 1
        '        If 100 * Rnd.NextDouble < rate Then
        '            If Rnd.NextDouble() < 0.5 Then
        '                BiasO.Value(I, J) *= (100 - factor) / 100
        '            Else
        '                BiasO.Value(I, J) *= (100 + factor) / 100
        '            End If
        '        End If
        '    Next
        'Next
    End Sub

    Private Function Activation(value As Double) As Double
        Return SigmoidActivation(value) 'Can be modified to use different functions
    End Function

    Private Function Gradient(value As Double) As Double
        'This is the derivitive of the Activation
        'Gradient(X) = d(Activation(X))/dX
        Return SigmoidGradient(value) 'Can be modified to use different functions
    End Function

    Public Sub Save(filename As String)
        Dim myStream As StreamWriter = Nothing
        Try
            myStream = New StreamWriter(filename)
            If (myStream IsNot Nothing) Then
                'Write the NeuralNet configuration to the file
                myStream.WriteLine(my_InputNr)
                myStream.WriteLine(my_HiddenNr)
                myStream.WriteLine(my_OutputNr)
                myStream.WriteLine(my_LearningRate.ToString())
                myStream.WriteLine(my_RandomizeNormal.ToString())
                myStream.WriteLine(WeightsIH.ToString())
                myStream.WriteLine(WeightsHO.ToString())
                myStream.WriteLine(BiasH.ToString())
                myStream.WriteLine(BiasO.ToString())
            End If
        Catch Ex As Exception
            MessageBox.Show("Cannot save the NeuralNet data. Original error: " & Ex.Message, "NeuralNet error", MessageBoxButton.OK, MessageBoxImage.Error)
        Finally
            If (myStream IsNot Nothing) Then
                myStream.Close()
            End If
        End Try
    End Sub

    Private Function SigmoidActivation(value As Double) As Double
        'Example of an Activation function
        'S(x) = 1/(1+e^(-x))
        Return 1 / (1 + Math.Exp(-1 * value))
    End Function

    Private Function SigmoidGradient(value As Double) As Double
        'Example of an Gradient function
        'dS(x)/dx = s(x)*(1-s(x))
        Return (1 / (1 + Math.Exp(-1 * value))) * (1 - 1 / (1 + Math.Exp(-1 * value)))
    End Function

End Class
