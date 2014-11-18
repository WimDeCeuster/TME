Imports TME.CarConfigurator.Administration.ActiveFilterTool.Enumerations

Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class FilterValues
        Inherits StronglySortedListBase(Of FilterValues, FilterValue)

#Region " Business Properties & Methods "
        Public Property Filter() As ValueListFilter
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, ValueListFilter)
            End Get
            Private Set(value As ValueListFilter)
                SetParent(value)
            End Set
        End Property


#Region " Delegates & Events "
        Friend Delegate Sub FilterValuesChangedHandler()
        Friend Event FilterValuesChanged As FilterValuesChangedHandler


        Private Overloads Sub OnListChanged(sender As Object, e As ComponentModel.ListChangedEventArgs) Handles Me.ListChanged
            RaiseEvent FilterValuesChanged()
        End Sub
        Private Overloads Sub OnRemovingItem(sender As Object, e As Core.RemovingItemEventArgs) Handles Me.RemovingItem
            RaiseEvent FilterValuesChanged()
        End Sub

        Friend Sub OnFilterValuesChanged()
            RaiseEvent FilterValuesChanged()
        End Sub

#End Region


#End Region

#Region " Shared Factory Methods "
        Public Shared Function GetFilterValues(ByVal filter As ValueListFilter) As FilterValues
            Dim values As FilterValues
            If filter.IsNew Then
                values = New FilterValues()
            Else
                values = DataPortal.Fetch(Of FilterValues)(New CustomCriteria(filter))
            End If
            values.Filter = filter
            Return values
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

            Private ReadOnly _vehicleType As VehicleType
            Private ReadOnly _filterCode As String
            Public Sub New(ByVal filter As Filter)
                _vehicleType = filter.VehicleType
                _filterCode = filter.Code
            End Sub
            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@VEHICLETYPE", CType(_vehicleType, Int16))
                command.Parameters.AddWithValue("@FILTERCODE", _filterCode)
            End Sub
        End Class
#End Region


    End Class
End NameSpace