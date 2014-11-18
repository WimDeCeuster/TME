Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class CarParts
    Inherits BaseObjects.StronglySortedListBase(Of CarParts, CarPart)

#Region " Shared Factory Methods "
    Public Shared Function GetCarParts() As CarParts
        Return DataPortal.Fetch(Of CarParts)(New Criteria)
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MyBase.AllowEdit = MyContext.GetContext().IsGlobal()
        MyBase.AllowNew = MyBase.AllowEdit
        MyBase.AllowRemove = MyBase.AllowEdit
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class CarPart
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of CarPart)
    Implements BaseObjects.ISortedIndex
    Implements BaseObjects.ISortedIndexSetter

#Region " Business Properties & Methods "

    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _index As Integer

    <XmlInfo(XmlNodeType.Attribute)> Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            If _code <> Value Then
                _code = Value
                PropertyHasChanged("Code")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            If _name <> Value Then
                _name = Value
                PropertyHasChanged("Name")
            End If
        End Set
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Index() As Integer Implements BaseObjects.ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements BaseObjects.ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not CanWriteProperty("Index") Then Exit Property
            If Not _index.Equals(Value) Then
                _index = Value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property

#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Name")
    End Sub
#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Overrides Function Equals(ByVal value As String) As Boolean
        Return String.Compare(Me.Code, value, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MyBase.AllowEdit = MyContext.GetContext().IsGlobal()
        MyBase.AllowNew = MyBase.AllowEdit
        MyBase.AllowRemove = MyBase.AllowEdit
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        'Load object data	into variables
        With dataReader
            _code = .GetString("CODE")
            _name = .GetString("NAME")
            _index = .GetInt16("SORTORDER")
        End With
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@CODE", Me.Code)
        command.Parameters.AddWithValue("@NAME", Me.Name)
        command.Parameters.AddWithValue("@SORTORDER", Me.Index)
    End Sub

#End Region

End Class