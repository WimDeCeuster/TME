Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums

Namespace Components
    <Serializable()>
    Public Class ModelGenerationOptionAccessoryComponent
        Inherits ModelGenerationOptionComponent
        Implements IMasterObjectReference


#Region "Business Properties & Methods"
        Private _accessory As ModelGenerationAccessory

        Private Property Accessory() As ModelGenerationAccessory
            Get
                If _accessory Is Nothing Then
                    _accessory = DirectCast(Parent, ModelGenerationOptionComponents).Option.Generation.Equipment.OfType(Of ModelGenerationAccessory).SingleOrDefault(Function(acc) acc.ID.Equals(ID))

                    If _accessory Is Nothing Then Throw New AccessoryComponentNotFoundException(String.Format("The accessory with id {0} is not found for option {1}", ID, DirectCast(Parent, ModelGenerationOptionComponents).Option))
                End If

                Return _accessory
            End Get
            Set(value As ModelGenerationAccessory)
                _accessory = value
            End Set
        End Property

        Public ReadOnly Property PartNumber() As String
            Get
                Return Accessory.PartNumber
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return Accessory.Name
            End Get
        End Property

        Public Overrides ReadOnly Property Description() As String
            Get
                Return String.Format("{0} : {1}", PartNumber, Name)
            End Get
        End Property

        Public Overrides ReadOnly Property Type() As String
            Get
                Return "Accessory"
            End Get
        End Property

        Friend Overrides ReadOnly Property AssetSet() As AssetSet
            Get
                Return Accessory.AssetSet
            End Get
        End Property

        Friend Overrides Sub ChangeReference(ByVal updatedAssetSet As AssetSet)
            Accessory.ChangeReference(updatedAssetSet)
        End Sub

        Public Overrides ReadOnly Property Entity() As Entity
            Get
                Return Accessory.Entity
            End Get
        End Property
        Public Overrides ReadOnly Property Owner() As String
            Get
                Return Accessory.Owner
            End Get
        End Property

        Public Overrides ReadOnly Property GenerationID() As Guid
            Get
                Return Accessory.GenerationID
            End Get
        End Property


        Public ReadOnly Property MasterID() As Guid Implements IMasterObjectReference.MasterID
            Get
                Return Accessory.MasterID
            End Get
        End Property

        Public ReadOnly Property MasterDescription() As String Implements IMasterObjectReference.MasterDescription
            Get
                Return Accessory.MasterDescription
            End Get
        End Property

#End Region

#Region "Shared Factory Methods"
        Friend Shared Function NewAccessoryComponent(ByVal accessory As ModelGenerationAccessory) As ModelGenerationOptionAccessoryComponent
            Dim component = New ModelGenerationOptionAccessoryComponent()
            component.Create(accessory.ID)
            component.Accessory = accessory
            Return component
        End Function

        Public Shared Function GetAccessoryComponent(ByVal dataReader As SafeDataReader) As ModelGenerationOptionAccessoryComponent
            Dim component = New ModelGenerationOptionAccessoryComponent()
            component.Fetch(dataReader)
            Return component
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
        End Sub
#End Region

#Region " Exceptions "
        Private Class AccessoryComponentNotFoundException
            Inherits Exception

            Public Sub New(ByVal message As String)
                MyBase.New(message)
            End Sub
        End Class
#End Region
    End Class
End Namespace