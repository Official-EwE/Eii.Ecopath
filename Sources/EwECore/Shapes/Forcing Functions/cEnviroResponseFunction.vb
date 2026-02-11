' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cEnviroResponseFunction
    Inherits cMediationBaseFunction

    Friend Sub New(EcoSimData As cEcosimDatastructures, Manager As cBaseShapeManager,
                   data As cMediationDataStructures, DBID As Integer, DataType As eDataTypes)
        MyBase.New(EcoSimData, Manager, data, DBID, DataType)

    End Sub

    Public Overrides Function Update() As Boolean
        MyBase.Update()

        'do not update during initialization
        If Me.m_bInInit Then
            Return False
        End If

        'tell the manager that a shape has changed its data
        Me.ShapeChanged()
        Return True

    End Function

#Region " Response function "

    ''' <summary>
    ''' Minimum value of the input map that the response will be computed for. 
    ''' All values less than this will return the first value of the response function. 
    ''' </summary>
    ''' <remarks>
    ''' Left margin of the X Axis considered to be inbounds of the response function.
    ''' Updates <see cref="cMediationDataStructures.XAxisMin">cMediationDataStructures.XAxisMin</see>
    ''' </remarks>
    Public Property ResponseLeftLimit() As Single
        Get
            Return Me.m_medData.XAxisMin(Me.Index)
        End Get
        Set(value As Single)
            Me.m_medData.XAxisMin(Me.Index) = value
            'tell the manager that a shape has changed its data
            Me.ShapeChanged()
        End Set

    End Property

    ''' <summary>
    ''' Get/set the units of the response function. These will be inert in the model,
    ''' units only serve to advice the user on response application.
    ''' </summary>
    Public Property Units() As String
        Get
            Return Me.m_medData.Units(Me.Index)
        End Get
        Set(value As String)
            Me.m_medData.Units(Me.Index) = value
        End Set
    End Property

    ''' <summary>
    ''' Maximum value of the input map that the response will be computed for. 
    ''' All values greater than this will return the last value of the response function. 
    ''' </summary>
    ''' <remarks>
    ''' Right margin of the X Axis considered to be inbounds of the response function. 
    ''' Updates <see cref="cMediationDataStructures.XAxisMax">cMediationDataStructures.XAxisMax</see>
    ''' </remarks>
    Public Property ResponseRightLimit() As Single
        Get
            Return Me.m_medData.XAxisMax(Me.Index)
        End Get
        Set(value As Single)
            Me.m_medData.XAxisMax(Me.Index) = value
            'tell the manager that a shape has changed its data
            Me.ShapeChanged()
        End Set

    End Property

    Public ReadOnly Property ResponseMean() As Single
        Get
            Return (Me.m_medData.XAxisMin(Me.Index) + Me.m_medData.XAxisMax(Me.Index)) * 0.5F
        End Get
    End Property

#End Region ' Response function

#Region "Groups and Fleets interfaces not used by a cEnviroResponseFunction "

    Public Overrides Function AddFleet(iFleet As Integer, weight As Single) As Boolean
        Debug.Assert(False, "Not implemented by cEnviroResponseFunction.")
        Return False
    End Function

    Public Overrides Function AddGroup(iGroup As Integer, weight As Single, Optional iFleetIndex As Integer = -9999) As Boolean
        Debug.Assert(False, "Not implemented by cEnviroResponseFunction.")
        Return False
    End Function

    Public Overrides Property Fleet(iIndex As Integer) As cMediatingFleet
        Get
            Debug.Assert(False, "Not implemented by cEnviroResponseFunction.")
            Return Nothing
        End Get
        Set(value As cMediatingFleet)
            Debug.Assert(False, "Not implemented by cEnviroResponseFunction.")
        End Set
    End Property

    Public Overrides Property Group(iIndex As Integer) As cMediatingGroup
        Get
            Debug.Assert(False, "Not implemented by cEnviroResponseFunction.")
            Return Nothing
        End Get
        Set(value As cMediatingGroup)
            Debug.Assert(False, "Not implemented by cEnviroResponseFunction.")
        End Set
    End Property

    Public Overrides ReadOnly Property NumFleet() As Integer
        Get
            'Debug.Assert(False, "Not implemented by cEnviroResponseFunction.")
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property NumGroups() As Integer
        Get
            'Debug.Assert(False, "Not implemented by cEnviroResponseFunction.")
            Return 0
        End Get
    End Property

#End Region

End Class ' Response Function

