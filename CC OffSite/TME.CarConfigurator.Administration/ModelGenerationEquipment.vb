Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Exceptions
Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
Imports TME.CarConfigurator.Administration.Components.Interfaces
Imports TME.CarConfigurator.Administration.Components
Imports TME.BusinessObjects.Validation

<Serializable()> Public NotInheritable Class ModelGenerationEquipment
    Inherits ContextUniqueGuidListBase(Of ModelGenerationEquipment, ModelGenerationEquipmentItem)

#Region " Delegates & Events "
    Friend Delegate Sub EquipmentChangedHandler(ByVal item As ModelGenerationEquipmentItem)
    Friend Event EquipmentAdded As EquipmentChangedHandler
    Friend Event EquipmentRemoved As EquipmentChangedHandler
#End Region

#Region " Business Properties & Methods "

    <NonSerialized()> Private _referenceMap As Dictionary(Of Guid, ModelGenerationEquipmentItem)

    Friend ReadOnly Property Generation() As ModelGeneration
        Get
            Return DirectCast(Parent, ModelGeneration)
        End Get
    End Property

    Default Public Overloads Overrides ReadOnly Property Item(ByVal id As Guid) As ModelGenerationEquipmentItem
        Get
            If id.Equals(Guid.Empty) Then Return Nothing
            If ReferenceMap.ContainsKey(id) Then
                Return ReferenceMap.Item(id)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Default Public Overloads ReadOnly Property Item(ByVal code As String) As ModelGenerationEquipmentItem
        Get
            Return FirstOrDefault(Function(x) x.Equals(code))
        End Get
    End Property

    Public Overloads Function Add(ByVal equipmentItem As EquipmentItem) As ModelGenerationEquipmentItem
        If Contains(equipmentItem.ID) Then Throw New ObjectAlreadyExists("The item already exists in this collection")
        If Not equipmentItem.Approved Then Throw New ApplicationException(String.Format("The equipment item ""{0}"" has not been approved for publication. You need to approve the item before it can be added to the generation.", equipmentItem.ToString()))
        If Generation.Mode = LocalizationMode.LocalConfiguration AndAlso equipmentItem.Type = EquipmentType.Option AndAlso Not equipmentItem.IsMasterObject() Then Throw New ApplicationException(String.Format("The equipment item ""{0}"" is not a master item and can not be added to the generation.", equipmentItem.ToString()))

        Dim generationEquipmentItem As ModelGenerationEquipmentItem = Nothing
        Select Case equipmentItem.Type
            Case EquipmentType.Option
                generationEquipmentItem = ModelGenerationOption.GetOption(DirectCast(equipmentItem, [Option]))
            Case EquipmentType.Accessory
                generationEquipmentItem = ModelGenerationAccessory.GetAccessory(DirectCast(equipmentItem, Accessory))
        End Select
        ReferenceMap.Add(generationEquipmentItem.ID, generationEquipmentItem)
        Add(generationEquipmentItem)
        Return generationEquipmentItem
    End Function


    Private ReadOnly Property ReferenceMap() As Dictionary(Of Guid, ModelGenerationEquipmentItem)
        Get
            If _referenceMap Is Nothing Then
                _referenceMap = New Dictionary(Of Guid, ModelGenerationEquipmentItem)(Count)
                For Each generationEquipmentItem As ModelGenerationEquipmentItem In Me
                    _referenceMap.Add(generationEquipmentItem.ID, generationEquipmentItem)
                Next
            End If
            Return _referenceMap
        End Get
    End Property
    Private Sub MyListChanged(ByVal sender As Object, ByVal e As ComponentModel.ListChangedEventArgs) Handles Me.ListChanged
        If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded Then
            Dim generationEquipmentItem As ModelGenerationEquipmentItem = Me(e.NewIndex)
            If Not ReferenceMap.ContainsKey(generationEquipmentItem.ID) Then ReferenceMap.Add(generationEquipmentItem.ID, generationEquipmentItem)
            RaiseEvent EquipmentAdded(generationEquipmentItem)
        End If

    End Sub
    Private Sub MyRemovingItem(ByVal sender As Object, ByVal e As Core.RemovingItemEventArgs) Handles Me.RemovingItem
        ReferenceMap.Remove(DirectCast(e.RemovingItem, ModelGenerationEquipmentItem).ID)
        RaiseEvent EquipmentRemoved(DirectCast(e.RemovingItem, ModelGenerationEquipmentItem))
    End Sub

    Public Function Options() As IEnumerable(Of ModelGenerationOption)
        Return OfType(Of ModelGenerationOption)()
    End Function
    Public Function Accessories() As IEnumerable(Of ModelGenerationAccessory)
        Return OfType(Of ModelGenerationAccessory)()
    End Function

#End Region

#Region " Contains Methods "
    Public Overloads Overrides Function Contains(ByVal id As Guid) As Boolean
        Return ReferenceMap.ContainsKey(id)
    End Function
    Public Overloads Function Contains(ByVal obj As ModelGenerationEquipmentItem) As Boolean
        Return ReferenceMap.ContainsKey(obj.ID)
    End Function
    Public Overloads Function Contains(ByVal obj As EquipmentItem) As Boolean
        Return ReferenceMap.ContainsKey(obj.ID)
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewEquipment(ByVal generation As ModelGeneration) As ModelGenerationEquipment
        Dim equipment As ModelGenerationEquipment = New ModelGenerationEquipment()
        equipment.SetParent(generation)
        Return equipment
    End Function
    Friend Shared Function GetEquipment(ByVal generation As ModelGeneration) As ModelGenerationEquipment
        Dim equipment As ModelGenerationEquipment = DataPortal.Fetch(Of ModelGenerationEquipment)(New CustomCriteria(generation))
        equipment.SetParent(generation)
        Return equipment
    End Function
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _generationID As Guid

        Public Sub New(ByVal generation As ModelGeneration)
            _generationID = generation.ID
            CommandText = "getGenerationEquipment"
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
        End Sub

    End Class
