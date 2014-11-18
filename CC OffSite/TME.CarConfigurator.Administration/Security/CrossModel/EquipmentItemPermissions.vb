Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.CrossModel
    Public Class EquipmentItemPermissions
        Implements IEquipmentItemPermissions
        Public Property Activate As Boolean Implements IEquipmentItemPermissions.Activate
        Public Property Approve As Boolean Implements IEquipmentItemPermissions.Approve
        Public Property Create As Boolean Implements IEquipmentItemPermissions.Create
        Public Property Delete As Boolean Implements IEquipmentItemPermissions.Delete
        Public Property Preview As Boolean Implements IEquipmentItemPermissions.Preview
        Public Property Price As Boolean Implements IEquipmentItemPermissions.Price
        Public Property Sort As Boolean Implements IEquipmentItemPermissions.Sort
        Public Property Translate As Boolean Implements IEquipmentItemPermissions.Translate
        Public Property Update As Boolean Implements IEquipmentItemPermissions.Update
        Public Property UsesNonVATPrice As Boolean Implements IEquipmentItemPermissions.UsesNonVATPrice
        Public Property ViewAssets As Boolean Implements IEquipmentItemPermissions.ViewAssets
        Public Property ViewDetails As Boolean Implements IEquipmentItemPermissions.ViewDetails
        Public Property Links As Boolean Implements IEquipmentItemPermissions.Links
    End Class
End Namespace