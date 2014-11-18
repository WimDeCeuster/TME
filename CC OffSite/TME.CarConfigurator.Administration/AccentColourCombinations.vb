Imports System.Collections.Generic
Imports TME.BusinessObjects.Templates

<Serializable(), CommandClassName("PackAccentColourCombinations")> Public NotInheritable Class AccentColourCombinations
    Inherits ContextUniqueGuidListBase(Of AccentColourCombinations, AccentColourCombination)

#Region " Business Properties & Methods "
    Friend Property Pack() As ModelGenerationPack
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationPack)
        End Get
        Private Set(ByVal value As ModelGenerationPack)
            SetParent(value)
        End Set
    End Property
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetAccentColourCombinations(ByVal modelGenerationPack As ModelGenerationPack) As AccentColourCombinations
        Dim accentColourCombinations As AccentColourCombinations
        If modelGenerationPack.IsNew Then
            accentColourCombinations = New AccentColourCombinations()
        Else
            accentColourCombinations = DataPortal.Fetch(Of AccentColourCombinations)(New ParentCriteria(modelGenerationPack.ID, "@PACKID"))
        End If
        accentColourCombinations.Pack = modelGenerationPack
        Return accentColourCombinations
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Synchronize Colours"
    Friend Sub Synchronize()
        Dim bodyColours = Pack.BodyColours().ToList()
        Dim primaryAccentColours = Pack.PrimaryAccentColours().ToList()
        Dim secondaryAccentColours = Pack.SecondaryAccentColours().ToList()

        Dim combinations = GetAllPossibleCombinations(bodyColours, primaryAccentColours, secondaryAccentColours)
        RemoveCombinationsThatNoLongerExist(combinations)
        AddMissingCombinations(combinations)
        MakeSureThatThereIsAtLeastOneDefaultPerBodyColour()

    End Sub

    Private Sub RemoveCombinationsThatNoLongerExist(ByVal combinations As IEnumerable(Of AccentColourCombination))
        Dim toBeRemoved = Where(Function(x) Not combinations.Any(Function(y) x.SameValuesAs(y))).ToList()
        For Each combination In toBeRemoved
            Remove(combination)
        Next
    End Sub
    Private Sub AddMissingCombinations(ByVal combinations As IEnumerable(Of AccentColourCombination))
        Dim toBeAdded = combinations.Where(Function(x) Not Items.Any(Function(y) y.SameValuesAs(x))).ToList()
        For Each combination In toBeAdded
            Dim newCombination = Add()
            newCombination.BodyColour = combination.BodyColour
            newCombination.PrimaryAccentColour = combination.PrimaryAccentColour
            newCombination.SecondaryAccentColour = combination.SecondaryAccentColour
            newCombination.IsAvailable = ShouldBeAvailableByDefault(newCombination)
            newCombination.CheckRules()
        Next
    End Sub

    Private Function ShouldBeAvailableByDefault(ByVal accentColourCombination As AccentColourCombination) As Boolean
        If accentColourCombination.BodyColour.Equals(accentColourCombination.PrimaryAccentColour) Then Return False
        If accentColourCombination.PrimaryAccentColour.IsEmpty() AndAlso accentColourCombination.BodyColour.Equals(accentColourCombination.SecondaryAccentColour) Then Return False
        If accentColourCombination.PrimaryAccentColour.Equals(accentColourCombination.SecondaryAccentColour) Then Return False
        If Not accentColourCombination.PrimaryAccentColour.IsEmpty() AndAlso accentColourCombination.SecondaryAccentColour.IsEmpty() AndAlso SecondaryAccentColoursAvailable(accentColourCombination.BodyColour, accentColourCombination.PrimaryAccentColour) Then Return False
        Return True
    End Function

    Private Function SecondaryAccentColoursAvailable(ByVal bodyColour As ExteriorColourInfo, ByVal primaryAccentColour As ExteriorColourInfo) As Boolean
        Return Items.Any(Function(x) x.BodyColour.Equals(bodyColour) AndAlso x.PrimaryAccentColour.Equals(primaryAccentColour) AndAlso Not x.SecondaryAccentColour.IsEmpty())
    End Function


    Private Sub MakeSureThatThereIsAtLeastOneDefaultPerBodyColour()
        Dim bodyColours = Items.Where(Function(x) x.IsAvailable).GroupBy(Function(x) x.BodyColour)
        Dim bodyColoursWithoutDefault = bodyColours.Where(Function(bodyColour) Not bodyColour.Any(Function(x) x.IsDefault))

        For Each group In bodyColoursWithoutDefault
            group.OrderBy(Function(x) x.ToString()).First().IsDefault = True
        Next
    End Sub

    Private Shared Function GetAllPossibleCombinations(ByVal bodyColours As IEnumerable(Of ExteriorColourInfo), ByVal primaryAccentColours As List(Of ExteriorColourInfo), ByVal secondaryAccentColours As List(Of ExteriorColourInfo)) As IList(Of AccentColourCombination)
        If Not (primaryAccentColours.Any() OrElse secondaryAccentColours.Any()) Then Return New List(Of AccentColourCombination)()

        If Not secondaryAccentColours.Any() Then Return GetAllBodyAndPrimaryColourCombinations(bodyColours, primaryAccentColours)
        If Not primaryAccentColours.Any() Then Return GetAllBodyAndSecondaryColourCombinations(bodyColours, secondaryAccentColours)
        Return GetAllBodyPrimaryAndSecondaryColourCombinations(bodyColours, primaryAccentColours, secondaryAccentColours)
    End Function

    Private Shared Function GetAllBodyPrimaryAndSecondaryColourCombinations(ByVal bodyColours As IEnumerable(Of ExteriorColourInfo), ByVal primaryAccentColours As List(Of ExteriorColourInfo), ByVal secondaryAccentColours As List(Of ExteriorColourInfo)) As IList(Of AccentColourCombination)
        Dim primaryColoursWithEmptySecondaryAccentColours = GetAllBodyAndPrimaryColourCombinations(bodyColours, primaryAccentColours)
        Dim fullCombinations = (
            From bodyColour In bodyColours
                From primaryAccentColour In primaryAccentColours
                From secondaryAccentColour In secondaryAccentColours
                Select New AccentColourCombination() With
                    {
                    .BodyColour = bodyColour,
                    .PrimaryAccentColour = primaryAccentColour,
                    .SecondaryAccentColour = secondaryAccentColour
                    }
            )

        Dim response = primaryColoursWithEmptySecondaryAccentColours.ToList()
        response.AddRange(fullCombinations)
        Return response
    End Function

    Private Shared Function GetAllBodyAndSecondaryColourCombinations(ByVal bodyColours As IEnumerable(Of ExteriorColourInfo), ByVal secondaryAccentColours As List(Of ExteriorColourInfo)) As IList(Of AccentColourCombination)

        Return (
            From bodyColour In bodyColours
                From secondaryAccentColour In secondaryAccentColours
                Select New AccentColourCombination() With
                    {
                    .BodyColour = bodyColour,
                    .SecondaryAccentColour = secondaryAccentColour
                    }
            ).ToList()
    End Function
    Private Shared Function GetAllBodyAndPrimaryColourCombinations(ByVal bodyColours As IEnumerable(Of ExteriorColourInfo), primaryAccentColours As IEnumerable(Of ExteriorColourInfo)) As IList(Of AccentColourCombination)
        Return (
            From bodyColour In bodyColours
                From primaryAccentColour In primaryAccentColours
                Select New AccentColourCombination() With
                    {
                    .BodyColour = bodyColour,
                    .PrimaryAccentColour = primaryAccentColour
                    }
            ).ToList()
    End Function


