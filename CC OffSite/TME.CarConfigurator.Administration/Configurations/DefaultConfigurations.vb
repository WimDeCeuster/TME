Imports TME.CarConfigurator.Administration.Enums
Imports TME.BusinessObjects.Validation

Namespace Configurations
    <Serializable()> Public NotInheritable Class DefaultConfigurations
        Inherits ContextUniqueGuidListBase(Of DefaultConfigurations, DefaultConfiguration)


#Region " Business Properties & Methods "
        Friend Property Generation() As ModelGeneration
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, ModelGeneration)
            End Get
            Private Set(ByVal value As ModelGeneration)
                SetParent(value)

                AddHandler value.CarAvailabilityChanged, AddressOf CheckRules
                AddHandler value.CarRemoved, AddressOf CheckRules
                AddHandler value.ColourAvailabilityChanged, AddressOf CheckRules
            End Set
        End Property

        Private Sub CheckRules()
            For Each defaultConfiguration In Me
                defaultConfiguration.CheckRules()
            Next
        End Sub

        Public Shadows Function Add(ByVal type As ConfigurationType, ByVal target As ModelGeneration) As DefaultConfiguration
            Dim newDefaultConfiguration = DefaultConfiguration.NewDefaultConfiguration(type, target.ID, Entity.MODELGENERATION)
            Add(newDefaultConfiguration)
            Return newDefaultConfiguration
        End Function
        Public Shadows Function Add(ByVal type As ConfigurationType, ByVal target As ModelGeneration, ByVal exteriorColour As ExteriorColourInfo) As DefaultConfiguration
            Dim newDefaultConfiguration = DefaultConfiguration.NewDefaultConfiguration(type, target.ID, Entity.MODELGENERATION, exteriorColour)
            Add(newDefaultConfiguration)
            Return newDefaultConfiguration
        End Function
        Public Shadows Function Add(ByVal type As ConfigurationType, ByVal target As ModelGenerationSubModel) As DefaultConfiguration
            Dim newDefaultConfiguration = DefaultConfiguration.NewDefaultConfiguration(type, target.ID, Entity.SUBMODEL)
            Add(newDefaultConfiguration)
            Return newDefaultConfiguration
        End Function
        Public Shadows Function Add(ByVal type As ConfigurationType, ByVal target As ModelGenerationSubModel, ByVal exteriorColour As ExteriorColourInfo) As DefaultConfiguration
            Dim newDefaultConfiguration = DefaultConfiguration.NewDefaultConfiguration(type, target.ID, Entity.SUBMODEL, exteriorColour)
            Add(newDefaultConfiguration)
            Return newDefaultConfiguration
        End Function
        Public Shadows Function Add(ByVal type As ConfigurationType, ByVal target As ModelGenerationGrade) As DefaultConfiguration
            Dim newDefaultConfiguration = DefaultConfiguration.NewDefaultConfiguration(type, target.ID, Entity.MODELGENERATIONGRADE)
            Add(newDefaultConfiguration)
            Return newDefaultConfiguration
        End Function
        Public Shadows Function Add(ByVal type As ConfigurationType, ByVal target As ModelGenerationGrade, ByVal exteriorColour As ExteriorColourInfo) As DefaultConfiguration
            Dim newDefaultConfiguration = DefaultConfiguration.NewDefaultConfiguration(type, target.ID, Entity.MODELGENERATIONGRADE, exteriorColour)
            Add(newDefaultConfiguration)
            Return newDefaultConfiguration
        End Function
        Public Shadows Function Add(ByVal type As ConfigurationType, ByVal target As ModelGenerationGradeSubModel) As DefaultConfiguration
            Dim newDefaultConfiguration = DefaultConfiguration.NewDefaultConfiguration(type, target.ID, Entity.MODELGENERATIONGRADESUBMODEL)
            Add(newDefaultConfiguration)
            Return newDefaultConfiguration
        End Function
        Public Shadows Function Add(ByVal type As ConfigurationType, ByVal target As ModelGenerationGradeSubModel, ByVal exteriorColour As ExteriorColourInfo) As DefaultConfiguration
            Dim newDefaultConfiguration = DefaultConfiguration.NewDefaultConfiguration(type, target.ID, Entity.MODELGENERATIONGRADESUBMODEL, exteriorColour)
            Add(newDefaultConfiguration)
            Return newDefaultConfiguration
        End Function
        Public Shadows Function Add(ByVal type As ConfigurationType, ByVal target As ModelGenerationPack) As DefaultConfiguration
            Dim newDefaultConfiguration = DefaultConfiguration.NewDefaultConfiguration(type, target.ID, Entity.PACK)
            Add(newDefaultConfiguration)
            Return newDefaultConfiguration
        End Function
        Public Shadows Function Add(ByVal type As ConfigurationType, ByVal target As ModelGenerationPack, ByVal exteriorColour As ExteriorColourInfo) As DefaultConfiguration
            Dim newDefaultConfiguration = DefaultConfiguration.NewDefaultConfiguration(type, target.ID, Entity.PACK, exteriorColour)
            Add(newDefaultConfiguration)
            Return newDefaultConfiguration
        End Function

        Private Shadows Sub Add(ByVal value As DefaultConfiguration)
            If value.ExteriorColourID.Equals(Guid.Empty) Then
                If Any(Function(configuration) configuration.ForObjectID.Equals(value.ForObjectID) AndAlso configuration.ConfigurationType.Equals(value.ConfigurationType) AndAlso configuration.ForObjectEntity.Equals(value.ForObjectEntity)) Then
                    Throw New ValidationException("A default configuration for this object and type already exists")
                End If
            Else
                If Any(Function(configuration) configuration.ForObjectID.Equals(value.ForObjectID) AndAlso configuration.ConfigurationType.Equals(value.ConfigurationType) AndAlso configuration.ForObjectEntity.Equals(value.ForObjectEntity) AndAlso configuration.ExteriorColourID.Equals(value.ExteriorColourID)) Then
                    Throw New ValidationException("A default configuration for this object, type and exterior colour already exists")
                End If
            End If

            MyBase.Add(value)
        End Sub
