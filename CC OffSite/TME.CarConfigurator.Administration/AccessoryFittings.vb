Imports TME.BusinessObjects.Templates
Imports TME.BusinessObjects.Validation

<Serializable()>
<CommandClassName("AccessoryFittings")>
Public NotInheritable Class AccessoryFittings
    Inherits EquipmentFittings

#Region " Business Properties & Methods "
    Public Overloads Overrides Function Add() As EquipmentFitting
        Dim fitting As AccessoryFitting = AccessoryFitting.NewAccessoryFitting()
        Add(fitting)
        fitting.CheckRules()
        Return fitting
    End Function
    Public Overloads Overrides Function Add(ByVal partialCarSpecification As PartialCarSpecification) As EquipmentFitting
        Dim fitting As AccessoryFitting = AccessoryFitting.NewAccessoryFitting()
        With partialCarSpecification
            fitting.PartialCarSpecification.EngineID = .EngineID
            fitting.PartialCarSpecification.TransmissionID = .TransmissionID
            fitting.PartialCarSpecification.WheelDriveID = .WheelDriveID
            fitting.PartialCarSpecification.BodyTypeID = .BodyTypeID
            fitting.PartialCarSpecification.FactoryGradeID = .FactoryGradeID
            fitting.PartialCarSpecification.SteeringID = .SteeringID
            fitting.PartialCarSpecification.GradeID = .GradeID
            fitting.PartialCarSpecification.VersionID = .VersionID
        End With
        Add(fitting)
        Return fitting
    End Function

#End Region

#Region "Events"

    Public Event FittingFactoryGenerationChanged(ByVal accessoryFitting As AccessoryFitting, ByVal oldValue As FactoryGenerationInfo, ByVal newValue As FactoryGenerationInfo)

    Public Sub FactoryGenerationChanged(ByVal accessoryFitting As AccessoryFitting, ByVal oldValue As FactoryGenerationInfo, ByVal newValue As FactoryGenerationInfo)
        RaiseEvent FittingFactoryGenerationChanged(accessoryFitting, oldValue, newValue)
    End Sub

#End Region

#Region " Constructors "
    Friend Sub New()
        MyBase.New()
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Function GetObject(ByVal dataReader As Common.Database.SafeDataReader) As EquipmentFitting
        Return AccessoryFitting.GetAccessoryFitting(dataReader)
    End Function

#End Region
End Class
<Serializable()> Public NotInheritable Class AccessoryFitting
    Inherits EquipmentFitting

#Region " Business Properties & Methods "
    Private _factoryGeneration As FactoryGenerationInfo = FactoryGenerationInfo.Empty
    Private _fittingTimeNewCar As TimeSpan
    Private _fittingTimeExistingCar As TimeSpan
    Private _fittingInstructions As String = String.Empty



    Public Property FactoryGeneration() As FactoryGenerationInfo
        Get
            Return _factoryGeneration
        End Get
        Set(ByVal value As FactoryGenerationInfo)
            If value.Equals(_factoryGeneration) Then Return

            Dim oldValue = _factoryGeneration
            _factoryGeneration = value
            PropertyHasChanged("FactoryGeneration")
            DirectCast(Parent, AccessoryFittings).FactoryGenerationChanged(Me, oldValue, value)
        End Set
    End Property
    Public Property FittingHoursNewCar() As Integer
        Get
            Return (_fittingTimeNewCar.Hours + (_fittingTimeNewCar.Days * 24))
        End Get
        Set(ByVal value As Integer)
            If FittingHoursNewCar.Equals(value) Then Return

            _fittingTimeNewCar = New TimeSpan(value, _fittingTimeNewCar.Minutes, _fittingTimeNewCar.Seconds)
            PropertyHasChanged("FittingHoursNewCar")
        End Set
    End Property
    Public Property FittingMinutesNewCar() As Integer
        Get
            Return _fittingTimeNewCar.Minutes
        End Get
        Set(ByVal value As Integer)
            If _fittingTimeNewCar.Minutes.Equals(value) Then Return

            _fittingTimeNewCar = New TimeSpan(_fittingTimeNewCar.Hours, value, _fittingTimeNewCar.Seconds)
            PropertyHasChanged("FittingMinutesNewCar")
        End Set
    End Property

    Public Property FittingHoursExistingCar() As Integer
        Get
            Return (_fittingTimeExistingCar.Hours + (_fittingTimeExistingCar.Days * 24))
        End Get
        Set(ByVal value As Integer)
            If FittingHoursExistingCar.Equals(value) Then Return

            _fittingTimeExistingCar = New TimeSpan(value, _fittingTimeExistingCar.Minutes, _fittingTimeExistingCar.Seconds)
            PropertyHasChanged("FittingHoursExistingCar")
        End Set
    End Property
    Public Property FittingMinutesExistingCar() As Integer
        Get
            Return _fittingTimeExistingCar.Minutes
        End Get
        Set(ByVal value As Integer)
            If _fittingTimeExistingCar.Minutes.Equals(value) Then Return

            _fittingTimeExistingCar = New TimeSpan(_fittingTimeExistingCar.Hours, value, _fittingTimeExistingCar.Seconds)
            PropertyHasChanged("FittingHoursExistingCar")
        End Set
    End Property

    Public Property FittingInstructions() As String
        Get
            If (_fittingInstructions Is Nothing) Then Return String.Empty
            Return _fittingInstructions
        End Get
        Set(ByVal value As String)
            If _fittingInstructions.Equals(value) Then Return

            _fittingInstructions = value
            PropertyHasChanged("FittingInstructions")
        End Set
    End Property


