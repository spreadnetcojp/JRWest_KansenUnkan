Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Module Module1

    Sub Main()
        Dim oFooter1 As New EkMasterDataFileFooter("G", "DSH", DateTime.Now, "255", "���D�@�̉����}�X�^", "2012-11-19 14:24:59")
        oFooter1.AddInto("TestData.bin")

        Dim oFooter2 As New EkMasterDataFileFooter("TestData.bin")
        oFooter2.ApplicableModel = "Y"
        oFooter2.DispName = "�����̉����̃}�X�^"
        oFooter2.UpdateInto("TestData.bin")
    End Sub

End Module
