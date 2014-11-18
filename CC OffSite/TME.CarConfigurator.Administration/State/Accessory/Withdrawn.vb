Namespace State.Accessory
    <Serializable> Public Class Withdrawn
        Inherits AccessoryState

        Friend Sub New(ByVal accessory As Administration.Accessory)
            MyBase.New(accessory)
            DBValue = StateValue.Withdrawn
        End Sub
        
        Public Overrides Function ToString() As String
            Return "Withdrawn"
        End Function
    End Class
End Namespace