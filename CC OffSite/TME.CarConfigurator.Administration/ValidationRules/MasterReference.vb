
Namespace ValidationRules
    Friend Class MasterReference

        Friend Shared Function Valid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
            Dim obj = DirectCast(target, IMasterObject)

            e.Description = "The description of the master is required!"

            Return obj.MasterID.Equals(Guid.Empty) OrElse Not String.IsNullOrEmpty(obj.MasterDescription)
        End Function


        Private Sub New()
            'prevent direct creation
        End Sub
    End Class
End Namespace
