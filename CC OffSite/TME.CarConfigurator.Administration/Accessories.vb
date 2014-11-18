Imports TME.BusinessObjects.Templates.Exceptions
Imports TME.BusinessObjects.Server
Imports TME.CarConfigurator.Administration.State.Accessory
Imports TME.CarConfigurator.Administration.Components
Imports TME.CarConfigurator.Administration.Enums
Imports TME.BusinessObjects.Validation
Imports TME.CarConfigurator.Administration.Extensions.StringExtensions

<Serializable()> Public NotInheritable Class Accessories
    Inherits ContextUniqueGuidListBase(Of Accessories, Accessory)

#Region "Business Properties & Methods"

    Public Overloads Function Add() As Accessory
        Dim newAccessory = Accessory.NewAccessory()
        Add(newAccessory)
        Return newAccessory
    End Function

    Public Overloads Function Add(ByVal accessory As Accessory) As Accessory
        MyBase.Add(accessory)
        Return accessory
    End Function

#End Region

#Region "Shared Factory Methods"

    Public Shared Function GetAccessoriesByAccessoryCategory(ByVal accessoryCategoryId As Guid) As Accessories
        Return BusinessObjects.DataPortal.Fetch(Of Accessories)(New AccessoryCategoryCriteria(accessoryCategoryId))
    End Function

    Public Shared Function GetLocalAccessories() As Accessories
        Return BusinessObjects.DataPortal.Fetch(Of Accessories)(New LocalAccessoryCriteria())
    End Function

#Region "Criteria"
    Private Class AccessoryCategoryCriteria
        Inherits Criteria

        Private Property Id() As Guid

        Public Sub New(ByVal accessoryCategoryId As Guid)
            Id = accessoryCategoryId
        End Sub

        Public Overrides Sub AddCommandFields(ByVal command As SqlCommand)
            MyBase.AddCommandFields(command)

            command.Parameters.AddWithValue("ACCESSORYCATEGORYID", Id.GetDbValue())
        End Sub
    End Class
    Private Class LocalAccessoryCriteria
        Inherits Criteria

        Public Sub New()
        End Sub

        Public Overrides Sub AddCommandFields(ByVal command As SqlCommand)
            MyBase.AddCommandFields(command)

            command.CommandText = "getLocalAccessories"
        End Sub
    End Class
#End Region

#End Region

#Region "Constructors"

    Private Sub New()
        'prevent direct creation
    End Sub

#End Region
End Class

<Serializable()> Public NotInheritable Class Accessory
    Inherits EquipmentItem

