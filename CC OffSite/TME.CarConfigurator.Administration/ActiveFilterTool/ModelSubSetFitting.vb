Namespace ActiveFilterTool
    <Serializable()> Friend NotInheritable Class ModelSubSetFittings
        Inherits BaseObjects.ContextReadOnlyBase(Of ModelSubSetFittings)

#Region " Business Properties & Methods "
        Private _compatibilites As ModelSubSetCompatibilites = Nothing
        Private _incompatibilites As ModelSubSetInCompatibilites = Nothing

        Public ReadOnly Property Compatibilites() As ModelSubSetCompatibilites
            Get
                Return _compatibilites
            End Get
        End Property
        Public ReadOnly Property Incompatibilites() As ModelSubSetInCompatibilites
            Get
                Return _incompatibilites
            End Get
        End Property

        Protected Overrides Function GetIdValue() As Object
            Return Me.Compatibilites.ModelSubSet.ID
        End Function
#End Region

#Region " Shared Factory Methods "

        Friend Shared Function GetModelSubSetFittings(ByVal modelSubSet As ModelSubSet) As ModelSubSetFittings
            Dim _fittings As ModelSubSetFittings = DataPortal.Fetch(Of ModelSubSetFittings)(New CustomCriteria(modelSubSet))
            _fittings.Compatibilites.ModelSubSet = modelSubSet
            _fittings.Incompatibilites.ModelSubSet = modelSubSet
            Return _fittings
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            'Allow data portal to create us
        End Sub
#End Region

#Region " Criteria "
        <Serializable()> Private Class CustomCriteria
            Inherits CommandCriteria

            'Add Data Portal criteria here
            Private ReadOnly _modelSubSetID As Guid
            Public Sub New(ByVal modelSubSet As ModelSubSet)
                _modelSubSetID = modelSubSet.ID
            End Sub
            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@MODELSUBSETID", Me._modelSubSetID)
            End Sub
        End Class
#End Region

#Region " Data Access "

        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            If _compatibilites Is Nothing Then _compatibilites = ModelSubSetCompatibilites.prepareModelSubSetCompatibilites()
            If _incompatibilites Is Nothing Then _incompatibilites = ModelSubSetInCompatibilites.prepareModelSubSetInCompatibilites()

            'add current record
            Add(dataReader)

            'add next records
            While (dataReader.Read())
                Add(dataReader)
            End While

        End Sub
        Private Sub Add(ByVal dataReader As Common.Database.SafeDataReader)
            If dataReader.GetBoolean("NONFIT") Then
                _incompatibilites.Add(dataReader)
            Else
                _compatibilites.Add(dataReader)
            End If
        End Sub
#End Region

    End Class
    <Serializable()> Public NotInheritable Class ModelSubSetCompatibilites
        Inherits BaseObjects.ContextUniqueGuidListBase(Of ModelSubSetCompatibilites, ModelSubSetCompatibility)

#Region " Business Properties & Methods "
        Private _modelSubSet As ModelSubSet
        Friend Property ModelSubSet() As ModelSubSet
            Get
                Return _modelSubSet
            End Get
            Set(ByVal value As ModelSubSet)
                _modelSubSet = Value
                For Each _fitting As ModelSubSetFitting In Me
                    _fitting.CarSpecification.SetModelInfo(Value.Model)
                Next
            End Set
        End Property

        Friend Overloads Function Add(ByVal dataReader As SafeDataReader) As ModelSubSetCompatibility
            Dim _fitting As ModelSubSetCompatibility = ModelSubSetCompatibility.GetModelSubSetCompatibility(dataReader)
            MyBase.Add(_fitting)
            Return _fitting
        End Function

#End Region

#Region " Shared Factory Methods "

        Friend Shared Function PrepareModelSubSetCompatibilites() As ModelSubSetCompatibilites
            Return New ModelSubSetCompatibilites
        End Function
        Friend Shared Function NewModelSubSetCompatibilites(ByVal modelSubSet As ModelSubSet) As ModelSubSetCompatibilites
            Dim _fittings As ModelSubSetCompatibilites = New ModelSubSetCompatibilites
            _fittings.ModelSubSet = modelSubSet
            Return _fittings
        End Function


#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            'Allow data portal to create us
            MarkAsChild()
        End Sub
#End Region

#Region " Data Access "

        Friend Overloads Sub Update(ByVal transaction As SqlTransaction)
            'If there is a second ftting, then we should remove the all cars fitting
            If Me.Count > 1 Then
                For i As Integer = (Me.Count - 1) To 0 Step -1
                    If Me(i).CarSpecification.IsEmpty() Then
                        Me.RemoveAt(i)
                        Exit For
                    End If
                Next
            ElseIf Me.Count = 0 Then
                Me.Add() 'Add a new "all cars" selection if no compatbility was found
            End If

            MyBase.Update(transaction)
        End Sub

