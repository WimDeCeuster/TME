Namespace Exceptions
    Public Class SuffixOptionAlreadyOfferedThroughPackException
        Inherits Exception

        Public Sub New(ByVal modelGenerationGradeOption As ModelGenerationGradeOption, ByVal modelGenerationPack As ModelGenerationPack)
            MyBase.New(String.Format("Suffix option {0} is already offered through pack {1}", modelGenerationGradeOption.Name, modelGenerationPack.Name))
        End Sub
    End Class
End Namespace