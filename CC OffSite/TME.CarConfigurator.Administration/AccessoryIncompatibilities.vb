Imports TME.BusinessObjects.Templates
Imports TME.BusinessObjects.Validation
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()>
<CommandClassName("AccessoryIncompatibilities")>
Public NotInheritable Class AccessoryIncompatibilities
    Inherits EquipmentIncompatibilities

#Region " Business Properties & Methods "
    Public Overloads Overrides Function Add() As EquipmentIncompatibility
        Dim incompatibility As AccessoryIncompatibility = AccessoryIncompatibility.NewAccessoryIncompatibility()
        Add(incompatibility)
        incompatibility.CheckRules()
        Return incompatibility
    End Function
    Public Overloads Overrides Function Add(ByVal partialCarSpecification As PartialCarSpecification) As EquipmentIncompatibility
        Dim incompatibility As AccessoryIncompatibility = AccessoryIncompatibility.NewAccessoryIncompatibility()
        With partialCarSpecification
            incompatibility.PartialCarSpecification.ModelID = .ModelID
            incompatibility.PartialCarSpecification.GenerationID = .GenerationID
            incompatibility.PartialCarSpecification.EngineID = .EngineID
            incompatibility.PartialCarSpecification.TransmissionID = .TransmissionID
            incompatibility.PartialCarSpecification.WheelDriveID = .WheelDriveID
            incompatibility.PartialCarSpecification.BodyTypeID = .BodyTypeID
            incompatibility.PartialCarSpecification.FactoryGradeID = .FactoryGradeID
            incompatibility.PartialCarSpecification.SteeringID = .SteeringID
            incompatibility.PartialCarSpecification.GradeID = .GradeID
            incompatibility.PartialCarSpecification.VersionID = .VersionID
        End With
        Add(incompatibility)
        incompatibility.CheckRules()
        Return incompatibility
    End Function
#End Region

#Region " Constructors "
    Friend Sub New()
        MyBase.New()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Function GetObject(ByVal dataReader As Common.Database.SafeDataReader) As EquipmentIncompatibility
        Return AccessoryIncompatibility.GetAccessoryIncompatibility(dataReader)
    End Function
#End Region

End Class

<Serializable()> Public NotInheritable Class AccessoryIncompatibility
    Inherits EquipmentIncompatibility

#Region " Business Properties & Methods "
    Private _reason As String = String.Empty
    Private _factoryGeneration As FactoryGenerationInfo = FactoryGenerationInfo.Empty

    Public Property Reason() As String
        Get
            Return _reason
        End Get
        Set(ByVal value As String)
            If Not _reason.Equals(value) Then
                _reason = value
                PropertyHasChanged("Reason")
            End If
        End Set
    End Property

    Public Property FactoryGeneration() As FactoryGenerationInfo
        Get
            Return _factoryGeneration
        End Get
        Set(ByVal value As FactoryGenerationInfo)
            If value.Equals(_factoryGeneration) Then Return

            _factoryGeneration = value
            PropertyHasChanged("FactoryGeneration")
        End Set
    End Property

    Public Sub CheckRules()
        ValidationRules.CheckRules()
    End Sub

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        MyBase.AddBusinessRules()

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, RuleHandler), New Rules.String.MaxLengthRuleArgs("Reason", 512))

        ValidationRules.AddRule(DirectCast(AddressOf CannotHaveModelID, RuleHandler), "PartialCarSpecification")
        ValidationRules.AddRule(DirectCast(AddressOf CannotHaveGenerationID, RuleHandler), "PartialCarSpecification")
        ValidationRules.AddRule(DirectCast(AddressOf FactoryGenerationIDRequired, RuleHandler), "FactoryGeneration")
    End Sub
    Private Shared Function ParentDoesNotExistOrIsAttachedToALegacyAccessory(ByVal accessoryIncompatibility As AccessoryIncompatibility) As Boolean

        If (accessoryIncompatibility.Parent Is Nothing) Then Return True

        If (accessoryIncompatibility.EquipmentItem IsNot Nothing AndAlso TypeOf accessoryIncompatibility.EquipmentItem Is Accessory AndAlso DirectCast(accessoryIncompatibility.EquipmentItem, Accessory).IsLegacy) Then
            Return True
        End If

        Return False
    End Function

    Private Shared Function CannotHaveModelID(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim incompatibility = DirectCast(target, AccessoryIncompatibility)

        e.Description = "An accessory incompatibility cannot have a related Model."

        Return ParentDoesNotExistOrIsAttachedToALegacyAccessory(incompatibility) OrElse incompatibility.PartialCarSpecification.ModelID.Equals(Guid.Empty)
    End Function

    Private Shared Function CannotHaveGenerationID(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim incompatibility = DirectCast(target, AccessoryIncompatibility)

        e.Description = "An accessory incompatibility cannot have a related Generation."

        Return ParentDoesNotExistOrIsAttachedToALegacyAccessory(incompatibility) OrElse incompatibility.PartialCarSpecification.GenerationID.Equals(Guid.Empty)
    End Function

    Private Shared Function FactoryGenerationIDRequired(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim incompatibility = DirectCast(target, AccessoryIncompatibility)

        e.Description = "An accessory incompatibility is required to have a related Factory Generation."

        Return ParentDoesNotExistOrIsAttachedToALegacyAccessory(incompatibility) OrElse Not incompatibility.FactoryGeneration.IsEmpty()
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overrides Function ToString() As String
        If FactoryGeneration.IsEmpty() Then Return MyBase.ToString()
        Return String.Format("{0}, {1}", FactoryGeneration, MyBase.ToString())
    End Function

#End Region
#Region " Shared Factory Methods "
    Friend Shared Function NewAccessoryIncompatibility() As AccessoryIncompatibility
        Dim incompatibility = New AccessoryIncompatibility
        incompatibility.Create()
        Return incompatibility
    End Function
    Friend Shared Function GetAccessoryIncompatibility(ByVal dataReader As SafeDataReader) As AccessoryIncompatibility
        Dim incompatibility As AccessoryIncompatibility = New AccessoryIncompatibility
        incompatibility.Fetch(dataReader)
        Return incompatibility
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
        _reason = dataReader.GetString("REASON")
        _factoryGeneration = FactoryGenerationInfo.GetFactoryGenerationInfo(dataReader)
    End Sub
    Protected Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@ACCESSORYID", EquipmentItem.ID)
        command.Parameters.AddWithValue("@REASON", Reason)
        command.Parameters.AddWithValue("@FACTORYGENERATIONID", FactoryGeneration.ID)
        PartialCarSpecification.AppendParameters(command)
    End Sub
#End Region

End Class
