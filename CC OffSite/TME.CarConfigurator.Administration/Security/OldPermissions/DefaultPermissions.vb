Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.OldPermissions
    <Serializable()>
    Public NotInheritable Class DefaultPermissions
        Implements IGlobalPermissions

#Region " Business Properties & Methods "

        <NotUndoable()> Private ReadOnly _context As MyContext
        <NotUndoable()> Private ReadOnly _corePermissions As CorePermissions
        <NotUndoable()> Private ReadOnly _assetPermissions As LinkedAssetPermissions
        <NotUndoable()> Private ReadOnly _carPermissions As CarPermissions
        <NotUndoable()> Private ReadOnly _equipmentPermissions As EquipmentPermissions
        <NotUndoable()> Private ReadOnly _exteriorColourPermissions As ExteriorColourPermissions
        <NotUndoable()> Private ReadOnly _factoryCarPermissions As FactoryCarPermissions
        <NotUndoable()> Private ReadOnly _interiorColourPermissions As InteriorColourPermissions
        <NotUndoable()> Private ReadOnly _modelGenerationPermissions As ModelGenerationPermissions
        <NotUndoable()> Private ReadOnly _modelPermissions As ModelPermissions
        <NotUndoable()> Private ReadOnly _activeFilterToolPermissions As ActiveFilterToolPermissions
        <NotUndoable()> Private ReadOnly _sepcificationPermissions As SpecificationPermissions
        <NotUndoable()> Private ReadOnly _trimmingPermissions As TrimmingPermissions
        <NotUndoable()> Private ReadOnly _upholsteryPermissions As UpholsteryPermissions
        <NotUndoable()> Private ReadOnly _linkPermissions As LinkPermissions

        Public ReadOnly Property Core() As CorePermissions
            Get
                Return _corePermissions
            End Get
        End Property

        Public ReadOnly Property Model() As ModelPermissions
            Get
                Return _modelPermissions
            End Get
        End Property

        Public ReadOnly Property ActiveFilterTool() As ActiveFilterToolPermissions
            Get
                Return _activeFilterToolPermissions
            End Get
        End Property

        Public ReadOnly Property ModelGeneration() As ModelGenerationPermissions
            Get
                Return _modelGenerationPermissions
            End Get
        End Property

        Public ReadOnly Property Car() As CarPermissions
            Get
                Return _carPermissions
            End Get
        End Property

        Public ReadOnly Property FactoryCar() As FactoryCarPermissions
            Get
                Return _factoryCarPermissions
            End Get
        End Property

        Public ReadOnly Property ExteriorColour() As ExteriorColourPermissions
            Get
                Return _exteriorColourPermissions
            End Get
        End Property


        Public ReadOnly Property Upholstery() As UpholsteryPermissions
            Get
                Return _upholsteryPermissions
            End Get
        End Property

        Public ReadOnly Property InteriorColour() As InteriorColourPermissions
            Get
                Return _interiorColourPermissions
            End Get
        End Property

        Public ReadOnly Property Trimming() As TrimmingPermissions
            Get
                Return _trimmingPermissions
            End Get
        End Property

        Public ReadOnly Property Equipment() As EquipmentPermissions
            Get
                Return _equipmentPermissions
            End Get
        End Property

        Public ReadOnly Property Specifications() As SpecificationPermissions
            Get
                Return _sepcificationPermissions
            End Get
        End Property

        Public ReadOnly Property Assets() As LinkedAssetPermissions
            Get
                Return _assetPermissions
            End Get
        End Property

        Public ReadOnly Property Translate() As Boolean Implements IGlobalPermissions.Translate
            Get
                'Return Not oContext.CountryCode.Equals(Environment.GlobalCountryCode) AndAlso (Thread.CurrentPrincipal.IsInRole("MKT Administrator") OrElse Thread.CurrentPrincipal.IsInRole("NMSC Administrator"))
                Return _
                    (Thread.CurrentPrincipal.IsInRole("MKT Administrator") OrElse
                     Thread.CurrentPrincipal.IsInRole("NMSC Administrator") OrElse
                     Thread.CurrentPrincipal.IsInRole("CSG LTH Administrator") OrElse
                     Thread.CurrentPrincipal.IsInRole("NMSC LTH Administrator"))
            End Get
        End Property

        Public ReadOnly Property Links() As LinkPermissions
            Get
                Return _linkPermissions
            End Get
        End Property

        Public ReadOnly Property ManageSettings() As Boolean Implements IGlobalPermissions.ManageSettings
            Get
                Return Thread.CurrentPrincipal.IsInRole("ISG Administrator") AndAlso MyContext.GetContext().IsGlobal
            End Get
        End Property

#End Region

#Region " Constructors "

        Protected Friend Sub New(ByVal context As MyContext)
            _context = context
            _corePermissions = New CorePermissions(_context)
            _modelPermissions = New ModelPermissions(context)
            _activeFilterToolPermissions = New ActiveFilterToolPermissions(context)
            _modelGenerationPermissions = New ModelGenerationPermissions(context)
            _factoryCarPermissions = New FactoryCarPermissions(context)
            _carPermissions = New CarPermissions(context)
            _exteriorColourPermissions = New ExteriorColourPermissions(context)
            _upholsteryPermissions = New UpholsteryPermissions(context)
            _interiorColourPermissions = New InteriorColourPermissions(context)
            _trimmingPermissions = New TrimmingPermissions(context)

            _sepcificationPermissions = New SpecificationPermissions(context)
            _equipmentPermissions = New EquipmentPermissions(context)

            _assetPermissions = New LinkedAssetPermissions(context)
            _linkPermissions = New LinkPermissions()
        End Sub

#End Region
    End Class
End Namespace