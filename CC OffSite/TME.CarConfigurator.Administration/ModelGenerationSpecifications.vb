Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class ModelGenerationSpecifications
    Inherits ContextUniqueGuidListBase(Of ModelGenerationSpecifications, ModelGenerationSpecification)

#Region " Business Properties & Methods "

    Friend Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGeneration)
        End Get
        Private Set(ByVal value As ModelGeneration)
            SetParent(value)
        End Set
    End Property


    Private Sub NormalizeValues()
        For Each generationSpecification In Me
            generationSpecification.NormalizeValues()
        Next
    End Sub
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetModelGenerationSpecifications(ByVal generation As ModelGeneration) As ModelGenerationSpecifications
        Dim specifications As ModelGenerationSpecifications = DataPortal.Fetch(Of ModelGenerationSpecifications)(New CustomCriteria(generation))
        specifications.Generation = generation
        specifications.NormalizeValues()
        Return specifications
    End Function


#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
        MarkAsChild()
        AllowNew = False
        AllowRemove = False
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Public ReadOnly ModelID As Guid
        Public ReadOnly GenerationID As Guid

        Public Sub New(ByVal generation As ModelGeneration)
            ModelID = generation.Model.ID
            GenerationID = generation.ID
            CommandText = "getModelGenerationSpecifications"
            CommandTimeout = 500
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", GenerationID)
        End Sub

    End Class
#End Region

#Region " Data Access "
    Protected Overrides Sub Fetch(ByVal dataReader As Common.Database.SafeDataReader, ByVal criteria As CommandCriteria)
        Dim modelId As Guid = DirectCast(criteria, CustomCriteria).ModelID
        Dim generationId As Guid = DirectCast(criteria, CustomCriteria).GenerationID

        AllowNew = True
        While dataReader.Read()
            Add(ModelGenerationSpecification.GetModelGenerationSpecification(modelId, generationId, dataReader))
        End While
        AllowNew = False
    End Sub
    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            Dim id As Guid = Guid.Empty
            Dim specification As ModelGenerationSpecification = Nothing

            While .Read()
                If Not id.Equals(.GetGuid("TECHSPECID")) Then
                    id = .GetGuid("TECHSPECID")
                    specification = Item(id)
                End If
                If Not specification Is Nothing Then
                    Dim country = dataReader.GetString("COUNTRY")
                    Dim language = dataReader.GetString("LANGUAGE")
                    Dim carSpecification = PartialCarSpecification.GetPartialCarSpecification(dataReader)

                    If Not specification.Values.Contains(country, language, carSpecification) Then
                        specification.Values.Add(dataReader)
                    End If
                End If
            End While
        End With
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class ModelGenerationSpecification
    Inherits LocalizeableBusinessBase
    Implements IOwnedBy

