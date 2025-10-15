Imports System.Data

Partial Class Setting_Company_Detail
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Session("companyDetail") = "" Then
            Response.Redirect("~/setting/company", False)
            Exit Sub
        End If

        txtId.Text = Session("companyDetail")
        If Not IsPostBack Then
            Call BackColor()
            Call BindDataCompany(txtId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtId.Text = "" Then
                Call MessageError(True, "COMPANY CODE IS REQUIRED !")
                txtId.CssClass = "form-control  is-invalid"
                txtId.Focus()
                Exit Sub
            End If

            If txtId.Text = publicCfg.GetItemData("SELECT Id FROM Company WHERE Id = '" + txtId.Text + "'") Then
                Call MessageError(True, "COMPANY CODE ALREADY EXISTS !")
                txtId.CssClass = "form-control  is-invalid"
                txtId.Focus()
                Exit Sub
            End If

            If txtName.Text = "" Then
                Call MessageError(True, "COMPANY NAME IS REQUIRED !")
                txtName.CssClass = "form-control  is-invalid"
                txtName.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Update()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "UPDATE COMPANY. ID : " & txtId.Text)

                Response.Redirect("~/setting/company", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/company", False)
    End Sub

    Private Sub BindDataCompany(Id As String)
        Call BackColor()
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM Company WHERE Id = '" + Id + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/company", False)
                Exit Sub
            End If
            txtId.Text = myData.Tables(0).Rows(0).Item("Id").ToString() : txtId.ReadOnly = True
            txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))

        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtId.CssClass  ="form-control "
        txtName.CssClass    ="form-control "
        txtDescription.CssClass ="form-control"
        ddlActive.CssClass  ="form-select "
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
