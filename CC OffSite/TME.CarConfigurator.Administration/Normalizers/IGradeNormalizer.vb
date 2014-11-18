Imports System.Collections.Generic

Namespace Normalizers
    Public Interface IGradeNormalizer
        Sub Normalize(ByVal grade As ModelGenerationGrade, ByVal cars As IEnumerable(Of Car))
    End Interface
End Namespace
