
Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.CrossModel

    Public Class CrossModelPermissionBroker
        Implements ICrossModelPermissionBroker
        Private Property Context() As MyContext

#Region "Shared Factory Methods"
        Friend Shared Function GetBroker(Optional ByVal context As MyContext = Nothing) As CrossModelPermissionBroker
            If context Is Nothing Then context = MyContext.GetContext()
            Return New CrossModelPermissionBroker(context)
        End Function
#End Region

#Region "Constructors"
        Private Sub New(ByVal context As MyContext)
            Me.Context = context
        End Sub
#End Region

#Region "Permission Objects"
        Private _translationsAndLinksEquipmentItemPermissions As IEquipmentItemPermissions
        Private _pricestranslationsAndLinksEquipmentItemPermissions As IEquipmentItemPermissions
        Private _noEquipmentItemPermissions As IEquipmentItemPermissions
        Private _allEquipmentItemPermissions As IEquipmentItemPermissions

        Private ReadOnly Property TranslationsAndLinksEquipmentItemPermissions As IEquipmentItemPermissions
            Get
                If _translationsAndLinksEquipmentItemPermissions IsNot Nothing Then Return _translationsAndLinksEquipmentItemPermissions
                _translationsAndLinksEquipmentItemPermissions = New EquipmentItemPermissions() With {
                    .Translate = True,
                    .Links = True
                    }
                Return _translationsAndLinksEquipmentItemPermissions
            End Get
        End Property

        Private ReadOnly Property PricesTranslationsAndLinksEquipmentItemPermissions As IEquipmentItemPermissions
            Get
                If _pricestranslationsAndLinksEquipmentItemPermissions IsNot Nothing Then Return _pricestranslationsAndLinksEquipmentItemPermissions
                _pricestranslationsAndLinksEquipmentItemPermissions = New EquipmentItemPermissions() With {
                    .Price = True,
                    .Translate = True,
                    .Links = True
                    }
                Return _pricestranslationsAndLinksEquipmentItemPermissions
            End Get
        End Property

        Private ReadOnly Property NoEquipmentItemPermissions As IEquipmentItemPermissions
            Get
                If _noEquipmentItemPermissions IsNot Nothing Then Return _noEquipmentItemPermissions
                _noEquipmentItemPermissions = New EquipmentItemPermissions()
                Return _noEquipmentItemPermissions
            End Get
        End Property

        Private ReadOnly Property AllEquipmentItemPermissions As IEquipmentItemPermissions
            Get
                If _allEquipmentItemPermissions IsNot Nothing Then Return _allEquipmentItemPermissions
                _allEquipmentItemPermissions = New EquipmentItemPermissions() With {
                    .Activate = True,
                    .Approve = True,
                    .Create = True,
                    .Delete = True,
                    .Links = True,
                    .Preview = True,
                    .Price = True,
                    .Sort = True,
                    .Translate = True,
                    .Update = True,
                    .UsesNonVATPrice = True,
                    .ViewAssets = True,
                    .ViewDetails = True
                    }
                Return _allEquipmentItemPermissions
            End Get
        End Property
#End Region

        Public Function GetAccessoryPermissions(ByVal accessory As Accessory) As IEquipmentItemPermissions Implements ICrossModelPermissionBroker.GetAccessoryPermissions
            'flow chart at O:\1810\Projects\Product Planning\CarDB intergration Localisation Extension\07_Analysis\Flowcharts\A2A accessory permissions.vsd
            If Context.IsGlobal() Then Return GetGlobalAccessoryPermissions(accessory)
            Return GetLocalAccessoryPermissions(accessory)
        End Function

#Region "Helper methods"
        Private Function GetGlobalAccessoryPermissions(ByVal accessory As Accessory) As IEquipmentItemPermissions
            If accessory.IsLocalizedInA2A() OrElse GlobalIsLocalizedInA2A() Then
                If accessory.IsOwnedByGlobal() Then Return TranslationsAndLinksEquipmentItemPermissions 'genuine accessory
                Return NoEquipmentItemPermissions
            End If
            Return AllEquipmentItemPermissions
        End Function

        Private Function GetLocalAccessoryPermissions(ByVal accessory As Accessory) As IEquipmentItemPermissions
            If accessory.IsOwnedByEurope() Then Return TranslationsAndLinksEquipmentItemPermissions

            Dim permissions = PricesTranslationsAndLinksEquipmentItemPermissions
            If Not Context.Country.LocalizedInA2A Then permissions.Approve = True
            Return permissions
        End Function
        Private Function GlobalIsLocalizedInA2A() As Boolean
            Dim globalCountry = MyContext.GetContext().Countries.First(Function(country) country.Code = Environment.GlobalCountryCode)

            Return globalCountry.LocalizedInA2A
        End Function


#End Region
    End Class
End Namespace