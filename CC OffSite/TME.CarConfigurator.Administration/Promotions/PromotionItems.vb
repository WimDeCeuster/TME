Imports TME.CarConfigurator.Administration.BaseObjects
Imports TME.BusinessObjects.Templates.Exceptions
Imports TME.CarConfigurator.Administration.Promotions.Enums

Namespace Promotions
    <Serializable()>
    Public NotInheritable Class PromotionItems
        Inherits StronglySortedListBase(Of PromotionItems, PromotionItem)

#Region "Business Properties & Methods"
        Public Property Promotion As Promotion
            Get
                Return DirectCast(Parent, Promotion)
            End Get
            Private Set(ByVal value As Promotion)
                SetParent(value)
            End Set
        End Property

#Region "Add promotionItems in list"

        Public Shadows Function Add(ByVal accessory As Accessory) As PromotionItem
            ValidateNewItemAgainstPromotion(PromotionEntity.EQUIPMENT, "accessories") 'Can only add accessory if promotion is defined on equipment
            Return Add(PromotionItem.NewPromotionItem(accessory, accessory.PartNumber, PromotionEntity.ACCESSORY))
        End Function

        Public Shadows Function Add(ByVal [option] As [Option]) As PromotionItem
            ValidateNewItemAgainstPromotion(PromotionEntity.EQUIPMENT, "options") 'Can only add option if promotion is defined on equipment
            Return Add(PromotionItem.NewPromotionItem([option]))
        End Function

        Public Shadows Function Add(ByVal exteriorColour As ExteriorColour) As PromotionItem
            ValidateNewItemAgainstPromotion(PromotionEntity.EXTERIORCOLOUR, "exterior colours") 'Can only add exteriorcolour if promotion is defined on exteriorcolour
            Return Add(PromotionItem.NewPromotionItem(exteriorColour, exteriorColour.Code))
        End Function

        Public Shadows Function Add(ByVal exteriorColourType As ExteriorColourType) As PromotionItem
            ValidateNewItemAgainstPromotion(PromotionEntity.EXTERIORCOLOURTYPE, "exterior colour types") 'Can only add exteriorcolourtype if promotion is defined on exteriorcolourtype
            Return Add(PromotionItem.NewPromotionItem(exteriorColourType))
        End Function

        Public Shadows Function Add(ByVal upholstery As Upholstery) As PromotionItem
            ValidateNewItemAgainstPromotion(PromotionEntity.UPHOLSTERY, "upholsteries") 'Can only add upholsteries if promotion is defined on upholsteries
            Return Add(PromotionItem.NewPromotionItem(upholstery, upholstery.Code))
        End Function

        Public Shadows Function Add(ByVal upholsteryType As UpholsteryType) As PromotionItem
            ValidateNewItemAgainstPromotion(PromotionEntity.UPHOLSTERYTYPE, "upholstery types") 'Can only add upholstery types if promotion is defined on upholstery types
            Return Add(PromotionItem.NewPromotionItem(upholsteryType))
        End Function

        Public Shadows Function Add(ByVal pack As ModelGenerationPack) As PromotionItem
            ValidateNewItemAgainstPromotion(PromotionEntity.PACK, "packs") 'Can only add packs if promotion is defined on packs
            AddPromotionApplicabilityForGenerationIfNotExists(pack.Generation) 'Add an applicability for the pack's generation to the promotion
            Return Add(PromotionItem.NewPromotionItem(pack, pack.Generation.Name)) 'create and return the promotion item
        End Function

        Private Shadows Function Add(ByVal promotionItem As PromotionItem) As PromotionItem
            If Contains(promotionItem.ID) Then
                Throw New ObjectAlreadyExistsException(String.Format("This promotion already contains the item '{0}'.", promotionItem.Name))
            End If

            MyBase.Add(promotionItem)
            Return promotionItem
        End Function

#End Region

        Private Sub ValidateNewItemAgainstPromotion(ByVal itemEntity As PromotionEntity, ByVal itemEntityName As String)
            If Not Promotion.PromotionEntity = itemEntity Then 'Can only add item if promotion is defined on item's entity
                Throw New ArgumentException(String.Format("This promotion does not accept {0}.", itemEntityName))
            End If
        End Sub

        Private Sub AddPromotionApplicabilityForGenerationIfNotExists(ByVal generation As ModelGeneration)
            Dim generationInfo = generation.GetInfo() 'Get the modelgenerationinfo for this generation
            'check if promotion has an applicability for this generation => if not, add one
            If Not Promotion.Applicabilities.Contains(generationInfo) Then Promotion.Applicabilities.Add(generationInfo)
        End Sub

