Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module OptionMappingExtensions
        
        <Extension()> Public Function HasExteriorColour(ByVal mappingLines As IEnumerable(Of OptionMappingLine), ByVal exteriorColour As String) As Boolean
            Return mappingLines.Any(Function(line) line.ExteriorColourCode.Equals(exteriorColour, StringComparison.InvariantCultureIgnoreCase))
        End Function

    End Module
End Namespace