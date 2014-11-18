Namespace State.Accessory
    <Serializable> Public Class PhasedOut
        Inherits AccessoryState

        Friend Sub New(ByVal accessory As Administration.Accessory)
            MyBase.New(accessory)
            DBValue = StateValue.PhasedOut
        End Sub

        Public Overrides Function ToString() As String
            Return "Phased out"
        End Function
    End Class
End Namespace