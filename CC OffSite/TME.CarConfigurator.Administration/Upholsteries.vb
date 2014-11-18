Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class Upholsteries
    Inherits BaseObjects.ContextUniqueGuidListBase(Of Upholsteries, Upholstery)

#Region " Shared Factory Methods "

    Public Shared Function GetUpholsteries() As Upholsteries
        Return DataPortal.Fetch(Of Upholsteries)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class Upholstery
    Inherits BaseObjects.TranslateableBusinessBase

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private WithEvents _aliases As Aliases

    Private _interiorColour As InteriorColourInfo
    Private _trim As TrimInfo
    Private _type As UpholsteryTypeInfo

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            value = value.Trim()
            If _code <> value Then
                _code = value
                Me.Aliases.BaseCode = value
                PropertyHasChanged("Code")
            End If
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            value = value.Trim()
            If _name <> value Then
                _name = value
                PropertyHasChanged("Name")
            End If
        End Set
    End Property
    Public Property Type() As UpholsteryTypeInfo
        Get
            Return _type
        End Get
        Set(ByVal value As UpholsteryTypeInfo)
            If value Is Nothing Then value = UpholsteryTypeInfo.Empty
            If value.Equals(_type) Then Return

            _type = value
            PropertyHasChanged("Type")
        End Set
    End Property
    Public ReadOnly Property Aliases() As Aliases
        Get
            Return _aliases
        End Get
    End Property
    Private Sub AliasesListChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _aliases.ListChanged
        Call Me.Aliases.CheckRules()
        Call ValidationRules.CheckRules("Code")
        Me.MarkDirty()
    End Sub

    Public Property InteriorColour() As InteriorColourInfo
        Get
            Return _interiorColour
        End Get
        Set(ByVal value As InteriorColourInfo)
            If value Is Nothing Then value = InteriorColourInfo.Empty
            If value.Equals(_interiorColour) Then Return

            _interiorColour = value
            PropertyHasChanged("InteriorColour")
        End Set
    End Property
    Public Property Trim() As TrimInfo
        Get
            Return _trim
        End Get
        Set(ByVal value As TrimInfo)
            If value Is Nothing Then value = TrimInfo.Empty
            If value.Equals(_trim) Then Return

            _trim = value
            PropertyHasChanged("Trim")
        End Set
    End Property


    Public Function GetInfo() As UpholsteryInfo
        Return UpholsteryInfo.GetUpholsteryInfo(Me)
    End Function
