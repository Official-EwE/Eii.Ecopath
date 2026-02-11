' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Data Adapter specific to Relative PP.
    ''' </summary>
    ''' <remarks>
    ''' Does not actually scale the data rather it sets <see cref="cEcospaceDataStructures.PPScale"/> 
    ''' which is used by <see cref="cSpaceSolver">cSpaceSolver.derivtRed</see> to scale RelPP.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cRelPPDataAdapter
        Inherits cSpatialScalarDataAdapterBase

#Region " Private vars "

        Private m_sPreservedScale As Double = cCore.NULL_VALUE
        Private m_spaceData As cEcospaceDataStructures

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(core As cCore, varName As eVarNameFlags, cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialScalarDataAdapter.Initialize"/>.
        ''' -------------------------------------------------------------------
        Public Overrides Sub Initialize()

            MyBase.Initialize()
            Me.m_spaceData = Me.m_core.m_EcospaceData

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataAdapter.InitRun"/>
        ''' <remarks>
        ''' Overridden to clear the PP scale factor.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Sub InitRun(bPreserveLayerData As Boolean)
            MyBase.InitRun(bPreserveLayerData)

            ' Reset preserved PP scale
            Me.m_sPreservedScale = cCore.NULL_VALUE
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialScalarDataAdapter.Adapt"/>
        ''' <remarks>
        ''' Called before data from an external source is copied into <see cref="cEcospaceDataStructures.RelPP"/>
        ''' EcoSpace uses an internal scaler to scale PP data to Ecopath levels. <see cref="cEcospaceDataStructures.PPScale"/>
        ''' This is the mean relative PP across all water cells computed from the currently loaded  <see cref="cEcospaceDataStructures.RelPP"/> array.
        ''' <see cref="cSpatialScalarDataAdapter.SetCell"/> will scale external data to a the first timestep or a user defined value.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Friend Overrides Function Adapt(bm As cEcospaceBasemap,
                                                  layer As cEcospaceLayer,
                                                  conn As cSpatialDataConnection,
                                                  iTime As Integer,
                                                  dt As Date,
                                                  dataExternal As ISpatialRaster,
                                                  dNullValue As Double) As Boolean

            Try

                'jb 13-Mar-2022 tweaked logic to rescale if there is a new conn.Scale value
                'This means a new dataset has been loaded
                'but only preserve the original PPScale value
                Dim breset As Boolean = False
                Dim bpreserve As Boolean = False
                If Me.m_spaceData.PPScale <> (1 / conn.Scale) Then
                    breset = True
                End If
                If (Me.m_sPreservedScale = cCore.NULL_VALUE) And (Me.m_spaceData.PPScale <> cCore.NULL_VALUE) Then
                    breset = True
                    bpreserve = True
                End If

                'If this is the first time step?
                'Get the base line PP Scalar
                'jb 13-Mar-2022 check above
                'If (Me.m_sPreservedScale = cCore.NULL_VALUE) And (Me.m_spaceData.PPScale <> cCore.NULL_VALUE) Then
                If breset Then
                    If bpreserve Then
                        Me.m_sPreservedScale = Me.m_spaceData.PPScale
                    End If
                    'In Ecospace PP is scaled as  [PP = RelPP(i, j) / PPScale] 
                    'PPScale is the mean over the base line map [Total PP] / [n water cells]
                    'DataScale() in the spatial temporal is calculate as [n water cells]/[total]
                    Me.m_spaceData.PPScale = (1 / conn.Scale)
                End If

            Catch ex As Exception
                System.Console.WriteLine("Exception: " & Me.ToString & ".Adapt() " & ex.Message)
                Return False
            End Try

            'Return True
            Return MyBase.Adapt(bm, layer, conn, iTime, dt, dataExternal, dNullValue)

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataAdapter.EndRun"/>
        ''' <summary>
        ''' Overridden to restore PP scale factor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub EndRun()
            MyBase.EndRun()

            ' Has preserved PP scale?
            If (Me.m_sPreservedScale <> cCore.NULL_VALUE) Then
                ' #Yes: Restore preserved PP scale
                Me.m_spaceData.PPScale = Me.m_sPreservedScale
                Me.m_sPreservedScale = cCore.NULL_VALUE
            End If

        End Sub

#End Region ' Overrides

    End Class

End Namespace

