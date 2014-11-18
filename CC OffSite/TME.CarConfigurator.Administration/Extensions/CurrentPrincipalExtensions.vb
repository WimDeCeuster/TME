Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module CurrentPrincipalExtensions
        <Extension>
        Public Function IsInAnyRole(ByVal principal As IPrincipal) As Boolean
            Return _
                principal.IsInRole("NMSC Administrator") OrElse _
                principal.IsInRole("MKT Administrator") OrElse _
                principal.IsInRole("NMSC ACCESSORY Administrator") OrElse _
                principal.IsInRole("ISG Administrator") OrElse _
                principal.IsInRole("CSG Administrator") OrElse _
                principal.IsInRole("CORE Administrator") OrElse _
                principal.IsInRole("BASE Administrator")
        End Function
    End Module
End Namespace