Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class CarPacks
    Inherits ContextUniqueGuidListBase(Of CarPacks, CarPack)

#Region " Business Properties & Methods "

    Public Property Car() As Car
        Get
            Return DirectCast(Parent, Car)
        End Get
        Private Set(ByVal value As Car)
            SetParent(value)
            AddHandler value.Generation.Grades(value.Grade.ID).Packs.PackAdded, AddressOf OnPackAdded
            AddHandler value.Generation.Grades(value.Grade.ID).Packs.PackRemoved, AddressOf OnPackRemoved
        End Set
    End Property
    Private Sub OnPackAdded(ByVal gradePack As ModelGenerationGradePack)
        If Contains(gradePack.ID) Then Throw New ApplicationException("The item already exists in this collection")

        Dim carPack As CarPack = carPack.NewCarPack(gradePack)

        AllowNew = True
        Add(carPack)
        AllowNew = False
    End Sub
    Private Sub OnPackRemoved(ByVal gradePack As ModelGenerationGradePack)
        Dim carPack As CarPack = Me(gradePack.ID)

        AllowRemove = True
        carPack.Remove()
        AllowRemove = False
    End Sub


#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetPacks(ByVal car As Car) As CarPacks
        Dim carPacks As CarPacks = New CarPacks
        Dim gradePacks As ModelGenerationGradePacks = car.Generation.Grades(car.GradeID).Packs
        If car.IsNew Then
            carPacks.Combine(gradePacks, Nothing)
        Else
            carPacks.Combine(gradePacks, DataPortal.Fetch(Of CarPacks)(New ParentCriteria(car.ID, "@CARID")))
        End If
        carPacks.Car = car
        Return carPacks
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowNew = False
        AllowRemove = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
        Get
            Return False
        End Get
    End Property
    Private Sub OnBeforeUpdateCommand(ByVal obj As System.Data.SqlClient.SqlTransaction) Handles Me.BeforeUpdateCommand
        'Clear the list of deleted objects. 
        'These database will take care of this for us via deleteGenerationEquipmentItem
        DeletedList.Clear()
    End Sub


    Private Sub Combine(ByVal gradePacks As IEnumerable(Of ModelGenerationGradePack), ByVal carPacks As CarPacks)
        AllowNew = True
        For Each gradePack As ModelGenerationGradePack In gradePacks
            If carPacks IsNot Nothing AndAlso carPacks.Contains(gradePack.ID) Then
                Dim carPack As CarPack = carPacks(gradePack.ID)
                carPack.GradePack = gradePack
                Add(carPack)
            Else
                Add(CarPack.NewCarPack(gradePack))
            End If
        Next
        AllowNew = False
    End Sub


    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        AllowNew = True 'make it possible to add the ghosts

        FetchCarExteriorColourApplicabilities(dataReader)

        dataReader.NextResult()
        FetchCarUpholsteryApplicabilities(dataReader)

        dataReader.NextResult()
        FetchCarRules(dataReader) 'carpack equipment item rules

        dataReader.NextResult()
        FetchCarRules(dataReader) 'carpack pack rules

        AllowNew = False
    End Sub

    Private Sub FetchCarExteriorColourApplicabilities(ByVal dataReader As SafeDataReader)
        While dataReader.Read
            Dim packId As Guid = dataReader.GetGuid("PACKID")
            Dim carPack = Me(packId)

            If carPack IsNot Nothing Then
                carPack.ExteriorColourApplicabilities.Add(dataReader)
            Else
                AddGhost(packId).ExteriorColourApplicabilities.Add(dataReader)
            End If
        End While
    End Sub
    Private Sub FetchCarUpholsteryApplicabilities(ByVal dataReader As SafeDataReader)
        While dataReader.Read
            Dim packId As Guid = dataReader.GetGuid("PACKID")
            Dim carPack = Me(packId)

            If carPack IsNot Nothing Then
                carPack.UpholsteryApplicabilities.Add(dataReader)
            Else
                AddGhost(packId).UpholsteryApplicabilities.Add(dataReader)
            End If
        End While
    End Sub
    Private Sub FetchCarRules(ByVal dataReader As SafeDataReader)
        While dataReader.Read
            Dim packId As Guid = dataReader.GetGuid("PACKID")
            Dim carPack = Me(packId)

            If carPack IsNot Nothing Then
                carPack.Rules.Add(dataReader)
            Else
                AddGhost(packId).Rules.Add(dataReader)
            End If
        End While
    End Sub

    Private Function AddGhost(ByVal id As Guid) As CarPack
        Dim ghost As CarPack = CarPack.GetGhost(id)
        Add(ghost)
        Return ghost
    End Function
