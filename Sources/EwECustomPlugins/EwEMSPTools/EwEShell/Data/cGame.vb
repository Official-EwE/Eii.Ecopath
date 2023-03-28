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
Imports System.Xml
Imports EwECore
Imports System.Drawing
Imports System.Text
Imports EwEUtils.Utilities
Imports EwEUtils.SystemUtilities
Imports System.Web

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Container for a single game (model + its configuration) in the MSP software.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cGame
    Implements IMELItem

#Region " Private variables "

    Private m_core As cCore = Nothing
    Private m_pressures As New List(Of cPressure)
    Private m_pressuredrivers As New Dictionary(Of String, String)
    Private m_pressuremultipliers As New Dictionary(Of String, Double)
    Private m_outputs As New List(Of cOutcome)

#End Region ' Private variables

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new <see cref="cGame"/>.
    ''' </summary>
    ''' <param name="core">The core to operate onto.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore)

        MyBase.New()
        Me.m_core = core

    End Sub

#End Region ' Construction

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the unique name for the game.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String Implements IMELItem.Name

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the list of pressures definitions in the game.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Pressures As ICollection(Of cPressure)
        Get
            Return Me.m_pressures
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return a single pressure definitions by name.
    ''' </summary>
    ''' <param name="strName">The name of the pressure to retrieve.</param>
    ''' <remarks>The name search is not case sensitive.</remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Pressure(strName As String) As cPressure
        Get
            For Each p As cPressure In Me.m_pressures
                If (String.Compare(p.Name, strName, True) = 0) Then Return p
            Next
            Return Nothing
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a <see cref="cPressure"/> to the game.
    ''' </summary>
    ''' <param name="pressure">The <see cref="cPressure"/> to add.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Add(pressure As cPressure)
        For Each t As cPressure In Me.Pressures
            If (String.Compare(t.Name, pressure.Name, True) = 0) Then
                Return
            End If
        Next
        Me.m_pressures.Add(pressure)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a <see cref="cPressure"/> from the game.
    ''' </summary>
    ''' <param name="pressure">The <see cref="cPressure"/> to remove.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Remove(pressure As cPressure)
        Me.Driver(pressure.Name) = Nothing
        Me.m_pressures.Remove(pressure)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the list of output configurations defined in the game.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Outputs As ICollection(Of cOutcome)
        Get
            Return Me.m_outputs
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a <see cref="cOutcome"/> to the game.
    ''' </summary>
    ''' <param name="output">The <see cref="cOutcome"/> to add.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Add(output As cOutcome)
        For Each t As cOutcome In Me.Outputs
            If (String.Compare(t.Name, output.Name, True) = 0) Then
                Return
            End If
        Next
        Me.m_outputs.Add(output)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a <see cref="cOutcome"/> from the game.
    ''' </summary>
    ''' <param name="output">The <see cref="cOutcome"/> to remove.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Remove(output As cOutcome)
        Me.m_outputs.Remove(output)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the list of drivers available in the Ecospace scenario for 
    ''' receiving pressures.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Drivers(Optional datatype As cPressure.eDataTypes = cPressure.eDataTypes.NotSet) As cDriver()
        Get
            Return cDriverFactory.GetDrivers(Me.m_core, Me, datatype)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the game author information.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Author As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the game contact information.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Contact As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the game version.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Version As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the game description.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Description As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cCoreInputOutputBase.DBID"/> of the Ecosim scenario 
    ''' to load for the game.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EcosimID As Integer = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cCoreInputOutputBase.DBID"/> of Ecospace scenario 
    ''' to load for the game.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EcospaceID As Integer = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of spin-up years for initializing Ecospace.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property SpinupYears As Integer = 0

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether Ecospace must execute expensive trophic level and 
    ''' biodiversity indicator calculations.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property CalculateIndicators As Boolean = True

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of years to run Ecospace for in the MSP game. This
    ''' should be long enough to allow a full MSP run to complete, but otherwise
    ''' will not cause major memory constraints.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property RunYears As Integer = 1000

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the max scale of outcome content to MEL. By default this value
    ''' is 10, or one order of magnitude.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property OutcomeRange As Double = 10

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set how much of a cell needs to be covered by protection to close the
    ''' cell for fishing. This value is expressed as a cell area ratio [0, 1].
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property MPACellClosureRatio As Single = 0.25

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the driver connected to a specific pressure.
    ''' </summary>
    ''' <param name="pressure">The name of the pressure to find the driver for.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Property Driver(pressure As String) As cDriver
        Get
            If String.IsNullOrWhiteSpace(pressure) Then Return Nothing
            If (Not Me.m_pressuredrivers.ContainsKey(pressure)) Then Return Nothing
            For Each t As cDriver In Me.Drivers
                If (t.ValueID = Me.m_pressuredrivers(pressure)) Then Return t
            Next
            Return Nothing
        End Get
        Set(value As cDriver)
            If String.IsNullOrWhiteSpace(pressure) Then Return
            If (value IsNot Nothing) Then
                Me.m_pressuredrivers(pressure) = value.ValueID
            Else
                If (Me.m_pressuredrivers.ContainsKey(pressure)) Then
                    Me.m_pressuredrivers.Remove(pressure)
                End If
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the driver multiplier for to a specific pressure.
    ''' </summary>
    ''' <param name="pressure">The name of the pressure to find the multiplier for.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Property Multiplier(pressure As String) As Double
        Get
            If String.IsNullOrWhiteSpace(pressure) Then Return 1.0!
            If (Not Me.m_pressuremultipliers.ContainsKey(pressure)) Then Return 1.0!
            Return Me.m_pressuremultipliers(pressure)
        End Get
        Set(value As Double)
            If String.IsNullOrWhiteSpace(pressure) Then Return
            Me.m_pressuremultipliers(pressure) = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Find the output that describes how a specific outcome grid should be populated.
    ''' </summary>
    ''' <param name="grid">The grid to find the output for.</param>
    ''' <returns>An output if configured, or nothing if the output could not be found.</returns>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Output(grid As cGrid) As cOutcome
        Get
            If (grid Is Nothing) Then Return Nothing
            For Each out As cOutcome In Me.Outputs
                If (String.Compare(out.Name, grid.Name, True) = 0) Then Return out
            Next
            Return Nothing
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns an informative string summarizing the game configuration.
    ''' </summary>
    ''' <returns>Minime</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ToString() As String

        Dim sim As cEcoSimScenario = Me.EcosimScenario
        Dim spc As cEcospaceScenario = Me.EcospaceScenario
        Dim strSim As String = My.Resources.LABEL_ERROR
        Dim strSpace As String = My.Resources.LABEL_ERROR
        If (sim IsNot Nothing) Then strSim = sim.Name
        If (spc IsNot Nothing) Then strSpace = spc.Name

        Return cStringUtils.Localize(My.Resources.GAME_SUMMARY, Me.Name, strSim, strSpace)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, returns the actual <see cref="cEcoSimScenario"/> for the currently
    ''' configured <see cref="EcosimID"/> 
    ''' </summary>
    ''' <returns>A scenario, or nothing if this could not be found. </returns>
    ''' -----------------------------------------------------------------------
    Public Function EcosimScenario() As cEcoSimScenario
        If (Me.m_core IsNot Nothing) Then
            For i As Integer = 1 To Me.m_core.nEcosimScenarios
                Dim s As cEcoSimScenario = Me.m_core.EcosimScenarios(i)
                If s.DBID = Me.EcosimID Then Return s
            Next
        End If
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, returns the actual <see cref="cEcoSpaceScenario"/> for the currently
    ''' configured <see cref="EcospaceID"/> 
    ''' </summary>
    ''' <returns>A scenario, or nothing if this could not be found. </returns>
    ''' -----------------------------------------------------------------------
    Public Function EcospaceScenario() As cEcospaceScenario
        If (Me.m_core IsNot Nothing) Then
            For i As Integer = 1 To Me.m_core.nEcospaceScenarios
                Dim s As cEcospaceScenario = Me.m_core.EcospaceScenarios(i)
                If s.DBID = Me.EcospaceID Then Return s
            Next
        End If
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' MEL API call, validate the game configuration against expected parameters.
    ''' </summary>
    ''' <param name="timestep">Check: the number of time steps per year.</param>
    ''' <param name="longitude">Check: the model longitude origin to validate. Ignored for now because of slight numerical precision differences between MSP, MEL and EwE.</param>
    ''' <param name="latitude">Check: the model latitude origin to validate. Ignored for now because of slight numerical precision differences between MSP, MEL and EwE.</param>
    ''' <param name="size">Check: the model cell size. Ignored for now because of slight numerical precision differences between MSP, MEL and EwE.</param>
    ''' <param name="ncolumns">Check: the number of columns in the model.</param>
    ''' <param name="nrows">Check: the number of rows in the model.</param>
    ''' <param name="pressures">Check: the pressures that the model should support.</param>
    ''' <param name="outcomelayers">Check: the outcomes that the model should support.</param>
    ''' <returns>True if the game validated.</returns>
    ''' <exception cref="cMELException">A MEL exception will be thrown if something went wrong.</exception>
    ''' -----------------------------------------------------------------------
    Public Function Validate(timestep As Integer,
                             longitude As Double, latitude As Double, size As Double, ncolumns As Integer, nrows As Integer,
                             pressures As ICollection(Of cPressure), outcomelayers As ICollection(Of cGrid)) As Boolean

        If Me.m_core.ActiveEcospaceScenarioIndex <= 0 Then
            cEwEShell.RaiseException("Unable to validate, Ecospace not loaded.", False)
            Return False
        End If

        Dim parms As cEcospaceModelParameters = Me.m_core.EcospaceModelParameters
        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim ptTL As PointF = bm.PosTopLeft
        Dim bOK As Boolean = True

        ' This is rather important, but fails when ran on MSP live server
        If (Math.Round(parms.NumberOfTimeStepsPerYear) <> timestep) Then
            cEwEShell.RaiseException("Validation failed; time step " & Math.Round(parms.NumberOfTimeStepsPerYear) & " expected.", False)
            bOK = False
        End If

        'If (longitude <> cCore.NULL_VALUE) And (latitude <> cCore.NULL_VALUE) And ((Math.Abs(longitude - ptTL.X) > 0.01) Or (Math.Abs(latitude - ptTL.Y) > 0.01)) Then
        '    cEwEShell.RaiseException("Validation failed; spatial bounds lon=" & ptTL.X & ", lat=" & ptTL.Y & " expected.", False)
        '    bOK = False
        'End If

        'If (size <> cCore.NULL_VALUE) And (Math.Abs(bm.CellSize - size) > 0.01) Then
        '    cEwEShell.RaiseException("Validation failed; cell size " & bm.CellSize & " expected.", False)
        '    bOK = False
        'End If

        If ((ncolumns <> bm.InCol) Or (nrows <> bm.InRow)) Then
            cEwEShell.RaiseException("Validation failed; grid width=" & bm.InCol & ", height=" & bm.InRow & " expected.", False)
            bOK = False
        End If

        ' Validate if all pressures are configured in the game
        For Each pressure As cPressure In pressures
            Dim pressureConfig As cPressure = Me.Pressure(pressure.Name)
            Dim driver As cDriver = Me.Driver(pressure.Name)

            If (pressureConfig Is Nothing) Then
                cEwEShell.RaiseException("Validation failed; pressure '" & pressure.Name & "' is absent from the EwE model.", False)
                bOK = False
            ElseIf (driver Is Nothing) Then
                cEwEShell.RaiseException("Validation failed; pressure '" & pressure.Name & "' is not connected to an Ecospace driver.", False)
                bOK = False
            End If
        Next

        ' Validate if all outcomes are configured in the game
        For Each outcome As cGrid In outcomelayers
            Dim output As cOutcome = Me.Output(outcome)

            If (output Is Nothing) Then
                cEwEShell.RaiseException("Validation warning; outcome '" & outcome.Name & "' is not defined in the EwE model.", False)
            Else
                If (Not output.IsConfigured()) Then
                    cEwEShell.RaiseException("Validation warning; outcome '" & outcome.Name & "' is defined but is not configured to receive input.", False)
                    ' Not an error
                End If
            End If
        Next

        Return bOK

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the effort start values configured in the game.
    ''' </summary>
    ''' <returns>A collection of Scalars (fleet name, value).</returns>
    ''' -----------------------------------------------------------------------
    Public Function EffortStartValues() As ICollection(Of cScalar)
        Dim items As New List(Of cScalar)
        For Each p As cPressure In Me.Pressures
            If p.IsScalar Then
                Dim d As cEffortDriver = DirectCast(Me.Driver(p.Name), cEffortDriver)
                Dim s As New cScalar(p.Name, d.StartValue / Me.Multiplier(p.Name))
                items.Add(s)
            End If
        Next
        Return items
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply pressure layers to the running Ecospace scenario.
    ''' </summary>
    ''' <param name="pressurelayers">The pressures to apply.</param>
    ''' <param name="spaceDS">Optional Ecospace datastructures to directly apply pressurs to.</param>
    ''' <returns>True if successful.</returns>
    ''' <exception cref="cMELException">A MEL exception will be thrown if something went wrong.</exception>
    ''' -----------------------------------------------------------------------
    Public Function ApplyPressures(pressurelayers As cPressure(), Optional spaceDS As cEcospaceDataStructures = Nothing) As Boolean

        Dim bOK As Boolean = True

        ' Validate and apply pressure layers received from MEL. Do not worry about pressure drivers connected in EwE NOT provided by MEL,
        ' although a warning might be nice
        For Each pressure As cPressure In pressurelayers

            Dim pressureConfig As cPressure = Me.Pressure(pressure.Name)
            Dim driver As cDriver = Me.Driver(pressure.Name)

            If (pressureConfig Is Nothing) Then
                cEwEShell.RaiseException("Configuration mismatch; '" & pressure.Name & "' not defined as MEL pressure in model.", False)
                bOK = False
            End If

            If (driver Is Nothing) Then
                cEwEShell.RaiseException("Configuration mismatch; '" & pressure.Name & "' not mapped to EwE variable in model.", False)
                bOK = False
            End If

            If (pressureConfig IsNot Nothing And driver IsNot Nothing) Then
                'Debug.WriteLine("@@ Applying pressure " & pressure.Name & " to driver " & driver.Name)
                If (driver.DataType = pressure.DataType) Then
                    If Not driver.Apply(pressure, spaceDS, Me.Multiplier(pressure.Name)) Then
                        cEwEShell.RaiseException("Pressure mismatch; '" & pressure.Name & "' failed to apply to EwE driver " & driver.Name & ".", True)
                        bOK = False
                    End If
                Else
                    cEwEShell.RaiseException("Pressure mismatch; '" & pressure.Name & "' incompatible with EwE driver " & driver.Name & ".", True)
                End If
            End If

        Next

        ' Invalidate capacity if directly using Ecospace data structures
        If (spaceDS IsNot Nothing) Then
            spaceDS.isCapacityChanged = True
            For i As Integer = 1 To Me.m_core.nGroups
                spaceDS.isGroupHabCapChanged(i) = True
            Next
        End If

        Return bOK

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load all outcomes from the current time step results.
    ''' </summary>
    ''' <param name="outcomelayers">The outcome layers to populate.</param>
    ''' <param name="results">Ecospace time step data to populate the outcomes from.</param>
    ''' <returns>True if successful.</returns>
    ''' <exception cref="cMELException">A MEL exception will be thrown if something went wrong.</exception>
    ''' <see cref="cEcospaceTimestep"/>
    ''' -----------------------------------------------------------------------
    Public Function LoadOutcomes(outcomelayers() As cGrid, results As cEcospaceTimestep) As Boolean

        Dim bOK As Boolean = True
        For Each grid As cGrid In outcomelayers
            Dim output As cOutcome = Me.Output(grid)
            If (output IsNot Nothing) Then
                Debug.WriteLine("@@ Loading output " & output.Name & " into outcome " & grid.Name)
                bOK = bOK And output.Populate(grid, results, Me.OutcomeRange)
            Else
                cEwEShell.RaiseException("Outcome mismatch; grid '" & grid.Name & "' is not configured to receive EwE outputs.", False)
                bOK = False
            End If
        Next grid

        Return bOK

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Prepare the game. Load sim and space, run space spin-up.
    ''' </summary>
    ''' <remarks>True if successful.</remarks>
    ''' <exception cref="cMELException">A MEL exception will be thrown if something went wrong.</exception>
    ''' -----------------------------------------------------------------------
    Public Function Load() As Boolean

        If (Not Me.m_core.LoadEcosimScenario(Me.EcosimScenario)) Then
            cEwEShell.RaiseException("Game load error; failed to load Ecosim scenario with ID " & Me.EcosimID, True)
            Return False
        End If

        If (Not Me.m_core.LoadEcospaceScenario(Me.EcospaceScenario)) Then
            cEwEShell.RaiseException("Game load error; failed to load Ecospace scenario with ID " & Me.EcospaceID, True)
            Return False
        End If

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create default pressures and outputs.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub AddDefaultPressures()

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap

        Me.Add(New cPressure("Noise", bm.InCol, bm.InRow))
        Me.Add(New cPressure("Bottom disturbance", bm.InCol, bm.InRow))
        Me.Add(New cPressure("Surface disturbance", bm.InCol, bm.InRow))
        Me.Add(New cPressure("Artificial habitat", bm.InCol, bm.InRow))
        For i As Integer = 1 To Me.m_core.nFleets
            Me.Add(New cPressure("Protection " & Me.m_core.EcopathFleetInputs(i).Name, bm.InCol, bm.InRow))
            Me.Add(New cPressure("Fishing intensity " & Me.m_core.EcopathFleetInputs(i).Name, 1.0))
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of drivers that are configured to receive pressure data.
    ''' </summary>
    ''' <returns>The number of drivers that are configured to receive pressure data.</returns>
    ''' -----------------------------------------------------------------------
    Public Function NumConnectedDrivers() As Integer
        Dim n As Integer = 0
        For Each strPressure As String In Me.m_pressuredrivers.Keys()
            If (Me.Driver(strPressure) IsNot Nothing) Then
                n += 1
            End If
        Next
        Return n
    End Function

#End Region ' Public access

#Region " XML serialization "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Serialize to XML.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Function ToXML(doc As XmlDocument) As XmlNode

        Dim xnGame As XmlNode = doc.CreateElement("game")
        Dim xa As XmlAttribute = Nothing

        xa = doc.CreateAttribute("name")
        xa.InnerText = HttpUtility.UrlEncode(Me.Name)
        xnGame.Attributes.Append(xa)

        xa = doc.CreateAttribute("version")
        xa.InnerText = HttpUtility.UrlEncode(Me.Version)
        xnGame.Attributes.Append(xa)

        xa = doc.CreateAttribute("author")
        xa.InnerText = HttpUtility.UrlEncode(Me.Author)
        xnGame.Attributes.Append(xa)

        xa = doc.CreateAttribute("contact")
        xa.InnerText = HttpUtility.UrlEncode(Me.Contact)
        xnGame.Attributes.Append(xa)

        xa = doc.CreateAttribute("ecosim_id")
        xa.InnerText = cStringUtils.FormatNumber(Me.EcosimID)
        xnGame.Attributes.Append(xa)

        xa = doc.CreateAttribute("ecospace_id")
        xa.InnerText = cStringUtils.FormatNumber(Me.EcospaceID)
        xnGame.Attributes.Append(xa)

        xa = doc.CreateAttribute("spinup_years")
        xa.InnerText = cStringUtils.FormatNumber(Me.SpinupYears)
        xnGame.Attributes.Append(xa)

        xa = doc.CreateAttribute("run_years")
        xa.InnerText = cStringUtils.FormatNumber(Me.RunYears)
        xnGame.Attributes.Append(xa)

        xa = doc.CreateAttribute("calc_indicators")
        xa.InnerText = Convert.ToString(Me.CalculateIndicators)
        xnGame.Attributes.Append(xa)

        xa = doc.CreateAttribute("mpa_cell_closure")
        xa.InnerText = cStringUtils.FormatNumber(Me.MPACellClosureRatio)
        xnGame.Attributes.Append(xa)

        Dim xnDescr As XmlNode = doc.CreateElement("description")
        xnDescr.InnerText = HttpUtility.UrlEncode(Me.Description)
        xnGame.AppendChild(xnDescr)

        ' Also serialize pressures
        Dim xnPressures As XmlNode = doc.CreateElement("pressures")
        For Each p As cPressure In Me.Pressures

            Dim xnPressure As XmlNode = doc.CreateElement("pressure")

            xa = doc.CreateAttribute("name")
            xa.InnerText = HttpUtility.UrlEncode(p.Name)
            xnPressure.Attributes.Append(xa)

            xa = doc.CreateAttribute("type")
            xa.InnerText = p.DataType.ToString()
            xnPressure.Attributes.Append(xa)

            xnPressures.AppendChild(xnPressure)

        Next
        xnGame.AppendChild(xnPressures)

        Dim xnMappings As XmlNode = doc.CreateElement("mappings")
        For Each strKey As String In Me.m_pressuredrivers.Keys

            Dim xnMapping As XmlNode = doc.CreateElement("mapping")

            xa = doc.CreateAttribute("pressure")
            xa.InnerText = HttpUtility.UrlEncode(strKey)
            xnMapping.Attributes.Append(xa)

            xa = doc.CreateAttribute("driver")
            xa.InnerText = HttpUtility.UrlEncode(Me.m_pressuredrivers(strKey))
            xnMapping.Attributes.Append(xa)

            If (Me.m_pressuremultipliers.ContainsKey(strKey)) Then
                xa = doc.CreateAttribute("multiplier")
                xa.InnerText = cStringUtils.FormatNumber(Me.m_pressuremultipliers(strKey))
                xnMapping.Attributes.Append(xa)
            End If

            xnMappings.AppendChild(xnMapping)

        Next
        xnGame.AppendChild(xnMappings)

        ' Add outputs
        Dim xnOutputs As XmlNode = doc.CreateElement("outputs")
        Dim sb As New StringBuilder()
        For Each output As cOutcome In Outputs

            Dim xnOutput As XmlNode = doc.CreateElement("output")

            xa = doc.CreateAttribute("name")
            xa.InnerText = HttpUtility.UrlEncode(output.Name)
            xnOutput.Attributes.Append(xa)

            xa = doc.CreateAttribute("type")
            xa.InnerText = output.LayerType().ToString
            xnOutput.Attributes.Append(xa)

            xa = doc.CreateAttribute("numerators")
            sb.Clear()
            For i As Integer = 1 To output.NumItems
                If (i > 1) Then sb.Append(",")
                sb.Append(cStringUtils.FormatNumber(output.Numerator(i)))
            Next
            xa.InnerText = sb.ToString()
            xnOutput.Attributes.Append(xa)

            xa = doc.CreateAttribute("denominators")
            sb.Clear()
            For i As Integer = 1 To output.NumItems
                If (i > 1) Then sb.Append(",")
                sb.Append(cStringUtils.FormatNumber(output.Denominator(i)))
            Next
            xa.InnerText = sb.ToString()
            xnOutput.Attributes.Append(xa)

            xnOutputs.AppendChild(xnOutput)

        Next
        xnGame.AppendChild(xnOutputs)

        Return xnGame

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' De-serialize from XML.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Function FromXML(xnGame As XmlNode) As Boolean

        ' Initialize defaults
        Me.RunYears = 10000
        Me.SpinupYears = 0
        Me.MPACellClosureRatio = 0.25

        Try
            For Each xa As XmlAttribute In xnGame.Attributes
                Select Case xa.Name
                    Case "name" : Me.Name = HttpUtility.UrlDecode(xa.InnerText)
                    Case "version" : Me.Version = HttpUtility.UrlDecode(xa.InnerText)
                    Case "author" : Me.Author = HttpUtility.UrlDecode(xa.InnerText)
                    Case "contact" : Me.Contact = HttpUtility.UrlDecode(xa.InnerText)
                    Case "ecosim_id" : Me.EcosimID = cStringUtils.ConvertToInteger(xa.InnerText)
                    Case "ecospace_id" : Me.EcospaceID = cStringUtils.ConvertToInteger(xa.InnerText)
                    Case "spinup_years" : Me.SpinupYears = cStringUtils.ConvertToInteger(xa.InnerText)
                    Case "run_years" : Me.RunYears = cStringUtils.ConvertToInteger(xa.InnerText)
                    Case "mpa_cell_closure" : Me.MPACellClosureRatio = cStringUtils.ConvertToSingle(xa.InnerText)
                    Case "calc_indicators" : Me.CalculateIndicators = Convert.ToBoolean(xa.InnerText)
                End Select
            Next
        Catch ex As Exception
            Console.WriteLine("cGame.FromXML-attr: " & ex.Message)
        End Try

        For Each xn As XmlNode In xnGame.ChildNodes
            Select Case xn.Name
                Case "description"
                    Try
                        Me.Description = HttpUtility.UrlDecode(xn.InnerText)
                    Catch ex As Exception
                        Console.WriteLine("cGame.FromXML-description: " & ex.Message)
                    End Try
                Case "pressures"
                    Try
                        For Each xnPressure As XmlNode In xn.ChildNodes
                            Dim strPressure As String = ""
                            Dim t As cPressure.eDataTypes = cPressure.eDataTypes.NotSet
                            For Each xa As XmlAttribute In xnPressure.Attributes
                                Select Case xa.Name
                                    Case "name" : strPressure = HttpUtility.UrlDecode(xa.InnerText)
                                    Case "type" : [Enum].TryParse(xa.InnerText, t)
                                End Select
                            Next
                            If (t <> cPressure.eDataTypes.NotSet) Then
                                Me.Add(New cPressure(strPressure, t))
                            End If
                        Next
                    Catch ex As Exception
                        Console.WriteLine("cGame.FromXML-pressures: " & ex.Message)
                    End Try

                Case "mappings"
                    Try
                        For Each xnMapping As XmlNode In xn.ChildNodes
                            Dim strPressure As String = ""
                            Dim strDriver As String = ""
                            Dim dMultiplier As Double = 1.0!
                            For Each xa As XmlAttribute In xnMapping.Attributes
                                Select Case xa.Name
                                    Case "pressure" : strPressure = HttpUtility.UrlDecode(xa.InnerText)
                                    Case "driver" : strDriver = HttpUtility.UrlDecode(xa.InnerText)
                                    Case "multiplier" : dMultiplier = cStringUtils.ConvertToDouble(xa.InnerText)
                                End Select
                            Next
                            If (Not String.IsNullOrWhiteSpace(strPressure)) And (Not String.IsNullOrWhiteSpace(strDriver)) Then
                                Me.m_pressuredrivers(strPressure) = strDriver
                                Me.m_pressuremultipliers(strPressure) = dMultiplier
                            End If
                        Next
                    Catch ex As Exception
                        Console.WriteLine("cGame.FromXML-mappings: " & ex.Message)
                    End Try

                Case "outputs"
                    Try
                        For Each xnOutput As XmlNode In xn.ChildNodes
                            Dim strName As String = ""
                            Dim type As cOutcome.eLayerType
                            Dim strNumerators As String = ""
                            Dim strDenominators As String = ""
                            For Each xa As XmlAttribute In xnOutput.Attributes
                                Select Case xa.Name
                                    Case "name" : strName = HttpUtility.UrlDecode(xa.InnerText)
                                    Case "type" : [Enum].TryParse(xa.InnerText, type)
                                    Case "items", "numerators" : strNumerators = xa.InnerText
                                    Case "denominators" : strDenominators = xa.InnerText
                                End Select
                            Next

                            Dim output As New cOutcome(Me.m_core, strName, type)
                            If (strNumerators.Contains(",")) Then
                                Dim nums As String() = strNumerators.Split(","c)
                                For i As Integer = 1 To Math.Min(nums.Count, output.NumItems)
                                    output.Numerator(i) = cStringUtils.ConvertToDouble(nums(i - 1))
                                Next
                                nums = strDenominators.Split(","c)
                                For i As Integer = 1 To Math.Min(nums.Count, output.NumItems)
                                    output.Denominator(i) = cStringUtils.ConvertToDouble(nums(i - 1))
                                Next
                            Else
                                ' Backward compatibility
                                For i As Integer = 1 To Math.Min(strNumerators.Length, output.NumItems)
                                    output.Numerator(i) = If(strNumerators(i - 1) = "1", 1, 0)
                                    output.Denominator(i) = 0
                                Next
                            End If
                            Me.Add(output)
                        Next
                    Catch ex As Exception
                        Console.WriteLine("cGame.FromXML-outputs: " & ex.Message)
                    End Try

            End Select
        Next

        Return True

    End Function

#End Region ' XML serialization 

End Class
