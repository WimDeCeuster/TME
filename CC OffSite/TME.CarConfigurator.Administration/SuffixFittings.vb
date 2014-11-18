Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class SuffixFittings
    Inherits BaseObjects.ContextUniqueGuidListBase(Of SuffixFittings, SuffixFitting)

#Region " Business Properties & Methods "
    Public Overloads Function Add(ByVal factoryGeneration As ModelGenerationFactoryGeneration) As SuffixFitting
        Dim _fitting As SuffixFitting = MyBase.Add()
        _fitting.Generation = factoryGeneration.Generation.GetInfo()
        _fitting.FactoryGeneration = factoryGeneration.GetInfo()
        Return _fitting
    End Function

    Friend ReadOnly Property EquipmentItem() As EquipmentItem
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, EquipmentItem)
        End Get
    End Property
#End Region

#Region " Contains  Methods "
    Public Overloads Function Contains(ByVal generation As ModelGeneration, ByVal factoryGeneration As FactoryGeneration) As Boolean
        Return Me.Any(Function(x) x.Generation.ID.Equals(generation.ID) AndAlso x.FactoryGeneration.ID.Equals(factoryGeneration.ID))
    End Function
    Public Overloads Function Contains(ByVal factoryGeneration As ModelGenerationFactoryGeneration) As Boolean
        Return Me.Any(Function(x) x.Generation.ID.Equals(factoryGeneration.Generation.ID) AndAlso x.FactoryGeneration.ID.Equals(factoryGeneration.ID))
    End Function
    Friend Function ContainsMatch(ByVal partialSpecification As PartialCarSpecification) As Boolean
        Return Me.Any(Function(x) x.Matches(partialSpecification))
    End Function
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewSuffixFittings(ByVal [option] As [Option]) As SuffixFittings
        Dim _fittings As SuffixFittings = New SuffixFittings()
        _fittings.SetParent([option])
        Return _fittings
    End Function
#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
        Me.AllowNew = MyContext.GetContext().IsGlobal()
        Me.AllowEdit = False
        Me.AllowRemove = Me.AllowNew
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "

    Friend Sub FetchRow(ByVal dataReader As SafeDataReader)
        Me.SupressSecurityCheck = True
        MyBase.Add(GetObject(dataReader))
        Me.SupressSecurityCheck = False
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class SuffixFitting
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of SuffixFitting)

#Region " Business Properties & Methods "
    Private _generation As ModelGenerationInfo = Nothing
    Private _factoryGeneration As FactoryGenerationInfo = Nothing

    Public Property Generation() As ModelGenerationInfo
        Get
            Return _generation
        End Get
        Set(ByVal value As ModelGenerationInfo)
            If _generation Is Nothing AndAlso value Is Nothing Then Exit Property
            If value Is Nothing OrElse _generation Is Nothing OrElse Not _generation.Equals(value) Then
                _generation = value
                PropertyHasChanged("Generation")
            End If
        End Set
    End Property
    Public Property FactoryGeneration() As FactoryGenerationInfo
        Get
            Return _factoryGeneration
        End Get
        Set(ByVal value As FactoryGenerationInfo)
            If _factoryGeneration Is Nothing AndAlso value Is Nothing Then Exit Property
            If value Is Nothing OrElse _factoryGeneration Is Nothing OrElse Not _factoryGeneration.Equals(value) Then
                _factoryGeneration = value
                PropertyHasChanged("FactoryGeneration")
            End If
        End Set
    End Property

    Private ReadOnly Property EquipmentItem() As EquipmentItem
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, SuffixFittings).EquipmentItem
        End Get
    End Property

    Friend Function Matches(ByVal specification As PartialCarSpecification) As Boolean
        If Not (specification.ModelID.Equals(Guid.Empty) OrElse Me.Generation.Model.ID.Equals(specification.ModelID)) Then Return False
        If Not (specification.GenerationID.Equals(Guid.Empty) OrElse Me.Generation.ID.Equals(specification.GenerationID)) Then Return False
        Return True
    End Function

#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "Generation")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "FactoryGeneration")

        ValidationRules.AddRule(DirectCast(AddressOf SuffixFitting.ObjectUnique, Validation.RuleHandler), "Generation")
        ValidationRules.AddRule(DirectCast(AddressOf SuffixFitting.ObjectUnique, Validation.RuleHandler), "FactoryGeneration")
    End Sub
    Private Shared Function ObjectUnique(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _value As SuffixFitting = DirectCast(target, SuffixFitting)
        If _value.Parent Is Nothing Then Return True
        If _value.Generation Is Nothing Then Return True
        If _value.FactoryGeneration Is Nothing Then Return True

        If DirectCast(_value.Parent, SuffixFittings).Any(Function(x) Not x.ID.Equals(_value.ID) AndAlso x.Generation.Equals(_value.Generation) AndAlso x.FactoryGeneration.Equals(_value.FactoryGeneration)) Then
            e.Description = "There already is fitting available for " & _value.ToString()
            Return False
        End If

        Return True
    End Function
#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        If Me.Generation Is Nothing AndAlso Me.FactoryGeneration Is Nothing Then Return String.Empty
        If Me.Generation Is Nothing Then Return String.Format("??? - {0}", Me.FactoryGeneration.ToString())
        If Me.FactoryGeneration Is Nothing Then Return String.Format("{0} - ???", Me.Generation.ToString())
        Return String.Format("{0} - {1}", Me.Generation.ToString(), Me.FactoryGeneration.ToString())
    End Function
#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
        Me.AllowNew = MyContext.GetContext().IsGlobal()
        Me.AllowEdit = False
        Me.AllowRemove = Me.AllowNew
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _generation = ModelGenerationInfo.GetModelGenerationInfo(dataReader)
        _factoryGeneration = FactoryGenerationInfo.GetFactoryGenerationInfo(dataReader)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub

    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@EQUIPMENTID", Me.EquipmentItem.ID)
        command.Parameters.AddWithValue("@MODELID", Me.Generation.Model.ID)
        command.Parameters.AddWithValue("@GENERATIONID", Me.Generation.ID)
        command.Parameters.AddWithValue("@FACTORYGENERATIONID", Me.FactoryGeneration.ID)
    End Sub
#End Region

End Class