#End Region

End Class
<Serializable()> Public NotInheritable Class CarPack
    Inherits ContextUniqueGuidBusinessBase(Of CarPack)
    Implements IPrice
    Implements IOverwritable
    Implements IMasterListObjectReference

#Region " Business Properties & Methods "
    Private _overwritten As Boolean = False
    Private _availability As Availability
    Private _price As Decimal = 0D
    Private _vatPrice As Decimal = 0D

    Private _items As CarPackItems
    Private _exteriorColourApplicabilities As CarExteriorColourApplicabilities
    Private _upholsteryApplicabilities As CarUpholsteryApplicabilities
    Private _rules As CarPackRules
    Private _ghost As Boolean = False

    <XmlInfo(XmlNodeType.Attribute)> Public Property Overwritten() As Boolean
        Get
            Return _overwritten
        End Get
        Private Set(ByVal value As Boolean)
            If Not value.Equals(_overwritten) Then
                _overwritten = value
                AllowEdit = value
                PropertyHasChanged("Overwritten")
            End If
        End Set
    End Property
    Public Function HasBeenOverwritten() As Boolean Implements IOverwritable.HasBeenOverwritten
        Return Overwritten
    End Function
    Public Sub Overwrite() Implements IOverwritable.Overwrite
        Overwritten = True
    End Sub
    Public Sub Revert() Implements IOverwritable.Revert
        If Not Overwritten Then Return
        SetRefProperties()
        Overwritten = False
        Return
    End Sub

    <XmlInfo(XmlNodeType.Attribute)> Public Property Availability() As Availability
        Get
            Return _availability
        End Get
        Set(ByVal value As Availability)
            If value.Equals(Availability) Then Return

            _availability = value

            Equipment.RecalculateAvailabilities()

            PropertyHasChanged("Availability")
        End Set
    End Property
    Public Property Price() As Decimal Implements IPrice.PriceExcludingVat
        Get
            If Availability = Availability.Optional Then Return _price
            Return 0D
        End Get
        Set(ByVal value As Decimal)
            If _price.Equals(value) Then Return

            _price = value
            PropertyHasChanged("Price")
        End Set
    End Property
    Public Property VatPrice() As Decimal Implements IPrice.PriceIncludingVat
        Get
            If Availability = Availability.Optional Then Return _vatPrice
            Return 0D
        End Get
        Set(ByVal value As Decimal)
            If _vatPrice.Equals(value) Then Return
            _vatPrice = value
            PropertyHasChanged("VatPrice")
        End Set
    End Property

    Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If propertyName.Equals("Overwritten") Then Return True
        Return MyBase.CanWriteProperty(propertyName)
    End Function

    Friend Sub Remove()
        AllowRemove = True
        DirectCast(Parent, CarPacks).Remove(Me)
        AllowRemove = False
    End Sub

    Private Sub SetRefProperties()
        _ghost = False 'it is not a ghost anymore once it has a reference object
        _availability = GradePack.Availability
        _price = GradePack.Price
        _vatPrice = GradePack.VatPrice
    End Sub

    Public ReadOnly Property ExteriorColourApplicabilities As CarExteriorColourApplicabilities
        Get
            If _exteriorColourApplicabilities IsNot Nothing Then Return _exteriorColourApplicabilities

            _exteriorColourApplicabilities = CarExteriorColourApplicabilities.NewApplicabilities(Me)
            Return _exteriorColourApplicabilities
        End Get
    End Property
    Public ReadOnly Property UpholsteryApplicabilities As CarUpholsteryApplicabilities
        Get
            If _upholsteryApplicabilities IsNot Nothing Then Return _upholsteryApplicabilities

            _upholsteryApplicabilities = CarUpholsteryApplicabilities.NewApplicabilities(Me)
            Return _upholsteryApplicabilities
        End Get
    End Property

    Public ReadOnly Property Rules() As CarPackRules
        Get
            If _rules IsNot Nothing Then Return _rules

            _rules = CarPackRules.NewRules(Me)
            Return _rules
        End Get
    End Property
    Public ReadOnly Property Equipment() As CarPackItems
        Get
            If _items IsNot Nothing Then Return _items

            _items = CarPackItems.GetPackItems(Me)
            Return _items
        End Get
    End Property

    Public Sub RecalculateAvailability()
        Dim suffixOptions = OfferedSuffixOptions().ToList()

        If Not suffixOptions.Any() Then Return 'no need to recalculate, there are no suffix options => pack availability stays the same as before

        If suffixOptions.All(Function(o) o.Availability.Equals(Availability.Optional)) Then
            If Availability = Availability.Standard Then
                Availability = Availability.Standard
            Else
                Availability = Availability.Optional
            End If
            Return
        End If

        If Availability = Availability.NotAvailable Then Availability = Availability.Optional 'pack can not be unavailable when there are suffix options, otherwise the suffix options cannot be selected

        If Not suffixOptions.Any(Function(o) o.SuffixAvailability = Availability.Standard) Then Return 'pack availability can stay what it was before

        Availability = Availability.Standard
    End Sub

    Public Function OfferedSuffixOptions() As IEnumerable(Of CarOption)
        Return Equipment.
            OfType(Of CarPackOption)().
            Select(Function(o) DirectCast(o.Pack.Car.Equipment(o.ID), CarOption)).
            Where(Function(o) o.SuffixOption)
    End Function

