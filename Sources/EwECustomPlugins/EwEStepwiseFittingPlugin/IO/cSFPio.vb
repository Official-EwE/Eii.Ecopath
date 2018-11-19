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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
'    Scottish Association for Marine Science, Oban, Scotland
'
' Stepwise Fitting Procedure by Sheila Heymans, Erin Scott, Jeroen Steenbeek
' Copyright 2015- Scottish Association for Marine Science, Oban, Scotland
'
' Erin Scott was funded by the Scottish Informatics and Computer Science
' Alliance (SICSA) Postgraduate Industry Internship Programme.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Xml
Imports EwEUtils.Utilities
Imports EwEUtils.Core
Imports System.Text

#End Region ' Imports

Public Class cSFPio

    Private m_man As cSFPManager = Nothing

    Public Sub New(man As cSFPManager)
        Me.m_man = man
    End Sub

    Public Function ToXML(file As String) As Boolean

        ' Only allow this if has ran

        Dim xnRoot As XmlNode = Nothing
        Dim doc As XmlDocument = cXMLUtils.NewDoc("stepwise_fittine", xnRoot)
        Dim core As cCore = Me.m_man.Core

        If core.SaveWithFileHeader Then
            xnRoot.AppendChild(core.DefaultFileHeader(doc, eAutosaveTypes.Ecosim, core.EcosimFirstYear))
        End If

        Dim xnSettings As XmlElement = doc.CreateElement("settings")
        Dim xa As XmlAttribute = Nothing

        xa = doc.CreateAttribute("model")
        xa.InnerText = core.DataSource.ToString
        xnSettings.Attributes.Append(xa)

        xa = doc.CreateAttribute("ecosim")
        xa.InnerText = core.EcosimScenarios(core.ActiveEcosimScenarioIndex).Name
        xnSettings.Attributes.Append(xa)

        xa = doc.CreateAttribute("timeseries")
        xa.InnerText = core.TimeSeriesDataset(core.ActiveTimeSeriesDatasetIndex).Name
        xnSettings.Attributes.Append(xa)

        xa = doc.CreateAttribute("prefk")
        xa.InnerText = cStringUtils.FormatNumber(Me.m_man.K)
        xnSettings.Attributes.Append(xa)

        xa = doc.CreateAttribute("anomalyshape")
        xa.InnerText = ""
        If (Me.m_man.Parameters.AppliedShape IsNot Nothing) Then xa.InnerText = Me.m_man.Parameters.AppliedShape.Name
        xnSettings.Attributes.Append(xa)

        xa = doc.CreateAttribute("search")
        xa.InnerText = If(Me.m_man.Parameters.PredOrPredPreySSToV, "Pred", "PredPrey")
        xnSettings.Attributes.Append(xa)

        xa = doc.CreateAttribute("splinestep")
        xa.InnerText = CStr(Me.m_man.Parameters.AnomalySearchSplineStepSize)
        xnSettings.Attributes.Append(xa)

        xa = doc.CreateAttribute("absbiomass")
        xa.InnerText = CStr(Me.m_man.Parameters.EnableAbsoluteBiomass)
        xnSettings.Attributes.Append(xa)

        xnRoot.AppendChild(xnSettings)

        Dim xnIts As XmlElement = doc.CreateElement("iterations")
        For Each it As ISFPIterations In Me.m_man.Iterations
            xnIts.AppendChild(IterationNode(it, core, doc))
        Next
        xnRoot.AppendChild(xnIts)

        Try
            doc.Save(file)
            Return True
        Catch ex As Exception

        End Try
        Return False

    End Function

    Private Function IterationNode(it As ISFPIterations, core As cCore, doc As XmlDocument) As XmlNode

        Dim ndIteration As XmlElement = doc.CreateElement("iteration")
        Dim xa As XmlAttribute = Nothing

        xa = doc.CreateAttribute("type")
        xa.InnerText = cTypeUtils.TypeToString(it.GetType())
        ndIteration.Attributes.Append(xa)

        xa = doc.CreateAttribute("name")
        xa.InnerText = it.Name
        ndIteration.Attributes.Append(xa)

        xa = doc.CreateAttribute("enabled")
        xa.InnerText = it.Enabled.ToString()
        ndIteration.Attributes.Append(xa)

        xa = doc.CreateAttribute("k")
        xa.InnerText = CStr(it.K)
        ndIteration.Attributes.Append(xa)

        xa = doc.CreateAttribute("numvuls")
        xa.InnerText = cStringUtils.FormatNumber(it.EstimatedV)
        ndIteration.Attributes.Append(xa)

        xa = doc.CreateAttribute("numspline")
        xa.InnerText = cStringUtils.FormatNumber(it.SplinePoints)
        ndIteration.Attributes.Append(xa)

        xa = doc.CreateAttribute("ss")
        xa.InnerText = cStringUtils.FormatNumber(it.SS)
        ndIteration.Attributes.Append(xa)

        xa = doc.CreateAttribute("aic")
        xa.InnerText = cStringUtils.FormatNumber(it.AIC)
        ndIteration.Attributes.Append(xa)

        xa = doc.CreateAttribute("aicc")
        xa.InnerText = cStringUtils.FormatNumber(it.AICc)
        ndIteration.Attributes.Append(xa)

        xa = doc.CreateAttribute("state")
        xa.InnerText = it.RunState.ToString()
        ndIteration.Attributes.Append(xa)

        xa = doc.CreateAttribute("best")
        xa.InnerText = it.IsBestFit.ToString()
        ndIteration.Attributes.Append(xa)

        If (it.Vulnerabilities IsNot Nothing) And (it.EstimatedV > 0) Then
            Dim xnVuls As XmlElement = doc.CreateElement("vulnerabilities")
            For i As Integer = 1 To core.nLivingGroups
                Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(i)
                For j As Integer = 1 To core.nGroups
                    If (grp.DietComp(j) > 0) And (it.Vulnerabilities(j, i) <> 2) Then
                        Dim xnV As XmlElement = doc.CreateElement("v")
                        xa = doc.CreateAttribute("i") : xa.InnerText = i.ToString() : xnV.Attributes.Append(xa)
                        xa = doc.CreateAttribute("j") : xa.InnerText = j.ToString() : xnV.Attributes.Append(xa)
                        xa = doc.CreateAttribute("v") : xa.InnerText = cStringUtils.FormatNumber(it.Vulnerabilities(j, i)) : xnV.Attributes.Append(xa)
                        xnVuls.AppendChild(xnV)
                    End If
                Next
            Next
            ndIteration.AppendChild(xnVuls)
        End If

        Dim pts As Single() = it.AnomalyShape
        If (pts IsNot Nothing) And (it.SplinePoints > 0) Then
            Dim ndA As XmlNode = doc.CreateElement("anomaly")
            Dim sb As New StringBuilder()
            For i As Integer = 0 To it.AnomalyShape.Length - 1
                If (i > 0) Then sb.Append(",")
                sb.Append(cStringUtils.FormatSingle(it.AnomalyShape(i)))
            Next
            ndA.InnerText = sb.ToString()
            ndIteration.AppendChild(ndA)
        End If
        Return ndIteration

    End Function

    Private m_iterations As New List(Of ISFPIterations)

    ''' <summary>
    ''' Read stepwise fitting results from file.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Function FromXML(file As String) As Boolean

        Dim doc As New XmlDocument()
        Dim core As cCore = Me.m_man.Core
        doc.Load(file)

        Dim parms As cSFPParameters = Me.m_man.Parameters

        ' Make snapshot of iterations
        Me.m_iterations.AddRange(Me.m_man.Iterations)

        ' Parse settings first
        For Each xnSettings As XmlNode In doc.SelectSingleNode("/stepwise_fitting/settings")
            Dim bValidModel As TriState = TriState.UseDefault
            Dim WantedModel As String = ""
            Dim bValidEcosim As TriState = TriState.UseDefault
            Dim WantedEcosim As String = ""
            Dim bValidTS As TriState = TriState.UseDefault
            Dim WantedTS As String = ""
            Dim bValidShape As TriState = TriState.UseDefault
            Dim WantedShape As String = ""
            Dim bPredOrPredPreySSToV As Boolean = False
            Dim nSplineStep As Integer = 0
            Dim bAbsB As Boolean = False
            Dim nK As Integer = 0
            For Each xa As XmlAttribute In xnSettings.Attributes
                Select Case xa.Name.ToLower
                    Case "model"
                        WantedModel = xa.InnerText
                        bValidModel = If(String.Compare(core.DataSource.ToString, WantedModel, True) = 0, TriState.True, TriState.False)
                    Case "ecosim"
                        WantedEcosim = xa.InnerText
                        bValidEcosim = If(String.Compare(core.EcosimScenarios(core.ActiveEcosimScenarioIndex).Name, WantedEcosim, True) = 0, TriState.True, TriState.False)
                    Case "timeseries"
                        WantedTS = xa.InnerText
                        bValidTS = If(String.Compare(core.TimeSeriesDataset(core.ActiveTimeSeriesDatasetIndex).Name, WantedTS, True) = 0, TriState.True, TriState.False)
                    Case "anomalyshape"
                        WantedShape = xa.InnerText
                    Case "search" : bPredOrPredPreySSToV = (String.Compare(xa.InnerText, "Pred", True) = 0)
                    Case "splinestep" : nSplineStep = cStringUtils.ConvertToInteger(xa.InnerText)
                    Case "absbiomass" : bAbsB = (String.Compare(xa.InnerText, True.ToString(), True) = 0)
                    Case "prefk" : nK = cStringUtils.ConvertToInteger(xa.InnerText)
                End Select
            Next

            If (Not String.IsNullOrWhiteSpace(WantedShape)) Then
                bValidShape = TriState.False
                If (parms.AppliedShape IsNot Nothing) Then bValidShape = If(String.Compare(parms.AppliedShape.Name, WantedShape, True) = 0, TriState.True, TriState.False)
            Else
                bValidShape = If(parms.AppliedShape Is Nothing, TriState.True, TriState.False)
            End If

            If (bValidModel <> TriState.True) Or (bValidEcosim <> TriState.True) Or (bValidTS <> TriState.True) or (bValidShape <> TriState.True) Then
                Dim msg As New cMessage("Unable to load Stepwise Fitting results", eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Warning)
                If (bValidModel <> TriState.True) Then
                    msg.AddVariable(New cVariableStatus(eStatusFlags.ErrorEncountered, String.Format("Model {0} expected", WantedModel), eVarNameFlags.NotSet, eDataTypes.External, eCoreComponentType.EcoSimFitToTimeSeries, -1))
                End If
                If (bValidEcosim <> TriState.True) Then
                    msg.AddVariable(New cVariableStatus(eStatusFlags.ErrorEncountered, String.Format("Ecosim scenario {0} expected", WantedEcosim), eVarNameFlags.NotSet, eDataTypes.External, eCoreComponentType.EcoSimFitToTimeSeries, -1))
                End If
                If (bValidTS <> TriState.True) Then
                    msg.AddVariable(New cVariableStatus(eStatusFlags.ErrorEncountered, String.Format("Time series {0} expected", WantedTS), eVarNameFlags.NotSet, eDataTypes.External, eCoreComponentType.EcoSimFitToTimeSeries, -1))
                End If
                If (bValidShape <> TriState.True) Then
                    msg.AddVariable(New cVariableStatus(eStatusFlags.ErrorEncountered, String.Format("Anomaly shape {0} expected", WantedShape), eVarNameFlags.NotSet, eDataTypes.External, eCoreComponentType.EcoSimFitToTimeSeries, -1))
                End If
                core.Messages.SendMessage(msg)
                Return False
            End If

            parms.PredOrPredPreySSToV = bPredOrPredPreySSToV
            parms.AnomalySearchSplineStepSize = nSplineStep
            parms.AppliedShape = parms.AppliedShape ' Todo: resolve this
            parms.EnableAbsoluteBiomass = bAbsB
            Me.m_man.Refresh(nK)

        Next


    End Function

End Class
