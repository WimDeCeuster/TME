Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module OwnerExtensions
        <Extension>
        Public Function IsOwnedByGlobal(ByVal ownedBy As IOwnedBy) As Boolean
            Return ownedBy.Owner.Equals("ZZ", StringComparison.InvariantCultureIgnoreCase)
        End Function
        <Extension>
        Public Function IsOwnedByEurope(ByVal ownedBy As IOwnedBy) As Boolean
            Return ownedBy.Owner.Equals("EU", StringComparison.InvariantCultureIgnoreCase)
        End Function
        <Extension>
        Public Function IsLocalizedInA2A(ByVal ownedBy As IOwnedBy) As Boolean
            Dim country = MyContext.GetContext().Countries(ownedBy.Owner)
            If country Is Nothing Then Return False
            Return country.LocalizedInA2A
        End Function
    End Module
End Namespace