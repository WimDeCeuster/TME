Imports TME.CarConfigurator.Administration.Security
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class Specifications
    Inherits BaseObjects.StronglySortedListBase(Of Specifications, Specification)

#Region " Business Properties & Methods "

    Public ReadOnly Property Category() As SpecificationCategory
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, SpecificationCategory)
        End Get
    End Property

    Friend Overloads Sub Add(ByVal dataReader As SafeDataReader)
        MyBase.Add(GetObject(dataReader))
    End Sub
    Default Public Overloads ReadOnly Property Item(ByVal code As String, ByVal localCode As Boolean) As Specification
        Get
            For Each _item As Specification In Me
                If _item.Equals(code, localCode) Then
                    Return _item
                End If
            Next
            Return Nothing
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewSpecifications(ByVal category As SpecificationCategory) As Specifications
        Dim _specifications As Specifications = New Specifications()
        _specifications.SetParent(category)
        Return _specifications
    End Function
    Friend Shared Function GetSpecifications(ByVal category As SpecificationCategory) As Specifications
        Dim _specifications As Specifications = DataPortal.Fetch(Of Specifications)(New CustomCriteria(category))
        _specifications.SetParent(category)
        Return _specifications
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _categoryID As Guid

        Public Sub New(ByVal category As SpecificationCategory)
            _categoryID = category.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@CATEGORYID", _categoryID)
        End Sub
    End Class
#End Region

End Class
<Serializable()> Public NotInheritable Class Specification
    Inherits BaseObjects.LocalizeableBusinessBase
    Implements BaseObjects.ISortedIndex
    Implements BaseObjects.ISortedIndexSetter
    Implements IOwnedBy

