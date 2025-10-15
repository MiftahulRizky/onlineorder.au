Imports System.Data

Partial Class Setting_Region_Detail
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Session("regionDetail") = "" Then
            Response.Redirect("~/setting/region", False)
            Exit Sub
        End If

        lblId.Text = UCase(Session("regionDetail")).ToString()
        If Not IsPostBack Then
            Call BackColor()
            Call BindDataRegion(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtAliasName.Text = "" Then
                Call MessageError(True, "ALIAS NAME IS REQUIRED !")
                txtAliasName.CssClass = "form-control  is-invalid"
                txtAliasName.Focus()
                Exit Sub
            End If

            If txtFullName.Text = "" Then
                Call MessageError(True, "FULL NAME IS REQUIRED !")
                txtFullName.CssClass = "form-control  is-invalid"
                txtFullName.Focus()
                Exit Sub
            End If

            If ddlOperator.SelectedValue = "" Then
                Call MessageError(True, "OPERATOR IS REQUIRED !")
                ddlOperator.CssClass = "form-select  is-invalid"
                ddlOperator.Focus()
                Exit Sub
            End If

            If ddlActive.SelectedValue = "" Then
                Call MessageError(True, "ACTIVE IS REQUIRED !")
                ddlActive.CssClass = "form-select  is-invalid"
                ddlActive.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Update()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "UPDATE REGION. ALIAS NAME : " & txtAliasName.Text)

                Response.Redirect("~/setting/region", False)
                Exit Sub
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/region", False)
    End Sub

    Private Sub BindDataRegion(Id As String)
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM Regions WHERE Id = '" + Id + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/region", False)
                Exit Sub
            End If

            txtAliasName.Text = myData.Tables(0).Rows(0).Item("AliasName").ToString()
            txtFullName.Text = myData.Tables(0).Rows(0).Item("FullName").ToString()
            ddlOperator.SelectedValue = myData.Tables(0).Rows(0).Item("Operator").ToString()
            txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtAliasName.CssClass = "form-control "
        txtFullName.CssClass = "form-control "
        ddlOperator.CssClass = "form-select "
        txtDescription.CssClass = "form-control"
        ddlActive.CssClass = "form-select "
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
