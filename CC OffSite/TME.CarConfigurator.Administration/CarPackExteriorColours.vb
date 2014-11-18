Imports System.Security
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public Class CarPackExteriorColours
    Inherits ContextUniqueGuidListBase(Of CarPackExteriorColours, CarPackExteriorColour)

#Region " Business Properties & Methods "

    Friend Property ExteriorColourType() As CarPackExteriorColourType
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, CarPackExteriorColourType)
        End Get
        Private Set(value As CarPackExteriorColourType)
            SetParent(value)
        End Set
    End Property

    Friend Sub PropertyChanged(ByVal e As ComponentModel.PropertyChangedEventArgs)
        If Not (e.PropertyName.Equals("Approved") OrElse e.PropertyName.Equals("Declined")) Then Return
        If Any(Function(x) x.Approved) Then Return
        If ExteriorColourType.Availability = Availability.NotAvailable Then Return

        If Not ExteriorColourType.Overwritten Then ExteriorColourType.Overwrite()
        ExteriorColourType.Availability = Availability.NotAvailable
    End Sub

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetCarPackExteriorColours(ByVal carPackExteriorColourType As CarPackExteriorColourType) As CarPackExteriorColours
        Dim carPackExteriorColours As CarPackExteriorColours = New CarPackExteriorColours
        Dim generationPackExteriorColours As ModelGenerationPackExteriorColours = DirectCast(carPackExteriorColourType.GenerationPackItem, ModelGenerationPackExteriorColourType).ExteriorColours
        carPackExteriorColours.ExteriorColourType = carPackExteriorColourType
        If carPackExteriorColourType.GenerationPackItem.IsNew Then
            carPackExteriorColours.Combine(generationPackExteriorColours, Nothing)
        Else
            Dim retrievedCarPackExteriorColours = DataPortal.Fetch(Of CarPackExteriorColours)(New CustomCriteria(carPackExteriorColourType))
            retrievedCarPackExteriorColours.ExteriorColourType = carPackExteriorColourType
            carPackExteriorColours.Combine(generationPackExteriorColours, retrievedCarPackExteriorColours)
        End If

        Return carPackExteriorColours
    End Function

#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _carID As Guid
        Private ReadOnly _packID As Guid
        Private ReadOnly _exteriorColourTypeID As Guid

        Public Sub New(ByVal exteriorColourType As CarPackExteriorColourType)
            _carID = exteriorColourType.Pack.Car.ID
            _packID = exteriorColourType.Pack.ID
            _exteriorColourTypeID = exteriorColourType.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@CARID", _carID)
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

#Region " Data Access "

    Private Sub Combine(ByVal generationPackExteriorColours As ModelGenerationPackExteriorColours, ByVal carPackExteriorColours As CarPackExteriorColours)
        AllowNew = True
        For Each generationPackExteriorColour In generationPackExteriorColours

            Dim carPackExteriorColour As CarPackExteriorColour
            If Not carPackExteriorColours Is Nothing AndAlso carPackExteriorColours.Contains(generationPackExteriorColour.ID) Then
                carPackExteriorColour = carPackExteriorColours(generationPackExteriorColour.ID)
                carPackExteriorColour.GenerationPackExteriorColour = generationPackExteriorColour
            Else
                carPackExteriorColour = carPackExteriorColour.NewCarPackExteriorColour(generationPackExteriorColour)
            End If
            Add(carPackExteriorColour)

        Next
        AllowNew = False
    End Sub
#End Region

End Class
<Serializable()> Public Class CarPackExteriorColour
    Inherits ContextUniqueGuidBusinessBase(Of CarPackExteriorColour)
    Implements IOverwritable

