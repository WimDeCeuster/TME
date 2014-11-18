Imports TME.BusinessObjects.Validation

<Serializable()> Public Class PartialCarSpecification
    Inherits BusinessBase(Of PartialCarSpecification)

#Region " Business Properties & Methods "
    Protected _modelID As Guid = Guid.Empty
    Protected _generationID As Guid = Guid.Empty
    Protected _bodyTypeID As Guid = Guid.Empty
    Protected _engineID As Guid = Guid.Empty
    Protected _gradeID As Guid = Guid.Empty
    Protected _factoryGradeID As Guid = Guid.Empty
    Protected _transmissionID As Guid = Guid.Empty
    Protected _wheelDriveID As Guid = Guid.Empty
    Protected _steeringID As Guid = Guid.Empty
    Protected _versionID As Guid = Guid.Empty

    Protected _modelName As String = String.Empty
    Protected _generationName As String = String.Empty
    Protected _bodyTypeName As String = String.Empty
    Protected _engineName As String = String.Empty
    Protected _gradeName As String = String.Empty
    Protected _factoryGradeName As String = String.Empty
    Protected _transmissionName As String = String.Empty
    Protected _wheelDriveName As String = String.Empty
    Protected _steeringName As String = String.Empty

    Public Property ModelID() As Guid
        Get
            Return _modelID
        End Get
        Set(ByVal value As Guid)
            If Not _modelID.Equals(value) Then
                _modelID = value
                PropertyHasChanged("ModelID")
            End If
        End Set
    End Property
    Public Property GenerationID() As Guid
        Get
            Return _generationID
        End Get
        Set(ByVal value As Guid)
            If Not _generationID.Equals(value) Then
                Dim oldGenerationId = _generationID
                _generationID = value
                PropertyHasChanged("GenerationID", oldGenerationId, _generationID)
            End If
        End Set
    End Property
    Public Property BodyTypeID() As Guid
        Get
            Return _bodyTypeID
        End Get
        Set(ByVal value As Guid)
            If Not _bodyTypeID.Equals(value) Then
                _bodyTypeID = value
                PropertyHasChanged("BodyTypeID")
            End If
        End Set
    End Property
    Public Property EngineID() As Guid
        Get
            Return _engineID
        End Get
        Set(ByVal value As Guid)
            If Not _engineID.Equals(value) Then
                _engineID = value
                PropertyHasChanged("EngineID")
            End If
        End Set
    End Property
    Public Property GradeID() As Guid
        Get
            Return _gradeID
        End Get
        Set(ByVal value As Guid)
            If Not _gradeID.Equals(value) Then
                _gradeID = value
                PropertyHasChanged("GradeID")
            End If
        End Set
    End Property
    Public Property FactoryGradeID() As Guid
        Get
            Return _factoryGradeID
        End Get
        Set(ByVal value As Guid)
            If Not _factoryGradeID.Equals(value) Then
                _factoryGradeID = value
                PropertyHasChanged("FactoryGradeID")
            End If
        End Set
    End Property
    Public Property TransmissionID() As Guid
        Get
            Return _transmissionID
        End Get
        Set(ByVal value As Guid)
            If Not _transmissionID.Equals(value) Then
                _transmissionID = value
                PropertyHasChanged("TransmissionID")
            End If
        End Set
    End Property
    Public Property WheelDriveID() As Guid
        Get
            Return _wheelDriveID
        End Get
        Set(ByVal value As Guid)
            If Not _wheelDriveID.Equals(value) Then
                _wheelDriveID = value
                PropertyHasChanged("WheelDriveID")
            End If
        End Set
    End Property
    Public Property SteeringID() As Guid
        Get
            Return _steeringID
        End Get
        Set(ByVal value As Guid)
            If Not _steeringID.Equals(value) Then
                _steeringID = value
                PropertyHasChanged("SteeringID")
            End If
        End Set
    End Property
    Public Property VersionID() As Guid
        Get
            Return _versionID
        End Get
        Set(ByVal value As Guid)
            If Not _versionID.Equals(value) Then
                _versionID = value
                PropertyHasChanged("VersionID")
            End If
        End Set
    End Property

    Public Property ModelName() As String
        Get
            Return _modelName
        End Get
        Friend Set(ByVal value As String)
            _modelName = value
        End Set
    End Property
    Public Property GenerationName() As String
        Get
            Return _generationName
        End Get
        Friend Set(ByVal value As String)
            _generationName = value
        End Set
    End Property
    Public Property BodyTypeName() As String
        Get
            Return _bodyTypeName
        End Get
        Friend Set(ByVal value As String)
            _bodyTypeName = value
        End Set
    End Property
    Public Property EngineName() As String
        Get
            Return _engineName
        End Get
        Friend Set(ByVal value As String)
            _engineName = value
        End Set
    End Property
    Public Property GradeName() As String
        Get
            Return _gradeName
        End Get
        Friend Set(ByVal value As String)
            _gradeName = value
        End Set
    End Property
    Public Property FactoryGradeName() As String
        Get
            Return _factoryGradeName
        End Get
        Friend Set(ByVal value As String)
            _factoryGradeName = value
        End Set
    End Property
    Public Property TransmissionName() As String
        Get
            Return _transmissionName
        End Get
        Friend Set(ByVal value As String)
            _transmissionName = value
        End Set
    End Property
    Public Property WheelDriveName() As String
        Get
            Return _wheelDriveName
        End Get
        Friend Set(ByVal value As String)
            _wheelDriveName = value
        End Set
    End Property
    Public Property SteeringName() As String
        Get
            Return _steeringName
        End Get
        Friend Set(ByVal value As String)
            _steeringName = value
        End Set
    End Property

    Public Shadows Function Clone() As PartialCarSpecification
        Dim _clone As PartialCarSpecification = New PartialCarSpecification()
        _clone._modelID = _modelID
        _clone._generationID = _generationID
        _clone._bodyTypeID = _bodyTypeID
        _clone._engineID = _engineID
        _clone._gradeID = _gradeID
        _clone._factoryGradeID = _factoryGradeID
        _clone._transmissionID = _transmissionID
        _clone._wheelDriveID = _wheelDriveID
        _clone._steeringID = _steeringID
        _clone._versionID = _versionID

        _clone._modelName = _modelName
        _clone._generationName = _generationName
        _clone._bodyTypeName = _bodyTypeName
        _clone._engineName = _engineName
        _clone._gradeName = _gradeName
        _clone._factoryGradeName = _factoryGradeName
        _clone._transmissionName = _transmissionName
        _clone._wheelDriveName = _wheelDriveName
        _clone._steeringName = _steeringName
        Return _clone
    End Function


