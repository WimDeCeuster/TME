Imports TME.BusinessObjects.Templates.SqlServer
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class FactoryCars
    Inherits BaseObjects.ContextUniqueGuidListBase(Of FactoryCars, FactoryCar)

#Region " Business Properties & Methods "

    Friend Property FactoryGeneration() As FactoryGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, FactoryGeneration)
        End Get
        Private Set(ByVal value As FactoryGeneration)
            Me.SetParent(value)
        End Set
    End Property

    Default Public Overloads ReadOnly Property Item(ByVal partialCarSpecification As PartialFactoryCarSpecification) As FactoryCar
        Get
            Return (From car In Me Where car.PartialFactoryCarSpecification().Matches(partialCarSpecification)).FirstOrDefault()
        End Get
    End Property
    Public Overloads Function Contains(ByVal partialCarSpecification As PartialFactoryCarSpecification) As Boolean
        Return (From car In Me Where car.PartialFactoryCarSpecification().Matches(partialCarSpecification)).Any()
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewFactoryCars(ByVal factoryGeneration As FactoryGeneration) As FactoryCars
        Dim _cars As FactoryCars = New FactoryCars
        _cars.FactoryGeneration = factoryGeneration
        Return _cars
    End Function
    Friend Shared Function GetFactoryCars(ByVal factoryGeneration As FactoryGeneration) As FactoryCars
        Dim _cars As FactoryCars = DataPortal.Fetch(Of FactoryCars)(New CustomCriteria(factoryGeneration))
        _cars.FactoryGeneration = factoryGeneration
        Return _cars
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _generationID As Guid

        Public Sub New(ByVal factoryGeneration As FactoryGeneration)
            _generationID = factoryGeneration.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@FACTORYGENERATIONID", _generationID)
        End Sub
    End Class
#End Region

End Class
<Serializable()> Public NotInheritable Class FactoryCar
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of FactoryCar)