#Region " Business Properties & Methods "

    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _status As Integer
    Private _index As Integer

    Private _owner As String = String.Empty
    Private _expression As String = String.Empty
    Private _typeCode As TypeCode
    Private _helpText As String = String.Empty
    Private _valueFormat As String
    Private _importFormat As String = String.Empty
    Private WithEvents _mapping As SpecificationMapping

    Private _brochure As Boolean
    Private _energyEfficiencySpecification As Boolean
    Private _quickSpecification As Boolean
    Private _fullSpecification As Boolean
    Private _compareable As Boolean
    Private _summaryValue As ValueSelectionType
    Private _promotedValue As ValueSelectionType

    Private _unit As UnitInfo
    Private _dependency As SpecificationDependency

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            If _code <> value Then
                _code = value
                If Me.AllowEdit Then _localCode = value
                PropertyHasChanged("Code")
            End If
        End Set
    End Property
    Private Sub SpecificatiopnPropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If e.PropertyName.Equals("LocalCode", StringComparison.InvariantCultureIgnoreCase) Then
            If Me.Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) Then
                Me.Code = Me.LocalCode
            End If
        End If
    End Sub

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            If _name <> value Then
                _name = value
                PropertyHasChanged("Name")
            End If
        End Set
    End Property

    Public Property Activated() As Boolean
        Get
            Return ((_status And Status.AvailableToNmscs) = Status.AvailableToNmscs)
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Me.Activated) Then
                If Me.Activated Then
                    _status -= Status.AvailableToNmscs
                Else
                    _status += Status.AvailableToNmscs
                End If
                PropertyHasChanged("Activated")
            End If
        End Set
    End Property
    Public Property Approved() As Boolean
        Get
            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Me.Approved) Then
                If value Then
                    _status += Status.ApprovedForLive
                Else
                    _status -= Status.ApprovedForLive
                End If
                Me.Declined = Not (value)
                PropertyHasChanged("Approved")
            End If
        End Set
    End Property
    Public Property Declined() As Boolean
        Get
            Return ((_status And Status.Declined) = Status.Declined)
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Me.Declined) Then
                If value Then
                    _status += Status.Declined
                Else
                    _status -= Status.Declined
                End If
                Me.Approved = Not (value)
                PropertyHasChanged("Declined")
            End If
        End Set
    End Property

    Public Property Brochure() As Boolean
        Get
            Return _brochure
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Me.Brochure) Then
                _brochure = value
                PropertyHasChanged("Brochure")
            End If
        End Set
    End Property
    Public Property EnergyEfficiencySpecification() As Boolean
        Get
            Return _energyEfficiencySpecification
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Me.EnergyEfficiencySpecification) Then
                _energyEfficiencySpecification = value
                PropertyHasChanged("EnergyEfficiencySpecification")
            End If
        End Set
    End Property
    Public Property QuickSpecification() As Boolean
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
    Public Property FullSpecification() As Boolean
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
    Public Property Compareable() As Boolean
        Get
            Return _compareable
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_compareable) Then
                _compareable = value
                PropertyHasChanged("Compareable")
            End If
        End Set
    End Property

    Public Property SummaryValue() As ValueSelectionType
        Get
            Return _summaryValue
        End Get
        Set(ByVal value As ValueSelectionType)
            If Not value.Equals(_summaryValue) Then
                _summaryValue = value
                PropertyHasChanged("SummaryValue")
            End If
        End Set
    End Property
    Public Property PromotedValue() As ValueSelectionType
        Get
            Return _promotedValue
        End Get
        Set(ByVal value As ValueSelectionType)
            If Not value.Equals(_promotedValue) Then
                _promotedValue = value
                PropertyHasChanged("PromotedValue")
            End If
        End Set
    End Property

    Public ReadOnly Property Index() As Integer Implements BaseObjects.ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements BaseObjects.ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not PermissionBroker.GetBroker().GetPermissions(Me).Sort Then Exit Property
            If Not _index.Equals(value) Then
                _index = value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property

    Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return _owner
        End Get
    End Property
    Public Sub ChangeOwner(ByVal value As String)
        If _owner <> value Then
            _owner = value
            PropertyHasChanged("Owner")
        End If
    End Sub
    Public Property Expression() As String
        Get
            Return _expression
        End Get
        Set(ByVal value As String)
            If _expression <> value Then
                _expression = value
                PropertyHasChanged("Expression")
            End If
        End Set
    End Property
    Public Property TypeCode() As TypeCode
        Get
            Return _typeCode
        End Get
        Set(ByVal value As TypeCode)
            If Not _typeCode.Equals(value) Then
                _typeCode = value
                PropertyHasChanged("TypeCode")
            End If
        End Set
    End Property
    Public Property HelpText() As String
        Get
            Return _helpText
        End Get
        Set(ByVal value As String)
            If _helpText <> value Then
                _helpText = value
                PropertyHasChanged("HelpText")
            End If
        End Set
    End Property
    Public Property ValueFormat() As String
        Get
            Return _valueFormat
        End Get
        Set(ByVal value As String)
            value = ConvertToFriendlyPlaceHolders(value)
            If _valueFormat <> value Then
                _valueFormat = value
                PropertyHasChanged("ValueFormat")
            End If
        End Set
    End Property
    Public Property ImportFormat() As String
        Get
            Return _importFormat
        End Get
        Set(ByVal value As String)
            If _importFormat <> value Then
                _importFormat = value
                PropertyHasChanged("ImportFormat")
            End If
        End Set
    End Property

    Private _hasMapping As Nullable(Of Boolean) = New Nullable(Of Boolean)
    Public ReadOnly Property Mapping() As SpecificationMapping
        Get
            If _mapping Is Nothing Then
                If Me.IsNew Then
                    _mapping = SpecificationMapping.NewSpecificationMapping(Me)
                Else
                    _mapping = SpecificationMapping.GetSpecificationMapping(Me)
                End If
                _hasMapping = (_mapping.Count > 0)
            End If
            Return _mapping
        End Get
    End Property
    Private Sub MappingListChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _mapping.ListChanged
        If Not _hasMapping.HasValue Then Exit Sub 'not loaded yet
        If Not _hasMapping.Value.Equals(_mapping.Count > 0) Then
            _hasMapping = (_mapping.Count > 0)
            MarkDirty()
        End If
    End Sub
    Friend Sub PrepareMapping()
        _mapping = SpecificationMapping.NewSpecificationMapping(Me)
    End Sub


    Public Property Unit() As UnitInfo
        Get
            Return _unit
        End Get
        Set(ByVal value As UnitInfo)
            If Not _unit.Equals(value) Then
                _unit = value
                PropertyHasChanged("Unit")
            End If
        End Set
    End Property
    Public ReadOnly Property Dependency() As SpecificationDependency
        Get
            Return _dependency
        End Get
    End Property
    Public Property Category() As SpecificationCategory
        Get
            Return DirectCast(Me.Parent, Specifications).Category
        End Get
        Set(ByVal value As SpecificationCategory)
            If value Is Nothing Then Exit Property

            If Not Me.Category.Equals(value) Then

                'Remove from old parent, if there is one...
                Me.Category.Specifications.Remove(Me)

                'Add to new parent
                value.Specifications.Add(Me)

                'Since removal marked me for deletion,
                'I'll have to un mark myself for deletion
                Me.MarkUnDeleted()

                MarkDirty()
            End If

        End Set
    End Property


    Private Shared Function ConvertToFriendlyPlaceHolders(ByVal value As String) As String
        Return value.Replace("{0}", "{value}").Replace("{1}", "{unit}")
    End Function
    Private Shared Function ConvertToSystemPlaceHolders(ByVal value As String) As String
        Return value.Replace("{value}", "{0}").Replace("{unit}", "{1}")
    End Function

    Public Overloads Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If MyContext.GetContext().IsGlobal Then Return MyBase.CanWriteProperty(propertyName)
        If Me.Owner.Equals(MyContext.GetContext().CountryCode(), StringComparison.InvariantCultureIgnoreCase) Then Return MyBase.CanWriteProperty(propertyName)

        Select Case propertyName
            Case "Index", "Approved", "Declined", "Brochure", "EnergyEfficiencySpecification", "QuickSpecification", "FullSpecification", "ValueFormat", "Compareable", "LocalCode", "SummaryValue", "PromotedValue"
                Return MyBase.CanWriteProperty(propertyName)
            Case Else
                Return False
        End Select

    End Function

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Owner")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "ValueFormat")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 80))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Owner", 2))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("ValueFormat", 80))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("ImportFormat", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Expression", 255))

        ValidationRules.AddRule(DirectCast(AddressOf ExpressionValid, Validation.RuleHandler), "Expression")
        ValidationRules.AddRule(DirectCast(AddressOf ValueFormatValid, Validation.RuleHandler), "ValueFormat")
        ValidationRules.AddRule(DirectCast(AddressOf ImportFormatValid, Validation.RuleHandler), "ImportFormat")
        ValidationRules.AddRule(DirectCast(AddressOf TypeCodeValid, Validation.RuleHandler), "TypeCode")
        ValidationRules.AddRule(DirectCast(AddressOf TypeCodeCompareable, Validation.RuleHandler), "TypeCode")
        ValidationRules.AddRule(DirectCast(AddressOf TypeCodeCompareable, Validation.RuleHandler), "Compareable")

    End Sub

    Private Shared Function ExpressionValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _specification As Specification = DirectCast(target, Specification)
        If _specification.Expression.Length = 0 Then Return True

        Try
            Dim _regEx As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex(_specification.Expression)
            Return Not (_regEx Is Nothing)
        Catch ex As Exception
            e.Description = "The expression is not valid: " & ex.Message.Replace("{", "{{").Replace("}", "}}")
            Return False
        End Try

    End Function
    Private Shared Function ValueFormatValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _specification As Specification = DirectCast(target, Specification)
        If _specification.ValueFormat.Length = 0 Then Return True

        Try
            Return (String.Format(ConvertToSystemPlaceHolders(_specification.ValueFormat), "value", "unit").Length > 0)
        Catch ex As Exception
            e.Description = "The Display Format is not valid: " & ex.Message
            Return False
        End Try

    End Function
    Private Shared Function ImportFormatValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _specification As Specification = DirectCast(target, Specification)
        If _specification.ImportFormat.Length = 0 Then Return True

        Try
            Return (String.Format(_specification.ImportFormat, "value", "value", "value", "value", "value", "value", "value", "value", "value", "value", "value", "value", "value", "value", "value").Length > 0)
        Catch ex As Exception
            e.Description = "The Import Format is not valid: " & ex.Message
            Return False
        End Try

    End Function
    Private Shared Function TypeCodeValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _specification As Specification = DirectCast(target, Specification)
        If _specification.TypeCode = TypeCode.String Then Return True
        If _specification.TypeCode = TypeCode.Decimal Then Return True
        If _specification.TypeCode = TypeCode.Int32 Then Return True

        e.Description = "The type '" & _specification.TypeCode.ToString() & "' is currently not supported."
        Return False
    End Function
    Private Shared Function TypeCodeCompareable(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _specification As Specification = DirectCast(target, Specification)
        If _specification.Compareable AndAlso _specification.TypeCode = TypeCode.String Then
            e.Description = "The specification '" & _specification.Name & "' is of type 'String' and hence can not be compared. Either set the specification as not compareable or change the value type."
            Return False
        Else
            e.Description = ""
            Return True
        End If
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_mapping Is Nothing) AndAlso Not _mapping.IsValid Then Return False
            If Not (_dependency Is Nothing) AndAlso Not _dependency.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_mapping Is Nothing) AndAlso _mapping.IsDirty Then Return True
            If Not (_dependency Is Nothing) AndAlso _dependency.IsDirty Then Return True
            Return False
        End Get
    End Property


