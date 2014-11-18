Namespace ActiveFilterTool
    <Serializable()> Public Class FilterValueApplicability
        Inherits ContextUniqueGuidBusinessBase(Of FilterValueApplicability)

#Region " Business Properties & Methods "

        Private _key As String = String.Empty
        Private _generation As ModelGenerationInfo
        Private _bodyType As BodyTypeInfo
        Private _engine As EngineInfo
        Private _grade As GradeInfo
        Private _transmission As TransmissionInfo
        Private _wheelDrive As WheelDriveInfo
        Private _steering As SteeringInfo

        Private ReadOnly Property Key() As String
            Get
                If _key.Length = 0 Then _key = GetPartialCarSpecification().ToString()
                Return _key
            End Get
        End Property
        Public Property Generation() As ModelGenerationInfo
            Get
                Return _generation
            End Get
            Set(ByVal value As ModelGenerationInfo)
                If value Is Nothing Then value = ModelGenerationInfo.Empty
                If _generation.Equals(value) Then Return

                _generation = value
                PropertyHasChanged("Generation")
            End Set
        End Property
        Public Property BodyType() As BodyTypeInfo
            Get
                Return _bodyType
            End Get
            Set(ByVal value As BodyTypeInfo)
                If value Is Nothing Then value = BodyTypeInfo.Empty
                If _bodyType.Equals(value) Then Return

                _bodyType = value
                PropertyHasChanged("BodyType")
            End Set
        End Property
        Public Property Engine() As EngineInfo
            Get
                Return _engine
            End Get
            Set(ByVal value As EngineInfo)
                If value Is Nothing Then value = EngineInfo.Empty
                If _engine.Equals(value) Then Return

                _engine = value
                PropertyHasChanged("Engine")
            End Set
        End Property
        Public Property Grade() As GradeInfo
            Get
                Return _grade
            End Get
            Set(ByVal value As GradeInfo)
                If value Is Nothing Then value = GradeInfo.Empty
                If _grade.Equals(value) Then Return

                _grade = value
                PropertyHasChanged("Grade")
            End Set
        End Property
        Public Property Transmission() As TransmissionInfo
            Get
                Return _transmission
            End Get
            Set(ByVal value As TransmissionInfo)
                If value Is Nothing Then value = TransmissionInfo.Empty
                If _transmission.Equals(value) Then Return

                _transmission = value
                PropertyHasChanged("Transmission")
            End Set
        End Property
        Public Property WheelDrive() As WheelDriveInfo
            Get
                Return _wheelDrive
            End Get
            Set(ByVal value As WheelDriveInfo)
                If value Is Nothing Then value = WheelDriveInfo.Empty
                If _wheelDrive.Equals(value) Then Return

                _wheelDrive = value
                PropertyHasChanged("WheelDrive")
            End Set
        End Property
        Public Property Steering() As SteeringInfo
            Get
                Return _steering
            End Get
            Set(ByVal value As SteeringInfo)
                If value Is Nothing Then value = SteeringInfo.Empty
                If _steering.Equals(value) Then Return

                _steering = value
                PropertyHasChanged("Steering")
            End Set
        End Property


        Public ReadOnly Property FilterValue() As FilterValue
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, FilterValueApplicabilities).FilterValue
            End Get
        End Property
        Friend Sub SetSecurityRights()
            Dim ownedByMe = FilterValue.Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase)
            AllowNew = ownedByMe
            AllowEdit = ownedByMe
            AllowRemove = ownedByMe
        End Sub


        Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
            If propertyName.Equals("Grade") AndAlso MyContext.GetContext().IsGlobal Then Return False
            Return MyBase.CanWriteProperty(propertyName)
        End Function
        Protected Overrides Sub PropertyHasChanged(ByVal propertyName As String)
            ResetKey()
            MyBase.PropertyHasChanged(propertyName)
            ValidationRules.CheckRules("Key")
        End Sub
        Private Sub ResetKey()
            _key = String.Empty
        End Sub



