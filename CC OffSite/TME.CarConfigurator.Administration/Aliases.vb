Imports System.Collections.Generic
Imports TME.BusinessObjects.Validation
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class Aliases
    Inherits BusinessListBase(Of Aliases, [Alias])

#Region " Business Properties & Methods "

    Private _codeLength As Integer = 50
    Private _baseCode As String = String.Empty

    Friend Property BaseCode() As String
        Get
            Return _baseCode
        End Get
        Set(ByVal value As String)
            If Not String.Compare(value, _baseCode, StringComparison.InvariantCultureIgnoreCase) = 0 Then
                _baseCode = value
                CheckRules()
            End If
        End Set
    End Property
    Private ReadOnly Property MaxLength() As Integer
        Get
            Return _codeLength
        End Get
    End Property

    Friend Sub CheckRules()
        For Each _alias As [Alias] In Me
            _alias.CheckRules()
        Next
    End Sub

    Public Shadows Function Add(ByVal code As String) As [Alias]
        Dim _alias As [Alias] = [Alias].NewAlias(code, Me.MaxLength)
        MyBase.Add(_alias)
        _alias.CheckRules()
        Return _alias
    End Function

#End Region

#Region " System.Object Overrides"
    Public Overloads Overrides Function ToString() As String
        ' Return text for converting object to text representation
        Dim _buffer As String = String.Empty
        For Each _alias As [Alias] In Me
            _buffer += _alias.Code & ";"
        Next
        If Not _buffer.Equals(String.Empty) Then
            _buffer = _buffer.Substring(0, _buffer.Length - 1)
        End If
        Return _buffer
    End Function
#End Region


#Region " Shared Factory Methods "
    Friend Shared Function NewAliases(ByVal baseCode As String, ByVal maxLength As Integer, Optional ByVal [readOnly] As Boolean = False) As Aliases
        Dim _aliases As Aliases = New Aliases
        _aliases._baseCode = baseCode
        _aliases._codeLength = maxLength
        If [readOnly] Then
            _aliases.AllowEdit = False
            _aliases.AllowNew = False
            _aliases.AllowRemove = False
        End If
        Return _aliases
    End Function
    Friend Shared Function GetAliases(ByVal baseCode As String, ByVal maxLength As Integer, ByVal aliases() As String, Optional ByVal [readOnly] As Boolean = False) As Aliases
        Dim _aliases As Aliases = New Aliases
        _aliases._baseCode = baseCode
        _aliases._codeLength = maxLength
        _aliases.Fetch(aliases, [readOnly])
        If [readOnly] Then
            _aliases.AllowEdit = False
            _aliases.AllowNew = False
            _aliases.AllowRemove = False
        End If
        Return _aliases
    End Function
    Friend Shared Function GetAliases(ByVal aliases As Aliases, Optional ByVal [readOnly] As Boolean = False) As Aliases
        Dim _aliases As Aliases = New Aliases
        _aliases._baseCode = aliases.BaseCode
        _aliases._codeLength = aliases.MaxLength
        _aliases.Fetch(aliases, [readOnly])
        If [readOnly] Then
            _aliases.AllowEdit = False
            _aliases.AllowNew = False
            _aliases.AllowRemove = False
        End If
        Return _aliases
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
        'Tell the framework we'are a child collection
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "

    Private Sub Fetch(ByVal aliases() As String, ByVal [readOnly] As Boolean)
        'Create a child object for each row in the data source 
        For Each _alias As String In aliases
            MyBase.Add([Alias].GetAlias(_alias.Trim, Me.MaxLength, [readOnly]))
        Next
    End Sub
    Private Sub Fetch(ByVal aliases As IEnumerable(Of [Alias]), ByVal [readOnly] As Boolean)
        'Create a child object for each row in the data source 
        For Each _alias As [Alias] In aliases
            MyBase.Add([Alias].GetAlias(_alias.Code, Me.MaxLength, [readOnly]))
        Next
    End Sub

#End Region

End Class
<Serializable()> Public NotInheritable Class [Alias]
    Inherits BusinessBase(Of [Alias])

#Region " Business Properties & Methods "
    ' Declare variables to contain object state
    ' Declare variables for any child collections

    Private ReadOnly _id As Guid = Guid.NewGuid()
    Private _code As String = String.Empty
    Private _codeLength As Integer = 50

    ' Implement properties and methods for interaction of the UI,
    ' or any other client code, with the object
    Private ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    Private ReadOnly Property CodeLength() As Integer
        Get
            Return _codeLength
        End Get
    End Property

    Protected Overrides Function GetIdValue() As Object
        Return Me.ID
    End Function

    Friend Sub CheckRules()
        Me.ValidationRules.CheckRules()
    End Sub
#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", Me.CodeLength))
        ValidationRules.AddRule(DirectCast(AddressOf [Alias].IsValueUnique, RuleHandler), New Validation.RuleArgs("Code"))
    End Sub

    Private Shared Function IsValueUnique(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _success As Boolean = True
        Dim _target As [Alias] = DirectCast(target, [Alias])

        If Not _target.Parent Is Nothing Then
            Dim _parent As Aliases = DirectCast(_target.Parent, Aliases)
            Select Case e.PropertyName
                Case "Code"
                    If Not _target.Code.Length = 0 Then
                        If String.Compare(_parent.BaseCode, _target.Code, True) = 0 Then
                            _success = False
                            e.Description = "This code (" & _target.Code & ") is already defined as base code"
                        Else
                            If _parent.Any(Function(x) String.Compare(x.Code, _target.Code, True) = 0 AndAlso Not x.ID.Equals(_target.ID)) Then
                                _success = False
                                e.Description = "This code (" & _target.Code & ") is already in use by another alias"
                            End If
                        End If
                    End If
            End Select
        End If

        If _success Then
            e.Description = ""
            Return True
        Else
            Return False
        End If

    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Code
    End Function
    Public Overloads Function Equals(ByVal value As Guid) As Boolean
        Return Me.ID.Equals(value)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is [Alias] Then
            Return Me.Equals(DirectCast(obj, [Alias]))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        ElseIf TypeOf obj Is String Then
            Return Me.Equals(DirectCast(obj, String))
        Else
            Return False
        End If
    End Function
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewAlias(ByVal code As String, ByVal maxLength As Integer) As [Alias]
        Dim _alias As [Alias] = New [Alias]
        _alias._codeLength = maxLength
        _alias._code = code
        Return _alias
    End Function
    Friend Shared Function GetAlias(ByVal code As String, ByVal maxLength As Integer, ByVal [readOnly] As Boolean) As [Alias]
        Dim _alias As [Alias] = New [Alias]
        _alias._codeLength = maxLength
        _alias._code = code
        _alias.ValidationRules.UnBreakRules()
        _alias.MarkOld()
        If [readOnly] Then
            _alias.AllowEdit = False
            _alias.AllowNew = False
            _alias.AllowRemove = False
        End If
        Return _alias
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
        AddBusinessRules()
    End Sub
#End Region

End Class