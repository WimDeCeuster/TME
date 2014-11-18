Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module StringExtensions
        ''' <summary>
        ''' Compares 2 strings. If both are Nothing, this method reports them as being the same.
        ''' </summary>
        ''' <param name="first">String that gets compared to the seconds string.</param>
        ''' <param name="second">String that is used to compare the first string against.</param>
        ''' <param name="whiteSpaceEqualsNothing">Flags whether null equals whitespace/empty or not.</param>
        ''' <param name="comparisonType">Comparison type to use when strings aren't nothing.</param>
        ''' <returns>True if strings are same or both are nothing, False otherwise</returns>
        <Extension>
        Public Function IsSameAs(ByVal first As String, ByVal second As String, Optional ByVal whiteSpaceEqualsNothing As Boolean = False, Optional ByVal comparisonType As StringComparison = StringComparison.InvariantCultureIgnoreCase) As Boolean
            if Not whiteSpaceEqualsNothing Then Return If(first Is Nothing, second Is Nothing, first.Equals(second, comparisonType))

            Return if (string.IsNullOrWhiteSpace(first), string.IsNullOrWhiteSpace(second), first.Equals(second, comparisonType))
        End Function
    End Module
End Namespace