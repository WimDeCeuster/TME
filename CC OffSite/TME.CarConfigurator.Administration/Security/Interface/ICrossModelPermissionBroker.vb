Namespace Security.Interface
    Public Interface ICrossModelPermissionBroker
        Function GetAccessoryPermissions(ByVal accessory As Accessory) As IEquipmentItemPermissions
    End Interface
End Namespace