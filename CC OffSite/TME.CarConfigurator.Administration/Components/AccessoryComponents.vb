Imports TME.BusinessObjects.Templates.Exceptions

Namespace Components

    <Serializable> Public Class AccessoryComponents
        Inherits ContextUniqueGuidListBase(Of AccessoryComponents, AccessoryComponent)

#Region "Business Properties & Methods"
        Friend ReadOnly Property Accessory() As Accessory
            Get
                Return DirectCast(Parent, Accessory)
            End Get
        End Property

        Public Shadows Function Add(ByVal accessoryToAdd As Accessory) As AccessoryComponent
            ValidateNewComponent(accessoryToAdd)

            Dim component = AccessoryComponent.NewAccessoryComponent(accessoryToAdd)
            MyBase.Add(component)
            Return component
        End Function

        Private Sub ValidateNewComponent(ByVal accessoryToAdd As Accessory)
            If Not Contains(accessoryToAdd.ID) Then Return
            Throw New ObjectAlreadyExistsException(String.Format("The accessory '{0}' already contains the component '{1}'.", Accessory.PartNumber, accessoryToAdd.PartNumber))
        End Sub
#End Region

#Region "Shared Factory Methods"
        Public Shared Function NewAccessoryComponents(ByVal accessory As Accessory) As AccessoryComponents
            Dim components = New AccessoryComponents()
            components.SetParent(accessory)
            Return components
        End Function

        Public Shared Function GetAccessoryComponents(ByVal accessory As Accessory) As AccessoryComponents
            Dim components = DataPortal.Fetch(Of AccessoryComponents)(New ParentCriteria(accessory.ID, "@EQUIPMENTID"))
            components.SetParent(accessory)
            Return components
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
        End Sub
#End Region


    End Class

    <Serializable> Public Class AccessoryComponent
        Inherits ContextUniqueGuidBusinessBase(Of AccessoryComponent)
#Region "Business Properties & Methods"
        Friend ReadOnly Property Accessory As Accessory
            Get
                Return DirectCast(Parent, AccessoryComponents).Accessory
            End Get
        End Property
#End Region

#Region "Shared Factory Methods"
        Public Shared Function NewAccessoryComponent(ByVal accessory As Accessory) As AccessoryComponent
            Dim component = New AccessoryComponent()
            component.Create(accessory.ID)
            Return component
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
        End Sub
#End Region

#Region "Data Access"
        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@ACCESSORYID", Accessory.ID)
        End Sub

        Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@ACCESSORYID", Accessory.ID)
        End Sub
#End Region
    End Class
End Namespace