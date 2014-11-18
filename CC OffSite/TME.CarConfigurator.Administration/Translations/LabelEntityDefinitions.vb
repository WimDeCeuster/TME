Imports TME.CarConfigurator.Administration.Enums

Namespace Translations
    <Serializable()> Public Class LabelEntityDefinitions
        Inherits BaseObjects.ContextReadOnlyListBase(Of LabelEntityDefinitions, LabelEntityDefinition)

#Region " Business Properties & Methods "

        Friend Property LabelDefinition() As LabelDefinition
            Get
                Return DirectCast(Me.Parent, LabelDefinition)
            End Get
            Private Set(ByVal value As LabelDefinition)
                Me.SetParent(value)
            End Set
        End Property

        Friend Overloads Function Add(ByVal dataReader As SafeDataReader) As LabelEntityDefinition
            Dim _object As LabelEntityDefinition = GetObject(dataReader)
            MyBase.IsReadOnly = False
            MyBase.Add(_object)
            MyBase.IsReadOnly = True
            Return _object
        End Function

        Default Public Overloads ReadOnly Property Item(ByVal entity As Entity) As LabelEntityDefinition
            Get
                Return Me.FirstOrDefault(Function(d) d.Entity = entity)
            End Get
        End Property


        Public Overloads Function Contains(ByVal entity As Entity) As Boolean
            Return Me.Any(Function(d) d.Entity = entity)
        End Function
#End Region

#Region " Shared Factory Methods "
        Friend Shared Function NewLabelEntityDefinitions(ByVal labelDefinition As LabelDefinition) As LabelEntityDefinitions
            Dim _definitions As LabelEntityDefinitions = New LabelEntityDefinitions()
            _definitions.LabelDefinition = labelDefinition
            Return _definitions
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

    End Class
    <Serializable()> Public Class LabelEntityDefinition
        Inherits BaseObjects.ContextReadOnlyBase(Of LabelEntityDefinition)

#Region " Business Properties & Methods "
        Private _entity As Entity
        Private _mandatory As Boolean
        Private _defaultValue As String
        Private _index As Integer

        Public ReadOnly Property Entity() As Entity
            Get
                Return _entity
            End Get
        End Property
        Public ReadOnly Property Mandatory() As Boolean
            Get
                Return _mandatory
            End Get
        End Property
        Public ReadOnly Property DefaultValue() As String
            Get
                Return _defaultValue
            End Get
        End Property
        Public ReadOnly Property Index() As Integer
            Get
                Return _index
            End Get
        End Property

        Protected Overrides Function GetIdValue() As Object
            Return Me.Entity
        End Function

        Friend ReadOnly Property LabelDefinition() As LabelDefinition
            Get
                Return DirectCast(Me.Parent, LabelEntityDefinitions).LabelDefinition
            End Get
        End Property

#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return Me.Entity.ToString()
        End Function
        Public Overloads Function Equals(ByVal obj As Entity) As Boolean
            Return (Me.Entity = obj)
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            _entity = dataReader.GetEntity("ENTITY")
            _mandatory = dataReader.GetBoolean("MANDATORY")
            _defaultValue = dataReader.GetString("DEFAULTVALUE")
            _index = dataReader.GetInt16("SORTORDER")
            MyBase.FetchFields(dataReader)
        End Sub
#End Region

    End Class
End Namespace
