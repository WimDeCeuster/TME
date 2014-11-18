Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.LocalConfiguration

    Public Class GlobalPermissions
        Implements IGlobalPermissions

        Public ReadOnly Property ManageSettings() As Boolean Implements IGlobalPermissions.ManageSettings
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property Translate() As Boolean Implements IGlobalPermissions.Translate
            Get
                Return True
            End Get
        End Property
    End Class
End Namespace