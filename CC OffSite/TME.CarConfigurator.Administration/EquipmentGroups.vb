Imports TME.CarConfigurator.Administration.Security
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class EquipmentGroups
    Inherits StronglySortedListBase(Of EquipmentGroups, EquipmentGroup)

#Region " Business Properties & Methods "
    <NotUndoable()> Private _referenceMap As Generic.Dictionary(Of Guid, EquipmentGroup)


    Public Function FindEquipment(ByVal obj As ModelGenerationGradeEquipmentItem) As EquipmentItem
        Dim group = ReferenceMap.Item(obj.Group.ID)
        Dim equipmentItem = group.Equipment(obj.ID)

        If equipmentItem Is Nothing Then Throw New ApplicationException("Could not find the equipmentitem '" & obj.Name & "'?")
        Return equipmentItem
    End Function

    Public Function FindEquipment(ByVal id As Guid) As EquipmentItem
        If id.Equals(Guid.Empty) Then Return Nothing

        Return (
            From group In Me.ReferenceMap.Values
                Where group.Equipment.Contains(id)
            Select group.Equipment(id)
            ).FirstOrDefault()

    End Function
    Public Function FindEquipment(ByVal code As String) As EquipmentItem
        If code.Equals(String.Empty) Then Return Nothing

        Return (
            From group In Me.ReferenceMap.Values
                Where group.Equipment.Contains(code)
            Select group.Equipment(code)
        ).FirstOrDefault()
    End Function

    Public Function FindAccessory(ByVal id As Guid) As Accessory
        Dim _item As EquipmentItem = FindEquipment(id)
        If _item Is Nothing OrElse Not TypeOf _item Is Accessory Then Return Nothing
        Return DirectCast(_item, Accessory)
    End Function
    Public Function FindAccessory(ByVal code As String) As Accessory
        If code.Equals(String.Empty) Then Return Nothing

        Dim firstGroupThatContainsTheAccessory = ReferenceMap.Values.FirstOrDefault(Function(group) group.Equipment.Accessories.Any(Function(acc) acc.Equals(code)))

        If firstGroupThatContainsTheAccessory Is Nothing Then Return Nothing

        Return firstGroupThatContainsTheAccessory.Equipment.Accessories.First(Function(acc) acc.Equals(code))
    End Function
    Public Function FindOption(ByVal id As Guid) As [Option]
        Dim _item As EquipmentItem = FindEquipment(id)
        If _item Is Nothing Then Return Nothing
        If Not TypeOf _item Is [Option] Then Return Nothing
        Return DirectCast(_item, [Option])
    End Function
    Public Function FindOption(ByVal code As String) As [Option]
        If code.Equals(String.Empty) Then Return Nothing

        Return (
            From group In Me.ReferenceMap.Values
                Where group.Equipment.Options.Contains(code)
            Select group.Equipment.Options(code)
        ).FirstOrDefault()
    End Function

    Public Function Find(ByVal id As Guid) As EquipmentGroup
        Return If(ReferenceMap.ContainsKey(id), ReferenceMap.Item(id), Nothing)
    End Function

    Public ReadOnly Property ParentGroup() As EquipmentGroup
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, EquipmentGroup)
        End Get
    End Property
    Private ReadOnly Property ReferenceMap() As Generic.Dictionary(Of Guid, EquipmentGroup)
        Get
            If _referenceMap Is Nothing Then
                _referenceMap = Me.ParentGroup.ParentGroup.Groups.ReferenceMap()
            End If
            Return _referenceMap
        End Get
    End Property

    Private Sub EquipmentGroupsListChanged(ByVal sender As Object, ByVal e As ComponentModel.ListChangedEventArgs) Handles Me.ListChanged
        If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded Then
            Dim _group As EquipmentGroup = Me(e.NewIndex)
            Me.ReferenceMap.Add(_group.ID, _group)
        End If
    End Sub
    Private Sub EquipmentGroupsRemovingItem(ByVal sender As Object, ByVal e As Core.RemovingItemEventArgs) Handles Me.RemovingItem
        Me.ReferenceMap.Remove(DirectCast(e.RemovingItem, EquipmentGroup).ID)
    End Sub
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetEquipmentGroups(ByVal lazyLoad As Boolean) As EquipmentGroups
        Return DirectCast(DataPortal.Fetch(New CustomCriteria(lazyLoad)), EquipmentGroups)
    End Function
    Friend Shared Function NewEquipmentGroups(ByVal parent As EquipmentGroup) As EquipmentGroups
        Dim _groups As EquipmentGroups = New EquipmentGroups()
        _groups.SetParent(parent)
        _groups.MarkAsChild()
        Return _groups
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _lazyLoad As Boolean
        Public Sub New(ByVal lazyLoad As Boolean)
            _lazyLoad = lazyLoad
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@LAZYLOAD", _lazyLoad)
        End Sub
    End Class
