<Serializable()> Public NotInheritable Class SuffixColourCombinations
    Inherits BaseObjects.ContextListBase(Of SuffixColourCombinations, SuffixColourCombination)

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

    Public Overloads Function Add(ByVal exteriorColour As ExteriorColourInfo, ByVal upholstery As UpholsteryInfo) As SuffixColourCombination
        Dim colourCombination As SuffixColourCombination = SuffixColourCombination.NewSuffixColourCombination(exteriorColour, upholstery)
        MyBase.Add(colourCombination)
        Return colourCombination
    End Function

    Friend Function OnlyHasOneExteriorColour() As Boolean
        Return ((From combination In Me Select combination.ExteriorColour.Code).Distinct().Count() = 1)
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetSuffixColourCombinations(ByVal suffix As Suffix) As SuffixColourCombinations
        Dim colourCombinations As SuffixColourCombinations
        If suffix.IsNew Then
            colourCombinations = New SuffixColourCombinations()
        Else
            colourCombinations = DataPortal.Fetch(Of SuffixColourCombinations)(New CustomCriteria(suffix))
        End If
        colourCombinations.Suffix = suffix
        Return colourCombinations
    End Function
    Friend Shared Function GetSuffixColourCombinations(ByVal suffix As Suffix, ByVal dataReader As SafeDataReader) As SuffixColourCombinations
        Dim colourCombinations As SuffixColourCombinations = New SuffixColourCombinations()
        colourCombinations.Fetch(dataReader)
        colourCombinations.Suffix = suffix
        Return colourCombinations
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
<Serializable()> Public NotInheritable Class SuffixColourCombination
    Inherits BaseObjects.ContextBusinessBase(Of SuffixColourCombination)

#Region " Business Properties & Methods "

    Private _exteriorColour As ExteriorColourInfo
    Private _upholstery As UpholsteryInfo

    Public ReadOnly Property Suffix() As Suffix
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, SuffixColourCombinations).Suffix
        End Get
    End Property
    Public Property ExteriorColour() As ExteriorColourInfo
        Get
            Return _exteriorColour
        End Get
        Private Set(ByVal value As ExteriorColourInfo)
            _exteriorColour = value
        End Set
    End Property
    Public Property Upholstery() As UpholsteryInfo
        Get
            Return _upholstery
        End Get
        Private Set(ByVal value As UpholsteryInfo)
            _upholstery = value
        End Set
    End Property
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.ExteriorColour.ToString & "-" & Me.Upholstery.ToString
    End Function

    Public Overloads Function Equals(ByVal obj As SuffixColourCombination) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Suffix.Equals(obj.Suffix.ID) AndAlso Me.GetIdValue().Equals(obj.GetIdValue())
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

    Friend Shared Function NewSuffixColourCombination(ByVal exteriorColour As ExteriorColourInfo, ByVal upholstery As UpholsteryInfo) As SuffixColourCombination
        Dim colourCombination As SuffixColourCombination = New SuffixColourCombination
        colourCombination.Create()
        colourCombination.ExteriorColour = exteriorColour
        colourCombination.Upholstery = upholstery
        Return colourCombination
    End Function

#End Region

#Region "Framework Overrides"

    Protected Overrides Function GetIdValue() As Object
        Return String.Format("{0}-{1}", Me.ExteriorColour.ID, Me.Upholstery.ID)
    End Function

#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        _exteriorColour = ExteriorColourInfo.GetExteriorColourInfo(dataReader)
        _upholstery = UpholsteryInfo.GetUpholsteryInfo(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@SUFFIXID", Me.Suffix.ID)
        command.Parameters.AddWithValue("@EXTERIORCOLOURID", Me.ExteriorColour.ID)
        command.Parameters.AddWithValue("@UPHOLSTERYID", Me.Upholstery.ID)
    End Sub

    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@SUFFIXID", Me.Suffix.ID)
        command.Parameters.AddWithValue("@EXTERIORCOLOURID", Me.ExteriorColour.ID)
        command.Parameters.AddWithValue("@UPHOLSTERYID", Me.Upholstery.ID)
    End Sub

#End Region

End Class