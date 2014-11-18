Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class ModelGenerationSubModels
    Inherits StronglySortedListBase(Of ModelGenerationSubModels, ModelGenerationSubModel)

#Region " Business Properties & Methods "

    Friend Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGeneration)
        End Get
        Private Set(ByVal value As ModelGeneration)
            SetParent(value)
        End Set
    End Property

    Default Public Overloads ReadOnly Property Item(ByVal submodelInfo As SubModelInfo) As ModelGenerationSubModel
        Get
            If submodelInfo Is Nothing Then Return Nothing
            Return Me(submodelInfo.ID)
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationSubModels(ByVal generation As ModelGeneration) As ModelGenerationSubModels
        Dim subModels As ModelGenerationSubModels = New ModelGenerationSubModels
        subModels.Generation = generation
        Return subModels
    End Function
    Friend Shared Function GetModelGenerationSubModels(ByVal generation As ModelGeneration) As ModelGenerationSubModels
        Dim subModels As ModelGenerationSubModels = DataPortal.Fetch(Of ModelGenerationSubModels)(New ParentCriteria(generation.ID, "@GENERATIONID"))
        subModels.Generation = generation
        Return subModels
    End Function
    Friend Shared Function GetModelGenerationSubModels(ByVal generation As ModelGeneration, ByVal dataReader As SafeDataReader) As ModelGenerationSubModels
        Dim subModels As ModelGenerationSubModels = New ModelGenerationSubModels
        subModels.Generation = generation
        subModels.Fetch(dataReader)
        Return subModels
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        MarkAsChild()
        AllowNew = Not MyContext.GetContext().IsSlaveRegionCountry
        AllowEdit = AllowNew
        AllowRemove = AllowNew
    End Sub
#End Region

