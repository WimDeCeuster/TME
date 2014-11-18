Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.ReadOnly

    Public Class Permissions
        Implements IPermissions

        Private _activate As Boolean
        Private _approve As Boolean
        Private _create As Boolean
        Private _delete As Boolean
        Private _preview As Boolean
        Private _price As Boolean
        Private _sort As Boolean
        Private _translate As Boolean
        Private _update As Boolean
        Private _viewDetails As Boolean = True
        Private _viewAssets As Boolean = True

        Public Property Activate() As Boolean Implements IPermissions.Activate
            Get
                Return _activate
            End Get
            Set(ByVal value As Boolean)
                _activate = value
            End Set
        End Property

        Public Property Approve() As Boolean Implements IPermissions.Approve
            Get
                Return _approve
            End Get
            Set(ByVal value As Boolean)
                _approve = value
            End Set
        End Property

        Public Property Create() As Boolean Implements IPermissions.Create
            Get
                Return _create
            End Get
            Set(ByVal value As Boolean)
                _create = value
            End Set
        End Property

        Public Property Delete() As Boolean Implements IPermissions.Delete
            Get
                Return _delete
            End Get
            Set(ByVal value As Boolean)
                _delete = value
            End Set
        End Property

        Public Property Preview() As Boolean Implements IPermissions.Preview
            Get
                Return _preview
            End Get
            Set(ByVal value As Boolean)
                _preview = value
            End Set
        End Property

        Public Property Price() As Boolean Implements IPermissions.Price
            Get
                Return _price
            End Get
            Set(ByVal value As Boolean)
                _price = value
            End Set
        End Property

        Public Property Sort() As Boolean Implements IPermissions.Sort
            Get
                Return _sort
            End Get
            Set(ByVal value As Boolean)
                _sort = value
            End Set
        End Property

        Public Property Translate() As Boolean Implements IPermissions.Translate
            Get
                Return _translate
            End Get
            Set(ByVal value As Boolean)
                _translate = value
            End Set
        End Property

        Public Property Update() As Boolean Implements IPermissions.Update
            Get
                Return _update
            End Get
            Set(ByVal value As Boolean)
                _update = value
            End Set
        End Property

        Public Property UsesNonVATPrice() As Boolean Implements IPermissions.UsesNonVATPrice
            Get
                Return MyContext.GetContext().UsesNonVATPrice()
            End Get
            Set(ByVal value As Boolean)
                'do nothing
            End Set
        End Property

        Public Property ViewDetails() As Boolean Implements IPermissions.ViewDetails
            Get
                Return _viewDetails
            End Get
            Set(ByVal value As Boolean)
                _viewDetails = value
            End Set
        End Property

        Public Property ViewAssets() As Boolean Implements IPermissions.ViewAssets
            Get
                Return _viewAssets
            End Get
            Set(ByVal value As Boolean)
                _viewAssets = value
            End Set
        End Property
    End Class
End Namespace