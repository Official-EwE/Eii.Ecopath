#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports System.Reflection
Imports EwEUtils.Utilities
Imports EwEUtils.Database
Imports EwECore
Imports ScientificInterfaceShared.Style

#End Region ' Imports

<TypeConverter(GetType(cPropertySorter)), _
    DefaultProperty("Name"), _
    Serializable()> _
Public MustInherit Class cUnit
    Inherits EwEUtils.Database.cEwEDatabase.cOOPStorable

    Protected Const sPROPCAT_GENERAL As String = "1. General"
    'Protected Const sPROPCAT_INTEGRATION As String = "2. Ecopath integration"
    Protected Const sPROPCAT_PRODUCTS As String = "2. Products ($/t)"
    Protected Const sPROPCAT_REVENUE As String = "3. Revenue ($/effort)"
    'Protected Const sPROPCAT_EFFORTSUBSIDIES As String = "4. Effort subsidies ($/effort)"
    Protected Const sPROPCAT_SUBSIDIES As String = "4. Subsidies ($/t)"
    Protected Const sPROPCAT_PAY As String = "5. Pay ($/t)"
    Protected Const sPROPCAT_SHARE As String = "5. Share (% revenue)"
    'Protected Const sPROPCAT_EFFORTCOST As String = "6. Effort cost ($/effort)"
    Protected Const sPROPCAT_INPUTCOST As String = "6. Input cost ($/t)"
    Protected Const sPROPCAT_TAXES As String = "7. Taxes ($/t)"
    Protected Const sPROPCAT_SOCIAL As String = "8. Social (#/t)"


    ''' <summary>Index of the unit, which this unit needs to store its values in the Results object</summary>
    Private m_iSequence As Integer = 0
    ''' <summary>List of input variables that this unit needs in order to perform its calculations.</summary>
    Private m_lReceivedInputs As New List(Of cInput)
    ''' <summary>Name of the unit</summary>
    Private m_strName As String
    ''' <summary>Nationality of a unit.</summary>
    Private m_iNationality As Integer
    ''' <summary>Zhe ceur</summary>
    Private m_core As cCore = Nothing

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        MyBase.New()
    End Sub

#End Region ' Constructor

#Region " Links "

    ''' <summary>Units that receive outputs from this unit.</summary>
    Private m_llinkOutput As New List(Of cLink)
    ''' <summary>Units that provide inputs for this unit.</summary>
    Private m_llinkInput As New List(Of cLink)

    Public Function LinkOutCount() As Integer
        Return Me.m_llinkOutput.Count
    End Function

    Public Function LinkOut(ByVal iIndex As Integer) As cLink
        Return Me.m_llinkOutput(iIndex)
    End Function

    Public Sub AddLink(ByVal link As cLink)

        ' Sanity check
        Debug.Assert(Object.ReferenceEquals(link.Source, Me))

        Me.m_llinkOutput.Add(link)
        link.Target.AddInputLink(link)
    End Sub

    Public Sub RemoveLink(ByVal link As cLink)
        Me.m_llinkOutput.Remove(link)
        link.Target.RemoveInputLink(link)
    End Sub

    Public Function LinkInCount() As Integer
        Return Me.m_llinkInput.Count
    End Function

    Public Function LinkIn(ByVal iIndex As Integer) As cLink
        Return Me.m_llinkInput(iIndex)
    End Function

    Protected Sub AddInputLink(ByVal link As cLink)

        ' Sanity check
        Debug.Assert(Object.ReferenceEquals(link.Target, Me))

        Me.m_llinkInput.Add(link)
    End Sub

    Protected Sub RemoveInputLink(ByVal link As cLink)
        Me.m_llinkInput.Remove(link)
    End Sub

    Public Function IsLoop(ByVal unit As cUnit) As Boolean

        ' Linked to self?
        Dim bIsLoop As Boolean = Object.ReferenceEquals(unit, Me)

        'Console.WriteLine("{0}:{1}={2}", Me.Name, unit.Name, bIsLoop)

        ' If no loop yet
        If Not bIsLoop Then
            ' Follow each output link
            For Each link As cLink In Me.m_llinkOutput
                ' See the target link is the requesting unit
                If link.Target.IsLoop(unit) Then bIsLoop = True : Exit For
            Next link
        End If

        Return bIsLoop
    End Function

    Public Function HasTarget(ByVal unit As cUnit) As Boolean

        ' Follow each output link
        For Each link As cLink In Me.m_llinkOutput
            ' See the target link is the requesting unit
            If Object.ReferenceEquals(link.Target, unit) Then Return True
        Next link
        Return False

    End Function

#End Region ' Links 

#Region " Running "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the unit for a new Ecosim or Ecospace run.
    ''' </summary>
    ''' <param name="core">The EwE core that this run is performed onto.</param>
    ''' <param name="iSequence">The sequence number to assign to this unit for the run.</param>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub InitRun(ByVal core As cCore, ByVal iSequence As Integer)
        Me.Sequence = iSequence
        Me.Core = core
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the unit for running a chain.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub Clear()
        ' Clear all pending inputs
        Me.m_lReceivedInputs.Clear()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculate the economics for this unit.
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="input"></param>
    ''' <param name="iTimeStep"></param>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub Process(ByVal results As cResults, _
                                   ByVal input As cInput, _
                                   ByVal iTimeStep As Integer, _
                                   ByVal iFleet As Integer)

        Dim inputTotal As cInput = Nothing
        Dim sTotalOutputBiomass As Single = 0
        Dim sTotalOutputValue As Single = 0
        Dim sValuePerTon As Single = 0.0!

        ' Store received values
        Me.m_lReceivedInputs.Add(input)

        ' At least expected inputs received?
        If (Me.m_lReceivedInputs.Count >= Me.LinkInCount()) Then

            ' #Yes: Process!

            ' Combine all inputs
            inputTotal = Me.SumInputs(Me.m_lReceivedInputs)

            ' Determine outgoing biomass
            For Each link As cLink In Me.m_llinkOutput
                ' Determing output biomass for a single link
                Dim sOutputBiomass As Single = link.BiomassRatio * inputTotal.Tons

                sValuePerTon = link.ValuePerTon
                ' Is default link value?
                If (sValuePerTon = 1.0!) And (inputTotal.Tons <> 0.0!) Then
                    ' #Yes: use aggregated input value
                    sValuePerTon = inputTotal.Value / inputTotal.Tons
                End If

                sTotalOutputValue += sValuePerTon * sOutputBiomass
                sTotalOutputBiomass += sOutputBiomass
            Next

            results.StoreFleetContribution(iFleet, Me, iTimeStep, inputTotal.Value)

            ' No fleet specified?
            If iFleet = 0 Then
                ' #Yes: make all calculations
                Me.Calculate(results, _
                    inputTotal.Tons, inputTotal.Value, _
                    sTotalOutputBiomass, sTotalOutputValue, iTimeStep)
            End If


            ' Pass biomass to all targets in the flow
            For Each outputLink As cLink In Me.m_llinkOutput
                ' Pass resulting data to the next unit in the flow, and tell it to process

                ' Get system-defined value per ton
                sValuePerTon = outputLink.ValuePerTon
                ' Is default value?
                If (sValuePerTon = 1.0!) And (inputTotal.Tons <> 0.0!) Then
                    ' #Yes: use aggregated input value
                    sValuePerTon = inputTotal.Value / inputTotal.Tons
                End If

                outputLink.Target.Process(results, _
                        New cInput(inputTotal.Tons * outputLink.BiomassRatio, _
                                   inputTotal.Tons * outputLink.BiomassRatio * sValuePerTon), iTimeStep, iFleet)
            Next outputLink
        End If

    End Sub

    Protected Overridable Function SumInputs(ByVal lInputs As List(Of cInput)) As cInput
        Dim sTonsTotal As Single = 0.0
        Dim sValueTotal As Single = 0.0
        For Each input As cInput In lInputs
            sTonsTotal += input.Tons
            If (input.CustomValuePerTon <> 1) Then
                sValueTotal += (input.Tons * input.CustomValuePerTon)
            Else
                sValueTotal += input.Value
            End If
        Next
        Return New cInput(sTonsTotal, sValueTotal)
    End Function

    ''' <summary>
    ''' Make all calculations.
    ''' </summary>
    ''' <param name="results">The results object to store calculation results in.</param>
    Protected Overridable Function Calculate(ByVal results As cResults, _
            ByVal sInputBiomass As Single, ByVal sInputValue As Single, _
            ByVal sOutputBiomass As Single, ByVal sOutputValue As Single, _
            ByVal iTimeStep As Integer) As Boolean

        ' All good
        Return True

    End Function

#End Region ' Run

#Region " Copy / paste "

    Public Overrides Sub CopyFrom(ByVal obj As cEwEDatabase.cOOPStorable)
        Me.AllowEvents = False
        MyBase.CopyFrom(obj)
        Me.AllowEvents = True
    End Sub

#End Region ' Copy / paste

#Region " Properties "

    <Browsable(False)> _
    Public Property Sequence() As Integer
        Get
            Return Me.m_iSequence
        End Get
        Private Set(ByVal value As Integer)
            Me.m_iSequence = value
        End Set
    End Property

    <Browsable(False)> _
    Protected Property Core() As cCore
        Get
            Return Me.m_core
        End Get
        Private Set(ByVal value As cCore)
            Me.m_core = value
        End Set
    End Property

    <Browsable(False)> _
    Public MustOverride ReadOnly Property UnitType() As cUnitFactory.eUnitType

    <Browsable(False)> _
    Public Overridable ReadOnly Property HasError() As Boolean
        Get
            Return False
        End Get
    End Property

    <Browsable(True), _
        Category(sPROPCAT_GENERAL), _
        DisplayName("Name"), _
        Description("Name of this unit"), _
        cPropertySorter.PropertyOrder(1)> _
    Public Overridable Property Name() As String
        Get
            Return m_strName
        End Get
        Set(ByVal value As String)
            Me.m_strName = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True), _
        Category(sPROPCAT_GENERAL), _
        DisplayName("Nationality"), _
        Description("Nationality of this unit"), _
        cPropertySorter.PropertyOrder(3)> _
    Public Overridable Property Nationality() As Integer
        Get
            Return Me.m_iNationality
        End Get
        Set(ByVal value As Integer)
            Me.m_iNationality = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True), _
        Category(sPROPCAT_GENERAL), _
        DisplayName("Category"), _
        Description("Category to which this unit belongs"), _
        cPropertySorter.PropertyOrder(2)> _
    Public MustOverride ReadOnly Property Category() As String

    <Browsable(False)> _
    Public Overridable ReadOnly Property Style() As cStyleGuide.eStyleFlags
        Get
            Return cStyleGuide.eStyleFlags.OK
        End Get
    End Property

#End Region ' Properties

End Class
