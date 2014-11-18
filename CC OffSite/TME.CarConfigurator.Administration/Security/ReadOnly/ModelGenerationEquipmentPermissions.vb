Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.ReadOnly

    Public Class ModelGenerationEquipmentPermissions
        Implements IModelGenerationEquipmentPermissions

        Private ReadOnly _items As IPermissions = New Permissions()
        Private ReadOnly _packs As IPermissions = New Permissions()

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
    End Class
End Namespace