#End Region

#Region " Shared Factory Methods "

        Friend Shared Function GetDefaultConfigurations(ByVal generation As ModelGeneration) As DefaultConfigurations
            Dim defaultConfigurations = DataPortal.Fetch(Of DefaultConfigurations)(New ParentCriteria(generation.ID, "@GENERATIONID"))
            defaultConfigurations.Generation = generation
            Return defaultConfigurations
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            MarkAsChild()
        End Sub
#End Region


    End Class
    <Serializable()> Public NotInheritable Class DefaultConfiguration
        Inherits ContextUniqueGuidBusinessBase(Of DefaultConfiguration)

#Region " Business Properties & Methods "
        Private _configurationType As String
        Private _forObjectId As Guid
        Private _forObjectEntity As Entity

        Private _carId As Guid
        Private _exteriorColourId As Guid
        Private _upholsteryId As Guid

        Private _equipment As DefaultConfigurationSelectedEquipment
        Private _packs As DefaultConfigurationSelectedPacks

        Private ReadOnly Property Generation() As ModelGeneration
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, DefaultConfigurations).Generation
            End Get
        End Property

        Public ReadOnly Property ConfigurationType() As String
            Get
                Return _configurationType
            End Get
        End Property
        Public ReadOnly Property ForObjectID() As Guid
            Get
                Return _forObjectId
            End Get
        End Property
        Public ReadOnly Property ForObjectEntity() As Entity
            Get
                Return _forObjectEntity
            End Get
        End Property

        Public Property Car() As Car
            Get
                If Generation Is Nothing Then Return Nothing
                Return Generation.Cars(_carId)
            End Get
            Set(ByVal value As Car)
                Dim newCarId = If(value Is Nothing, Guid.Empty, value.ID)
                If _carId = newCarId Then Return

                _carId = newCarId
                PropertyHasChanged("Car")
            End Set
        End Property
        Public Property ExteriorColour() As ExteriorColourInfo
            Get
                Dim carExteriorColour = Car.ColourCombinations.ExteriorColours().SingleOrDefault(Function(x) x.ID.Equals(_exteriorColourId))
                If carExteriorColour Is Nothing Then Return ExteriorColourInfo.Empty
                Return carExteriorColour.GetInfo()

            End Get
            Set(ByVal value As ExteriorColourInfo)
                If _exteriorColourId = value.ID Then Return

                _exteriorColourId = value.ID
                PropertyHasChanged("ExteriorColour")
            End Set
        End Property
        Public Property Upholstery() As UpholsteryInfo
            Get
                Dim carUpholstery = Car.ColourCombinations.Upholsteries().SingleOrDefault(Function(x) x.ID.Equals(_upholsteryId))
                If carUpholstery Is Nothing Then Return UpholsteryInfo.Empty
                Return carUpholstery.GetInfo()

            End Get
            Set(ByVal value As UpholsteryInfo)
                If _upholsteryId = value.ID Then Return

                _upholsteryId = value.ID
                PropertyHasChanged("Upholstery")
            End Set
        End Property

        Public ReadOnly Property ExteriorColourID() As Guid
            Get
                Return _exteriorColourId
            End Get
        End Property

        Public ReadOnly Property Equipment As DefaultConfigurationSelectedEquipment
            Get
                If _equipment Is Nothing Then
                    _equipment = DefaultConfigurationSelectedEquipment.GetSelectedEquipment(Me)
                End If
                Return _equipment
            End Get
        End Property

        Public ReadOnly Property Packs As DefaultConfigurationSelectedPacks
            Get
                If _packs Is Nothing Then
                    _packs = DefaultConfigurationSelectedPacks.GetSelectedPacks(Me)
                End If
                Return _packs
            End Get
        End Property

        Protected Overrides Sub OnPropertyChanged(ByVal propertyName As String)
            MyBase.OnPropertyChanged(propertyName)
            ValidationRules.CheckRules("Car")
            ValidationRules.CheckRules("ExteriorColour")
            ValidationRules.CheckRules("Upholstery")
        End Sub
