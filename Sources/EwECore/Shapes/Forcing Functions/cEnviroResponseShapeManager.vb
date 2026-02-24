' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Implemenation of the Base class for capacity shapes
''' </summary>
Public Class cEnviroResponseShapeManager
    Inherits cBaseShapeManager

    Private m_medData As cMediationDataStructures
    Private m_spaceData As cEcospaceDataStructures

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, SpaceData As cEcospaceDataStructures, ByRef theCore As cCore, DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)

        Me.m_spaceData = SpaceData
        Me.Init()

    End Sub

    Public Overrides ReadOnly Property NPoints() As Integer
        Get
            Return Me.m_medData.NMedPoints
        End Get
    End Property

    ''' <summary>
    ''' Create a new Mediation shape
    ''' </summary>
    Public Overrides Function CreateNewShape(strName As String, asData As Single(),
                                             Optional shapeType As Long = eShapeFunctionType.NotSet,
                                             Optional params() As Single = Nothing) As cForcingFunction

        Dim dbID As Integer

        If Me.m_core.AddShape(strName, Me.m_DataType, dbID, asData, shapeType, params) Then

            Dim medFunct As cEnviroResponseFunction

            'create a new shape that is hooked up to the underlying ecosim data
            medFunct = New cEnviroResponseFunction(Me.m_SimData, Me, Me.m_medData, dbID, Me.m_DataType)
            medFunct.ID = Me.m_shapes.Count
            medFunct.Load()

            'Add the new shape to the list 
            MyBase.Add(medFunct)

            Me.m_core.onChanged(Me, eMessageType.DataAddedOrRemoved)

            Return medFunct

        End If

        Return Nothing

    End Function

    Friend Overrides Function Init() As Boolean
        Dim medFunct As cEnviroResponseFunction

        'get the Enviromental response function for Capacity 
        Me.m_medData = Me.m_SimData.CapEnvResData

        'clear out any existing data
        Me.m_shapes.Clear()

        For imed As Integer = 1 To Me.m_medData.MediationShapes
            'All mediation shapes from the core will have an object 
            medFunct = New cEnviroResponseFunction(Me.m_SimData, Me, Me.m_medData, Me.m_medData.MediationDBIDs(imed), Me.m_DataType)
            medFunct.ID = Me.m_shapes.Count
            medFunct.Load()
            Me.m_shapes.Add(medFunct)

        Next imed
        Me.Load()

    End Function

End Class

