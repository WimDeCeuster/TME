Namespace Security.OldPermissions
    <Serializable()>
    Public Class CorePermissions
        Private ReadOnly _bodyTypes As Security.Interface.IPermissions
        Private ReadOnly _engines As EnginePermissions
        Private ReadOnly _engineTypes As Security.Interface.IPermissions
        Private ReadOnly _factoryGrades As Security.Interface.IPermissions
        Private ReadOnly _grades As Security.Interface.IPermissions
        Private ReadOnly _gradeClassifications As Security.Interface.IPermissions
        Private ReadOnly _subModels As Security.Interface.IPermissions
        Private ReadOnly _transmissions As TransmissionPermissions
        Private ReadOnly _wheelDrives As WheelDrivePermissions

        Public ReadOnly Property BodyTypes() As Security.Interface.IPermissions
            Get
                Return _bodyTypes
            End Get
        End Property

        Public ReadOnly Property Engines() As EnginePermissions
            Get
                Return _engines
            End Get
        End Property

        Public ReadOnly Property EngineTypes() As Security.Interface.IPermissions
            Get
                Return _engineTypes
            End Get
        End Property

        Public ReadOnly Property FactoryGrades() As Security.Interface.IPermissions
            Get
                Return _factoryGrades
            End Get
        End Property

        Public ReadOnly Property Grades() As Security.Interface.IPermissions
            Get
                Return _grades
            End Get
        End Property
        Public ReadOnly Property GradeClassifications() As Security.Interface.IPermissions
            Get
                Return _gradeClassifications
            End Get
        End Property


        Public ReadOnly Property SubModels() As Security.Interface.IPermissions
            Get
                Return _subModels
            End Get
        End Property

        Public ReadOnly Property Transmissions() As TransmissionPermissions
            Get
                Return _transmissions
            End Get
        End Property

        Public ReadOnly Property WheelDrives() As WheelDrivePermissions
            Get
                Return _wheelDrives
            End Get
        End Property

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            _bodyTypes = New BodyTypePermissions(context)
            _engines = New EnginePermissions(context)
            _engineTypes = New EngineTypePermissions(context)
            _factoryGrades = New FactoryGradePermissions(context)
            _grades = New GradePermissions(context)
            _gradeClassifications = New GradeClassificationPermissions(context)
            _subModels = New SubModelPermissions(context)
            _transmissions = New TransmissionPermissions(context)
            _wheelDrives = New WheelDrivePermissions(context)
        End Sub
#End Region

    End Class
End Namespace