#End Region

#Region " Business & Validation Rules "
        Friend Sub CheckRules()
            ValidationRules.CheckRules()
        End Sub
        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf IsCarAvailable, RuleHandler), "Car")
            ValidationRules.AddRule(DirectCast(AddressOf IsExteriorColurAvailable, RuleHandler), "ExteriorColour")
            ValidationRules.AddRule(DirectCast(AddressOf IsUpholsteryAvailable, RuleHandler), "Upholstery")
        End Sub

        Private Shared Function IsCarAvailable(ByVal target As Object, ByVal e As RuleArgs) As Boolean
            Dim defaultConfiguration = DirectCast(target, DefaultConfiguration)

            If defaultConfiguration._carId.Equals(Guid.Empty) Then
                e.Description = "No vehicle has been selected"
                Return False
            End If
            
            If defaultConfiguration.Car Is Nothing Then
                e.Description = "The selected vehicle is no longer available"
                Return False
            End If

            If Not defaultConfiguration.Car.Activated Then
                e.Description = "The selected vehicle is not activated"
                Return False
            End If

            If Not (defaultConfiguration.Car.Approved OrElse defaultConfiguration.Car.Preview) Then
                e.Description = "The selected vehicle is not approved or not in preview"
                Return False
            End If

            Return True
        End Function
        Private Shared Function IsExteriorColurAvailable(ByVal target As Object, ByVal e As RuleArgs) As Boolean
            Dim defaultConfiguration = DirectCast(target, DefaultConfiguration)
            If defaultConfiguration.Car Is Nothing Then Return True 'already validated by IsCarValidated 

            If defaultConfiguration._exteriorColourId.Equals(Guid.Empty) Then
                e.Description = "No exterior colour has been selected"
                Return False
            End If

            If Not defaultConfiguration.Car.ColourCombinations.ExteriorColours().Any(Function(x) x.Equals(defaultConfiguration._exteriorColourId)) Then
                e.Description = "The selected exterior colour is no longer available"
                Return False
            End If

            If defaultConfiguration._upholsteryId.Equals(Guid.Empty) Then Return True 'no need to validate the combination

            Dim combination = defaultConfiguration.Car.ColourCombinations(defaultConfiguration._exteriorColourId, defaultConfiguration._upholsteryId)
            If combination Is Nothing Then
                e.Description = "The selected colour combination is no longer available"
                Return False
            End If

            If Not combination.Approved Then
                e.Description = "The selected colour combination is not approved"
                Return False
            End If

            Return True
        End Function
        Private Shared Function IsUpholsteryAvailable(ByVal target As Object, ByVal e As RuleArgs) As Boolean
            Dim defaultConfiguration = DirectCast(target, DefaultConfiguration)
            If defaultConfiguration.Car Is Nothing Then Return True 'already validated by IsCarValidated 

            If defaultConfiguration._upholsteryId.Equals(Guid.Empty) Then
                e.Description = "No upholstery has been selected"
                Return False
            End If

            If Not defaultConfiguration.Car.ColourCombinations.Upholsteries().Any(Function(x) x.Equals(defaultConfiguration._upholsteryId)) Then
                e.Description = "The selected upholstery is no longer available"
                Return False
            End If

            If defaultConfiguration._exteriorColourId.Equals(Guid.Empty) Then Return True 'no need to validate the combination
            If Not defaultConfiguration.Car.ColourCombinations.Contains(defaultConfiguration._exteriorColourId, defaultConfiguration._upholsteryId) Then
                e.Description = "The selected colour combination is no longer available"
                Return False
            End If
            Return True
        End Function


