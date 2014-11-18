Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class FilterValue
        Inherits ContextUniqueGuidBusinessBase(Of FilterValue)
        Implements ISortedIndex
        Implements ISortedIndexSetter
        Implements IOwnedBy

#Region " Business Properties & Methods "
        Private _owner As String
        Private _label As String
        Private _default As Boolean
        Private _cleared As Boolean
        Private _index As Integer
        Private _equipment As FilterValueEquipment
        Private _applicabilities As FilterValueApplicabilities


        Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
            Get
                Return _owner
            End Get
        End Property
        Public Property Label() As String
            Get
                Return _label
            End Get
            Set(ByVal value As String)
                If _label.Equals(value) Then Return

                _label = value
                PropertyHasChanged("Label")
            End Set
        End Property
        Public Property [Default]() As Boolean
            Get
                Return _default
            End Get
            Set(ByVal value As Boolean)
                If _default.Equals(value) Then Return

                _default = value
                If _default Then ResetPreviousDefaultValue()

                PropertyHasChanged("Default")
                Filter.Values.OnFilterValuesChanged()
            End Set
        End Property
        Public Property Cleared() As Boolean
            Get
                Return _cleared
            End Get
            Set(ByVal value As Boolean)
                If _cleared.Equals(value) Then Return
                _cleared = value
                [Default] = False
                PropertyHasChanged("Cleared")
            End Set
        End Property

        Private Sub ResetPreviousDefaultValue()
            Dim previousDefaultValue = From value In Filter.Values Where value.Default AndAlso Not value.ID.Equals(ID)
            For Each filterValue As FilterValue In previousDefaultValue
                filterValue.Default = False
            Next
        End Sub

        Public ReadOnly Property Index() As Integer Implements ISortedIndex.Index
            Get
                Return _index
            End Get
        End Property
        Friend WriteOnly Property SetIndex() As Integer Implements ISortedIndexSetter.SetIndex
            Set(ByVal value As Integer)
                If _index.Equals(value) Then Return

                _index = value
                PropertyHasChanged("Index")
            End Set
        End Property

        Public ReadOnly Property Equipment() As FilterValueEquipment
            Get
                If _equipment Is Nothing Then _equipment = FilterValueEquipment.GetEquipmentFilterValueEquipment(Me)
                Return _equipment
            End Get
        End Property
        Public ReadOnly Property Applicabilities() As FilterValueApplicabilities
            Get
                If _applicabilities Is Nothing Then _applicabilities = FilterValueApplicabilities.GetFilterValueApplicabilities(Me)
                Return _applicabilities
            End Get
        End Property
        Public ReadOnly Property Filter() As ValueListFilter
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, FilterValues).Filter
            End Get
        End Property

#End Region

#Region " Move Methods "
        Public Function CanMoveDown() As Boolean
            Return DirectCast(Parent, FilterValues).CanMoveDown(Me)
        End Function
        Public Function CanMoveUp() As Boolean
            Return DirectCast(Parent, FilterValues).CanMoveUp(Me)
        End Function
        Public Function MoveDown() As Boolean
            Return DirectCast(Parent, FilterValues).MoveDown(Me)
        End Function
        Public Function MoveUp() As Boolean
            Return DirectCast(Parent, FilterValues).MoveUp(Me)
        End Function
#End Region

#Region " Business & Validation Rules "
        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, Validation.RuleHandler), "Label")
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, Validation.RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("Label", 255))
        End Sub
#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return Label
        End Function
#End Region

#Region " Framework Overrides "
        Protected Overrides Function GetIdValue() As Object
            Return ID
        End Function
        Public Overloads Overrides ReadOnly Property IsValid() As Boolean
            Get
                If Not MyBase.IsValid Then Return False
                If Not (_equipment Is Nothing) AndAlso Not _equipment.IsValid Then Return False
                If Not (_applicabilities Is Nothing) AndAlso Not _applicabilities.IsValid Then Return False
                Return True
            End Get
        End Property
        Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
            Get
                If MyBase.IsDirty Then Return True
                If Not (_equipment Is Nothing) AndAlso _equipment.IsDirty Then Return True
                If Not (_applicabilities Is Nothing) AndAlso _applicabilities.IsDirty Then Return True
                Return False
            End Get
        End Property
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "

        Protected Overrides Sub InitializeFields()
            _owner = MyContext.GetContext().CountryCode
            _label = String.Empty
            AllowRemove = True
        End Sub
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            _owner = dataReader.GetString("OWNER")
            _label = dataReader.GetString("LABEL")
            _default = dataReader.GetBoolean("DEFAULTVALUE")
            _cleared = dataReader.GetBoolean("CLEARED")
            _index = dataReader.GetInt16("SORTORDER")
            AllowRemove = MyContext.GetContext().CountryCode.Equals(_owner, StringComparison.InvariantCultureIgnoreCase)
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@VEHICLETYPE", Filter.VehicleType)
            command.Parameters.AddWithValue("@FILTERCODE", Filter.Code)
            AddUpdateCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@LABEL", Label)
            command.Parameters.AddWithValue("@DEFAULTVALUE", [Default])
            command.Parameters.AddWithValue("@SORTORDER", Index)
            command.Parameters.AddWithValue("@CLEARED", Cleared)
        End Sub

        Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            MyBase.UpdateChildren(transaction)
            If Not _equipment Is Nothing Then _equipment.Update(transaction)
            If Not _applicabilities Is Nothing Then _applicabilities.Update(transaction)
        End Sub

#End Region

    End Class
End NameSpace