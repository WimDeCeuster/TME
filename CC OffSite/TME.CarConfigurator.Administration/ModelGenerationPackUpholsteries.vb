Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public Class ModelGenerationPackUpholsteries
    Inherits ContextUniqueGuidListBase(Of ModelGenerationPackUpholsteries, ModelGenerationPackUpholstery)

#Region " Business Properties & Methods "

    Friend ReadOnly Property Pack() As ModelGenerationPack
        Get
            If UpholsteryType Is Nothing Then Return Nothing
            Return UpholsteryType.Pack
        End Get
    End Property
    Friend ReadOnly Property UpholsteryType() As ModelGenerationPackUpholsteryType
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationPackUpholsteryType)
        End Get
    End Property

    Friend Shadows Function Add(ByVal dataReader As SafeDataReader) As ModelGenerationPackUpholstery
        Dim packUpholstery As ModelGenerationPackUpholstery = ModelGenerationPackUpholstery.GetUpholstery(dataReader)
        RaiseListChangedEvents = False
        AllowNew = True
        MyBase.Add(packUpholstery)
        AllowNew = False
        RaiseListChangedEvents = True
        Return packUpholstery
    End Function

    Friend Sub PropertyChanged(ByVal e As ComponentModel.PropertyChangedEventArgs)
        If Not (e.PropertyName.Equals("Approved") OrElse e.PropertyName.Equals("Declined")) Then Return
        UpholsteryType.ValidateBusinessRules("Availability")
    End Sub

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationPackUpholsteries(ByVal upholsteryType As ModelGenerationPackUpholsteryType) As ModelGenerationPackUpholsteries
        Dim upholsteries As ModelGenerationPackUpholsteries = New ModelGenerationPackUpholsteries
        upholsteries.SetParent(upholsteryType)
        Return upholsteries
    End Function
    Friend Shared Function GetModelGenerationPackUpholsteries(ByVal upholsteryType As ModelGenerationPackUpholsteryType) As ModelGenerationPackUpholsteries
        Dim upholsteries As ModelGenerationPackUpholsteries = DataPortal.Fetch(Of ModelGenerationPackUpholsteries)(New CustomCriteria(upholsteryType))
        upholsteries.SetParent(upholsteryType)
        Return upholsteries
    End Function

#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _packID As Guid
        Private ReadOnly _upholsteryTypeId As Guid

        Public Sub New(ByVal upholsteryType As ModelGenerationPackUpholsteryType)
            _packID = upholsteryType.Pack.ID
            _upholsteryTypeId = upholsteryType.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@PACKID", _packID)
            command.Parameters.AddWithValue("@UPHOLSTERYTYPEID", _upholsteryTypeId)
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
<Serializable()> Public Class ModelGenerationPackUpholstery
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationPackUpholstery)

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
        DirectCast(Parent, ModelGenerationPackUpholsteries).PropertyChanged(e)
    End Sub

#End Region

#Region " Reference Properties & Methods "
    Private _refObject As ModelGenerationUpholstery

    Private ReadOnly Property Pack() As ModelGenerationPack
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationPackUpholsteries).Pack
        End Get
    End Property
    Private ReadOnly Property Generation() As ModelGeneration
        Get
            If Pack Is Nothing Then Return Nothing
            Return Pack.Generation
        End Get
    End Property

    Private ReadOnly Property RefObject() As ModelGenerationUpholstery
        Get
            If _refObject Is Nothing Then _refObject = Generation.ColourCombinations.Upholsteries.First(Function(x) x.Equals(ID))
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

    Public Overloads Function Equals(ByVal obj As ModelGenerationPackUpholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID) AndAlso Pack.ID.Equals(obj.Pack.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationPackUpholstery Then
            Equals(DirectCast(obj, ModelGenerationPackUpholstery))
        Else
            Return GetIdValue().Equals(obj)
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetUpholstery(ByVal dataReader As SafeDataReader) As ModelGenerationPackUpholstery
        Dim upholstery As ModelGenerationPackUpholstery = New ModelGenerationPackUpholstery()
        upholstery.Fetch(dataReader)
        Return upholstery
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
        ID = dataReader.GetGuid("UPHOLSTERYID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _carSpecification = GenerationCarSpecification.GetCarSpecification(dataReader)
        _status = dataReader.GetInt16("STATUSID")
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@PACKID", Pack.ID)
        command.Parameters.AddWithValue("@UPHOLSTERYID", ID)
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