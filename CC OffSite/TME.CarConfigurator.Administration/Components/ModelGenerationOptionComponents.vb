Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
Imports TME.BusinessObjects.Templates.Exceptions

Namespace Components
    <Serializable()>
    Public Class ModelGenerationOptionComponents
        Inherits ContextUniqueGuidListBase(Of ModelGenerationOptionComponents, ModelGenerationOptionComponent)

#Region "Business Properties & Methods"
        Friend Property [Option] As ModelGenerationOption
            Get
                Return DirectCast(Parent, ModelGenerationOption)
            End Get
            Private Set(ByVal value As ModelGenerationOption)
                SetParent(value)
                ResetPermissions()
            End Set
        End Property

        Friend Sub ResetPermissions()
            SetPermissions([Option].Owner.Equals(MyContext.GetContext().CountryCode) AndAlso Not [Option].SuffixOption)
        End Sub

        Friend Sub SetPermissions(ByVal permission As Boolean)
            AllowNew = permission
            AllowEdit = permission
            AllowRemove = permission
        End Sub

        Public Overloads Function Add(ByVal accessory As ModelGenerationAccessory) As ModelGenerationOptionAccessoryComponent
            Dim component = ModelGenerationOptionAccessoryComponent.NewAccessoryComponent(accessory)
            TryAdd(component)
            Return component
        End Function

        Public Overloads Function Add(ByVal factoryOptionValue As ModelGenerationFactoryOptionValue) As ModelGenerationOptionFactoryOptionValueComponent
            Dim component = ModelGenerationOptionFactoryOptionValueComponent.NewFactoryOptionValueComponent(factoryOptionValue)
            TryAdd(component)
            Return component
        End Function

        Public Sub AddRange(ByVal components As List(Of ModelGenerationFactoryOptionValue))
            components.ForEach(Function(c) Add(c))
        End Sub


        Friend Overloads Function Add(ByVal dataReader As SafeDataReader) As ModelGenerationOptionComponent
            Dim component = ModelGenerationOptionComponent.GetComponent(dataReader)
            Add(component) 'is in database => doesn't need to be validated before adding
            Return component
        End Function

        Private Sub TryAdd(ByVal component As ModelGenerationOptionComponent)
            ValidateNewComponent(component)
            Add(component)
        End Sub

        Private Sub ValidateNewComponent(ByVal component As ModelGenerationOptionComponent)
            If Not Contains(component.ID) Then Return
            Throw New ObjectAlreadyExistsException(String.Format("This option already contains the component {0}", component.Description))
        End Sub
#End Region

#Region "Shared Factory Methods"
        Friend Shared Function NewComponents(ByVal modelGenerationOption As ModelGenerationOption) As ModelGenerationOptionComponents
            Dim components = New ModelGenerationOptionComponents()
            components.Option = modelGenerationOption
            Return components
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
            MarkAsChild()
        End Sub
#End Region

    End Class

    <Serializable()>
    Public MustInherit Class ModelGenerationOptionComponent
        Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationOptionComponent)
        Implements IUpdatableAssetSet

#Region "Business Properties & Methods"
        Public MustOverride ReadOnly Property Description() As String
        Public MustOverride ReadOnly Property Type() As String
        Friend MustOverride ReadOnly Property AssetSet As AssetSet Implements IHasAssetSet.AssetSet
        Public MustOverride ReadOnly Property Entity() As Entity Implements IHasAssetSet.Entity
        Public MustOverride ReadOnly Property Owner() As String Implements IHasAssetSet.Owner
        Public MustOverride ReadOnly Property GenerationID() As Guid Implements IHasAssetSet.GenerationID

        Public Sub Remove()
            DirectCast(Parent, ModelGenerationOptionComponents).Remove(Me)
        End Sub


        Friend MustOverride Sub ChangeReference(ByVal updatedAssetSet As AssetSet) Implements IUpdatableAssetSet.ChangeReference

#End Region

#Region "Shared Factory Methods"
        Friend Shared Function GetComponent(ByVal dataReader As SafeDataReader) As ModelGenerationOptionComponent
            Dim componentEntityString = dataReader.GetString("ENTITY")
            Dim componentEntity = DirectCast([Enum].Parse(GetType(Entity), componentEntityString), Entity)

            Select Case componentEntity
                Case Entity.ACCESSORY
                    Return ModelGenerationOptionAccessoryComponent.GetAccessoryComponent(dataReader)
                Case Entity.FACTORYGENERATIONOPTIONVALUE
                    Return ModelGenerationOptionFactoryOptionValueComponent.GetFactoryOptionValueComponent(dataReader)
            End Select

            Throw New ArgumentException(String.Format("The database returned a {0} component, which is not valid for a ModelGenerationOption.", componentEntityString))
        End Function
#End Region

#Region "Constructors"
        Protected Sub New()
            'prevent direct creation
            MarkAsChild()
        End Sub
#End Region

#Region "Data Access"
        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            Dim parentOption = DirectCast(Parent, ModelGenerationOptionComponents).Option
            With command.Parameters
                .AddWithValue("@OPTIONID", parentOption.ID)
                .AddWithValue("@GENERATIONID", parentOption.Generation.ID)
            End With
        End Sub

        Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
            Dim parentOption = DirectCast(Parent, ModelGenerationOptionComponents).Option
            With command.Parameters
                .AddWithValue("@OPTIONID", parentOption.ID)
                .AddWithValue("@GENERATIONID", parentOption.Generation.ID)
            End With

            command.CommandText = "deleteModelGenerationOptionComponent"
        End Sub
#End Region



    End Class
End Namespace