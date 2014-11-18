Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.OldPermissions
    <Serializable()>
    Public Class EquipmentPermissions
        Implements IEquipmentPermissions

        Private ReadOnly _groups As IPermissions
        Private ReadOnly _categories As IPermissions
        Private ReadOnly _accessories As AccessoriesPermissions
        Private ReadOnly _options As OptionsPermissions
        Private ReadOnly _usesNonVATPrice As Boolean

        Public ReadOnly Property Groups() As IPermissions
            Get
                Return _groups
            End Get
        End Property

        Public ReadOnly Property Categories() As IPermissions
            Get
                Return _categories
            End Get
        End Property

        Public ReadOnly Property Accessories() As AccessoriesPermissions
            Get
                Return _accessories
            End Get
        End Property

        Public ReadOnly Property Options() As OptionsPermissions
            Get
                Return _options
            End Get
        End Property

        Public ReadOnly Property UsesNonVATPrice() As Boolean Implements IEquipmentPermissions.UsesNonVATPrice
            Get
                Return _usesNonVATPrice
            End Get
        End Property

#Region " Constructors "

        Friend Sub New(ByVal context As MyContext)
            _groups = New EquipmentGroupsPermissions(context)
            _categories = New EquipmentCategoriesPermissions(context)
            _accessories = New AccessoriesPermissions(context)
            _options = New OptionsPermissions(context)
            _usesNonVATPrice = context.UsesNonVATPrice
        End Sub

#End Region
    End Class
End Namespace