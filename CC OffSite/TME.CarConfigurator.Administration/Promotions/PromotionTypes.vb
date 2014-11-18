Namespace Promotions
    <Serializable()>
    Public NotInheritable Class PromotionTypes
        Inherits ContextReadOnlyListBase(Of PromotionTypes, PromotionType)

#Region "Shared Factory Methods"

        Public Shared Function GetPromotionTypes() As PromotionTypes
            Return DataPortal.Fetch(Of PromotionTypes)(New Criteria)
        End Function

#End Region

#Region "Constructors"
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

    End Class

    <Serializable()>
    Public NotInheritable Class PromotionType
        Inherits ContextReadOnlyBase(Of PromotionType)

#Region "Business Properties"

        Private _id As Short
        Private _name As String
        Private _description As String

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
#End Region

#Region "Framework Overrides"
        Protected Overrides Function GetIdValue() As Object
            Return ID
        End Function
#End Region

#Region "Shared Factory Methods"

        Friend Shared Function GetPromotionType(ByVal dataReader As SafeDataReader) As PromotionType
            Dim type = New PromotionType()
            type.FieldPrefix = "PROMOTIONTYPE"
            type.Fetch(dataReader)
            Return type
        End Function

#End Region

#Region "Constructors"
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region "Data Access"
        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            _id = dataReader.GetInt16(GetFieldName("ID"))
            _name = dataReader.GetString(GetFieldName("NAME"))
            _description = dataReader.GetString(GetFieldName("DESCRIPTION"))

            MyBase.FetchFields(dataReader)
        End Sub
#End Region

#Region "Object overrides"
        Public Overloads Function Equals(ByVal otherType As PromotionType) As Boolean
            Return ID = otherType.ID AndAlso Name.Equals(otherType.Name) AndAlso Description.Equals(otherType.Description)
        End Function
#End Region

    End Class
End Namespace