#Region " Business Properties & Methods "
    Private _objectId As Guid = Guid.Empty
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _status As Integer
    Private _owner As String = String.Empty
    Private _keyFeature As Boolean
    Private _brochure As Boolean
    Private _energyEfficiencySpecification As Boolean
    Private _quickSpecification As Boolean
    Private _fullSpecification As Boolean
    Private _hasMapping As Boolean
    Private _unit As UnitInfo
    Private _category As SpecificationCategoryInfo
    Private _dependency As ModelGenerationSpecificationDependency
    Private _values As ModelGenerationSpecificationValues
    Private _expression As String = String.Empty
    Private _helpText As String = String.Empty
    Private _typeCode As TypeCode = TypeCode.String
    Private _index As Integer

    <NotUndoable()> Private _modelID As Guid = Guid.Empty
    <NotUndoable()> Private _generationID As Guid = Guid.Empty



    Friend ReadOnly Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationSpecifications).Generation
        End Get
    End Property

    Private ReadOnly Property ObjectID() As Guid
        Get
            Return _objectId
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public Property Approved() As Boolean
        Get
            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Approved) Then Return

            If value Then
                _status += Status.ApprovedForLive
            Else
                _status -= Status.ApprovedForLive
            End If
            Declined = Not (value)
            PropertyHasChanged("Approved")
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Declined() As Boolean
        Get
            Return ((_status And Status.Declined) = Status.Declined)
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Declined) Then Return

            If value Then
                _status += Status.Declined
            Else
                _status -= Status.Declined
            End If
            Approved = Not (value)
            PropertyHasChanged("Declined")

        End Set
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public Property KeyFeature() As Boolean
        Get
            Return _keyFeature
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_keyFeature) Then
                _keyFeature = value
                PropertyHasChanged("KeyFeature")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Brochure() As Boolean
        Get
            Return _brochure
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_brochure) Then
                _brochure = value
                PropertyHasChanged("Brochure")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property EnergyEfficiencySpecification() As Boolean
        Get
            Return _energyEfficiencySpecification
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_energyEfficiencySpecification) Then
                _energyEfficiencySpecification = value
                PropertyHasChanged("EnergyEfficiencySpecification")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property QuickSpecification() As Boolean
        Get
            Return _quickSpecification
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_quickSpecification) Then
                _quickSpecification = value
                PropertyHasChanged("QuickSpecification")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property FullSpecification() As Boolean
        Get
            Return _fullSpecification
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_fullSpecification) Then
                _fullSpecification = value
                PropertyHasChanged("FullSpecification")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property HasMapping() As Boolean
        Get
            Return _hasMapping
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return _owner
        End Get
    End Property
    Public ReadOnly Property Unit() As UnitInfo
        Get
            Return _unit
        End Get
    End Property
    Public ReadOnly Property Dependency() As ModelGenerationSpecificationDependency
        Get
            Return _dependency
        End Get
    End Property
    Public ReadOnly Property Category() As SpecificationCategoryInfo
        Get
            Return _category
        End Get
    End Property
    Public ReadOnly Property Values() As ModelGenerationSpecificationValues
        Get
            Return _values
        End Get
    End Property
    Friend Function GetValue(ByVal car As Car, ByVal country As String, ByVal language As String) As ModelGenerationSpecificationValue
        Dim carValues As IEnumerable(Of ModelGenerationSpecificationValue) =
            (From value In Values
                 Where
                        value.Matches(car) AndAlso
                        value.Equals(country, language)
                ).ToArray()
        If carValues.Count() > 1 Then
            Throw New Exceptions.MultipleSpecificationValuesFoundForCar(car, Me, carValues)
        End If
        Return carValues.FirstOrDefault()
    End Function
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Expression() As String
        Get
            Return _expression
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property TypeCode() As TypeCode
        Get
            Return _typeCode
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property HelpText() As String
        Get
            Return _helpText
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Index() As Integer
        Get
            Return _index
        End Get
    End Property



    Public Function IsMasterSpecification() As Boolean
        If Not HasMapping Then Return False
        Return Generation.Mode = LocalizationMode.LocalConfiguration OrElse Generation.Mode = LocalizationMode.Suffix
    End Function


    Friend Sub NormalizeValues()
        For Each generationValue In Values
            generationValue.NormalizeValues()
        Next
    End Sub

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        If Owner = Environment.GlobalCountryCode OrElse MyContext.GetContext.CountryCode.Equals(Owner) Then
            Return Name
        Else
            Return Name & " [" & Owner & "]"
        End If
    End Function

    Public Overloads Function Equals(ByVal obj As Specification) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationSpecification) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationSpecification Then
            Return Equals(DirectCast(obj, ModelGenerationSpecification))
        ElseIf TypeOf obj Is Specification Then
            Return Equals(DirectCast(obj, Specification))
        ElseIf TypeOf obj Is Guid Then
            Return ID.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not Dependency.IsValid Then Return False
            If Not Values.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Dependency.IsDirty Then Return True
            If Values.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetModelGenerationSpecification(ByVal modelID As Guid, ByVal generationID As Guid, ByVal dataReader As SafeDataReader) As ModelGenerationSpecification
        Dim specification As ModelGenerationSpecification = New ModelGenerationSpecification()
        specification._modelID = modelID
        specification._generationID = generationID
        specification.Fetch(dataReader)
        Return specification
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowRemove = False
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _objectId = .GetGuid("OBJECTID")
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _owner = .GetString("OWNER")
            _status = .GetInt16("STATUSID")
            _energyEfficiencySpecification = .GetBoolean("ENERGYEFFICIENCYSPEC")
            _keyFeature = .GetBoolean("KEYFEATURE")
            _brochure = .GetBoolean("BROCHURE")
            _quickSpecification = .GetBoolean("QUICKSPEC")
            _fullSpecification = .GetBoolean("FULLSPEC")
            _hasMapping = .GetBoolean("HASMAPPING")
            _expression = .GetString("EXPRESSION")
            _helpText = .GetString("HELPTEXT")
            _typeCode = CType(.GetInt16("TYPECODE"), TypeCode)
            _index = .GetInt16("SORTORDER")
        End With
        _unit = UnitInfo.GetUnitInfo(dataReader)
        _category = SpecificationCategoryInfo.GetSpecificationCategoryInfo(dataReader)
        _dependency = ModelGenerationSpecificationDependency.GetModelGenerationSpecificationDependency(Me, dataReader)
        _values = ModelGenerationSpecificationValues.NewModelGenerationSpecificationValues(Me)
        MyBase.FetchFields(dataReader)

        If _objectId.Equals(Guid.Empty) Then
            'In case the model generation specifc instance of this specification does not exist yet
            'then make sure it gets inserted and the dependency is copied in case the specification is owner by the country
            _objectId = Guid.NewGuid()
            If _dependency.AllowEdit Then _dependency.MarkDirty()
            MarkNew()
        End If

    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Private Sub AddCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@ID", ObjectID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@TECHSPECID", ID)
        command.Parameters.AddWithValue("@GENERATIONID", GenerationID)
        command.Parameters.AddWithValue("@STATUSID", _status)
        command.Parameters.AddWithValue("@KEYFEATURE", KeyFeature)
        command.Parameters.AddWithValue("@BROCHURE", Brochure)
        command.Parameters.AddWithValue("@ENERGYEFFICIENCYSPEC", EnergyEfficiencySpecification)
        command.Parameters.AddWithValue("@QUICKSPEC", QuickSpecification)
        command.Parameters.AddWithValue("@FULLSPEC", FullSpecification)
    End Sub
    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Dependency.IsDirty Then Dependency.Update(transaction)
        If Values.IsDirty Then Values.Update(transaction)
    End Sub