#Region " Data Access "
    Friend Sub Synchronize()
        If Not AllowEdit Then Exit Sub

        For Each subModel In Me
            Dim availableCars = Generation.Cars.Where(Function(car) car.SubModelID.Equals(subModel.ID)).ToList()
            subModel.Activated = availableCars.Any()
            subModel.Approved = availableCars.Any(Function(car) car.Approved)
            subModel.Preview = availableCars.Any(Function(car) car.Preview)
        Next
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class ModelGenerationSubModel
    Inherits LocalizeableBusinessBase
    Implements ISortedIndex
    Implements ISortedIndexSetter
    Implements IUpdatableAssetSet
    Implements IOwnedBy
    Implements ILinks

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _owner As String = String.Empty
    Private _status As Integer
    Private _index As Integer
    Private _assetSet As AssetSet
    Private _links As Links
    Private _equipment As ModelGenerationSubModelEquipment
    Private _fittings As ModelGenerationSubModelDefaultFittings

    Public ReadOnly Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationSubModels).Generation
        End Get
    End Property
    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            If _code <> value Then
                _code = value
                If CanWriteProperty("LocalCode") Then _localCode = value
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
    Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return _owner
        End Get
    End Property
    Public ReadOnly Property Index() As Integer Implements ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not _index.Equals(value) Then
                _index = value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property
    Public Property Activated() As Boolean
        Get
            Return ((_status And Status.AvailableToNMSCs) = Status.AvailableToNMSCs)
        End Get
        Friend Set(ByVal value As Boolean)
            If Not value.Equals(Activated) Then
                If Activated Then
                    _status -= Status.AvailableToNmscs
                Else
                    _status += Status.AvailableToNmscs
                End If
                PropertyHasChanged("Activated")
            End If
        End Set
    End Property
    Public Property Approved() As Boolean
        Get
            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Friend Set(ByVal value As Boolean)
            If Not value.Equals(Approved) Then
                If Approved Then
                    _status -= Status.ApprovedForLive
                    If Not Declined Then _status += Status.Declined
                Else
                    _status += Status.ApprovedForLive
                    If Declined Then _status -= Status.Declined
                End If
                PropertyHasChanged("Approved")
            End If
        End Set
    End Property
    Public Property Declined() As Boolean
        Get
            Return ((_status And Status.Declined) = Status.Declined)
        End Get
        Friend Set(ByVal value As Boolean)
            If Not value.Equals(Declined) Then
                If Declined Then
                    _status -= Status.Declined
                    If Not Approved Then _status += Status.ApprovedForLive
                Else
                    _status += Status.Declined
                    If Approved Then _status -= Status.ApprovedForLive
                End If
                PropertyHasChanged("Declined")
            End If
        End Set
    End Property
    Public Property Preview() As Boolean
        Get
            Return ((_status And Status.ApprovedForPreview) = Status.ApprovedForPreview)
        End Get
        Friend Set(ByVal value As Boolean)
            If Not value.Equals(Preview) Then
                If Preview Then
                    _status -= Status.ApprovedForPreview
                Else
                    _status += Status.ApprovedForPreview
                End If
                PropertyHasChanged("Preview")
            End If
        End Set
    End Property
    Public ReadOnly Property Visible() As Boolean
        Get
            Return Activated AndAlso (Approved OrElse Preview)
        End Get
    End Property
    Public ReadOnly Property AssetSet() As AssetSet Implements IHAsAssetSet.AssetSet
        Get
            If _assetSet Is Nothing Then
                _assetSet = AssetSet.NewAssetSet(Me)
            End If
            Return _assetSet
        End Get
    End Property

    Public Sub ChangeReference(ByVal updatedAssetSet As AssetSet) Implements IUpdatableAssetSet.ChangeReference
        _assetSet = updatedAssetSet
    End Sub

    Public ReadOnly Property Links() As Links Implements ILinks.Links
        Get
            If _links Is Nothing Then _links = Links.GetLinks(Me)
            Return _links
        End Get
    End Property
    Public ReadOnly Property Equipment() As ModelGenerationSubModelEquipment
        Get
            If _equipment Is Nothing Then _equipment = ModelGenerationSubModelEquipment.GetEquipment(Me)
            Return _equipment
        End Get
    End Property
    Public ReadOnly Property DefaultFittings() As ModelGenerationSubModelDefaultFittings
        Get
            If _fittings Is Nothing Then _fittings = ModelGenerationSubModelDefaultFittings.GetFittings(Me)
            Return _fittings
        End Get
    End Property


    Public Function GetInfo() As SubModelInfo
        Return SubModelInfo.GetSubModelInfo(Me)
    End Function

    Public Overloads Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If MyContext.GetContext().IsGlobal Then Return MyBase.CanWriteProperty(propertyName)
        If Owner.Equals(MyContext.GetContext().CountryCode(), StringComparison.InvariantCultureIgnoreCase) Then Return MyBase.CanWriteProperty(propertyName)

        Select Case propertyName
            Case "Index", "Activated", "Approved", "Declined", "Preview", "LocalCode"
                Return MyBase.CanWriteProperty(propertyName)
            Case Else
                Return False
        End Select

    End Function
    Private Sub MyPropertyChanged(ByVal sender As Object, ByVal e As ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If e.PropertyName.Equals("LocalCode", StringComparison.InvariantCultureIgnoreCase) Then
            If Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) Then
                Code = LocalCode
            End If
        End If
    End Sub

    Public Function Cars() As IEnumerable(Of Car)
        Return (From x In Generation.Cars Where x.SubModel.Equals(Me))
    End Function
