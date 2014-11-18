Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class ModelGenerationSubModelDefaultFittings
    Inherits BaseObjects.ContextUniqueGuidListBase(Of ModelGenerationSubModelDefaultFittings, ModelGenerationSubModelDefaultFitting)

#Region " Business Properties & Methods "
    Friend Property SubModel() As ModelGenerationSubModel
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationSubModel)
        End Get
        Private Set(ByVal value As ModelGenerationSubModel)
            Me.SetParent(value)
        End Set
    End Property
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetFittings(ByVal obj As ModelGenerationSubModel) As ModelGenerationSubModelDefaultFittings
        If obj.IsNew Then Return ModelGenerationSubModelDefaultFittings.NewFittings(obj)

        Dim _fittings As ModelGenerationSubModelDefaultFittings = DataPortal.Fetch(Of ModelGenerationSubModelDefaultFittings)(New ParentCriteria(obj.ID, "@SUBMODELID"))
        _fittings.SubModel = obj
        Return _fittings
    End Function
    Private Shared Function NewFittings(ByVal obj As ModelGenerationSubModel) As ModelGenerationSubModelDefaultFittings
        Dim _fittings As ModelGenerationSubModelDefaultFittings = New ModelGenerationSubModelDefaultFittings()
        _fittings.SubModel = obj
        Return _fittings
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
    End Sub
#End Region

End Class

<Serializable()> Public NotInheritable Class ModelGenerationSubModelDefaultFitting
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of ModelGenerationSubModelDefaultFitting)

