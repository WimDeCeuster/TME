Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

Namespace Extensions
    Public Module VisibilityExtensions
        <Extension>
        Public Function AsString(ByVal visibility As ItemVisibility) As String
            If visibility = ItemVisibility.None Then Return "Nowhere"
            If (visibility And ItemVisibility.Website) = ItemVisibility.Website AndAlso (visibility And ItemVisibility.CarConfigurator) = ItemVisibility.CarConfigurator AndAlso (visibility And ItemVisibility.Brochure) = ItemVisibility.Brochure Then Return "Everywhere"

            If visibility = ItemVisibility.Website Then Return "Website Only"
            If visibility = ItemVisibility.CarConfigurator Then Return "Car Configurator Only"
            If visibility = ItemVisibility.Brochure Then Return "Brochure Only"
            

            Dim list = New List(Of String)()
            If (visibility And ItemVisibility.Website) = ItemVisibility.Website Then list.Add("Website")
            If (visibility And ItemVisibility.CarConfigurator) = ItemVisibility.CarConfigurator Then list.Add("Car Configurator")
            If (visibility And ItemVisibility.Brochure) = ItemVisibility.Brochure Then list.Add("Brochure")

            Dim result = String.Join(" and ", list)
            Return result
        End Function
    End Module
End Namespace