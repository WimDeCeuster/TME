Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class Suffixes
    Inherits BaseObjects.ContextUniqueGuidListBase(Of Suffixes, Suffix)

#Region " Business Properties & Methods "

    Friend Property FactoryCar() As FactoryCar
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, FactoryCar)
        End Get
        Private Set(ByVal value As FactoryCar)
            Me.SetParent(value)
        End Set
    End Property

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetSuffixes(ByVal factoryCar As FactoryCar) As Suffixes
        Dim _suffixes As Suffixes = DataPortal.Fetch(Of Suffixes)(New CustomCriteria(factoryCar))
        _suffixes.FactoryCar = factoryCar
        Return _suffixes
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _factoryCarID As Guid

        Public Sub New(ByVal factoryCar As FactoryCar)
            _factoryCarID = factoryCar.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@FACTORYCARID", _factoryCarID)
        End Sub
    End Class
#End Region

End Class
<Serializable()> Public NotInheritable Class Suffix
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of Suffix)

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _country As String = String.Empty
    Private _fromDate As DateTime
    Private _toDate As DateTime
    Private _mmoGrade As String = String.Empty
    Private _description As String = String.Empty
    Private _factoryCarInfo As FactoryCarInfo = Nothing
    Private WithEvents _suffixOptions As SuffixOptions = Nothing
    Private WithEvents _suffixColourCombinations As SuffixColourCombinations = Nothing
    Private _suffixChanged As Boolean = False
    Private _nmscCode As String

    Public ReadOnly Property FactoryCar() As FactoryCarInfo
        Get
            If _factoryCarInfo Is Nothing AndAlso Not Me.Parent Is Nothing Then
                _factoryCarInfo = DirectCast(Me.Parent, Suffixes).FactoryCar.GetInfo()
            End If
            Return _factoryCarInfo
        End Get
    End Property
    Public Function GetFactoryCar() As FactoryCar
        If Not Me.Parent Is Nothing Then Return DirectCast(Me.Parent, Suffixes).FactoryCar
        If Not _factoryCarInfo Is Nothing Then Return FactoryGeneration.GetFactoryGeneration(_factoryCarInfo.FactoryGeneration.ID).FactoryCars(_factoryCarInfo.ID)
        Return Nothing
    End Function

    Public Property Country() As String
        Get
            Return _country
        End Get
        Set(ByVal value As String)
            If _country <> value Then
                _country = value
                PropertyHasChanged("Country")
            End If
        End Set
    End Property
    Public Property NmscCode As String
        Get
            Return _nmscCode
        End Get
        Set(ByVal value As String)
            If _nmscCode Is Nothing OrElse Not _nmscCode.Equals(value) Then
                _nmscCode = value
                PropertyHasChanged("NmscCode")
            End If
        End Set
    End Property

    Public Property FromDate() As DateTime
        Get
            Return _fromDate
        End Get
        Set(ByVal value As DateTime)
            If _fromDate <> value Then
                _fromDate = value
                _suffixChanged = True 'This could indicate a change in the technical specifications
                PropertyHasChanged("FromDate")
            End If
        End Set
    End Property
    Public Property ToDate() As DateTime
        Get
            Return _toDate
        End Get
        Set(ByVal value As DateTime)
            If _toDate <> value Then
                _toDate = value
                PropertyHasChanged("ToDate")
            End If
        End Set
    End Property

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            If _code <> value Then
                _code = value
                PropertyHasChanged("Code")
            End If
        End Set
    End Property
    Public Property MmoGrade() As String
        Get
            Return _mmoGrade
        End Get
        Set(ByVal value As String)
            If _mmoGrade <> value Then
                _mmoGrade = value
                PropertyHasChanged("MmoGrade")
            End If
        End Set
    End Property
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            If _description <> value Then
                _description = value
                PropertyHasChanged("Description")
            End If
        End Set
    End Property

    Public ReadOnly Property Options() As SuffixOptions
        Get
            If _suffixOptions Is Nothing Then _suffixOptions = SuffixOptions.GetSuffixOptions(Me)
            Return _suffixOptions
        End Get
    End Property
    Public ReadOnly Property ColourCombinations() As SuffixColourCombinations
        Get
            If _suffixColourCombinations Is Nothing Then _suffixColourCombinations = SuffixColourCombinations.GetSuffixColourCombinations(Me)
            Return _suffixColourCombinations
        End Get
    End Property

 
    Private Sub SuffixOptionsListChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _suffixOptions.ListChanged, _suffixColourCombinations.ListChanged
        If Me.IsNew Then Exit Sub
        If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded OrElse e.ListChangedType = ComponentModel.ListChangedType.ItemDeleted Then
            _suffixChanged = True
            MarkDirty()
        End If
    End Sub

    Public Function GetInfo() As SuffixInfo
        Return SuffixInfo.GetSuffixInfo(Me)
    End Function

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Country")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "FromDate")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "ToDate")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "MmoGrade")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 2))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("MmoGrade", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Country", 2))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Description", 255))
    End Sub

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Code
    End Function

    Public Overloads Function Equals(ByVal obj As Suffix) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function

    Public Overloads Function Equals(ByVal obj As CarSuffix) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function

#End Region