#End Region

#Region " Data Access "
    Protected Overrides Sub Fetch(ByVal dataReader As Common.Database.SafeDataReader)
        RaiseListChangedEvents = True

        Dim rootGroup As EquipmentGroup = EquipmentGroup.DummyRootGroup(Me)
        SetParent(rootGroup)

        _referenceMap = New Generic.Dictionary(Of Guid, EquipmentGroup)
        _referenceMap.Add(rootGroup.ID, rootGroup)

        While dataReader.Read()
            Dim group As EquipmentGroup = GetObject(dataReader)
            _referenceMap(dataReader.GetGuid("PARENTGROUPID")).Groups.Add(group)
        End While
    End Sub

    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        FetchRules(dataReader)
        If dataReader.NextResult() Then FetchRules(dataReader)
        If dataReader.NextResult() Then FetchEquipment(dataReader)
        If dataReader.NextResult() Then FetchEquipmentFittings(dataReader)
        If dataReader.NextResult() Then FetchEquipmentIncompatibilities(dataReader)
        If dataReader.NextResult() Then FetchSuffixFittings(dataReader)
    End Sub
    Private Sub FetchRules(ByVal dataReader As SafeDataReader)
        With dataReader
            Dim _id As Guid = Guid.Empty
            Dim _group As EquipmentGroup = Nothing

            While .Read()
                If Not _id.Equals(.GetGuid("OWNERID")) Then
                    _id = .GetGuid("OWNERID")
                    _group = Me.Find(_id)
                End If
                If Not _group Is Nothing Then
                    _group.Rules.FetchRule(dataReader)
                End If
            End While

        End With
    End Sub

    Private Sub FetchEquipment(ByVal dataReader As SafeDataReader)
        PrepareForEagerLoadOfEquipment()

        With dataReader
            Dim _id As Guid = Guid.Empty
            Dim _group As EquipmentGroup = Nothing
            
      
            While .Read()
                If Not _id.Equals(.GetGuid("EQUIPMENTGROUPID")) Then
                    _id = .GetGuid("EQUIPMENTGROUPID")
                    _group = Find(_id)
                End If
                If _group IsNot Nothing Then
                    Call _group.Equipment.Add(dataReader)
                End If
            End While

        End With
    End Sub
    Private Sub FetchEquipmentFittings(ByVal dataReader As SafeDataReader)
        PrepareForEagerLoadOfApplicabilities()

        With dataReader
            Dim gGroupID As Guid = Guid.Empty
            Dim oGroup As EquipmentGroup = Nothing

            Dim gEquipmentID As Guid = Guid.Empty
            Dim oEquipmentItem As EquipmentItem = Nothing

            While .Read()
                If Not gGroupID.Equals(.GetGuid("EQUIPMENTGROUPID")) Then
                    gGroupID = .GetGuid("EQUIPMENTGROUPID")
                    oGroup = Me.Find(gGroupID)
                End If

                If Not gEquipmentID.Equals(.GetGuid("EQUIPMENTID")) Then
                    gEquipmentID = .GetGuid("EQUIPMENTID")
                    oEquipmentItem = oGroup.Equipment.Item(gEquipmentID)
                End If

                If Not oEquipmentItem Is Nothing Then
                    Call oEquipmentItem.FetchFitting(dataReader)
                End If

            End While

        End With
    End Sub
    Private Sub FetchEquipmentIncompatibilities(ByVal dataReader As SafeDataReader)

        With dataReader
            Dim gGroupID As Guid = Guid.Empty
            Dim oGroup As EquipmentGroup = Nothing

            Dim gEquipmentID As Guid = Guid.Empty
            Dim oEquipmentItem As EquipmentItem = Nothing

            While .Read()
                If Not gGroupID.Equals(.GetGuid("EQUIPMENTGROUPID")) Then
                    gGroupID = .GetGuid("EQUIPMENTGROUPID")
                    oGroup = Me.Find(gGroupID)
                End If

                If Not gEquipmentID.Equals(.GetGuid("EQUIPMENTID")) Then
                    gEquipmentID = .GetGuid("EQUIPMENTID")
                    oEquipmentItem = oGroup.Equipment.Item(gEquipmentID)
                End If

                If Not oEquipmentItem Is Nothing Then
                    Call oEquipmentItem.FetchIncompatibility(dataReader)
                End If

            End While

        End With
    End Sub
    Private Sub FetchSuffixFittings(ByVal dataReader As SafeDataReader)
        Dim _id As Guid = Guid.Empty
        Dim _item As [Option] = Nothing

        While dataReader.Read
            If Not _id.Equals(dataReader.GetGuid("EQUIPMENTID")) Then
                _id = dataReader.GetGuid("EQUIPMENTID")
                _item = DirectCast(Me.FindEquipment(_id), [Option])
            End If
            If Not _item Is Nothing Then
                _item.FetchSuffixFitting(dataReader)
            End If
        End While
    End Sub

    Private Sub PrepareForEagerLoadOfEquipment()
        For Each group In _referenceMap.Values
            group.PrepareForEagerLoadOfEquipment()
        Next
    End Sub
    Private Sub PrepareForEagerLoadOfApplicabilities()
        For Each group In _referenceMap.Values
            group.Equipment.PrepareForEagerLoadOfApplicabilities()
        Next
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class EquipmentGroup
    Inherits ContextUniqueGuidBusinessBase(Of EquipmentGroup)
    Implements ISortedIndex
    Implements ISortedIndexSetter

