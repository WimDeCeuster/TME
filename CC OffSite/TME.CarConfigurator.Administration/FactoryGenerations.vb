Imports TME.BusinessObjects.Templates.SqlServer
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class FactoryGenerations
    Inherits ContextUniqueGuidListBase(Of FactoryGenerations, FactoryGeneration)

#Region " Business Properties & Methods "

    Private Property FactoryModel() As FactoryModel
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, FactoryModel)
        End Get
        Set(ByVal value As FactoryModel)
            SetParent(value)
        End Set
    End Property

    Public Overrides Function Add(ByVal id As Guid) As FactoryGeneration
        Dim factoryGeneration As FactoryGeneration = MyBase.Add(id)
        factoryGeneration.FactoryModel = FactoryModel.GetInfo()
        Return factoryGeneration
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetFactoryGenerations(ByVal factoryModel As FactoryModel) As FactoryGenerations
        Dim factoryGenerations = DataPortal.Fetch(Of FactoryGenerations)(New GetByModelCodeCriteria(factoryModel.Code))
        factoryGenerations.FactoryModel = factoryModel
        Return factoryGenerations
    End Function
    Public Shared Function GetFactoryGenerationsByProjectID(ByVal id As Guid) As FactoryGenerations
        Dim factoryGenerations = DataPortal.Fetch(Of FactoryGenerations)(New GetByProjectIDCriteria(id))
        factoryGenerations.AllowNew = False
        Return factoryGenerations
    End Function
#End Region

#Region " Criteria "
    <Serializable()> Private Class GetByModelCodeCriteria
        Inherits CommandCriteria

        Private ReadOnly _factoryModelCode As String

        Public Sub New(ByVal factoryModelCode As String)
            _factoryModelCode = factoryModelCode
        End Sub

        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@MODELCODE", _factoryModelCode)
        End Sub

    End Class

    <Serializable()> Private Class GetByProjectIDCriteria
        Inherits CommandCriteria

        Private ReadOnly _projectId As Guid

        Public Sub New(ByVal projectId As Guid)
            _projectId = projectId
            CommandText = "getFactoryGenerationsByProjectID"
        End Sub

        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@PROJECTID", _projectId)
        End Sub

    End Class
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowEdit = MyContext.GetContext().IsGlobal()
        AllowNew = AllowEdit
        AllowRemove = AllowEdit
    End Sub
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function Contains(ByVal ssn As String) As Boolean
        Return Items.Any(Function(fg) fg.SSN = ssn)
    End Function

#End Region

End Class
<Serializable()> Public NotInheritable Class FactoryGeneration
    Inherits ContextUniqueGuidBusinessBase(Of FactoryGeneration)

#Region " Business Properties & Methods "
    Private _modelInfo As FactoryModelInfo
    Private _ssn As String
    Private _fromDate As Date
    Private _toDate As Date
    Private _projectId As Guid
    Private _projectCode As String
    Private _carFamilyCode As String
    Private _description As String
    Private _options As FactoryGenerationOptions
    Private WithEvents _factoryCars As FactoryCars

    Public Property FactoryModel() As FactoryModelInfo
        Get
            Return _modelInfo
        End Get
        Friend Set(ByVal value As FactoryModelInfo)
            _modelInfo = value
        End Set
    End Property


    Public Property SSN() As String
        Get
            Return _ssn
        End Get
        Set(ByVal value As String)
            If _ssn.IsSameAs(value, False, StringComparison.InvariantCulture) Then Return

            _ssn = value
            PropertyHasChanged("SSN")
        End Set
    End Property
    Public Property FromDate() As Date
        Get
            Return _fromDate
        End Get
        Set(ByVal value As Date)
            If _fromDate.Equals(value) Then Return

            _fromDate = value
            PropertyHasChanged("FromDate")
    End Set
    End Property
    Public Property ToDate() As Date
        Get
            Return _toDate
        End Get
        Set(ByVal value As Date)
            If _toDate.Equals(value) Then Return

            _toDate = value
            PropertyHasChanged("ToDate")
        End Set
    End Property
    Public Property ProjectID() As Guid
        Get
            Return _projectId
        End Get
        Set(ByVal value As Guid)
            If _projectId.Equals(value) Then Return

            _projectId = value
            PropertyHasChanged("ProjectID")
        End Set
    End Property
    Public Property ProjectCode() As String
        Get
            Return _projectCode
        End Get
        Set(ByVal value As String)
            If _projectCode.IsSameAs(value) Then Return

            _projectCode = value
            PropertyHasChanged("ProjectCode")
        End Set
    End Property
    Public Property CarFamilyCode() As String
        Get
            Return _carFamilyCode
        End Get
        Set(ByVal value As String)
            If _carFamilyCode.IsSameAs(value) Then Return

            _carFamilyCode = value
            PropertyHasChanged("CarFamilyCode")
 End Set
    End Property
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            If _description.IsSameAs(value) Then Return

            _description = value
            PropertyHasChanged("Description")
        End Set
    End Property
    Public ReadOnly Property Options() As FactoryGenerationOptions
        Get
            If _options Is Nothing Then
                If IsNew Then
                    _options = FactoryGenerationOptions.NewFactoryGenerationOptions(Me)
                Else
                    _options = FactoryGenerationOptions.GetFactoryGenerationOptions(Me)
                End If
            End If
            Return _options
        End Get
    End Property
    Public ReadOnly Property FactoryCars() As FactoryCars
        Get
            If _factoryCars Is Nothing Then
                If IsNew Then
                    _factoryCars = FactoryCars.NewFactoryCars(Me)
                Else
                    _factoryCars = FactoryCars.GetFactoryCars(Me)
                End If
            End If
            Return _factoryCars
        End Get
    End Property

    Public Function GetInfo() As FactoryGenerationInfo
        Return FactoryGenerationInfo.GetFactoryGenerationInfo(Me)
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return SSN
    End Function

