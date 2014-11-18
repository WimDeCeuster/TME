Imports TME.CarConfigurator.Administration.Enums

Namespace Assets
    <Serializable()> Public NotInheritable Class AssetTypes
        Inherits ContextReadOnlyListBase(Of AssetTypes, AssetType)

#Region " Business Properties & Methods "

        Private ReadOnly Property Group() As AssetTypeGroup
            Get
                Return DirectCast(Parent, AssetTypeGroup)
            End Get
        End Property

        Friend Shadows Sub Add(ByVal dataReader As SafeDataReader)
            Add(GetObject(dataReader))
        End Sub
        Private Shadows Sub Add(ByVal type As AssetType)
            RaiseListChangedEvents = False
            IsReadOnly = False
            MyBase.Add(type)
            IsReadOnly = True
            RaiseListChangedEvents = True
        End Sub

        Friend Sub RemoveNonExistingTypes(ByVal generation As ModelGeneration)
            IsReadOnly = False
            For i As Integer = Count - 1 To 0 Step -1
                If Not generation.AssetTypes.Contains(Me(i)) Then
                    RemoveAt(i)
                End If
            Next
            IsReadOnly = True
        End Sub

        Protected Overrides Function GetObject(ByVal dataReader As Common.Database.SafeDataReader) As AssetType
            Return AssetType.GetAssetType(dataReader, Group)
        End Function

#End Region

#Region " Shared Factory Methods "

        Friend Shared Function NewAssetTypes(ByVal group As AssetTypeGroup) As AssetTypes
            Dim types As AssetTypes = New AssetTypes
            types.SetParent(group)
            Return types
        End Function

        Public Shared Function GetAssetTypes() As AssetTypes
            Dim types As AssetTypes = New AssetTypes
            Dim groups As AssetTypeGroups = MyContext.GetContext().AssetTypeGroups

            For Each typeGroup In groups
                For Each type As AssetType In typeGroup.Types
                    types.Add(type)
                    type.Group = typeGroup
                Next
            Next

            Return types
        End Function
        Public Shared Function GetAssetTypes(ByVal entity As Entity) As AssetTypes
            Dim types As AssetTypes = New AssetTypes
            Dim groups As AssetTypeGroups = AssetTypeGroups.GetAssetTypeGroups(entity)

            For Each typeGroup In groups
                For Each type As AssetType In typeGroup.Types
                    types.Add(type)
                Next
            Next

            Return types
        End Function


#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            'Allow data portal to create us
        End Sub
#End Region

    End Class
    <Serializable()> Public NotInheritable Class AssetType
        Inherits ContextReadOnlyBase(Of AssetType)

#Region " Business Properties & Methods "
        Private _group As AssetTypeGroup
        Private _code As String
        Private _name As String
        Private _canBeUsedAsPreview As Boolean
        Private _details As AssetTypeDetails


        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Code() As String
            Get
                Return _code
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property
        <XmlInfo(XmlNodeType.None)> Public Property Group() As AssetTypeGroup
            Get
                If _group Is Nothing Then _group = MyContext.GetContext().AssetTypeGroups.FindType(Code).Group
                Return _group
            End Get
            Friend Set(ByVal value As AssetTypeGroup)
                _group = value
            End Set
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property CanBeUsedAsPreview() As Boolean
            Get
                Return _canBeUsedAsPreview
            End Get
        End Property
        Public ReadOnly Property Details() As AssetTypeDetails
            Get
                If _details Is Nothing Then _details = New AssetTypeDetails(Me)
                Return _details
            End Get
        End Property

#End Region

#Region " Framework Overrides "
        Protected Overrides Function GetIdValue() As Object
            Return Code
        End Function
#End Region

#Region " System.Object Overrides "

        Public Overloads Overrides Function ToString() As String
            Return Name
        End Function

        Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
            Return (String.Compare(Code, obj, True) = 0) OrElse (String.Compare(Name, obj, True) = 0)
        End Function

#End Region

#Region " Shared Factory Methods "

        Friend Shared Function GetAssetType(ByVal dataReader As SafeDataReader, ByVal group As AssetTypeGroup) As AssetType
            Dim type As AssetType = New AssetType
            type.Fetch(dataReader)
            type._group = group
            Return type
        End Function
        Friend Shared Function GetAssetType(ByVal dataReader As SafeDataReader) As AssetType
            Dim type As AssetType = New AssetType
            type.Fetch(dataReader)
            Return type
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            With dataReader
                If .FieldExists("ASSETTYPETYPE") Then FieldPrefix = "ASSETTYPE"

                _code = dataReader.GetInt16(GetFieldName("TYPE")).ToString()
                _name = dataReader.GetString(GetFieldName("DESCRIPTION"))
                _canBeUsedAsPreview = dataReader.GetBoolean(GetFieldName("CANBEUSEDASPREVIEW"))
            End With
        End Sub
#End Region

    End Class
End NameSpace