#End Region

#Region " IsEmpty "
    Public Overridable Function IsEmpty() As Boolean
        If Not Me.ModelID.Equals(Guid.Empty) Then Return False
        If Not Me.GenerationID.Equals(Guid.Empty) Then Return False
        If Not Me.BodyTypeID.Equals(Guid.Empty) Then Return False
        If Not Me.EngineID.Equals(Guid.Empty) Then Return False
        If Not Me.GradeID.Equals(Guid.Empty) Then Return False
        If Not Me.FactoryGradeID.Equals(Guid.Empty) Then Return False
        If Not Me.TransmissionID.Equals(Guid.Empty) Then Return False
        If Not Me.WheelDriveID.Equals(Guid.Empty) Then Return False
        If Not Me.SteeringID.Equals(Guid.Empty) Then Return False
        If Not Me.VersionID.Equals(Guid.Empty) Then Return False
        Return True
    End Function
#End Region

#Region " Matches "
    Public Enum MatchType
        OuterJoin = 0
        InnerJoin = 1
        LeftJoin = 2
        RightJoin = 3
    End Enum
    Public Function Matches(ByVal carSpecification As PartialCarSpecification, Optional ByVal sameGenerationGuaranteed As Boolean = False) As Boolean
        Return MatchesOuterJoin(carSpecification, sameGenerationGuaranteed)
    End Function
    Public Function Matches(ByVal carSpecification As PartialCarSpecification, ByVal matchType As MatchType) As Boolean
        Select Case matchType
            Case matchType.OuterJoin
                Return MatchesOuterJoin(carSpecification)
            Case matchType.InnerJoin
                Return MatchesInnerJoin(carSpecification)
            Case matchType.LeftJoin
                Return MatchesLeftJoin(carSpecification)
            Case matchType.RightJoin
                Return MatchesRightJoin(carSpecification)
        End Select
    End Function

    Private Function MatchesOuterJoin(ByVal carSpecification As PartialCarSpecification, Optional ByVal sameGenerationGuaranteed As Boolean = False) As Boolean
        If Not sameGenerationGuaranteed Then
            If Not (Me.ModelID.Equals(Guid.Empty) OrElse carSpecification.ModelID.Equals(Guid.Empty) OrElse Me.ModelID.Equals(carSpecification.ModelID)) Then Return False
            If Not (Me.GenerationID.Equals(Guid.Empty) OrElse carSpecification.GenerationID.Equals(Guid.Empty) OrElse Me.GenerationID.Equals(carSpecification.GenerationID)) Then Return False
        End If
        If Not (Me.BodyTypeID.Equals(Guid.Empty) OrElse carSpecification.BodyTypeID.Equals(Guid.Empty) OrElse Me.BodyTypeID.Equals(carSpecification.BodyTypeID)) Then Return False
        If Not (Me.EngineID.Equals(Guid.Empty) OrElse carSpecification.EngineID.Equals(Guid.Empty) OrElse Me.EngineID.Equals(carSpecification.EngineID)) Then Return False
        If Not (Me.GradeID.Equals(Guid.Empty) OrElse carSpecification.GradeID.Equals(Guid.Empty) OrElse Me.GradeID.Equals(carSpecification.GradeID)) Then Return False
        If Not (Me.FactoryGradeID.Equals(Guid.Empty) OrElse carSpecification.FactoryGradeID.Equals(Guid.Empty) OrElse Me.FactoryGradeID.Equals(carSpecification.FactoryGradeID)) Then Return False
        If Not (Me.TransmissionID.Equals(Guid.Empty) OrElse carSpecification.TransmissionID.Equals(Guid.Empty) OrElse Me.TransmissionID.Equals(carSpecification.TransmissionID)) Then Return False
        If Not (Me.WheelDriveID.Equals(Guid.Empty) OrElse carSpecification.WheelDriveID.Equals(Guid.Empty) OrElse Me.WheelDriveID.Equals(carSpecification.WheelDriveID)) Then Return False
        If Not (Me.SteeringID.Equals(Guid.Empty) OrElse carSpecification.SteeringID.Equals(Guid.Empty) OrElse Me.SteeringID.Equals(carSpecification.SteeringID)) Then Return False
        If Not (Me.VersionID.Equals(Guid.Empty) OrElse carSpecification.VersionID.Equals(Guid.Empty) OrElse Me.VersionID.Equals(carSpecification.VersionID)) Then Return False
        Return True
    End Function
    Private Function MatchesInnerJoin(ByVal carSpecification As PartialCarSpecification) As Boolean
        If Not (Me.ModelID.Equals(carSpecification.ModelID)) Then Return False
        If Not (Me.GenerationID.Equals(carSpecification.GenerationID)) Then Return False
        If Not (Me.BodyTypeID.Equals(carSpecification.BodyTypeID)) Then Return False
        If Not (Me.EngineID.Equals(carSpecification.EngineID)) Then Return False
        If Not (Me.GradeID.Equals(carSpecification.GradeID)) Then Return False
        If Not (Me.FactoryGradeID.Equals(carSpecification.FactoryGradeID)) Then Return False
        If Not (Me.TransmissionID.Equals(carSpecification.TransmissionID)) Then Return False
        If Not (Me.WheelDriveID.Equals(carSpecification.WheelDriveID)) Then Return False
        If Not (Me.SteeringID.Equals(carSpecification.SteeringID)) Then Return False
        If Not (Me.VersionID.Equals(carSpecification.VersionID)) Then Return False
        Return True
    End Function
    Private Function MatchesLeftJoin(ByVal carSpecification As PartialCarSpecification) As Boolean
        If Not (Me.ModelID.Equals(Guid.Empty) OrElse Me.ModelID.Equals(carSpecification.ModelID)) Then Return False
        If Not (Me.GenerationID.Equals(Guid.Empty) OrElse Me.GenerationID.Equals(carSpecification.GenerationID)) Then Return False
        If Not (Me.BodyTypeID.Equals(Guid.Empty) OrElse Me.BodyTypeID.Equals(carSpecification.BodyTypeID)) Then Return False
        If Not (Me.EngineID.Equals(Guid.Empty) OrElse Me.EngineID.Equals(carSpecification.EngineID)) Then Return False
        If Not (Me.GradeID.Equals(Guid.Empty) OrElse Me.GradeID.Equals(carSpecification.GradeID)) Then Return False
        If Not (Me.FactoryGradeID.Equals(Guid.Empty) OrElse Me.FactoryGradeID.Equals(carSpecification.FactoryGradeID)) Then Return False
        If Not (Me.TransmissionID.Equals(Guid.Empty) OrElse Me.TransmissionID.Equals(carSpecification.TransmissionID)) Then Return False
        If Not (Me.WheelDriveID.Equals(Guid.Empty) OrElse Me.WheelDriveID.Equals(carSpecification.WheelDriveID)) Then Return False
        If Not (Me.SteeringID.Equals(Guid.Empty) OrElse Me.SteeringID.Equals(carSpecification.SteeringID)) Then Return False
        If Not (Me.VersionID.Equals(Guid.Empty) OrElse Me.VersionID.Equals(carSpecification.VersionID)) Then Return False
        Return True
    End Function
    Private Function MatchesRightJoin(ByVal carSpecification As PartialCarSpecification) As Boolean
        If Not (carSpecification.ModelID.Equals(Guid.Empty) OrElse Me.ModelID.Equals(carSpecification.ModelID)) Then Return False
        If Not (carSpecification.GenerationID.Equals(Guid.Empty) OrElse Me.GenerationID.Equals(carSpecification.GenerationID)) Then Return False
        If Not (carSpecification.BodyTypeID.Equals(Guid.Empty) OrElse Me.BodyTypeID.Equals(carSpecification.BodyTypeID)) Then Return False
        If Not (carSpecification.EngineID.Equals(Guid.Empty) OrElse Me.EngineID.Equals(carSpecification.EngineID)) Then Return False
        If Not (carSpecification.GradeID.Equals(Guid.Empty) OrElse Me.GradeID.Equals(carSpecification.GradeID)) Then Return False
        If Not (carSpecification.FactoryGradeID.Equals(Guid.Empty) OrElse Me.FactoryGradeID.Equals(carSpecification.FactoryGradeID)) Then Return False
        If Not (carSpecification.TransmissionID.Equals(Guid.Empty) OrElse Me.TransmissionID.Equals(carSpecification.TransmissionID)) Then Return False
        If Not (carSpecification.WheelDriveID.Equals(Guid.Empty) OrElse Me.WheelDriveID.Equals(carSpecification.WheelDriveID)) Then Return False
        If Not (carSpecification.SteeringID.Equals(Guid.Empty) OrElse Me.SteeringID.Equals(carSpecification.SteeringID)) Then Return False
        If Not (carSpecification.VersionID.Equals(Guid.Empty) OrElse Me.VersionID.Equals(carSpecification.VersionID)) Then Return False
        Return True
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Dim oStringBuilder As System.Text.StringBuilder = New System.Text.StringBuilder
        If Me.ModelName.Length > 0 Then oStringBuilder.Append(Me.ModelName & ", ")
        If Me.GenerationName.Length > 0 Then oStringBuilder.Append(Me.GenerationName & ", ")
        If Me.BodyTypeName.Length > 0 Then oStringBuilder.Append(Me.BodyTypeName & ", ")
        If Me.EngineName.Length > 0 Then oStringBuilder.Append(Me.EngineName & ", ")
        If Me.WheelDriveName.Length > 0 Then oStringBuilder.Append(Me.WheelDriveName & ", ")
        If Me.TransmissionName.Length > 0 Then oStringBuilder.Append(Me.TransmissionName & ", ")
        If Me.FactoryGradeName.Length > 0 Then oStringBuilder.Append(Me.FactoryGradeName & ", ")
        If Me.GradeName.Length > 0 Then oStringBuilder.Append(Me.GradeName & ", ")
        If Me.SteeringName.Length > 0 Then oStringBuilder.Append(Me.SteeringName & ", ")

        Dim sBuffer As String = oStringBuilder.ToString()
        If sBuffer.Length > 0 Then
            sBuffer = sBuffer.Remove(sBuffer.Length - 2, 2)
        ElseIf _modelID.Equals(Guid.Empty) Then
            sBuffer = "All cars"
        Else
            sBuffer = MyBase.ToString()
        End If
        Return sBuffer

    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Dim l As Long = 0
        l += Me.ModelID.GetHashCode
        l += Me.GenerationID.GetHashCode
        l += Me.BodyTypeID.GetHashCode
        l += Me.EngineID.GetHashCode
        l += Me.GradeID.GetHashCode
        l += Me.FactoryGradeID.GetHashCode
        l += Me.TransmissionID.GetHashCode
        l += Me.SteeringID.GetHashCode
        l += Me.VersionID.GetHashCode
        Return l.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As PartialCarSpecification) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.MatchesInnerJoin(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is PartialCarSpecification Then
            Return Me.Equals(DirectCast(obj, PartialCarSpecification))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Framework Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return Me.GetHashCode()
    End Function
#End Region

#Region " Shared Factory Methods "
    Public Shared Function NewPartialCarSpecification() As PartialCarSpecification
        Return New PartialCarSpecification
    End Function
    Public Shared Function NewPartialCarSpecification(ByVal modelID As Guid) As PartialCarSpecification
        Dim _partialCarSpecification As PartialCarSpecification = New PartialCarSpecification()
        _partialCarSpecification._modelID = modelID
        Return _partialCarSpecification
    End Function
    Public Shared Function NewPartialCarSpecification(ByVal modelID As Guid, ByVal generationID As Guid) As PartialCarSpecification
        Dim _partialCarSpecification As PartialCarSpecification = New PartialCarSpecification()
        _partialCarSpecification._modelID = modelID
        _partialCarSpecification._generationID = generationID
        Return _partialCarSpecification
    End Function
    Public Shared Function NewPartialCarSpecification(ByVal car As Car) As PartialCarSpecification
        Dim _partialCarSpecification As PartialCarSpecification = New PartialCarSpecification()
        With _partialCarSpecification
            ._modelID = car.Generation.Model.ID
            ._generationID = car.Generation.ID
            ._bodyTypeID = car.BodyTypeID
            ._engineID = car.EngineID
            ._gradeID = car.GradeID
            ._factoryGradeID = car.FactoryGradeID
            ._transmissionID = car.TransmissionID
            ._wheelDriveID = car.WheelDriveID
            ._steeringID = car.SteeringID
            ._versionID = car.VersionID
        End With
        Return _partialCarSpecification
    End Function
    Public Shared Function NewFactoryCarSpecification(ByVal car As Car) As PartialCarSpecification
        Dim _partialCarSpecification As PartialCarSpecification = New PartialCarSpecification()
        With _partialCarSpecification

            ._modelID = car.Generation.Model.ID
            ._generationID = car.Generation.ID
            ._bodyTypeID = car.FactoryBodyTypeID
            ._engineID = car.FactoryEngineID
            ._gradeID = car.GradeID
            ._factoryGradeID = car.FactoryGradeID
            ._transmissionID = car.FactoryTransmissionID
            ._wheelDriveID = car.WheelDriveID
            ._steeringID = car.SteeringID
            ._versionID = car.VersionID
        End With
        Return _partialCarSpecification
    End Function

    Friend Shared Function GetPartialCarSpecification(ByVal dataReader As SafeDataReader) As PartialCarSpecification
        Dim _partialCarSpecification As PartialCarSpecification = New PartialCarSpecification()
        _partialCarSpecification.Fetch(dataReader)
        Return _partialCarSpecification
    End Function

#End Region

#Region " Constructors "
    Protected Sub New()
        MarkAsChild()
        MarkOld()
    End Sub
#End Region

#Region " Data Access Helper Functions "
    Protected Friend Overridable Sub AppendParameters(ByVal command As SqlCommand)
        With command.Parameters
            .AddWithValue("@MODELID", Me.ModelID)
            .AddWithValue("@GENERATIONID", Me.GenerationID)
            .AddWithValue("@BODYID", Me.BodyTypeID)
            .AddWithValue("@ENGINEID", Me.EngineID)
            .AddWithValue("@GRADEID", Me.GradeID)
            .AddWithValue("@FACTORYGRADEID", Me.FactoryGradeID)
            .AddWithValue("@TRANSMISSIONID", Me.TransmissionID)
            .AddWithValue("@WHEELDRIVEID", Me.WheelDriveID)
            .AddWithValue("@STEERINGID", Me.SteeringID)
            .AddWithValue("@VERSIONID", Me.VersionID)
        End With
    End Sub
    Protected Overridable Sub Fetch(ByVal dataReader As SafeDataReader)

        With dataReader
            _modelID = .GetGuid("MODELID", Guid.Empty)
            _generationID = .GetGuid("GENERATIONID", Guid.Empty)
            _bodyTypeID = .GetGuid("BODYID", Guid.Empty)
            _engineID = .GetGuid("ENGINEID", Guid.Empty)
            _gradeID = .GetGuid("GRADEID", Guid.Empty)
            _factoryGradeID = .GetGuid("FACTORYGRADEID", Guid.Empty)
            _transmissionID = .GetGuid("TRANSMISSIONID", Guid.Empty)
            _wheelDriveID = .GetGuid("WHEELDRIVEID", Guid.Empty)
            _steeringID = .GetGuid("STEERINGID", Guid.Empty)
            _versionID = .GetGuid("VERSIONID", Guid.Empty)

            _modelName = .GetString("MODELNAME", String.Empty)
            _generationName = .GetString("GENERATIONNAME", String.Empty)
            _bodyTypeName = .GetString("BODYNAME", String.Empty)
            _engineName = .GetString("ENGINENAME", String.Empty)
            _gradeName = .GetString("GRADENAME", String.Empty)
            _factoryGradeName = .GetString("FACTORYGRADENAME", String.Empty)
            _transmissionName = .GetString("TRANSMISSIONNAME", String.Empty)
            _wheelDriveName = .GetString("WHEELDRIVENAME", String.Empty)
            _steeringName = .GetString("STEERINGNAME", String.Empty)

        End With
        MarkOld()
    End Sub
#End Region


End Class

<Serializable()> Public NotInheritable Class GenerationCarSpecification
    Inherits BusinessBase(Of GenerationCarSpecification)

#Region " Business Properties & Methods "
    Private _generation As ModelGeneration

    Private _bodyTypeID As Guid = Guid.Empty
    Private _engineID As Guid = Guid.Empty
    Private _gradeID As Guid = Guid.Empty
    Private _transmissionID As Guid = Guid.Empty
    Private _wheelDriveID As Guid = Guid.Empty
    Private _steeringID As Guid = Guid.Empty
    Private _versionID As Guid = Guid.Empty

    Friend Property Generation() As ModelGeneration
        Get
            Return _generation
        End Get
        Set(ByVal value As ModelGeneration)
            _generation = value
        End Set
    End Property
    Public Property BodyType() As BodyTypeInfo
        Get
            If _bodyTypeID.Equals(Guid.Empty) Then Return BodyTypeInfo.Empty
            Return Me.Generation.BodyTypes(_bodyTypeID).GetInfo()
        End Get
        Set(ByVal value As BodyTypeInfo)
            If Not _bodyTypeID.Equals(value.ID) Then
                _bodyTypeID = value.ID
                PropertyHasChanged("BodyType")
            End If
        End Set
    End Property
    Public Property Engine() As EngineInfo
        Get
            If _engineID.Equals(Guid.Empty) Then Return EngineInfo.Empty
            Return Me.Generation.Engines(_engineID).GetInfo()
        End Get
        Set(ByVal value As EngineInfo)
            If Not _engineID.Equals(value.ID) Then
                _engineID = value.ID
                PropertyHasChanged("Engine")
            End If
        End Set
    End Property
    Public Property Grade() As GradeInfo
        Get
            If _gradeID.Equals(Guid.Empty) Then Return GradeInfo.Empty
            Return Me.Generation.Grades(_gradeID).GetInfo()
        End Get
        Set(ByVal value As GradeInfo)
            If Not _gradeID.Equals(value.ID) Then
                _gradeID = value.ID
                PropertyHasChanged("Grade")
            End If
        End Set
    End Property
    Public Property Transmission() As TransmissionInfo
        Get
            If _transmissionID.Equals(Guid.Empty) Then Return TransmissionInfo.Empty
            Return Me.Generation.Transmissions(_transmissionID).GetInfo()
        End Get
        Set(ByVal value As TransmissionInfo)
            If Not _transmissionID.Equals(value.ID) Then
                _transmissionID = value.ID
                PropertyHasChanged("Transmission")
            End If
        End Set
    End Property
    Public Property WheelDrive() As WheelDriveInfo
        Get
            If _wheelDriveID.Equals(Guid.Empty) Then Return WheelDriveInfo.Empty
            Return Me.Generation.WheelDrives(_wheelDriveID).GetInfo()
        End Get
        Set(ByVal value As WheelDriveInfo)
            If Not _wheelDriveID.Equals(value.ID) Then
                _wheelDriveID = value.ID
                PropertyHasChanged("WheelDrive")
            End If
        End Set
    End Property
    Public Property Steering() As SteeringInfo
        Get
            If _steeringID.Equals(Guid.Empty) Then Return SteeringInfo.Empty
            Return Steerings.GetSteerings().Item(_steeringID).GetInfo()
        End Get
        Set(ByVal value As SteeringInfo)
            If Not _steeringID.Equals(value.ID) Then
                _steeringID = value.ID
                PropertyHasChanged("Steering")
            End If
        End Set
    End Property
    Public Property VersionID() As Guid
        Get
            Return _versionID
        End Get
        Set(ByVal value As Guid)
            If Not _versionID.Equals(value) Then
                _versionID = value
                PropertyHasChanged("VersionID")
            End If
        End Set
    End Property

    Friend Shadows Sub MarkOld()
        MyBase.MarkOld()
    End Sub

#End Region

#Region " IsEmpty "
    Public Function IsEmpty() As Boolean
        If Not _bodyTypeID.Equals(Guid.Empty) Then Return False
        If Not _engineID.Equals(Guid.Empty) Then Return False
        If Not _gradeID.Equals(Guid.Empty) Then Return False
        If Not _transmissionID.Equals(Guid.Empty) Then Return False
        If Not _wheelDriveID.Equals(Guid.Empty) Then Return False
        If Not _steeringID.Equals(Guid.Empty) Then Return False
        If Not _versionID.Equals(Guid.Empty) Then Return False
        Return True
    End Function
#End Region

#Region " Matches "
    
    Friend Function Matches(ByVal car As Car) As Boolean
        If Not Generation.Equals(car.Generation) Then Return False
        If Not BodyType.IsEmpty() AndAlso Not car.BodyType.Equals(BodyType) Then Return False
        If Not Engine.IsEmpty() AndAlso Not car.Engine.Equals(Engine) Then Return False
        If Not Transmission.IsEmpty() AndAlso Not car.Transmission.Equals(Transmission) Then Return False
        If Not WheelDrive.IsEmpty() AndAlso Not car.WheelDrive.Equals(WheelDrive) Then Return False
        If Not Grade.IsEmpty() AndAlso Not car.Grade.Equals(Grade) Then Return False
        If Not Steering.IsEmpty() AndAlso Not car.Steering.Equals(Steering) Then Return False
        If Not VersionID.Equals(Guid.Empty) AndAlso Not car.VersionID.Equals(VersionID) Then Return False
        Return True
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String

        Dim oStringBuilder As System.Text.StringBuilder = New System.Text.StringBuilder
        If Not _bodyTypeID.Equals(Guid.Empty) Then oStringBuilder.Append(Me.BodyType.Name & ", ")
        If Not _engineID.Equals(Guid.Empty) Then oStringBuilder.Append(Me.Engine.Name & ", ")
        If Not _wheelDriveID.Equals(Guid.Empty) Then oStringBuilder.Append(Me.WheelDrive.Name & ", ")
        If Not _transmissionID.Equals(Guid.Empty) Then oStringBuilder.Append(Me.Transmission.Name & ", ")
        If Not _gradeID.Equals(Guid.Empty) Then oStringBuilder.Append(Me.Grade.Name & ", ")
        If Not _steeringID.Equals(Guid.Empty) Then oStringBuilder.Append(Me.Steering.Name & ", ")

        Dim sBuffer As String = oStringBuilder.ToString()
        If sBuffer.Length > 0 Then
            sBuffer = sBuffer.Remove(sBuffer.Length - 2, 2)
        Else
            sBuffer = "All cars"
        End If
        Return sBuffer

    End Function
    Public Overloads Function ToString(ByVal alternateName As Boolean) As String
        If Not alternateName Then Return Me.ToString()

        Dim oStringBuilder As System.Text.StringBuilder = New System.Text.StringBuilder
        If Not _bodyTypeID.Equals(Guid.Empty) Then oStringBuilder.Append(Me.Generation.BodyTypes(_bodyTypeID).AlternateName & ", ")
        If Not _engineID.Equals(Guid.Empty) Then oStringBuilder.Append(Me.Generation.Engines(_engineID).AlternateName & ", ")
        If Not _wheelDriveID.Equals(Guid.Empty) Then oStringBuilder.Append(Me.Generation.WheelDrives(_wheelDriveID).AlternateName & ", ")
        If Not _transmissionID.Equals(Guid.Empty) Then oStringBuilder.Append(Me.Generation.Transmissions(_transmissionID).AlternateName & ", ")
        If Not _gradeID.Equals(Guid.Empty) Then oStringBuilder.Append(Me.Generation.Grades(_gradeID).AlternateName & ", ")
        If Not _steeringID.Equals(Guid.Empty) Then oStringBuilder.Append(Steerings.GetSteerings()(_steeringID).AlternateName & ", ")

        Dim sBuffer As String = oStringBuilder.ToString()
        If sBuffer.Length > 0 Then
            sBuffer = sBuffer.Remove(sBuffer.Length - 2, 2)
        Else
            sBuffer = "All cars"
        End If
        Return sBuffer

    End Function

    Public Overloads Overrides Function GetHashCode() As Integer
        Dim l As Long = 0
        l += Me.Generation.ID.GetHashCode
        l += _bodyTypeID.GetHashCode
        l += _engineID.GetHashCode
        l += _gradeID.GetHashCode
        l += _transmissionID.GetHashCode
        l += _steeringID.GetHashCode
        l += _versionID.GetHashCode
        Return l.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As GenerationCarSpecification) As Boolean
        If obj Is Nothing Then Return False

        If Not (Me.Generation.ID.Equals(obj.Generation.ID)) Then Return False
        If Not (Me._bodyTypeID.Equals(obj._bodyTypeID)) Then Return False
        If Not (Me._engineID.Equals(obj._engineID)) Then Return False
        If Not (Me._gradeID.Equals(obj._gradeID)) Then Return False
        If Not (Me._transmissionID.Equals(obj._transmissionID)) Then Return False
        If Not (Me._wheelDriveID.Equals(obj._wheelDriveID)) Then Return False
        If Not (Me._steeringID.Equals(obj._steeringID)) Then Return False
        If Not (Me._versionID.Equals(obj._versionID)) Then Return False
        Return True
    End Function
    Public Overloads Function Equals(ByVal obj As PartialCarSpecification) As Boolean
        If obj Is Nothing Then Return False

        If Not (Me.Generation.ID.Equals(obj.GenerationID)) Then Return False
        If Not (Me._bodyTypeID.Equals(obj.BodyTypeID)) Then Return False
        If Not (Me._engineID.Equals(obj.EngineID)) Then Return False
        If Not (Me._gradeID.Equals(obj.GradeID)) Then Return False
        If Not (Me._transmissionID.Equals(obj.TransmissionID)) Then Return False
        If Not (Me._wheelDriveID.Equals(obj.WheelDriveID)) Then Return False
        If Not (Me._steeringID.Equals(obj.SteeringID)) Then Return False
        If Not (Me._versionID.Equals(obj.VersionID)) Then Return False
        Return True
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is GenerationCarSpecification Then
            Return Me.Equals(DirectCast(obj, GenerationCarSpecification))
        ElseIf TypeOf obj Is PartialCarSpecification Then
            Return Me.Equals(DirectCast(obj, PartialCarSpecification))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Framework Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return Me.GetHashCode()
    End Function
