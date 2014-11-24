<Serializable()> Public NotInheritable Class LinkedExteriorColours
    Inherits ContextUniqueGuidListBase(Of LinkedExteriorColours, LinkedExteriorColour)

#Region " Business Properties & Methods "
    <NotUndoable()> Private _generation As ModelGeneration
    <NotUndoable()> Private _partialCarSpecification As PartialCarSpecification

    Friend Property Generation() As ModelGeneration
        Get
            Return _generation
        End Get
        Set(ByVal value As ModelGeneration)
            _generation = value
        End Set
    End Property
    Friend Property PartialCarSpecification() As PartialCarSpecification
        Get
            Return _partialCarSpecification
        End Get
        Private Set(ByVal value As PartialCarSpecification)
            _partialCarSpecification = value
        End Set
    End Property

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetLinkedExteriorColours(ByVal partialCarSpecification As PartialCarSpecification, ByVal dataReader As SafeDataReader) As LinkedExteriorColours
        Dim exteriorColours As LinkedExteriorColours = New LinkedExteriorColours()
        exteriorColours.PartialCarSpecification = partialCarSpecification
        exteriorColours.Fetch(dataReader)
        Return exteriorColours
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

End Class
<Serializable(), XmlInfo("exteriorcolour")> Public NotInheritable Class LinkedExteriorColour
    Inherits ContextUniqueGuidBusinessBase(Of LinkedExteriorColour)
    Implements IPrice
    Implements IOverwritable

#Region " Business Properties & Methods "

    Private _objectId As Guid = Guid.Empty
    Private _price As Decimal = 0D
    Private _vatPrice As Decimal = 0D
    Private _partialCarSpecification As PartialCarSpecification

    Private ReadOnly Property ObjectID() As Guid
        Get
            Return _objectId
        End Get
    End Property
    Public Property Price() As Decimal Implements IPrice.PriceExcludingVat
        Get
            Return _price
        End Get
        Set(ByVal value As Decimal)
            If Not _price.Equals(value) Then
                _price = value
                PropertyHasChanged("Price")
            End If
        End Set
    End Property
    Public Property VatPrice() As Decimal Implements IPrice.PriceIncludingVat
        Get
            Return _vatPrice
        End Get
        Set(ByVal value As Decimal)
            If Not _vatPrice.Equals(value) Then
                _vatPrice = value
                PropertyHasChanged("VatPrice")
            End If
        End Set
    End Property

    <XmlInfo(XmlNodeType.None)> Public Overloads Overrides Property AllowEdit() As Boolean
        Get
            Return Overwritten()
        End Get
        Protected Set(ByVal value As Boolean)
            MyBase.AllowEdit = value
        End Set
    End Property
    Public Overloads Overrides Property AllowRemove() As Boolean
        Get
            Return Overwritten()
        End Get
        Protected Set(ByVal value As Boolean)
            MyBase.AllowRemove = value
        End Set
    End Property

    Public Function Overwritten() As Boolean
        Return PartialCarSpecification.Equals(DirectCast(Parent, LinkedExteriorColours).PartialCarSpecification)
    End Function

    Public Function HasBeenOverwritten() As Boolean Implements IOverwritable.HasBeenOverwritten
        Return Overwritten
    End Function
    Public Sub Overwrite() Implements IOverwritable.Overwrite
        If Not Overwritten() Then
            TakeOwnership(DirectCast(Parent, LinkedExteriorColours).PartialCarSpecification)
        End If
    End Sub
    Public Sub Revert() Implements IOverwritable.Revert
        If Overwritten() Then
            DirectCast(Parent, LinkedExteriorColours).Remove(Me)
        End If
    End Sub

    Friend Sub TakeOwnership(ByVal newCarSpecification As PartialCarSpecification)
        _partialCarSpecification = newCarSpecification
        _objectId = Guid.NewGuid()
        AllowEdit = True
        AllowRemove = True
        MarkNew()
    End Sub
    Private ReadOnly Property PartialCarSpecification() As PartialCarSpecification
        Get
            Return _partialCarSpecification
        End Get
    End Property

    Public Function GetInfo() As ExteriorColourInfo
        Return ExteriorColourInfo.GetExteriorColourInfo(Me)
    End Function
