
Imports System.Runtime.CompilerServices
Imports TME.CarConfigurator.Administration.Assets.Enums
Imports TME.CarConfigurator.Administration.Enums

Namespace Extensions
    Public Module DataReaderExtensions
        <Extension>
        Public Function GetEquipmentType(ByVal dataReader As Common.Database.SafeDataReader, ByVal fieldName As String) As EquipmentType
            Dim type As String = dataReader.GetString(fieldName)
            Select Case type
                Case Environment.DBAccessoryCode
                    Return EquipmentType.Accessory
                Case Environment.DBOptionCode
                    Return EquipmentType.Option
                Case Environment.DBExteriorColourTypeCode
                    Return EquipmentType.ExteriorColourType
                Case Environment.DBUpholsteryTypeCode
                    Return EquipmentType.UpholsteryType
                Case Else
                    Throw New Exceptions.InvalidEquipmentType("""" & type & """ is not a valid equipment type!")
            End Select
        End Function

        <Extension>
        Public Function GetEntity(ByVal dataReader As Common.Database.SafeDataReader, ByVal fieldName As String) As Entity
            Return DirectCast([Enum].Parse(GetType(Entity), dataReader.GetString(fieldName)), Entity)
        End Function

        <Extension>
        Public Function GetAvailability(ByVal dataReader As Common.Database.SafeDataReader, ByVal fieldName As String) As Availability
            Return CType(dataReader.GetInt16(fieldName), Availability)
        End Function

        <Extension>
        Public Function GetColouringModes(ByVal dataReader As Common.Database.SafeDataReader, ByVal fieldName As String) As ColouringModes
            Return CType(dataReader.GetInt16(fieldName), ColouringModes)
        End Function

        <Extension>
        Public Function GetAssetScope(ByVal dataReader As Common.Database.SafeDataReader, ByVal fieldName As String) As AssetScope
            Return CType(dataReader.GetInt16(fieldName), AssetScope)
        End Function

    End Module
End Namespace