#Region " Helper Methods "

        Private Function GetPartialCarSpecification() As PartialCarSpecification
            Dim carSpecification As PartialCarSpecification = PartialCarSpecification.NewPartialCarSpecification
            With carSpecification
                .GenerationID = Generation.ID
                .GenerationName = Generation.Name

                .BodyTypeID = BodyType.ID
                .BodyTypeName = BodyType.Name

                .EngineID = Engine.ID
                .EngineName = Engine.Name

                .GradeID = Grade.ID
                .GradeName = Grade.Name

                .TransmissionID = Transmission.ID
                .TransmissionName = Transmission.Name

                .WheelDriveID = WheelDrive.ID
                .WheelDriveName = WheelDrive.Name

                .SteeringID = Steering.ID
                .SteeringName = Steering.Name

            End With
            Return (carSpecification)
        End Function

#End Region

#End Region


#Region " Business & Validation Rules "

        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf KeyValid, Validation.RuleHandler), "Key")
            ValidationRules.AddRule(DirectCast(AddressOf KeyUnique, Validation.RuleHandler), "Key")
        End Sub

        Private Shared Function KeyUnique(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
            Dim applicability As FilterValueApplicability = DirectCast(target, FilterValueApplicability)
            If applicability.Parent Is Nothing Then Return True

            If DirectCast(applicability.Parent, FilterValueApplicabilities).Any(Function(x) x.Key.Equals(applicability.Key) AndAlso Not x.ID.Equals(applicability.ID)) Then
                e.Description = String.Format("An other asset already exists for the same parameters ({0})", applicability.Key)
                Return False
            End If

            Return True
        End Function
        Private Shared Function KeyValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
            Dim applicability As FilterValueApplicability = DirectCast(target, FilterValueApplicability)
            If applicability.Parent Is Nothing Then Return True

            If applicability.Key = String.Empty Then
                e.Description = String.Format("At least of of the fields needs to be filled in")
                Return False
            End If

            Return True
        End Function

#End Region

#Region " System.Object Overrides "

        Public Overloads Overrides Function ToString() As String
            Return Key
        End Function

#End Region

#Region " Constructors "

        Private Sub New()
            'Prevent direct creation
        End Sub

#End Region

#Region " Data Access "

        Protected Overrides Sub InitializeFields()
            _generation = ModelGenerationInfo.Empty
            _bodyType = BodyTypeInfo.Empty
            _engine = EngineInfo.Empty
            _grade = GradeInfo.Empty
            _transmission = TransmissionInfo.Empty
            _wheelDrive = WheelDriveInfo.Empty
            _steering = SteeringInfo.Empty
        End Sub

        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            _generation = ModelGenerationInfo.GetModelGenerationInfo(dataReader)
            _bodyType = BodyTypeInfo.GetBodyTypeInfo(dataReader)
            _engine = EngineInfo.GetEngineInfo(dataReader)
            _grade = GradeInfo.GetGradeInfo(dataReader)
            _transmission = TransmissionInfo.GetTransmissionInfo(dataReader)
            _wheelDrive = WheelDriveInfo.GetWheelDriveInfo(dataReader)
            _steering = SteeringInfo.GetSteeringInfo(dataReader)
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            AddUpdateCommandFields(command)
        End Sub

        Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@FILTERVALUEID", FilterValue.ID)
            command.Parameters.AddWithValue("@GENERATIONID", Generation.ID.GetDbValue())
            command.Parameters.AddWithValue("@BODYID", BodyType.ID.GetDbValue())
            command.Parameters.AddWithValue("@ENGINEID", Engine.ID.GetDbValue())
            command.Parameters.AddWithValue("@GRADEID", Grade.ID.GetDbValue())
            command.Parameters.AddWithValue("@TRANSMISSIONID", Transmission.ID.GetDbValue())
            command.Parameters.AddWithValue("@WHEELDRIVEID", WheelDrive.ID.GetDbValue())
            command.Parameters.AddWithValue("@STEERINGID", Steering.ID.GetDbValue())
        End Sub

#End Region
    End Class
End Namespace