Namespace Exceptions
    Public Class AccessoryCategoryContainsSubCategoriesException
        Inherits Exception

        Public Sub New(ByVal categoryName As String)
            MyBase.New(String.Format("The accessory category '{0}' contains subcategories. Please delete all subcategories of '{0}' before deleting '{0}'.", categoryName))
        End Sub
    End Class
End Namespace