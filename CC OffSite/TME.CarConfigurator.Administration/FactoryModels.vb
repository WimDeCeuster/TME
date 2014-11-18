<Serializable()> Public NotInheritable Class FactoryModels
    Inherits ContextListBase(Of FactoryModels, FactoryModel)

#Region " Shared Factory Methods "

    'Calls the SP: getModelFactoryModels
    Friend Shared Function GetFactoryModels(ByVal model As Model) As FactoryModels
        Return DataPortal.Fetch(Of FactoryModels)(New CustomCriteria(model))
    End Function

#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _modelID As Guid

        Public Sub New(ByVal model As Model)
            CommandText = "getModelFactoryModels"
            _modelID = model.ID
        End Sub

        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@MODELID", _modelID)
        End Sub

    End Class
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowEdit = False
        AllowNew = False
        AllowRemove = False
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class FactoryModel
    Inherits ContextBusinessBase(Of FactoryModel)

#Region " Business Properties & Methods "
    Private _code As String
    Private _name As String
    Private _factoryGenerations As FactoryGenerations

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    <XmlInfo(XmlNodeType.None)> Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property

    Public ReadOnly Property FactoryGenerations() As FactoryGenerations
        Get
            If _factoryGenerations Is Nothing Then
                _factoryGenerations = FactoryGenerations.GetFactoryGenerations(Me)
            End If
            Return _factoryGenerations
        End Get
    End Property

    Public Function GetInfo() As FactoryModelInfo
        Return FactoryModelInfo.GetFactoryModelInfo(Me)
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Code.ToString()
    End Function

#End Region

#Region " Framework Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return Code
    End Function

    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If Not _factoryGenerations Is Nothing AndAlso _factoryGenerations.IsDirty Then Return True
            Return MyBase.IsDirty
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not _factoryGenerations Is Nothing AndAlso Not _factoryGenerations.IsValid Then Return False
            Return MyBase.IsValid
        End Get
    End Property
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowEdit = False
        AllowNew = False
        AllowRemove = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _code = dataReader.GetString(GetFieldName("CODE"))
        _name = dataReader.GetString(GetFieldName("NAME"))
    End Sub
    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not _factoryGenerations Is Nothing Then _factoryGenerations.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub
#End Region

End Class
<Serializable(), XmlInfo("factorymodel")> Public NotInheritable Class FactoryModelInfo

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _name As String = String.Empty

    Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return Code.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As FactoryModel) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.Code)
    End Function
    Public Overloads Function Equals(ByVal obj As FactoryModelInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.Code)
    End Function
    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return Code.Equals(obj, StringComparison.InvariantCultureIgnoreCase)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is FactoryModelInfo Then
            Return Equals(DirectCast(obj, FactoryModelInfo))
        ElseIf TypeOf obj Is FactoryModel Then
            Return Equals(DirectCast(obj, FactoryModel))
        ElseIf TypeOf obj Is String Then
            Return Equals(DirectCast(obj, String))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is FactoryModelInfo Then
            Return DirectCast(objA, FactoryModelInfo).Equals(objB)
        ElseIf TypeOf objB Is FactoryModelInfo Then
            Return DirectCast(objB, FactoryModelInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetFactoryModelInfo(ByVal factoryModel As FactoryModel) As FactoryModelInfo
        Dim info As FactoryModelInfo = New FactoryModelInfo
        info.Fetch(factoryModel)
        Return info
    End Function
    Friend Shared Function GetFactoryModelInfo(ByVal dataReader As SafeDataReader) As FactoryModelInfo
        Dim info As FactoryModelInfo = New FactoryModelInfo
        info.Fetch(dataReader)
        Return info
    End Function
    Public Shared ReadOnly Property Empty() As FactoryModelInfo
        Get
            Return New FactoryModelInfo
        End Get
    End Property

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Private Sub Fetch(ByVal factoryModel As FactoryModel)
        With factoryModel
            _code = .Code
            _name = .Name
        End With
    End Sub
    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _code = .GetString("FACTORYMODELCODE")
            _name = .GetString("FACTORYMODELNAME")
        End With
    End Sub
#End Region

End Class