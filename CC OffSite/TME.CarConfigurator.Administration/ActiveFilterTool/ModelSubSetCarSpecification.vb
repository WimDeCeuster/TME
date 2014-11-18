Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class ModelSubSetCarSpecification
        Inherits PartialCarSpecification

#Region " Business Properties & Methods "
        Friend Sub SetModelInfo(ByVal modelInfo As ModelInfo)
            _modelID = modelInfo.ID
            _modelName = modelInfo.Name
        End Sub
#End Region

#Region " Shared Factory Methods "
        Friend Shared Function NewModelSubSetCarSpecification(ByVal modelSubSet As ModelSubSet) As ModelSubSetCarSpecification
            Dim _specification As ModelSubSetCarSpecification = New ModelSubSetCarSpecification
            _specification.SetModelInfo(modelSubSet.Model)
            Return _specification
        End Function
        Friend Shared Function GetModelSubSetCarSpecification(ByVal dataReader As SafeDataReader) As ModelSubSetCarSpecification
            Dim _specification As ModelSubSetCarSpecification = New ModelSubSetCarSpecification
            _specification.Fetch(dataReader)
            Return _specification
        End Function
#End Region

#Region " Base Class Overrides "

        Protected Overloads Sub Fetch(ByVal dataReader As SafeDataReader)

            With dataReader
                _bodyTypeID = .GetGuid("BODYID")
                _engineID = .GetGuid("ENGINEID")
                _gradeID = .GetGuid("GRADEID")

                If .FieldExists("BODYNAME") Then
                    _bodyTypeName = .GetString("BODYNAME")
                    _engineName = .GetString("ENGINENAME")
                    _gradeName = .GetString("GRADENAME")
                End If

            End With
            MarkOld()
        End Sub
        Protected Friend Overrides Sub AppendParameters(ByVal command As System.Data.SqlClient.SqlCommand)
            With command.Parameters
                .AddWithValue("@BODYID", Me.BodyTypeID)
                .AddWithValue("@ENGINEID", Me.EngineID)
                .AddWithValue("@GRADEID", Me.GradeID)
            End With
        End Sub

        Public Overloads Overrides Function IsEmpty() As Boolean
            If Not Me.BodyTypeID.Equals(Guid.Empty) Then Return False
            If Not Me.EngineID.Equals(Guid.Empty) Then Return False
            If Not Me.GradeID.Equals(Guid.Empty) Then Return False
            Return True
        End Function
        Public Overloads Overrides Function ToString() As String
            Dim _buffer As System.Text.StringBuilder = New System.Text.StringBuilder
            If Me.BodyTypeName.Length > 0 Then _buffer.Append(Me.BodyTypeName & ", ")
            If Me.EngineName.Length > 0 Then _buffer.Append(Me.EngineName & ", ")
            If Me.GradeName.Length > 0 Then _buffer.Append(Me.GradeName & ", ")

            Dim sBuffer As String = _buffer.ToString()
            If sBuffer.Length > 0 Then
                sBuffer = sBuffer.Remove(sBuffer.Length - 2, 2)
            Else
                sBuffer = "All cars"
            End If
            Return sBuffer

        End Function

#End Region

    End Class
End Namespace
