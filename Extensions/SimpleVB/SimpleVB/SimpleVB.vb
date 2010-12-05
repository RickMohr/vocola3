Imports Vocola


Namespace Library

    Public Class SimpleVB
        Inherits VocolaExtension

        <VocolaFunction()> _
        Public Shared Sub LogHelloWorld()
            VocolaApi.LogMessage(LogLevel.Medium, "Hello, world!")
        End Sub

        <VocolaFunction()> _
        Public Shared Function Subtract(ByVal a As Integer, ByVal b As Integer) As Integer
            Return a - b
        End Function

    End Class

End Namespace