#End Region

#Region " Reference Properties & Methods "
    Private _grade As ModelGenerationGrade
    Private _gradePack As ModelGenerationGradePack

    Public ReadOnly Property Car() As Car
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, CarPacks).Car
        End Get
    End Property
    Private ReadOnly Property Grade() As ModelGenerationGrade
        Get
            If _grade Is Nothing Then
                If Car Is Nothing Then Return Nothing
                _grade = Car.Generation.Grades(Car.GradeID)
            End If
            Return _grade
        End Get
    End Property

    Friend Property GradePack() As ModelGenerationGradePack
        Get
            If _gradePack Is Nothing Then
                If Car Is Nothing Then Return Nothing
                _gradePack = Grade.Packs(ID)
            End If
            Return _gradePack
        End Get
        Set(ByVal value As ModelGenerationGradePack)
            _gradePack = value
            Rules.InheritedRules = value.GenerationPack.Rules
            If _ghost Then SetRefProperties()

            ExteriorColourApplicabilities.InheritedExteriorColourApplicabilities = value.GenerationPack.ExteriorColourApplicabilities
            UpholsteryApplicabilities.InheritedUpholsteryApplicabilities = value.GenerationPack.UpholsteryApplicabilities

            AddHandler value.PropertyChanged, AddressOf ParentPropertyChanged
        End Set
    End Property
    Private Sub ParentPropertyChanged(ByVal sender As Object, ByVal e As ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If e.PropertyName.Length = 0 Then Exit Sub 'In case the property name was not known, then I'm not intersested
        If Not Overwritten Then
            SetRefProperties()
        End If
    End Sub

    Public ReadOnly Property MasterIDs() As List(Of Guid) Implements IMasterListObjectReference.MasterIDs
        Get
            Return GradePack.MasterIDs
        End Get
    End Property
    Public ReadOnly Property MasterPacks() As ModelGenerationMasterPacks
        Get
            Return GradePack.MasterPacks
        End Get
    End Property
    Public ReadOnly Property MasterType() As MasterPackType
        Get
            Return GradePack.MasterType
        End Get
    End Property

    Public ReadOnly Property ShortID() As Nullable(Of Integer)
        Get
            Return GradePack.ShortID
        End Get
    End Property
    Public ReadOnly Property LocalCode() As String
        Get
            Return GradePack.LocalCode
        End Get
    End Property
    Public ReadOnly Property Code() As String Implements IMasterListObjectReference.MasterCode
        Get
            Return GradePack.Code
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return GradePack.Name
        End Get
    End Property

    Public ReadOnly Property Index() As Integer
        Get
            Return GradePack.Index
        End Get
    End Property

    Public ReadOnly Property Translation() As Translations.Translation
        Get
            Return GradePack.Translation
        End Get
    End Property
    Public ReadOnly Property AlternateName() As String
        Get
            Return GradePack.AlternateName
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewCarPack(ByVal gradePack As ModelGenerationGradePack) As CarPack
        Dim pack As CarPack = New CarPack
        pack.Create(gradePack)
        Return pack
    End Function

    Friend Shared Function GetGhost(ByVal id As Guid) As CarPack
        Dim carPack As CarPack = New CarPack
        carPack.Create(id)
        carPack._ghost = True
        carPack.MarkOld()
        Return carPack
    End Function
#End Region

#Region " Constructors "

    Private Sub New()
        MarkAsChild()
        AllowNew = False
        AllowRemove = False
        AllowEdit = False
    End Sub

#End Region

#Region " System.Object Overrides "
    Public Overloads Function Equals(ByVal obj As ModelGenerationPack) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationGradePack) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As CarPack) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return Not (obj Is Nothing) AndAlso GradePack.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationPack Then
            Return Equals(DirectCast(obj, ModelGenerationPack))
        ElseIf TypeOf obj Is ModelGenerationGradePack Then
            Return Equals(DirectCast(obj, ModelGenerationGradePack))
        ElseIf TypeOf obj Is CarPack Then
            Return Equals(DirectCast(obj, CarPack))
        ElseIf TypeOf obj Is String Then
            Return Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
