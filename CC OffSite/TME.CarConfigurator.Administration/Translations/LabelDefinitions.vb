Imports TME.CarConfigurator.Administration.Enums

Namespace Translations

    <Serializable()> Public NotInheritable Class LabelDefinitions
        Inherits BaseObjects.ContextUniqueGuidReadOnlyListBase(Of LabelDefinitions, LabelDefinition)

#Region " Shared Factory Methods "
        Private Shared _definitions As LabelDefinitions = Nothing
        Private Shared _timeStamp As DateTime = DateTime.MinValue

        Public Shared Function GetLabelDefinitions() As LabelDefinitions
            If _definitions Is Nothing OrElse DateTime.Now > _timeStamp.AddMinutes(15) Then
                _definitions = DataPortal.Fetch(Of LabelDefinitions)(New Criteria)
                _timeStamp = DateTime.Now
            End If
            Return _definitions
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
            With dataReader
                While .Read()
                    Me(.GetGuid("LABELID")).Entities.Add(dataReader)
                End While
            End With
        End Sub
#End Region

    End Class
    <Serializable()> Public NotInheritable Class LabelDefinition
        Inherits BaseObjects.ContextUniqueGuidReadOnlyBase(Of LabelDefinition)

#Region " Business Properties & Methods "
        Private _code As String
        Private _name As String
        Private _description As String
        Private _displayControl As DisplayControl
        Private _maxSize As Integer
        Private _entities As LabelEntityDefinitions

        Public ReadOnly Property Code() As String
            Get
                Return _code
            End Get
        End Property
        Public ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property
        Public ReadOnly Property Description() As String
            Get
                Return _description
            End Get
        End Property
        Public ReadOnly Property DisplayControl() As DisplayControl
            Get
                Return _displayControl
            End Get
        End Property
        Public ReadOnly Property MaxSize() As Integer
            Get
                Return _maxSize
            End Get
        End Property
        Public ReadOnly Property Entities() As LabelEntityDefinitions
            Get
                If _entities Is Nothing Then _entities = LabelEntityDefinitions.NewLabelEntityDefinitions(Me)
                Return _entities
            End Get
        End Property

#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return Me.Name
        End Function
        Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
            Return String.Compare(Me.Code, obj, True) = 0 OrElse String.Compare(Me.Name, obj, False) = 0
        End Function
#End Region

#Region " Shared Factory Methods "
        Friend Shared Function GetLabelDefinition(ByVal id As Guid) As LabelDefinition
            Return LabelDefinitions.GetLabelDefinitions().Item(id)
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            With dataReader
                _code = .GetString("CODE")
                _name = .GetString("NAME")
                _description = .GetString("DESCRIPTION")
                _displayControl = CType(.GetInt16("DISPLAYCONTROLID"), DisplayControl)
                _maxSize = .GetInt32("MAXSIZE")
            End With
            MyBase.FetchFields(dataReader)
        End Sub
#End Region

    End Class

End Namespace