#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        If Me.Owner = Environment.GlobalCountryCode OrElse MyContext.GetContext.CountryCode.Equals(Me.Owner) Then
            Return Me.Name
        Else
            Return Me.Name & " [" & Me.Owner & "]"
        End If
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        If Not Me.Equals(obj, True) AndAlso Not Me.Equals(obj, False) Then Return False
        Return True
    End Function

    Public Overloads Function Equals(ByVal codeToCompare As String, ByVal isLocalCode As Boolean) As Boolean
        Dim _buffer As String
        If isLocalCode Then
            _buffer = ";" + Me.LocalCode.ToLower() + ";"
        Else
            _buffer = ";" + Me.Code.ToLower() + ";"
        End If
        Return (_buffer.IndexOf(";" & codeToCompare.ToLower & ";") > -1)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _code = ID.ToString().Substring(0, 13).ToUpper()
        _owner = MyContext.GetContext.CountryCode
        _dependency = SpecificationDependency.NewSpecificationDependency(Me)
        _unit = UnitInfo.Default()
        _summaryValue = ValueSelectionType.MostOccuring
        _promotedValue = ValueSelectionType.MostOccuring
        _valueFormat = "{value}"
        _typeCode = TypeCode.Decimal
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _status = .GetInt16("STATUSID")
            _brochure = .GetBoolean("BROCHURE")
            _energyEfficiencySpecification = .GetBoolean("ENERGYEFFICIENCYSPEC")
            _quickSpecification = .GetBoolean("QUICKSPEC")
            _fullSpecification = .GetBoolean("FULLSPEC")
            _compareable = .GetBoolean("COMPAREABLE")
            _expression = .GetString("EXPRESSION")
            _helpText = .GetString("HELPTEXT")
            _typeCode = CType(.GetInt16("TYPECODE"), TypeCode)
            _valueFormat = ConvertToFriendlyPlaceHolders(.GetString("VALUEFORMAT"))
            _importFormat = .GetString("IMPORTFORMAT")
            _index = .GetInt16("SORTORDER")
            _owner = .GetString("OWNER")
            _summaryValue = CType(.GetInt16("SUMMARYVALUE"), ValueSelectionType)
            _promotedValue = CType(.GetInt16("PROMOTEDVALUE"), ValueSelectionType)
        End With
        _unit = UnitInfo.GetUnitInfo(dataReader)
        _dependency = SpecificationDependency.GetSpecificationDependency(Me, dataReader)
        MyBase.FetchFields(dataReader)

        Me.AllowNew = True
        Me.AllowEdit = True
        Me.AllowRemove = (MyContext.GetContext().IsGlobal()) OrElse (String.Compare(MyContext.GetContext().CountryCode, Me.Owner, True) = 0)
        If Me.AllowRemove AndAlso Me.SupportsLocalCode Then
            _localCode = _code
        End If
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command
            .Parameters.AddWithValue("@OWNER", Me.Owner)
            .Parameters.AddWithValue("@INTERNALCODE", Me.Code)
            .Parameters.AddWithValue("@LOCALCODE", Me.LocalCode)
            .Parameters.AddWithValue("@SHORTNAME", Me.Name)
            .Parameters.AddWithValue("@TECHSPECCATEGORYID", Me.Category.ID)
            .Parameters.AddWithValue("@UNITID", Me.Unit.ID)
            .Parameters.AddWithValue("@EXPRESSION", Me.Expression)
            .Parameters.AddWithValue("@TYPECODE", Me.TypeCode)
            .Parameters.AddWithValue("@HELPTEXT", Me.HelpText)
            .Parameters.AddWithValue("@VALUEFORMAT", ConvertToSystemPlaceHolders(Me.ValueFormat))
            .Parameters.AddWithValue("@IMPORTFORMAT", Me.ImportFormat)
            .Parameters.AddWithValue("@STATUSID", _status)
            .Parameters.AddWithValue("@BROCHURE", Me.Brochure)
            .Parameters.AddWithValue("@ENERGYEFFICIENCYSPEC", Me.EnergyEfficiencySpecification)
            .Parameters.AddWithValue("@QUICKSPEC", Me.QuickSpecification)
            .Parameters.AddWithValue("@FULLSPEC", Me.FullSpecification)
            .Parameters.AddWithValue("@COMPAREABLE", Me.Compareable)
            .Parameters.AddWithValue("@SORTORDER", Me.Index)
            .Parameters.AddWithValue("@HASMAPPING", HasMapping())
            .Parameters.AddWithValue("@SUMMARYVALUE", Me.SummaryValue)
            .Parameters.AddWithValue("@PROMOTEDVALUE", Me.PromotedValue)
        End With
    End Sub
    Private Function HasMapping() As Object
        If _mapping Is Nothing Then Return System.DBNull.Value
        Return _mapping.Count > 0
    End Function

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        Me.Dependency.Update(transaction)
        If Not _mapping Is Nothing Then _mapping.Update(transaction)
    End Sub