#End Region

#Region " Shared Factory Methods "
    Public Shared Function GetFactoryGeneration(ByVal id As Guid) As FactoryGeneration
        Return DataPortal.Fetch(Of FactoryGeneration)(New Criteria(id))
    End Function
#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "SSN")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "FromDate")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "ToDate")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Description")
        ValidationRules.AddRule(DirectCast(AddressOf ProjectRequired, Validation.RuleHandler), "ProjectID")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "ProjectCode")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "CarFamilyCode")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("SSN", 10))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Description", 510))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("ProjectCode", 10))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("CarFamilyCode", 10))

    End Sub

    Private Shared Function ProjectRequired(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim obj = DirectCast(target, FactoryGeneration)

        If obj.ProjectID.Equals(Guid.Empty) Then
            e.Description = "The project id is required!"
            Return False
        End If
        Return True
    End Function
#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_options Is Nothing) AndAlso Not _options.IsValid Then Return False
            If Not (_factoryCars Is Nothing) AndAlso Not _factoryCars.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_options Is Nothing) AndAlso _options.IsDirty Then Return True
            If Not (_factoryCars Is Nothing) AndAlso _factoryCars.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Not _options Is Nothing Then _options.Update(transaction)
        If Not _factoryCars Is Nothing Then
            _factoryCars.Update(transaction, ChangesToUpdate.Insert Or ChangesToUpdate.Update)
            _factoryCars.Update(transaction, ChangesToUpdate.Delete)
        End If
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@MODELCODE", FactoryModel.Code)
        command.Parameters.AddWithValue("@SSN", SSN)
        command.Parameters.AddWithValue("@FROMDATE", FromDate)
        command.Parameters.AddWithValue("@TODATE", ToDate)
        command.Parameters.AddWithValue("@PROJECTID", ProjectID)
        command.Parameters.AddWithValue("@PROJECTCODE", ProjectCode)
        command.Parameters.AddWithValue("@CARFAMILYCODE", CarFamilyCode)
        command.Parameters.AddWithValue("@DESCRIPTION", Description)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@TODATE", ToDate)
        command.Parameters.AddWithValue("@PROJECTID", ProjectID)
        command.Parameters.AddWithValue("@PROJECTCODE", ProjectCode)
        command.Parameters.AddWithValue("@CARFAMILYCODE", CarFamilyCode)
        command.Parameters.AddWithValue("@DESCRIPTION", Description)
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _modelInfo = FactoryModelInfo.GetFactoryModelInfo(dataReader)
        _ssn = dataReader.GetString(GetFieldName("SSN"))
        _fromDate = dataReader.GetDateTime(GetFieldName("FROMDATE"))
        _toDate = dataReader.GetDateTime(GetFieldName("TODATE"))
        _projectId = dataReader.GetGuid(GetFieldName("PROJECTID"))
        _projectCode = dataReader.GetString(GetFieldName("PROJECTCODE"))
        _carFamilyCode = dataReader.GetString(GetFieldName("CARFAMILYCODE"))
        _description = dataReader.GetString(GetFieldName("DESCRIPTION"))
    End Sub
    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        _options = FactoryGenerationOptions.GetFactoryGenerationOptions(Me, dataReader)

        dataReader.NextResult()
        While dataReader.Read
            _options(dataReader.GetGuid("FACTORYGENERATIONOPTIONID")).Values.Add(dataReader)
        End While
    End Sub

