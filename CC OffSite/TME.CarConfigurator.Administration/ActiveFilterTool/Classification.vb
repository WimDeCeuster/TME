Imports TME.CarConfigurator.Administration.ActiveFilterTool.Enumerations
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class Classifications
        Inherits BaseObjects.StronglySortedListBase(Of Classifications, Classification)

#Region " Business Properties & Methods "
        Private _type As VehicleType
        Private ReadOnly Property Type() As VehicleType
            Get
                Return _type
            End Get
        End Property


        Public Overloads Overrides Function Add() As Classification
            Dim _classification As Classification = Classification.NewClassificationChild(Me.Type)
            _classification.SetIndex = Me.Count
            MyBase.Add(_classification)
            Return _classification
        End Function
#End Region

#Region " Shared Factory Methods "

        Public Shared Function GetClassifications(ByVal type As VehicleType) As Classifications
            Dim _classifications As Classifications = DataPortal.Fetch(Of Classifications)(New CustomCriteria(type))
            _classifications._type = type
            Return _classifications
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

    End Class
    <Serializable()> Public NotInheritable Class Classification
        Inherits BaseObjects.TranslateableBusinessBase
        Implements BaseObjects.ISortedIndex
        Implements BaseObjects.ISortedIndexSetter

#Region " Business Properties & Methods "

        Private _code As String = String.Empty
        Private _name As String = String.Empty
        Private _status As Integer
        Private _index As Integer
        Private _owner As String
        Private _type As VehicleType

        Public Property Code() As String
            Get
                Return _code
            End Get
            Set(ByVal value As String)
                If _code <> value Then
                    _code = value
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
        Public Property Activated() As Boolean
            Get
                Return ((_status And Status.AvailableToNMSCs) = Status.AvailableToNMSCs)
            End Get
            Set(ByVal value As Boolean)
                If Not value.Equals(Me.Activated) Then
                    If Me.Activated Then
                        _status -= Status.AvailableToNMSCs
                    Else
                        _status += Status.AvailableToNMSCs
                    End If
                    PropertyHasChanged("Activated")
                End If
            End Set
        End Property
        Public Function HasOwnership() As Boolean
            Return (String.Compare(_owner, MyContext.GetContext().CountryCode, True) = 0)
        End Function

        Public ReadOnly Property Index() As Integer Implements BaseObjects.ISortedIndex.Index
            Get
                Return _index
            End Get
        End Property
        Friend WriteOnly Property SetIndex() As Integer Implements BaseObjects.ISortedIndexSetter.SetIndex
            Set(ByVal value As Integer)
                If Not _index.Equals(value) Then
                    _index = value
                    PropertyHasChanged("Index")
                End If
            End Set
        End Property

        Public ReadOnly Property Type() As VehicleType
            Get
                Return _type
            End Get
        End Property
#End Region

#Region " Business & Validation Rules "
        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
            ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 100))

            ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")
            ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))
        End Sub
#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return Me.Name
        End Function

        Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
            Return String.Compare(obj, Me.Code, True) = 0 OrElse String.Compare(obj, Me.Name, True) = 0
        End Function
#End Region

#Region " Shared Factory Methods "
        Public Shared Function NewClassification(ByVal type As VehicleType) As Classification
            Dim _classification As Classification = New Classification
            _classification.Create(type)
            Return _classification
        End Function
        Friend Shared Function NewClassificationChild(ByVal type As VehicleType) As Classification
            Dim _classification As Classification = New Classification
            _classification.Create(type)
            _classification.MarkAsChild()
            Return _classification
        End Function

        Public Shared Function GetClassification(ByVal id As Guid) As Classification
            Return DataPortal.Fetch(Of Classification)(New Criteria(id))
        End Function

        Public Shared Sub DeleteClassification(ByVal id As Guid)
            DataPortal.Delete(Of Classification)(New Criteria(id))
        End Sub
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "

        Private Overloads Sub Create(ByVal vehicleType As VehicleType)
            MyBase.Create()
            _type = vehicleType
            _index = Int16.MaxValue
            Me.Activated = Not (MyContext.GetContext().CountryCode = Environment.GlobalCountryCode)
        End Sub
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            With dataReader
                ID = .GetGuid("ID")
                _code = .GetString("INTERNALCODE")
                _name = .GetString("SHORTNAME")
                _status = .GetInt16("STATUSID")
                _index = .GetInt16("SORTORDER")
                _owner = dataReader.GetString("OWNER")
            End With
            MyBase.FetchFields(dataReader)
        End Sub
        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            Me.AddCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            Me.AddCommandFields(command)
        End Sub
        Private Sub AddCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@VEHICLETYPE", Me.Type)
            command.Parameters.AddWithValue("@INTERNALCODE", Me.Code)
            command.Parameters.AddWithValue("@SHORTNAME", Me.Name)
            command.Parameters.AddWithValue("@STATUSID", _status)
            command.Parameters.AddWithValue("@SORTORDER", Me.Index)
        End Sub

#End Region

#Region " Base Object Overrides "

        Protected Friend Overrides Function GetBaseName() As String
            Return Me.Name
        End Function
        Public Overrides ReadOnly Property Entity As Entity
            Get
                Return Entity.CLASSIFICATION
            End Get
        End Property
#End Region

    End Class
End Namespace