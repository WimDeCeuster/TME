Imports TME.CarConfigurator.Administration.Enums
Imports TME.CarConfigurator.Administration.ActiveFilterTool.Enumerations

Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class PriceFilter
        Inherits DecimalValueFilter

#Region " Business Properties & Methods "
        Private _priceType As PriceType = PriceType.InVat

        Public Property PriceType() As PriceType
            Get
                Return _priceType
            End Get
            Set(ByVal value As PriceType)
                If Not value.Equals(_priceType) Then
                    _priceType = value
                    PropertyHasChanged("PriceType")
                End If
            End Set
        End Property
        Public Overrides ReadOnly Property FilterType() As FilterType
            Get
                Return FilterType.PriceFilter
            End Get
        End Property

#End Region

#Region " Business & Validation Rules "

        Protected Overrides Sub AddBusinessRules()
            MyBase.AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf PriceTypeValid, Validation.RuleHandler), "PriceType")
        End Sub
        Private Shared Function PriceTypeValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
            If DirectCast(target, PriceFilter).PriceType = 0 Then
                e.Description = "You need to select a price mode for the filter"
                Return False
            End If
            Return True
        End Function
#End Region

#Region " Data Access "

        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            MyBase.FetchFields(dataReader)
            _priceType = CType(dataReader.GetValue("PRICETYPE"), PriceType)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            MyBase.AddUpdateCommandFields(command)
            command.Parameters.AddWithValue("@PRICETYPE", PriceType)
            command.Parameters.AddWithValue("@TECHSPECID", DBNull.Value)
        End Sub

#End Region

    End Class
End Namespace