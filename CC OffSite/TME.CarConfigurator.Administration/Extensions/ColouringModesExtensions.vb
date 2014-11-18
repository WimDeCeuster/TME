Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

Namespace Extensions
    Public Module ColouringModesExtensions
        <Extension>
        Public Function AsString(ByVal colouringModes As ColouringModes) As String
            If colouringModes = colouringModes.None Then Return "None"
            If colouringModes = colouringModes.BodyColour Then Return "Body Colour"
            If colouringModes = colouringModes.PrimaryAccentColour Then Return "Primary Accent Colour"
            If colouringModes = colouringModes.SecondaryAccentColour Then Return "Secondary Accent Colour"

            Dim list = New List(Of String)()
            If (colouringModes And colouringModes.BodyColour) = colouringModes.BodyColour Then list.Add("Body")
            If (colouringModes And colouringModes.PrimaryAccentColour) = colouringModes.PrimaryAccentColour Then list.Add("Primary Accent")
            If (colouringModes And colouringModes.SecondaryAccentColour) = colouringModes.SecondaryAccentColour Then list.Add("Secondary Accent")

            Dim result = String.Join(", ", list)
            Dim lastSeperator = result.LastIndexOf(", ", StringComparison.Ordinal)
            result = result.Remove(lastSeperator, 2).Insert(lastSeperator, " or ")
            result = result + " Colour"
            Return result

        End Function
    End Module
End Namespace