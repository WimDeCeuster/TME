<Serializable()> Public NotInheritable Class SuffixOptions
    Inherits BaseObjects.ContextListBase(Of SuffixOptions, SuffixOption)

#Region " Business Properties & Methods "

    Friend Property Suffix() As Suffix
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, Suffix)
        End Get
        Private Set(ByVal value As Suffix)
            Me.SetParent(value)
        End Set
    End Property

    Public Overloads Function Add(ByVal value As FactoryGenerationOptionValue) As SuffixOption
        If Not Me.Suffix.FactoryCar.FactoryGeneration.Equals(value.Option.FactoryGeneration) Then Throw New Exceptions.InvalidFactoryOption(value, Me.Suffix)

        Dim _suffixOption As SuffixOption = SuffixOption.NewSuffixOption(value.GetInfo())
        MyBase.Add(_suffixOption)
        Return _suffixOption
    End Function

    Public Overloads Function Contains(ByVal smsCode As String, ByVal smsValue As String) As Boolean
        Return Me.Any(Function(suffixOption) suffixOption.Equals(smsCode, smsValue))
    End Function


#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetSuffixOptions(ByVal suffix As Suffix) As SuffixOptions
        Dim options As SuffixOptions
        If (suffix.IsNew) Then
            options = New SuffixOptions
        Else
            options = DataPortal.Fetch(Of SuffixOptions)(New CustomCriteria(suffix))
        End If
        options.Suffix = suffix
        Return options
    End Function
    Friend Shared Function GetSuffixOptions(ByVal suffix As Suffix, ByVal dataReader As SafeDataReader) As SuffixOptions
        Dim options As SuffixOptions = New SuffixOptions()
        options.Fetch(dataReader)
        options.Suffix = suffix
        Return options
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

        Private ReadOnly _suffixID As Guid

        Public Sub New(ByVal suffix As Suffix)
            _suffixID = suffix.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@SUFFIXID", _suffixID)
        End Sub
    End Class
#End Region

End Class
<Serializable()> Public NotInheritable Class SuffixOption
    Inherits BaseObjects.ContextBusinessBase(Of SuffixOption)

#Region " Business Properties & Methods "

    Private _optionValue As FactoryGenerationOptionValueInfo

    Public ReadOnly Property Suffix() As Suffix
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, SuffixOptions).Suffix
        End Get
    End Property
    Public Property FactoryOptionValue() As FactoryGenerationOptionValueInfo
        Get
            Return _optionValue
        End Get
        Private Set(ByVal optionValue As FactoryGenerationOptionValueInfo)
            _optionValue = optionValue
        End Set
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.FactoryOptionValue.ToString
    End Function

    Public Overloads Function Equals(ByVal obj As SuffixOption) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Suffix.Equals(obj.Suffix.ID) AndAlso Me.GetIdValue().Equals(obj.GetIdValue())
    End Function

    Public Overloads Function Equals(ByVal smsCode As String, ByVal smsValue As String) As Boolean
        Return FactoryOptionValue.Equals(smsCode, smsValue)
    End Function


#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
        Me.AutoDiscover = False
        Me.AllowEdit = False
    End Sub
#End Region

#Region " Shared Factory Method "

    Friend Shared Function NewSuffixOption(ByVal value As FactoryGenerationOptionValueInfo) As SuffixOption
        Dim _suffixOption As SuffixOption = New SuffixOption
        _suffixOption.Create()
        _suffixOption.FactoryOptionValue = value
        Return _suffixOption
    End Function

#End Region

#Region "Framework Overrides"

    Protected Overrides Function GetIdValue() As Object
        Return Me.FactoryOptionValue.ID
    End Function

#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        _optionValue = FactoryGenerationOptionValueInfo.GetInfo(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@SUFFIXID", Me.Suffix.ID)
        command.Parameters.AddWithValue("@FACTORYGENERATIONOPTIONVALUEID", Me.FactoryOptionValue.ID)
    End Sub

    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@SUFFIXID", Me.Suffix.ID)
        command.Parameters.AddWithValue("@FACTORYGENERATIONOPTIONVALUEID", Me.FactoryOptionValue.ID)
    End Sub

#End Region

End Class