#End Region

#Region " Data Access "
    Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
        Get
            Return False
        End Get
    End Property
    Protected Overrides Function GetObject(ByVal dataReader As Common.Database.SafeDataReader) As ModelGenerationEquipmentItem
        Dim type As EquipmentType = dataReader.GetEquipmentType("TYPE")
        Select Case type
            Case EquipmentType.Accessory
                Return ModelGenerationAccessory.GetAccessory(dataReader)
            Case EquipmentType.Option
                Return ModelGenerationOption.GetOption(dataReader)
            Case EquipmentType.ExteriorColourType
                Return ModelGenerationExteriorColourType.GetExteriorColourType(dataReader)
            Case EquipmentType.UpholsteryType
                Return ModelGenerationUpholsteryType.GetUpholsteryType(dataReader)
            Case Else
                Throw New InvalidEquipmentType("""" & type & """ is not a valid equipment type!")
        End Select
    End Function

    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        FetchExteriorColourApplicabilities(dataReader)
        If dataReader.NextResult Then FetchUpholsteryApplicabilities(dataReader)
        If dataReader.NextResult Then FetchRules(dataReader) 'Item rules
        If dataReader.NextResult Then FetchRules(dataReader) 'Pack rules
        If dataReader.NextResult Then FetchComponents(dataReader)
    End Sub

    Private Sub FetchExteriorColourApplicabilities(ByVal dataReader As Common.Database.SafeDataReader)
        While dataReader.Read
            FetchExteriorColourApplicability(dataReader)
        End While
    End Sub

    Private Sub FetchUpholsteryApplicabilities(ByVal dataReader As Common.Database.SafeDataReader)
        While dataReader.Read
            FetchUpholsteryApplicability(dataReader)
        End While
    End Sub

    Private Sub FetchRules(ByVal dataReader As Common.Database.SafeDataReader)
        While dataReader.Read
            FetchRule(dataReader)
        End While
    End Sub

    Private Sub FetchComponents(ByVal dataReader As Common.Database.SafeDataReader)
        While dataReader.Read
            FetchComponent(dataReader)
        End While
    End Sub

#Region "Helper Methods"

    Private Sub FetchExteriorColourApplicability(ByVal dataReader As SafeDataReader)
        Dim theOption = TryGetOption(dataReader)
        If theOption Is Nothing Then Return
        theOption.ExteriorColourApplicabilities.Add(dataReader)
    End Sub

    Private Sub FetchUpholsteryApplicability(ByVal dataReader As SafeDataReader)
        Dim theOption = TryGetOption(dataReader)
        If theOption Is Nothing Then Return
        theOption.UpholsteryApplicabilities.Add(dataReader)
    End Sub

    Private Sub FetchRule(ByVal dataReader As SafeDataReader)
        Dim theEquipmentItem = TryGetEquipmentItem(dataReader)
        If theEquipmentItem Is Nothing Then Return
        theEquipmentItem.Rules.Add(dataReader)
    End Sub

    Private Sub FetchComponent(ByVal dataReader As SafeDataReader)
        Dim theOption = TryGetOption(dataReader)
        If theOption Is Nothing Then Return
        theOption.Components.SetPermissions(True) 'make sure we can add to the list, even if the current context doesn't allow for it
        theOption.Components.Add(dataReader)
        theOption.Components.ResetPermissions() 'reset permissions to what they should be
    End Sub

    Private Function TryGetOption(ByVal dataReader As SafeDataReader) As ModelGenerationOption
        Dim [option] = TryGetEquipmentItem(dataReader)
        Return If([option] Is Nothing OrElse [option].Type <> EquipmentType.Option, Nothing, DirectCast([option], ModelGenerationOption))
    End Function
    Private Function TryGetEquipmentItem(ByVal dataReader As SafeDataReader) As ModelGenerationEquipmentItem
        Dim id As Guid = dataReader.GetGuid("EQUIPMENTID")
        Dim equipmentItem = Me(id)
        Return equipmentItem
    End Function

#End Region
#End Region

End Class

<Serializable()> Public MustInherit Class ModelGenerationEquipmentItem
    Inherits LocalizeableBusinessBase
    Implements IUpdatableAssetSet
    Implements IOwnedBy
    Implements IMasterObjectReference
    Implements IEquipmentBestVisibleIn

#Region " Business Properties & Methods "
    Private _shortID As Nullable(Of Integer) = Nothing
    Private _objectId As Guid = Guid.Empty
    Private _partNumber As String = String.Empty
    Private _name As String = String.Empty
    Private _keyFeature As Boolean = False
    Private _keyFeatureRequiresValidation As Boolean = False
    Private _visibility As ItemVisibility
    Private _owner As String = String.Empty
    Private _index As Integer
    Private _group As EquipmentGroupInfo
    Private _category As EquipmentCategoryInfo
    Private _assetSet As AssetSet
    Private _rules As ModelGenerationEquipmentRules
    Private _colour As ExteriorColourInfo

    Private _bestVisibleInMode As String
    Private _bestVisibleInView As String
    Private _bestVisibleInAngle As Integer

    Private _crossModelBestVisibleInMode As String
    Private _crossModelBestVisibleInView As String
    Private _crossModelBestVisibleInAngle As Integer

    Private _masterID As Guid
    Private _masterDescription As String
    Private _masterType As MasterEquipmentType

    Public ReadOnly Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationEquipment).Generation
        End Get
    End Property

    Public ReadOnly Property ShortID() As Nullable(Of Integer)
        Get
            Return _shortID
        End Get
    End Property
    Private Property ObjectID() As Guid
        Get
            Return _objectId
        End Get
        Set(ByVal value As Guid)
            _objectId = value
        End Set
    End Property
    Public ReadOnly Property PartNumber() As String
        Get
            Return _partNumber
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public Overridable Property KeyFeature() As Boolean
        Get
            Return _keyFeature
        End Get
        Set(ByVal value As Boolean)
            If KeyFeature = value Then Return

            _keyFeature = value
            _keyFeatureRequiresValidation = True
            PropertyHasChanged("KeyFeature")
            ValidationRules.CheckRules("KeyFeature")
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Visibility() As ItemVisibility
        Get
            Return _visibility
        End Get
        Set(ByVal value As ItemVisibility)
            If Visibility = value Then Return

            _visibility = value
            PropertyHasChanged("Visibility")
        End Set
    End Property
    <XmlInfo(XmlNodeType.None)> Public MustOverride ReadOnly Property Type() As EquipmentType
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return _owner
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public Property Index() As Integer
        Get
            Return _index
        End Get
        Set(ByVal value As Integer)
            If _index.Equals(value) Then Return

            _index = value
            PropertyHasChanged("Index")
        End Set
    End Property
    Public ReadOnly Property Group() As EquipmentGroupInfo
        Get
            Return _group
        End Get
    End Property
    Public ReadOnly Property Category() As EquipmentCategoryInfo
        Get
            Return _category
        End Get
    End Property
    Public ReadOnly Property Colour() As ExteriorColourInfo
        Get
            Return _colour
        End Get
    End Property
    Public ReadOnly Property AssetSet() As AssetSet Implements IHasAssetSet.AssetSet
        Get
            If _assetSet Is Nothing Then _assetSet = AssetSet.NewAssetSet(Me)
            Return _assetSet
        End Get
    End Property

    Public Sub ChangeReference(ByVal updatedAssetSet As AssetSet) Implements IUpdatableAssetSet.ChangeReference
        _assetSet = updatedAssetSet
    End Sub


    Public ReadOnly Property Rules() As ModelGenerationEquipmentRules
        Get
            If _rules Is Nothing Then _rules = ModelGenerationEquipmentRules.NewRules(Me)
            Return _rules
        End Get
    End Property


    Public ReadOnly Property MasterID() As Guid Implements IMasterObjectReference.MasterID
        Get
            Return _masterID
        End Get
    End Property
    Public ReadOnly Property MasterDescription() As String Implements IMasterObjectReference.MasterDescription
        Get
            Return _masterDescription
        End Get
    End Property
    Public ReadOnly Property MasterType() As MasterEquipmentType
        Get
            Return _masterType
        End Get
    End Property


    Public Function GetInfo() As EquipmentItemInfo
        Return EquipmentItemInfo.GetEquipmentInfo(Me)
    End Function

    Public Overridable ReadOnly Property SortPath() As String
        Get
            Return String.Format("{0}/{1}", Index.AddLeadingZeros(), AlternateName)
        End Get
    End Property

    Public Property BestVisibleInMode() As String Implements IEquipmentBestVisibleIn.BestVisibleInMode
        Get
            If String.IsNullOrEmpty(_bestVisibleInMode) AndAlso Not String.IsNullOrEmpty(CrossModelBestVisibleInMode) Then
                _bestVisibleInMode = CrossModelBestVisibleInMode
                _bestVisibleInAngle = CrossModelBestVisibleInAngle
            End If
            Return _bestVisibleInMode
        End Get
        Set(ByVal value As String)
            If _bestVisibleInMode.Equals(value) Then Return
            _bestVisibleInMode = value
            PropertyHasChanged("BestVisibleInMode")
            ValidationRules.CheckRules("BestVisibleIn")
        End Set
    End Property

    Public Property BestVisibleInView() As String Implements IEquipmentBestVisibleIn.BestVisibleInView
        Get
            If String.IsNullOrEmpty(_bestVisibleInView) AndAlso Not String.IsNullOrEmpty(CrossModelBestVisibleInView) Then
                _bestVisibleInView = CrossModelBestVisibleInView
                _bestVisibleInAngle = CrossModelBestVisibleInAngle
            End If
            Return _bestVisibleInView
        End Get
        Set(ByVal value As String)
            If _bestVisibleInView.Equals(value) Then Return
            _bestVisibleInView = value
            PropertyHasChanged("BestVisibleInView")
            ValidationRules.CheckRules("BestVisibleIn")
        End Set
    End Property

    Public Property BestVisibleInAngle() As Integer Implements IEquipmentBestVisibleIn.BestVisibleInAngle
        Get
            Return _bestVisibleInAngle
        End Get
        Set(ByVal value As Integer)
            If _bestVisibleInAngle.Equals(value) Then Return
            _bestVisibleInAngle = value
            PropertyHasChanged("BestVisibleInAngle")
            ValidationRules.CheckRules("BestVisibleIn")
        End Set
    End Property

    Private ReadOnly Property CrossModelBestVisibleInAngle() As Integer
        Get
            Return _crossModelBestVisibleInAngle
        End Get
    End Property

    Private ReadOnly Property CrossModelBestVisibleInView() As String
        Get
            Return _crossModelBestVisibleInView
        End Get
    End Property

    Private ReadOnly Property CrossModelBestVisibleInMode() As String
        Get
            Return _crossModelBestVisibleInMode
        End Get

    End Property