#End Region

#Region " Shared Factory Methods "
    Public Shared Function NewCarSpecification(ByVal generation As ModelGeneration) As GenerationCarSpecification
        Dim _specification As GenerationCarSpecification = New GenerationCarSpecification()
        _specification.Generation = generation
        Return _specification
    End Function

    Friend Shared Function GetCarSpecification(ByVal dataReader As SafeDataReader) As GenerationCarSpecification
        Dim _specification As GenerationCarSpecification = New GenerationCarSpecification()
        _specification.Fetch(dataReader)
        Return _specification
    End Function

#End Region

#Region " Constructors "

    Private Sub New()
        MarkAsChild()
        MarkOld()
    End Sub
#End Region

#Region " Data Access Helper Functions "

    Friend Sub AppendParameters(ByVal command As SqlCommand)
        With command.Parameters
            .AddWithValue("@BODYID", _bodyTypeID)
            .AddWithValue("@ENGINEID", _engineID)
            .AddWithValue("@GRADEID", _gradeID)
            .AddWithValue("@TRANSMISSIONID", _transmissionID)
            .AddWithValue("@WHEELDRIVEID", _wheelDriveID)
            .AddWithValue("@STEERINGID", _steeringID)
            .AddWithValue("@VERSIONID", _versionID)
        End With
    End Sub

    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _bodyTypeID = .GetGuid("BODYID", Guid.Empty)
            _engineID = .GetGuid("ENGINEID", Guid.Empty)
            _gradeID = .GetGuid("GRADEID", Guid.Empty)
            _transmissionID = .GetGuid("TRANSMISSIONID", Guid.Empty)
            _wheelDriveID = .GetGuid("WHEELDRIVEID", Guid.Empty)
            _steeringID = .GetGuid("STEERINGID", Guid.Empty)
            _versionID = .GetGuid("VERSIONID", Guid.Empty)
        End With
        MarkOld()
    End Sub