#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 5))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Upholstery.IsCodeUnique, Validation.RuleHandler), "Code")

        ValidationRules.AddRule(DirectCast(AddressOf Upholstery.TypeMandatory, Validation.RuleHandler), "Type")
        ValidationRules.AddRule(DirectCast(AddressOf Upholstery.InteriorColourMandatory, Validation.RuleHandler), "InteriorColour")
        ValidationRules.AddRule(DirectCast(AddressOf Upholstery.TrimMandatory, Validation.RuleHandler), "Trim")
    End Sub

    Private Shared Function IsCodeUnique(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _target As Upholstery = DirectCast(target, Upholstery)

        If _target.Parent Is Nothing Then Return True
        Dim _upholsteries As Upholsteries = DirectCast(_target.Parent, Upholsteries)

        If _target.Code.Length > 0 Then
            For Each _upholstery As Upholstery In _upholsteries
                If String.Compare(_upholstery.Code, _target.Code, True) = 0 AndAlso Not _upholstery.ID.Equals(_target.ID) Then
                    e.Description = String.Format("The code {0} is already in use by another upholstery", _target.Code)
                    Return False
                Else
                    If _upholstery.Aliases.Any(Function(x) String.Compare(x.Code, _target.Code, True) = 0) Then
                        If _upholstery.ID.Equals(_target.ID) Then
                            e.Description = String.Format("The code {0} is already defined by one of the aliases", _target.Code)
                        Else
                            e.Description = String.Format("The code {0} is already in use by an alias of another upholstery", _target.Code)
                        End If
                        Return False
                    End If
                End If
            Next
        End If
        For Each _alias As [Alias] In _target.Aliases
            For Each _upholstery As Upholstery In _upholsteries
                If Not _upholstery.ID.Equals(_target.ID) Then
                    If String.Compare(_upholstery.Code, _alias.Code, True) = 0 Then
                        e.Description = String.Format("The alias code {0} is already in use by another upholstery", _alias.Code)
                        Return False
                    Else
                        If _upholstery.Aliases.Any(Function(x) String.Compare(x.Code, _alias.Code, True) = 0) Then
                            e.Description = String.Format("The alias code {0} is already in use by an alias of another upholstery", _alias.Code)
                            Return False
                        End If
                    End If
                End If
            Next
        Next


        Return True
    End Function

    Private Shared Function TypeMandatory(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim upholstery As Upholstery = DirectCast(target, Upholstery)

        If Not upholstery.Type.IsEmpty() Then Return True

        e.Description = String.Format("The type is mandatory")
        Return False
    End Function
    Private Shared Function InteriorColourMandatory(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim upholstery As Upholstery = DirectCast(target, Upholstery)

        If Not upholstery.InteriorColour.IsEmpty() Then Return True

        e.Description = String.Format("The interior colour is mandatory")
        Return False
    End Function
    Private Shared Function TrimMandatory(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim upholstery As Upholstery = DirectCast(target, Upholstery)

        If Not upholstery.Trim.IsEmpty() Then Return True

        e.Description = String.Format("The trim is mandatory")
        Return False
    End Function


#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Code & " - " & Me.Name
    End Function
    Public Overloads Overrides Function Equals(ByVal value As String) As Boolean
        Return String.Compare(Me.Code, value, StringComparison.InvariantCultureIgnoreCase) = 0 OrElse Me.Aliases.Contains(value)
    End Function

    Public Overloads Function Equals(ByVal interiorColourCode As String, ByVal trimCode As String) As Boolean
        Return InteriorColour.Equals(interiorColourCode) AndAlso Trim.Equals(trimCode)
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not _aliases.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If _aliases.IsDirty Then Return True
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
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _aliases = Administration.Aliases.NewAliases(String.Empty, 6)
        _type = UpholsteryTypeInfo.Empty
        _interiorColour = InteriorColourInfo.Empty
        _trim = TrimInfo.Empty
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        _code = dataReader.GetString("INTERNALCODE").Trim()
        _name = dataReader.GetString("SHORTNAME").Trim()
        _type = UpholsteryTypeInfo.GetUpholsteryTypeInfo(dataReader)
        _interiorColour = InteriorColourInfo.GetInteriorColourInfo(dataReader)
        _trim = TrimInfo.GetTrimInfo(dataReader)

        If _code.IndexOf(";") > 0 Then
            Dim _allCodes() As String = _code.Split(";"c)
            Dim _aliasCodes(_allCodes.GetUpperBound(0) - 1) As String
            _code = _allCodes(0)
            Array.Copy(_allCodes, 1, _aliasCodes, 0, _allCodes.GetUpperBound(0))
            _aliases = Aliases.GetAliases(_code, 6, _aliasCodes)
        Else
            _aliases = Aliases.NewAliases(_code, 6)
        End If

        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        Dim _fullCode As String = Me.Code
        If Me.Aliases.Count > 0 Then _fullCode += ";" & Me.Aliases.ToString

        command.Parameters.AddWithValue("@INTERNALCODE", _fullCode)
        command.Parameters.AddWithValue("@SHORTNAME", Name)
        command.Parameters.AddWithValue("@TYPEID", Type.ID)
        command.Parameters.AddWithValue("@INTERIORCOLOURID", InteriorColour.ID)
        command.Parameters.AddWithValue("@TRIMMINGID", Trim.ID)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region

#Region " Base Object Overrides "

    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.UPHOLSTERY
        End Get
    End Property
#End Region

End Class
<Serializable()> Public NotInheritable Class UpholsteryInfo

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
        Return Me.ID.Equals(Guid.Empty)
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return Me.ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As Upholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As LinkedUpholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationUpholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As UpholsteryInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return String.Compare(Me.Code, obj, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is UpholsteryInfo Then
            Return Me.Equals(DirectCast(obj, UpholsteryInfo))
        ElseIf TypeOf obj Is ModelGenerationUpholstery Then
            Return Me.Equals(DirectCast(obj, ModelGenerationUpholstery))
        ElseIf TypeOf obj Is LinkedUpholstery Then
            Return Me.Equals(DirectCast(obj, LinkedUpholstery))
        ElseIf TypeOf obj Is Upholstery Then
            Return Me.Equals(DirectCast(obj, Upholstery))
        ElseIf TypeOf obj Is String Then
            Return Me.Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is UpholsteryInfo Then
            Return DirectCast(objA, UpholsteryInfo).Equals(objB)
        ElseIf TypeOf objB Is UpholsteryInfo Then
            Return DirectCast(objB, UpholsteryInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetUpholsteryInfo(ByVal dataReader As SafeDataReader) As UpholsteryInfo
        Dim _info As UpholsteryInfo = New UpholsteryInfo
        _info.Fetch(dataReader)
        Return _info
    End Function
    Friend Shared Function GetUpholsteryInfo(ByVal upholstery As Upholstery) As UpholsteryInfo
        Dim _info As UpholsteryInfo = New UpholsteryInfo
        _info.Fetch(upholstery)
        Return _info
    End Function
    Friend Shared Function GetUpholsteryInfo(ByVal upholstery As ModelGenerationUpholstery) As UpholsteryInfo
        Dim _info As UpholsteryInfo = New UpholsteryInfo
        _info.Fetch(upholstery)
        Return _info
    End Function
    Friend Shared Function GetUpholsteryInfo(ByVal upholstery As LinkedUpholstery) As UpholsteryInfo
        Dim _info As UpholsteryInfo = New UpholsteryInfo
        _info.Fetch(upholstery)
        Return _info
    End Function
    Public Shared ReadOnly Property Empty() As UpholsteryInfo
        Get
            Return New UpholsteryInfo
        End Get
    End Property
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _id = .GetGuid("UPHOLSTERYID")
            _code = .GetString("UPHOLSTERYCODE")
            _name = .GetString("UPHOLSTERYNAME")
        End With
    End Sub
    Private Sub Fetch(ByVal upholstery As Upholstery)
        With upholstery
            _id = .ID
            _code = .Code
            _name = .AlternateName
        End With
    End Sub
    Private Sub Fetch(ByVal upholstery As ModelGenerationUpholstery)
        With upholstery
            _id = .ID
            _code = .Code
            _name = .AlternateName
        End With
    End Sub
    Private Sub Fetch(ByVal upholstery As LinkedUpholstery)
        With upholstery
            _id = .ID
            _code = .Code
            _name = .AlternateName
        End With
    End Sub
#End Region

End Class
