Imports TME.CarConfigurator.Administration.Security.Interface
Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class SpecificationItemsPermissions
        Inherits Permissions
        Implements ISpecificationPermissions

        Private ReadOnly _setCompare As Boolean = False
        Public ReadOnly Property SetCompare() As Boolean Implements ISpecificationPermissions.SetCompare
            Get
                Return _setCompare
            End Get
        End Property

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)
            If context.CountryCode.Length = 0 Then Return

            If Thread.CurrentPrincipal.IsInRole("NMSC Administrator") OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator") Then
                _setCompare = String.Compare(context.CountryCode, Environment.GlobalCountryCode, True) = 0
                Create = Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry
                Update = Create
                Delete = Create
            End If
        End Sub
#End Region

    End Class
End Namespace