
Imports System.Data.SqlTypes
Imports TME.BusinessObjects.Templates
Imports TME.BusinessObjects.Validation

<Serializable()>
Public Class Markets
    Inherits ContextListBase(Of Markets, Market)

#Region "Business properties & methods"
    Public Shadows Function Add(ByVal countryCode As String) As Market
        Dim newMarket = Market.NewMarket(countryCode)
        MyBase.Add(newMarket)
        Return newMarket
    End Function

    Friend ReadOnly Property EquipmentItem() As EquipmentItem
        Get
            Return DirectCast(Parent, EquipmentItem)
        End Get
    End Property
#End Region

#Region "Shared factory methods"
    Friend Shared Function NewMarkets(ByVal equipmentItem As EquipmentItem) As Markets
        Return InitializeMarkets(equipmentItem, New Markets())
    End Function

    Friend Shared Function GetMarkets(ByVal equipmentItem As EquipmentItem) As Markets
        Return InitializeMarkets(equipmentItem, DataPortal.Fetch(Of Markets)(New EquipmentItemCriteria(equipmentItem)))
    End Function

    Private Shared Function InitializeMarkets(ByVal equipmentItem As EquipmentItem, ByVal markets As Markets) As Markets
        markets.SetParent(equipmentItem)
        Return markets
    End Function

#End Region

#Region "Criteria"
    <Serializable()> Private Class EquipmentItemCriteria
        Inherits CommandCriteria

        Private ReadOnly _id As Guid
        Public Sub New(ByVal equipmentItem As EquipmentItem)
            _id = equipmentItem.ID
            CommandText = "getEquipmentItemMarkets"
        End Sub

        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@EQUIPMENTID", _id)
        End Sub
    End Class
#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
        MarkAsChild()
    End Sub
#End Region

End Class

<Serializable()>
<CommandClassName("EquipmentItemMarkets")>
Public Class Market
    Inherits ContextBusinessBase(Of Market)

#Region "Business properties & methods"
    Private _countryCode As String
    Private _embargoPublic As Date
    Private _embargoNmsc As Date

    Public ReadOnly Property CountryCode() As String
        Get
            Return _countryCode
        End Get
    End Property
    Public Property EmbargoPublic() As Date
        Get
            Return _embargoPublic
        End Get
        Set(value As Date)
            If _embargoPublic.CompareTo(value) = 0 Then Return
            _embargoPublic = value
            PropertyHasChanged("EmbargoPublic")
        End Set
    End Property
    Public Property EmbargoNmsc() As Date
        Get
            Return _embargoNmsc
        End Get
        Set(value As Date)
            If _embargoPublic.CompareTo(value) = 0 Then Return
            _embargoNmsc = value
            PropertyHasChanged("EmbargoNmsc")
        End Set
    End Property

    Private ReadOnly Property EquipmentItem As EquipmentItem
        Get
            Return DirectCast(Parent, Markets).EquipmentItem
        End Get
    End Property

    Protected Overrides Function GetIdValue() As Object
        Return CountryCode
    End Function
#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Value.Unique, RuleHandler), "CountryCode")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, RuleHandler), "CountryCode")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("CountryCode", 2))
    End Sub

#End Region

#Region "Object overrides"

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim otherMarket = TryCast(obj, Market)
        If otherMarket Is Nothing Then Return MyBase.Equals(obj)
        Return Equals(otherMarket.CountryCode)
    End Function

    Public Overrides Function Equals(ByVal otherCountryCode As String) As Boolean
        Return otherCountryCode.Equals(CountryCode, StringComparison.InvariantCultureIgnoreCase)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return CountryCode.GetHashCode()
    End Function

#End Region

#Region "Shared factory methods"
    Friend Shared Function NewMarket(ByVal countryCode As String) As Market
        Dim market = New Market()
        market._countryCode = countryCode
        market.Create()

        Return market
    End Function
#End Region

#Region "Constructors"

    Private Sub New()
        'prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region "Data Access"

    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        EmbargoNmsc = DateTime.MaxValue
        EmbargoPublic = DateTime.MaxValue
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        MyBase.FetchFields(dataReader)
        _countryCode = dataReader.GetString("COUNTRYCODE")
        _embargoPublic = dataReader.GetDateTime("EMBARGOPUBLIC")
        If _embargoPublic = SqlDateTime.MinValue Then
            _embargoPublic = DateTime.MinValue
        End If
        _embargoNmsc = dataReader.GetDateTime("EMBARGONMSC")
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        MyBase.AddInsertCommandFields(command)
        AddCommandFields(command)
    End Sub

    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        MyBase.AddUpdateCommandFields(command)
        AddCommandFields(command)
    End Sub

    Private Sub AddCommandFields(command As SqlCommand)
        command.Parameters.AddWithValue("@EQUIPMENTID", EquipmentItem.ID)
        command.Parameters.AddWithValue("@COUNTRYCODE", CountryCode)
        If (EmbargoPublic = DateTime.MinValue) Then
            command.Parameters.AddWithValue("@EMBARGOPUBLIC", SqlDateTime.MinValue)
        Else
            command.Parameters.AddWithValue("@EMBARGOPUBLIC", EmbargoPublic)
        End If

        command.Parameters.AddWithValue("@EMBARGONMSC", EmbargoNmsc)
    End Sub

    Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
        MyBase.AddDeleteCommandFields(command)
        command.Parameters.AddWithValue("@EQUIPMENTID", EquipmentItem.ID)
        command.Parameters.AddWithValue("@COUNTRYCODE", CountryCode)
    End Sub

#End Region

End Class