#Region " Business Properties & Methods "

    Private _fullName As String = String.Empty
    Private _price As Decimal = 0D
    Private _vatPrice As Decimal = 0D
    Private _exteriorColourApplicabilities As ExteriorColourApplicabilities
    Private _interiorColourApplicabilities As InteriorColourApplicabilities
    Private _components As AccessoryComponents
    Private _state As AccessoryState
    Private _accessoryCategory As AccessoryCategoryInfo
    Private _generations As AccessoryGenerations

    Public ReadOnly Property IsLegacy() As Boolean
        Get
            Return MasterID.Equals(Guid.Empty)
        End Get
    End Property

    Public Property FullName() As String
        Get
            Return _fullName
        End Get
        Set(ByVal value As String)
            If _fullName.IsSameAs(value) Then Return

            _fullName = value
            PropertyHasChanged("FullName")
        End Set
    End Property
    Public Property Price() As Decimal
        Get
            Return _price
        End Get
        Set(ByVal value As Decimal)
            If Not _price.Equals(value) Then
                _price = value
                PropertyHasChanged("Price")
            End If
        End Set
    End Property
    Public Property VatPrice() As Decimal
        Get
            Return _vatPrice
        End Get
        Set(ByVal value As Decimal)
            If Not _vatPrice.Equals(value) Then
                _vatPrice = value
                PropertyHasChanged("VatPrice")
            End If
        End Set
    End Property

    Public ReadOnly Property ExteriorColourApplicabilities() As ExteriorColourApplicabilities
        Get
            If _exteriorColourApplicabilities IsNot Nothing Then Return _exteriorColourApplicabilities
            _exteriorColourApplicabilities = If(IsNew,
                                                ExteriorColourApplicabilities.NewExteriorColourApplicabilities(Me),
                                                ExteriorColourApplicabilities.GetExteriorColourApplicabilities(Me))
            Return _exteriorColourApplicabilities
        End Get
    End Property

    Public ReadOnly Property InteriorColourApplicabilities() As InteriorColourApplicabilities
        Get
            If _interiorColourApplicabilities IsNot Nothing Then Return _interiorColourApplicabilities
            _interiorColourApplicabilities = If(IsNew,
                                                InteriorColourApplicabilities.NewInteriorColourApplicabilities(Me),
                                                InteriorColourApplicabilities.GetInteriorColourApplicabilities(Me))
            Return _interiorColourApplicabilities
        End Get
    End Property

    Public Overloads Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        Select Case propertyName.ToLowerInvariant()
            Case "price", "vatprice"
                Return True
            Case Else
                Return MyBase.CanWriteProperty(propertyName)
        End Select
    End Function

    Public Property State() As AccessoryState
        Get
            Return _state
        End Get
        Friend Set(value As AccessoryState)
            If _state.DBValue = value.DBValue Then Return
            _state = value
            PropertyHasChanged("State")
        End Set
    End Property


    Public ReadOnly Property Components() As AccessoryComponents
        Get
            If _components IsNot Nothing Then Return _components
            _components = If(IsNew, AccessoryComponents.NewAccessoryComponents(Me), AccessoryComponents.GetAccessoryComponents(Me))
            Return _components
        End Get
    End Property

    Public Property AccessoryCategory() As AccessoryCategoryInfo
        Get
            Return _accessoryCategory
        End Get
        Set(value As AccessoryCategoryInfo)
            If _accessoryCategory Is Nothing AndAlso value Is Nothing Then Return
            If _accessoryCategory IsNot Nothing AndAlso _accessoryCategory.Equals(value) Then Return

            _accessoryCategory = If(value, AccessoryCategoryInfo.Empty)
            PropertyHasChanged("AccessoryCategory")
        End Set
    End Property
    Private Overloads Sub OnPropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If (e.PropertyName.Equals("Owner", StringComparison.InvariantCultureIgnoreCase)) Then
            ValidationRules.CheckRules("AccessoryCategory")
            Return
        End If

    End Sub
    Public ReadOnly Property Generations() As AccessoryGenerations
        Get
            _generations = If(_generations, AccessoryGenerations.NewAccessoryGenerations(DirectCast(Fittings, AccessoryFittings)))
            Return _generations
        End Get
    End Property

    Public Function IsPack() As Boolean
        Return Components.Any()
    End Function
    Public Function IsGenuine() As Boolean
        Return "ZZ".Equals(Owner, StringComparison.InvariantCultureIgnoreCase)
    End Function

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        MyBase.AddBusinessRules()

        ValidationRules.AddRule(DirectCast(AddressOf AccessoryCategoryRequired, RuleHandler), "AccessoryCategory")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, RuleHandler), "PartNumber")
        'ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("PartNumber", 14))
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("FullName", 255))
        ValidationRules.AddRule(DirectCast(AddressOf OwnerIsInMarketList, RuleHandler), "Owner")
    End Sub

    Private Shared Function AccessoryCategoryRequired(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim accessory = DirectCast(target, Accessory)

        'For legacy accessories we don't AccessoryCategory is not required
        If accessory.IsLegacy Then Return True

        e.Description = String.Format("Accessory {0} is a local accessory, hence can not have an accessory category associated with it", accessory.PartNumber)

        If Not accessory.Owner.Equals("ZZ") Then Return accessory.AccessoryCategory.IsEmpty() 'local accessories don't need to have an accessory category

        e.Description = String.Format("Accessory {0} is a genuine accessory, hence should have an accessory category assigned.", accessory.PartNumber)

        Return Not accessory.AccessoryCategory.IsEmpty()
    End Function


    Private Shared Function OwnerIsInMarketList(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim accessory = DirectCast(target, Accessory)

        If accessory.IsDraft() OrElse Not MyContext.GetContext.Countries(accessory.Owner).LocalizedInA2A Then Return True 'the item isn't localized in A2A => should not be in the Markets list

        e.Description = "The owner of the equipment item should be present in the list of markets of that item." 'unless the item is owned by ZZ

        Return accessory.Owner = "ZZ" OrElse accessory.Markets.Contains(accessory.Owner)
    End Function

    Private Function IsDraft() As Boolean
        Return State.DBValue = AccessoryState.StateValue.Draft
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Function Equals(ByVal obj As Accessory) As Boolean
        Return Not (obj Is Nothing) AndAlso (ID.Equals(obj.ID) OrElse Equals(obj.PartNumber))
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        Return PartNumbers.AreIdentical(PartNumber, obj)
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewAccessory() As Accessory
        Dim theAccessory As Accessory = New Accessory()
        theAccessory.Create()
        theAccessory.MarkAsChild()
        Return theAccessory
    End Function
    Friend Shared Function GetAccessory(ByVal dataReader As SafeDataReader) As Accessory
        Dim theAccessory As Accessory = New Accessory
        theAccessory.Fetch(dataReader)
        theAccessory.MarkAsChild()
        Return theAccessory
    End Function
    Public Shared Function GetAccessory(ByVal accessoryId As Guid) As Accessory
        Return BusinessObjects.DataPortal.Fetch(Of Accessory)(New Criteria(accessoryId))
    End Function

    Public Shared Function FindByMasterID(ByVal masterID As Guid) As Accessory
        Try
            Return BusinessObjects.DataPortal.Fetch(Of Accessory)(New MasterCriteria(masterID))
        Catch exception As BusinessObjects.DataPortalException
            If exception.InnerException IsNot Nothing AndAlso
                TypeOf exception.InnerException Is CallMethodException AndAlso
                exception.InnerException.InnerException IsNot Nothing AndAlso
                TypeOf exception.InnerException.InnerException Is NoDataReturnedException Then

                Return Nothing
            End If
            Throw
        End Try
    End Function

    Private Class MasterCriteria
        Inherits CommandCriteria
        Private _masterID As Guid

        Public Sub New(ByVal masterID As Guid)
            CommandText = "getAccessoryByMaster"
            _masterID = masterID
        End Sub

        Public Overrides Sub AddCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@MASTERID", _masterID.GetDbValue())
        End Sub
    End Class

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MyBase.New()
        Type = EquipmentType.Accessory
    End Sub
#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If _exteriorColourApplicabilities IsNot Nothing AndAlso Not _exteriorColourApplicabilities.IsValid Then Return False
            If _interiorColourApplicabilities IsNot Nothing AndAlso Not _interiorColourApplicabilities.IsValid Then Return False
            If _generations IsNot Nothing AndAlso Not _generations.IsValid Then Return False
            If _components IsNot Nothing AndAlso Not _components.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If _exteriorColourApplicabilities IsNot Nothing AndAlso _exteriorColourApplicabilities.IsDirty Then Return True
            If _interiorColourApplicabilities IsNot Nothing AndAlso _interiorColourApplicabilities.IsDirty Then Return True
            If _generations IsNot Nothing AndAlso _generations.IsDirty Then Return True
            If _components IsNot Nothing AndAlso _components.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region

#Region " Data Access "

    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _state = AccessoryState.NewState(Me)
        _accessoryCategory = AccessoryCategoryInfo.Empty
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _fullName = .GetString("FULLNAME")
            _price = Environment.ConvertPrice(CType(.GetValue("PRICE"), Decimal), .GetString("CURRENCY"))
            _vatPrice = Environment.ConvertPrice(CType(.GetValue("PRICEVAT"), Decimal), .GetString("CURRENCY"))
        End With

        _state = AccessoryState.GetState(Me, dataReader)
        _accessoryCategory = AccessoryCategoryInfo.GetAccessoryCategoryInfo(dataReader)

        MyBase.FetchFields(dataReader)
    End Sub


    Protected Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        MyBase.AddCommandFields(command)
        With command
            .Parameters.AddWithValue("@FULLNAME", FullName)
            .Parameters.AddWithValue("@PRICE", _price)
            .Parameters.AddWithValue("@PRICEVAT", _vatPrice)
            .Parameters.AddWithValue("@CURRENCY", MyContext.GetContext().Currency.Code)
            .Parameters.AddWithValue("@STATE", State.DBValue)
            .Parameters.AddWithValue("@ACCESSORYCATEGORYID", AccessoryCategory.ID.GetDbValue())
        End With
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If _exteriorColourApplicabilities IsNot Nothing Then _exteriorColourApplicabilities.Update(transaction)
        If _interiorColourApplicabilities IsNot Nothing Then _interiorColourApplicabilities.Update(transaction)
        If _components IsNot Nothing Then _components.Update(transaction)
        If _generations IsNot Nothing Then _generations.Update(transaction)
    End Sub
#End Region

End Class