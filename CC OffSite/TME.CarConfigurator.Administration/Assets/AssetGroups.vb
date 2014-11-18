Imports System.Collections.Generic
Imports System.ComponentModel
Imports TME.BusinessObjects.Core
Imports TME.CarConfigurator.Administration.Enums
Imports TME.BusinessObjects.ValidationRules
Imports TME.BusinessObjects.Validation

Namespace Assets

    <Serializable()> Public NotInheritable Class AssetGroups
        Inherits StronglySortedListBase(Of AssetGroups, AssetGroup)

#Region " Business Properties & Methods "

        <NotUndoable()> Private _referenceMap As Dictionary(Of Guid, AssetGroup)

        Public Function Find(ByVal id As Guid) As AssetGroup
            If Not ReferenceMap.ContainsKey(id) Then Return Nothing
            Return ReferenceMap.Item(id)
        End Function
        Private ReadOnly Property ReferenceMap() As Dictionary(Of Guid, AssetGroup)
            Get
                If _referenceMap Is Nothing Then _referenceMap = ParentGroup.ParentGroup.Groups.ReferenceMap()
                Return _referenceMap
            End Get
        End Property

        Public ReadOnly Property ParentGroup() As AssetGroup
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, AssetGroup)
            End Get
        End Property
        Private Function GetRootGroup() As AssetGroup
            Return ReferenceMap(Guid.Empty)
        End Function

        Public Function GetGroupFor(ByVal [object] As Object, ByVal assetTypeGroup As AssetTypeGroup, Optional ByVal create As Boolean = False) As AssetGroup
            If TypeOf [object] Is ModelGenerationEquipmentItem Then Return GetGroupFor(DirectCast([object], ModelGenerationEquipmentItem), assetTypeGroup, create)
            If TypeOf [object] Is ModelGenerationPack Then Return GetGroupFor(DirectCast([object], ModelGenerationPack), assetTypeGroup, create)

            If TypeOf [object] Is ModelGenerationExteriorColour Then Return GetGroupFor(DirectCast([object], ModelGenerationExteriorColour), assetTypeGroup, create)
            If TypeOf [object] Is ModelGenerationUpholstery Then Return GetGroupFor(DirectCast([object], ModelGenerationUpholstery), assetTypeGroup, create)


            If TypeOf [object] Is ModelGenerationCarPart Then Return GetGroupFor(DirectCast([object], ModelGenerationCarPart), assetTypeGroup, create)
            If TypeOf [object] Is ModelGenerationGrade Then Return GetGroupFor(DirectCast([object], ModelGenerationGrade).Generation, assetTypeGroup, create)
            If TypeOf [object] Is ModelGenerationGradeSubModel Then Return GetGroupFor(DirectCast([object], ModelGenerationGradeSubModel).Grade.Generation, assetTypeGroup, create)
            If TypeOf [object] Is ModelGenerationSubModel Then Return GetGroupFor(DirectCast([object], ModelGenerationSubModel).Generation, assetTypeGroup, create)
            If TypeOf [object] Is ModelGenerationWheelDrive Then Return GetGroupFor(DirectCast([object], ModelGenerationWheelDrive).Generation, assetTypeGroup, create)
            If TypeOf [object] Is ModelGenerationEngine Then Return GetGroupFor(DirectCast([object], ModelGenerationEngine).Generation, assetTypeGroup, create)
            If TypeOf [object] Is ModelGenerationTransmission Then Return GetGroupFor(DirectCast([object], ModelGenerationTransmission).Generation, assetTypeGroup, create)
            If TypeOf [object] Is ModelGenerationBodyType Then Return GetGroupFor(DirectCast([object], ModelGenerationBodyType).Generation, assetTypeGroup, create)

            If TypeOf [object] Is ModelGeneration Then Return GetGroupFor(DirectCast([object], ModelGeneration), assetTypeGroup, create)

            If TypeOf [object] Is ModelGenerationFactoryOptionValue Then Return GetGroupFor(DirectCast([object], ModelGenerationFactoryOptionValue), assetTypeGroup, create)
            Throw New ArgumentException("This method does not support the object of type " & [object].GetType().Name, "object")
        End Function

        Public Function GetGroupFor(ByVal obj As ModelGenerationEquipmentItem, ByVal assetTypeGroup As AssetTypeGroup, Optional ByVal create As Boolean = False) As AssetGroup
            If TypeOf obj Is ModelGenerationAccessory Then
                Return GetGroupFor(obj.Generation, Entity.ACCESSORY, assetTypeGroup, create)
            Else
                Return GetGroupFor(obj.Generation, Entity.EQUIPMENT, assetTypeGroup, create)
            End If
        End Function
        Public Function GetGroupFor(ByVal obj As ModelGenerationPack, ByVal assetTypeGroup As AssetTypeGroup, Optional ByVal create As Boolean = False) As AssetGroup
            Return GetGroupFor(obj.Generation, Entity.PACK, assetTypeGroup, create)
        End Function
        Public Function GetGroupFor(ByVal obj As ModelGenerationFactoryOptionValue, ByVal assetTypeGroup As AssetTypeGroup, Optional ByVal create As Boolean = False) As AssetGroup
            Return GetGroupFor(obj.[Option].FactoryGeneration.Generation, Entity.FACTORYGENERATIONOPTIONVALUE, assetTypeGroup, create)
        End Function
        Public Function GetGroupFor(ByVal obj As ModelGenerationExteriorColour, ByVal assetTypeGroup As AssetTypeGroup, Optional ByVal create As Boolean = False) As AssetGroup
            Return GetGroupFor(obj.Generation, Entity.EXTERIORCOLOUR, assetTypeGroup, create)
        End Function
        Public Function GetGroupFor(ByVal obj As ModelGenerationUpholstery, ByVal assetTypeGroup As AssetTypeGroup, Optional ByVal create As Boolean = False) As AssetGroup
            Return GetGroupFor(obj.Generation, Entity.UPHOLSTERY, assetTypeGroup, create)
        End Function
        Public Function GetGroupFor(ByVal obj As ModelGenerationCarPart, ByVal assetTypeGroup As AssetTypeGroup, Optional ByVal create As Boolean = False) As AssetGroup
            Return GetGroupFor(obj.Generation, Entity.CARPART, assetTypeGroup, create)
        End Function
        Private Function GetGroupFor(ByVal generation As ModelGeneration, ByVal obj As Entity, ByVal assetTypeGroup As AssetTypeGroup, Optional ByVal create As Boolean = False) As AssetGroup
            Dim generationAndAssetTypeGroup As AssetGroup = GetGroupFor(generation, assetTypeGroup, create)
            If generationAndAssetTypeGroup Is Nothing AndAlso Not create Then Return Nothing

            Dim itemGroup As AssetGroup = generationAndAssetTypeGroup.Groups.FirstOrDefault(Function(x) x.Name.Equals(obj.GetTitle(), StringComparison.InvariantCultureIgnoreCase))
            If itemGroup IsNot Nothing Then Return itemGroup
            If Not create Then Return Nothing

            itemGroup = generationAndAssetTypeGroup.Groups.Add()
            itemGroup.Name = obj.GetTitle()
            AddToReferenceMap(itemGroup)
            Return itemGroup
        End Function

        Public Function GetGroupFor(ByVal obj As ModelGeneration, ByVal assetTypeGroup As AssetTypeGroup, Optional ByVal create As Boolean = False) As AssetGroup
            Dim generationGroup As AssetGroup = GetGroupFor(obj, create)
            If generationGroup Is Nothing AndAlso Not create Then Return Nothing

            Dim assetTypeGroupGroup As AssetGroup = generationGroup.Groups.FirstOrDefault(Function(x) x.Name.Equals(assetTypeGroup.Name, StringComparison.InvariantCultureIgnoreCase))
            If assetTypeGroupGroup IsNot Nothing Then Return assetTypeGroupGroup
            If Not create Then Return Nothing

            assetTypeGroupGroup = generationGroup.Groups.Add()
            assetTypeGroupGroup.Name = assetTypeGroup.Name
            AddToReferenceMap(assetTypeGroupGroup)
            Return assetTypeGroupGroup
        End Function
        Public Function GetGroupFor(ByVal generation As ModelGeneration, Optional ByVal create As Boolean = False) As AssetGroup
            If ReferenceMap.ContainsKey(generation.ID) Then Return ReferenceMap(generation.ID)
            If Not create Then Return Nothing

            Dim modelGroup As AssetGroup
            If ReferenceMap.ContainsKey(generation.Model.ID) Then
                modelGroup = ReferenceMap(generation.Model.ID)
            Else
                modelGroup = GetRootGroup().Groups.Add(generation.Model.ID)
                modelGroup.Name = generation.Model.Name
                AddToReferenceMap(modelGroup)
            End If

            Dim generationGroup As AssetGroup = modelGroup.Groups.Add(generation.ID)
            generationGroup.Name = generation.Name.Replace(generation.Model.Name, String.Empty).Trim()
            AddToReferenceMap(generationGroup)
            Return generationGroup
        End Function



        Private Sub OnAssetGroupsListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs) Handles Me.ListChanged
            If e.ListChangedType = ListChangedType.ItemAdded Then
                AddToReferenceMap(Me(e.NewIndex))
            End If

        End Sub

        Private Sub AddToReferenceMap(ByVal group As AssetGroup)
            If ReferenceMap.ContainsKey(group.ID) Then Return
            ReferenceMap.Add(group.ID, group)
        End Sub

        Private Sub OnAssetGroupsRemovingItem(ByVal sender As Object, ByVal e As RemovingItemEventArgs) Handles Me.RemovingItem
            ReferenceMap.Remove(DirectCast(e.RemovingItem, AssetGroup).ID)
        End Sub