#End Region

#Region " Business & Validation Rules "

    Friend Sub SubModelKeyFeatureChanged(ByVal value As Boolean)
        If Not Generation.SubModels.Any(Function(s) Not s.Equipment(ID).KeyFeature = value) Then
            KeyFeature = value
        Else
            _keyFeatureRequiresValidation = True
            ValidationRules.CheckRules("KeyFeature")
        End If
    End Sub
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf KeyFeatureValid, RuleHandler), "KeyFeature")
        ValidationRules.AddRule(DirectCast(AddressOf ModeAndViewCombinationValid, RuleHandler), "BestVisibleIn")
        ValidationRules.AddRule(DirectCast(AddressOf AngleShouldBeInLimits, RuleHandler), "BestVisibleIn")
    End Sub

    Private Shared Function KeyFeatureValid(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim generationEquipmentItem As ModelGenerationEquipmentItem = DirectCast(target, ModelGenerationEquipmentItem)
        If Not generationEquipmentItem._keyFeatureRequiresValidation Then Return True
        If generationEquipmentItem.Parent Is Nothing Then Throw New DevelopperException(" _item.Parent should not be nothing in case _keyFeatureChanged has changed!")

        If generationEquipmentItem.Generation.SubModels.Count = 0 Then Return True
        If Not generationEquipmentItem.Generation.SubModels.Any(Function(s) s.Equipment(generationEquipmentItem.ID).KeyFeature = generationEquipmentItem.KeyFeature) Then
            If generationEquipmentItem.KeyFeature Then
                e.Description = "The {0} {1} is not a key feature for any of the submodels, hence it can not be a key feature for the model."
            Else
                e.Description = "The {0} {1} is a key feature for all submodels, hence it has to be a key feature for the model as well."
            End If
            e.Description = String.Format(e.Description, generationEquipmentItem.Type.GetTitle(False), generationEquipmentItem.Name)
            Return False
        End If

        Return True
    End Function

    Private Shared Function ModeAndViewCombinationValid(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim obj = DirectCast(target, [ModelGenerationEquipmentItem])
        If String.IsNullOrEmpty(obj.BestVisibleInMode) AndAlso String.IsNullOrEmpty(obj.BestVisibleInView) Then Return True
        If String.IsNullOrEmpty(obj.BestVisibleInMode) OrElse String.IsNullOrEmpty(obj.BestVisibleInView) Then
            e.Description = "Both mode and view should be filled in."
            Return False
        End If

        If Not MyContext.GetContext().AssetTypeGroups.Any(Function(x) x.Mode = obj.BestVisibleInMode AndAlso x.View = obj.BestVisibleInView) Then
            e.Description = String.Format("The mode/view combination {0}/{1} is not valid", obj.BestVisibleInMode, obj.BestVisibleInView)
            Return False
        End If
        Return True
    End Function

    Private Shared Function AngleShouldBeInLimits(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        If Not ModeAndViewCombinationValid(target, e) Then Return True

        Dim obj = DirectCast(target, [ModelGenerationEquipmentItem])

        If String.IsNullOrEmpty(obj.BestVisibleInMode) AndAlso String.IsNullOrEmpty(obj.BestVisibleInView) Then Return True

        Dim assetTypeGroup = MyContext.GetContext().AssetTypeGroups.First(Function(x) x.Mode = obj.BestVisibleInMode AndAlso x.View = obj.BestVisibleInView)

        If (obj.BestVisibleInAngle < assetTypeGroup.MinimumAngle OrElse obj.BestVisibleInAngle > assetTypeGroup.MaximumAngle) Then
            e.Description = String.Format("Angle should be in the limits ({0},{1})", assetTypeGroup.MinimumAngle, assetTypeGroup.MaximumAngle)
            Return False
        End If
        Return True
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        If Owner = Environment.GlobalCountryCode Then
            Return Name
        Else
            Return Name & " [" & Owner & "]"
        End If
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationEquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As EquipmentItemInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As EquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj) OrElse ObjectID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        If obj.Equals(String.Empty) Then Return False
        Return String.Compare(PartNumber, obj, True) = 0
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationEquipmentItem Then
            Return Equals(DirectCast(obj, ModelGenerationEquipmentItem))
        ElseIf TypeOf obj Is EquipmentItemInfo Then
            Return Equals(DirectCast(obj, EquipmentItemInfo))
        ElseIf TypeOf obj Is EquipmentItem Then
            Return Equals(DirectCast(obj, EquipmentItem))
        ElseIf TypeOf obj Is String Then
            Return Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Constructors "

    Protected Sub New()
        MarkAsChild()
        CanHaveLocalCode = False
    End Sub

#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_assetSet Is Nothing) AndAlso Not _assetSet.IsValid Then Return False
            If Not (_rules Is Nothing) AndAlso Not _rules.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_assetSet Is Nothing) AndAlso _assetSet.IsDirty Then Return True
            If Not (_rules Is Nothing) AndAlso _rules.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("EQUIPMENTID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _shortID = .GetInt32("SHORTID")
            _objectId = .GetGuid("ID")
            _partNumber = .GetString("PARTNUMBER")
            _name = .GetString("SHORTNAME")
            _owner = .GetString("OWNER")
            _index = .GetInt16("SORTORDER")
            _keyFeature = .GetBoolean("KEYFEATURE")
            _visibility = DirectCast(.GetInt32("VISIBILITY"), ItemVisibility)
            _masterID = .GetGuid("MASTERID")
            _masterDescription = .GetString("MASTERDESCRIPTION")
            _masterType = DirectCast(.GetInt16(GetFieldName("MASTERTYPE")), MasterEquipmentType)
            _group = EquipmentGroupInfo.GetEquipmentGroupInfo(dataReader)
            _category = EquipmentCategoryInfo.GetEquipmentCategoryInfo(dataReader)
            _colour = ExteriorColourInfo.GetExteriorColourInfo(dataReader)
            _assetSet = AssetSet.GetAssetSet(Me, dataReader)
            _bestVisibleInView = dataReader.GetString("BESTVISIBLEINVIEW")
            _bestVisibleInMode = dataReader.GetString("BESTVISIBLEINMODE")
            _bestVisibleInAngle = .GetInt16("BESTVISIBLEINANGLE")
            _crossModelBestVisibleInView = dataReader.GetString("CROSSMODELBESTVISIBLEINVIEW")
            _crossModelBestVisibleInMode = dataReader.GetString("CROSSMODELBESTVISIBLEINMODE")
            _crossModelBestVisibleInAngle = .GetInt16("CROSSMODELBESTVISIBLEINANGLE")
        End With
        If _objectId.Equals(Guid.Empty) Then
            _objectId = Guid.NewGuid()
            'In case this is the model generation, record then we need to create it (but not for region countries)!
            If Not MyContext.GetContext().IsRegionCountry OrElse MyContext.GetContext().IsMainRegionCountry Then
                MarkDirty()
            End If
            AlwaysUpdateSelf = True
        End If
        MyBase.FetchFields(dataReader)

    End Sub
    Protected Overridable Overloads Sub Create(ByVal equipmenItem As EquipmentItem)
        Create(equipmenItem.ID)
        ObjectID = Guid.NewGuid()
        With equipmenItem
            _partNumber = .PartNumber
            _name = .Name
            _owner = .Owner
            _group = .Group
            _category = .Category
            _colour = .Colour
            _masterID = .MasterID
            _masterDescription = .MasterDescription
            _masterType = .MasterType
        End With
        _visibility = ItemVisibility.Brochure Or ItemVisibility.CarConfigurator Or ItemVisibility.Website
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "insertGenerationEquipmentItem"
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateGenerationEquipmentItem"
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "deleteGenerationEquipmentItem"
        AddCommandSpecializedFields(command)
    End Sub
    Private Sub AddCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@ID", ObjectID)
        command.Parameters.AddWithValue("@GENERATIONID", Generation.ID)
        command.Parameters.AddWithValue("@EQUIPMENTID", ID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overridable Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@KEYFEATURE", KeyFeature)
        command.Parameters.AddWithValue("@VISIBILITY", Visibility)
        command.Parameters.AddWithValue("@SORTORDER", Index)

        If (String.IsNullOrEmpty(BestVisibleInMode) Or (BestVisibleInMode = CrossModelBestVisibleInMode AndAlso BestVisibleInView = CrossModelBestVisibleInView And BestVisibleInAngle = CrossModelBestVisibleInAngle)) Then
            command.Parameters.AddWithValue("@BESTVISIBLEINMODE", DBNull.Value)
            command.Parameters.AddWithValue("@BESTVISIBLEINVIEW", DBNull.Value)
            command.Parameters.AddWithValue("@BESTVISIBLEINANGLE", DBNull.Value)
        Else
            command.Parameters.AddWithValue("@BESTVISIBLEINMODE", CType(BestVisibleInMode, Object))
            command.Parameters.AddWithValue("@BESTVISIBLEINVIEW", If(String.IsNullOrEmpty(BestVisibleInView), DBNull.Value, CType(BestVisibleInView, Object)))
            command.Parameters.AddWithValue("@BESTVISIBLEINANGLE", BestVisibleInAngle)
        End If

    End Sub
    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If _assetSet IsNot Nothing Then _assetSet.Update(transaction)
        If _rules IsNot Nothing Then _rules.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub
#End Region

#Region " Base Object Overrides"

    Public Overrides ReadOnly Property ModelID() As Guid
        Get
            Return Generation.Model.ID
        End Get
    End Property

    Public Overrides ReadOnly Property GenerationID() As Guid
        Get
            Return Generation.ID
        End Get
    End Property

    Protected Friend Overrides Function GetBaseCode() As String
        Return PartNumber
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Name
    End Function

    Public Overrides ReadOnly Property Entity As Entity
        Get
            Select Case Type
                Case EquipmentType.Accessory
                    Return Entity.CARACCESSORY
                Case EquipmentType.Option
                    Return Entity.CAREQUIPMENT
                Case EquipmentType.ExteriorColourType
                    Return Entity.EXTERIORCOLOURTYPE
                Case EquipmentType.UpholsteryType
                    Return Entity.UPHOLSTERYTYPE
                Case Else
                    Throw New ArgumentException(String.Format("No case clause defined for {0}", Type.ToString()))
            End Select
        End Get
    End Property
#End Region

End Class

<Serializable(), XmlInfo("accessory")> Public NotInheritable Class ModelGenerationAccessory
    Inherits ModelGenerationEquipmentItem
    Implements ICanBeModelGenerationOptionComponent

#Region " Business Properties & Methods "
    Private _fullName As String = String.Empty

    Private _basePrice As Decimal = 0D
    Private _baseVatPrice As Decimal = 0D

    Public Overloads Overrides ReadOnly Property Type() As EquipmentType
        Get
            Return EquipmentType.Accessory
        End Get
    End Property

    Public ReadOnly Property FullName() As String
        Get
            Return _fullName
        End Get
    End Property
    Public ReadOnly Property BasePrice() As Decimal
        Get
            Return _basePrice
        End Get
    End Property
    Public ReadOnly Property BaseVatPrice() As Decimal
        Get
            Return _baseVatPrice
        End Get
    End Property

#Region "ComponentProperties"
    Public ReadOnly Property Description() As String Implements ICanBeModelGenerationOptionComponent.Description
        Get
            Return String.Format("{0} : {1}", PartNumber, Name)
        End Get
    End Property

    Public ReadOnly Property ComponentType() As String Implements ICanBeModelGenerationOptionComponent.Type
        Get
            Return "Accessory"
        End Get
    End Property
#End Region

#End Region

#Region " System.Object Overrides "

    Public Overloads Function Equals(ByVal obj As Accessory) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID) OrElse Equals(obj.PartNumber))
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationAccessory) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID) OrElse Equals(obj.PartNumber))
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetAccessory(ByVal dataReader As SafeDataReader) As ModelGenerationAccessory
        Dim generationAccessory As ModelGenerationAccessory = New ModelGenerationAccessory()
        generationAccessory.Fetch(dataReader)
        Return generationAccessory
    End Function
    Friend Shared Function GetAccessory(ByVal accessory As Accessory) As ModelGenerationAccessory
        Dim generationAccessory As ModelGenerationAccessory = New ModelGenerationAccessory()
        generationAccessory.Create(accessory)
        Return generationAccessory
    End Function
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _fullName = .GetString("FULLNAME")
            _basePrice = Environment.ConvertPrice(CType(.GetValue("BASEPRICE"), Decimal), .GetString("BASECURRENCY"))
            _baseVatPrice = Environment.ConvertPrice(CType(.GetValue("BASEPRICEVAT"), Decimal), .GetString("BASECURRENCY"))
        End With
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub Create(ByVal equipmenItem As EquipmentItem)
        MyBase.Create(equipmenItem)
        With DirectCast(equipmenItem, Accessory)
            _fullName = .FullName
            _basePrice = .Price
            _baseVatPrice = .VatPrice
        End With
    End Sub

    Protected Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        MyBase.AddCommandFields(command)
        command.Parameters.AddWithValue("@TECHITEM", False)
        command.Parameters.AddWithValue("@VISIBLE", True)
    End Sub
