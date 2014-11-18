Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class ModelSubSetClassifications
        Inherits BaseObjects.ContextUniqueGuidListBase(Of ModelSubSetClassifications, ModelSubSetClassification)

#Region " Business Properties & Methods "
        Friend ReadOnly Property SubSet() As ModelSubSet
            Get
                Return DirectCast(Me.Parent, ModelSubSet)
            End Get
        End Property

        Public Overloads Function Add(ByVal classification As Classification) As ModelSubSetClassification
            If Me.Contains(classification) Then Throw New Exceptions.ObjectAlreadyExists("classification", classification)

            Dim _item As ModelSubSetClassification = ModelSubSetClassification.NewModelSubSetClassification(classification)
            MyBase.Add(_item)
            Return _item
        End Function
#End Region

#Region " Shared Factory Methods "
        Friend Shared Function NewModelSubSetClassifications(ByVal modelSubSet As ModelSubSet) As ModelSubSetClassifications
            Dim _classifications As ModelSubSetClassifications = New ModelSubSetClassifications()
            _classifications.SetParent(modelSubSet)
            Return _classifications
        End Function
        Friend Shared Function GetModelSubSetClassifications(ByVal modelSubSet As ModelSubSet) As ModelSubSetClassifications
            Dim _classifications As ModelSubSetClassifications = DataPortal.Fetch(Of ModelSubSetClassifications)(New CustomCriteria(modelSubSet))
            _classifications.SetParent(modelSubSet)
            Return _classifications
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

            'Add Data Portal criteria here
            Private ReadOnly _id As Guid

            Public Sub New(ByVal modelSubSet As ModelSubSet)
                _id = modelSubSet.ID
            End Sub
            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@MODELSUBSETID", _id)
            End Sub

        End Class
#End Region

    End Class
    <Serializable()> Public NotInheritable Class ModelSubSetClassification
        Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of ModelSubSetClassification)

#Region " Business Properties & Methods "
        ' Declare variables to contain object state
        ' Declare variables for any child collections
        Private _name As String

        ' Implement properties and methods for interaction of the UI,
        ' or any other client code, with the object
        Public ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property
        Friend ReadOnly Property SubSet() As ModelSubSet
            Get
                If Me.Parent Is Nothing Then Return Nothing
                Return DirectCast(Me.Parent, ModelSubSetClassifications).SubSet
            End Get
        End Property

#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return Me.Name
        End Function

        Public Overloads Function Equals(ByVal item As Classification) As Boolean
            Return Not (item Is Nothing) AndAlso Me.ID.Equals(item.ID)
        End Function
        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            If TypeOf obj Is ModelSubSetClassification Then
                Return Me.Equals(DirectCast(obj, ModelSubSetClassification))
            ElseIf TypeOf obj Is Classification Then
                Return Me.Equals(DirectCast(obj, Classification))
            ElseIf TypeOf obj Is String Then
                Return Me.Equals(DirectCast(obj, String))
            ElseIf TypeOf obj Is Guid Then
                Return Me.Equals(DirectCast(obj, Guid))
            Else
                Return False
            End If
        End Function

#End Region

#Region " Shared Factory Methods "
        Friend Shared Function NewModelSubSetClassification(ByVal classification As Classification) As ModelSubSetClassification
            Dim _classification As ModelSubSetClassification = New ModelSubSetClassification
            _classification.Create(classification)
            Return _classification
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            Me.AutoDiscover = False
        End Sub
#End Region

#Region " Data Access "
        Private Overloads Sub Create(ByVal classification As Classification)
            ID = classification.ID
            _name = classification.Name
            InitializeAuditFields()
        End Sub
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            _name = dataReader.GetString("NAME")
        End Sub
        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            Me.AddCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            Me.AddCommandFields(command)
        End Sub
        Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
            Me.AddCommandFields(command)
        End Sub
        Private Sub AddCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@MODELSUBSETID", Me.SubSet.ID)
        End Sub
#End Region


    End Class
End Namespace
