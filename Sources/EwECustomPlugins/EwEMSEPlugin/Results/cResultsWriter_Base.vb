' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports System.IO

Public MustInherit Class cResultsWriter_Base

    Protected m_MSE As cMSE
    Protected m_Core As cCore
    Protected m_nStrategies As Integer

    MustOverride Sub Initialise(msgReport As cMessage, MSE As cMSE, Results_Array As cResultsCollector_Base, FolderPath As cMSEUtils.eMSEPaths)

    MustOverride Sub WriteResults()

    MustOverride Sub ReleaseWriters()

    Protected ReadOnly Property StrategyName(iStrategy As Integer) As String
        Get
            Return Me.m_MSE.Strategies(iStrategy - 1).Name
        End Get
    End Property

End Class
