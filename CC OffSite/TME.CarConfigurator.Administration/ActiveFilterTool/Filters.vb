Imports TME.CarConfigurator.Administration.ActiveFilterTool.Enumerations

Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class Filters
        Inherits ContextListBase(Of Filters, Filter)

#Region " Business Properties & Methods "
        Private _type As VehicleType
        Public Property VehicleType() As VehicleType
            Get
                Return _type
            End Get
            Private Set(value As VehicleType)
                _type = value
            End Set
        End Property
#End Region

#Region " Shared Factory Methods "

        Public Shared Function GetFilters(ByVal type As VehicleType) As Filters
            Dim filters = DataPortal.Fetch(Of Filters)(New CustomCriteria(type))
            filters.VehicleType = type
            For Each filter In filters
                filter.CheckRules()
            Next
            Return filters
        End Function

#End Region

#Region " Constructors "

        Private Sub New()
            'Prevent direct creation
            AllowRemove = False
            AllowNew = False
        End Sub

#End Region

#Region " Criteria "

        <Serializable()>
        Private Class CustomCriteria
            Inherits CommandCriteria

            'Add Data Portal criteria here
            Private ReadOnly _vehicleType As VehicleType

            Public Sub New(ByVal type As VehicleType)
                _vehicleType = type
            End Sub

            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@VEHICLETYPE", CType(_vehicleType, Int16))
            End Sub
        End Class

#End Region

#Region " Data Access "

        Protected Overrides Function GetObject(ByVal dataReader As Common.Database.SafeDataReader) As Filter
            Return (Filter.GetFilter(dataReader))
        End Function

#End Region

    End Class
    <Serializable()> Public MustInherit Class Filter
        Inherits ContextBusinessBase(Of Filter)

#Region " Business Properties & Methods "

        Private _vehicleType As VehicleType
        Private _code As String
        Private _label As String = String.Empty
        Private _visible As Boolean = False


        Public ReadOnly Property VehicleType() As VehicleType
            Get
                Return _vehicleType
            End Get
        End Property
        Public MustOverride ReadOnly Property FilterType() As FilterType

        Public ReadOnly Property Code() As String
            Get
                Return _code
            End Get
        End Property
        Public Property Label() As String
            Get
                Return _label
            End Get
            Set(ByVal value As String)
                If _label <> value Then
                    _label = value
                    PropertyHasChanged("Label")
                End If
            End Set
        End Property
        Public Property Visible() As Boolean
            Get
                Return _visible
            End Get
            Set(ByVal value As Boolean)
                If Not value.Equals(_visible) Then
                    _visible = value
                    PropertyHasChanged("Visible")
                End If
            End Set
        End Property


#End Region

#Region " Business & Validation Rules "

        Friend Sub CheckRules()
            ValidationRules.CheckRules()
        End Sub
        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, Validation.RuleHandler), "Label")
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, Validation.RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("Label", 255))
        End Sub
#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return Label
        End Function
        Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
            Return String.Compare(obj, Code, StringComparison.InvariantCultureIgnoreCase) = 0
        End Function
#End Region

#Region " Framework Overrides "
        Protected Overrides Function GetIdValue() As Object
            Return String.Format("{0}-{1}", VehicleType, Code)
        End Function
#End Region

#Region " Shared Factory Methods "

        Public Shared Function GetFilter(ByVal vehicleType As VehicleType, ByVal filterCode As String) As Filter
            Dim type As FilterType
            If Not FilterType.TryParse(ContextCommand.ExecuteScalar(New GetFilterType(vehicleType, filterCode)).ToString(), type) Then Throw New ApplicationException(String.Format("Unkown Filter Type for {0} {1}", vehicleType, filterCode))

            Dim filter As Filter
            Select Case type
                Case FilterType.PriceFilter
                    filter = DirectCast(DataPortal.Fetch(New Criteria(New PriceFilter().GetType(), vehicleType, filterCode)), PriceFilter)
                Case FilterType.SpecificationFilter
                    filter = DirectCast(DataPortal.Fetch(New Criteria(New SpecificationFilter().GetType(), vehicleType, filterCode)), SpecificationFilter)
                Case FilterType.ValueListFilter
                    filter = DirectCast(DataPortal.Fetch(New Criteria(New ValueListFilter().GetType(), vehicleType, filterCode)), ValueListFilter)
                Case Else
                    Throw New ApplicationException(String.Format("The GetFilter factory method does not provide an implementation for the {0} filter type.", type))
            End Select
            filter.ValidationRules.CheckRules()
            Return filter
        End Function

        Friend Shared Function GetFilter(ByVal dataReader As SafeDataReader) As Filter
            Dim type As FilterType
            If Not FilterType.TryParse(dataReader.GetInt16("FILTERTYPE").ToString(), type) Then Throw New ApplicationException(String.Format("Unkown Filter Type {0}", dataReader.GetInt16("FILTERTYPE")))

            Dim filter As Filter
            Select Case type
                Case FilterType.PriceFilter
                    filter = New PriceFilter
                Case FilterType.SpecificationFilter
                    filter = New SpecificationFilter()
                Case FilterType.ValueListFilter
                    filter = New ValueListFilter()
                Case Else
                    Throw New ApplicationException(String.Format("The GetFilter factory method does not provide an implementation for the {0} filter type.", type))
            End Select
            filter.Fetch(dataReader)
            filter.MarkAsChild()
            Return filter
        End Function

#End Region

#Region " Criteria "

        <Serializable()> Private Class Criteria
            Inherits CommandCriteriaBase

            Private ReadOnly _vehicleType As VehicleType
            Private ReadOnly _filterCode As String

            Public Sub New(ByVal type As Type, ByVal vehicleType As VehicleType, ByVal filterCode As String)
                MyBase.New(type)
                _vehicleType = vehicleType
                _filterCode = filterCode
                CommandText = "getFilter"
            End Sub

            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@VEHICLETYPE", CType(_vehicleType, Int16))
                command.Parameters.AddWithValue("@FILTERCODE", _filterCode)
            End Sub

        End Class
#End Region

#Region " Constructors "
        Protected Sub New()
            AllowRemove = False
            AutoDiscover = False
            AlwaysUpdateSelf = True
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            With dataReader
                _vehicleType = CType(.GetValue("VEHICLETYPE"), VehicleType)
                _code = .GetString("CODE")
                _label = .GetString("LABEL")
                _visible = .GetBoolean("VISIBLE")
            End With
        End Sub
        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            AddUpdateCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            command.CommandText = "updateFilter"
            command.Parameters.AddWithValue("@VEHICLETYPE", VehicleType)
            command.Parameters.AddWithValue("@CODE", Code)
            command.Parameters.AddWithValue("@LABEL", Label)
            command.Parameters.AddWithValue("@VISIBLE", Visible)
        End Sub


        <Serializable()> Private Class GetFilterType
            Inherits ContextCommand.CommandInfo

            Private ReadOnly _vehicleType As VehicleType
            Private ReadOnly _filterCode As String
            Public Sub New(ByVal vehicleType As VehicleType, ByVal filterCode As String)
                _vehicleType = vehicleType
                _filterCode = filterCode
            End Sub
            Public Overloads Overrides ReadOnly Property CommandText() As String
                Get
                    Return "getFilterType"
                End Get
            End Property
            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@VEHICLETYPE", _vehicleType)
                command.Parameters.AddWithValue("@FILTERCODE", _filterCode)
            End Sub
        End Class

#End Region

    End Class


End Namespace