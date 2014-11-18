Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.OldPermissions.Base
    <Serializable()> _
    Public MustInherit Class Permissions
        Implements IPermissions

        Protected Friend Sub New()
            'Empty
        End Sub
        Protected Friend Sub New(ByVal context As MyContext)
            If context.CountryCode.Length = 0 Then Return

            UsesNonVATPrice = context.UsesNonVATPrice

            If context.IsGlobal() AndAlso Thread.CurrentPrincipal.IsInRole("MKT Administrator") Then
                Activate = True
                Approve = True
                Create = True
                Delete = True
                Preview = True
                Sort = True
                Translate = True
                Update = True
                ViewAssets = True
                ViewDetails = True
                Return
            End If

            ViewAssets = context.CanViewAssets OrElse (Thread.CurrentPrincipal.IsInRole("MKT Administrator") AndAlso IsNotARegionCountryOrIsTheMainRegionCountry(context))

            If Not context.IsEurope() Then Price = Thread.CurrentPrincipal.IsInRole("NMSC Administrator")

            If Not Thread.CurrentPrincipal.IsInRole("NMSC Administrator") AndAlso Not Thread.CurrentPrincipal.IsInRole("MKT Administrator") Then Return

            Approve = IsNotARegionCountryOrIsTheMainRegionCountry(context)
            Preview = Approve
            Sort = Approve
            Translate = True
            ViewDetails = True
        End Sub

        Private Shared Function IsNotARegionCountryOrIsTheMainRegionCountry(ByVal context As MyContext) As Boolean
            Return Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry
        End Function

#Region " Business Properties & Methods "
        Private _activate As Boolean = False
        Private _approve As Boolean = False
        Private _create As Boolean = False
        Private _delete As Boolean = False
        Private _preview As Boolean = False
        Private _price As Boolean = False
        Private _sort As Boolean = False
        Private _translate As Boolean = False
        Private _update As Boolean = False
        Private _usesNonVATPrice As Boolean = False
        Private _viewAssets As Boolean = False
        Private _viewDetails As Boolean = False

        Public Function HasAdminRights() As Boolean
            Return Me.Activate OrElse Me.Approve OrElse Me.Create OrElse Me.Delete OrElse Me.Preview OrElse Me.Price OrElse Me.Sort OrElse Me.Translate OrElse Me.Update
        End Function

        Public Property Activate() As Boolean Implements IPermissions.Activate
            Get
                Return _activate
            End Get
            Friend Set(ByVal value As Boolean)
                _activate = value
            End Set
        End Property

        Public Property Approve() As Boolean Implements IPermissions.Approve
            Get
                Return _approve
            End Get
            Friend Set(ByVal value As Boolean)
                _approve = value
            End Set
        End Property

        Public Property Create() As Boolean Implements IPermissions.Create
            Get
                Return _create
            End Get
            Friend Set(ByVal value As Boolean)
                _create = value
            End Set
        End Property

        Public Property Preview() As Boolean Implements IPermissions.Preview
            Get
                Return _preview
            End Get
            Friend Set(ByVal value As Boolean)
                _preview = value
            End Set
        End Property

        Public Property Delete() As Boolean Implements IPermissions.Delete
            Get
                Return _delete
            End Get
            Friend Set(ByVal value As Boolean)
                _delete = value
            End Set
        End Property

        Public Property Price() As Boolean Implements IPermissions.Price
            Get
                Return _price
            End Get
            Friend Set(ByVal value As Boolean)
                _price = value
            End Set
        End Property

        Public Property Sort() As Boolean Implements IPermissions.Sort
            Get
                Return _sort
            End Get
            Friend Set(ByVal value As Boolean)
                _sort = value
            End Set
        End Property

        Public Property Translate() As Boolean Implements IPermissions.Translate
            Get
                Return _translate
            End Get
            Friend Set(ByVal value As Boolean)
                _translate = value
            End Set
        End Property

        Public Property Update() As Boolean Implements IPermissions.Update
            Get
                Return _update
            End Get
            Friend Set(ByVal value As Boolean)
                _update = value
            End Set
        End Property

        Public Property UsesNonVATPrice() As Boolean Implements IPermissions.UsesNonVATPrice
            Get
                Return _usesNonVATPrice
            End Get
            Friend Set(ByVal value As Boolean)
                _usesNonVATPrice = value
            End Set
        End Property

        Public Property ViewDetails() As Boolean Implements IPermissions.ViewDetails
            Get
                Return _viewDetails
            End Get
            Friend Set(ByVal value As Boolean)
                _viewDetails = value
            End Set
        End Property

        Public Property ViewAssets() As Boolean Implements IPermissions.ViewAssets
            Get
                Return _viewAssets
            End Get
            Friend Set(ByVal value As Boolean)
                _viewAssets = value
            End Set
        End Property
#End Region

    End Class
End Namespace