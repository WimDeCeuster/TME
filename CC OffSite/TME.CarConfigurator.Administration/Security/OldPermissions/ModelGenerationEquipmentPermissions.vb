Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.OldPermissions
    <Serializable()> Public Class ModelGenerationEquipmentPermissions
        Implements IModelGenerationEquipmentPermissions

        Private ReadOnly _items As IPermissions
        Private ReadOnly _packs As IPermissions

        Public ReadOnly Property Items() As IPermissions Implements IModelGenerationEquipmentPermissions.Items
            Get
                Return _items
            End Get
        End Property
        Public ReadOnly Property Packs() As IPermissions Implements IModelGenerationEquipmentPermissions.Packs
            Get
                Return _packs
            End Get
        End Property

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            _items = New EquipmentItemsPermissions(context)
            _packs = New EquipmentPackPermissions(context)
        End Sub
#End Region

    End Class
End Namespace