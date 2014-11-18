Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()>
Public Class LocalConfigurations
    Inherits ContextUniqueGuidListBase(Of LocalConfigurations, LocalConfiguration)

#Region " Business Properties & Methods "

    Friend Property Car() As Car
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, Car)
        End Get
        Private Set(ByVal value As Car)
            SetParent(value)
        End Set
    End Property

    Public ReadOnly Property Base As LocalConfiguration
        Get
            Return SingleOrDefault(Function(localConfiguration) localConfiguration.Base)
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetLocalConfigurations(ByVal car As Car) As LocalConfigurations
        Dim configurations As LocalConfigurations
        If car.IsNew Then
            configurations = New LocalConfigurations
        Else
            configurations = DataPortal.Fetch(Of LocalConfigurations)(New ParentCriteria(car.ID, "@CARID"))
        End If
        configurations.Car = car
        Return configurations
    End Function


#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
        With MyContext.GetContext()
            AllowNew = Not .IsRegionCountry OrElse .IsMainRegionCountry
            AllowEdit = AllowNew
            AllowRemove = AllowNew
        End With
    End Sub
#End Region


End Class
<Serializable()> Public NotInheritable Class LocalConfiguration
    Inherits ContextUniqueGuidBusinessBase(Of LocalConfiguration)

#Region " Business Properties & Methods "

    Private _version As Integer
    Private _code As String = String.Empty
    Private _description As String = String.Empty
    Private _productionFrom As DateTime = DateTime.MinValue
    Private _productionTo As DateTime = DateTime.MinValue
    Private _base As Boolean

    Public ReadOnly Property Car() As Car
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, LocalConfigurations).Car
        End Get
    End Property
    Public ReadOnly Property NmscCode() As String
        Get
            Return MyContext.GetContext().Country.NmscCode
        End Get
    End Property

    Public Property Version() As Integer
        Get
            Return _version
        End Get
        Set(ByVal value As Integer)
            If _version = value Then Return

            _version = value
            PropertyHasChanged("Version")
        End Set
    End Property
    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            If _code.Equals(value) Then Return

            _code = value
            PropertyHasChanged("Code")
        End Set
    End Property
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            If _description.Equals(value) Then Return

            _description = value
            PropertyHasChanged("Description")
        End Set
    End Property
    Public Property ProductionFrom() As DateTime
        Get
            Return _productionFrom
        End Get
        Set(ByVal value As DateTime)
            If _productionFrom.Equals(value) Then Return

            _productionFrom = value
            PropertyHasChanged("ProductionFrom")
            ValidationRules.CheckRules("ProductionRange")
        End Set
    End Property
    Public Property ProductionTo() As DateTime
        Get
            Return _productionTo
        End Get
        Set(ByVal value As DateTime)
            If _productionTo.Equals(value) Then Return

            _productionTo = value
            PropertyHasChanged("ProductionTo")
            ValidationRules.CheckRules("ProductionRange")
        End Set
    End Property
    Public Property Base() As Boolean
        Get
            Return _base
        End Get
        Set(ByVal value As Boolean)
            If _base.Equals(value) Then Return

            _base = value
            PropertyHasChanged("Base")
            Car.BaseLocalConfigurationChanged()
        End Set
    End Property

#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 40))
        ValidationRules.AddRule(DirectCast(AddressOf AssignmentUnique, Validation.RuleHandler), "Code") 'Add rule here as this property is mandatory anyway

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Description")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Description", 255))



        ValidationRules.AddRule(DirectCast(AddressOf ProductionFromValid, Validation.RuleHandler), "ProductionFrom")
        ValidationRules.AddRule(DirectCast(AddressOf ProductionToValid, Validation.RuleHandler), "ProductionTo")
        ValidationRules.AddRule(DirectCast(AddressOf ProductionRangeValid, Validation.RuleHandler), "ProductionRange")
    End Sub

    Private Shared Function AssignmentUnique(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim configuration = DirectCast(target, LocalConfiguration)
        Dim configurationCar = configuration.Car

        If configurationCar Is Nothing Then
            e.Description = "Can not validate at this point, revalidate later in the process"
            Return False
        End If

        For Each generationCar As Car In configurationCar.Generation.Cars
            If generationCar.Equals(configurationCar) Then Continue For
            If Not generationCar.LocalConfigurations.Contains(configuration.ID) Then Continue For

            e.Description = String.Format("The local configuration {0} has already been assigned to the car {1}", configuration, generationCar)
            Return False
        Next

        Return True
    End Function
    Private Shared Function ProductionFromValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim configuration = DirectCast(target, LocalConfiguration)
        If configuration.ProductionFrom = DateTime.MinValue Then
            e.Description = String.Format("The local configuration {0} does not have a valid production from date", configuration)
            Return False
        End If
        configuration.ValidationRules.CheckRules("ProductionRange")
        Return True
    End Function
    Private Shared Function ProductionToValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim configuration = DirectCast(target, LocalConfiguration)
        If configuration.ProductionTo = DateTime.MinValue Then
            e.Description = String.Format("The local configuration {0} does not have a valid production to date", configuration)
            Return False
        End If
        configuration.ValidationRules.CheckRules("ProductionRange")
        Return True
    End Function
    Private Shared Function ProductionRangeValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim configuration = DirectCast(target, LocalConfiguration)
        If configuration.ProductionFrom > configuration.ProductionTo Then
            e.Description = String.Format("The local configuration {0} production range (from {1} to {2}) is not valid.", configuration, configuration.ProductionFrom.ToShortDateString(), configuration.ProductionTo.ToShortDateString())
            Return False
        End If
        Return True
    End Function


#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return String.Format("{0} - {1}", Code, Description)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
        With MyContext.GetContext()
            AllowNew = Not .IsRegionCountry OrElse .IsMainRegionCountry
            AllowEdit = AllowNew
            AllowRemove = AllowNew
        End With
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        _version = dataReader.GetInt32("VERSION")
        _code = dataReader.GetString("CODE")
        _description = dataReader.GetString("DESCRIPTION")
        _productionFrom = dataReader.GetDateTime("PRODUCTIONFROM")
        _productionTo = dataReader.GetDateTime("PRODUCTIONTO")
        _base = dataReader.GetBoolean("BASE")
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@CARID", Car.ID)
        command.Parameters.AddWithValue("@VERSION", Version)
        command.Parameters.AddWithValue("@CODE", Code)
        command.Parameters.AddWithValue("@DESCRIPTION", Description)
        command.Parameters.AddWithValue("@PRODUCTIONFROM", ProductionFrom)
        command.Parameters.AddWithValue("@PRODUCTIONTO", ProductionTo)
        command.Parameters.AddWithValue("@BASE", Base)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddInsertCommandFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@CARID", Car.ID)
    End Sub

#End Region

End Class