#End Region

#Region "Base Object Overrides"
    Protected Friend Overrides Function GetBaseCode() As String
        Return Me.Code
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function

    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.TECHSPEC
        End Get
    End Property
#End Region

End Class
<Serializable()> Public NotInheritable Class SpecificationDependency
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of SpecificationDependency)

#Region " Business Properties & Methods "
    ' Declare variables to contain object state
    ' Declare variables for any child collections
    <NotUndoable()> Private _specification As Specification
    Private _modelOrder As Integer
    Private _generationOrder As Integer
    Private _bodyTypeOrder As Integer
    Private _engineOrder As Integer
    Private _transmissionOrder As Integer
    Private _wheelDriveOrder As Integer
    Private _gradeOrder As Integer

    ' Implement properties and methods for interaction of the UI,
    ' or any other client code, with the object
    Private Property Specification() As Specification
        Get
            Return _specification
        End Get
        Set(ByVal value As Specification)
            _specification = value
        End Set
    End Property
    Public ReadOnly Property Code() As String
        Get
            Return Me.ModelOrder.ToString() & Me.GenerationOrder.ToString() & Me.BodyTypeOrder.ToString() & Me.EngineOrder.ToString() & Me.TransmissionOrder.ToString() & Me.WheelDriveOrder.ToString() & Me.GradeOrder.ToString()
        End Get
    End Property
    Public Property ModelOrder() As Integer
        Get
            Return _modelOrder
        End Get
        Set(ByVal value As Integer)
            If Not _modelOrder.Equals(value) Then
                _modelOrder = value
                PropertyHasChanged("ModelOrder")
            End If
        End Set
    End Property
    Public Property GenerationOrder() As Integer
        Get
            Return _generationOrder
        End Get
        Set(ByVal value As Integer)
            If Not _generationOrder.Equals(value) Then
                _generationOrder = value
                PropertyHasChanged("GenerationOrder")
            End If
        End Set
    End Property
    Public Property BodyTypeOrder() As Integer
        Get
            Return _bodyTypeOrder
        End Get
        Set(ByVal value As Integer)
            If Not _bodyTypeOrder.Equals(value) Then
                _bodyTypeOrder = value
                PropertyHasChanged("BodyTypeOrder")
            End If
        End Set
    End Property
    Public Property EngineOrder() As Integer
        Get
            Return _engineOrder
        End Get
        Set(ByVal value As Integer)
            If Not _engineOrder.Equals(value) Then
                _engineOrder = value
                PropertyHasChanged("EngineOrder")
            End If
        End Set
    End Property
    Public Property TransmissionOrder() As Integer
        Get
            Return _transmissionOrder
        End Get
        Set(ByVal value As Integer)
            If Not _transmissionOrder.Equals(value) Then
                _transmissionOrder = value
                PropertyHasChanged("TransmissionOrder")
            End If
        End Set
    End Property
    Public Property WheelDriveOrder() As Integer
        Get
            Return _wheelDriveOrder
        End Get
        Set(ByVal value As Integer)
            If Not _wheelDriveOrder.Equals(value) Then
                _wheelDriveOrder = value
                PropertyHasChanged("WheelDriveOrder")
            End If
        End Set
    End Property
    Public Property GradeOrder() As Integer
        Get
            Return _gradeOrder
        End Get
        Set(ByVal value As Integer)
            If Not _gradeOrder.Equals(value) Then
                _gradeOrder = value
                PropertyHasChanged("GradeOrder")
            End If
        End Set
    End Property
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Code
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewSpecificationDependency(ByVal specification As Specification) As SpecificationDependency
        Dim _dependency As SpecificationDependency = New SpecificationDependency
        _dependency.Specification = specification
        _dependency.Create()
        Return _dependency
    End Function
    Friend Shared Function GetSpecificationDependency(ByVal specification As Specification, ByVal dataReader As SafeDataReader) As SpecificationDependency
        Dim _dependency As SpecificationDependency = New SpecificationDependency
        _dependency.Specification = specification
        _dependency.Fetch(dataReader)
        Return _dependency
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
        AutoDiscover = False
        AllowRemove = False
    End Sub