#Region " Business Properties & Methods "

    Private _key As String = String.Empty
    Private _bodyType As BodyTypeInfo = Nothing
    Private _engine As EngineInfo = Nothing
    Private _grade As GradeInfo = Nothing
    Private _transmission As TransmissionInfo = Nothing
    Private _steering As SteeringInfo = Nothing
    Private _wheelDrive As WheelDriveInfo = Nothing


    Private ReadOnly Property SubModel() As ModelGenerationSubModel
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationSubModelDefaultFittings).SubModel
        End Get
    End Property

    Private Property Key() As String
        Get
            If Me.Parent Is Nothing Then Return _key
            If _key.Length = 0 Then _key = GetPartialCarSpecification().ToString()
            Return _key
        End Get
        Set(ByVal value As String)
            _key = value
            ValidationRules.CheckRules("Key")
        End Set
    End Property

    Public Property BodyType() As BodyTypeInfo
        Get
            Return _bodyType
        End Get
        Set(ByVal value As BodyTypeInfo)
            If _bodyType Is Nothing OrElse Not _bodyType.Equals(value) Then
                _bodyType = value
                PropertyHasChanged("BodyType")
            End If
        End Set
    End Property

    Public Property Engine() As EngineInfo
        Get
            Return _engine
        End Get
        Set(ByVal value As EngineInfo)
            If _engine Is Nothing OrElse Not _engine.Equals(value) Then
                _engine = value
                PropertyHasChanged("Engine")
            End If
        End Set
    End Property

    Public Property Grade() As GradeInfo
        Get
            Return _grade
        End Get
        Set(ByVal value As GradeInfo)
            If _grade Is Nothing OrElse Not _grade.Equals(value) Then
                _grade = value
                PropertyHasChanged("Grade")
            End If
        End Set
    End Property

    Public Property Transmission() As TransmissionInfo
        Get
            Return _transmission
        End Get
        Set(ByVal value As TransmissionInfo)
            If _transmission Is Nothing OrElse Not _transmission.Equals(value) Then
                _transmission = value
                PropertyHasChanged("Transmission")
            End If
        End Set
    End Property

    Public Property WheelDrive() As WheelDriveInfo
        Get
            Return _wheelDrive
        End Get
        Set(ByVal value As WheelDriveInfo)
            If _wheelDrive Is Nothing OrElse Not _wheelDrive.Equals(value) Then
                _wheelDrive = value
                PropertyHasChanged("WheelDrive")
            End If
        End Set
    End Property

    Public Property Steering() As SteeringInfo
        Get
            Return _steering
        End Get
        Set(ByVal value As SteeringInfo)
            If _steering Is Nothing OrElse Not _steering.Equals(value) Then
                _steering = value
                PropertyHasChanged("Steering")
            End If
        End Set
    End Property


    Protected Overrides Sub PropertyHasChanged(ByVal propertyName As String)
        MyBase.PropertyHasChanged(propertyName)

        If Me.IsBaseDirty Then
            Me.Key = GetPartialCarSpecification().ToString()

        End If

    End Sub

    Friend Function Matches(ByVal car As Car) As Boolean
        Return GetPartialCarSpecification().Matches(car.PartialCarSpecification)
    End Function
    Private Function GetPartialCarSpecification() As PartialCarSpecification
        Dim _carSpecification As PartialCarSpecification = PartialCarSpecification.NewPartialCarSpecification
        With _carSpecification
            .ModelID = Me.SubModel.Generation.Model.ID
            .GenerationID = Me.SubModel.Generation.ID
            If Not Me.BodyType Is Nothing Then
                .BodyTypeID = Me.BodyType.ID
                .BodyTypeName = Me.BodyType.Name
            End If
            If Not Me.Engine Is Nothing Then
                .EngineID = Me.Engine.ID
                .EngineName = Me.Engine.Name
            End If
            If Not Me.Grade Is Nothing Then
                .GradeID = Me.Grade.ID
                .GradeName = Me.Grade.Name
            End If
            If Not Me.Transmission Is Nothing Then
                .TransmissionID = Me.Transmission.ID
                .TransmissionName = Me.Transmission.Name
            End If
            If Not Me.WheelDrive Is Nothing Then
                .WheelDriveID = Me.WheelDrive.ID
                .WheelDriveName = Me.WheelDrive.Name
            End If
            If Not Me.Steering Is Nothing Then
                .SteeringID = Me.Steering.ID
                .SteeringName = Me.Steering.Name
            End If
        End With
        Return (_carSpecification)
    End Function

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Key", "Empty fittings are not allowed.")
        ValidationRules.AddRule(DirectCast(AddressOf ModelGenerationSubModelDefaultFitting.KeyUnique, Validation.RuleHandler), "Key")
    End Sub


    Private Shared Function KeyUnique(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _fitting As ModelGenerationSubModelDefaultFitting = DirectCast(target, ModelGenerationSubModelDefaultFitting)
        If _fitting.Parent Is Nothing Then Return True

        If DirectCast(_fitting.Parent, ModelGenerationSubModelDefaultFittings).Any(Function(x) x.Key.Equals(_fitting.Key) AndAlso Not x.ID.Equals(_fitting.ID)) Then
            e.Description = "This fitting has already been defined"
            Return False
        End If

        Return True
    End Function




#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Key
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationSubModelDefaultFitting) As Boolean
        Return Not (obj Is Nothing) AndAlso (Me.ID.Equals(obj.ID) OrElse Me.Key.Equals(obj.Key))
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        _bodyType = BodyTypeInfo.Empty
        _engine = EngineInfo.Empty
        _grade = GradeInfo.Empty
        _transmission = TransmissionInfo.Empty
        _wheelDrive = WheelDriveInfo.Empty
        _steering = SteeringInfo.Empty
        MyBase.InitializeFields()
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _bodyType = BodyTypeInfo.GetBodyTypeInfo(dataReader)
        _engine = EngineInfo.GetEngineInfo(dataReader)
        _grade = GradeInfo.GetGradeInfo(dataReader)
        _transmission = TransmissionInfo.GetTransmissionInfo(dataReader)
        _wheelDrive = WheelDriveInfo.GetWheelDriveInfo(dataReader)
        _steering = SteeringInfo.GetSteeringInfo(dataReader)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command.Parameters
            .AddWithValue("@SUBMODELID", Me.SubModel.ID)
            .AddWithValue("@BODYID", Me.BodyType.ID.GetDbValue())
            .AddWithValue("@ENGINEID", Me.Engine.ID.GetDbValue())
            .AddWithValue("@GRADEID", Me.Grade.ID.GetDbValue())
            .AddWithValue("@TRANSMISSIONID", Me.Transmission.ID.GetDbValue())
            .AddWithValue("@WHEELDRIVEID", Me.WheelDrive.ID.GetDbValue())
            .AddWithValue("@STEERINGID", Me.Steering.ID.GetDbValue())
        End With
    End Sub
#End Region

End Class
