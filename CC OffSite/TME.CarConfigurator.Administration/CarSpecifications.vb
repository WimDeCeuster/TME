Imports System.Collections.Generic

<Serializable()> Public NotInheritable Class CarSpecifications
    Inherits ContextUniqueGuidListBase(Of CarSpecifications, CarSpecification)

#Region " Business Properties & Methods "


    Friend Property Car() As Car
        Get
            Return DirectCast(Parent, Car)
        End Get
        Private Set(ByVal value As Car)
            SetParent(value)
        End Set
    End Property


    Private Sub NormalizeValues()
        For Each carSpecification In Me
            carSpecification.NormalizeValues()
        Next
    End Sub


#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetCarSpecifications(ByVal car As Car) As CarSpecifications
        Dim carSpecifications As CarSpecifications = New CarSpecifications()
        carSpecifications.Car = car
        carSpecifications.Combine(car.Generation.Specifications, DataPortal.Fetch(Of CarSpecifications)(New CustomCriteria(car)))
        carSpecifications.NormalizeValues()
        Return carSpecifications
    End Function


#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _carId As Guid

        Public Sub New(ByVal car As Car)
            CommandText = "getCarSpecifications"
            _carId = car.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@CARID", _carId)
        End Sub
    End Class
#End Region

#Region " Data Access "
    Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
        Get
            Return False
        End Get
    End Property

    Private Sub Combine(ByVal generationSpecifications As IEnumerable(Of ModelGenerationSpecification), ByVal carSpecifications As CarSpecifications)

        AllowNew = True
        For Each generationSpecification As ModelGenerationSpecification In generationSpecifications
            Dim carSpecification = carSpecifications(generationSpecification.ID)
            If carSpecification Is Nothing Then carSpecification = carSpecification.GetCarSpecification(generationSpecification.ID)
            Add(carSpecification)


            carSpecification.GenerationSpecification = generationSpecification
            carSpecification.CompleteWithMissingLangauges()
        Next
        AllowNew = False
    End Sub
    Protected Overrides Sub Fetch(dataReader As SafeDataReader)
        While dataReader.Read()
            Dim id = dataReader.GetGuid("TECHSPECID")
            Dim carSpecification = Me(id)
            If carSpecification Is Nothing Then
                carSpecification = carSpecification.GetCarSpecification(id)
                Add(carSpecification)
            End If

            carSpecification.Values.Add(dataReader)
        End While

    End Sub




#End Region

End Class
<Serializable(), XmlInfo("specification")> Public NotInheritable Class CarSpecification
    Inherits ContextUniqueGuidBusinessBase(Of CarSpecification)
    Implements IOwnedBy

#Region " Business Properties & Methods "
    Private _values As CarSpecificationValues

    Public ReadOnly Property Values() As CarSpecificationValues
        Get
            If _values Is Nothing Then _values = CarSpecificationValues.NewCarSpecificationValues(Me)
            Return _values
        End Get
    End Property

    Friend Sub CompleteWithMissingLangauges()
        For Each generationValue As ModelGenerationSpecificationValue In GenerationSpecification.Values.Where(Function(value) value.Matches(Car))
            If Not Values.Any(Function(value) value.LanguageCode.Equals(generationValue.LanguageCode, StringComparison.InvariantCultureIgnoreCase)) Then
                Values.Add(generationValue)
            End If
        Next
    End Sub


    Public Sub CheckValueRules()
        For Each carValue In Values
            carValue.CheckRules()
        Next
    End Sub
    Friend Sub NormalizeValues()
        For Each carValue In Values
            carValue.NormalizeValues()
        Next
    End Sub

#End Region

#Region " Reference Properties & Methods "
    Private _refObject As ModelGenerationSpecification


    Friend ReadOnly Property Car() As Car
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, CarSpecifications).Car
        End Get
    End Property
    Friend Property GenerationSpecification() As ModelGenerationSpecification
        Get
            If _refObject Is Nothing Then
                If Car Is Nothing Then Return Nothing

                _refObject = Car.Generation.Specifications(ID)
            End If

            Return _refObject
        End Get
        Set(ByVal obj As ModelGenerationSpecification)
            _refObject = obj
        End Set
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Code() As String
        Get
            Return GenerationSpecification.Code
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return GenerationSpecification.Name
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Index() As Integer
        Get
            Return GenerationSpecification.Index
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Approved() As Boolean
        Get
            Return GenerationSpecification.Approved
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Declined() As Boolean
        Get
            Return GenerationSpecification.Declined
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property QuickSpecification() As Boolean
        Get
            Return GenerationSpecification.QuickSpecification
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property FullSpecification() As Boolean
        Get
            Return GenerationSpecification.FullSpecification
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property HasMapping() As Boolean
        Get
            Return GenerationSpecification.HasMapping
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return GenerationSpecification.Owner
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Expression() As String
        Get
            Return GenerationSpecification.Expression
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property TypeCode() As TypeCode
        Get
            Return GenerationSpecification.TypeCode
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property HelpText() As String
        Get
            Return GenerationSpecification.HelpText
        End Get
    End Property

    Public Function IsMasterSpecification() As Boolean
        Return GenerationSpecification.IsMasterSpecification()
    End Function

    Public ReadOnly Property Unit() As UnitInfo
        Get
            Return GenerationSpecification.Unit
        End Get
    End Property
    Public ReadOnly Property Category() As SpecificationCategoryInfo
        Get
            Return GenerationSpecification.Category
        End Get
    End Property

    Public ReadOnly Property Translation() As Translations.Translation
        Get
            Return GenerationSpecification.Translation
        End Get
    End Property
    Public ReadOnly Property AlternateName() As String
        Get
            Return GenerationSpecification.AlternateName
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetCarSpecification(ByVal id As Guid) As CarSpecification
        Dim carSpecification As CarSpecification = New CarSpecification()
        carSpecification.Create(id)
        carSpecification.MarkOld()
        Return carSpecification
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not Values.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Values.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        If Owner = Environment.GlobalCountryCode OrElse MyContext.GetContext.CountryCode.Equals(Owner) Then
            Return Name
        Else
            Return Name & " [" & Owner & "]"
        End If
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As Specification) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationSpecification) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As CarSpecification) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationSpecification Then
            Return Equals(DirectCast(obj, ModelGenerationSpecification))
        ElseIf TypeOf obj Is CarSpecification Then
            Return Equals(DirectCast(obj, CarSpecification))
        ElseIf TypeOf obj Is Specification Then
            Return Equals(DirectCast(obj, Specification))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        MarkAsChild()
        AutoDiscover = False
        AllowNew = False
        AllowRemove = False
        AllowEdit = False
    End Sub
#End Region

#Region " Data Access "




    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Values.IsDirty Then Values.Update(transaction)
    End Sub
#End Region


 
End Class