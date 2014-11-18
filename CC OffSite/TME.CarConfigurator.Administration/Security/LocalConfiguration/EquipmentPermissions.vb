Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.LocalConfiguration

    Public Class EquipmentPermissions
        Implements IEquipmentPermissions

        Public ReadOnly Property UsesNonVATPrice() As Boolean Implements IEquipmentPermissions.UsesNonVATPrice
            Get
                Return MyContext.GetContext().UsesNonVATPrice
            End Get
        End Property
    End Class
End Namespace