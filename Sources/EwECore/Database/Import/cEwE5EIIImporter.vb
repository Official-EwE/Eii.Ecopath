#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore.Database
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports System.Text
Imports EwECore.DataSources

#End Region ' Imports 

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Imports an EwE5 .eii into an EwE6 database
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cEwE5EIIImporter
        Inherits cEwE5ModelImporter

#Region " Private helper class "

        Private Class cImportData
            Inherits cEcopathDataStructures

            Public Sub New(ByVal CoreMessagePublisher As cMessagePublisher)
                MyBase.New(CoreMessagePublisher)
            End Sub


            Public UnitTime As eUnitTimeType = eUnitTimeType.Year
            Public UnitTimeCustom As String = ""
            Public UnitCurrencyCustom As String = ""

        End Class

#End Region ' Private helper class

#Region " Private vars "

        ''' <summary>Source file index to read from.</summary>
        Private m_iFNum As Integer = cCore.NULL_VALUE

        ''' <summary>Data buffer.</summary>
        Private m_data As cImportData

#End Region ' Private vars

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' <param name="core">The core to import into.</param>
        ''' <param name="strEwE5File">Path to the Ecopath 5 document to import.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, ByVal strEwE5File As String)
            MyBase.New(core, strEwE5File)

            m_data = New cImportData(Me.m_core.Messages)

        End Sub

#End Region ' Construction

#Region " Overrides "

        ''' -----------------------------------------------------------------------
        ''' <inheritdoc cref="cEwE5ModelImporter.Close"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Open() As Boolean

            Debug.Assert(Not Me.IsOpen())

            Me.m_iFNum = FreeFile()
            Try
                FileOpen(Me.m_iFNum, Me.m_strEwE5File, OpenMode.Input)
            Catch ex As Exception
                Me.LogMessage(".LoadEcopath(...) Error opening eii file. " + vbCrLf + m_strEwE5File + vbCrLf + "Error:" + ex.Message())
                Me.m_iFNum = cCore.NULL_VALUE
                Return False
            End Try
            Return True

        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdoc cref="cEwE5ModelImporter.Close"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub Close()
            Debug.Assert(Me.IsOpen())

            FileClose(Me.m_iFNum)
            Me.m_iFNum = cCore.NULL_VALUE

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cEwE5ModelImporter.IsOpen"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsOpen() As Boolean

            Return (Me.m_iFNum <> cCore.NULL_VALUE)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdoc cref="cEwE5ModelImporter.GetModels"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function GetModels() As cEwE5ModelImporter.cEwE5ModelInfo()

            Debug.Assert(Me.IsOpen())

            Dim info As New cEwE5ModelImporter.cEwE5ModelInfo("1", Path.GetFileNameWithoutExtension(Me.m_strEwE5File), "Ecopath 5 EII file", 0)
            Return New cEwE5ModelImporter.cEwE5ModelInfo() {info}

        End Function

#End Region ' Overrides 

#Region " The import "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Imports and converts a model in an EwE5 database into a provided EwE6 database.
        ''' </summary>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function PerformImport() As Boolean

            Dim dbUpd As cDatabaseUpdater = Nothing
            Dim bSucces As Boolean = False

            Me.m_iNumSteps = 7

            If Me.Open() Then
                Me.LogProgress("Loading eii file...")
                If Me.LoadEII() Then
                    bSucces = Me.Save()
                End If
                Me.Close()
            End If

            ' Set version
            Me.m_dbEwE6.SetVersion(Me.m_dbEwE6.GetVersion(), "Imported from EII file '" & Me.m_strEwE5File & "'")

            ' Now run all available updates on the new EwE6 database
            dbUpd = New cDatabaseUpdater(Me.m_core, 6.0!)
            dbUpd.UpdateDatabase(Me.m_dbEwE6)
            dbUpd = Nothing

            ' Release DB
            Me.m_dbEwE6 = Nothing

            Me.LogMessage(My.Resources.CoreMessages.IMPORT_PROGRESS_COMPLETE)

            Return bSucces

        End Function

#End Region ' The import 

