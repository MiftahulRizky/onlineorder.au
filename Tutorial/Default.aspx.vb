Imports System.IO

Partial Class Tutorial_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            txtSearch.Text = Session("tutorialSearch")
            Call BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("tutorialSearch") = txtSearch.Text
        Response.Redirect("~/tutorial/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(txtSearch.Text)
    End Sub

    Protected Sub lvData_ItemCommand(sender As Object, e As ListViewCommandEventArgs)
        Call MessageError(False, String.Empty)
        Try
            If e.CommandName = "Unduh" Then
                Dim fileName As String = e.CommandArgument
                Dim directoryPath As String = Server.MapPath(String.Format("~/File/Tutorial/PDF"))
                Response.Clear()

                Response.ContentType = ContentType
                Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(directoryPath + "/" + fileName)))
                Response.WriteFile(directoryPath + "/" + fileName)
                Response.Flush()
                Response.SuppressContent = True
                ApplicationInstance.CompleteRequest()
            End If

            If e.CommandName = "Lihat" Then
                Session("tutorialSearch") = txtSearch.Text
                Session("tutorialDetail") = e.CommandArgument
                Response.Redirect("~/tutorial/view", False)
            End If

            If e.CommandName = "Ubah" Then
                Session("tutorialSearch") = txtSearch.Text
                Session("tutorialDetail") = e.CommandArgument
                Response.Redirect("~/tutorial/edit", False)
            End If

            If e.CommandName = "Aktif" Then
                lblId.Text = UCase(e.CommandArgument).ToString()
                sdsPage.Update()

                Call BindData(txtSearch.Text)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "lvData_ItemCommand", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, "")
        Try
            lblId.Text = txtIdDelete.Text
            sdsPage.Delete()

            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnDelete_Click", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Session("tutorialSearch") = "" : Session("tutorialDetail") = ""
        Try
            Dim roleId As String = UCase(Session("RoleId")).ToString()
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " AND (Title LIKE '%" + SearchText + "%' OR Description LIKE '%" + SearchText + "%' OR Thumbnail LIKE '%" + SearchText + "%' OR [File] LIKE '%" + SearchText + "%')"
            End If

            lvData.DataSource = publicCfg.GetListData(String.Format("SELECT *, LEFT(Description, 40) AS NewDesc FROM Tutorials CROSS APPLY STRING_SPLIT(RoleId, ',') AS roleArray WHERE roleArray.VALUE = '" + roleId + "' {0} ORDER BY Id ASC", search))
            lvData.DataBind()

            btnAdd.Visible = False
            If Session("RoleName") = "Administrator" Then
                btnAdd.Visible = True   
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindData", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False 
        ' msgError.InnerText = Msg
        If Show = True Then 
        '  divError.Visible = True 
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub

    Protected Function VisibleAction() As Boolean
        Dim result As Boolean = False
        If Session("RoleName") = "Administrator" Then : result = True : End If
        Return result
    End Function

    Protected Function TextActive(Active As String) As String
        Dim result As String = "Activate"
        If Active = "True" Then : Return "Deactivate" : End If
        Return result
    End Function
End Class