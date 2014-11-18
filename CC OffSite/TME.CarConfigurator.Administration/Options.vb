Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums
Imports TME.BusinessObjects.Validation

<Serializable()> Public NotInheritable Class Options
    Inherits ComponentModel.BindingList(Of [Option])

#Region " Business Properties & Methods "

    <NotUndoable()> Private WithEvents _equipment As EquipmentItems

    Default Public Overloads ReadOnly Property Item(ByVal id As Guid) As [Option]
        Get
            For Each optionItem As [Option] In Me
                If optionItem.Equals(id) Then
                    Return optionItem
                End If
            Next
            Return Nothing
        End Get
    End Property
    Default Public Overloads ReadOnly Property Item(ByVal code As String) As [Option]
        Get
            For Each optionItem As [Option] In Me
                If optionItem.Equals(code) Then
                    Return optionItem
                End If
            Next
            Return Nothing
        End Get
    End Property

    Public Overloads Function Contains(ByVal id As Guid) As Boolean
        Return Any(Function(x) x.Equals(id))
    End Function
    Public Overloads Function Contains(ByVal code As String) As Boolean
        Return Any(Function(x) x.Equals(code))
    End Function

    Public Overloads Sub Remove(ByVal obj As [Option])
        _equipment.Remove(obj)
    End Sub
    Public Overloads Sub Remove(ByVal id As Guid)
        _equipment.Remove(id)
    End Sub
    Public Overloads Sub Remove(ByVal code As String)
        _equipment.Remove(code)
    End Sub
    Public Overloads Sub Remove(ByVal index As Integer)
        _equipment.Remove(Me(index).ID)
    End Sub

    Private Sub EquipmentListChanged(ByVal sender As Object, ByVal e As ComponentModel.ListChangedEventArgs) Handles _equipment.ListChanged
        If Not (e.ListChangedType = ComponentModel.ListChangedType.ItemAdded OrElse e.ListChangedType = ComponentModel.ListChangedType.ItemDeleted) Then Exit Sub
        Fetch()
    End Sub

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetOptions(ByVal equipment As EquipmentItems) As Options
        Dim options As Options = New Options(equipment)
        options.Fetch()
        Return options
    End Function

#End Region

#Region " Constructors "
    Private Sub New(ByVal equipment As EquipmentItems)
        'Prevent direct creation
        _equipment = equipment
    End Sub
#End Region

#Region " Data Access "
    Private Sub Fetch()
        Clear()
        For Each equipmentItem In _equipment.OfType(Of [Option])().OrderBy(Function(x) x.SortPath)
            Add(equipmentItem)
        Next
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class [Option]
    Inherits EquipmentItem
    Implements IMasterPathObject

