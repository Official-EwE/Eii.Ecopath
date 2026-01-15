' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'


Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' <summary>
''' A fish s
''' </summary>
''' <remarks></remarks>
Public Class cFishingRateShape
    Inherits cForcingFunction

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cFishingRateShape)()


    Friend Sub New(EcoSimData As cEcosimDatastructures, Manager As cBaseShapeManager, DBID As Integer, strFleetName As String)

        MyBase.New(EcoSimData, Manager, DBID, eDataTypes.FishingEffort)

        Me.m_bInInit = True
        Me.DBID = DBID

        Me.m_data = EcoSimData
        'this can be changed to use the database id see load

        Me.Name = strFleetName ' the fleetname is only stored in the Ecopath Data so it has to be passed into the constructor

        'create a new forcing data object this only happens when the shape is created
        'if the shape is being initialized from the EcoSim the forcing object must already exist

        Me.Load()

        Me.m_bInInit = False

    End Sub


    ''' <summary>
    ''' Initialize the propeties from the underlying EcoSim data structures at the existing array index (iEcoSimIndex)
    ''' </summary>
    ''' <returns>True if successful</returns>
    ''' <remarks>This seperates creation from initialization so that an existing object can be repopluated from its underlying data</remarks>
    Protected Friend Overrides Function Load() As Boolean

        'copy the Fishing rate data into an array that will be used to create a forcing data object
        Me.m_bInInit = True
        Dim m_ntimesteps As Integer = Me.m_data.NTimes

        Debug.Assert(Me.m_iEcoSimIndex > -1, Me.ToString & " database ID invalid.")
        If Me.m_iEcoSimIndex = -1 Then Return False

        Me.ResizeData(m_ntimesteps)

        For ipt As Integer = 1 To m_ntimesteps
            Me.ShapeData(ipt) = Me.m_data.FishRateGear(Me.m_iEcoSimIndex, ipt) 'FishRateGear(NFleets,nTime)
        Next ipt

        Me.m_nYears = Me.m_data.NumYears

        Me.m_bInInit = False

        Return True

    End Function

    ''' <summary>
    ''' Update the underlying EcoSim data structures
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Update_Org() As Boolean

        'do not update during initialization
        If Me.m_bInInit Then
            Return False
        End If

        'can not update if there is not an index to the underlying data structures
        If (Me.m_iEcoSimIndex = cCore.NULL_VALUE) Or (Me.m_iEcoSimIndex > Me.m_data.nGear + 1) Then
            m_logger.LogError(Me.ToString & ".update(m_data) index out of bounds. Data not updated.")
            Return False
        End If

        'make sure the shape data is the same size as the EcoSim Shape data
        'this is a double check as the data size was check when the forcing function was added to the Shape Manager
        'however it could have been changed be an interface at a later date
        'jb Sept-06 this can not happen for this type of object because all time dimensioning is handled by the core
        'and a new object will be created by the core with the new number of time steps changes when the time changes
        'm_Xdata.ResizeData(m_ntimesteps)

        Dim orgvalue As Single, newvalue As Single
        ' Dim bhaschanged As Boolean
        Dim isCombinedFleets As Boolean

        If Me.m_iEcoSimIndex = Me.m_data.nGear + 1 Then
            isCombinedFleets = True
        End If

        Dim f(,) As Single = Me.m_manager.Core.m_TSData.ForcedFs

        Debug.Assert(Me.m_data.NTimes >= Me.EndEditPoint, Me.ToString & " Warning Shape data contain more timestep then Ecosim data!")
        'we have to loop over all the time steps because we don't know what has changed
        For it As Integer = Me.StartEditPoint To Me.EndEditPoint
            orgvalue = Me.m_data.FishRateGear(Me.m_iEcoSimIndex, it)
            newvalue = Me.ShapeData(it)

            'update FishRateGear() with the new values 
            Me.m_data.FishRateGear(Me.m_iEcoSimIndex, it) = Me.ShapeData(it)

            'this shape is the combined fleets type so update all the fleets types with the new value
            If isCombinedFleets Then
                For iFlt As Integer = 1 To Me.m_data.nGear
                    Me.m_data.FishRateGear(iFlt, it) = Me.m_data.FishRateGear(Me.m_iEcoSimIndex, it) 'dont worry about overwriting the fleet we just updated
                Next
            End If

            'FishRateGear() is a multiplier that is used to change fishing effort from the base Ecopath value for a fleet
            'Now use FishRateGear/effort to update the fishing mortality for each group fished by this fleet
            For igrp As Integer = 1 To Me.m_data.nGroups

                If f(igrp, it) < 0 Then

                    Debug.Assert(igrp <> 17)

                    'don't change the fishing mortality if there is fishing mortality timeseries loaded for this group
                    'JB 21-Feb-2011 Changed so Effort can be edited when F time series is loaded
                    If Not isCombinedFleets Then
                        Dim EcopathCatch As Single = Me.m_manager.Core.m_EcopathData.Landing(Me.m_iEcoSimIndex, igrp) + Me.m_manager.Core.m_EcopathData.Discard(Me.m_iEcoSimIndex, igrp)
                        If EcopathCatch > 0 Then

                            'this fleet exploits this group update the F
                            ' m_data.FishRateNo(igrp, it) = m_data.FishRateNo(igrp, it) + m_data.FishMGear(m_iEcoSimIndex, igrp) * (m_data.FishRateGear(m_iEcoSimIndex, it) - orgvalue)
                            Me.m_data.FishRateNo(igrp, it) = Me.m_data.FishMGear(Me.m_iEcoSimIndex, igrp) * Me.m_data.FishRateGear(Me.m_iEcoSimIndex, it)
                            If Me.m_data.FishRateNo(igrp, it) < 0 Then Me.m_data.FishRateNo(igrp, it) = 0
                        End If

                    Else
                        'combined fleet this changes all the mortality
                        Me.m_data.FishRateNo(igrp, it) = 0
                        For iflt As Integer = 1 To Me.m_data.nGear
                            Me.m_data.FishRateNo(igrp, it) = Me.m_data.FishRateNo(igrp, it) + Me.m_data.FishMGear(iflt, igrp) * Me.m_data.FishRateGear(iflt, it)
                            ' JS 3sept11: Fixed #1041
                            If Me.m_data.FishRateNo(igrp, it) < 0 Then Me.m_data.FishRateNo(igrp, it) = 0
                        Next iflt

                    End If ' Not isCombinedFleets
                End If 'If f(igrp, it) < 0 Then
            Next igrp
        Next it

        'tell the manager that a shape has changed its data
        Me.ShapeChanged()

        Return True

    End Function


    ''' <summary>
    ''' Update the underlying EcoSim data structures
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Update() As Boolean

        'do not update during initialization
        If Me.m_bInInit Then
            Return False
        End If

        'can not update if there is not an index to the underlying data structures
        If (Me.m_iEcoSimIndex = cCore.NULL_VALUE) Or (Me.m_iEcoSimIndex > Me.m_data.nGear + 1) Then
            m_logger.LogError(Me.ToString & ".update(m_data) index out of bounds. Data not updated.")
            Return False
        End If

        Dim isCombinedFleets As Boolean

        If Me.m_iEcoSimIndex = Me.m_data.nGear + 1 Then
            isCombinedFleets = True
        End If

        Debug.Assert(Me.m_data.NTimes >= Me.EndEditPoint, Me.ToString & " Warning Shape data contain more timestep then Ecosim data!")
        'we have to loop over all the time steps because we don't know what has changed
        For it As Integer = Me.StartEditPoint To Me.EndEditPoint

            'update FishRateGear() with the new values 
            Me.m_data.FishRateGear(Me.m_iEcoSimIndex, it) = Me.ShapeData(it)

            'this shape is the combined fleets type so update all the fleets types with the new value
            If isCombinedFleets Then
                For iFlt As Integer = 1 To Me.m_data.nGear
                    Me.m_data.FishRateGear(iFlt, it) = Me.m_data.FishRateGear(Me.m_iEcoSimIndex, it) 'dont worry about overwriting the fleet we just updated
                Next

            End If

            ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            ''Moved this to cCore.OnChanged()
            ''Let the core set fishing motality rates 
            ''from the updated Effort
            'Me.m_manager.Core.SetFtimeFromGear(it, q, True)
            ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        Next it

        'tell the manager that a shape has changed its data
        Me.ShapeChanged()

        Return True

    End Function


End Class
