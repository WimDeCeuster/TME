Namespace Exceptions
    Public Class OptionIsNotASuffixOptionException
        Inherits Exception

        Public Sub New(ByVal carPackItem As CarPackItem)
            MyBase.New(String.Format("The item {0} in pack {1} is not a suffix option, so should not be recalculated!", carPackItem.AlternateName, carPackItem.Pack.AlternateName))
        End Sub
    End Class
End NameSpace