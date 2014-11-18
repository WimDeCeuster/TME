Imports System.Collections.Generic
Imports System.ComponentModel
Imports TME.BusinessObjects.Templates

<Serializable(), CommandClassName("GenerationColourCombinations")> Public NotInheritable Class ModelGenerationColourCombinations
    Inherits ContextListBase(Of ModelGenerationColourCombinations, ModelGenerationColourCombination)

#Region " Business Properties & Methods "

    Private _exteriorColours As ModelGenerationExteriorColours
    Private _upholsteries As ModelGenerationUpholsteries

    Public Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGeneration)
        End Get
        Private Set(ByVal value As ModelGeneration)
            SetParent(value)
            If Not _exteriorColours Is Nothing Then _exteriorColours.Generation = value
            If Not _upholsteries Is Nothing Then _upholsteries.Generation = value
        End Set
    End Property

    Default Public Overloads ReadOnly Property Item(ByVal id As Guid) As ModelGenerationColourCombination
        Get
            Return FirstOrDefault(Function(combination) combination.ID = id)
        End Get
    End Property
    Default Public Overloads ReadOnly Property Item(ByVal exteriorColour As Guid, ByVal upholstery As Guid) As ModelGenerationColourCombination
        Get
            Return FirstOrDefault(Function(combination) combination.ExteriorColour.Equals(exteriorColour) AndAlso combination.Upholstery.Equals(upholstery))
        End Get
    End Property

    Public Overloads Function Add(ByVal exteriorColour As ExteriorColour, ByVal upholstery As Upholstery) As ModelGenerationColourCombination
        If Any(Function(x) x.ExteriorColour.Equals(exteriorColour) AndAlso x.Upholstery.Equals(upholstery)) Then Throw New Exceptions.ObjectAlreadyExists("colour combination", String.Format("{0} - {1}", exteriorColour.Name, upholstery.Name))

        If Not _exteriorColours.Contains(exteriorColour.ID) Then _exteriorColours.Add(ModelGenerationExteriorColour.NewModelGenerationExteriorColour(Generation, exteriorColour))
        If Not _upholsteries.Contains(upholstery.ID) Then _upholsteries.Add(ModelGenerationUpholstery.NewModelGenerationUpholstery(Generation, upholstery))
        Return Add(exteriorColour.ID, upholstery.ID)
    End Function
    Private Overloads Function Add(ByVal exteriorColourID As Guid, ByVal upholsteryID As Guid) As ModelGenerationColourCombination
        Dim colourCombination As ModelGenerationColourCombination = ModelGenerationColourCombination.NewModelGenerationColourCombination(_exteriorColours.Item(exteriorColourID), _upholsteries.Item(upholsteryID))
        Add(colourCombination)
        Return colourCombination
    End Function

    Public Function ExteriorColours() As IEnumerable(Of ModelGenerationExteriorColour)
        Return (From colourCombination In Me Select colourCombination.ExteriorColour Distinct)
    End Function
    Public Function Upholsteries() As IEnumerable(Of ModelGenerationUpholstery)
        Return (From colourCombination In Me Select colourCombination.Upholstery Distinct)
    End Function


    Public Overloads Function Contains(ByVal exteriorColourCode As String, ByVal interiorColourCode As String, ByVal trimCode As String) As Boolean
        Return Any(Function(combination) combination.ExteriorColour.Equals(exteriorColourCode) AndAlso combination.Upholstery.InteriorColour.Equals(interiorColourCode) AndAlso combination.Upholstery.Trim.Equals(trimCode))
    End Function


    Protected Overrides Sub OnListChanged(ByVal e As ListChangedEventArgs)
        MyBase.OnListChanged(e)
        If Generation Is Nothing Then Exit Sub

        Generation.RaiseColourAvailabilityChanged()
    End Sub


#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationColourCombinations(ByVal generation As ModelGeneration) As ModelGenerationColourCombinations
        Dim colourCombinations As ModelGenerationColourCombinations = New ModelGenerationColourCombinations()
        colourCombinations.Generation = generation
        colourCombinations._exteriorColours = ModelGenerationExteriorColours.NewModelGenerationExteriorColours(generation)
        colourCombinations._upholsteries = ModelGenerationUpholsteries.NewModelGenerationUpholsteriesy(generation)
        Return colourCombinations
    End Function
    Friend Shared Function GetModelGenerationColourCombinations(ByVal generation As ModelGeneration) As ModelGenerationColourCombinations
        Dim colourCombinations As ModelGenerationColourCombinations = DataPortal.Fetch(Of ModelGenerationColourCombinations)(New ParentCriteria(generation.ID, "@GENERATIONID"))
        colourCombinations.Generation = generation
        Return colourCombinations
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub Fetch(ByVal dataReader As Common.Database.SafeDataReader)
        RaiseListChangedEvents = RaiseListChangedEventsDuringFetch
        With dataReader
            _exteriorColours = ModelGenerationExteriorColours.GetModelGenerationExteriorColours(dataReader)

            .NextResult()
            _upholsteries = ModelGenerationUpholsteries.GetModelGenerationUpholsteries(dataReader)

            .NextResult()
            While .Read
                Add(ModelGenerationColourCombination.GetModelGenerationColourCombination(dataReader, _exteriorColours(.GetGuid("EXTERIORCOLOURID")), _upholsteries(.GetGuid("UPHOLSTERYID"))))
            End While
        End With
        RaiseListChangedEvents = True
    End Sub


    Private Sub BeforeUpdate(ByVal transaction As System.Data.SqlClient.SqlTransaction) Handles Me.BeforeUpdateCommand
        _exteriorColours.Update(transaction)
        _upholsteries.Update(transaction)
    End Sub