#End Region

#Region " Data Access "

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
        MarkOld()
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command
            .Parameters.AddWithValue("@TECHSPECID", Me.Specification.ID)
            .Parameters.AddWithValue("@MODELORDER", Me.ModelOrder)
            .Parameters.AddWithValue("@GENERATIONORDER", Me.GenerationOrder)
            .Parameters.AddWithValue("@BODYORDER", Me.BodyTypeOrder)
            .Parameters.AddWithValue("@ENGINEORDER", Me.EngineOrder)
            .Parameters.AddWithValue("@TRANSMISSIONORDER", Me.TransmissionOrder)
            .Parameters.AddWithValue("@WHEELDRIVEORDER", Me.WheelDriveOrder)
            .Parameters.AddWithValue("@GRADEORDER", Me.GradeOrder)
        End With
    End Sub

#End Region

End Class

<Serializable()> Public Structure SpecificationInfo
#Region " Business Properties & Methods "
    Private _id As Guid
    Private _name As String
    Private _typeCode As TypeCode
    Private _approved As Boolean
    Private _compareable As Boolean

    Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    Public ReadOnly Property TypeCode() As TypeCode
        Get
            Return _typeCode
        End Get
    End Property
    Public ReadOnly Property Approved() As Boolean
        Get
            Return _approved
        End Get
    End Property
    Public ReadOnly Property Compareable() As Boolean
        Get
            Return _compareable
        End Get
    End Property