#End Region

#Region "Framework Overrides"
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_exteriorColourApplicabilities Is Nothing) AndAlso Not _exteriorColourApplicabilities.IsValid Then Return False
            If Not (_upholsteryApplicabilities Is Nothing) AndAlso Not _upholsteryApplicabilities.IsValid Then Return False
            If Not (_rules Is Nothing) AndAlso Not _rules.IsValid Then Return False
            If Not (_items Is Nothing) AndAlso Not _items.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_exteriorColourApplicabilities Is Nothing) AndAlso _exteriorColourApplicabilities.IsDirty Then Return True
            If Not (_upholsteryApplicabilities Is Nothing) AndAlso _upholsteryApplicabilities.IsDirty Then Return True
            If Not (_rules Is Nothing) AndAlso _rules.IsDirty Then Return True
            If Not (_items Is Nothing) AndAlso _items.IsDirty Then Return True
            Return False
        End Get
    End Property



#End Region

#Region " Data Access "
    Private Overloads Sub Create(ByVal newGradePack As ModelGenerationGradePack)
        Create(newGradePack.ID)
        GradePack = newGradePack
        SetRefProperties()
        MarkOld()
    End Sub
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("PACKID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        _availability = CType(dataReader.GetValue("AVAILABILITY"), Availability)
        _price = Environment.ConvertPrice(CType(dataReader.GetValue("PRICE"), Decimal), dataReader.GetString("CURRENCY"))
        _vatPrice = Environment.ConvertPrice(CType(dataReader.GetValue("PRICEVAT"), Decimal), dataReader.GetString("CURRENCY"))
        _overwritten = True
        AllowEdit = True
    End Sub

    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Private Sub AddCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@CARID", Car.ID)
        command.Parameters.AddWithValue("@PACKID", ID)
    End Sub

    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        If Not Overwritten Then
            command.CommandText = "deleteCarPack"
            Exit Sub
        End If

        command.CommandText = "updateCarPack"
        command.Parameters.AddWithValue("@AVAILABILITY", Availability)
        command.Parameters.AddWithValue("@CURRENCY", MyContext.GetContext().Currency.Code)
        command.Parameters.AddWithValue("@PRICE", Price)
        command.Parameters.AddWithValue("@PRICEVAT", VatPrice)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As SqlTransaction)
        If Not _rules Is Nothing Then _rules.Update(transaction)
        If Not _exteriorColourApplicabilities Is Nothing Then _exteriorColourApplicabilities.Update(transaction)
        If Not _upholsteryApplicabilities Is Nothing Then _upholsteryApplicabilities.Update(transaction)
        If Not _items Is Nothing Then _items.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region


End Class
