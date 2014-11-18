Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

Namespace Configurations
    <Serializable()> Public NotInheritable Class ConfigurationTypes
        Inherits ContextReadOnlyListBase(Of ConfigurationTypes, ConfigurationType)


#Region " Shared Factory Methods "
        Public Shared Function GetConfigurationTypes() As ConfigurationTypes
            Return DataPortal.Fetch(Of ConfigurationTypes)(New Criteria)
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchNextResult(ByVal dataReader As SafeDataReader)
            While dataReader.Read()
                Item(dataReader.GetString("CONFIGURATIONTYPECODE")).AddSupportedEntity(dataReader)
            End While
        End Sub
#End Region

    End Class
    <Serializable()> Public NotInheritable Class ConfigurationType
        Inherits ContextReadOnlyBase(Of ConfigurationType)

#Region " Business Properties & Methods "

        Private _code As String
        Private _description As String
        Private ReadOnly _supportedEntities As List(Of Entity) = New List(Of Entity)()
        Private _differentConfigurationPerExteriorColour As Boolean

        Public ReadOnly Property Code() As String
            Get
                Return _code
            End Get
        End Property
        Public ReadOnly Property Description() As String
            Get
                Return _description
            End Get
        End Property
        Public ReadOnly Property SupportedEntities() As IEnumerable(Of Entity)
            Get
                Return _supportedEntities
            End Get
        End Property
        Public ReadOnly Property DifferentConfigurationPerExteriorColour As Boolean
            Get
                Return _differentConfigurationPerExteriorColour
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
            Return Description
        End Function

        Public Overrides Function Equals(ByVal obj As String) As Boolean
            Return Code.Equals(obj, StringComparison.InvariantCultureIgnoreCase)
        End Function

#End Region

#Region " Shared Factory Methods "
        Public Shared Function GetConfigurationType(ByVal code As String) As ConfigurationType
            Return ConfigurationTypes.GetConfigurationTypes()(code)
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            _code = dataReader.GetString("CODE")
            _description = dataReader.GetString("DESCRIPTION")
            _differentConfigurationPerExteriorColour = dataReader.GetBoolean("DIFFERENTCONFIGURATIONPEREXTERIORCOLOUR")
        End Sub
        Friend Sub AddSupportedEntity(ByVal dataReader As SafeDataReader)
            _supportedEntities.Add(dataReader.GetEntity("ENTITY"))
        End Sub
#End Region

    End Class
End Namespace