#End Region

    End Class
    <Serializable()> Public NotInheritable Class ModelSubSetInCompatibilites
        Inherits BaseObjects.ContextUniqueGuidListBase(Of ModelSubSetInCompatibilites, ModelSubSetInCompatibility)

#Region " Business Properties & Methods "
        Private _modelSubSet As ModelSubSet
        Friend Property ModelSubSet() As ModelSubSet
            Get
                Return _modelSubSet
            End Get
            Set(ByVal value As ModelSubSet)
                _modelSubSet = Value
                For Each _fitting As ModelSubSetFitting In Me
                    _fitting.CarSpecification.SetModelInfo(Value.Model)
                Next
            End Set
        End Property

        Friend Overloads Function Add(ByVal dataReader As SafeDataReader) As ModelSubSetInCompatibility
            Dim _fitting As ModelSubSetInCompatibility = ModelSubSetInCompatibility.GetModelSubSetInCompatibility(dataReader)
            MyBase.Add(_fitting)
            Return _fitting
        End Function
#End Region

#Region " Shared Factory Methods "

        Friend Shared Function PrepareModelSubSetInCompatibilites() As ModelSubSetInCompatibilites
            Return New ModelSubSetInCompatibilites
        End Function
        Friend Shared Function NewModelSubSetInCompatibilites(ByVal modelSubSet As ModelSubSet) As ModelSubSetInCompatibilites
            Dim _fittings As ModelSubSetInCompatibilites = New ModelSubSetInCompatibilites
            _fittings.ModelSubSet = modelSubSet
            Return _fittings
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            MarkAsChild()
        End Sub
#End Region

    End Class

    <Serializable()> Public MustInherit Class ModelSubSetFitting
        Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of ModelSubSetFitting)

#Region " Business Properties & Methods "

        ' Declare variables to contain object state
        ' Declare variables for any child collections
        Private _carSpecification As ModelSubSetCarSpecification

        Private ReadOnly Property ModelSubSet() As ModelSubSet
            Get
                If TypeOf (Me.Parent) Is ModelSubSetCompatibilites Then
                    Return DirectCast(Me.Parent, ModelSubSetCompatibilites).ModelSubSet
                Else
                    Return DirectCast(Me.Parent, ModelSubSetInCompatibilites).ModelSubSet
                End If
            End Get
        End Property

        Public ReadOnly Property CarSpecification() As ModelSubSetCarSpecification
            Get
                If _carSpecification Is Nothing Then
                    _carSpecification = ModelSubSetCarSpecification.NewModelSubSetCarSpecification(Me.ModelSubSet)
                End If
                Return _carSpecification
            End Get
        End Property
        Friend MustOverride ReadOnly Property NonFit() As Boolean

#End Region

#Region " System.Object Overrides "

        Public Overloads Overrides Function ToString() As String
            Return Me.CarSpecification.ToString()
        End Function

#End Region

#Region " Constructors "
        Protected Sub New()
            'Prevent direct creation
            Me.MarkAsChild()
            Me.AutoDiscover = False
            Me.AllowEdit = False
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            _carSpecification = ModelSubSetCarSpecification.GetModelSubSetCarSpecification(dataReader)
        End Sub
        Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.CommandText = "insertModelSubSetFitting"
            command.Parameters.AddWithValue("@MODELSUBSETID", Me.ModelSubSet.ID)
            Me.CarSpecification.AppendParameters(command)
            command.Parameters.AddWithValue("@NONFIT", Me.NonFit)
        End Sub
        Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.CommandText = "deleteModelSubSetFitting"
        End Sub

#End Region

    End Class
    <Serializable()> Public NotInheritable Class ModelSubSetCompatibility
        Inherits ModelSubSetFitting

#Region " Shared Factory Methods "
        Friend Shared Function GetModelSubSetCompatibility(ByVal dataReader As SafeDataReader) As ModelSubSetCompatibility
            Dim _fitting As ModelSubSetCompatibility = New ModelSubSetCompatibility
            _fitting.Fetch(dataReader)
            Return _fitting
        End Function
#End Region

        Friend Overrides ReadOnly Property NonFit() As Boolean
            Get
                Return False
            End Get
        End Property
    End Class
    <Serializable()> Public NotInheritable Class ModelSubSetInCompatibility
        Inherits ModelSubSetFitting

#Region " Shared Factory Methods "
        Friend Shared Function GetModelSubSetInCompatibility(ByVal dataReader As SafeDataReader) As ModelSubSetInCompatibility
            Dim _fitting As ModelSubSetInCompatibility = New ModelSubSetInCompatibility
            _fitting.Fetch(dataReader)
            Return _fitting
        End Function
#End Region

        Friend Overrides ReadOnly Property NonFit() As Boolean
            Get
                Return True
            End Get
        End Property
    End Class
End Namespace