#End Region

End Class

<Serializable()> Public NotInheritable Class PartialFactoryCarSpecification
    Inherits BusinessBase(Of PartialFactoryCarSpecification)

#Region " Business Properties & Methods "
    Private _factoryGeneration As FactoryGenerationInfo = FactoryGenerationInfo.Empty
    Private _factoryGenerationID As Guid = Guid.Empty
    Private _bodyTypeID As Guid = Guid.Empty
    Private _engineID As Guid = Guid.Empty
    Private _factoryGradeID As Guid = Guid.Empty
    Private _transmissionID As Guid = Guid.Empty
    Private _wheelDriveID As Guid = Guid.Empty
    Private _steeringID As Guid = Guid.Empty
    Private _productionPlantID As Guid = Guid.Empty
    Private _destinationID As Guid = Guid.Empty


    Public Property FactoryGeneration() As FactoryGenerationInfo
        Get
            If _factoryGeneration.IsEmpty() AndAlso Not _factoryGenerationID.Equals(Guid.Empty) Then
                _factoryGeneration = Administration.FactoryGeneration.GetFactoryGeneration(_factoryGenerationID).GetInfo()
            End If
            Return _factoryGeneration
        End Get
        Set(ByVal value As FactoryGenerationInfo)
            If _factoryGenerationID.Equals(value.ID) Then Return

            _factoryGenerationID = value.ID
            _factoryGeneration = value
            PropertyHasChanged("FactoryGenerationID")
        End Set
    End Property

    Public Property FactoryGenerationID() As Guid
        Get
            Return _factoryGenerationID
        End Get
        Set(ByVal value As Guid)
            If _factoryGenerationID.Equals(value) Then Return

            _factoryGenerationID = value
            _factoryGeneration = FactoryGenerationInfo.Empty
            PropertyHasChanged("FactoryGenerationID")
        End Set
    End Property
    Public Property BodyTypeID() As Guid
        Get
            Return _bodyTypeID
        End Get
        Set(ByVal value As Guid)
            If Not _bodyTypeID.Equals(value) Then
                _bodyTypeID = value
                PropertyHasChanged("BodyTypeID")
            End If
        End Set
    End Property
    Public Property EngineID() As Guid
        Get
            Return _engineID
        End Get
        Set(ByVal value As Guid)
            If Not _engineID.Equals(value) Then
                _engineID = value
                PropertyHasChanged("EngineID")
            End If
        End Set
    End Property
    Public Property FactoryGradeID() As Guid
        Get
            Return _factoryGradeID
        End Get
        Set(ByVal value As Guid)
            If Not _factoryGradeID.Equals(value) Then
                _factoryGradeID = value
                PropertyHasChanged("FactoryGradeID")
            End If
        End Set
    End Property
    Public Property TransmissionID() As Guid
        Get
            Return _transmissionID
        End Get
        Set(ByVal value As Guid)
            If Not _transmissionID.Equals(value) Then
                _transmissionID = value
                PropertyHasChanged("TransmissionID")
            End If
        End Set
    End Property
    Public Property WheelDriveID() As Guid
        Get
            Return _wheelDriveID
        End Get
        Set(ByVal value As Guid)
            If Not _wheelDriveID.Equals(value) Then
                _wheelDriveID = value
                PropertyHasChanged("WheelDriveID")
            End If
        End Set
    End Property
    Public Property SteeringID() As Guid
        Get
            Return _steeringID
        End Get
        Set(ByVal value As Guid)
            If Not _steeringID.Equals(value) Then
                _steeringID = value
                PropertyHasChanged("SteeringID")
            End If
        End Set
    End Property
    Public Property ProductionPlantID() As Guid
        Get
            Return _productionPlantID
        End Get
        Set(ByVal value As Guid)
            If Not _productionPlantID.Equals(value) Then
                _productionPlantID = value
                PropertyHasChanged("ProductionPlantID")
            End If
        End Set
    End Property
    Public Property DestinationID() As Guid
        Get
            Return _destinationID
        End Get
        Set(ByVal value As Guid)
            If Not _destinationID.Equals(value) Then
                _destinationID = value
                PropertyHasChanged("DestinationID")
            End If
        End Set
    End Property

