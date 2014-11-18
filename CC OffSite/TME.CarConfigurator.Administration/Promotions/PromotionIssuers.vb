Namespace Promotions

    <Serializable()>
    Public NotInheritable Class PromotionIssuers
        Inherits BaseObjects.ContextReadOnlyListBase(Of PromotionIssuers, PromotionIssuer)

#Region "Shared Factory Methods"

        Public Shared Function GetPromotionIssuers() As PromotionIssuers
            Return DataPortal.Fetch(Of PromotionIssuers)(New Criteria)
        End Function

#End Region

#Region "Constructors"
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

    End Class

    <Serializable()>
    Public NotInheritable Class PromotionIssuer
        Inherits BaseObjects.ContextReadOnlyBase(Of PromotionIssuer)

#Region "Business Properties"

        Private _id As Short
        Private _name As String
        Private _description As String
        Private _code As String

        Public ReadOnly Property ID As Short
            Get
                Return _id
            End Get
        End Property

        Public ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        Public ReadOnly Property Description As String
            Get
                Return _description
            End Get
        End Property

        Public ReadOnly Property Code As String
            Get
                Return _code
            End Get
        End Property

#End Region

#Region "System.Object Overrides"
        Public Overloads Function Equals(ByVal otherPromotionIssuer As PromotionIssuer) As Boolean
            If otherPromotionIssuer Is Nothing Then Return False
            Return Code.Equals(otherPromotionIssuer.Code)
        End Function
#End Region

#Region "Framework Overrides"

        Protected Overrides Function GetIdValue() As Object
            Return ID
        End Function

#End Region

#Region "Shared Factory Methods"

        Friend Shared Function GetPromotionIssuer(ByVal dataReader As SafeDataReader) As PromotionIssuer
            Dim issuer = New PromotionIssuer
            issuer.FieldPrefix = "PROMOTIONISSUER"
            issuer.Fetch(dataReader)
            Return issuer
        End Function

#End Region

#Region "Constructors"

        Private Sub New()
            ' prevent direct creation
        End Sub

#End Region

#Region "Data Access"

        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            _id = dataReader.GetInt16(GetFieldName("ID"))
            _name = dataReader.GetString(GetFieldName("NAME"))
            _description = dataReader.GetString(GetFieldName("DESCRIPTION"))
            _code = dataReader.GetString(GetFieldName("CODE"))

            MyBase.FetchFields(dataReader)
        End Sub

#End Region

    End Class
End Namespace