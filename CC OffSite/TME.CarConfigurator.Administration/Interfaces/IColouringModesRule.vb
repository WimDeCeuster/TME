Imports TME.CarConfigurator.Administration.Enums

Namespace Interfaces
    Public Interface IColouringModesRule
        Inherits IRule

        Property ColouringMode As ColouringModes
        ReadOnly Property Colour As ExteriorColourInfo
    End Interface
End Namespace