#End Region

#Region " Base Object Overrides"
    Protected Friend Overrides Function GetBaseCode() As String
        Return Code
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Name
    End Function
    Public Overrides ReadOnly Property ModelID() As Guid
        Get
            Return _modelID
        End Get
    End Property
    Public Overrides ReadOnly Property GenerationID() As Guid
        Get
            Return _generationID
        End Get
    End Property


    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.MODELGENERATIONTECHSPEC
        End Get
    End Property
#End Region



End Class
<Serializable()> Public NotInheritable Class ModelGenerationSpecificationDependency
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationSpecificationDependency)

#Region " Business Properties & Methods "
    <NotUndoable()> Private _specification As ModelGenerationSpecification
    Private _modelOrder As Integer
    Private _generationOrder As Integer
    Private _bodyTypeOrder As Integer
    Private _engineOrder As Integer
    Private _transmissionOrder As Integer
    Private _wheelDriveOrder As Integer
    Private _gradeOrder As Integer

    Public ReadOnly Property Code() As String
        Get
            Return ModelOrder.ToString() & GenerationOrder.ToString() & BodyTypeOrder.ToString() & EngineOrder.ToString() & TransmissionOrder.ToString() & WheelDriveOrder.ToString() & GradeOrder.ToString()
        End Get
    End Property
    Public Property ModelOrder() As Integer
        Get
            Return _modelOrder
        End Get
        Set(ByVal value As Integer)
            If _modelOrder.Equals(value) Then Return
            _modelOrder = value
            PropertyHasChanged("ModelOrder")
        End Set
    End Property
    Public Property GenerationOrder() As Integer
        Get
            Return _generationOrder
        End Get
        Set(ByVal value As Integer)
            If _generationOrder.Equals(value) Then Return
            _generationOrder = value
            PropertyHasChanged("GenerationOrder")
        End Set
    End Property
    Public Property BodyTypeOrder() As Integer
        Get
            Return _bodyTypeOrder
        End Get
        Set(ByVal value As Integer)
            If _bodyTypeOrder.Equals(value) Then Return

            _bodyTypeOrder = value
            PropertyHasChanged("BodyTypeOrder")
        End Set
    End Property
    Public Property EngineOrder() As Integer
        Get
            Return _engineOrder
        End Get
        Set(ByVal value As Integer)
            If _engineOrder.Equals(value) Then Return

            _engineOrder = value
            PropertyHasChanged("EngineOrder")
        End Set
    End Property
    Public Property TransmissionOrder() As Integer
        Get
            Return _transmissionOrder
        End Get
        Set(ByVal value As Integer)
            If _transmissionOrder.Equals(value) Then Return

            _transmissionOrder = value
            PropertyHasChanged("TransmissionOrder")
        End Set
    End Property
    Public Property WheelDriveOrder() As Integer
        Get
            Return _wheelDriveOrder
        End Get
        Set(ByVal value As Integer)
            If _wheelDriveOrder.Equals(value) Then Return

            _wheelDriveOrder = value
            PropertyHasChanged("WheelDriveOrder")
        End Set
    End Property
    Public Property GradeOrder() As Integer
        Get
            Return _gradeOrder
        End Get
        Set(ByVal value As Integer)
            If _gradeOrder.Equals(value) Then Return

            _gradeOrder = value
            PropertyHasChanged("GradeOrder")
        End Set
    End Property

    Private Property Specification() As ModelGenerationSpecification
        Get
            Return _specification
        End Get
        Set(ByVal value As ModelGenerationSpecification)
            _specification = value
        End Set
    End Property

    Friend Overloads Sub MarkDirty()
        MyBase.MarkDirty()
    End Sub