#End Region

End Class
<Serializable(), XmlInfo("option")> Public NotInheritable Class ModelGenerationOption
    Inherits ModelGenerationEquipmentItem
    Implements ICanHaveComponentsWithAssets
    Implements IMasterPathObjectReference

#Region " Business Properties & Methods "

    Private _code As String = String.Empty
    Private _suffixOption As Boolean = False
    Private _postProductionOption As Boolean = False
    Private _techItem As Boolean = False
    Private _visible As Boolean = True
    Private WithEvents _extColourApplicabilities As ExteriorColourApplicabilities
    Private WithEvents _upholsteryApplicabilities As UpholsteryApplicabilities
    Private _components As ModelGenerationOptionComponents
    Private _masterPath As String
    Private _parentOptionID As Guid

    Public Overloads Overrides ReadOnly Property Type() As EquipmentType
        Get
            Return EquipmentType.Option
        End Get
    End Property
    Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    Public ReadOnly Property SuffixOption() As Boolean
        Get
            Return _suffixOption
        End Get
    End Property
    Public Property PostProductionOption() As Boolean
        Get
            Return _postProductionOption
        End Get
        Set(value As Boolean)
            If Not _postProductionOption.Equals(value) Then
                _postProductionOption = value
                PropertyHasChanged("PostProductionOption")
            End If
        End Set
    End Property

    Public Property TechnologyItem() As Boolean
        Get
            Return _techItem
        End Get
        Set(ByVal value As Boolean)
            If Not _techItem.Equals(value) Then
                _techItem = value
                PropertyHasChanged("TechnologyItem")
            End If
        End Set
    End Property
    Public Property Visible() As Boolean
        Get
            Return _visible
        End Get
        Set(ByVal value As Boolean)
            If Not _visible.Equals(value) Then
                _visible = value
                PropertyHasChanged("Visible")
            End If
        End Set
    End Property

    Public ReadOnly Property ExteriorColourApplicabilities() As ExteriorColourApplicabilities
        Get
            If _extColourApplicabilities Is Nothing Then _extColourApplicabilities = ExteriorColourApplicabilities.NewExteriorColourApplicabilities(Me)
            Return _extColourApplicabilities
        End Get
    End Property
    Public ReadOnly Property UpholsteryApplicabilities() As UpholsteryApplicabilities
        Get
            If _upholsteryApplicabilities Is Nothing Then _upholsteryApplicabilities = UpholsteryApplicabilities.NewUpholsteryApplicabilities(Me)
            Return _upholsteryApplicabilities
        End Get
    End Property

    Friend ReadOnly Property ComponentsWithAssets() As IEnumerable(Of IHasAssetSet) Implements ICanHaveComponentsWithAssets.Components
        Get
            Return Components.Cast(Of IHasAssetSet)()
        End Get
    End Property

    Public ReadOnly Property Components As ModelGenerationOptionComponents
        Get
            If _components Is Nothing Then _components = ModelGenerationOptionComponents.NewComponents(Me)
            Return _components
        End Get
    End Property

    Public Function GetMappingCodes() As IEnumerable(Of String)
        Return (From value In GetFactoryGenerationOptionValues() Select (value.SMSCode + value.SMSValue).Trim())
    End Function

    Public Function GetFactoryGenerationOptionValues() As IEnumerable(Of FactoryGenerationOptionValueInfo)
        Return (
            From factoryGeneration In Generation.FactoryGenerations
            From mappingLine In factoryGeneration.OptionMapping
            Where mappingLine.Option.ID.Equals(ID) AndAlso Not mappingLine.MarketingIrrelevant And Not mappingLine.Parked
            Select mappingLine.FactoryOptionValue
            ).Distinct()

    End Function

    Public ReadOnly Property MasterPath() As String Implements IMasterPathObjectReference.MasterPath
        Get
            Return _masterPath
        End Get
    End Property


    Public ReadOnly Property ParentOption() As ModelGenerationOption
        Get
            If Not HasParentOption Then Return Nothing
            Dim generationOption As ModelGenerationOption = DirectCast(Generation.Equipment(_parentOptionID), ModelGenerationOption)
            If generationOption Is Nothing Then Throw New ApplicationException(String.Format("Parent can not be found for option '{0}'", Name))
            Return generationOption
        End Get
    End Property
    Public Function GetRootOption() As ModelGenerationOption
        Return GetRootOption(Me)
    End Function
    Private Shared Function GetRootOption(ByVal [option] As ModelGenerationOption) As ModelGenerationOption
        Dim rootOption As ModelGenerationOption
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

    Public ReadOnly Property ChildOptions() As IEnumerable(Of ModelGenerationOption)
        Get
            Return Generation.Equipment.Options().Where(Function(x) Not x.ParentOption Is Nothing AndAlso x.ParentOption.ID.Equals(ID))
        End Get
    End Property

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

    Public Overrides ReadOnly Property SortPath() As String
        Get
            Return If(HasParentOption, String.Format("{0}/{1}", ParentOption.SortPath, MyBase.SortPath), MyBase.SortPath)
        End Get
    End Property

    Private Sub ExteriorColourApplicabilityAdded(exteriorColourApplicability As ExteriorColourApplicability) Handles _extColourApplicabilities.ApplicabilityAdded
        PropertyHasChanged("ExteriorColourApplicabilities")
    End Sub

    Private Sub ExteriorColourApplicabilityRemoved(exteriorColourApplicability As ExteriorColourApplicability) Handles _extColourApplicabilities.ApplicabilityRemoved
        RemoveExteriorColourApplicabilityFromChildOptions(exteriorColourApplicability)
        PropertyHasChanged("ExteriorColourApplicabilities")
    End Sub

    Private Sub RemoveExteriorColourApplicabilityFromChildOptions(ByVal exteriorColourApplicability As ExteriorColourApplicability)
        For Each generationOption As ModelGenerationOption In ChildOptions
            RemoveExteriorColourApplicabilityFromChildOption(generationOption, exteriorColourApplicability)
        Next
    End Sub

    Private Sub RemoveExteriorColourApplicabilityFromChildOption(ByVal generationOption As ModelGenerationOption, ByVal exteriorColourApplicability As ExteriorColourApplicability)
        If generationOption.ExteriorColourApplicabilities.Contains(exteriorColourApplicability.ID) Then
            generationOption.ExteriorColourApplicabilities.Remove(exteriorColourApplicability.ID)
        End If
    End Sub

    Private Sub UpholsteryApplicabilityAdded(upholsteryApplicability As UpholsteryApplicability) Handles _upholsteryApplicabilities.ApplicabilityAdded
        PropertyHasChanged("UpholsteryApplicabilities")
    End Sub

    Private Sub UpholsteryApplicabilityRemoved(upholsteryApplicability As UpholsteryApplicability) Handles _upholsteryApplicabilities.ApplicabilityRemoved
        RemoveUpholsteryApplicabilityFromChildOptions(upholsteryApplicability)
        PropertyHasChanged("UpholsteryApplicabilities")
    End Sub

    Private Sub RemoveUpholsteryApplicabilityFromChildOptions(ByVal upholsteryApplicability As UpholsteryApplicability)
        For Each generationOption As ModelGenerationOption In ChildOptions
            RemoveUpholsteryApplicabilityFromChildOption(generationOption, upholsteryApplicability)
        Next
    End Sub

    Private Sub RemoveUpholsteryApplicabilityFromChildOption(ByVal generationOption As ModelGenerationOption, ByVal upholsteryApplicability As UpholsteryApplicability)
        If generationOption.UpholsteryApplicabilities.Contains(upholsteryApplicability.ID) Then
            generationOption.UpholsteryApplicabilities.Remove(upholsteryApplicability.ID)
        End If
    End Sub

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        MyBase.AddBusinessRules()

        ValidationRules.AddRule(DirectCast(AddressOf ExteriorColoursShouldBeAvailableOnTheParent, RuleHandler), "ExteriorColourApplicabilities")
        ValidationRules.AddRule(DirectCast(AddressOf UpholsteriesShouldBeAvailableOnTheParent, RuleHandler), "UpholsteryApplicabilities")
        ValidationRules.AddRule(DirectCast(AddressOf CannotBeSuffixOptionAndPpoInTheSameTime, RuleHandler), "PostProductionOption")
    End Sub

    Private Shared Function CannotBeSuffixOptionAndPpoInTheSameTime(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim obj = DirectCast(target, ModelGenerationOption)
        If Not (obj.SuffixOption AndAlso obj.PostProductionOption) Then Return True

        e.Description = "An option cannot be a suffix option and a post production option at the same time."

        Return False
    End Function

    Private Shared Function ExteriorColoursShouldBeAvailableOnTheParent(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim obj = DirectCast(target, ModelGenerationOption)
        If obj.Parent Is Nothing Then Return True
        If Not obj.HasParentOption Then Return True
        'if count is 0, then Parent Option has applicabilities for all Exterior Colours
        If obj.ParentOption.ExteriorColourApplicabilities.Count = 0 Then Return True
        e.Description = "An option can only have exterior colours that are available on its parent."

        Return obj.ExteriorColourApplicabilities.All(Function(applicability) obj.ParentOption.ExteriorColourApplicabilities.Contains(applicability.ID))
    End Function

    Private Shared Function UpholsteriesShouldBeAvailableOnTheParent(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim obj = DirectCast(target, ModelGenerationOption)
        If obj.Parent Is Nothing Then Return True
        If Not obj.HasParentOption Then Return True
        'if count is 0, then Parent Option has applicabilities for all upholsteries
        If obj.ParentOption.UpholsteryApplicabilities.Count = 0 Then Return True
        e.Description = "An option can only have upholsteries that are available on its parent."

        Return obj.UpholsteryApplicabilities.All(Function(applicability) obj.ParentOption.UpholsteryApplicabilities.Contains(applicability.ID))
    End Function
    
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        If obj.Length = 0 Then Return False
        If (";" + LocalCode.ToLower() + ";").IndexOf(";" & obj.ToLower & ";", StringComparison.Ordinal) > -1 Then
            Return True
        Else
            Return String.Compare(Code, obj, True) = 0
        End If
    End Function
    Public Overloads Function Equals(ByVal obj As [Option]) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationOption) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetOption(ByVal dataReader As SafeDataReader) As ModelGenerationOption
        Dim generationOption As ModelGenerationOption = New ModelGenerationOption()
        generationOption.Fetch(dataReader)
        Return generationOption
    End Function
    Friend Shared Function GetOption(ByVal [option] As [Option]) As ModelGenerationOption
        Dim generationOption As ModelGenerationOption = New ModelGenerationOption
        generationOption.Create([option])
        Return generationOption
    End Function

#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_extColourApplicabilities Is Nothing) AndAlso Not _extColourApplicabilities.IsValid Then Return False
            If Not (_upholsteryApplicabilities Is Nothing) AndAlso Not _upholsteryApplicabilities.IsValid Then Return False
            If _components IsNot Nothing AndAlso Not _components.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_extColourApplicabilities Is Nothing) AndAlso _extColourApplicabilities.IsDirty Then Return True
            If Not (_upholsteryApplicabilities Is Nothing) AndAlso _upholsteryApplicabilities.IsDirty Then Return True
            If _components IsNot Nothing AndAlso _components.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _parentOptionID = Guid.Empty
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _code = .GetString("INTERNALCODE")
            _suffixOption = .GetBoolean("SUFFIXOPTION")
            _postProductionOption = .GetBoolean("POSTPRODUCTIONOPTION")
            _techItem = .GetBoolean("TECHITEM")
            _visible = .GetBoolean("VISIBLE")
            _masterPath = .GetString("MASTERPATH")
            _parentOptionID = .GetGuid("PARENTEQUIPMENTID")
        End With
        MyBase.FetchFields(dataReader)

        AllowRemove = Not _suffixOption
    End Sub
    Protected Overrides Sub Create(ByVal equipmenItem As EquipmentItem)
        MyBase.Create(equipmenItem)
        _code = DirectCast(equipmenItem, [Option]).Code
        _masterPath = DirectCast(equipmenItem, [Option]).MasterPath
    End Sub

    Protected Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        MyBase.AddCommandFields(command)
        command.Parameters.AddWithValue("@TECHITEM", TechnologyItem)
        command.Parameters.AddWithValue("@VISIBLE", Visible)
        command.Parameters.AddWithValue("@POSTPRODUCTIONOPTION", PostProductionOption)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Not _extColourApplicabilities Is Nothing Then _extColourApplicabilities.Update(transaction)
        If Not _upholsteryApplicabilities Is Nothing Then _upholsteryApplicabilities.Update(transaction)
        If _components IsNot Nothing Then _components.Update(transaction)
    End Sub

#End Region

#Region "Base Object Overrides"
    Protected Friend Overrides Function GetBaseCode() As String
        Return Code
    End Function
#End Region
End Class

<Serializable(), XmlInfo("exteriorcolourtype")> Public NotInheritable Class ModelGenerationExteriorColourType
    Inherits ModelGenerationEquipmentItem

#Region " Business Properties & Methods "

    Private _code As String = String.Empty

    Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property

    Public Overloads Overrides ReadOnly Property Type() As EquipmentType
        Get
            Return EquipmentType.ExteriorColourType
        End Get
    End Property
#End Region

#Region " System.Object Overrides "

    Public Overloads Function Equals(ByVal obj As ExteriorColourType) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationExteriorColourType) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetExteriorColourType(ByVal dataReader As SafeDataReader) As ModelGenerationExteriorColourType
        Dim generationExteriorColourType As ModelGenerationExteriorColourType = New ModelGenerationExteriorColourType()
        generationExteriorColourType.Fetch(dataReader)
        Return generationExteriorColourType
    End Function