#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        MyBase.AddBusinessRules()

        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("FittingInstructions", 512))

        ValidationRules.AddRule(DirectCast(AddressOf CannotHaveModelID, RuleHandler), "PartialCarSpecification")
        ValidationRules.AddRule(DirectCast(AddressOf CannotHaveGenerationID, RuleHandler), "PartialCarSpecification")
        ValidationRules.AddRule(DirectCast(AddressOf FactoryGenerationRequired, RuleHandler), "FactoryGeneration")

    End Sub

    Private Shared Function ParentDoesNotExistOrIsAttachedToALegacyAccessory(ByVal accessoryFitting As AccessoryFitting) As Boolean

        If (accessoryFitting.Parent Is Nothing) Then Return True

        If (accessoryFitting.EquipmentItem IsNot Nothing AndAlso TypeOf accessoryFitting.EquipmentItem Is Accessory AndAlso DirectCast(accessoryFitting.EquipmentItem, Accessory).IsLegacy) Then
            Return True
        End If

        Return False
    End Function

    Private Shared Function CannotHaveModelID(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim fitting = DirectCast(target, AccessoryFitting)

        e.Description = "An accessory fitting cannot have a related Model."

        Return ParentDoesNotExistOrIsAttachedToALegacyAccessory(fitting) OrElse fitting.PartialCarSpecification.ModelID.Equals(Guid.Empty)
    End Function

    Private Shared Function CannotHaveGenerationID(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim fitting = DirectCast(target, AccessoryFitting)

        e.Description = "An accessory fitting cannot have a related Generation."

        Return ParentDoesNotExistOrIsAttachedToALegacyAccessory(fitting) OrElse fitting.PartialCarSpecification.GenerationID.Equals(Guid.Empty)
    End Function

    Private Shared Function FactoryGenerationRequired(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim fitting = DirectCast(target, AccessoryFitting)

        e.Description = "An accessory fitting is required to have a related Factory Generation."

        Return ParentDoesNotExistOrIsAttachedToALegacyAccessory(fitting) OrElse Not fitting.FactoryGeneration.IsEmpty()
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overrides Function ToString() As String
        If FactoryGeneration.IsEmpty() Then Return MyBase.ToString()
        Return String.Format("{0}, {1}", FactoryGeneration, MyBase.ToString())
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewAccessoryFitting() As AccessoryFitting
        Dim fitting = New AccessoryFitting
        fitting.Create()
        Return fitting
    End Function
    Friend Shared Function GetAccessoryFitting(ByVal dataReader As SafeDataReader) As AccessoryFitting
        Dim fitting As AccessoryFitting = New AccessoryFitting
        fitting.Fetch(dataReader)
        Return fitting
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MyBase.New()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        With dataReader
            _fittingTimeNewCar = New TimeSpan(0, Decimal.ToInt32(CType(.GetValue("FITTINGTIMENEW"), Decimal)), 0)
            _fittingTimeExistingCar = New TimeSpan(0, Decimal.ToInt32(CType(.GetValue("FITTINGTIMEEXISTING"), Decimal)), 0)
            _fittingInstructions = .GetString("FITTINGINSTRUCTIONS")
            _factoryGeneration = FactoryGenerationInfo.GetFactoryGenerationInfo(dataReader)
        End With
    End Sub
    Protected Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@ACCESSORYID", EquipmentItem.ID)
        command.Parameters.AddWithValue("@SORTORDER", Index)
        command.Parameters.AddWithValue("@CLEARED", IsCleared)
        command.Parameters.AddWithValue("@FITTINGTIMENEW", _fittingTimeNewCar.TotalMinutes())
        command.Parameters.AddWithValue("@FITTINGTIMEEXISTING", _fittingTimeExistingCar.TotalMinutes())
        command.Parameters.AddWithValue("@FITTINGINSTRUCTIONS", FittingInstructions)
        command.Parameters.AddWithValue("@FACTORYGENERATIONID", FactoryGeneration.ID)
        PartialCarSpecification.AppendParameters(command)
    End Sub
#End Region

End Class