#Region " Shared Factory Methods "
    Public Shared Function GetSuffix(ByVal id As Guid) As Suffix
        Return DataPortal.Fetch(Of Suffix)(New Criteria(id))
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.AutoDiscover = False
        Me.AllowEdit = True
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_suffixOptions Is Nothing) AndAlso Not _suffixOptions.IsValid Then Return False
            If Not (_suffixColourCombinations Is Nothing) AndAlso Not _suffixColourCombinations.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If (Not _suffixOptions Is Nothing) AndAlso _suffixOptions.IsDirty Then Return True
            If (Not _suffixColourCombinations Is Nothing) AndAlso _suffixColourCombinations.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Data Access "

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Not _suffixOptions Is Nothing Then _suffixOptions.Update(transaction)
        If Not _suffixColourCombinations Is Nothing Then _suffixColourCombinations.Update(transaction)
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        With dataReader
            _factoryCarInfo = FactoryCarInfo.GetFactoryCarInfo(dataReader)
            _code = .GetString("CODE")
            _country = .GetString("COUNTRY")
            _fromDate = .GetDateTime("FROMDATE")
            _toDate = .GetDateTime("TODATE")
            _mmoGrade = .GetString("MMOGRADE")
            _description = .GetString("DESCRIPTION")
            _nmscCode = .GetString("NMSCCODE").Trim()
            _suffixChanged = False
        End With
    End Sub
    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        _suffixOptions = SuffixOptions.GetSuffixOptions(Me, dataReader)

        dataReader.NextResult()
        _suffixColourCombinations = SuffixColourCombinations.GetSuffixColourCombinations(Me, dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@FACTORYCARID", Me.FactoryCar.ID)
        command.Parameters.AddWithValue("@CODE", Me.Code)
        command.Parameters.AddWithValue("@SUFFIXCOUNTRY", Me.Country)
        command.Parameters.AddWithValue("@FROMDATE", Me.FromDate)
        command.Parameters.AddWithValue("@TODATE", Me.ToDate)
        command.Parameters.AddWithValue("@MMOGRADE", Me.MmoGrade)
        command.Parameters.AddWithValue("@DESCRIPTION", Me.Description)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@TODATE", Me.ToDate)
        command.Parameters.AddWithValue("@MMOGRADE", Me.MmoGrade)
        command.Parameters.AddWithValue("@DESCRIPTION", Me.Description)
        command.Parameters.AddWithValue("@SIGNALSUFFIXCHANGE", _suffixChanged)
    End Sub

#End Region


End Class
<Serializable(), XmlInfo("suffixinfo")> Public NotInheritable Class SuffixInfo

#Region " Business Properties & Methods "

    Private _id As Guid = Guid.Empty
    Private _code As String = String.Empty
    Private _country As String = String.Empty
    Private _fromDate As DateTime
    Private _toDate As DateTime
    Private _description As String = String.Empty
    Private _mmoGrade As String = String.Empty
    Private _factoryCarInfo As FactoryCarInfo
    Private _nmscCode As String

    Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    Public ReadOnly Property Country() As String
        Get
            Return _country
        End Get
    End Property
    Public ReadOnly Property FromDate() As DateTime
        Get
            Return _fromDate
        End Get
    End Property
    Public ReadOnly Property ToDate() As DateTime
        Get
            Return _toDate
        End Get
    End Property
    Public ReadOnly Property MmoGrade() As String
        Get
            Return _mmoGrade
        End Get
    End Property
    Public ReadOnly Property Description() As String
        Get
            Return _description
        End Get
    End Property
    Public ReadOnly Property FactoryCar() As FactoryCarInfo
        Get
            Return _factoryCarInfo
        End Get
    End Property
    Public ReadOnly Property NmscCode As String
        Get
            Return _nmscCode
        End Get
    End Property

    Public Sub Refresh(ByVal suffix As Suffix)
        If Not suffix.ID.Equals(Me.ID) Then Exit Sub

        With suffix
            _code = .Code
            _country = .Country
            _fromDate = .FromDate
            _toDate = .ToDate
            _mmoGrade = .MmoGrade
            _description = .Description
            _factoryCarInfo = .FactoryCar
            _nmscCode = .NmscCode
        End With
    End Sub
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Code
    End Function

    Public Overloads Function Equals(ByVal obj As SuffixInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function

#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Shared Factory Method "

    Friend Shared Function GetSuffixInfo(ByVal suffix As Suffix) As SuffixInfo
        Dim _info As SuffixInfo = New SuffixInfo
        _info.Fetch(suffix)
        Return _info
    End Function
    Friend Shared Function GetSuffixInfo(ByVal dataReader As SafeDataReader) As SuffixInfo
        Dim _info As SuffixInfo = New SuffixInfo
        _info.Fetch(dataReader)
        Return _info
    End Function

#End Region

#Region " Data Access "

    Private Sub Fetch(ByVal suffix As Suffix)
        With suffix
            _id = .ID
            _code = .Code
            _country = .Country
            _fromDate = .FromDate
            _toDate = .ToDate
            _mmoGrade = .MmoGrade
            _description = .Description
            _nmscCode = .NmscCode
        End With
        _factoryCarInfo = suffix.FactoryCar
    End Sub

    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _id = .GetGuid("ID")
            _code = .GetString("CODE")
            _country = .GetString("COUNTRY")
            _fromDate = .GetDateTime("FROMDATE")
            _toDate = .GetDateTime("TODATE")
            _mmoGrade = .GetString("MMOGRADE")
            _description = .GetString("DESCRIPTION")
            _nmscCode = .GetString("NMSCCODE").Trim()
        End With
        _factoryCarInfo = FactoryCarInfo.GetFactoryCarInfo(dataReader)
    End Sub

#End Region

End Class