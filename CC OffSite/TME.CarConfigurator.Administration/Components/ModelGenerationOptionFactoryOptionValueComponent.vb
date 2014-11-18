Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums

Namespace Components
    <Serializable()>
    Public Class ModelGenerationOptionFactoryOptionValueComponent
        Inherits ModelGenerationOptionComponent
#Region "Business Properties & Methods"

        Private _factoryOptionValue As ModelGenerationFactoryOptionValue

        Private Property FactoryOptionValue As ModelGenerationFactoryOptionValue
            Get
                If _factoryOptionValue Is Nothing Then
                    Dim factoryGenerations = DirectCast(Parent, ModelGenerationOptionComponents).Option.Generation.FactoryGenerations
                    _factoryOptionValue = factoryGenerations _
                                            .SelectMany(Function(factoryGeneration) factoryGeneration.Options) _
                                            .SelectMany(Function(factoryGenerationOption) factoryGenerationOption.Values) _
                                            .Single(Function(factoryGenerationOptionValue) factoryGenerationOptionValue.ID = ID)
                End If

                Return _factoryOptionValue
            End Get
            Set(value As ModelGenerationFactoryOptionValue)
                _factoryOptionValue = value
            End Set
        End Property

        Public Overrides ReadOnly Property Description() As String
            Get
                Return FactoryOptionValue.FullDescription
            End Get
        End Property
        Public Overrides ReadOnly Property Type() As String
            Get
                Return "Option"
            End Get
        End Property

        Public ReadOnly Property FactoryMasterSpec() As FactoryMasterSpecInfo
            Get
                Return FactoryOptionValue.[Option].FactoryMasterSpec
            End Get
        End Property
        Public ReadOnly Property FactoryMasterSpecValue() As FactoryMasterSpecValueInfo
            Get
                Return FactoryOptionValue.FactoryMasterSpecValue
            End Get
        End Property

        Friend Overrides ReadOnly Property AssetSet() As AssetSet
            Get
                Return FactoryOptionValue.AssetSet
            End Get
        End Property

        Friend Overrides Sub ChangeReference(ByVal updatedAssetSet As AssetSet)
            FactoryOptionValue.ChangeReference(updatedAssetSet)
        End Sub

        Public Overrides ReadOnly Property Entity() As Entity
            Get
                Return FactoryOptionValue.Entity
            End Get
        End Property
        Public Overrides ReadOnly Property Owner() As String
            Get
                Return FactoryOptionValue.Owner
            End Get
        End Property
        Public Overrides ReadOnly Property GenerationID() As Guid
            Get
                Return FactoryOptionValue.GenerationID
            End Get
        End Property

#End Region

#Region "Shared Factory Methods"
        Friend Shared Function NewFactoryOptionValueComponent(ByVal factoryOptionValue As ModelGenerationFactoryOptionValue) As ModelGenerationOptionFactoryOptionValueComponent
            Dim component = New ModelGenerationOptionFactoryOptionValueComponent()
            component.Create(factoryOptionValue.ID)
            component.FactoryOptionValue = factoryOptionValue
            Return component
        End Function

        Public Shared Function GetFactoryOptionValueComponent(ByVal dataReader As SafeDataReader) As ModelGenerationOptionFactoryOptionValueComponent
            Dim component = New ModelGenerationOptionFactoryOptionValueComponent()
            component.Fetch(dataReader)
            Return component
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
        End Sub
#End Region


    End Class
End Namespace