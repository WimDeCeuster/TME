Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class ExteriorColours
    Inherits ContextUniqueGuidListBase(Of ExteriorColours, ExteriorColour)

#Region " Shared Factory Methods "

    Public Shared Function GetExteriorColours() As ExteriorColours
        Return DataPortal.Fetch(Of ExteriorColours)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class ExteriorColour
    Inherits TranslateableBusinessBase
    Implements ILinkedAssets

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _type As ExteriorColourTypeInfo
    Private _assets As LinkedAssets

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            Value = Value.Trim()
            If _code <> Value Then
                _code = Value
                PropertyHasChanged("Code")
            End If
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            Value = Value.Trim()
            If _name <> Value Then
                _name = Value
                PropertyHasChanged("Name")
            End If
        End Set
    End Property
    Public Property Type() As ExteriorColourTypeInfo
        Get
            Return _type
        End Get
        Set(ByVal value As ExteriorColourTypeInfo)
            If _type Is Nothing OrElse Not _type.Equals(Value) Then
                _type = Value
                PropertyHasChanged("Type")
            End If
        End Set
    End Property
    Public ReadOnly Property Assets() As LinkedAssets Implements ILinkedAssets.Assets
        Get
            If _assets Is Nothing Then
                If IsNew Then
                    _assets = LinkedAssets.NewLinkedAssets(ID)
                Else
                    _assets = LinkedAssets.GetLinkedAssets(ID)
                End If
            End If
            Return _assets
        End Get
    End Property

    Public Function GetInfo() As ExteriorColourInfo
        Return ExteriorColourInfo.GetExteriorColourInfo(Me)
    End Function

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 3))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "Type")
    End Sub

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return String.Format("{0} - {1}", Code, Name)
    End Function
    Public Overloads Overrides Function Equals(ByVal value As String) As Boolean
        Return String.Compare(Code, value, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function

#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_assets Is Nothing) AndAlso Not _assets.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_assets Is Nothing) AndAlso _assets.IsDirty Then Return True
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

    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        With dataReader
            _code = .GetString("INTERNALCODE").Trim()
            _name = .GetString("SHORTNAME").Trim()
            _type = ExteriorColourTypeInfo.GetExteriorColourTypeInfo(dataReader)
        End With
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@INTERNALCODE", Code)
        command.Parameters.AddWithValue("@SHORTNAME", Name)
        command.Parameters.AddWithValue("@TYPEID", Type.ID)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Not _assets Is Nothing Then _assets.Update(transaction)
    End Sub

#End Region

#Region " Base Object Overrides "

    Protected Friend Overrides Function GetBaseName() As String
        Return Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.EXTERIORCOLOUR
        End Get
    End Property
#End Region

End Class
<Serializable()> Public NotInheritable Class ExteriorColourInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _code As String = String.Empty
    Private _name As String = String.Empty

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
        Get
            Return _id
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

    Public Function IsEmpty() As Boolean
        Return ID.Equals(Guid.Empty)
    End Function

#End Region

#Region " System.Object Overrides "

  Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As ExteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As LinkedExteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ExteriorColourInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return String.Compare(Code, obj, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ExteriorColourInfo Then
            Return Equals(DirectCast(obj, ExteriorColourInfo))
        ElseIf TypeOf obj Is LinkedExteriorColour Then
            Return Equals(DirectCast(obj, LinkedExteriorColour))
        ElseIf TypeOf obj Is ExteriorColour Then
            Return Equals(DirectCast(obj, ExteriorColour))
        ElseIf TypeOf obj Is String Then
            Return Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is ExteriorColourInfo Then
            Return DirectCast(objA, ExteriorColourInfo).Equals(objB)
        ElseIf TypeOf objB Is ExteriorColourInfo Then
            Return DirectCast(objB, ExteriorColourInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetExteriorColourInfo(ByVal dataReader As SafeDataReader, Optional prefix As String = "") As ExteriorColourInfo
        Dim info As ExteriorColourInfo = New ExteriorColourInfo
        info.Fetch(dataReader, prefix)
        Return info
    End Function
    Friend Shared Function GetExteriorColourInfo(ByVal exteriorColour As ExteriorColour) As ExteriorColourInfo
        Dim info As ExteriorColourInfo = New ExteriorColourInfo
        info.Fetch(exteriorColour)
        Return info
    End Function
    Friend Shared Function GetExteriorColourInfo(ByVal exteriorColour As ModelGenerationExteriorColour) As ExteriorColourInfo
        Dim info As ExteriorColourInfo = New ExteriorColourInfo
        info.Fetch(exteriorColour)
        Return info
    End Function
    Friend Shared Function GetExteriorColourInfo(ByVal exteriorColour As LinkedExteriorColour) As ExteriorColourInfo
        Dim info As ExteriorColourInfo = New ExteriorColourInfo
        info.Fetch(exteriorColour)
        Return info
    End Function
    Public Shared Function GetExteriorColourInfo(ByVal exteriorColourApplicability As ExteriorColourApplicability) As ExteriorColourInfo
        Dim info As ExteriorColourInfo = New ExteriorColourInfo
        info.Fetch(exteriorColourApplicability)
        Return info
    End Function



    Public Shared ReadOnly Property Empty() As ExteriorColourInfo
        Get
            Return New ExteriorColourInfo
        End Get
    End Property
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Private Sub Fetch(ByVal dataReader As SafeDataReader, Optional prefix As String = "")
        With dataReader
            _id = If(String.IsNullOrEmpty(prefix), .GetGuid("EXTERIORCOLOURID"), .GetGuid(prefix + "COLOURID"))
            _code = If(String.IsNullOrEmpty(prefix), .GetString("EXTERIORCOLOURCODE"), .GetString(prefix + "COLOURCODE"))
            _name = If(String.IsNullOrEmpty(prefix), .GetString("EXTERIORCOLOURNAME"), .GetString(prefix + "COLOURNAME"))
        End With
    End Sub
    Private Sub Fetch(ByVal exteriorColour As ExteriorColour)
        With exteriorColour
            _id = .ID
            _code = .Code
            _name = .AlternateName
        End With
    End Sub
    Private Sub Fetch(ByVal exteriorColour As ModelGenerationExteriorColour)
        With exteriorColour
            _id = .ID
            _code = .Code
            _name = .AlternateName
        End With
    End Sub
    Private Sub Fetch(ByVal exteriorColour As LinkedExteriorColour)
        With exteriorColour
            _id = .ID
            _code = .Code
            _name = .AlternateName
        End With
    End Sub
    Private Sub Fetch(ByVal exteriorColourApplicability As ExteriorColourApplicability)
        With exteriorColourApplicability
            _id = .ID
            _code = .Code
            _name = .Name
        End With
    End Sub
#End Region

 
End Class