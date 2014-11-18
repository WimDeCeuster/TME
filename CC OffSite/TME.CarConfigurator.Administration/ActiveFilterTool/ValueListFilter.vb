Imports TME.CarConfigurator.Administration.ActiveFilterTool.Enumerations

Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class ValueListFilter
        Inherits Filter

#Region " Business Properties & Methods "

        Private WithEvents _values As FilterValues

        Public ReadOnly Property Values() As FilterValues
            Get
                If _values Is Nothing Then _values = FilterValues.GetFilterValues(Me)
                Return _values
            End Get
        End Property
        Private Sub OnFilterValuesChanged() Handles _values.FilterValuesChanged
            ValidationRules.CheckRules("Visible")
        End Sub

        Public Overrides ReadOnly Property FilterType() As FilterType
            Get
                Return FilterType.ValueListFilter
            End Get
        End Property

        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf ValuesValid, Validation.RuleHandler), "Visible")
        End Sub
        Private Shared Function ValuesValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
            Dim filter As ValueListFilter = DirectCast(target, ValueListFilter)
            If Not filter.Visible Then Return True

            e.Description = ""

            If filter.Values.Count < 2 Then
                e.Description = "You need to have at least 2 possible values for a visible filter."
                Return False
            End If

            If Not filter.Values.Any(Function(value) value.Default) Then
                e.Description = "You need have a default value selected for a visible filter."
                Return False
            End If

            Return True
        End Function

#End Region

#Region " Framework Overrides "
        Public Overloads Overrides ReadOnly Property IsValid() As Boolean
            Get
                If Not MyBase.IsValid Then Return False
                If Not (_values Is Nothing) AndAlso Not _values.IsValid Then Return False
                Return True
            End Get
        End Property
        Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
            Get
                If MyBase.IsDirty Then Return True
                If Not (_values Is Nothing) AndAlso _values.IsDirty Then Return True
                Return False
            End Get
        End Property
#End Region

#Region " Data Access "

        Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            MyBase.UpdateChildren(transaction)
            If Not _values Is Nothing Then _values.Update(transaction)
        End Sub

#End Region


    End Class

End Namespace