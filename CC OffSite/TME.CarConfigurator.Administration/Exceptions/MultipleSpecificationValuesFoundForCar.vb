Imports System.Collections.Generic

Namespace Exceptions
    <Serializable()> Public Class MultipleSpecificationValuesFoundForCar
        Inherits Exception

        Private _car As Car
        Private _specification As ModelGenerationSpecification
        Private _values As IEnumerable(Of ModelGenerationSpecificationValue)

        Public Property Car() As Car
            Get
                Return _car
            End Get
            Private Set(ByVal value As Car)
                _car = value
            End Set
        End Property
        Public Property Specification() As ModelGenerationSpecification
            Get
                Return _specification
            End Get
            Private Set(ByVal value As ModelGenerationSpecification)
                _specification = value
            End Set
        End Property
        Public Property Values() As IEnumerable(Of ModelGenerationSpecificationValue)
            Get
                Return _values
            End Get
            Private Set(ByVal value As IEnumerable(Of ModelGenerationSpecificationValue))
                _values = value
            End Set
        End Property
        
        Public Sub New(ByVal car As Car, ByVal specification As ModelGenerationSpecification, ByVal values As IEnumerable(Of ModelGenerationSpecificationValue))
            MyBase.New(String.Format("Multiple '{0}' values were found for the vehicle '{1}'", specification, car))
            Me.Car = car
            Me.Specification = specification
            Me.Values = values
        End Sub

        Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class
End Namespace