#Region " Business Properties & Methods "
    Private _name As String = String.Empty
    Private _code As String = String.Empty
    Private _index As Integer

    Private _children As EquipmentGroups
    Private WithEvents _equipment As EquipmentItems
    Private WithEvents _rules As EquipmentRules

    Public Shadows ReadOnly Property ID() As Guid
        Get
            Return MyBase.ID
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Code() As String
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
    Public ReadOnly Property Index() As Integer Implements ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not PermissionBroker.GetBroker().GetPermissions(Me).Sort Then Exit Property
            If Not _index.Equals(value) Then
                _index = value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.None)> Public Property ParentGroup() As EquipmentGroup
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, EquipmentGroups).ParentGroup
        End Get
        Set(ByVal value As EquipmentGroup)
            If value Is Nothing Then
                value = Me.ParentGroup
                Do While Not value.ID.Equals(Guid.Empty)
                    value = value.ParentGroup
                Loop
            End If

            If Not Me.Parent.Equals(value) Then
                Me.ParentGroup.Groups.Remove(Me)
                value.Groups.Add(Me)
                Me.MarkUnDeleted()
                PropertyHasChanged("ParentGroup")
            End If
        End Set
    End Property

    <XmlInfo(XmlNodeType.None)> Public ReadOnly Property Path() As String
        Get
            Dim _group As EquipmentGroup = Me.ParentGroup
            Dim _path As String = Me.Name

            While (Not _group Is Nothing) AndAlso (Not _group.ID.Equals(Guid.Empty))
                _path = _group.Name & "/" & _path
                _group = _group.ParentGroup
            End While

            Return _path
        End Get
    End Property


    Public ReadOnly Property Groups() As EquipmentGroups
        Get
            If _children Is Nothing Then _children = EquipmentGroups.NewEquipmentGroups(Me)
            Return _children
        End Get
    End Property
    Public ReadOnly Property Equipment() As EquipmentItems
        Get
            If _equipment Is Nothing Then
                If Me.IsNew Then
                    _equipment = EquipmentItems.NewEquipmentItems(Me)
                Else
                    _equipment = EquipmentItems.GetEquipmentItems(Me)
                End If
            End If
            Return _equipment
        End Get
    End Property
    Friend Sub MoveEquipmentItemToAnotherGroup(equipmentItem As EquipmentItem, newLocation As EquipmentGroupInfo)
        If newLocation Is Nothing Then Exit Sub

        Equipment.Remove(equipmentItem)
        Groups.Find(newLocation.ID).Equipment.Add(equipmentItem)
    End Sub

    Friend Sub PrepareForEagerLoadOfEquipment()
        _equipment = EquipmentItems.NewEquipmentItems(Me)
    End Sub

    Public ReadOnly Property Rules() As EquipmentRules
        Get
            If _rules Is Nothing Then
                _rules = EquipmentRules.NewEquipmentRules(Me)
            End If
            Return _rules
        End Get
    End Property

    Private Sub RulesCollectionChanged(ByVal sender As Object, ByVal e As ComponentModel.ListChangedEventArgs) Handles _rules.ListChanged
        If _rules.FetchingData Then Exit Sub
        If _equipment Is Nothing Then Exit Sub
        If Not (e.ListChangedType = ComponentModel.ListChangedType.ItemAdded OrElse e.ListChangedType = ComponentModel.ListChangedType.ItemDeleted) Then Exit Sub

        For Each _item As EquipmentItem In _equipment
            If _item.RulesFetched Then
                _item.Rules.Synchronize()
            End If
        Next
    End Sub
    Public Function GetInfo() As EquipmentGroupInfo
        Return EquipmentGroupInfo.GetEquipmentGroupInfo(Me)
    End Function

