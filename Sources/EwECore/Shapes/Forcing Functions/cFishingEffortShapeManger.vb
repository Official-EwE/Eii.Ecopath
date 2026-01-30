' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cFishingEffortShapeManger
    Inherits cFishingBaseShapeManager

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cFishingEffortShapeManger)()

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)
    End Sub

    ''' <summary>
    ''' Fishing rate shapes can not be dynamically created; they are part of the fleet setup.
    ''' </summary>
    ''' <returns>Always Nothing.</returns>
    Public Overrides Function CreateNewShape(strName As String, asData() As Single,
                                             Optional shapeType As Long = eShapeFunctionType.NotSet,
                                             Optional params As Single() = Nothing) As cForcingFunction
        Return Nothing
    End Function

    Friend Overrides Function Init() As Boolean
        Dim shape As cFishingRateShape = Nothing
        Dim iFleet As Integer

        'clear out any existing data
        Me.m_shapes.Clear()
        For iFleet = 1 To Me.m_SimData.nGear ' number of fishing fleets

            shape = New cFishingRateShape(Me.m_SimData, Me, Me.m_SimData.FishRateGearDBID(iFleet), Me.m_core.m_EcopathData.FleetName(iFleet))
            'keep the index of this forcing function in the list in the function itself
            'it will be used later to return the list item for a given EcoSim array index
            shape.ID = Me.m_shapes.Count
            shape.Index = iFleet
            shape.Load()
            Me.m_shapes.Add(shape)

        Next iFleet

        If Me.m_SimData.nGear > 0 Then
            'Add the Combined Gear types shape to the end of the list
            'Its iFleet index is m_Data.nGear + 1 
            'this is critical as that is how the shape itself decides that it the Combined Fleets shape
            'the Combined Fleets shape updates all the other fleets as well as the FishMort shapes
            shape = New cFishingRateShape(Me.m_SimData, Me, cCore.NULL_VALUE, My.Resources.CoreDefaults.CORE_ALL_FLEETS)
            shape.ID = Me.m_shapes.Count
            shape.Index = Me.m_SimData.nGear + 1
            shape.Load()
            Me.m_shapes.Add(shape)
        End If

        Me.Load()

    End Function

    Public Overrides Sub ResetToDefaults()
        Me.m_SimData.DefaultFishingRates()
        Me.Load()
        Me.ShapeChanged()
    End Sub

    Public Overrides Function EcopathBaseValue(iShape As Integer) As Single
        Return 1
    End Function

    Public Overrides Function Update(Optional bUpdateAll As Boolean = True) As Boolean

        If bUpdateAll Then
            Return MyBase.Update()

        Else
            Try
                'have each shape will update the underlying EcoSim data
                For Each shape As cForcingFunction In Me
                    'Don't update the All Fleets Shape because this will overwrite all the other shapes
                    'This fixes a bug in the MultiSim Plugin when driving Effort for a specific shape the All Fleets shape was overwriting the driven shape(s)
                    'see EwEMultiSimPlugin.cEngine.CommitShape()
                    If shape.Index <= Me.m_SimData.nGear Then
                        If Not shape.Update() Then
                            m_logger.LogError(".Update() Shape failed to update DBID=" & shape.DBID.ToString)
                            Debug.Assert(False, Me.ToString & ".Update() Shape failed to update DBID=" & shape.DBID.ToString)
                            'this will keep trying to update the rest of the data
                            'even if there was a problem with one of the shapes
                        End If
                    End If
                Next shape

                Return True
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".Update() Error: " & ex.Message)
            End Try
            'something went wrong
            Return False

        End If
    End Function

End Class