#Region " Business Properties & Methods "

    Private _katashiki As String = String.Empty
    Private _name As String = String.Empty
    Private _owner As String = String.Empty
    Private _bodyType As BodyTypeInfo = Nothing
    Private _engine As EngineInfo = Nothing
    Private _grade As FactoryGradeInfo = Nothing
    Private _transmission As TransmissionInfo = Nothing
    Private _wheelDrive As WheelDriveInfo = Nothing
    Private _steering As SteeringInfo = Nothing
    Private _productionPlant As ProductionPlant = Nothing
    Private _destination As Destination = Nothing
    Private _suffixes As Suffixes = Nothing

    Public ReadOnly Property FactoryGeneration() As FactoryGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, FactoryCars).FactoryGeneration
        End Get
    End Property
    Public Property Katashiki() As String
        Get
            Return _katashiki
        End Get
        Set(ByVal value As String)
            value = value.ToUpperInvariant()
            If _katashiki.Equals(value, StringComparison.InvariantCultureIgnoreCase) Then Return

            _katashiki = value
            PropertyHasChanged("Katashiki")
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            If _name <> value Then
                _name = value
                PropertyHasChanged("Name")
            End If
        End Set
    End Property
    Public Property Owner() As String
        Get
            Return _owner
        End Get
        Set(ByVal value As String)
            If _owner <> value Then
                _owner = value
                PropertyHasChanged("Owner")
            End If
        End Set
    End Property
    Public Property BodyType() As BodyTypeInfo
        Get
            Return _bodyType
        End Get
        Set(ByVal value As BodyTypeInfo)
            If value Is Nothing Then Exit Property
            If _bodyType Is Nothing OrElse Not _bodyType.Equals(value) Then
                If Not Me.IsNew Then Throw New Exceptions.CarCanNotBeChanged
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
            If value Is Nothing Then Exit Property
            If _engine Is Nothing OrElse Not _engine.Equals(value) Then
                If Not Me.IsNew Then Throw New Exceptions.CarCanNotBeChanged
                _engine = value
                MarkDirty()
                PropertyHasChanged("Engine")
            End If
        End Set
    End Property
    Public Property Grade() As FactoryGradeInfo
        Get
            Return _grade
        End Get
        Set(ByVal value As FactoryGradeInfo)
            If value Is Nothing Then Exit Property
            If _grade Is Nothing OrElse Not _grade.Equals(value) Then
                If Not Me.IsNew Then Throw New Exceptions.CarCanNotBeChanged
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
            If value Is Nothing Then Exit Property
            If _transmission Is Nothing OrElse Not _transmission.Equals(value) Then
                If Not Me.IsNew Then Throw New Exceptions.CarCanNotBeChanged
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
            If value Is Nothing Then Exit Property
            If _wheelDrive Is Nothing OrElse Not _wheelDrive.Equals(value) Then
                If Not Me.IsNew Then Throw New Exceptions.CarCanNotBeChanged
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
            If value Is Nothing Then Exit Property
            If _steering Is Nothing OrElse Not _steering.Equals(value) Then
                If Not Me.IsNew Then Throw New Exceptions.CarCanNotBeChanged
                _steering = value
                PropertyHasChanged("Steering")
            End If
        End Set
    End Property
    Public Property ProductionPlant() As ProductionPlant
        Get
            Return _productionPlant
        End Get
        Set(ByVal value As ProductionPlant)
            If value Is Nothing Then Exit Property
            If _productionPlant Is Nothing OrElse Not _productionPlant.Equals(value) Then
                If Not Me.IsNew Then Throw New Exceptions.CarCanNotBeChanged
                _productionPlant = value
                PropertyHasChanged("ProductionPlant")
            End If
        End Set
    End Property
    Public Property Destination() As Destination
        Get
            Return _destination
        End Get
        Set(ByVal value As Destination)
            If value Is Nothing Then Exit Property
            If _destination Is Nothing OrElse Not _destination.Equals(value) Then
                If Not Me.IsNew Then Throw New Exceptions.CarCanNotBeChanged
                _destination = value
                PropertyHasChanged("Destination")
            End If
        End Set
    End Property
    Public ReadOnly Property Suffixes() As Suffixes
        Get
            If _suffixes Is Nothing Then
                _suffixes = Suffixes.GetSuffixes(Me)
            End If
            Return _suffixes
        End Get
    End Property

    Public Function GenerateName() As String
        Dim oStringBuilder As System.Text.StringBuilder = New System.Text.StringBuilder
        If Me.FactoryGeneration.Description.Length > 0 Then oStringBuilder.Append(Me.FactoryGeneration.Description & " - ")
        If Me.Grade.Name.Length > 0 Then oStringBuilder.Append(Me.Grade.Name & " - ")
        If Me.BodyType.Name.Length > 0 Then oStringBuilder.Append(Me.BodyType.Name & "  ")
        If Me.Engine.Name.Length > 0 Then oStringBuilder.Append(Me.Engine.Name & " ")
        If Me.Transmission.Name.Length > 0 Then oStringBuilder.Append(Me.Transmission.Name & " ")
        If Me.Steering.Name.Length > 0 Then oStringBuilder.Append(Me.Steering.Name & " - ")
        If Me.ProductionPlant.Name.Length > 0 Then oStringBuilder.Append(Me.ProductionPlant.Name & ", ")
        If Me.Destination.Name.Length > 0 Then oStringBuilder.Append(Me.Destination.Name & ", ")

        Dim sBuffer As String = oStringBuilder.ToString()
        If sBuffer.Length > 0 Then
            sBuffer = sBuffer.Remove(sBuffer.Length - 2, 2)
        End If
        Return sBuffer
    End Function

    Friend Function HasSameComponents(ByVal car As FactoryCar) As Boolean
        Return FactoryGeneration.ID.Equals(car.FactoryGeneration.ID) AndAlso _
         BodyType.ID.Equals(car.BodyType.ID) AndAlso _
         Engine.ID.Equals(car.Engine.ID) AndAlso _
         Grade.ID.Equals(car.Grade.ID) AndAlso _
         Transmission.ID.Equals(car.Transmission.ID) AndAlso _
         WheelDrive.ID.Equals(car.WheelDrive.ID) AndAlso _
         Steering.ID.Equals(car.Steering.ID) AndAlso _
         ProductionPlant.ID.Equals(car.ProductionPlant.ID) AndAlso _
         Destination.ID.Equals(car.Destination.ID) AndAlso _
         Suffixes.Equals(car.Suffixes)
    End Function

    Friend Function PartialFactoryCarSpecification() As PartialFactoryCarSpecification
        Dim oFactoryCarSpecification As PartialFactoryCarSpecification = Administration.PartialFactoryCarSpecification.NewPartialFactoryCarSpecification( _
            Me.FactoryGeneration.GetInfo(), Me.BodyType.ID, Me.Engine.ID, Me.Grade.ID, _
            Me.Transmission.ID, Me.WheelDrive.ID, Me.Steering.ID, Me.ProductionPlant.ID, Me.Destination.ID)
        Return (oFactoryCarSpecification)
    End Function

    Public Function GetInfo() As FactoryCarInfo
        Return FactoryCarInfo.GetFactoryCarInfo(Me)
    End Function

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Katashiki")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Owner")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "BodyType")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "Engine")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "Grade")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "Transmission")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "WheelDrive")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "Steering")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "ProductionPlant")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "Destination")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Katashiki", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Owner", 2))


        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Katashiki")
    End Sub

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

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_suffixes Is Nothing) AndAlso Not _suffixes.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If (Not _suffixes Is Nothing) AndAlso _suffixes.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
        Me.AutoDiscover = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction, ByVal changesToUpdate As ChangesToUpdate)
        MyBase.UpdateChildren(transaction)
        If Not _suffixes Is Nothing Then _suffixes.Update(transaction)
    End Sub

    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _owner = MyContext.GetContext().CountryCode
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        With dataReader
            _name = .GetString("SHORTNAME")
            _owner = .GetString("OWNER")
            _katashiki = .GetString("KATASHIKICODE").ToUpperInvariant()
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
        MyBase.AllowRemove = MyBase.AllowEdit
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub

    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub

    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@SHORTNAME", Me.Name)
        command.Parameters.AddWithValue("@KATASHIKICODE", Me.Katashiki)
        command.Parameters.AddWithValue("@OWNER", Me.Owner)

        'The rest of the parameters:
        Me.PartialFactoryCarSpecification.AppendParameters(command)
    End Sub

