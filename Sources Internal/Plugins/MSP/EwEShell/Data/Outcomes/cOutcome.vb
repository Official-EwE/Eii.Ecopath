' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Style

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Descriptor of the content of a single ecological outcome layer in the MSP game. 
''' This class describes which Ecospace output layers need to be grouped 
''' together into a single MSP outcome layer.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cOutcome
    Implements IMELItem

#Region " Private parts "

    Private m_core As cCore = Nothing
    Private m_units As cUnits = Nothing

    Private m_numerators As Double()
    Private m_denominators As Double()
    Private m_layertype As eLayerType

    ' For scaling to base value
    Private m_scalar As Double = 0F
    Private m_cellcount As Integer = 0

#End Region ' Private parts

#Region " Construction / destruction "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="core">The core to draw information from.</param>
    ''' <param name="strName">The name of the outcome layer.</param>
    ''' <param name="type">The <see cref="eLayerType">category</see> of the layer.</param>
    ''' ---------------------------------------------------------------------------
    Public Sub New(core As cCore, strName As String, type As eLayerType)

        Me.m_core = core
        Me.m_units = New cUnits(core)

        Me.Name = strName
        Me.LayerType = type

    End Sub

#End Region ' Construction / destruction

#Region " Public access "

    ''' <summary>
    ''' Initially a mirror of <see cref="eDiversityIndexType"/>, this list can
    ''' include diversity indices not natively computed by EwE.
    ''' </summary>
    Public Enum eMSPDIversityIndex As Integer
        ''' <summary>Shannon diversity indicator.</summary>
        Shannon = 0
        ''' <summary>Kempton's Q indicator.</summary>
        KemptonsQ = 1
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the output.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String Implements IMELItem.Name

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Outcome layers categories.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eLayerType As Integer
        ''' <summary>Outcome contains an Ecospace biomass distribution summary.</summary>
        Biomass
        ''' <summary>Outcome contains an Ecospace catch distribution summary.</summary>
        [Catch]
        ''' <summary>Outcome contains an Ecospace fishing effort distribution summary.</summary>
        Effort
        ''' <summary>Outcome contains an spatially explicit <see cref="eMSPDIversityIndex">ecological indicator</see>.</summary>
        Indicator
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the <see cref="eLayerType">categry</see> of the layer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property LayerType As eLayerType
        Get
            Return Me.m_layertype
        End Get
        Set(value As eLayerType)
            If (value <> Me.m_layertype) Or (Me.m_numerators Is Nothing) Then
                Me.m_layertype = value
                ReDim Me.m_numerators(Me.NumItems)
                ReDim Me.m_denominators(Me.NumItems)
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the numerator weight for ecospace results for a specific item (group or fleet) 
    ''' for composing the outcome layer.
    ''' </summary>
    ''' <param name="iIndex">The one-based index of the item to configure, which 
    ''' ranges from 1 to <see cref="NumItems()">the total number of items</see>.</param>
    ''' -----------------------------------------------------------------------
    Public Property Numerator(iIndex As Integer) As Double
        Get
            If (1 <= iIndex And iIndex < Me.m_numerators.Count) Then
                Return Me.m_numerators(iIndex)
            End If
            Return 0
        End Get
        Set(value As Double)
            If (1 <= iIndex And iIndex < Me.m_numerators.Count) Then
                Me.m_numerators(iIndex) = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the denominator weight for ecospace results for a specific item (group or fleet) 
    ''' for composing the outcome layer.
    ''' </summary>
    ''' <param name="iIndex">The one-based index of the item to configure, which 
    ''' ranges from 1 to <see cref="NumItems()">the total number of items</see>.</param>
    ''' -----------------------------------------------------------------------
    Public Property Denominator(iIndex As Integer) As Double
        Get
            If (1 <= iIndex And iIndex < Me.m_denominators.Count) Then
                Return Me.m_denominators(iIndex)
            End If
            Return 0
        End Get
        Set(value As Double)
            If (1 <= iIndex And iIndex < Me.m_denominators.Count) Then
                Me.m_denominators(iIndex) = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns if this output is configured to receive data.
    ''' </summary>
    ''' <returns>
    ''' True if this instance is configured.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function IsConfigured() As Boolean
        Select Case Me.LayerType
            Case eLayerType.Biomass, eLayerType.Catch, eLayerType.Effort
                Return Me.NumItems > 0
            Case eLayerType.Indicator
                Return True
            Case Else
                Debug.Assert(False, "Layer type not supported")
        End Select
        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the total number of items that can be aggregated in the outcome layer. 
    ''' </summary>
    ''' <returns>The total number of items that can be aggregated in the outcome layer. </returns>
    ''' -----------------------------------------------------------------------
    Public Function NumItems() As Integer
        Select Case Me.LayerType
            Case eLayerType.Biomass
                Return Me.m_core.nGroups
            Case eLayerType.Effort, eLayerType.Catch
                Return Me.m_core.nFleets
            Case eLayerType.Indicator
                Return [Enum].GetValues(GetType(eMSPDIversityIndex)).Length
        End Select
        Return 0
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of items that will be aggregated in the outcome layer.
    ''' </summary>
    ''' <returns>The total number of items that will be aggregated in the outcome layer. </returns>
    ''' -----------------------------------------------------------------------
    Public Function NumUsed() As Integer
        Dim n As Integer = 0
        For i As Integer = 1 To Me.NumItems
            If (Me.m_numerators(i) > 0) Or (Me.m_denominators(i) > 0) Then
                n += 1
            End If
        Next
        Return n
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns an informative string for the outcome layer configuration.
    ''' </summary>
    ''' <returns>Oooooh!</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ToString() As String
        Return String.Format("{1}: {0}, {2}/{3}", Me.Name, Me.LayerType, Me.NumUsed, Me.NumItems)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Populate the actual outcome layer.
    ''' </summary>
    ''' <param name="grid">The grid to populate.</param>
    ''' <param name="timestepdata">The Ecospace time step data that contains
    ''' the Ecospace estimates for the current time step.</param>
    ''' <param name="outcomerange">Outcome range, specifying the magnitude of variation
    ''' in Ecospace predictions reported in the outcome map.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overridable Function Populate(grid As cGrid, timestepdata As cEcospaceTimestep, outcomerange As Double) As Boolean

        grid.Clear()
        grid.IsValid = False

        If (timestepdata.iTimeStep = 1) Then Me.Calibrate(timestepdata)

        Debug.Assert(Me.m_scalar > 0)

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim nRows As Integer = bm.InRow
        Dim nCols As Integer = bm.InCol
        Dim nBins As Integer = 100
        Dim dTotal As Double = 0

        ' Sanity check
        Debug.Assert(bm.InCol = grid.Width And bm.InRow = grid.Height, "Grid / Ecospace map mismatch")

        ' Clear grid stats
        grid.Min = Double.MaxValue
        grid.Max = Double.MinValue
        grid.Mean = 0
        grid.NumValueCells = Me.m_cellcount

        ' Ensure reasonable value range
        outcomerange = Math.Max(2, outcomerange)

        Select Case Me.m_layertype
            Case eLayerType.Biomass, eLayerType.Catch
                grid.Units = Me.m_units.ToString(cUnits.Currency) ' "t/km²"
            Case eLayerType.Effort
                ' ToDo: obtain from metadata once it's there
                grid.Units = My.Resources.UNITS_EFFORT
            Case eLayerType.Indicator
                grid.Units = My.Resources.UNITS_RATIO
        End Select

        Try
            For iRow As Integer = 1 To nRows
                For iCol As Integer = 1 To nCols
                    Dim dCell As Double = 0D
                    If (bm.IsModelledCell(iRow, iCol)) Then
                        Dim a As Double = 0
                        Dim b As Double = 0
                        For iItem As Integer = 1 To Me.NumItems
                            Dim dVal As Double = 0
                            Select Case Me.m_layertype
                                Case eLayerType.Biomass
                                    dVal = timestepdata.BiomassMap(iRow, iCol, iItem)
                                Case eLayerType.Catch
                                    dVal = timestepdata.CatchFleetMap(iRow, iCol, iItem)
                                Case eLayerType.Effort
                                    dVal = timestepdata.FishingEffortMap(iItem, iRow, iCol)
                                Case eLayerType.Indicator
                                    Select Case DirectCast(iItem - 1, eMSPDIversityIndex)
                                        Case eMSPDIversityIndex.Shannon
                                            dVal = timestepdata.ShannonDiversityMap(iRow, iCol)
                                        Case eMSPDIversityIndex.KemptonsQ
                                            dVal = timestepdata.KemptonsQMap(iRow, iCol)
                                        Case Else
                                            Debug.Assert(False, "Indicator unknown")
                                    End Select
                            End Select
                            If (dVal > 0) Then
                                a += dVal * Math.Max(0, Math.Min(1, Me.Numerator(iItem)))
                                b += dVal * Math.Max(0, Math.Min(1, Me.Denominator(iItem)))
                            End If
                        Next iItem

                        ' MEL expects output layers with values between [0, 1]
                        ' We reused the Ecospace map colour binning logic to do this, using relative map values in the range of [0.1, 10], 
                        ' binned to a colour bin where value 0.5 = baseline value, yielding values of <0, 1]
                        ' - Upshot: MEL can render these data fast and on a predictable colour range
                        ' - Downside: MEL has no way of knowing the actual model outcome values, which is not handy if these values were needed

                        If (a > 0) Then

                            If (b = 0) Then b = 1

                            ' Calculate real output cell value
                            dCell = a / b

                            ' Calculate stats to report back to MEL based on the real value
                            grid.Min = Math.Min(grid.Min, dCell)
                            grid.Max = Math.Max(grid.Max, dCell)
                            dTotal += dCell

                            ' -- Now let's mutilate the cell value --

                            ' Scale cell value to map average.
                            dCell = dCell / Me.m_scalar

                            ' Truncate value range to [1, nBins]
                            If (dCell < 1 / outcomerange Or Double.IsNegativeInfinity(dCell)) Then
                                dCell = 1 ' nBins
                            ElseIf (dCell > outcomeRange Or Double.IsPositiveInfinity(dCell)) Then
                                dCell = nBins
                            Else
                                dCell = Math.Round(nBins * dCell / (dCell + 1))
                            End If
                            ' Convert to <0, 1]
                            dCell = dCell / nBins
                        End If
                    Else
                        ' MEL land cells are 0
                        dCell = 0
                    End If
                    ' Zero-based indexes!
                    grid.Cell(iCol - 1, iRow - 1) = dCell

                Next iCol
            Next iRow

            ' Complete Stats
            grid.Mean = dTotal / Math.Max(1, Me.m_cellcount)

            ' Done
            grid.IsValid = True

        Catch ex As Exception
            cEwEShell.RaiseException("cOutput.Populate exception " & ex.Message, False)
        End Try

        Return True

    End Function