#End Region

#Region " Shared Factory Methods "

        Public Shared Function GetRootGroups() As AssetGroups
            Return DataPortal.Fetch(Of AssetGroups)(New Criteria)
        End Function
        Friend Shared Function NewGroups(ByVal parent As AssetGroup) As AssetGroups
            Dim groups As AssetGroups = New AssetGroups()
            groups.SetParent(parent)
            groups.MarkAsChild()
            Return groups
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            'Allow data portal to create us
        End Sub
#End Region

#Region " Data Access "

        Protected Overrides Sub Fetch(ByVal dataReader As SafeDataReader)
            Dim rootGroup As AssetGroup = AssetGroup.DummyRootGroup(Me)
            SetParent(rootGroup)

            _referenceMap = New Dictionary(Of Guid, AssetGroup)
            _referenceMap.Add(rootGroup.ID, rootGroup)
            While dataReader.Read()
                Dim group As AssetGroup = GetObject(dataReader)
                _referenceMap(dataReader.GetGuid("PARENTGROUPID")).Groups.Add(group)
            End While
        End Sub

#End Region

    End Class
    <Serializable()> Public NotInheritable Class AssetGroup
        Inherits ContextUniqueGuidBusinessBase(Of AssetGroup)
        Implements ISortedIndex
        Implements ISortedIndexSetter

