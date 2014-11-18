Namespace Security.OldPermissions
    <Serializable()> Public Class SpecificationPermissions

        Private ReadOnly _categories As SpecificationCategoriesPermissions
        Private ReadOnly _items As SpecificationItemsPermissions
        Private ReadOnly _values As SpecificationValuesPermissions

        Public ReadOnly Property Categories() As SpecificationCategoriesPermissions
            Get
                Return _categories
            End Get
        End Property

        Public ReadOnly Property Items() As SpecificationItemsPermissions
            Get
                Return _items
            End Get
        End Property

        Public ReadOnly Property Values() As SpecificationValuesPermissions
            Get
                Return _values
            End Get
        End Property

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            _categories = New SpecificationCategoriesPermissions(context)
            _items = New SpecificationItemsPermissions(context)
            _values = New SpecificationValuesPermissions(context)
        End Sub
#End Region

    End Class
End Namespace