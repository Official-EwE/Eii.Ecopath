#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core

#End Region 'Imports

Namespace Ecopath.Tools

    <CLSCompliant(False)> _
    Public Class gridPedigree
        Inherits EwEGrid

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim group As cCoreGroupBase = Nothing

            Me.Redim(Core.nGroups + 1, cPedigreeManager.SupportVariables.Count + 2)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)

            For iVariable As Integer = 0 To cPedigreeManager.SupportVariables.Count - 1
                Me(0, iVariable + 2) = New EwEColumnHeaderCell(cPedigreeManager.SupportVariables(iVariable).ToString)
            Next iVariable

            For iGroup As Integer = 1 To Core.nGroups
                group = Me.Core.EcoPathGroupInputs(iGroup)
                Me(iGroup, 0) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Index)
                Me(iGroup, 1) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)
            Next iGroup

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cCoreGroupBase = Nothing
            Dim man As cPedigreeManager = Nothing
            Dim cell As EwECellBase = Nothing
            Dim style As cStyleGuide.eStyleFlags = (cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.ValueComputed)

            For iGroup As Integer = 1 To Core.nGroups
                ' Get group
                group = Me.Core.EcoPathGroupInputs(iGroup)
                For iVariable As Integer = 0 To cPedigreeManager.SupportVariables.Count - 1
                    ' Get pedigree
                    man = Me.Core.GetPedigreeManager(cPedigreeManager.SupportVariables(iVariable))
                    cell = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.Pedigree, man)
                    Me(iGroup, 2 + iVariable) = cell
                Next
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
