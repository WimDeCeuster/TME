Imports TME.CarConfigurator.Administration.Exceptions

Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class FilterValueEquipment
        Inherits ContextListBase(Of FilterValueEquipment, FilterValueEquipmentItem)

#Region " Business Properties & Methods "
        Public Property FilterValue() As FilterValue
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, FilterValue)
            End Get
            Private Set(value As FilterValue)
                SetParent(value)
                SetSecurityRights()
            End Set
        End Property

        Private Sub SetSecurityRights()
            Dim ownedByMe = FilterValue.Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase)
            AllowNew = ownedByMe
            AllowEdit = ownedByMe
            AllowRemove = ownedByMe

            For Each equipmentItem As FilterValueEquipmentItem In Me
                equipmentItem.SetSecurityRights()
            Next
        End Sub

        Default Public Overloads ReadOnly Property Item(ByVal id As Guid) As FilterValueEquipmentItem
            Get
                Return FirstOrDefault(Function(x) x.ID.Equals(id))
            End Get
        End Property



        Public Shadows Function Add(ByVal equipmentItem As EquipmentItemInfo) As FilterValueEquipmentItem
            If Any(Function(x) x.ID.Equals(equipmentItem.ID)) Then Throw New ObjectAlreadyExists("The item is already a part of the collection")

            Dim value = FilterValueEquipmentItem.NewEquipmentFilterValueEquipmentItem(equipmentItem)
            MyBase.Add(value)
            Return value
        End Function

        Public Overloads Sub Remove(ByVal id As Guid)
            Dim toBeRemoved = FirstOrDefault(Function(x) x.ID.Equals(id))
            If toBeRemoved IsNot Nothing Then Remove(toBeRemoved)
        End Sub

#End Region

#Region " Shared Factory Methods "
        Friend Shared Function GetEquipmentFilterValueEquipment(ByVal filterValue As FilterValue) As FilterValueEquipment
            Dim equipment As FilterValueEquipment
            If filterValue.IsNew Then
                equipment = New FilterValueEquipment
            Else
                equipment = DataPortal.Fetch(Of FilterValueEquipment)(New ParentCriteria(filterValue.ID, "@FILTERVALUEID"))
            End If
            equipment.FilterValue = filterValue
            Return equipment
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            MarkAsChild()
        End Sub
#End Region



    End Class
End Namespace