Imports System.Globalization
Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module IntegerExtensions
        <Extension>
        Public Function AddLeadingZeros(ByVal index As Integer) As String
            Return index.ToString(CultureInfo.InvariantCulture).PadLeft(5, "0"c)
        End Function
    End Module
End Namespace