#End Region

#Region " Data Access "

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _code = dataReader.GetString("INTERNALCODE")
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        MyBase.AddCommandFields(command)
        command.Parameters.AddWithValue("@TECHITEM", False)
        command.Parameters.AddWithValue("@VISIBLE", False)
    End Sub
#End Region

#Region "Base Object Overrides"
    Protected Friend Overrides Function GetBaseCode() As String
        Return Code
    End Function
#End Region

End Class
<Serializable(), XmlInfo("upholsterytype")> Public NotInheritable Class ModelGenerationUpholsteryType
    Inherits ModelGenerationEquipmentItem

#Region " Business Properties & Methods "

    Private _code As String = String.Empty

    Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property

    Public Overloads Overrides ReadOnly Property Type() As EquipmentType
        Get
            Return EquipmentType.UpholsteryType
        End Get
    End Property
#End Region

#Region " System.Object Overrides "

    Public Overloads Function Equals(ByVal obj As UpholsteryType) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationUpholsteryType) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetUpholsteryType(ByVal dataReader As SafeDataReader) As ModelGenerationUpholsteryType
        Dim generationUpholsteryType As ModelGenerationUpholsteryType = New ModelGenerationUpholsteryType()
        generationUpholsteryType.Fetch(dataReader)
        Return generationUpholsteryType
    End Function
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _code = dataReader.GetString("INTERNALCODE")
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        MyBase.AddCommandFields(command)
        command.Parameters.AddWithValue("@TECHITEM", False)
        command.Parameters.AddWithValue("@VISIBLE", False)
    End Sub
#End Region

#Region "Base Object Overrides"
    Protected Friend Overrides Function GetBaseCode() As String
        Return Code
    End Function
#End Region

End Class