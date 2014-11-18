Imports System.Collections.Generic

Namespace Components.Interfaces
    Public Interface ICanHaveComponentsWithAssets
        ReadOnly Property Components As IEnumerable(Of IHasAssetSet)
    End Interface
End Namespace