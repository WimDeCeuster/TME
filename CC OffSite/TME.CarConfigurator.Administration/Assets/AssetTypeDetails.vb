Namespace Assets
    <Serializable()> Public Class AssetTypeDetails
        Implements IEquatable(Of AssetTypeDetails)

        Private _mode As String = String.Empty
        Private _view As String = String.Empty
        Private _side As String = String.Empty
        Private _type As String = String.Empty

        Public ReadOnly Property Mode() As String
            Get
                Return _mode
            End Get
        End Property
        Public ReadOnly Property View() As String
            Get
                Return _view
            End Get
        End Property
        Public ReadOnly Property Side() As String
            Get
                Return _side
            End Get
        End Property
        Public ReadOnly Property Type() As String
            Get
                Return _type
            End Get
        End Property

        Friend Sub New(ByVal assettype As AssetType)
            If assettype.Group.Mode.Length = 0 AndAlso assettype.Group.View.Length = 0 Then Return

            DisectDetails(assettype.Name)
        End Sub

        Friend Sub New(ByVal modelGenerationAssetType As ModelGenerationAssetType)
            DisectDetails(modelGenerationAssetType.Name)
        End Sub

        Private Sub DisectDetails(ByVal name As String)
            Dim parts() As String = name.Split("_"c)
            Dim length As Integer = parts.Length

            If length > 3 Then
                _mode = parts(0)
                _view = parts(1)
                _side = parts(2)
                _type = parts(3)

                Return
            End If

            If length > 2 Then
                _view = parts(0)
                _side = parts(1)
                _type = parts(2)
            End If
        End Sub

        Public Overloads Function Equals(ByVal other As AssetTypeDetails) As Boolean Implements IEquatable(Of AssetTypeDetails).Equals
            return Mode.Equals(other.Mode, StringComparison.InvariantCultureIgnoreCase) AndAlso 
                View.Equals(other.View, StringComparison.InvariantCultureIgnoreCase) AndAlso 
                Side.Equals(other.Side, StringComparison.InvariantCultureIgnoreCase) AndAlso 
                Type.Equals(other.Type, StringComparison.InvariantCultureIgnoreCase)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Mode.GetHashCode() Xor View.GetHashCode() Xor Side.GetHashCode() Xor Type.GetHashCode()
        End Function
    End Class
End Namespace