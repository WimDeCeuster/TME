Imports TME.CarConfigurator.Administration.Enums

Namespace Assets

    <Serializable()> Public NotInheritable Class AssetTypeGroups
        Inherits ContextReadOnlyListBase(Of AssetTypeGroups, AssetTypeGroup)

#Region " Business Properties & Methods "

        Public Function FindType(ByVal code As String) As AssetType
            For Each group As AssetTypeGroup In Me
                Dim type As AssetType = group.Types(code)
                If Not type Is Nothing Then Return type
            Next
            Return Nothing
        End Function

#End Region

#Region " Shared Factory Methods "

        Friend Shared Function GetAssetTypeGroups() As AssetTypeGroups
            Return DataPortal.Fetch(Of AssetTypeGroups)(New CustomCriteria(Entity.NOTHING))
        End Function
        Friend Shared Function GetAssetTypeGroups(ByVal entity As Entity) As AssetTypeGroups
            Return DataPortal.Fetch(Of AssetTypeGroups)(New CustomCriteria(entity))
        End Function
        Public Shared Function GetAssetTypeGroups(ByVal generation As ModelGeneration, ByVal entity As Entity) As AssetTypeGroups

            Dim groups As AssetTypeGroups = DataPortal.Fetch(Of AssetTypeGroups)(New CustomCriteria(entity))
            groups.IsReadOnly = False
            For i As Integer = groups.Count - 1 To 0 Step -1
                Dim group As AssetTypeGroup = groups(i)
                group.Types.RemoveNonExistingTypes(generation)
                If group.Types.Count = 0 Then groups.RemoveAt(i)
            Next
            groups.IsReadOnly = True
            Return groups

        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Criteria "
        <Serializable()> Private Class CustomCriteria
            Inherits CommandCriteria

            Private ReadOnly _entity As String = String.Empty

            Public Sub New(ByVal entity As Entity)
                If Not entity = entity.NOTHING Then
                    _entity = entity.ToString
                End If
            End Sub

            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                If _entity.Equals(String.Empty) Then
                    command.Parameters.AddWithValue("@ENTITY", DBNull.Value)
                Else
                    command.Parameters.AddWithValue("@ENTITY", _entity)
                End If
            End Sub

        End Class
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
            FetchRoles(dataReader)
            If dataReader.NextResult() Then FetchTypes(dataReader)

            IsReadOnly = False
            For iIndex As Integer = Count - 1 To 0 Step -1
                Dim group As AssetTypeGroup = Me(iIndex)
                If group.Types.Count = 0 OrElse (group.Roles.Count > 0 AndAlso Not IsInRole(group)) Then
                    RemoveAt(iIndex)
                End If
            Next
            IsReadOnly = True
        End Sub

        Private Sub FetchRoles(ByVal dataReader As SafeDataReader)
            If Count <= 0 Then Return

            Dim currentGroup As AssetTypeGroup = Me(0)
            While dataReader.Read()
                Dim code As String = dataReader.GetInt16("GROUPCODE").ToString()
                If currentGroup Is Nothing OrElse Not currentGroup.Equals(code) Then currentGroup = Me(code)
                If Not currentGroup Is Nothing Then
                    currentGroup.Roles.Add(dataReader)
                End If
            End While
        End Sub
        Private Sub FetchTypes(ByVal dataReader As SafeDataReader)
            If Count <= 0 Then Return

            Dim currentGroup As AssetTypeGroup = Me(0)
            While dataReader.Read()
                Dim code As String = dataReader.GetInt16("GROUPCODE").ToString()
                If currentGroup Is Nothing OrElse Not currentGroup.Equals(code) Then currentGroup = Me(code)
                If Not currentGroup Is Nothing Then
                    currentGroup.Types.Add(dataReader)
                End If
            End While
        End Sub

        Private Shared Function IsInRole(ByVal group As AssetTypeGroup) As Boolean
            Return group.Roles.Any(Function(role) Thread.CurrentPrincipal.IsInRole(role.Name))
        End Function
#End Region

    End Class
    <Serializable()> Public NotInheritable Class AssetTypeGroup
        Inherits ContextReadOnlyBase(Of AssetTypeGroup)