#End Region

End Class
<Serializable(), XmlInfo("colourcombination"), CommandClassName("GenerationColourCombination")> Public NotInheritable Class ModelGenerationColourCombination
    Inherits ContextBusinessBase(Of ModelGenerationColourCombination)

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _upholstery As ModelGenerationUpholstery
    Private _exteriorColour As ModelGenerationExteriorColour

    Private ReadOnly Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationColourCombinations).Generation
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public Property ID() As Guid
        Get
            Return _id
        End Get
        Private Set(ByVal value As Guid)
            _id = value
        End Set
    End Property
    Public Property Upholstery() As ModelGenerationUpholstery
        Get
            Return _upholstery
        End Get
        Private Set(ByVal value As ModelGenerationUpholstery)
            _upholstery = value
        End Set
    End Property
    Public Property ExteriorColour() As ModelGenerationExteriorColour
        Get
            Return _exteriorColour
        End Get
        Private Set(ByVal value As ModelGenerationExteriorColour)
            _exteriorColour = value
        End Set
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return ExteriorColour.ToString() & " - " & Upholstery.ToString()
    End Function

    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return obj.Equals(ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationColourCombination) As Boolean
        Return Not (obj Is Nothing) AndAlso obj.ID.Equals(ID)
    End Function
    Public Overloads Function Equals(ByVal obj As LinkedColourCombination) As Boolean
        Return Not (obj Is Nothing) AndAlso
                obj.ExteriorColour.ID.Equals(ExteriorColour.ID) AndAlso
                obj.Upholstery.ID.Equals(Upholstery.ID)
    End Function

    Public Overloads Function Equals(ByVal exteriorColourCode As String, ByVal interiorColourCode As String, ByVal trimCode As String) As Boolean
        Return ExteriorColour.Equals(exteriorColourCode) AndAlso
                Upholstery.InteriorColour.Equals(interiorColourCode) AndAlso
                Upholstery.Trim.Equals(trimCode)
    End Function


    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationColourCombination Then
            Return Equals(DirectCast(obj, ModelGenerationColourCombination))
        ElseIf TypeOf obj Is LinkedColourCombination Then
            Return Equals(DirectCast(obj, LinkedColourCombination))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_exteriorColour Is Nothing) AndAlso Not _exteriorColour.IsValid Then Return False
            If Not (_upholstery Is Nothing) AndAlso Not _upholstery.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_exteriorColour Is Nothing) AndAlso _exteriorColour.IsDirty Then Return True
            If Not (_upholstery Is Nothing) AndAlso _upholstery.IsDirty Then Return True
            Return False
        End Get
    End Property

    Protected Overrides Function GetIdValue() As Object
        Return ID
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationColourCombination(ByVal exteriorColour As ModelGenerationExteriorColour, ByVal upholstery As ModelGenerationUpholstery) As ModelGenerationColourCombination
        Dim colourCombination As ModelGenerationColourCombination = New ModelGenerationColourCombination()
        colourCombination.Create()
        colourCombination.ID = Guid.NewGuid()
        colourCombination.ExteriorColour = exteriorColour
        colourCombination.Upholstery = upholstery
        Return colourCombination
    End Function
    Friend Shared Function GetModelGenerationColourCombination(ByVal dataReader As SafeDataReader, ByVal exteriorColour As ModelGenerationExteriorColour, ByVal upholstery As ModelGenerationUpholstery) As ModelGenerationColourCombination
        Dim colourCombination As ModelGenerationColourCombination = New ModelGenerationColourCombination()
        colourCombination.Fetch(dataReader)
        colourCombination.ExteriorColour = exteriorColour
        colourCombination.Upholstery = upholstery
        Return colourCombination
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _id = dataReader.GetGuid("ID")
        If _id.Equals(Guid.Empty) Then
            _id = Guid.NewGuid()
            MarkNew()
            MarkDirty()
        End If
    End Sub

    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@ID", ID)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@ID", ID)
        command.Parameters.AddWithValue("@GENERATIONID", Generation.ID)
        command.Parameters.AddWithValue("@EXTERIORCOLOURID", ExteriorColour.ID)
        command.Parameters.AddWithValue("@UPHOLSTERYID", Upholstery.ID)
    End Sub

#End Region

End Class