#Region " Business Properties & Methods "

    Private _overwritten As Boolean = False
    Private _status As Integer


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
    Public Function HasBeenOverwritten() As Boolean Implements IOverwritable.HasBeenOverwritten
        Return Overwritten
    End Function
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
        Return Not GenerationPackExteriorColour.CarSpecification.Matches(Pack.Car)
    End Function

    Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If propertyName.Equals("Overwritten") Then Return True
        If CanNotBeFitted() Then Return False
        Return MyBase.CanWriteProperty(propertyName)
    End Function
 
    Public Property Approved() As Boolean
        Get
            If CanNotBeFitted() Then Return False
            If ExteriorColourType.Availability = Availability.NotAvailable Then Return False

            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Set(ByVal value As Boolean)
            If CanNotBeFitted() Then Throw New SecurityException("Can not change the approval because the colour is not available anyway")
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
            If CanNotBeFitted() Then Throw New SecurityException("Can not change the approval because the colour is not available anyway")
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
        DirectCast(Parent, CarPackExteriorColours).PropertyChanged(e)
    End Sub

#End Region

#Region " Reference Properties & Methods "
    Private WithEvents _generationPackExteriorColour As ModelGenerationPackExteriorColour



    Private ReadOnly Property Pack() As CarPack
        Get
            If ExteriorColourType Is Nothing Then Return Nothing
            Return ExteriorColourType.Pack
        End Get
    End Property

    Public ReadOnly Property ExteriorColourType() As CarPackExteriorColourType
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, CarPackExteriorColours).ExteriorColourType
        End Get
    End Property

    Friend Property GenerationPackExteriorColour() As ModelGenerationPackExteriorColour
        Get
            Return _generationPackExteriorColour
        End Get
        Set(value As ModelGenerationPackExteriorColour)
            _generationPackExteriorColour = value
            RefreshParentProperties()
        End Set
    End Property
    Private Sub OngenerationPackExteriorColourPropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Handles _generationPackExteriorColour.PropertyChanged
        If e.PropertyName.Length = 0 Then Exit Sub

        RefreshParentProperties()
    End Sub
    Private Sub RefreshParentProperties()

        If Overwritten Then
            If CanNotBeFitted() Then Revert()
            Return
        End If

        If GenerationPackExteriorColour.Approved Then
            _status = Status.ApprovedForLive
        Else
            _status = Status.Declined
        End If
    End Sub

    Public ReadOnly Property Code() As String
        Get
            Return GenerationPackExteriorColour.Code
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return GenerationPackExteriorColour.Name
        End Get
    End Property

    Public ReadOnly Property AlternateName() As String
        Get
            Return GenerationPackExteriorColour.AlternateName
        End Get
    End Property
    Public ReadOnly Property Translation() As Translations.Translation
        Get
            Return GenerationPackExteriorColour.Translation
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

    Public Overloads Function Equals(ByVal obj As CarPackExteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID) AndAlso Pack.ID.Equals(obj.Pack.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is CarPackExteriorColour Then
            Equals(DirectCast(obj, CarPackExteriorColour))
        Else
            Return GetIdValue().Equals(obj)
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Public Shared Function NewCarPackExteriorColour(ByVal generationPackExteriorColour As ModelGenerationPackExteriorColour) As CarPackExteriorColour
        Dim packExteriorColour = New CarPackExteriorColour
        packExteriorColour.Create(generationPackExteriorColour)
        Return packExteriorColour
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
    Private Overloads Sub Create(ByVal packExteriorColour As ModelGenerationPackExteriorColour)
        Create(packExteriorColour.ID)
        GenerationPackExteriorColour = packExteriorColour
        RefreshParentProperties()
        MarkClean()
    End Sub
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("EXTERIORCOLOURID")
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
        command.Parameters.AddWithValue("@EXTERIORCOLOURID", ID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateCarPackExteriorColour"
        command.Parameters.AddWithValue("@STATUSID", _status)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        If Not Overwritten Then
            command.CommandText = "deleteCarPackExteriorColour"
            Exit Sub
        End If

        command.CommandText = "updateCarPackExteriorColour"
        command.Parameters.AddWithValue("@STATUSID", _status)
    End Sub


#End Region




End Class