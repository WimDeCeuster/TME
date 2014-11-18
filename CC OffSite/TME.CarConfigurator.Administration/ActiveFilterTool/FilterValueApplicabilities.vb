Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class FilterValueApplicabilities
        Inherits ContextUniqueGuidListBase(Of FilterValueApplicabilities, FilterValueApplicability)

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

            For Each applicability As FilterValueApplicability In Me
                applicability.SetSecurityRights()
            Next
        End Sub

#End Region

#Region " Shared Factory Methods "
        Friend Shared Function GetFilterValueApplicabilities(ByVal filterValue As FilterValue) As FilterValueApplicabilities
            Dim applicabilities As FilterValueApplicabilities
            If filterValue.IsNew Then
                applicabilities = New FilterValueApplicabilities
            Else
                applicabilities = DataPortal.Fetch(Of FilterValueApplicabilities)(New ParentCriteria(filterValue.ID, "@FILTERVALUEID"))
            End If
            applicabilities.FilterValue = filterValue
            Return applicabilities
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