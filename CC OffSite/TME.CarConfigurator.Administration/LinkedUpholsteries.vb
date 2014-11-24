<Serializable()> Public NotInheritable Class LinkedUpholsteries
    Inherits ContextUniqueGuidListBase(Of LinkedUpholsteries, LinkedUpholstery)

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

    Default Public Overloads Overrides ReadOnly Property Item(ByVal code As String) As LinkedUpholstery
        Get
            Return FirstOrDefault(Function(x) x.Equals(code))
        End Get
    End Property
    Public Overloads Overrides Function Contains(ByVal code As String) As Boolean
        Return Any(Function(x) x.Equals(code))
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetLinkedUpholsteries(ByVal partialCarSpecification As PartialCarSpecification, ByVal dataReader As SafeDataReader) As LinkedUpholsteries
        Dim upholsteries As LinkedUpholsteries = New LinkedUpholsteries()
        upholsteries.PartialCarSpecification = partialCarSpecification
        upholsteries.Fetch(dataReader)
        Return upholsteries
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

End Class
<Serializable(), XmlInfo("upholstery")> Public NotInheritable Class LinkedUpholstery
    Inherits ContextUniqueGuidBusinessBase(Of LinkedUpholstery)
    Implements IPrice
    Implements IOverwritable

#Region " Business Properties & Methods "
    Private _objectId As Guid = Guid.Empty
    Private _price As Decimal = 0D
    Private _vatPrice As Decimal = 0D
    Private _partialCarSpecification As PartialCarSpecification

    <XmlInfo(XmlNodeType.Attribute)>
    Private ReadOnly Property ObjectID() As Guid
        Get
            Return _objectId
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public Property Price() As Decimal Implements IPrice.PriceExcludingVat
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
    <XmlInfo(XmlNodeType.Attribute)> Public Property VatPrice() As Decimal Implements IPrice.PriceIncludingVat
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
    <XmlInfo(XmlNodeType.None)> Public Overloads Overrides Property AllowRemove() As Boolean
        Get
            Return Overwritten()
        End Get
        Protected Set(ByVal value As Boolean)
            MyBase.AllowRemove = value
        End Set
    End Property

    Public Function Overwritten() As Boolean
        Return PartialCarSpecification.Equals(DirectCast(Parent, LinkedUpholsteries).PartialCarSpecification)
    End Function

    Public Function HasBeenOverwritten() As Boolean Implements IOverwritable.HasBeenOverwritten
        Return Overwritten
    End Function
    Public Sub Overwrite() Implements IOverwritable.Overwrite
        If Not Overwritten() Then
            TakeOwnership(DirectCast(Parent, LinkedUpholsteries).PartialCarSpecification)
        End If
    End Sub
    Public Sub Revert() Implements IOverwritable.Revert
        If Overwritten() Then
            DirectCast(Parent, LinkedUpholsteries).Remove(Me)
        End If
    End Sub

    Friend Sub TakeOwnership(ByVal specification As PartialCarSpecification)
        _partialCarSpecification = specification
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

    Public Function GetInfo() As UpholsteryInfo
        Return UpholsteryInfo.GetUpholsteryInfo(Me)
    End Function
#End Region

#Region " Reference Properties & Methods "
    Private _refObject As ModelGenerationUpholstery

    Private ReadOnly Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, LinkedUpholsteries).Generation
        End Get
    End Property

    Public ReadOnly Property GenerationUpholstery() As ModelGenerationUpholstery
        Get
            If _refObject Is Nothing Then _refObject = Generation.ColourCombinations.Upholsteries().First(Function(x) x.ID.Equals(ID))
            Return _refObject
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Code() As String
        Get
            Return GenerationUpholstery.Code
        End Get
    End Property
    Public ReadOnly Property Aliases() As Aliases
        Get
            Return GenerationUpholstery.Aliases
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return GenerationUpholstery.Name
        End Get
    End Property
    <XmlInfo(XmlNodeType.Element)> Public ReadOnly Property Type() As UpholsteryTypeInfo
        Get
            Return GenerationUpholstery.Type
        End Get
    End Property
    <XmlInfo(XmlNodeType.Element)> Public ReadOnly Property Trim() As TrimInfo
        Get
            Return GenerationUpholstery.Trim
        End Get
    End Property
    <XmlInfo(XmlNodeType.Element)> Public ReadOnly Property InteriorColour() As InteriorColourInfo
        Get
            Return GenerationUpholstery.InteriorColour
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Index() As Integer
        Get
            Return GenerationUpholstery.Index
        End Get
    End Property

    Public ReadOnly Property Translation() As Translations.Translation
        Get
            Return GenerationUpholstery.Translation
        End Get
    End Property
    Public ReadOnly Property AlternateName() As String
        Get
            Return GenerationUpholstery.AlternateName
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Code & " - " & Name
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationUpholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As LinkedUpholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Upholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        Return (String.Compare(Code, obj, True, Globalization.CultureInfo.InvariantCulture) = 0) OrElse Aliases.Contains(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Guid) As Boolean
        Return ObjectID.Equals(obj) OrElse ID.Equals(obj)
    End Function

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationUpholstery Then
            Return Equals(DirectCast(obj, ModelGenerationUpholstery))
        ElseIf TypeOf obj Is LinkedUpholstery Then
            Return Equals(DirectCast(obj, LinkedUpholstery))
        ElseIf TypeOf obj Is Upholstery Then
            Return Equals(DirectCast(obj, Upholstery))
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

    Friend Shared Function NewLinkedUpholstery(ByVal partialCarSpecification As PartialCarSpecification, ByVal generationUpholstery As ModelGenerationUpholstery) As LinkedUpholstery
        Dim upholstery As LinkedUpholstery = New LinkedUpholstery()
        upholstery.Create(partialCarSpecification, generationUpholstery)
        Return upholstery
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Private Overloads Sub Create(ByVal specification As PartialCarSpecification, ByVal upholstery As ModelGenerationUpholstery)
        Create(upholstery.ID)
        _objectId = Guid.NewGuid()
        _refObject = upholstery
        _partialCarSpecification = Specification
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
        command.Parameters.AddWithValue("@UPHOLSTERYID", ID)
        command.Parameters.AddWithValue("@PRICE", Price)
        command.Parameters.AddWithValue("@PRICEVAT", VatPrice)
        command.Parameters.AddWithValue("@CURRENCY", MyContext.GetContext().Currency.Code)
        _partialCarSpecification.AppendParameters(command)
    End Sub
#End Region

End Class