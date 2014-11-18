Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions

    Public Class GradeClassificationPermissions
        Inherits Permissions
#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            If context.CountryCode.Length = 0 Then Return


            If Thread.CurrentPrincipal.IsInRole("MKT Administrator") OrElse Thread.CurrentPrincipal.IsInRole("NMSC Administrator") Then
                Create = context.IsGlobal()
                Update = Not context.IsSlaveRegionCountry
                Delete = context.IsGlobal()
                Activate = context.IsGlobal()
                Approve = Not context.IsSlaveRegionCountry
                Sort = context.IsGlobal()
                Translate = True
            End If

        End Sub
#End Region
    End Class
End Namespace
