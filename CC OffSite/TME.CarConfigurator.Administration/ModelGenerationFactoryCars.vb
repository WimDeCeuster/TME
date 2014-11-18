<Serializable()> Public NotInheritable Class ModelGenerationFactoryCars
    Inherits BaseObjects.ContextUniqueGuidListBase(Of ModelGenerationFactoryCars, ModelGenerationFactoryCar)

#Region " Business Properties & Methods "

    Friend Property FactoryGeneration() As ModelGenerationFactoryGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationFactoryGeneration)
        End Get
        Private Set(ByVal value As ModelGenerationFactoryGeneration)
            Me.SetParent(value)
        End Set
    End Property

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetFactoryCars(ByVal factoryGeneration As ModelGenerationFactoryGeneration) As ModelGenerationFactoryCars
        Dim _cars As ModelGenerationFactoryCars = DataPortal.Fetch(Of ModelGenerationFactoryCars)(New CustomCriteria(factoryGeneration))
        _cars.FactoryGeneration = factoryGeneration
        Return _cars
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
        Me.AllowNew = False
        Me.AllowEdit = False
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _generationID As Guid
        Private ReadOnly _factoryGenerationID As Guid

        Public Sub New(ByVal factoryGeneration As ModelGenerationFactoryGeneration)
            _generationID = factoryGeneration.Generation.ID
            _factoryGenerationID = factoryGeneration.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
            command.Parameters.AddWithValue("@FACTORYGENERATIONID", _factoryGenerationID)
        End Sub
    End Class
#End Region

End Class
<Serializable()> Public NotInheritable Class ModelGenerationFactoryCar
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of ModelGenerationFactoryCar)

#Region " Business Properties & Methods "


    Public ReadOnly Property FactoryGeneration() As ModelGenerationFactoryGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationFactoryCars).FactoryGeneration
        End Get
    End Property

    Private _katashiki As String = String.Empty
    Private _name As String = String.Empty
    Private _owner As String = String.Empty
    Private _activated As Boolean = True
    Private _bodyType As BodyTypeInfo = Nothing
    Private _engine As EngineInfo = Nothing
    Private _grade As FactoryGradeInfo = Nothing
    Private _transmission As TransmissionInfo = Nothing
    Private _wheelDrive As WheelDriveInfo = Nothing
    Private _steering As SteeringInfo = Nothing
    Private _productionPlant As ProductionPlant = Nothing
    Private _destination As Destination = Nothing


    Public ReadOnly Property Katashiki() As String
        Get
            Return _katashiki
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    Public Property Activated() As Boolean
        Get
            Return _activated
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Me.Activated) Then
                _activated = value
                PropertyHasChanged("Activated")
            End If
        End Set
    End Property
    Public ReadOnly Property Owner() As String
        Get
            Return _owner
        End Get
    End Property
    Public ReadOnly Property BodyType() As BodyTypeInfo
        Get
            Return _bodyType
        End Get
    End Property
    Public ReadOnly Property Engine() As EngineInfo
        Get
            Return _engine
        End Get
    End Property
    Public ReadOnly Property Grade() As FactoryGradeInfo
        Get
            Return _grade
        End Get
    End Property
    Public ReadOnly Property Transmission() As TransmissionInfo
        Get
            Return _transmission
        End Get
    End Property
    Public ReadOnly Property WheelDrive() As WheelDriveInfo
        Get
            Return _wheelDrive
        End Get
    End Property
    Public ReadOnly Property Steering() As SteeringInfo
        Get
            Return _steering
        End Get

    End Property
    Public ReadOnly Property ProductionPlant() As ProductionPlant
        Get
            Return _productionPlant
        End Get
    End Property
    Public ReadOnly Property Destination() As Destination
        Get
            Return _destination
        End Get
    End Property


#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function

    Public Overloads Function Equals(ByVal obj As FactoryCar) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        Return (String.Compare(obj, Me.Katashiki, True) = 0)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
        Me.AutoDiscover = False
        Me.AllowNew = False
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        With dataReader
            _name = .GetString("SHORTNAME")
            _katashiki = .GetString("KATASHIKICODE")
            _activated = .GetBoolean("ACTIVATED")
            _owner = .GetString("OWNER")
            _bodyType = BodyTypeInfo.GetBodyTypeInfo(dataReader)
            _engine = EngineInfo.GetEngineInfo(dataReader)
            _grade = FactoryGradeInfo.GetFactoryGradeInfo(dataReader)
            _transmission = TransmissionInfo.GetTransmissionInfo(dataReader)
            _wheelDrive = WheelDriveInfo.GetWheelDriveInfo(dataReader)
            _steering = SteeringInfo.GetSteeringInfo(dataReader)
            _productionPlant = ProductionPlant.GetProductionPlant(dataReader)
            _destination = Destination.GetDestination(dataReader)
        End With
        MyBase.AllowEdit = MyContext.GetContext().CountryCode.Equals(Environment.GlobalCountryCode)
    End Sub

    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@GENERATIONID", Me.FactoryGeneration.Generation.ID)
        command.Parameters.AddWithValue("@FACTORYCARID", Me.ID)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@ACTIVATED", Me.Activated)
    End Sub

#End Region

End Class