#End Region

End Class
<Serializable(), XmlInfo("factorycar")> Public NotInheritable Class FactoryCarInfo

#Region " Business Properties & Methods "
    Private _id As Guid
    Private _katashiki As String
    Private _factoryGenerationInfo As FactoryGenerationInfo

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Katashiki() As String
        Get
            Return _katashiki
        End Get
    End Property
    Public ReadOnly Property FactoryGeneration() As FactoryGenerationInfo
        Get
            Return _factoryGenerationInfo
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Katashiki
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return Me.ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As FactoryCar) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As FactoryCarInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return Me.Katashiki.Equals(obj, StringComparison.InvariantCultureIgnoreCase)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is FactoryCarInfo Then
            Return Me.Equals(DirectCast(obj, FactoryCarInfo))
        ElseIf TypeOf obj Is FactoryCar Then
            Return Me.Equals(DirectCast(obj, FactoryCar))
        ElseIf TypeOf obj Is String Then
            Return Me.Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is FactoryCarInfo Then
            Return DirectCast(objA, FactoryCarInfo).Equals(objB)
        ElseIf TypeOf objB Is FactoryCarInfo Then
            Return DirectCast(objB, FactoryCarInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetFactoryCarInfo(ByVal factoryCar As FactoryCar) As FactoryCarInfo
        Dim _info As FactoryCarInfo = New FactoryCarInfo
        _info.Fetch(factoryCar)
        Return _info
    End Function
    Friend Shared Function GetFactoryCarInfo(ByVal dataReader As SafeDataReader) As FactoryCarInfo
        Dim _info As FactoryCarInfo = New FactoryCarInfo
        _info.Fetch(dataReader)
        Return _info
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Private Sub Fetch(ByVal factoryCar As FactoryCar)
        With factoryCar
            _id = .ID
            _katashiki = .Katashiki
        End With
        _factoryGenerationInfo = factoryCar.FactoryGeneration.GetInfo()
    End Sub

    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _id = .GetGuid("FACTORYCARID")
            _katashiki = .GetString("FACTORYCARKATASHIKI")
        End With
        _factoryGenerationInfo = FactoryGenerationInfo.GetFactoryGenerationInfo(dataReader)
    End Sub


#End Region
End Class