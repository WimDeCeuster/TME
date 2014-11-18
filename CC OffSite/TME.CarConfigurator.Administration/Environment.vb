Imports TME.CarConfigurator.Administration.Enums

Public Class Environment

    Private Sub New()
        'Prevent direct creation
    End Sub

#Region " Business Properties & Methods "
    Private Shared _connectionTimeout As Integer = 30


    Public Shared ReadOnly Property ConnectionTimeout() As Integer
        Get
            If System.Configuration.ConfigurationManager.AppSettings().AllKeys.Contains("ConnectionTimeout") Then
                _connectionTimeout = CInt(System.Configuration.ConfigurationManager.AppSettings()("ConnectionTimeout"))
            End If
            Return _connectionTimeout
        End Get
    End Property

#End Region

    Friend Shared Function ConnectionScope() As String
        If MyContext.GetContext().Brand.Equals(String.Empty) Then
            Throw New Exceptions.NoBrandSelected
        Else
            Return MyContext.GetContext().Brand
        End If
    End Function
    Public Const ConnectionEntity As String = "CarConfigurator.Administration"

    Friend Const DBAccessoryCode As String = "A"
    Friend Const DBOptionCode As String = "E"
    Friend Const DBExteriorColourTypeCode As String = "P"   'seat material
    Friend Const DBUpholsteryTypeCode As String = "S"       'paint
    Public Const GlobalCountryCode As String = "ZZ"
    Public Const EuropeCountryCode As String = "EU"
    
    Public Shared Function GetObjects(ByVal entity As Entity) As IList
        Select Case entity
            Case entity.EXTERIORCOLOUR
                Return ExteriorColours.GetExteriorColours()
            Case entity.EXTERIORCOLOURTYPE
                Return ExteriorColourTypes.GetExteriorColourTypes()
            Case entity.UPHOLSTERY
                Return Upholsteries.GetUpholsteries()
            Case entity.UPHOLSTERYTYPE
                Return UpholsteryTypes.GetUpholsteryTypes()

            Case entity.UNIT
                Return Units.GetUnits()

            Case entity.WHEELDRIVE
                Return WheelDrives.GetWheelDrives()
            Case entity.TRANSMISSION
                Return Transmissions.GetTransmissions()
            Case entity.TRANSMISSIONTYPE
                Return TransmissionTypes.GetTransmissionTypes()
            Case entity.ENGINE
                Return Engines.GetEngines()
            Case entity.ENGINECATEGORY
                Return EngineCategories.GetEngineCategories()
            Case entity.FUELTYPE
                Return FuelTypes.GetFuelTypes()
            Case entity.BODY
                Return BodyTypes.GetBodyTypes()
            Case entity.STEERING
                Return Steerings.GetSteerings()

            Case entity.PROMOTION
                Return Promotions.Promotions.GetPromotions()

            Case entity.ACCESSORY
                Return EquipmentItems.GetEquipmentItems(EquipmentType.Accessory)

            Case Else
                Throw New NotSupportedException("TME.CarConfigurator.Administration.Environment.GetObjects method does not support the type " + entity.ToString())
        End Select
    End Function

    Friend Shared Function ConvertPrice(ByVal value As Decimal, ByVal sourceCurrency As String) As Decimal
        If value = 0D Then Return value
        If MyContext.GetContext().Currency.Equals(sourceCurrency) Then Return value

        Dim money As TMME.Common.DataObjects.Money = New TMME.Common.DataObjects.Money(value, TMME.Common.DataObjects.Currency.GetCurrency(sourceCurrency))
        money.Currency = MyContext.GetContext().Currency
        Return money.Value

    End Function

End Class