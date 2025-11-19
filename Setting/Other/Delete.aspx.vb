
Imports System.IO

Partial Class Setting_Other_Delete
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not Session("LevelName") = "Leader" And Not Session("LevelName") = "Super Admin" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            DeleteAction()
        End If
    End Sub

    Private Sub DeleteAction()
        Try
            Dim directoryInv As New DirectoryInfo(Server.MapPath("~/File/Inv/"))
            EmptyFolder(directoryInv)

            Dim directoryMatrix As New DirectoryInfo(Server.MapPath("~/File/Matrix/"))
            EmptyFolder(directoryMatrix)

            Dim directoryNewsletter As New DirectoryInfo(Server.MapPath("~/File/Newsletter/"))
            EmptyFolder(directoryNewsletter)

            Dim directoryOrder As New DirectoryInfo(Server.MapPath("~/File/Order/"))
            EmptyFolder(directoryOrder)

            Dim directoryReport As New DirectoryInfo(Server.MapPath("~/File/Report/"))
            EmptyFolder(directoryReport)

            Dim directoryShipment As New DirectoryInfo(Server.MapPath("~/File/Shipment/"))
            EmptyFolder(directoryShipment)

            Response.Redirect("~/", False)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub EmptyFolder(directory As DirectoryInfo)
        For Each file As FileInfo In directory.GetFiles()
            If Not file.Name.Equals("README.txt", StringComparison.OrdinalIgnoreCase) Then
                file.Delete()
            End If
        Next

        For Each subDir As DirectoryInfo In directory.GetDirectories()
            subDir.Delete(True)
        Next
    End Sub
End Class
