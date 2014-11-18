Imports TME.CarConfigurator.Administration.Assets

Namespace Exceptions
    <Serializable()> Public Class AssetTypeAlreadyExistsException
        Inherits SystemException

        Public Sub New()
            MyBase.New("There is already such an asset the current collection")
        End Sub
        Public Sub New(ByVal assetType As AssetType)
            MyBase.New("The current collection already contains a " & assetType.Name & " asset.")
        End Sub
        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(ByVal message As String, ByVal innerException As Exception)
            MyBase.New(message, innerException)
        End Sub
        Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class
End NameSpace