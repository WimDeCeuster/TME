'<Serializable()> Public NotInheritable Class SpecificationCodes
'    Inherits TME.BusinessObjects.ReadOnlyListBase(Of SpecificationCodes, SpecificationCode)

'    Default Public Shadows ReadOnly Property Item(ByVal id As Integer) As SpecificationCode
'        Get
'            Return (From obj In Me Where obj.ID.Equals(id) Select obj).FirstOrDefault()
'        End Get
'    End Property

'#Region " Shared Factory Methods "
'    Private Shared _codes As SpecificationCodes = Nothing
'    Private Shared _timeStamp As DateTime = DateTime.MinValue

'    Public Shared Function GetSpecificationCodes() As SpecificationCodes
'        If _codes Is Nothing OrElse _timeStamp > DateTime.Now.AddHours(1) Then
'            _codes = DataPortal.Fetch(Of SpecificationCodes)()
'            _timeStamp = DateTime.Now
'        End If
'        Return _codes
'    End Function
'#End Region

'#Region " Constructors "
'    Private Sub New()
'        'Prevent direct creation
'    End Sub
'#End Region

'#Region " Data Access "
'    <RunLocal()> Private Overloads Sub DataPortal_Fetch()
'        Dim _gateway As DataHub.Gateway = New DataHub.Gateway()
'        Me.IsReadOnly = False
'        For Each _code As DataHub.TechnicalSpecification In _gateway.GetTechnicalSpecificationList()
'            Me.Add(SpecificationCode.GetSpecificationCode(_code))
'        Next
'        Me.IsReadOnly = True
'    End Sub
'#End Region

'End Class
'<Serializable()> Public NotInheritable Class SpecificationCode
'  Inherits TME.BusinessObjects.ReadOnlyBase(Of SpecificationCode)

'#Region " Business Properties & Methods "
'  Private _id As Integer
'  Private _name As String
'  Private _unit As String
'  Private _type As String

'  <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Integer
'    Get
'      Return _id
'    End Get
'  End Property
'  <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
'    Get
'      Return _name
'    End Get
'  End Property
'  <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Unit() As String
'    Get
'      Return _unit
'    End Get
'  End Property
'  <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Type() As String
'    Get
'      Return _type
'    End Get
'  End Property

'#End Region

'#Region " System.Object Overrides "
'  Protected Overrides Function GetIdValue() As Object
'    Return Me.ID
'  End Function
'  Public Overloads Overrides Function ToString() As String
'    Return Me.Name.ToString()
'  End Function
'#End Region

'#Region " Shared Factory Methods "
'    Friend Shared Function GetSpecificationCode(ByVal specification As TME.CarConfigurator.DataHub.TechnicalSpecification) As SpecificationCode
'        Dim _specificationCode As SpecificationCode = New SpecificationCode()
'        _specificationCode.Fetch(specification)
'        Return _specificationCode
'    End Function
'  Public Shared Function GetSpecificationCode(ByVal code As Integer) As SpecificationCode
'    Return SpecificationCodes.GetSpecificationCodes()(code)
'  End Function
'#End Region

'#Region " Constructors "
'  Private Sub New()
'    'Prevent direct creation
'  End Sub
'#End Region

'#Region " Data Access "

'    Private Sub Fetch(ByVal specification As DataHub.TechnicalSpecification)
'        _id = specification.ID
'        '_name = specification.Description
'        _name = specification.Path
'        _unit = specification.Unit.Name
'        'If specification.SubType.Length > 0 Then
'        '    _type = String.Format("{0} - {1}", specification.Name, specification.SubType)
'        'Else
'        _type = specification.Name
'        'End If
'    End Sub

'#End Region

'End Class