#End Region

End Class
<Serializable(), XmlInfo("factorygeneration")> Public NotInheritable Class FactoryGenerationInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _modelInfo As FactoryModelInfo = FactoryModelInfo.Empty
    Private _ssn As String = String.Empty
    Private _fromDate As Date = Date.MinValue
    Private _toDate As Date = Date.MaxValue
    Private _projectId As Guid = Guid.Empty
    Private _projectCode As String = String.Empty
    Private _carFamilyCode As String = String.Empty
    Private _description As String = String.Empty

    Public ReadOnly Property FactoryModel() As FactoryModelInfo
        Get
            Return _modelInfo
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property SSN() As String
        Get
            Return _ssn
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property FromDate() As Date
        Get
            Return _fromDate
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ToDate() As Date
        Get
            Return _toDate
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ProjectID() As Guid
        Get
            Return _projectId
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ProjectCode() As String
        Get
            Return _projectCode
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property CarFamilyCode() As String
        Get
            Return _carFamilyCode
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Description() As String
        Get
            Return _description
        End Get
    End Property
    Public Function IsEmpty() As Boolean
        Return ID.Equals(Guid.Empty)
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return String.Format("{0} ({1}) [{2} - {3}]", SSN, CarFamilyCode, FromDate.ToShortDateString(), ToDate.ToShortDateString())
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As FactoryGeneration) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationFactoryGeneration) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As FactoryGenerationInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is FactoryGenerationInfo Then
            Return Equals(DirectCast(obj, FactoryGenerationInfo))
        ElseIf TypeOf obj Is ModelGenerationFactoryGeneration Then
            Return Equals(DirectCast(obj, ModelGenerationFactoryGeneration))
        ElseIf TypeOf obj Is FactoryGeneration Then
            Return Equals(DirectCast(obj, FactoryGeneration))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is FactoryGenerationInfo Then
            Return DirectCast(objA, FactoryGenerationInfo).Equals(objB)
        ElseIf TypeOf objB Is FactoryGenerationInfo Then
            Return DirectCast(objB, FactoryGenerationInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetFactoryGenerationInfo(ByVal factoryGeneration As FactoryGeneration) As FactoryGenerationInfo
        Dim info As FactoryGenerationInfo = New FactoryGenerationInfo
        info.Fetch(factoryGeneration)
        Return info
    End Function
    Friend Shared Function GetFactoryGenerationInfo(ByVal dataReader As SafeDataReader) As FactoryGenerationInfo
        Dim info As FactoryGenerationInfo = New FactoryGenerationInfo
        info.Fetch(dataReader)
        Return info
    End Function
    Public Shared ReadOnly Property Empty() As FactoryGenerationInfo
        Get
            Return New FactoryGenerationInfo
        End Get
    End Property

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Private Sub Fetch(ByVal factoryGeneration As FactoryGeneration)
        With factoryGeneration
            _modelInfo = .FactoryModel
            _id = .ID
            _ssn = .SSN
            _fromDate = .FromDate
            _toDate = .ToDate
            _projectId = .ProjectID
            _projectCode = .ProjectCode
            _carFamilyCode = .CarFamilyCode
            _description = .Description
        End With
    End Sub

    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _modelInfo = FactoryModelInfo.GetFactoryModelInfo(dataReader)
            _id = .GetGuid("FACTORYGENERATIONID")
            _ssn = .GetString("FACTORYGENERATIONSSN")
            _fromDate = .GetDateTime("FACTORYGENERATIONFROMDATE")
            _toDate = .GetDateTime("FACTORYGENERATIONTODATE")
            _projectId = .GetGuid("FACTORYGENERATIONPROJECTID")
            _projectCode = .GetString("FACTORYGENERATIONPROJECTCODE")
            _carFamilyCode = .GetString("FACTORYGENERATIONCARFAMILYCODE")
            _description = .GetString("FACTORYGENERATIONDESCRIPTION")
        End With
    End Sub


#End Region

End Class