Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public Class ModelGenerationPackExteriorColours
    Inherits ContextUniqueGuidListBase(Of ModelGenerationPackExteriorColours, ModelGenerationPackExteriorColour)

#Region " Business Properties & Methods "

    Friend ReadOnly Property Pack() As ModelGenerationPack
        Get
            If ExteriorColourType Is Nothing Then Return Nothing
            Return ExteriorColourType.Pack
        End Get
    End Property
    Friend ReadOnly Property ExteriorColourType() As ModelGenerationPackExteriorColourType
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationPackExteriorColourType)
        End Get
    End Property

    Friend Shadows Function Add(ByVal dataReader As SafeDataReader) As ModelGenerationPackExteriorColour
        Dim exteriorColour As ModelGenerationPackExteriorColour = ModelGenerationPackExteriorColour.GetExteriorColour(dataReader)
        RaiseListChangedEvents = False
        AllowNew = True
        MyBase.Add(exteriorColour)
        AllowNew = False
        RaiseListChangedEvents = True
        Return exteriorColour
    End Function

    Friend Sub PropertyChanged(ByVal e As ComponentModel.PropertyChangedEventArgs)
        If Not (e.PropertyName.Equals("Approved") OrElse e.PropertyName.Equals("Declined")) Then Return
        ExteriorColourType.ValidateBusinessRules("Availability")
    End Sub
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationPackExteriorColours(ByVal exteriorColourType As ModelGenerationPackExteriorColourType) As ModelGenerationPackExteriorColours
        Dim exteriorColours As ModelGenerationPackExteriorColours = New ModelGenerationPackExteriorColours
        exteriorColours.SetParent(exteriorColourType)
        Return exteriorColours
    End Function
    Friend Shared Function GetModelGenerationPackExteriorColours(ByVal exteriorColourType As ModelGenerationPackExteriorColourType) As ModelGenerationPackExteriorColours
        Dim exteriorColours As ModelGenerationPackExteriorColours = DataPortal.Fetch(Of ModelGenerationPackExteriorColours)(New CustomCriteria(exteriorColourType))
        exteriorColours.SetParent(exteriorColourType)
        Return exteriorColours
    End Function

#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _packID As Guid
        Private ReadOnly _exteriorColourTypeID As Guid

        Public Sub New(ByVal exteriorColourType As ModelGenerationPackExteriorColourType)
            _packID = exteriorColourType.Pack.ID
            _exteriorColourTypeID = exteriorColourType.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@PACKID", _packID)
            command.Parameters.AddWithValue("@EXTERIORCOLOURTYPEID", _exteriorColourTypeID)
        End Sub

    End Class

#End Region

#Region " Constructors "
    Private Sub New()
        MarkAsChild()
        AllowNew = False
        AllowRemove = False
    End Sub
#End Region

End Class
<Serializable()> Public Class ModelGenerationPackExteriorColour
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationPackExteriorColour)

#Region " Business Properties & Methods "

    Private WithEvents _carSpecification As GenerationCarSpecification
    Private _status As Integer


    Public Property Approved() As Boolean
        Get
            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Approved) Then Return

            If value Then
                _status += Status.ApprovedForLive
            Else
                _status -= Status.ApprovedForLive
            End If
            Declined = Not (value)
            PropertyHasChanged("Approved")
        End Set
    End Property
    Public Property Declined() As Boolean
        Get
            Return ((_status And Status.Declined) = Status.Declined)
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Declined) Then Return

            If value Then
                _status += Status.Declined
            Else
                _status -= Status.Declined
            End If
            Approved = Not (value)
            PropertyHasChanged("Declined")
        End Set
    End Property
    Public ReadOnly Property CarSpecification() As GenerationCarSpecification
        Get
            If _carSpecification.Generation Is Nothing Then _carSpecification.Generation = Pack.Generation
            Return _carSpecification
        End Get
    End Property


    Private Sub OnCarSpecificationPropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Handles _carSpecification.PropertyChanged
        PropertyHasChanged("CarSpecification")
    End Sub
    Private Sub MePropertyChanged(ByVal sender As Object, ByVal e As ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If Parent Is Nothing Then Exit Sub
        DirectCast(Parent, ModelGenerationPackExteriorColours).PropertyChanged(e)
    End Sub

#End Region

#Region " Reference Properties & Methods "
    Private _refObject As ModelGenerationExteriorColour

    Private ReadOnly Property Pack() As ModelGenerationPack
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationPackExteriorColours).Pack
        End Get
    End Property
    Private ReadOnly Property Generation() As ModelGeneration
        Get
            If Pack Is Nothing Then Return Nothing
            Return Pack.Generation
        End Get
    End Property

    Private ReadOnly Property RefObject() As ModelGenerationExteriorColour
        Get
            If _refObject Is Nothing Then _refObject = Generation.ColourCombinations.ExteriorColours.First(Function(x) x.Equals(ID))
            Return _refObject
        End Get
    End Property

    Public ReadOnly Property Code() As String
        Get
            Return RefObject.Code
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return RefObject.Name
        End Get
    End Property

    Public ReadOnly Property AlternateName() As String
        Get
            Return RefObject.AlternateName
        End Get
    End Property
    Public ReadOnly Property Translation() As Translations.Translation
        Get
            Return RefObject.Translation
        End Get
    End Property

#End Region

#Region " Framework Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return String.Format("{0}-{1}", Pack.ID, ID)
    End Function

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not CarSpecification.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If CarSpecification.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationPackExteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID) AndAlso Pack.ID.Equals(obj.Pack.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationPackExteriorColour Then
            Equals(DirectCast(obj, ModelGenerationPackExteriorColour))
        Else
            Return GetIdValue().Equals(obj)
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetExteriorColour(ByVal dataReader As SafeDataReader) As ModelGenerationPackExteriorColour
        Dim exteriorColour As ModelGenerationPackExteriorColour = New ModelGenerationPackExteriorColour()
        exteriorColour.Fetch(dataReader)
        Return exteriorColour
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
        AllowNew = False
        AllowRemove = False
        AlwaysUpdateSelf = True
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("EXTERIORCOLOURID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _carSpecification = GenerationCarSpecification.GetCarSpecification(dataReader)
        _status = dataReader.GetInt16("STATUSID")
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@PACKID", Pack.ID)
        command.Parameters.AddWithValue("@EXTERIORCOLOURID", ID)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        CarSpecification.AppendParameters(command)
        command.Parameters.AddWithValue("@STATUSID", _status)
    End Sub

    Private Sub MeAfterUpdateCommand(ByVal obj As System.Data.SqlClient.SqlTransaction) Handles Me.AfterUpdateCommand
        CarSpecification.MarkOld()
    End Sub
#End Region



 
End Class