#End Region

#Region " Matches "
    Public Enum MatchType
        OuterJoin = 0
        InnerJoin = 1
        LeftJoin = 2
        RightJoin = 3
    End Enum
    Public Function Matches(ByVal carSpecification As PartialFactoryCarSpecification) As Boolean
        Return MatchesOuterJoin(carSpecification)
    End Function
    Public Function Matches(ByVal carSpecification As PartialFactoryCarSpecification, ByVal matchType As MatchType) As Boolean
        Select Case matchType
            Case matchType.OuterJoin
                Return MatchesOuterJoin(carSpecification)
            Case matchType.InnerJoin
                Return MatchesInnerJoin(carSpecification)
            Case matchType.LeftJoin
                Return MatchesLeftJoin(carSpecification)
            Case matchType.RightJoin
                Return MatchesRigthJoin(carSpecification)
        End Select
    End Function

    Private Function MatchesOuterJoin(ByVal carSpecification As PartialFactoryCarSpecification) As Boolean
        If Not (Me.FactoryGenerationID.Equals(Guid.Empty) OrElse carSpecification.FactoryGenerationID.Equals(Guid.Empty) OrElse Me.FactoryGenerationID.Equals(carSpecification.FactoryGenerationID)) Then Return False
        If Not (Me.BodyTypeID.Equals(Guid.Empty) OrElse carSpecification.BodyTypeID.Equals(Guid.Empty) OrElse Me.BodyTypeID.Equals(carSpecification.BodyTypeID)) Then Return False
        If Not (Me.EngineID.Equals(Guid.Empty) OrElse carSpecification.EngineID.Equals(Guid.Empty) OrElse Me.EngineID.Equals(carSpecification.EngineID)) Then Return False
        If Not (Me.FactoryGradeID.Equals(Guid.Empty) OrElse carSpecification.FactoryGradeID.Equals(Guid.Empty) OrElse Me.FactoryGradeID.Equals(carSpecification.FactoryGradeID)) Then Return False
        If Not (Me.TransmissionID.Equals(Guid.Empty) OrElse carSpecification.TransmissionID.Equals(Guid.Empty) OrElse Me.TransmissionID.Equals(carSpecification.TransmissionID)) Then Return False
        If Not (Me.WheelDriveID.Equals(Guid.Empty) OrElse carSpecification.WheelDriveID.Equals(Guid.Empty) OrElse Me.WheelDriveID.Equals(carSpecification.WheelDriveID)) Then Return False
        If Not (Me.SteeringID.Equals(Guid.Empty) OrElse carSpecification.SteeringID.Equals(Guid.Empty) OrElse Me.SteeringID.Equals(carSpecification.SteeringID)) Then Return False
        If Not (Me.ProductionPlantID.Equals(Guid.Empty) OrElse carSpecification.ProductionPlantID.Equals(Guid.Empty) OrElse Me.ProductionPlantID.Equals(carSpecification.ProductionPlantID)) Then Return False
        If Not (Me.DestinationID.Equals(Guid.Empty) OrElse carSpecification.DestinationID.Equals(Guid.Empty) OrElse Me.DestinationID.Equals(carSpecification.DestinationID)) Then Return False
        Return True
    End Function
    Private Function MatchesInnerJoin(ByVal carSpecification As PartialFactoryCarSpecification) As Boolean
        If Not (Me.FactoryGenerationID.Equals(carSpecification.FactoryGenerationID)) Then Return False
        If Not (Me.BodyTypeID.Equals(carSpecification.BodyTypeID)) Then Return False
        If Not (Me.EngineID.Equals(carSpecification.EngineID)) Then Return False
        If Not (Me.FactoryGradeID.Equals(carSpecification.FactoryGradeID)) Then Return False
        If Not (Me.TransmissionID.Equals(carSpecification.TransmissionID)) Then Return False
        If Not (Me.WheelDriveID.Equals(carSpecification.WheelDriveID)) Then Return False
        If Not (Me.SteeringID.Equals(carSpecification.SteeringID)) Then Return False
        If Not (Me.ProductionPlantID.Equals(carSpecification.ProductionPlantID)) Then Return False
        If Not (Me.DestinationID.Equals(carSpecification.DestinationID)) Then Return False
        Return True
    End Function
    Private Function MatchesLeftJoin(ByVal carSpecification As PartialFactoryCarSpecification) As Boolean
        If Not (Me.FactoryGenerationID.Equals(Guid.Empty) OrElse Me.FactoryGenerationID.Equals(carSpecification.FactoryGenerationID)) Then Return False
        If Not (Me.BodyTypeID.Equals(Guid.Empty) OrElse Me.BodyTypeID.Equals(carSpecification.BodyTypeID)) Then Return False
        If Not (Me.EngineID.Equals(Guid.Empty) OrElse Me.EngineID.Equals(carSpecification.EngineID)) Then Return False
        If Not (Me.FactoryGradeID.Equals(Guid.Empty) OrElse Me.FactoryGradeID.Equals(carSpecification.FactoryGradeID)) Then Return False
        If Not (Me.TransmissionID.Equals(Guid.Empty) OrElse Me.TransmissionID.Equals(carSpecification.TransmissionID)) Then Return False
        If Not (Me.WheelDriveID.Equals(Guid.Empty) OrElse Me.WheelDriveID.Equals(carSpecification.WheelDriveID)) Then Return False
        If Not (Me.SteeringID.Equals(Guid.Empty) OrElse Me.SteeringID.Equals(carSpecification.SteeringID)) Then Return False
        If Not (Me.ProductionPlantID.Equals(Guid.Empty) OrElse Me.ProductionPlantID.Equals(carSpecification.ProductionPlantID)) Then Return False
        If Not (Me.DestinationID.Equals(Guid.Empty) OrElse Me.DestinationID.Equals(carSpecification.DestinationID)) Then Return False
        Return True
    End Function
    Private Function MatchesRigthJoin(ByVal carSpecification As PartialFactoryCarSpecification) As Boolean
        If Not (carSpecification.FactoryGenerationID.Equals(Guid.Empty) OrElse Me.FactoryGenerationID.Equals(carSpecification.FactoryGenerationID)) Then Return False
        If Not (carSpecification.BodyTypeID.Equals(Guid.Empty) OrElse Me.BodyTypeID.Equals(carSpecification.BodyTypeID)) Then Return False
        If Not (carSpecification.EngineID.Equals(Guid.Empty) OrElse Me.EngineID.Equals(carSpecification.EngineID)) Then Return False
        If Not (carSpecification.FactoryGradeID.Equals(Guid.Empty) OrElse Me.FactoryGradeID.Equals(carSpecification.FactoryGradeID)) Then Return False
        If Not (carSpecification.TransmissionID.Equals(Guid.Empty) OrElse Me.TransmissionID.Equals(carSpecification.TransmissionID)) Then Return False
        If Not (carSpecification.WheelDriveID.Equals(Guid.Empty) OrElse Me.WheelDriveID.Equals(carSpecification.WheelDriveID)) Then Return False
        If Not (carSpecification.SteeringID.Equals(Guid.Empty) OrElse Me.SteeringID.Equals(carSpecification.SteeringID)) Then Return False
        If Not (carSpecification.ProductionPlantID.Equals(Guid.Empty) OrElse Me.ProductionPlantID.Equals(carSpecification.ProductionPlantID)) Then Return False
        If Not (carSpecification.DestinationID.Equals(Guid.Empty) OrElse Me.DestinationID.Equals(carSpecification.DestinationID)) Then Return False
        Return True
    End Function