#Region " Business Properties & Methods "
        Private _name As String = String.Empty
        Private _index As Integer

        Private _children As AssetGroups
        Private _assets As Assets

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                value = value.Replace("/", "-")
                If _name <> value Then
                    _name = value
                    PropertyHasChanged("Name")
                End If
            End Set
        End Property

        <XmlInfo(XmlNodeType.None)> Public ReadOnly Property Path() As String
            Get
                Dim group As AssetGroup = ParentGroup
                Dim buffer As String = Name

                While (Not group Is Nothing) AndAlso (Not group.ID.Equals(Guid.Empty))
                    buffer = group.Name & "/" & buffer
                    group = group.ParentGroup
                End While

                Return buffer
            End Get
        End Property

        Public Property ParentGroup() As AssetGroup
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, AssetGroups).ParentGroup
            End Get
            Set(ByVal value As AssetGroup)

                If value Is Nothing Then
                    value = ParentGroup
                    Do While Not value.ID.Equals(Guid.Empty)
                        value = value.ParentGroup
                    Loop
                End If

                If Not ParentGroup.Equals(value) Then
                    ParentGroup.Groups.Remove(Me)

                    'Add to new parent
                    value.Groups.Add(Me)

                    'Since removal from the parentGroup marked me for deletion,
                    'I'll have to un mark myself for deletion
                    MarkUnDeleted()
                    PropertyHasChanged("ParentGroup")
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
                If _index.Equals(value) Then Return

                _index = value
                PropertyHasChanged("Index")
            End Set
        End Property

        Public ReadOnly Property Groups() As AssetGroups
            Get
                If _children Is Nothing Then _children = AssetGroups.NewGroups(Me)
                Return _children
            End Get
        End Property
        Public ReadOnly Property Assets() As Assets
            Get
                If _assets Is Nothing Then
                    If IsNew Then
                        _assets = Assets.NewAssets(Me)
                    Else
                        _assets = Assets.GetAssets(Me)
                    End If
                End If
                Return _assets
            End Get
        End Property