#Region " Loading "

        ''' <summary>
        ''' The old datasource code, to be transmogrified into database import logic
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function LoadEII() As Boolean

            'read the contents of the eii file into a private EcopathParameters object
            'this is written using vb file access instead of a filestream to keep it as close to the original vb code as possible
            Dim pvar As Single
            Dim i As Integer
            Dim j As Integer
            Dim K As Integer
            Dim Dummy As Single
            Dim jnk As String
            Dim Import As Integer

            Debug.Assert(Me.IsOpen())

            'read the file
            Try
                Input(Me.m_iFNum, m_data.NumGroups)
                Input(Me.m_iFNum, m_data.NumLiving)
                Input(Me.m_iFNum, m_data.UnitCurrencyCustom)
                Input(Me.m_iFNum, m_data.currUnitIndex)

                If Not m_data.redimGroupVariables() Then
                    Me.LogMessage(".LoadModel(...) Failed to Re-Dimension group parameter arrays.")
                    Return False
                End If

                'groups
                For K = 1 To m_data.NumGroups
                    Input(Me.m_iFNum, m_data.GroupName(K)) : Input(Me.m_iFNum, pvar) : Input(Me.m_iFNum, m_data.DtImp(K))
                    Input(Me.m_iFNum, m_data.Ex(K)) : Input(Me.m_iFNum, m_data.fCatch(K)) : Input(Me.m_iFNum, m_data.DCInput(K, 0))
                    Input(Me.m_iFNum, m_data.Binput(K)) : Input(Me.m_iFNum, m_data.PBinput(K)) : Input(Me.m_iFNum, m_data.EEinput(K))
                    Input(Me.m_iFNum, m_data.GEinput(K)) : Input(Me.m_iFNum, m_data.QBinput(K))

                    m_data.BHinput(K) = m_data.Binput(K) / m_data.Area(K)

                    m_data.GroupDBID(K) = K

                    m_data.PP(K) = pvar - 2
                    If K > m_data.NumLiving Then m_data.PP(K) = 2
                    If m_data.GE(K) = 0 Then m_data.GE(K) = -9

                Next K

                ' "Read DietComp"
                ReDim m_data.DietChanged(1, 0)
                For K = 1 To m_data.NumGroups
                    For j = 1 To m_data.NumGroups
                        Input(Me.m_iFNum, m_data.DCInput(K, j))
                    Next j
                Next K

                If EOF(Me.m_iFNum) Then Return True

                'jb totp read in original routine using a string will read the entire line
                Input(Me.m_iFNum, jnk)
                'jb I have no idea what this is all about 
                If Import < 0 Then Import = 0

                'Unassimilated food
                For j = 1 To m_data.NumGroups
                    Input(Me.m_iFNum, Dummy) : Input(Me.m_iFNum, m_data.GS(j))
                    If Dummy < 0 Then Dummy = 0
                    m_data.GS(j) = Dummy + m_data.GS(j)
                    If m_data.GS(j) > 1 Then m_data.GS(j) = m_data.GS(j) / 100
                Next j

                Input(Me.m_iFNum, jnk)

                'the time unit name
                If EOF(Me.m_iFNum) = False Then
                    Dim tmpbuff As String
                    Input(Me.m_iFNum, tmpbuff)
                    m_data.TimeUnitName = tmpbuff.Trim
                End If

                'the ecosystem remarks.
                Input(Me.m_iFNum, jnk)

                For i = 1 To m_data.NumGroups             ' parms.Bomass accumulation added March 95/VC
                    Input(Me.m_iFNum, m_data.BA(i))
                Next i

                'If EOF(me.m_iFNum) = False And NumGroups > NumLiving + 1 Then
                'More than 1 detritusbox Any reason for this??
                For i = 1 To m_data.NumGroups
                    For j = m_data.NumLiving + 1 To m_data.NumGroups
                        Input(Me.m_iFNum, m_data.DF(i, j - m_data.NumLiving))     ' Diet Fate array added July 1994/VC
                    Next j
                Next i

                Input(Me.m_iFNum, jnk) ' 
                For i = 1 To m_data.NumGroups             ' Emigration added Dec 98/VC
                    Input(Me.m_iFNum, m_data.Emigration(i))
                Next i

                Input(Me.m_iFNum, jnk)
                For i = 1 To m_data.NumGroups                 ' immigration added Dec 98/VC
                    Input(Me.m_iFNum, m_data.Immig(i))
                Next i

                Input(Me.m_iFNum, jnk)  'NumGear
                Input(Me.m_iFNum, m_data.NumFleet)

                m_data.RedimFleetVariables(True)

                Input(Me.m_iFNum, jnk) 'Gearnames
                For i = 1 To m_data.NumFleet             ' Added Dec 98/VC
                    Input(Me.m_iFNum, m_data.FleetName(i))
                    m_data.FleetDBID(i) = i
                Next i

                Input(Me.m_iFNum, jnk)  'cost
                For i = 1 To m_data.NumFleet
                    'First is fixed cost, second is cost per unit effort' Added Dec 98/VC
                    Input(Me.m_iFNum, m_data.CostPct(i, eCostIndex.Fixed))
                    Input(Me.m_iFNum, m_data.CostPct(i, eCostIndex.CUPE))
                    Input(Me.m_iFNum, m_data.CostPct(i, eCostIndex.Sail))
                Next i

                Input(Me.m_iFNum, jnk)  'landing
                For i = 1 To m_data.NumFleet
                    For j = 1 To m_data.NumGroups
                        Input(Me.m_iFNum, m_data.Landing(i, j))    ' Landing added Dec 98/VC
                    Next j
                Next i

                Input(Me.m_iFNum, jnk)  'discard
                For i = 1 To m_data.NumFleet
                    For j = 1 To m_data.NumGroups
                        Input(Me.m_iFNum, m_data.Discard(i, j))    ' Added Dec 98/VC
                    Next j
                Next i

                Input(Me.m_iFNum, jnk)  'discard
                For i = 1 To m_data.NumFleet
                    For j = 1 To m_data.NumGroups - m_data.NumLiving
                        Input(Me.m_iFNum, m_data.DiscardFate(i, j))   ' Added Dec 98/VC
                    Next j
                Next i

                Input(Me.m_iFNum, jnk)  'market
                For i = 1 To m_data.NumFleet
                    For j = 1 To m_data.NumGroups
                        Input(Me.m_iFNum, m_data.Market(i, j))    ' Added Dec 98/VC
                    Next j
                Next i

                m_data.NoGearData = False

                'shadow
                Input(Me.m_iFNum, jnk)
                For i = 1 To m_data.NumGroups             ' Added Dec 98/VC
                    Input(Me.m_iFNum, m_data.Shadow(i))
                Next i

                'Habitatarea
                Input(Me.m_iFNum, jnk)  '
                For i = 1 To m_data.NumGroups             ' Added Dec 98/VC
                    Input(Me.m_iFNum, m_data.Area(i))
                    Input(Me.m_iFNum, m_data.BH(i))
                Next i

            Catch ex As Exception 'catch any error during the reading of the data
                'some kind of a reading error better find out what happend
                Me.LogMessage(".LoadEcopath() Error reading eii file. Error: " + ex.Message())
                Debug.Assert(False)
                Return False
            End Try

            Return True

        End Function

