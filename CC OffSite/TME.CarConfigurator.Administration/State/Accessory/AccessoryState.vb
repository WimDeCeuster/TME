Imports TME.CarConfigurator.Administration.Exceptions

Namespace State.Accessory
    <Serializable> Public MustInherit Class AccessoryState
#Region "Properties & Methods"
        Private _dbValue As StateValue
        Protected Property Accessory() As Administration.Accessory
        Public Property DBValue() As StateValue
            Get
                Return _dbValue
            End Get
            Set(value As StateValue)
                _dbValue = value
            End Set
        End Property
        Public MustOverride Overrides Function ToString() As String
#End Region

#Region "State Transfers"
        Public Overridable Sub Publish()
            Accessory.State = New Published(Accessory)
        End Sub

        Public Overridable Sub Withdraw()
            Accessory.State = New Withdrawn(Accessory)
        End Sub

        Public Overridable Sub PhaseOut()
            Accessory.State = New PhasedOut(Accessory)
        End Sub
#End Region

#Region "Shared Factory Methods"
        Friend Shared Function NewState(ByVal accessory As Administration.Accessory) As AccessoryState
            If accessory.IsLocalizedInA2A() Then
                Return New Draft(accessory)
            Else
                Return New Published(accessory)
            End If
        End Function
        Friend Shared Function GetState(ByVal accessory As Administration.Accessory, ByVal dataReader As SafeDataReader) As AccessoryState
            Dim state As StateValue = DirectCast(dataReader.GetInt32("STATE"), StateValue)
            Select Case state
                Case StateValue.Draft
                    Return New Draft(accessory)
                Case StateValue.Published
                    Return New Published(accessory)
                Case StateValue.Withdrawn
                    Return New Withdrawn(accessory)
                Case StateValue.PhasedOut
                    Return New PhasedOut(accessory)
            End Select
            Throw New StateNotValidException(String.Format("State {0} is not supported.", state.ToString()))
        End Function
#End Region

#Region "Constructors"
        Protected Sub New(ByVal accessory As Administration.Accessory)
            Me.Accessory = accessory
        End Sub
#End Region

#Region "Enums"
        Public Enum StateValue
            Draft = 0
            Published = 1
            Withdrawn = 2
            PhasedOut = 4
        End Enum
#End Region
    End Class
End Namespace