Namespace Exceptions
    Public Class NonSuffixOptionOfferedThroughPackException
        Inherits Exception

        Public Sub New(ByVal modelGenerationGradeOption As ModelGenerationGradeOption, ByVal modelGenerationPack As ModelGenerationPack)
            MyBase.New(String.Format("Cannot offer option {0} through pack {1}, since it isn't a suffix option.", modelGenerationGradeOption.Name, modelGenerationPack.Name))
        End Sub
    End Class
End Namespace