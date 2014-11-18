Namespace State.Accessory
    <Serializable> Public Class Published
        Inherits AccessoryState

        Friend Sub New(ByVal accessory As Administration.Accessory)
            MyBase.New(accessory)
            DBValue = StateValue.Published
        End Sub

        Public Overrides Function ToString() As String
            Return "Published"
        End Function
    End Class
End Namespace