#Region " Business Properties & Methods "
        Private _code As String
        Private _name As String
        Private _assetFilePath As String
        Private _supportsDeviations As Boolean
        Private _supportsPositioning As Boolean
        Private _usedByExteriorSpin As Boolean
        Private _usedByInteriorSpin As Boolean
        Private _usedByXRayHybridSpin As Boolean
        Private _usedByXRay4X4Spin As Boolean
        Private _usedByXRaySafetySpin As Boolean

        Private _mode As String
        Private _view As String
        Private _minimumAngle As Integer
        Private _maximumAngle As Integer

        Private _roles As AssetTypeGroupRoles
        Private _types As AssetTypes

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
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property AssetFilePath() As String
            Get
                Return _assetFilePath
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property SupportsDeviations() As Boolean
            Get
                Return _supportsDeviations
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property SupportsPositioning() As Boolean
            Get
                Return _supportsPositioning
            End Get
        End Property

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property UsedByExteriorSpin() As Boolean
            Get
                Return _usedByExteriorSpin
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property UsedByInteriorSpin() As Boolean
            Get
                Return _usedByInteriorSpin
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property UsedByXRayHybridSpin() As Boolean
            Get
                Return _usedByXRayHybridSpin
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property UsedByXRay4X4Spin() As Boolean
            Get
                Return _usedByXRay4x4Spin
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property UsedByXRaySafetySpin() As Boolean
            Get
                Return _usedByXRaySafetySpin
            End Get
        End Property

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Mode() As String
            Get
                Return _mode
            End Get
        End Property

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property View() As String
            Get
                Return _view
            End Get
        End Property

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property MaximumAngle() As Integer
            Get
                Return _maximumAngle
            End Get
        End Property

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property MinimumAngle() As Integer
            Get
                Return _minimumAngle
            End Get
        End Property

        Public ReadOnly Property Roles() As AssetTypeGroupRoles
            Get
                If _roles Is Nothing Then
                    _roles = AssetTypeGroupRoles.NewAssetTypeGroupRoles()
                End If
                Return _roles
            End Get
        End Property
        Public ReadOnly Property Types() As AssetTypes
            Get
                If _types Is Nothing Then
                    _types = AssetTypes.NewAssetTypes(Me)
                End If
                Return _types
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
            Return Name.ToString()
        End Function

        Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
            Return (String.Compare(Code, obj, True) = 0) OrElse (String.Compare(Name, obj, True) = 0)
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            _code = dataReader.GetInt16("CODE").ToString()
            _name = dataReader.GetString("DESCRIPTION")
            _assetFilePath = dataReader.GetString("ASSETFILEPATH")
            _supportsDeviations = dataReader.GetBoolean("SUPPORTSDEVIATIONS")
            _supportsPositioning = dataReader.GetBoolean("SUPPORTSPOSITIONING")
            _usedByExteriorSpin = dataReader.GetBoolean("EXTERIORSPIN")
            _usedByInteriorSpin = dataReader.GetBoolean("INTERIORSPIN")
            _usedByXRayHybridSpin = dataReader.GetBoolean("XRAYHYBRID")
            _usedByXRay4X4Spin = dataReader.GetBoolean("XRAY4X4")
            _usedByXRaySafetySpin = dataReader.GetBoolean("XRAYSAFETY")
            _mode = dataReader.GetString("MODE")
            _view = dataReader.GetString("VIEW")
            _minimumAngle = dataReader.GetInt16("MINANGLE")
            _maximumAngle = dataReader.GetInt16("MAXANGLE")

        End Sub
#End Region

    End Class

    <Serializable()> Public NotInheritable Class AssetTypeGroupRoles
        Inherits ContextReadOnlyListBase(Of AssetTypeGroupRoles, AssetTypeGroupRole)

#Region " Business Properties & Methods "

        Friend Overloads Sub Add(ByVal dataReader As SafeDataReader)
            RaiseListChangedEvents = False
            IsReadOnly = False
            Add(GetObject(dataReader))
            IsReadOnly = True
            RaiseListChangedEvents = True
        End Sub

#End Region

#Region " Shared Factory Methods "

        Public Shared Function NewAssetTypeGroupRoles() As AssetTypeGroupRoles
            Return New AssetTypeGroupRoles
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

    End Class
    <Serializable()> Public NotInheritable Class AssetTypeGroupRole
        Inherits ContextReadOnlyBase(Of AssetTypeGroupRole)

#Region " Business Properties & Methods "

        Private _name As String

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property

#End Region

#Region " Framework Overrides "
        Protected Overrides Function GetIdValue() As Object
            Return Name
        End Function
#End Region

#Region " System.Object Overrides "

        Public Overloads Overrides Function ToString() As String
            Return Name.ToString()
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            _name = dataReader.GetString("ROLE")
        End Sub
#End Region
    End Class
End Namespace