#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Code
    End Function

    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return Not (obj Is Nothing) AndAlso Code.Equals(obj)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationSpecificationDependency) As Boolean
        Return Not (obj Is Nothing) AndAlso Code.Equals(obj.Code)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationSpecificationDependency Then
            Return Equals(DirectCast(obj, ModelGenerationSpecificationDependency))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        ElseIf TypeOf obj Is String Then
            Return Equals(DirectCast(obj, String))
        Else
            Return False
        End If
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetModelGenerationSpecificationDependency(ByVal specification As ModelGenerationSpecification, ByVal dataReader As SafeDataReader) As ModelGenerationSpecificationDependency
        Dim dependency As ModelGenerationSpecificationDependency = New ModelGenerationSpecificationDependency
        dependency.Specification = specification
        dependency.Fetch(dataReader)
        dependency.AllowEdit = (String.Compare(MyContext.GetContext().CountryCode, specification.Owner, True) = 0)
        Return dependency
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowNew = False
        AllowRemove = False
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("TECHSPECRULEID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _modelOrder = .GetInt16("MODELORDER")
            _generationOrder = .GetInt16("GENERATIONORDER")
            _bodyTypeOrder = .GetInt16("BODYORDER")
            _engineOrder = .GetInt16("ENGINEORDER")
            _transmissionOrder = .GetInt16("TRANSMISSIONORDER")
            _wheelDriveOrder = .GetInt16("WHEELDRIVEORDER")
            _gradeOrder = .GetInt16("GRADEORDER")
        End With
    End Sub

    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command
            .Parameters.AddWithValue("@TECHSPECID", Specification.ID)
            .Parameters.AddWithValue("@GENERATIONID", Specification.Generation.ID)
            .Parameters.AddWithValue("@MODELORDER", ModelOrder)
            .Parameters.AddWithValue("@GENERATIONORDER", GenerationOrder)
            .Parameters.AddWithValue("@BODYORDER", BodyTypeOrder)
            .Parameters.AddWithValue("@ENGINEORDER", EngineOrder)
            .Parameters.AddWithValue("@TRANSMISSIONORDER", TransmissionOrder)
            .Parameters.AddWithValue("@WHEELDRIVEORDER", WheelDriveOrder)
            .Parameters.AddWithValue("@GRADEORDER", GradeOrder)
        End With
    End Sub

#End Region

End Class

