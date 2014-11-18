Imports System.Security
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public Class CarPackUpholsteries
    Inherits ContextUniqueGuidListBase(Of CarPackUpholsteries, CarPackUpholstery)

#Region " Business Properties & Methods "

    Friend Property UpholsteryType() As CarPackUpholsteryType
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, CarPackUpholsteryType)
        End Get
        Private Set(value As CarPackUpholsteryType)
            SetParent(value)
        End Set
    End Property

    Friend Sub PropertyChanged(ByVal e As ComponentModel.PropertyChangedEventArgs)
        If Not (e.PropertyName.Equals("Approved") OrElse e.PropertyName.Equals("Declined")) Then Return
        If Any(Function(x) x.Approved) Then Return
        If UpholsteryType.Availability = Availability.NotAvailable Then Return

        If Not UpholsteryType.Overwritten Then UpholsteryType.Overwrite()
        UpholsteryType.Availability = Availability.NotAvailable
    End Sub

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetCarPackUpholsteries(ByVal carPackUpholsteryType As CarPackUpholsteryType) As CarPackUpholsteries
        Dim carPackUpholsteries As CarPackUpholsteries = New CarPackUpholsteries
        Dim generationPackUpholsteries As ModelGenerationPackUpholsteries = DirectCast(carPackUpholsteryType.GenerationPackItem, ModelGenerationPackUpholsteryType).Upholsteries
        carPackUpholsteries.UpholsteryType = carPackUpholsteryType
        If carPackUpholsteryType.GenerationPackItem.IsNew Then
            carPackUpholsteries.Combine(generationPackUpholsteries, Nothing)
        Else
            Dim retrievedCarPackUpholsteries = DataPortal.Fetch(Of CarPackUpholsteries)(New CustomCriteria(carPackUpholsteryType))
            retrievedCarPackUpholsteries.UpholsteryType = carPackUpholsteryType
            carPackUpholsteries.Combine(generationPackUpholsteries, retrievedCarPackUpholsteries)
        End If

        Return carPackUpholsteries
    End Function

#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _carID As Guid
        Private ReadOnly _packID As Guid
        Private ReadOnly _upholsteryTypeID As Guid

        Public Sub New(ByVal upholsteryType As CarPackUpholsteryType)
            _carID = upholsteryType.Pack.Car.ID
            _packID = upholsteryType.Pack.ID
            _upholsteryTypeID = upholsteryType.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@CARID", _carID)
            command.Parameters.AddWithValue("@PACKID", _packID)
            command.Parameters.AddWithValue("@UPHOLSTERYTYPEID", _upholsteryTypeID)
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

#Region " Data Access "

    Private Sub Combine(ByVal generationPackUpholsteries As ModelGenerationPackUpholsteries, ByVal carPackUpholsteries As CarPackUpholsteries)
        AllowNew = True
        For Each generationPackUpholstery In generationPackUpholsteries

            Dim carPackUpholstery As CarPackUpholstery
            If Not carPackUpholsteries Is Nothing AndAlso carPackUpholsteries.Contains(generationPackUpholstery.ID) Then
                carPackUpholstery = carPackUpholsteries(generationPackUpholstery.ID)
                carPackUpholstery.GenerationPackUpholstery = generationPackUpholstery
            Else
                carPackUpholstery = carPackUpholstery.NewCarPackUpholstery(generationPackUpholstery)
            End If
            Add(carPackUpholstery)

        Next
        AllowNew = False
    End Sub
#End Region

End Class
<Serializable()> Public Class CarPackUpholstery
    Inherits ContextUniqueGuidBusinessBase(Of CarPackUpholstery)
    Implements IOverwritable