#Region " Business Properties & Methods "

    Private _code As String = String.Empty
    Private _suffixFittings As SuffixFittings
    Private _masterPath As String
    Private _parentOption As [Option]
    Private _parentOptionID As Guid


    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            If _code <> value Then
                _code = value
                If Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) Then
                    _localCode = value
                End If
                PropertyHasChanged("Code")
            End If
        End Set
    End Property
    Private Sub OptionPropertyChanged(ByVal sender As Object, ByVal e As ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If e.PropertyName.Equals("LocalCode", StringComparison.InvariantCultureIgnoreCase) Then
            If Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) Then
                _code = LocalCode
            End If
            Return
        End If
    End Sub





    Public ReadOnly Property SuffixFittings() As SuffixFittings
        Get
            If _suffixFittings Is Nothing Then _suffixFittings = SuffixFittings.NewSuffixFittings(Me)
            Return _suffixFittings
        End Get
    End Property

    Public Overloads Overrides Function CanBeFittedOn(ByVal partialConfiguration As PartialCarSpecification) As Boolean
        If SuffixFittings.ContainsMatch(partialConfiguration) Then Return True
        Return MyBase.CanBeFittedOn(partialConfiguration)
    End Function
    Public Overloads Function GetInfo() As OptionInfo
        Return OptionInfo.GetOptionInfo(Me)
    End Function

    Public Property MasterPath() As String Implements IMasterPathObject.MasterPath
        Get
            Return _masterPath
        End Get
        Set(value As String)
            If Not _masterPath.Equals(value, StringComparison.InvariantCultureIgnoreCase) Then
                _masterPath = value
                PropertyHasChanged("MasterPath")
            End If
        End Set
    End Property

    Private ReadOnly Property RefMasterPath() As String Implements IMasterPathObjectReference.MasterPath
        Get
            Return MasterPath
        End Get
    End Property

    Private ReadOnly Property ParentOptionID() As Guid
        Get
            Return _parentOptionID
        End Get
    End Property

    Public Property ParentOption() As [Option]
        Get
            If _parentOption Is Nothing Then _parentOption = DirectCast(Parent(ParentOptionID), [Option])
            Return _parentOption
        End Get
        Private Set(ByVal value As [Option])
            If Not Equals(_parentOption, value) Then
                _parentOptionID = If(value Is Nothing, Guid.Empty, value.ID)
                _parentOption = value
                PropertyHasChanged("ParentOption")
            End If
        End Set
    End Property
    Public Function GetRootOption() As [Option]
        Return GetRootOption(Me)
    End Function
    Private Shared Function GetRootOption(ByVal [option] As [Option]) As [Option]
        Dim rootOption As [Option]
        If [option].HasParentOption Then
            rootOption = [option].ParentOption
            While rootOption.HasParentOption
                rootOption = rootOption.ParentOption
            End While
        Else
            rootOption = [option]
        End If
        Return rootOption
    End Function

    Public ReadOnly Property ChildOptions() As IEnumerable(Of [Option])
        Get
            If Parent Is Nothing Then Return New List(Of [Option])
            Return Parent.Options.Where(Function(x) x.ParentOption IsNot Nothing AndAlso x.ParentOptionID.Equals(ID))
        End Get
    End Property

    Public Sub ClearParent()
        ParentOption = Nothing
    End Sub

    Public Sub ChangeParentOption(ByVal parentItem As [Option])
        ParentOption = parentItem
        Group = parentItem.Group
        Category = parentItem.Category
    End Sub

    Public ReadOnly Property HasParentOption() As Boolean
        Get
            Return Not _parentOptionID.Equals(Guid.Empty)
        End Get
    End Property

    Public ReadOnly Property HasChildOptions() As Boolean
        Get
            Return ChildOptions.Any()
        End Get
    End Property

    Public Overrides Property Approved() As Boolean
        Get
            Return MyBase.Approved
        End Get
        Set(value As Boolean)
            MyBase.Approved = value
            If value Then MarkParentAsApproved()
            If Not value Then MarkParentAsDisapproved()
        End Set
    End Property
    Private Sub MarkParentAsApproved()
        If HasParentOption() Then ParentOption.Approved = True
    End Sub
    Private Sub MarkParentAsDisapproved()
        If HasParentOption() AndAlso Not ParentOption.ChildOptions.Any(Function(x) x.Approved) Then ParentOption.Approved = False
    End Sub

    Public Overrides Property Group() As EquipmentGroupInfo
        Get
            Return MyBase.Group
        End Get
        Set(value As EquipmentGroupInfo)
            MyBase.Group = value
            MoveChildOptionsToTheSameGroup()
        End Set
    End Property
    Private Sub MoveChildOptionsToTheSameGroup()
        For Each childOption In ChildOptions
            childOption.Group = Group
        Next
    End Sub

    Public Overrides Property Category() As EquipmentCategoryInfo
        Get
            Return MyBase.Category
        End Get
        Set(value As EquipmentCategoryInfo)
            MyBase.Category = value
            MoveChildOptionsToTheSameCategory()
        End Set
    End Property
    Private Sub MoveChildOptionsToTheSameCategory()
        For Each childOption In ChildOptions
            childOption.Category = Category
        Next
    End Sub


    Public Overrides ReadOnly Property SortPath() As String
        Get
            If HasParentOption() Then Return String.Format("{0}{1}", ParentOption.SortPath(), Name)
            Return String.Format("{0}/", Name)
            'a slash (/) needs to be added to the name if we want to use this seperator
            'needed otherwise sorting on special characters ie. colon (:) is done wrong
        End Get
    End Property