#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))
    End Sub
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationSubModel) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As SubModelInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        Return String.Compare(Code, obj, True) = 0
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationSubModel Then
            Return Equals(DirectCast(obj, ModelGenerationSubModel))
        ElseIf TypeOf obj Is SubModelInfo Then
            Return Equals(DirectCast(obj, SubModelInfo))
        ElseIf TypeOf obj Is String Then
            Return Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_assetSet Is Nothing) AndAlso Not _assetSet.IsValid Then Return False
            If Not (_links Is Nothing) AndAlso Not _links.IsValid Then Return False
            If Not (_equipment Is Nothing) AndAlso Not _equipment.IsValid Then Return False
            If Not (_fittings Is Nothing) AndAlso Not _fittings.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_assetSet Is Nothing) AndAlso _assetSet.IsDirty Then Return True
            If Not (_links Is Nothing) AndAlso _links.IsDirty Then Return True
            If Not (_equipment Is Nothing) AndAlso _equipment.IsDirty Then Return True
            If Not (_fittings Is Nothing) AndAlso _fittings.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
        AlwaysUpdateSelf = True
        With MyContext.GetContext()
            AllowNew = Not .IsRegionCountry OrElse .IsMainRegionCountry
            AllowEdit = AllowNew
            AllowRemove = AllowNew
        End With
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _status = (Status.Declined)
        _owner = MyContext.GetContext().CountryCode
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _owner = .GetString("OWNER")
            _status = .GetInt16("STATUSID")
            _index = .GetInt16("SORTORDER")
            _assetSet = AssetSet.GetAssetSet(Me, dataReader)
        End With
        MyBase.FetchFields(dataReader)

        AllowRemove = (MyContext.GetContext().IsGlobal()) OrElse (String.Compare(MyContext.GetContext().CountryCode, Owner, True) = 0)
        If AllowRemove AndAlso SupportsLocalCode Then
            _localCode = _code
        End If
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@INTERNALCODE", Code)
        command.Parameters.AddWithValue("@LOCALCODE", LocalCode)
        command.Parameters.AddWithValue("@SHORTNAME", Name)
        command.Parameters.AddWithValue("@GENERATIONID", Generation.ID)
        command.Parameters.AddWithValue("@STATUSID", _status)
        command.Parameters.AddWithValue("@SORTORDER", Index)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not _assetSet Is Nothing Then _assetSet.Update(transaction)
        If Not _links Is Nothing Then _links.Update(transaction)
        If Not _equipment Is Nothing Then _equipment.Update(transaction)
        If Not _fittings Is Nothing Then _fittings.Update(transaction)

        MyBase.UpdateChildren(transaction)
    End Sub

#End Region

#Region " Base Object Overrides "

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
        Return Code
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.SUBMODEL
        End Get
    End Property
#End Region

    
End Class
<Serializable(), XmlInfo("submodel")> Public NotInheritable Class SubModelInfo

#Region " Business Properties & Methods "
    Private _id As Guid
    Private _name As String

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    Public Shared ReadOnly Property Empty() As SubModelInfo
        Get
            Return New SubModelInfo()
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

    Public Overloads Function Equals(ByVal obj As ModelGenerationSubModel) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As SubModelInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is SubModelInfo Then
            Return Equals(DirectCast(obj, SubModelInfo))
        ElseIf TypeOf obj Is ModelGenerationSubModel Then
            Return Equals(DirectCast(obj, ModelGenerationSubModel))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is SubModelInfo Then
            Return DirectCast(objA, SubModelInfo).Equals(objB)
        ElseIf TypeOf objB Is SubModelInfo Then
            Return DirectCast(objB, SubModelInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetSubModelInfo(ByVal dataReader As SafeDataReader) As SubModelInfo
        Dim subModel As SubModelInfo = New SubModelInfo
        subModel.Fetch(dataReader)
        Return subModel
    End Function
    Friend Shared Function GetSubModelInfo(ByVal generationsubmodel As ModelGenerationSubModel) As SubModelInfo
        Dim subModel As SubModelInfo = New SubModelInfo
        subModel.Fetch(generationsubmodel)
        Return subModel
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
            _id = .GetGuid("SUBMODELID")
            _name = .GetString("SUBMODELNAME")
        End With
    End Sub
    Private Sub Fetch(ByVal submodel As ModelGenerationSubModel)
        With submodel
            _id = .ID
            _name = .Name
        End With
    End Sub

#End Region

End Class