Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base
Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.OldPermissions
    <Serializable()>
    Public Class CarPermissions
        Inherits Permissions
        Implements ICarPermissions

        Private ReadOnly _assetPermission As IPermissions
        Private ReadOnly _coloursPersmission As IPermissions
        Private ReadOnly _equipmentPermissions As IModelGenerationEquipmentPermissions
        Private ReadOnly _packPermissions As IPermissions
        Private ReadOnly _assignSubModel As Boolean
        Private ReadOnly _promote As Boolean
        Private ReadOnly _approveForFinance As Boolean = False
        Private ReadOnly _overrule As Boolean

        Public ReadOnly Property Promote() As Boolean Implements ICarPermissions.Promote
            Get
                Return _promote
            End Get
        End Property
        Public ReadOnly Property ApproveForFinance() As Boolean Implements ICarPermissions.ApproveForFinance
            Get
                Return _approveForFinance
            End Get
        End Property

        Public ReadOnly Property Overrule() As Boolean Implements ICarPermissions.Overrule
            Get
                Return _overrule
            End Get
        End Property
        Public ReadOnly Property AssignSubModel() As Boolean Implements ICarPermissions.AssignSubModel
            Get
                Return _assignSubModel
            End Get
        End Property

        Public ReadOnly Property Assets() As IPermissions
            Get
                Return _assetPermission
            End Get
        End Property

        Public ReadOnly Property Colours() As IPermissions
            Get
                Return _coloursPersmission
            End Get
        End Property

        Public ReadOnly Property Equipment() As IModelGenerationEquipmentPermissions
            Get
                Return _equipmentPermissions
            End Get
        End Property

        Public ReadOnly Property Packs() As IPermissions
            Get
                Return _packPermissions
            End Get
        End Property

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            If context.CountryCode.Length = 0 Then Return

            If context.IsGlobal() Then
                If Thread.CurrentPrincipal.IsInRole("MKT Administrator") Then
                    _approveForFinance = True
                    Create = True
                    Update = True
                    Delete = True
                    Activate = True
                    Approve = True
                    Sort = True
                    _promote = True
                    ViewAssets = True
                    Preview = True
                End If
                _overrule = context.CanOverruleAtCarLevel
            Else
                If context.IsEurope() Then
                    Price = False
                    Translate = Thread.CurrentPrincipal.IsInRole("MKT Administrator")
                Else
                    Price = Thread.CurrentPrincipal.IsInRole("NMSC Administrator")
                    Translate = Thread.CurrentPrincipal.IsInRole("NMSC Administrator")
                End If
                If Thread.CurrentPrincipal.IsInRole("NMSC Administrator") OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator") Then
                    Approve = Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry

                    Create = Approve
                    Update = Approve  'True
                    Delete = Approve
                    _approveForFinance = Approve
                    Preview = Approve
                    Sort = Approve
                    _promote = Approve
                End If
                If Thread.CurrentPrincipal.IsInRole("NMSC FINANCE Administrator") Then
                    _approveForFinance = Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry
                End If
                _overrule = context.CanOverruleAtCarLevel
                ViewAssets = context.CanViewAssets OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator")
            End If

            UsesNonVATPrice = context.UsesNonVATPrice
            _assetPermission = New LinkedAssetPermissions(context)
            _coloursPersmission = New LinkedColourPermissions(context)
            _equipmentPermissions = New ModelGenerationEquipmentPermissions(context)
            _packPermissions = New PackPermissions(context)


            _assignSubModel = New SubModelPermissions(context).Approve
        End Sub
#End Region

    End Class
End Namespace