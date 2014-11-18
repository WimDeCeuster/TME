Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class SpecificationValuesPermissions
        Inherits Permissions

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)
            Update = True
        End Sub

#End Region

    End Class
End Namespace