#End Region

#Region " Shared Factory Methods "

        Friend Shared Function NewDefaultConfiguration(ByVal type As ConfigurationType, ByVal forObjectId As Guid, ByVal forObjectEntity As Entity) As DefaultConfiguration
            If Not type.SupportedEntities.Contains(forObjectEntity) Then Throw New ValidationException(String.Format("{0} can not be used for {1}", forObjectEntity.GetTitle(True), type.Description))

            Dim defaultConfiguration = New DefaultConfiguration()
            defaultConfiguration._configurationType = type.Code
            defaultConfiguration._forObjectId = forObjectId
            defaultConfiguration._forObjectEntity = forObjectEntity
            Return defaultConfiguration
        End Function

        Friend Shared Function NewDefaultConfiguration(ByVal type As ConfigurationType, ByVal forObjectId As Guid, ByVal forObjectEntity As Entity, ByVal exteriorColour As ExteriorColourInfo) As DefaultConfiguration
            If Not type.SupportedEntities.Contains(forObjectEntity) Then Throw New ValidationException(String.Format("{0} can not be used for {1}", forObjectEntity.GetTitle(True), type.Description))

            Dim defaultConfiguration = New DefaultConfiguration()
            defaultConfiguration._configurationType = type.Code
            defaultConfiguration._forObjectId = forObjectId
            defaultConfiguration._forObjectEntity = forObjectEntity
            defaultConfiguration._exteriorColourId = exteriorColour.ID
            Return defaultConfiguration
        End Function


#End Region

#Region " Framework Overrides "

        Public Overloads Overrides ReadOnly Property IsValid() As Boolean
            Get
                If Not MyBase.IsValid Then Return False
                If Not (_equipment Is Nothing) AndAlso Not _equipment.IsValid Then Return False
                If Not (_packs Is Nothing) AndAlso Not _packs.IsValid Then Return False
                Return True
            End Get
        End Property
        Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
            Get
                If MyBase.IsDirty Then Return True
                If (Not _equipment Is Nothing) AndAlso _equipment.IsDirty Then Return True
                If (Not _packs Is Nothing) AndAlso _packs.IsDirty Then Return True
                Return False
            End Get
        End Property

#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return String.Format("{0} ({1}/{2})", Car.ToString(), ExteriorColour.Code, Upholstery.Code)
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            AutoDiscover = False 'There is nothing to discover anyway
            MarkAsChild()
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub InitializeFields()
            MyBase.InitializeFields()
            _carId = Guid.Empty
            _exteriorColourId = Guid.Empty
            _upholsteryId = Guid.Empty
        End Sub
        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            MyBase.FetchFields(dataReader)
            _configurationType = dataReader.GetString("CONFIGURATIONTYPE")
            _forObjectId = dataReader.GetGuid("FOROBJECTID")
            _forObjectEntity = dataReader.GetEntity("FOROBJECTENTITY")
            _carId = dataReader.GetGuid("CARID")
            _exteriorColourId = dataReader.GetGuid("EXTERIORCOLOURID")
            _upholsteryId = dataReader.GetGuid("UPHOLSTERYID")
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
        End Sub
        Private Sub AddCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@CONFIGURATIONTYPE", ConfigurationType)
            command.Parameters.AddWithValue("@FOROBJECTID", ForObjectID.GetDbValue())
            command.Parameters.AddWithValue("@FOROBJECTENTITY", ForObjectEntity.ToString())
            command.Parameters.AddWithValue("@CARID", _carId.GetDbValue())
            command.Parameters.AddWithValue("@EXTERIORCOLOURID", _exteriorColourId.GetDbValue())
            command.Parameters.AddWithValue("@UPHOLSTERYID", _upholsteryId.GetDbValue())
        End Sub

        Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            MyBase.UpdateChildren(transaction)

            If Not _equipment Is Nothing Then _equipment.Update(transaction)
            If Not _packs Is Nothing Then _packs.Update(transaction)
        End Sub
#End Region

    End Class
End Namespace