#End Region

End Class

<Serializable(), CommandClassName("PackAccentColourCombination")> Public NotInheritable Class AccentColourCombination
    Inherits ContextUniqueGuidBusinessBase(Of AccentColourCombination)

#Region "Business Properties & Methods"
    Private _bodyColour As ExteriorColourInfo
    Private _primaryAccentColour As ExteriorColourInfo
    Private _secondaryAccentColour As ExteriorColourInfo

    Private _default As Boolean
    Private _available As Boolean

    Property BodyColour() As ExteriorColourInfo
        Get
            Return _bodyColour
        End Get
        Set(value As ExteriorColourInfo)
            If value Is Nothing Then value = ExteriorColourInfo.Empty
            If _bodyColour.Equals(value) Then Return

            _bodyColour = value
            PropertyHasChanged("BodyColour")
        End Set
    End Property

    Property PrimaryAccentColour() As ExteriorColourInfo
        Get
            Return _primaryAccentColour
        End Get
        Set(value As ExteriorColourInfo)
            If value Is Nothing Then value = ExteriorColourInfo.Empty
            If _primaryAccentColour.Equals(value) Then Return

            _primaryAccentColour = value
            PropertyHasChanged("PrimaryAccentColour")
        End Set
    End Property

    Property SecondaryAccentColour() As ExteriorColourInfo
        Get
            Return _secondaryAccentColour
        End Get
        Set(value As ExteriorColourInfo)
            If value Is Nothing Then value = ExteriorColourInfo.Empty
            If _secondaryAccentColour.Equals(value) Then Return

            _secondaryAccentColour = value
            PropertyHasChanged("SecondaryAccentColour")
        End Set
    End Property

    Private ReadOnly Property Pack() As ModelGenerationPack
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, AccentColourCombinations).Pack
        End Get
    End Property

    Public Property IsDefault() As Boolean
        Get
            Return _default
        End Get
        Set(ByVal value As Boolean)
            If _default = value Then Return

            _default = value
            PropertyHasChanged("IsDefault")
        End Set
    End Property

    Public Property IsAvailable() As Boolean
        Get
            Return _available
        End Get
        Set(ByVal value As Boolean)
            If _available = value Then Return

            _available = value
            PropertyHasChanged("IsAvailable")
            ValidationRules.CheckRules("IsDefault")
        End Set
    End Property

    Protected Overrides Sub OnPropertyChanged(ByVal propertyName As String)
        MyBase.OnPropertyChanged(propertyName)
        ValidationRules.CheckRules("ColourCombination")
    End Sub

    Public Function SameValuesAs(ByVal obj As AccentColourCombination) As Boolean
        If Not obj.BodyColour.Equals(BodyColour) Then Return False
        If Not obj.PrimaryAccentColour.Equals(PrimaryAccentColour) Then Return False
        If Not obj.SecondaryAccentColour.Equals(SecondaryAccentColour) Then Return False
        Return True
    End Function

