Imports TME.CarConfigurator.Administration.Assets

Namespace Interfaces
    Public Interface IHasAssetSet
        Inherits IGenerationObject
        Inherits IEntityObject
        Inherits IOwnedBy

        ReadOnly Property AssetSet() As AssetSet
    End Interface

    Friend Interface IUpdatableAssetSet
        Inherits IHasAssetSet

        Sub ChangeReference(ByVal updatedAssetSet As AssetSet)
    End Interface
End Namespace
