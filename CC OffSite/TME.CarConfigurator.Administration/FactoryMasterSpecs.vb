Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class FactoryMasterSpecs
    Inherits BaseObjects.ContextUniqueGuidListBase(Of FactoryMasterSpecs, FactoryMasterSpec)

#Region " Shared Factory Methods "

    Public Shared Function GetFactoryMasterSpecs() As FactoryMasterSpecs
        Return DataPortal.Fetch(Of FactoryMasterSpecs)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.AllowNew = MyContext.GetContext().IsGlobal()
        Me.AllowEdit = False
        Me.AllowRemove = False
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class FactoryMasterSpec
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of FactoryMasterSpec)

#Region " Business Properties & Methods "
    Private _description As String = String.Empty
    Private _marketingIrrelevant As Boolean

    <XmlInfo(XmlNodeType.Attribute)> Public Property Description() As String
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
    Public Property MarketingIrrelevant() As Boolean
        Get
            Return _marketingIrrelevant
        End Get
        Set(ByVal value As Boolean)
            If _marketingIrrelevant <> value Then
                _marketingIrrelevant = value
                PropertyHasChanged("MarketingIrrelevant")
            End If
        End Set
    End Property

    Public Function GetInfo() As FactoryMasterSpecInfo
        Return FactoryMasterSpecInfo.GetFactoryMasterSpecInfo(Me)
    End Function

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Description")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Description", 80))
    End Sub

#End Region

#Region " System.Object Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return Me.ID
    End Function
    Public Overloads Overrides Function ToString() As String
        Return Me.Description
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf (obj) Is Guid Then
            Me.Equals(DirectCast(obj, Guid))
        Else
            Return MyBase.Equals(obj)
        End If
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.AllowNew = MyContext.GetContext().IsGlobal()
        Me.AllowEdit = MyContext.GetContext().IsGlobal()
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        With dataReader
            _description = .GetString("DESCRIPTION")
            _marketingIrrelevant = .GetBoolean("MARKETINGIRRELEVANT")
        End With
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@DESCRIPTION", Me.Description)
        command.Parameters.AddWithValue("@MARKETINGIRRELEVANT", Me.MarketingIrrelevant)
    End Sub
#End Region


End Class
<Serializable(), XmlInfo("FactoryMasterSpec")> Public NotInheritable Class FactoryMasterSpecInfo

#Region " Business Properties & Methods "
    Private _id As Guid
    Private _description As String

    <XmlInfo(XmlNodeType.Attribute)> Public Property ID() As Guid
        Get
            Return _id
        End Get
        Private Set(ByVal value As Guid)
            _id = value
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Description() As String
        Get
            Return _description
        End Get
        Private Set(ByVal value As String)
            _description = value
        End Set
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Description
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return Me.ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As FactoryMasterSpec) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As FactoryMasterSpecInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is FactoryMasterSpecInfo Then
            Return Me.Equals(DirectCast(obj, FactoryMasterSpecInfo))
        ElseIf TypeOf obj Is FactoryMasterSpec Then
            Return Me.Equals(DirectCast(obj, FactoryMasterSpec))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If objA Is Nothing AndAlso objB Is Nothing Then Return True
        If objA Is Nothing AndAlso Not objB Is Nothing Then Return False
        If Not objA Is Nothing AndAlso objB Is Nothing Then Return False

        If TypeOf objA Is FactoryMasterSpecInfo Then
            Return DirectCast(objA, FactoryMasterSpecInfo).Equals(objB)
        ElseIf TypeOf objB Is FactoryMasterSpecInfo Then
            Return DirectCast(objB, FactoryMasterSpecInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetFactoryMasterSpecInfo(ByVal factoryMasterSpec As FactoryMasterSpec) As FactoryMasterSpecInfo
        Dim factoryMasterSpecInfo As FactoryMasterSpecInfo = New FactoryMasterSpecInfo
        factoryMasterSpecInfo.ID = factoryMasterSpec.ID
        factoryMasterSpecInfo.Description = factoryMasterSpec.Description
        Return factoryMasterSpecInfo
    End Function
    Friend Shared Function GetFactoryMasterSpecInfo(ByVal dataReader As SafeDataReader) As FactoryMasterSpecInfo
        Dim factoryMasterSpecInfo As FactoryMasterSpecInfo = New FactoryMasterSpecInfo
        factoryMasterSpecInfo.ID = dataReader.GetGuid("FACTORYMASTERSPECID")
        factoryMasterSpecInfo.Description = dataReader.GetString("FACTORYMASTERSPECDESCRIPTION")
        Return factoryMasterSpecInfo
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class