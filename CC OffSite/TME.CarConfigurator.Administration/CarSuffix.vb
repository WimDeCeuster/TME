<Serializable()> Public NotInheritable Class CarSuffixes
    Inherits ContextListBase(Of CarSuffixes, CarSuffix)

#Region " Business Properties & Methods "
    Default Overloads ReadOnly Property Item(ByVal id As Guid) As CarSuffix
        Get
            Return FirstOrDefault(Function(sfx) sfx.ID.Equals(id))
        End Get
    End Property

    Public Overloads Function Add(ByVal suffix As Suffix) As CarSuffix
        Dim carSuffix As CarSuffix = carSuffix.NewCarSuffix(suffix)
        Add(carSuffix)
        Return carSuffix
    End Function

    Public Overloads Sub Remove(ByVal id As Guid)
        Remove(First(Function(x) x.ID.Equals(id)))
    End Sub

    Friend Sub DefaultSuffixChanged(ByVal changedSuffix As CarSuffix)
        For Each suffix As CarSuffix In (From s In Me Where Not s.ID.Equals(changedSuffix.ID))
            suffix.Default = False
        Next
        Car.DefaultSuffixChanged()
    End Sub

    Public ReadOnly Property Car() As Car
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, Car)
        End Get
    End Property

    Public ReadOnly Property [Default]() As CarSuffix
        Get
            Return (From s In Me Where s.Default).SingleOrDefault()
        End Get
    End Property

    Public Overloads Function Contains(ByVal id As Guid) As Boolean
        Return Any(Function(x) x.ID.Equals(id))
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewCarSuffixes(ByVal car As Car) As CarSuffixes
        Dim list As CarSuffixes = New CarSuffixes()
        list.SetParent(car)
        Return list
    End Function
    Friend Shared Function GetCarSuffixes(ByVal car As Car) As CarSuffixes
        Dim list As CarSuffixes = DataPortal.Fetch(Of CarSuffixes)(New CustomCriteria(car))
        list.SetParent(car)
        Return list
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
        MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _carID As Guid

        Public Sub New(ByVal car As Car)
            _carID = car.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@CARID", _carID)
        End Sub

    End Class
#End Region


End Class
<Serializable()> Public NotInheritable Class CarSuffix
    Inherits ContextBusinessBase(Of CarSuffix)

#Region " Business Properties & Methods "
    Private _suffixInfo As SuffixInfo
    Private _default As Boolean

    Public ReadOnly Property Car() As Car
        Get
            Return DirectCast(Parent, CarSuffixes).Car
        End Get
    End Property
    Public ReadOnly Property FactoryCar() As FactoryCarInfo
        Get
            Return _suffixInfo.FactoryCar
        End Get
    End Property

    Public ReadOnly Property ID() As Guid
        Get
            Return _suffixInfo.ID
        End Get
    End Property
    Public ReadOnly Property Code() As String
        Get
            Return _suffixInfo.Code
        End Get
    End Property
    Public ReadOnly Property Country() As String
        Get
            Return _suffixInfo.Country
        End Get
    End Property
    Public ReadOnly Property FromDate() As Date
        Get
            Return _suffixInfo.FromDate
        End Get
    End Property
    Public ReadOnly Property ToDate() As Date
        Get
            Return _suffixInfo.ToDate
        End Get
    End Property
    Public ReadOnly Property MmoGrade() As String
        Get
            Return _suffixInfo.MmoGrade
        End Get
    End Property
    Public ReadOnly Property Description() As String
        Get
            Return _suffixInfo.Description
        End Get
    End Property
    Public ReadOnly Property Info() As SuffixInfo
        Get
            Return _suffixInfo
        End Get
    End Property
    Public Property [Default]() As Boolean
        Get
            Return _default
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(_default) Then Return

            _default = value
            PropertyHasChanged("Default")

            If _default Then DirectCast(Parent, CarSuffixes).DefaultSuffixChanged(Me)
        End Set
    End Property



    <NotUndoable(), NonSerialized()> Private _suffix As Suffix
    Public Function GetSuffix() As Suffix
        If _suffix Is Nothing Then _suffix = Suffix.GetSuffix(ID)
        Return _suffix
    End Function
    Public Function GetOptionMapping() As OptionMapping
        Return Car.Generation.FactoryGenerations(FactoryCar.FactoryGeneration.ID).OptionMapping
    End Function

#End Region

#Region " Query Methods "

    Public Function HasExteriorColour(ByVal exteriorColourCode As String) As Boolean
        Return GetSuffix().ColourCombinations.Any(Function(colourCombination) colourCombination.ExteriorColour.Code.Equals(exteriorColourCode, StringComparison.InvariantCultureIgnoreCase))
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return String.Format("{0} [{1} - {2}]", Code, FromDate.ToShortDateString(), ToDate.ToShortDateString())
    End Function

    Private ReadOnly _hashCode As Integer = Guid.NewGuid().GetHashCode()
    Public Overloads Overrides Function GetHashCode() As Integer
        Return _hashCode
    End Function

    Public Overloads Function Equals(ByVal obj As Suffix) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function

    Public Overloads Function Equals(ByVal obj As CarSuffix) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function

#End Region

#Region "Framework Overrides"

    Protected Overrides Function GetIdValue() As Object
        Return Car.ID.ToString() & "-" & ID.ToString()
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewCarSuffix(ByVal suffix As Suffix) As CarSuffix
        Dim carSuffix As CarSuffix = New CarSuffix()
        carSuffix._suffixInfo = suffix.GetInfo()
        Return carSuffix
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _suffixInfo = SuffixInfo.GetSuffixInfo(dataReader)
        _default = dataReader.GetBoolean("DEFAULT")
    End Sub

    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command
            .Parameters.AddWithValue("@CARID", Car.ID)
            .Parameters.AddWithValue("@SUFFIXID", ID)
        End With
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub

    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command
            .Parameters.AddWithValue("@CARID", Car.ID)
            .Parameters.AddWithValue("@SUFFIXID", ID)
            .Parameters.AddWithValue("@DEFAULT", [Default])
        End With
    End Sub

#End Region


End Class
