
Public Class PartNumbers

    Public Shared Function AreIdentical(ByVal partNumber1 As String, ByVal partNumber2 As String) As Boolean
        Return String.Equals(partNumber1.Replace("-", "").Trim(), partNumber2.Replace("-", "").Trim(), StringComparison.InvariantCultureIgnoreCase)
    End Function


    Private Sub New()

    End Sub

End Class
