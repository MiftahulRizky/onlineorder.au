Imports System.Data
Partial Class Setting_Application_Detail
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Session("appDetail") = "" Then
            Response.Redirect("~/setting/application", False)
            Exit Sub
        End If

        lblId.Text = UCase(Session("appDetail")).ToString()
        If Not IsPostBack Then
            Call BindDataApplication(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtName.Text = "" Then
                Call MessageError(True, "APPLICATION NAME IS REQUIRED !")
                txtName.CssClass = "form-control  is-invalid"
                txtName.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "UPDATE APPLICATION. NAME : " & txtName.Text)
                sdsPage.Update()
                Response.Redirect("~/setting/application", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/application/", False)
    End Sub

    Private Sub BindDataApplication(Id As String)
        Call BackColor()
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM Applications WHERE Id = '" + UCase(Id).ToString() + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/application", False)
                Exit Sub
            End If

            txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtName.CssClass = "form-control "
        txtDescription.CssClass = "form-control"
        ddlActive.CssClass = "form-control "
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        ' If Show = True Then : divError.Visible = True : End If
        If Show = True Then
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub
End Class
