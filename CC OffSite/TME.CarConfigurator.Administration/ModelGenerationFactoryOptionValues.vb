Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
Imports TME.CarConfigurator.Administration.Components.Interfaces

<Serializable()>
Public Class ModelGenerationFactoryOptionValues
    Inherits ContextUniqueGuidListBase(Of ModelGenerationFactoryOptionValues, ModelGenerationFactoryOptionValue)

#Region "Business Properties & Methods"

    Friend Property FactoryOption() As ModelGenerationFactoryOption
        Get
            Return DirectCast(Parent, ModelGenerationFactoryOption)
        End Get
        Set(ByVal value As ModelGenerationFactoryOption)
            SetParent(value)
        End Set
    End Property

    Friend Shadows Function Add(ByVal dataReader As SafeDataReader) As ModelGenerationFactoryOptionValue
        Dim value = ModelGenerationFactoryOptionValue.GetModelGenerationFactoryOptionValue(dataReader)
        MyBase.Add(value)
        Return value
    End Function
#End Region

#Region "Shared Factory Methods"
    Friend Shared Function NewModelGenerationFactoryOptionValues() As ModelGenerationFactoryOptionValues
        Return New ModelGenerationFactoryOptionValues()
    End Function
#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
    End Sub
#End Region

End Class

<Serializable()>
Public Class ModelGenerationFactoryOptionValue
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationFactoryOptionValue)
    Implements IUpdatableAssetSet
    Implements ICanBeModelGenerationOptionComponent

#Region "Business Properties & Methods"

    Private _factoryMasterSpecValue As FactoryMasterSpecValueInfo
    Private _description As String
    Private _smsValue As String
    Private _specValue As String
    Private _assetSet As AssetSet

    Public ReadOnly Property FactoryMasterSpecValue As FactoryMasterSpecValueInfo
        Get
            Return _factoryMasterSpecValue
        End Get
    End Property

    Public ReadOnly Property Description As String
        Get
            If _description.Length = 0 Then Return FactoryMasterSpecValue.Description
            Return _description
        End Get
    End Property
    Public ReadOnly Property SmsValue As String
        Get
            Return _smsValue
        End Get
    End Property
    Public ReadOnly Property SpecValue As String
        Get
            Return _specValue
        End Get
    End Property

    Public ReadOnly Property AssetSet() As AssetSet Implements IHasAssetSet.AssetSet
        Get
            If _assetSet Is Nothing Then _assetSet = AssetSet.NewAssetSet(Me)
            Return _assetSet
        End Get
    End Property

    Public Sub ChangeReference(ByVal updatedAssetSet As AssetSet) Implements IUpdatableAssetSet.ChangeReference
        _assetSet = updatedAssetSet
    End Sub

    Public ReadOnly Property Entity() As Entity Implements IHasAssetSet.Entity
        Get
            Return Entity.FACTORYGENERATIONOPTIONVALUE
        End Get
    End Property
    Public ReadOnly Property GenerationID() As Guid Implements IHasAssetSet.GenerationID
        Get
            Return Generation.ID
        End Get
    End Property

    Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return "ZZ"
        End Get
    End Property

    Private ReadOnly Property Generation() As ModelGeneration
        Get
            Return [Option].FactoryGeneration.Generation
        End Get
    End Property

    Public ReadOnly Property [Option]() As ModelGenerationFactoryOption
        Get
            Return DirectCast(Parent, ModelGenerationFactoryOptionValues).FactoryOption
        End Get
    End Property

    Public Function IsMarketingIrrelevant() As Boolean
        Return [Option].FactoryGeneration.OptionMapping.Where(Function(mappingLine) mappingLine.FactoryOptionValue.Equals(Me)).All(Function(mappingLine) mappingLine.MarketingIrrelevant)
    End Function

#Region "ICanBeModelGenerationOptionComponent Impelementation"

    Public ReadOnly Property FullDescription() As String Implements ICanBeModelGenerationOptionComponent.Description
        Get
            Return FullDescription([Option].FactoryGeneration.Generation.FactoryGenerations.Count > 1)
        End Get
    End Property
    Public ReadOnly Property FullDescription(ByVal prefixWithSSN As Boolean) As String
        Get
            Dim pattern As String = "{1}{2} : {3} - {4}"
            If prefixWithSSN Then pattern = "{0} - " & pattern

            Return String.Format(pattern,
                    [Option].FactoryGeneration.SSN,
                    [Option].SmsCode,
                    SmsValue,
                    [Option].Description,
                    Description)
        End Get
    End Property

    Public ReadOnly Property ComponentType() As String Implements ICanBeModelGenerationOptionComponent.Type
        Get
            Return "Option"
        End Get
    End Property
#End Region

#End Region

#Region "Shared Factory Methods"

    Friend Shared Function GetModelGenerationFactoryOptionValue(ByVal dataReader As SafeDataReader) As ModelGenerationFactoryOptionValue
        Dim value = New ModelGenerationFactoryOptionValue()
        value.Fetch(dataReader)
        Return value
    End Function

#End Region

#Region "Framework Overrides"
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If _assetSet IsNot Nothing AndAlso Not _assetSet.IsValid Then Return False
            Return MyBase.IsValid
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If _assetSet IsNot Nothing AndAlso _assetSet.IsDirty Then Return True
            Return MyBase.IsDirty
        End Get
    End Property
#End Region

#Region "Object Overrides "
    Public Overrides Function ToString() As String
        Return FullDescription
    End Function
#End Region

#Region "Constructors"

    Private Sub New()
        'prevent direct creation
    End Sub

#End Region

#Region "Data Access"

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)

        _factoryMasterSpecValue = FactoryMasterSpecValueInfo.GetFactoryMasterSpecValueInfo(dataReader)
        _description = dataReader.GetString("DESCRIPTION").Trim()
        _smsValue = dataReader.GetString("SMSVALUE").Trim()
        _specValue = dataReader.GetString("SPECVALUE").Trim()

        _assetSet = AssetSet.GetAssetSet(Me, dataReader)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As SqlTransaction)
        If Not _assetSet Is Nothing Then _assetSet.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region
    
End Class