#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        MyBase.AddBusinessRules()

        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Object.Required, RuleHandler), "Group")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Object.Required, RuleHandler), "Category")

        ValidationRules.AddRule(DirectCast(AddressOf GroupShouldBeTheSameAsParent, RuleHandler), "Group")
        ValidationRules.AddRule(DirectCast(AddressOf CategoryShouldBeTheSameAsParent, RuleHandler), "Category")

        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("Code", 50))

        ValidationRules.AddRule(DirectCast(AddressOf GroupIsNotEmpty, Validation.RuleHandler), "Group")
        ValidationRules.AddRule(DirectCast(AddressOf CategoryIsNotEmpty, Validation.RuleHandler), "Category")
        ValidationRules.AddRule(DirectCast(AddressOf AtLeastOneChildShouldBeApprovedWhenApprovingParent, RuleHandler), "Approved")
        ValidationRules.AddRule(DirectCast(AddressOf AllChildrenShouldBeDisapprovedWhenDisapprovingParent, RuleHandler), "Approved")

    End Sub

    Private Shared Function GroupIsNotEmpty(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim objOption As [Option] = DirectCast(target, [Option])
        If objOption.Group Is Nothing OrElse objOption.Group.IsEmpty() Then
            e.Description = "The Group is required."
            Return False
        End If
        Return True
    End Function
    Private Shared Function CategoryShouldBeTheSameAsParent(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim obj = DirectCast(target, [Option])

        If Not obj.HasParentOption Then Return True
        If obj.ParentOption.Category.Equals(obj.Category) Then Return True

        e.Description = "The option should have the same category as the parent option"
        Return False
    End Function

    Private Shared Function CategoryIsNotEmpty(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim objOption As [Option] = DirectCast(target, [Option])
        If objOption.Category Is Nothing OrElse objOption.Category.IsEmpty() Then
            e.Description = "The Category is required."
            Return False
        End If
        Return True
    End Function
    Private Shared Function GroupShouldBeTheSameAsParent(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim obj = DirectCast(target, [Option])

        If Not obj.HasParentOption Then Return True
        If obj.ParentOption.Group.Equals(obj.Group) Then Return True

        e.Description = "The option should have the same group as the parent option"
        Return False
    End Function

    Private Shared Function AtLeastOneChildShouldBeApprovedWhenApprovingParent(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim obj = DirectCast(target, [Option])

        If Not obj.ChildOptions.Any() Then Return True
        If Not obj.Approved Then Return True

        e.Description = "An option can not be approved if at least one of its children isn't approved."

        Return obj.Approved AndAlso obj.ChildOptions.Any(Function(x) x.Approved)
    End Function

    Private Shared Function AllChildrenShouldBeDisapprovedWhenDisapprovingParent(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim obj = DirectCast(target, [Option])
        If obj.Approved Then Return True

        e.Description = "An option can not be disapproved if one of its children is still approved."
        Return obj.ChildOptions.All(Function(x) Not x.Approved)
    End Function
#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_suffixFittings Is Nothing) AndAlso Not _suffixFittings.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_suffixFittings Is Nothing) AndAlso _suffixFittings.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region

#Region " System.Object Overrides "
    Public Overloads Function Equals(ByVal obj As [Option]) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID) OrElse Code.Equals(obj.Code, StringComparison.InvariantCultureIgnoreCase)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        If obj.Length = 0 Then Return False
        If (";" + LocalCode.ToLowerInvariant() + ";").IndexOf(";" & obj.ToLowerInvariant() & ";", StringComparison.Ordinal) > -1 Then Return True
        If (";" + Code.ToLowerInvariant() + ";").IndexOf(";" & obj.ToLowerInvariant() & ";", StringComparison.Ordinal) > -1 Then Return True
        Return False
    End Function
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewOption() As [Option]
        Dim [option] As [Option] = New [Option]()
        [option].Create()
        [option].MarkAsChild()
        Return [option]
    End Function
    Friend Shared Function GetOption(ByVal dataReader As SafeDataReader) As [Option]
        Dim [option] As [Option] = New [Option]
        [option].Fetch(dataReader)
        [option].MarkAsChild()
        Return [option]
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MyBase.New()
        Type = EquipmentType.Option
        CanHaveLocalCode = True
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _masterPath = String.Empty
        If String.Compare(Owner, Environment.GlobalCountryCode, True) = 0 Then
            Activated = False
            Code = ID.ToString().Substring(0, 13).ToUpper()
        Else
            Activated = True
        End If
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _code = dataReader.GetString("INTERNALCODE")
        _masterPath = dataReader.GetString("MASTERPATH")
        _parentOptionID = dataReader.GetGuid("PARENTEQUIPMENTID")
        Type = EquipmentType.Option
        MyBase.FetchFields(dataReader)
        AllowRemove = (String.Compare(MyContext.GetContext().CountryCode, Environment.GlobalCountryCode) = 0) OrElse (String.Compare(MyContext.GetContext().CountryCode, Owner, True) = 0)
        AllowEdit = AllowRemove
        AllowNew = AllowRemove
    End Sub

    Protected Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        MyBase.AddCommandFields(command)
        With command
            .Parameters.AddWithValue("@INTERNALCODE", Code)
            .Parameters.AddWithValue("@LOCALCODE", LocalCode)
            .Parameters.AddWithValue("@MASTERPATH", MasterPath)
            .Parameters.AddWithValue("@PARENTEQUIPMENTID", If(_parentOptionID.Equals(Guid.Empty), DBNull.Value, DirectCast(_parentOptionID, Object)))
        End With
    End Sub
    Friend Sub FetchSuffixFitting(ByVal dataReader As SafeDataReader)
        SuffixFittings.FetchRow(dataReader)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not _suffixFittings Is Nothing Then _suffixFittings.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub
#End Region

#Region " Base Object Overrides"
    Protected Friend Overrides Function GetBaseCode() As String
        Return Code
    End Function
#End Region

End Class

<Serializable(), XmlInfo("option")> Public NotInheritable Class OptionInfo

#Region " Business Properties & Methods "
    Private _id As Guid
    Private _name As String

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

    Public Function IsEmpty() As Boolean
        Return ID.Equals(Guid.Empty)
    End Function


#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Function Equals(ByVal obj As OptionInfo) As Boolean
        Return obj.ID.Equals(ID)
    End Function
    Public Overloads Function Equals(ByVal obj As CarOption) As Boolean
        Return obj.ID.Equals(ID)
    End Function
    Public Overloads Function Equals(ByVal obj As [Option]) As Boolean
        Return obj.ID.Equals(ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return obj.Equals(ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is OptionInfo Then
            Return Equals(DirectCast(obj, OptionInfo))
        ElseIf TypeOf obj Is CarOption Then
            Return Equals(DirectCast(obj, CarOption))
        ElseIf TypeOf obj Is [Option] Then
            Return Equals(DirectCast(obj, [Option]))
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

        If TypeOf objA Is OptionInfo Then
            Return DirectCast(objA, OptionInfo).Equals(objB)
        ElseIf TypeOf objB Is OptionInfo Then
            Return DirectCast(objB, OptionInfo).Equals(objA)
        Else
            Return False
        End If
    End Function
#End Region

#Region " Shared Factory Methods "
    Public Shared Function GetOptionInfo(ByVal [option] As [Option]) As OptionInfo
        Dim info As OptionInfo = New OptionInfo
        info._id = [option].ID
        info._name = [option].Name
        Return info
    End Function
    Friend Shared Function GetOptionInfo(ByVal dataReader As SafeDataReader) As OptionInfo
        Dim info As OptionInfo = New OptionInfo
        info._id = dataReader.GetGuid("EQUIPMENTID")
        info._name = dataReader.GetString("EQUIPMENTNAME")
        Return info
    End Function

    Public Shared ReadOnly Property Empty() As OptionInfo
        Get
            Return New OptionInfo
        End Get
    End Property

#End Region


End Class