#End Region

#Region "Business & Validation Rules"

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Object.Required, Validation.RuleHandler), "BodyColour")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Object.Required, Validation.RuleHandler), "PrimaryAccentColour")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Object.Required, Validation.RuleHandler), "SecondaryAccentColour")

        ValidationRules.AddRule(DirectCast(AddressOf BodyColourIsNotEmpty, Validation.RuleHandler), "BodyColour")
        ValidationRules.AddRule(DirectCast(AddressOf ColourCombinationIsUnique, Validation.RuleHandler), "ColourCombination")
        ValidationRules.AddRule(DirectCast(AddressOf ColourCombinationValid, Validation.RuleHandler), "ColourCombination")

        ValidationRules.AddRule(DirectCast(AddressOf IsDefaultValid, Validation.RuleHandler), "IsDefault")
    End Sub

    Private Shared Function IsDefaultValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim accentColourCombination As AccentColourCombination = DirectCast(target, AccentColourCombination)
        If Not accentColourCombination.IsDefault Then Return True
        If accentColourCombination.IsAvailable Then Return True

        e.Description = "The combination can not be a default if it's not available"
        Return False
    End Function
    Private Shared Function BodyColourIsNotEmpty(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim accentColourCombination As AccentColourCombination = DirectCast(target, AccentColourCombination)
        If accentColourCombination.BodyColour Is Nothing OrElse accentColourCombination.BodyColour.IsEmpty() Then
            e.Description = "The body colour can not be empty"
            Return False
        End If
        Return True
    End Function
    Private Shared Function ColourCombinationIsUnique(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim accentColourCombination As AccentColourCombination = DirectCast(target, AccentColourCombination)
        If accentColourCombination.Pack Is Nothing Then Return True

        e.Description = String.Format("The colour combinations for pack {0} have to be unique", accentColourCombination.Pack.Name)

        Return Not accentColourCombination.Pack.AccentColourCombinations.Any(Function(x) Not x.ID.Equals(accentColourCombination.ID) AndAlso x.SameValuesAs(accentColourCombination))
    End Function
    Private Shared Function ColourCombinationValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim accentColourCombination As AccentColourCombination = DirectCast(target, AccentColourCombination)
        Dim primaryColourIsEmpty = (accentColourCombination.PrimaryAccentColour Is Nothing OrElse accentColourCombination.PrimaryAccentColour.IsEmpty())
        Dim secondaryColourIsEmpty = (accentColourCombination.SecondaryAccentColour Is Nothing OrElse accentColourCombination.SecondaryAccentColour.IsEmpty())

        If primaryColourIsEmpty AndAlso secondaryColourIsEmpty Then
            e.Description = "You need to have either a primary or a secondary accent colour"
            Return False
        End If
        Return True
    End Function
    Friend Sub CheckRules()
        ValidationRules.CheckRules()
    End Sub

#End Region

#Region " System.Object Overrides "
    Public Overrides Function ToString() As String
        Return String.Format("{0} / {1} / {2}", BodyColour, PrimaryAccentColour, SecondaryAccentColour)
    End Function
#End Region

#Region "Constructor"
    Friend Sub New()
        Create()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _bodyColour = ExteriorColourInfo.Empty
        _primaryAccentColour = ExteriorColourInfo.Empty
        _secondaryAccentColour = ExteriorColourInfo.Empty
        _default = False
        _available = True
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _bodyColour = ExteriorColourInfo.GetExteriorColourInfo(dataReader, "BODY")
            _primaryAccentColour = ExteriorColourInfo.GetExteriorColourInfo(dataReader, "PRIMARYACCENT")
            _secondaryAccentColour = ExteriorColourInfo.GetExteriorColourInfo(dataReader, "SECONDARYACCENT")
            _default = .GetBoolean("ISDEFAULT")
            _available = .GetBoolean("ISAVAILABLE")
        End With
        MyBase.FetchFields(dataReader)
        AllowNew = True
        AllowEdit = True
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@PACKID", Pack.ID)
        command.Parameters.AddWithValue("@BODYCOLOURID", BodyColour.ID)
        command.Parameters.AddWithValue("@PRIMARYACCENTCOLOURID", PrimaryAccentColour.ID.GetDbValue())
        command.Parameters.AddWithValue("@SECONDARYACCENTCOLOURID", SecondaryAccentColour.ID.GetDbValue())
        command.Parameters.AddWithValue("@ISDEFAULT", IsDefault())
        command.Parameters.AddWithValue("@ISAVAILABLE", IsAvailable())
    End Sub
#End Region


End Class