#End Region ' Loading

#Region " Saving "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save imported data to the EwE6 database.
        ''' </summary>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Private Function Save() As Boolean
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_MODEL)
            Me.SaveModel()
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOPATHGROUPS)
            Me.SaveGroups()
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_POGRESS_DIETCOMP)
            Me.SaveDietComp()
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_FLEET)
            Me.SaveFleets()
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_CATCH)
            Me.SaveCatch()
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_CATCH)
            Me.SaveDiscardFate()
            Return True
        End Function

#Region " Model "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save Ecopath model
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub SaveModel()

            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim strYear As String = ""
            Dim dt As DateTime = Nothing

            ' Clear table
            Me.m_dbEwE6.Execute("DELETE * FROM EcopathModel")

            writer = m_dbEwE6.GetWriter("EcopathModel")

            drow = writer.NewRow()
            drow("ModelID") = 1
            drow("Name") = Path.GetFileNameWithoutExtension(Me.m_strEwE5File)
            drow("Description") = "Imported from EII file '" & Path.GetFileName(Me.m_strEwE5File) & "'"
            drow("NumDigits") = 3

            drow("UnitCurrency") = Me.m_data.currUnitIndex
            drow("UnitCurrencyCustom") = Me.m_data.UnitCurrencyCustom

            Select Case Me.m_data.UnitTimeCustom.Trim.ToLower()
                Case "year", "" : Me.m_data.UnitTime = eUnitTimeType.Year : Me.m_data.UnitTimeCustom = ""
                Case "day" : Me.m_data.UnitTime = eUnitTimeType.Day : Me.m_data.UnitTimeCustom = ""
                Case Else : Me.m_data.UnitTime = eUnitTimeType.Custom
            End Select
            drow("UnitTime") = Me.m_data.UnitTime
            drow("UnitTimeCustom") = Me.m_data.UnitTimeCustom

            drow("MonetaryUnit") = "EUR"
            writer.AddRow(drow)
            Me.m_dbEwE6.ReleaseWriter(writer, True)

        End Sub

#End Region ' Model

#Region " Groups "

        ''' <summary>
        ''' Save Ecopath groups
        ''' </summary>
        Private Function SaveGroups() As Boolean

            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim bSucces As Boolean = True

            ' Clear table(s)
            Me.m_dbEwE6.Execute("DELETE * FROM EcopathGroup")
            writer = m_dbEwE6.GetWriter("EcopathGroup")

            Try
                For iGroup As Integer = 1 To Me.m_data.NumGroups

                    drow = writer.NewRow()

                    drow("GroupID") = iGroup
                    drow("Sequence") = iGroup
                    drow("GroupName") = Me.m_data.GroupName(iGroup)
                    drow("Type") = Me.m_data.PP(iGroup)
                    drow("Area") = Me.m_data.Area(iGroup)
                    drow("BiomAcc") = Me.m_data.BA(iGroup)
                    drow("BiomAccRate") = Me.m_data.BaBi(iGroup)
                    drow("Unassim") = Me.m_data.GS(iGroup)
                    drow("DtImports") = Me.m_data.DtImp(iGroup)
                    drow("Export") = Me.m_data.Ex(iGroup)
                    drow("Catch") = Me.m_data.fCatch(iGroup)
                    drow("ImpVar") = Me.m_data.DCInput(iGroup, 0)
                    drow("GroupIsFish") = Me.m_data.GroupIsFish(iGroup)
                    drow("GroupIsInvert") = Me.m_data.GroupIsInvert(iGroup)
                    drow("NonMarketValue") = Me.m_data.Shadow(iGroup)
                    drow("Respiration") = Me.m_data.Resp(iGroup)

                    'variable with input/output pair only the input gets saved
                    drow("EcoEfficiency") = Me.m_data.EEinput(iGroup)
                    drow("ProdBiom") = Me.m_data.PBinput(iGroup)
                    drow("ConsBiom") = Me.m_data.QBinput(iGroup)
                    drow("ProdCons") = Me.m_data.GEinput(iGroup)
                    drow("Biomass") = Me.m_data.Binput(iGroup)

                    drow("Immigration") = Me.m_data.Immig(iGroup)
                    drow("Emigration") = Me.m_data.Emigration(iGroup)
                    drow("EmigRate") = Me.m_data.Emig(iGroup)
                    drow("PoolColor") = String.Format("{0:x8}", 0)

                    writer.AddRow(drow)

                Next iGroup

            Catch ex As Exception
                bSucces = False
            End Try
            Me.m_dbEwE6.ReleaseWriter(writer)
            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save Ecopath diets
        ''' </summary>
        ''' <returns>True if succesful</returns>
        ''' -------------------------------------------------------------------
        Private Function SaveDietComp() As Boolean

            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim bSucces As Boolean = True

            Me.m_dbEwE6.Execute("DELETE * FROM EcopathDietComp")
            writer = Me.m_dbEwE6.GetWriter("EcopathDietComp")

            Try
                For iPred As Integer = 1 To Me.m_data.NumGroups
                    For iPrey As Integer = 1 To Me.m_data.NumGroups

                        drow = writer.NewRow()

                        drow("PredID") = iPred
                        drow("PreyID") = iPrey
                        drow("Diet") = Me.m_data.DCInput(iPred, iPrey)
                        If iPrey > Me.m_data.NumLiving Then
                            drow("DetritusFate") = Me.m_data.DF(iPred, iPrey - Me.m_data.NumLiving)
                        Else
                            drow("DetritusFate") = 0
                        End If

                        writer.AddRow(drow)

                    Next iPrey
                Next iPred
            Catch ex As Exception
                bSucces = False
            End Try
            Me.m_dbEwE6.ReleaseWriter(writer, True)

            Return bSucces
        End Function

#End Region ' Groups

#Region " Fleets "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save Ecopath fleets
        ''' </summary>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Private Function SaveFleets() As Boolean

            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim bSucces As Boolean = True

            writer = Me.m_dbEwE6.GetWriter("EcopathFleet")
            Try
                For iFleet As Integer = 1 To Me.m_data.NumFleet

                    drow = writer.NewRow()

                    drow("Sequence") = iFleet
                    drow("FleetID") = iFleet
                    drow("FleetName") = Me.m_data.FleetName(iFleet)
                    drow("FixedCost") = Me.m_data.CostPct(iFleet, eCostIndex.Fixed)
                    drow("SailingCost") = Me.m_data.CostPct(iFleet, eCostIndex.Sail)
                    drow("variableCost") = Me.m_data.CostPct(iFleet, eCostIndex.CUPE)
                    drow("PoolColor") = String.Format("{0:x8}", 0)

                    writer.AddRow(drow)

                Next iFleet

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while saving EcopathFleet", ex.Message))
                bSucces = False
            End Try
            Me.m_dbEwE6.ReleaseWriter(writer, True)

            Return bSucces
        End Function

        ''' <summary>
        ''' Save Ecopath catch data
        ''' </summary>
        Private Function SaveCatch() As Boolean

            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim bSucces As Boolean = True

            Me.m_dbEwE6.Execute("DELETE * FROM EcopathCatch")
            writer = Me.m_dbEwE6.GetWriter("EcopathCatch")
            Try
                For iFleet As Integer = 1 To Me.m_data.NumFleet
                    For iGroup As Integer = 1 To Me.m_data.NumGroups

                        ' JS 04aug08: only save rows with data
                        If (Me.m_data.Landing(iFleet, iGroup) > 0.0!) Or _
                           (Me.m_data.Discard(iFleet, iGroup) > 0.0!) Or _
                           ((Me.m_data.Market(iFleet, iGroup) > 0.0!) And (Me.m_data.Market(iFleet, iGroup) < 1.0!)) Or _
                           (Me.m_data.PropDiscardMort(iFleet, iGroup) > 0.0!) Then

                            drow = writer.NewRow()
                            drow("FleetID") = iFleet
                            drow("GroupID") = iGroup
                            drow("Landing") = Me.m_data.Landing(iFleet, iGroup)
                            drow("Discards") = Me.m_data.Discard(iFleet, iGroup)
                            drow("Price") = Me.m_data.Market(iFleet, iGroup)
                            drow("DiscardMortality") = Me.m_data.PropDiscardMort(iFleet, iGroup)
                            writer.AddRow(drow)

                        End If

                    Next iGroup
                Next iFleet
            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while saving catch", ex.Message))
                bSucces = False
            End Try
            Me.m_dbEwE6.ReleaseWriter(writer)

            Return bSucces
        End Function

        ''' <summary>
        ''' Save Ecopath discard fate
        ''' </summary>
        Private Function SaveDiscardFate() As Boolean

            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim bSucces As Boolean = True

            Me.m_dbEwE6.Execute("DELETE * FROM EcopathDiscardFate")
            writer = Me.m_dbEwE6.GetWriter("EcopathDiscardFate")
            Try
                For iFleet As Integer = 1 To Me.m_data.NumFleet
                    For iGroup As Integer = 1 To Me.m_data.NumGroups - Me.m_data.NumLiving

                        drow = writer.NewRow()

                        drow("FleetID") = iFleet
                        drow("GroupID") = (iGroup + Me.m_data.NumLiving)
                        drow("DiscardFate") = Me.m_data.DiscardFate(iFleet, iGroup)

                        writer.AddRow(drow)

                    Next iGroup
                Next iFleet
            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while saving DiscardFate", ex.Message))
                bSucces = False
            End Try

            Me.m_dbEwE6.ReleaseWriter(writer)
            Return bSucces

        End Function

#End Region ' Fleets

#End Region ' Saving

    End Class

End Namespace ' Database