#End Region

#Region " Business  & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))
    End Sub

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If _children IsNot Nothing AndAlso Not _children.IsValid Then Return False
            If _equipment IsNot Nothing AndAlso Not _equipment.IsValid Then Return False
            If _rules IsNot Nothing AndAlso Not _rules.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If _children IsNot Nothing AndAlso _children.IsDirty Then Return True
            If _equipment IsNot Nothing AndAlso _equipment.IsDirty Then Return True
            If _rules IsNot Nothing AndAlso _rules.IsDirty Then Return True
            Return False
        End Get
    End Property

    Public Overloads Overrides Sub Delete()
        Me.ParentGroup.Groups.Remove(Me)
    End Sub

#End Region

#Region " Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return _name
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        Return MyBase.Equals(obj)
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function DummyRootGroup(ByVal rootGroups As EquipmentGroups) As EquipmentGroup
        Return New EquipmentGroup(rootGroups)
    End Function

#End Region

#Region " Constructors "

    Private Sub New(ByVal rootGroups As EquipmentGroups)
        'Constructor for the dummy root Group
        MyBase.ID = Guid.Empty
        _children = rootGroups
        AllowRemove = False
        AllowEdit = False
        MarkAsChild()
    End Sub
    Private Sub New()
        'Concstructor for a new and Group
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _index = .GetInt16("SORTORDER")
        End With
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command
            .Parameters.AddWithValue("@INTERNALCODE", Me.Code)
            .Parameters.AddWithValue("@SHORTNAME", Me.Name)
            .Parameters.AddWithValue("@SORTORDER", Me.Index)
            .Parameters.AddWithValue("@PARENTGROUPID", Me.ParentGroup.ID.GetDbValue())
        End With
    End Sub
    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If _children IsNot Nothing Then _children.Update(transaction)
        If _equipment IsNot Nothing Then _equipment.Update(transaction)
        If _rules IsNot Nothing Then _rules.Update(transaction)
    End Sub
#End Region



End Class
<Serializable()> Public NotInheritable Class EquipmentGroupInfo

#Region " Business Properties & Methods "
    
    Private _id As Guid = Guid.Empty
    Private _name As String = String.Empty

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property

    Public Shared ReadOnly Property Empty() As EquipmentGroupInfo
        Get
            Return New EquipmentGroupInfo()
        End Get
    End Property

    Public Function IsEmpty() As Boolean
        Return ID.Equals(Guid.Empty)
    End Function

#End Region

#Region " Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return Me.ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As EquipmentGroup) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As EquipmentGroupInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is EquipmentGroup Then
            Return Me.Equals(DirectCast(obj, EquipmentGroup))
        ElseIf TypeOf obj Is EquipmentGroupInfo Then
            Return Me.Equals(DirectCast(obj, EquipmentGroupInfo))
        ElseIf TypeOf obj Is Guid Then
            Return _id.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is EquipmentGroupInfo Then
            Return DirectCast(objA, EquipmentGroupInfo).Equals(objB)
        ElseIf TypeOf objB Is EquipmentGroupInfo Then
            Return DirectCast(objB, EquipmentGroupInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetEquipmentGroupInfo(ByVal dataReader As SafeDataReader) As EquipmentGroupInfo
        If dataReader.GetGuid("EQUIPMENTGROUPID").Equals(Guid.Empty) Then Return Empty

        Dim _info As EquipmentGroupInfo = New EquipmentGroupInfo
        _info.Fetch(dataReader)
        Return _info
    End Function
    Friend Shared Function GetEquipmentGroupInfo(ByVal group As EquipmentGroup) As EquipmentGroupInfo
        If group Is Nothing Then Return Empty

        Dim _info As EquipmentGroupInfo = New EquipmentGroupInfo
        _info.Fetch(group)
        Return _info
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "

    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _id = .GetGuid("EQUIPMENTGROUPID")
            _name = .GetString("EQUIPMENTGROUPNAME")
        End With
    End Sub
    Private Sub Fetch(ByVal group As EquipmentGroup)
        With group
            _id = .ID
            _name = .Name
        End With
    End Sub

#End Region
End Class