#End Region

#Region " Reference Properties & Methods "
    Private _refObject As ModelGenerationExteriorColour

    Private ReadOnly Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, LinkedExteriorColours).Generation
        End Get
    End Property
    Public ReadOnly Property GenerationExteriorColour() As ModelGenerationExteriorColour
        Get
            If _refObject Is Nothing Then _refObject = Generation.ColourCombinations.ExteriorColours().First(Function(x) x.ID.Equals(ID))
            Return _refObject
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Code() As String
        Get
            Return GenerationExteriorColour.Code
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return GenerationExteriorColour.Name
        End Get
    End Property
    <XmlInfo(XmlNodeType.Element)> Public ReadOnly Property Type() As ExteriorColourTypeInfo
        Get
            Return GenerationExteriorColour.Type
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Index() As Integer
        Get
            Return GenerationExteriorColour.Index
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Promoted() As Boolean
        Get
            Return GenerationExteriorColour.Promoted
        End Get
    End Property


    Public ReadOnly Property Translation() As Translations.Translation
        Get
            Return GenerationExteriorColour.Translation
        End Get
    End Property
    Public ReadOnly Property AlternateName() As String
        Get
            Return GenerationExteriorColour.AlternateName
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Code & " - " & Name
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationExteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As LinkedExteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ExteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ExteriorColourInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        Return (String.Compare(Code, obj, True, Globalization.CultureInfo.InvariantCulture) = 0)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Guid) As Boolean
        Return ObjectID.Equals(obj) OrElse ID.Equals(obj)
    End Function

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationExteriorColour Then
            Return Equals(DirectCast(obj, ModelGenerationExteriorColour))
        ElseIf TypeOf obj Is LinkedExteriorColour Then
            Return Equals(DirectCast(obj, LinkedExteriorColour))
        ElseIf TypeOf obj Is ExteriorColour Then
            Return Equals(DirectCast(obj, ExteriorColour))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        ElseIf TypeOf obj Is String Then
            Return Equals(DirectCast(obj, String))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewLinkedExteriorColour(ByVal partialCarSpecification As PartialCarSpecification, ByVal generationExteriorColour As ModelGenerationExteriorColour) As LinkedExteriorColour
        Dim exteriorColour As LinkedExteriorColour = New LinkedExteriorColour()
        exteriorColour.Create(partialCarSpecification, generationExteriorColour)
        exteriorColour.MarkAsChild()
        Return exteriorColour
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Private Overloads Sub Create(ByVal specification As PartialCarSpecification, ByVal exteriorColour As ModelGenerationExteriorColour)
        Create(exteriorColour.ID)
        _objectId = Guid.NewGuid()
        _refObject = exteriorColour
        _partialCarSpecification = specification
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _objectId = .GetGuid("OBJECTID")
            _price = Environment.ConvertPrice(CType(.GetValue("PRICE"), Decimal), .GetString("CURRENCY"))
            _vatPrice = Environment.ConvertPrice(CType(.GetValue("PRICEVAT"), Decimal), .GetString("CURRENCY"))
        End With
        _partialCarSpecification = PartialCarSpecification.GetPartialCarSpecification(dataReader)
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Private Sub AddCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@ID", ObjectID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@EXTERIORCOLOURID", ID)
        command.Parameters.AddWithValue("@PRICE", Price)
        command.Parameters.AddWithValue("@PRICEVAT", VatPrice)
        command.Parameters.AddWithValue("@CURRENCY", MyContext.GetContext().Currency.Code)
        _partialCarSpecification.AppendParameters(command)
    End Sub

#End Region

End Class