Namespace Promotions.Enums
    <Flags()>
    Public Enum DiscountPriceApplicability
        NetPrice = 1
        Tax = 2
        ListPrice = NetPrice Or Tax
    End Enum
End NameSpace