#End Region

#Region " Business  & Validation Rules "
        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf Value.Unique, RuleHandler), "Name")
            ValidationRules.AddRule(DirectCast(AddressOf [String].Required, RuleHandler), "Name")
            ValidationRules.AddRule(DirectCast(AddressOf [String].MaxLength, RuleHandler), New [String].MaxLengthRuleArgs("Name", 255))
        End Sub
#End Region

#Region " System.Object Overrides "

        Public Overloads Overrides Function ToString() As String
            Return Name
        End Function

#End Region

#Region " Framework Overrides "

        Public Overloads Overrides ReadOnly Property IsValid() As Boolean
            Get
                If Not MyBase.IsValid Then Return False
                If Not (_children Is Nothing) AndAlso Not _children.IsValid Then Return False
                If Not (_assets Is Nothing) AndAlso Not _assets.IsValid Then Return False
                Return True
            End Get
        End Property
        Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
            Get
                If MyBase.IsDirty Then Return True
                If Not (_children Is Nothing) AndAlso _children.IsDirty Then Return True
                If Not (_assets Is Nothing) AndAlso _assets.IsDirty Then Return True
                Return False
            End Get
        End Property

        Public Shadows Sub Delete()
            ParentGroup.Groups.Remove(Me)
        End Sub

#End Region

#Region " Shared Factory Methods "
        Friend Shared Function DummyRootGroup(ByVal rootGroups As AssetGroups) As AssetGroup
            Dim group As AssetGroup = New AssetGroup(rootGroups)
            group.ID = Guid.Empty
            Return group
        End Function
#End Region

#Region " Constructors "

        Private Sub New(ByVal rootGroups As AssetGroups)
            'Constructor for the dummy root Group
            _children = rootGroups
            AllowRemove = False
            AllowEdit = False
            MarkAsChild()
        End Sub
        Private Sub New()
            'Concstructor for a new Group
            MarkAsChild()
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            _name = dataReader.GetString("NAME")
            _index = dataReader.GetInt16("SORTORDER")
        End Sub
        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
        End Sub
        Private Sub AddCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@NAME", Name)
            command.Parameters.AddWithValue("@SORTORDER", Index)
            command.Parameters.AddWithValue("@PARENTGROUPID", ParentGroup.ID.GetDbValue())
        End Sub

        Protected Overrides Sub UpdateChildren(ByVal transaction As SqlTransaction)
            If Not _children Is Nothing Then _children.Update(transaction)
            If Not _assets Is Nothing Then _assets.Update(transaction)
        End Sub

#End Region

    End Class
End Namespace