#End Region

#Region "Shared Factory Methods"

        Friend Shared Function GetPromotionItems(ByVal promotion As Promotion) As PromotionItems
            Dim promotionItems As PromotionItems

            If promotion.IsNew Then
                promotionItems = New PromotionItems()
            Else
                promotionItems = DataPortal.Fetch(Of PromotionItems)(New ParentCriteria(promotion.ID, "@PROMOTIONID"))
            End If

            promotionItems.Promotion = promotion

            Return promotionItems
        End Function

#End Region

#Region "Constructors"
        Private Sub New()
            'Prevent direct creation
            MarkAsChild()
        End Sub
#End Region

    End Class

    <Serializable()>
    Public NotInheritable Class PromotionItem
        Inherits ContextUniqueGuidBusinessBase(Of PromotionItem)
        Implements ISortedIndex, ISortedIndexSetter

#Region "Business Propteries & Methods"

        Private _code As String
        Private _name As String
        Private _translation As String
        Private _entity As Nullable(Of PromotionEntity)
        Private _index As Integer

        Public ReadOnly Property Code As String
            Get
                Return _code
            End Get
        End Property
        Public ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property
        Public ReadOnly Property Translation As String
            Get
                Return _translation
            End Get
        End Property

        Public ReadOnly Property Entity As Nullable(Of PromotionEntity)
            Get
                'if entity isn't defined, use parent's entity to return
                Return If(_entity Is Nothing, Promotion.PromotionEntity, _entity)
            End Get
        End Property

        Public ReadOnly Property Promotion As Promotion
            Get
                Return DirectCast(Parent, PromotionItems).Promotion
            End Get
        End Property

        Public ReadOnly Property Index() As Integer Implements ISortedIndex.Index
            Get
                Return _index
            End Get
        End Property

        Friend WriteOnly Property SetIndex() As Integer Implements ISortedIndexSetter.SetIndex
            Set(ByVal value As Integer)
                If Not _index.Equals(value) Then
                    _index = value
                    PropertyHasChanged("Index")
                End If
            End Set
        End Property

#End Region

#Region "Shared Factory Methods"
        Friend Shared Function NewPromotionItem(ByVal obj As TranslateableBusinessBase, Optional ByVal code As String = "", Optional ByVal entity As Nullable(Of PromotionEntity) = Nothing) As PromotionItem
            Dim promotionItem = New PromotionItem()
            promotionItem.Create()
            promotionItem.ID = obj.ID
            promotionItem._code = code
            promotionItem._name = obj.GetBaseName()
            promotionItem._translation = obj.Translation.Name
            promotionItem._entity = entity
            Return promotionItem
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
            MarkAsChild()
        End Sub
#End Region

#Region "Data Access"
        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            _name = dataReader.GetString(GetFieldName("NAME"))

            If dataReader.FieldExists("CODE") Then _code = dataReader.GetString(GetFieldName("CODE"))
            If dataReader.FieldExists("TRANSLATION") Then _translation = dataReader.GetString(GetFieldName("TRANSLATION"))


            Dim promotionItemEntity = dataReader.GetString(GetFieldName("ENTITY"), "NOTHING") 'Take default of NOTHING if not specified
            If promotionItemEntity.Equals("NOTHING") Then
                _entity = Nothing
            Else
                _entity = DirectCast([Enum].Parse(GetType(PromotionEntity), promotionItemEntity), PromotionEntity)
            End If

            _index = dataReader.GetInt16("SORTORDER")

            MyBase.FetchFields(dataReader)
        End Sub

        Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As SqlCommand)
            With command.Parameters
                .AddWithValue("@PROMOTIONID", Promotion.ID)
                .AddWithValue("@OBJECTID", ID)
                .AddWithValue("@SORTORDER", Index) 'can be done in addinsertcommandfields (instead of specialized)
            End With
        End Sub

        Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As SqlCommand)
            With command.Parameters
                .AddWithValue("@PROMOTIONID", Promotion.ID)
                .AddWithValue("@OBJECTID", ID)
                .AddWithValue("@SORTORDER", Index) 'can be done in addupdatecommandfields (instead of specialized)
            End With
        End Sub

        Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As SqlCommand)
            With command.Parameters
                .AddWithValue("@PROMOTIONID", Promotion.ID)
                .AddWithValue("@OBJECTID", ID)
            End With
        End Sub
#End Region

    End Class
End Namespace