#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function GetHashCode() As Integer
        ' Return a hash value for the object
        Dim l As Long = 0
        l += Me.FactoryGenerationID.GetHashCode
        l += Me.BodyTypeID.GetHashCode
        l += Me.EngineID.GetHashCode
        l += Me.FactoryGradeID.GetHashCode
        l += Me.TransmissionID.GetHashCode
        l += Me.WheelDriveID.GetHashCode
        l += Me.SteeringID.GetHashCode
        l += Me.ProductionPlantID.GetHashCode
        l += Me.DestinationID.GetHashCode
        Return l.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As PartialFactoryCarSpecification) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.MatchesInnerJoin(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is PartialFactoryCarSpecification Then
            Return Me.Equals(DirectCast(obj, PartialFactoryCarSpecification))
        Else
            Return False
        End If
    End Function
#End Region

#Region " Framework Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return Me.GetHashCode()
    End Function
#End Region

#Region " Shared Factory Methods "

    Public Shared Function NewPartialFactoryCarSpecification() As PartialFactoryCarSpecification
        Return New PartialFactoryCarSpecification
    End Function

    Public Shared Function NewPartialFactoryCarSpecification(ByVal factoryGeneration As FactoryGenerationInfo, ByVal bodyTypeId As Guid, ByVal engineID As Guid, _
                                                             ByVal factoryGradeID As Guid, ByVal transmissionID As Guid, ByVal wheelDriveID As Guid, ByVal steeringID As Guid, _
                                                             ByVal productionPlantID As Guid, ByVal destinationID As Guid) As PartialFactoryCarSpecification
        Dim oPartialFactoryCarSpecitication As New PartialFactoryCarSpecification
        With oPartialFactoryCarSpecitication
            ._factoryGenerationID = factoryGeneration.ID
            ._factoryGeneration = factoryGeneration
            ._bodyTypeID = bodyTypeId
            ._engineID = engineID
            ._factoryGradeID = factoryGradeID
            ._transmissionID = transmissionID
            ._wheelDriveID = wheelDriveID
            ._steeringID = steeringID
            ._productionPlantID = productionPlantID
            ._destinationID = destinationID
        End With
        Return oPartialFactoryCarSpecitication
    End Function


#End Region

#Region " Constructors "
    Private Sub New()
        Me.AllowRemove = True
        Me.AllowEdit = True
        Me.AllowNew = True
        MarkAsChild()
        MarkOld()
    End Sub

#End Region

#Region " Data Access Helper Functions "
    Friend Sub AppendParameters(ByVal command As SqlCommand)
        With command.Parameters
            .AddWithValue("@FACTORYGENERATIONID", Me.FactoryGenerationID)
            .AddWithValue("@FACTORYBODYID", Me.BodyTypeID)
            .AddWithValue("@FACTORYENGINEID", Me.EngineID)
            .AddWithValue("@FACTORYGRADEID", Me.FactoryGradeID)
            .AddWithValue("@TRANSMISSIONID", Me.TransmissionID)
            .AddWithValue("@WHEELDRIVEID", Me.WheelDriveID)
            .AddWithValue("@STEERINGID", Me.SteeringID)
            .AddWithValue("@PRODUCTIONPLANTID", Me.ProductionPlantID)
            .AddWithValue("@DESTINATIONID", Me.DestinationID)
        End With
    End Sub
#End Region

End Class