#End Region

#Region " System.Object Overrides "
    Public Overloads Function Equals(ByVal specification As SpecificationInfo) As Boolean
        Return specification.ID.Equals(Me.ID)
    End Function
#End Region

#Region " Shared Factory Methods "
    Public Shared Function Empty() As SpecificationInfo
        Dim _info As SpecificationInfo = New SpecificationInfo
        _info._id = Guid.Empty
        _info._name = String.Empty
        _info._typeCode = System.TypeCode.Empty
        _info._approved = False
        _info._compareable = False
        Return _info
    End Function
    Public Shared Function GetSpecificationInfo(ByVal specifcation As Specification) As SpecificationInfo
        Dim _info As SpecificationInfo = New SpecificationInfo
        _info._id = specifcation.ID
        _info._name = specifcation.Name
        _info._typeCode = System.TypeCode.String
        _info._approved = specifcation.Approved
        _info._compareable = specifcation.Compareable
        Return _info
    End Function
    Friend Shared Function GetSpecificationInfo(ByVal dataReader As SafeDataReader) As SpecificationInfo
        Dim _info As SpecificationInfo = New SpecificationInfo
        _info._id = dataReader.GetGuid("TECHSPECID")
        _info._name = dataReader.GetString("TECHSPECNAME")
        _info._typeCode = CType(dataReader.GetInt16("TECHSPECTYPECODE"), System.TypeCode)
        _info._approved = dataReader.GetBoolean("TECHSPECAPPROVED")
        _info._compareable = dataReader.GetBoolean("TECHSPECCOMPAREABLE")
        Return _info
    End Function
#End Region
End Structure