Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base
Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.OldPermissions
    <Serializable()>
    Public Class ModelGenerationPermissions
        Inherits Permissions

        Private ReadOnly _assetPermission As Security.Interface.IPermissions
        Private ReadOnly _coloursPersmission As LinkedColourPermissions
        Private ReadOnly _specificationPermissions As SpecificationPermissions
        Private ReadOnly _equipmentPermissions As IModelGenerationEquipmentPermissions
        Private ReadOnly _packPermissions As PackPermissions
        Private ReadOnly _featuresPermissions As Security.Interface.IPermissions

        Public ReadOnly Property Assets() As Security.Interface.IPermissions
            Get
                Return _assetPermission
            End Get
        End Property

        Public ReadOnly Property Colours() As LinkedColourPermissions
            Get
                Return _coloursPersmission
            End Get
        End Property

        Public ReadOnly Property Specifications() As SpecificationPermissions
            Get
                Return _specificationPermissions
            End Get
        End Property

        Public ReadOnly Property Equipment() As IModelGenerationEquipmentPermissions
            Get
                Return _equipmentPermissions
            End Get
        End Property

        Public ReadOnly Property Packs() As PackPermissions
            Get
                Return _packPermissions
            End Get
        End Property

        Public ReadOnly Property Features() As IPermissions
            Get
                Return _featuresPermissions
            End Get
        End Property

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)

            If context.IsGlobal() AndAlso Thread.CurrentPrincipal.IsInRole("CORE Administrator") Then
                Update = True
            End If

            _assetPermission = New LinkedAssetPermissions(context)
            _coloursPersmission = New LinkedColourPermissions(context)
            _specificationPermissions = New SpecificationPermissions(context)
            _equipmentPermissions = New ModelGenerationEquipmentPermissions(context)
            _packPermissions = New PackPermissions(context)
            _featuresPermissions = New FeaturesPermissions(context)

            ViewAssets = (context.CanViewAssets OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator")) AndAlso (Thread.CurrentPrincipal.IsInRole("NMSC Administrator") OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator"))
            UsesNonVATPrice = context.UsesNonVATPrice

        End Sub
#End Region

    End Class
End Namespace