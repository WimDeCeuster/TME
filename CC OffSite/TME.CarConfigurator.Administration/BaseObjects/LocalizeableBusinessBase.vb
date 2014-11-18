Namespace BaseObjects
    <Serializable()> Public MustInherit Class LocalizeableBusinessBase
        Inherits TranslateableBusinessBase

#Region " Business Properties & Methods"
        Protected _localCode As String = String.Empty
        Protected _canHaveLocalCode As Boolean = True

        Protected Friend MustOverride Function GetBaseCode() As String
        <XmlInfo(XmlNodeType.None)> Public ReadOnly Property AlternateCode() As String
            ' this property is required because some controls bind directly or use it for sorting, thus no chance for evaluation
            Get
                If Me.LocalCode.Length = 0 Then
                    Return Me.BaseCode
                Else
                    Return Me.LocalCode
                End If
            End Get
        End Property
        <XmlInfo(XmlNodeType.None)> Public ReadOnly Property BaseCode() As String
            Get
                Return GetBaseCode()
            End Get
        End Property
        <XmlInfo(XmlNodeType.None)> Public ReadOnly Property SupportsLocalCode() As Boolean
            Get
                Return Me.CanHaveLocalCode AndAlso Not MyContext.GetContext().IsGlobal()
            End Get
        End Property
        <XmlInfo(XmlNodeType.None)> Protected Friend Property CanHaveLocalCode() As Boolean
            Get
                Return _canHaveLocalCode
            End Get
            Set(ByVal value As Boolean)
                _canHaveLocalCode = value
            End Set
        End Property
        Public Property LocalCode() As String
            Get
                Return _localCode
            End Get
            Set(ByVal value As String)
                If _localCode <> value Then
                    _localCode = value
                    PropertyHasChanged("LocalCode")
                End If
            End Set
        End Property
        Public Overloads Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
            If propertyName.Equals("LocalCode", StringComparison.InvariantCultureIgnoreCase) Then Return Me.SupportsLocalCode
            Return MyBase.CanWriteProperty(propertyName)
        End Function

        Friend Shadows Sub Copy(ByVal source As LocalizeableBusinessBase)
            _localCode = source.LocalCode
            _canHaveLocalCode = source.CanHaveLocalCode
            MyBase.Copy(source)
        End Sub

#End Region

#Region " Data Access "

        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            _localCode = dataReader.GetString(GetFieldName("LOCALCODE"))
            MyBase.FetchFields(dataReader)
        End Sub

#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
            If Me.SupportsLocalCode AndAlso String.Compare(Me.LocalCode, obj, True, Globalization.CultureInfo.InvariantCulture) = 0 Then Return True
            Return String.Compare(Me.GetBaseCode, obj, True, Globalization.CultureInfo.InvariantCulture) = 0
        End Function
#End Region

    End Class
End Namespace
