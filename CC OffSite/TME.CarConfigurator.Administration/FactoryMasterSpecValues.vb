Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class FactoryMasterSpecValues
    Inherits ContextUniqueGuidListBase(Of FactoryMasterSpecValues, FactoryMasterSpecValue)

#Region " Shared Factory Methods "

    Public Shared Function GetFactoryMasterSpecValues() As FactoryMasterSpecValues
        Return DataPortal.Fetch(Of FactoryMasterSpecValues)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowNew = MyContext.GetContext().IsGlobal()
        AllowEdit = False
        AllowRemove = False
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class FactoryMasterSpecValue
    Inherits ContextUniqueGuidBusinessBase(Of FactoryMasterSpecValue)

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

    Public Function GetInfo() As FactoryMasterSpecValueInfo
        Return FactoryMasterSpecValueInfo.GetFactoryMasterSpecValueInfo(Me)
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
        Return ID
    End Function
    Public Overloads Overrides Function ToString() As String
        Return Description
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf (obj) Is Guid Then
            Equals(DirectCast(obj, Guid))
        Else
            Return MyBase.Equals(obj)
        End If
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowNew = MyContext.GetContext().IsGlobal()
        AllowEdit = MyContext.GetContext().IsGlobal()
        AllowRemove = False
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
        AddUpdateCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@DESCRIPTION", Description)
        command.Parameters.AddWithValue("@MARKETINGIRRELEVANT", MarketingIrrelevant)
    End Sub

#End Region
    
End Class
<Serializable(), XmlInfo("FactoryMasterSpecValue")> Public NotInheritable Class FactoryMasterSpecValueInfo

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
        Return Description
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As FactoryMasterSpecValue) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As FactoryMasterSpecValueInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is FactoryMasterSpecValueInfo Then
            Return Equals(DirectCast(obj, FactoryMasterSpecValueInfo))
        ElseIf TypeOf obj Is FactoryMasterSpecValue Then
            Return Equals(DirectCast(obj, FactoryMasterSpecValue))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If objA Is Nothing AndAlso objB Is Nothing Then Return True
        If objA Is Nothing AndAlso Not objB Is Nothing Then Return False
        If Not objA Is Nothing AndAlso objB Is Nothing Then Return False

        If TypeOf objA Is FactoryMasterSpecValueInfo Then
            Return DirectCast(objA, FactoryMasterSpecValueInfo).Equals(objB)
        ElseIf TypeOf objB Is FactoryMasterSpecValueInfo Then
            Return DirectCast(objB, FactoryMasterSpecValueInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetFactoryMasterSpecValueInfo(ByVal factoryMasterSpecValue As FactoryMasterSpecValue) As FactoryMasterSpecValueInfo
        Dim info = New FactoryMasterSpecValueInfo
        info.ID = factoryMasterSpecValue.ID
        info.Description = factoryMasterSpecValue.Description
        Return info
    End Function
    Friend Shared Function GetFactoryMasterSpecValueInfo(ByVal dataReader As SafeDataReader) As FactoryMasterSpecValueInfo
        Dim info = New FactoryMasterSpecValueInfo
        info.ID = dataReader.GetGuid("FACTORYMASTERSPECVALUEID")
        info.Description = dataReader.GetString("FACTORYMASTERSPECVALUEDESCRIPTION")
        Return info
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class