#Region " Business Properties & Methods "

    Private _overwritten As Boolean = False
    Private _status As Integer

    Public Function HasBeenOverwritten() As Boolean Implements IOverwritable.HasBeenOverwritten
        Return Overwritten
    End Function
    <XmlInfo(XmlNodeType.Attribute)> Public Property Overwritten() As Boolean
        Get
            Return _overwritten
        End Get
        Private Set(ByVal value As Boolean)
            If value.Equals(_overwritten) Then Return

            _overwritten = value
            AllowEdit = value
            AllowNew = value
            PropertyHasChanged("Overwritten")
        End Set
    End Property


    Public Sub Overwrite() Implements IOverwritable.Overwrite
        If CanNotBeFitted() Then Throw New SecurityException("Can not overwrite the exterior colour since it can not fit anyway")
        Overwritten = True
    End Sub
    Public Sub Revert() Implements IOverwritable.Revert
        If Not Overwritten Then Return

        Overwritten = False
        RefreshParentProperties()

        If IsNew Then
            MarkClean()
        End If

    End Sub

    Public Function CanNotBeFitted() As Boolean
        Return Not GenerationPackUpholstery.CarSpecification.Matches(Pack.Car)
    End Function

    Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If propertyName.Equals("Overwritten") Then Return True
        If CanNotBeFitted() Then Return False
        Return MyBase.CanWriteProperty(propertyName)
    End Function

    Public Property Approved() As Boolean
        Get
            If CanNotBeFitted() Then Return False
            If UpholsteryType.Availability = Availability.NotAvailable Then Return False

            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Set(ByVal value As Boolean)
            If CanNotBeFitted() Then Throw New SecurityException("Can not change the approval because the upholstery is not available anyway")
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
            If CanNotBeFitted() Then Return True
            Return ((_status And Status.Declined) = Status.Declined)
        End Get
        Set(ByVal value As Boolean)
            If CanNotBeFitted() Then Throw New SecurityException("Can not change the approval because the upholstery is not available anyway")
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
    Private Sub MePropertyChanged(ByVal sender As Object, ByVal e As ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If Parent Is Nothing Then Exit Sub
        DirectCast(Parent, CarPackUpholsteries).PropertyChanged(e)
    End Sub

#End Region

#Region " Reference Properties & Methods "
    Private WithEvents _generationPackUpholstery As ModelGenerationPackUpholstery



    Private ReadOnly Property Pack() As CarPack
        Get
            If UpholsteryType Is Nothing Then Return Nothing
            Return UpholsteryType.Pack
        End Get
    End Property

    Public ReadOnly Property UpholsteryType() As CarPackUpholsteryType
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, CarPackUpholsteries).UpholsteryType
        End Get
    End Property

    Friend Property GenerationPackUpholstery() As ModelGenerationPackUpholstery
        Get
            Return _generationPackUpholstery
        End Get
        Set(value As ModelGenerationPackUpholstery)
            _generationPackUpholstery = value
            RefreshParentProperties()
        End Set
    End Property
    Private Sub OngenerationPackUpholsteryPropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Handles _generationPackUpholstery.PropertyChanged
        If e.PropertyName.Length = 0 Then Exit Sub

        RefreshParentProperties()
    End Sub
    Private Sub RefreshParentProperties()

        If Overwritten Then
            If CanNotBeFitted() Then Revert()
            Return
        End If

        If GenerationPackUpholstery.Approved Then
            _status = Status.ApprovedForLive
        Else
            _status = Status.Declined
        End If
    End Sub

    Public ReadOnly Property Code() As String
        Get
            Return GenerationPackUpholstery.Code
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return GenerationPackUpholstery.Name
        End Get
    End Property

    Public ReadOnly Property AlternateName() As String
        Get
            Return GenerationPackUpholstery.AlternateName
        End Get
    End Property
    Public ReadOnly Property Translation() As Translations.Translation
        Get
            Return GenerationPackUpholstery.Translation
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

    Public Overloads Function Equals(ByVal obj As CarPackUpholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID) AndAlso Pack.ID.Equals(obj.Pack.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is CarPackUpholstery Then
            Equals(DirectCast(obj, CarPackUpholstery))
        Else
            Return GetIdValue().Equals(obj)
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Public Shared Function NewCarPackUpholstery(ByVal generationPackUpholstery As ModelGenerationPackUpholstery) As CarPackUpholstery
        Dim packUpholstery = New CarPackUpholstery
        packUpholstery.Create(generationPackUpholstery)
        Return packUpholstery
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
        AllowNew = False
        AllowRemove = False
    End Sub
#End Region

#Region " Data Access "
    Private Overloads Sub Create(ByVal packUpholstery As ModelGenerationPackUpholstery)
        Create(packUpholstery.ID)
        GenerationPackUpholstery = packUpholstery
        RefreshParentProperties()
        MarkClean()
    End Sub
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("UPHOLSTERYID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _status = dataReader.GetInt16("STATUSID")
        _overwritten = True
        AllowEdit = True
        MyBase.FetchFields(dataReader)
    End Sub



    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Private Sub AddCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@CARID", Pack.Car.ID)
        command.Parameters.AddWithValue("@PACKID", Pack.ID)
        command.Parameters.AddWithValue("@UPHOLSTERYID", ID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateCarPackUpholstery"
        command.Parameters.AddWithValue("@STATUSID", _status)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        If Not Overwritten Then
            command.CommandText = "deleteCarPackUpholstery"
            Exit Sub
        End If

        command.CommandText = "updateCarPackUpholstery"
        command.Parameters.AddWithValue("@STATUSID", _status)
    End Sub


#End Region




End Class