#End Region ' Public access

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calibrates the output by calculating the scaling factor to use for the 
    ''' remainder of the run.
    ''' </summary>
    ''' <param name="timestepdata">The data with Ecospace time step data.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Calibrate(timestepdata As cEcospaceTimestep)

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim nRows As Integer = bm.InRow
        Dim nCols As Integer = bm.InCol
        Dim dTot As Double = 0

        Me.m_scalar = 0
        Me.m_cellcount = 0

        Try
            For iRow As Integer = 1 To nRows
                For iCol As Integer = 1 To nCols
                    If (bm.IsModelledCell(iRow, iCol)) Then
                        Dim A As Double = 0
                        Dim B As Double = 0
                        For iItem As Integer = 1 To Me.NumItems
                            Dim dVal As Double = 0
                            Select Case Me.m_layertype
                                Case eLayerType.Biomass
                                    dVal = timestepdata.BiomassMap(iRow, iCol, iItem)
                                Case eLayerType.Catch
                                    dVal = timestepdata.CatchFleetMap(iRow, iCol, iItem)
                                Case eLayerType.Effort
                                    dVal = timestepdata.FishingEffortMap(iItem, iRow, iCol)
                                Case eLayerType.Indicator
                                    Select Case DirectCast(iItem - 1, eMSPDIversityIndex)
                                        Case eMSPDIversityIndex.Shannon
                                            dVal = timestepdata.ShannonDiversityMap(iRow, iCol)
                                        Case eMSPDIversityIndex.KemptonsQ
                                            dVal = timestepdata.KemptonsQMap(iRow, iCol)
                                        Case Else
                                            Debug.Assert(False, "Indicator unknown")
                                    End Select
                            End Select
                            If (dVal > 0) Then
                                A += dVal * Math.Max(0, Math.Min(1, Me.Numerator(iItem)))
                                B += dVal * Math.Max(0, Math.Min(1, Me.Denominator(iItem)))
                            End If
                        Next iItem

                        If (B = 0) Then B = 1
                        dTot += A / B

                        Me.m_cellcount += 1
                    End If
                Next iCol
            Next iRow

        Catch ex As Exception
            cEwEShell.RaiseException("cOutput.Calibrate exception " & ex.Message, False)
        End Try

        ' Calculate scalar
        Me.m_scalar = dTot / Math.Max(1, Me.m_cellcount)
        If (Me.m_scalar = 0) Then Me.m_scalar = 1

    End Sub

#End Region ' Internals

End Class
