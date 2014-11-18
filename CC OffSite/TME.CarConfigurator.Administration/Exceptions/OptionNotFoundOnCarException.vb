Namespace Exceptions
    Public Class OptionNotFoundOnCarException
        Inherits Exception

        Public Sub New(ByVal carPackItem As CarPackItem)
            MyBase.New(String.Format("The item {0} cannot be found on car {1}!", carPackItem.AlternateName, carPackItem.Pack.Car.AlternateName))
        End Sub
    End Class
End NameSpace