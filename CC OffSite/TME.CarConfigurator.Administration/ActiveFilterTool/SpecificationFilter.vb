Imports TME.CarConfigurator.Administration.ActiveFilterTool.Enumerations

Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class SpecificationFilter
        Inherits DecimalValueFilter

#Region " Business Properties & Methods "
        Private _specificationInfo As SpecificationInfo = SpecificationInfo.Empty

        Public Property SpecificationInfo() As SpecificationInfo
            Get
                Return _specificationInfo
            End Get
            Set(ByVal value As SpecificationInfo)
                If Not value.Equals(_specificationInfo) Then
                    _specificationInfo = value
                    PropertyHasChanged("SpecificationInfo")
                End If
            End Set
        End Property
        Public Overrides ReadOnly Property FilterType() As FilterType
            Get
                Return FilterType.SpecificationFilter
            End Get
        End Property

#End Region

#Region " Business & Validation Rules "
        Protected Overrides Sub AddBusinessRules()
            MyBase.AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf SpecificationValid, Validation.RuleHandler), "SpecificationInfo")
        End Sub
        Private Shared Function SpecificationValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
            Dim filter As SpecificationFilter = DirectCast(target, SpecificationFilter)
            e.Description = ""

            If filter.SpecificationInfo.ID.Equals(Guid.Empty) Then
                e.Description = "You need to assign a Specification to the filter"
                Return False
            End If
            If Not filter.SpecificationInfo.Approved Then
                e.Description = "The Specification '" & filter.SpecificationInfo.Name & "' is not approved."
                Return False
            End If
            If Not filter.SpecificationInfo.Compareable Then
                e.Description = "The Specification '" & filter.SpecificationInfo.Name & "' is not compareable."
                Return False
            End If

            Return True
        End Function
#End Region

#Region " Data Access "

        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            MyBase.FetchFields(dataReader)
            _specificationInfo = SpecificationInfo.GetSpecificationInfo(dataReader)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            MyBase.AddUpdateCommandFields(command)
            command.Parameters.AddWithValue("@PRICETYPE", DBNull.Value)
            command.Parameters.AddWithValue("@TECHSPECID", SpecificationInfo.ID)
        End Sub

#End Region

    End Class
End Namespace