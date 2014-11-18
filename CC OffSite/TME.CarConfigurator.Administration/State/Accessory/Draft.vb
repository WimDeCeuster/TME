Namespace State.Accessory
    <Serializable> Public Class Draft
        Inherits AccessoryState

        Friend Sub New(ByVal accessory As Administration.Accessory)
            MyBase.New(accessory)
            DBValue = StateValue.Draft
        End Sub

        Public Overrides Function ToString() As String
            Return "Draft"
        End Function
    End Class
End Namespace