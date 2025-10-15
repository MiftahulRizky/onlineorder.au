
Partial Class Export_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColor()
            txtFromDate.Text = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd")
            txtToDate.Text = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd")
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlData.SelectedValue = "" Then
                Call MessageError(True, "ORDER DATA IS REQUIRED !")
                ddlData.CssClass = "form-select  is-invalid"
                ddlData.Focus()
                Exit Sub
            End If

            If ddlStatus.SelectedValue = "" Then
                Call MessageError(True, "STATUS IS REQUIRED !")
                ddlStatus.CssClass = "form-select  is-invalid"
                ddlStatus.Focus()
                Exit Sub
            End If

            If txtFromDate.Text = "" Then
                Call MessageError(True, "FROM DATE IS REQUIRED !")
                txtFromDate.CssClass = "form-select  is-invalid"
                txtFromDate.Focus()
                Exit Sub
            End If

            If Not txtFromDate.Text = "" Then
                If Not IsDate(txtFromDate.Text) Then
                    Call MessageError(True, "FROM DATE SHOULD BE DATE TYPE !")
                    txtFromDate.CssClass = "form-control  is-invalid"
                    txtFromDate.Focus()
                    Exit Sub
                End If
            End If

            If txtToDate.Text = "" Then
                Call MessageError(True, "TO DATE IS REQUIRED !")
                txtToDate.CssClass = "form-control  is-invalid"
                txtToDate.Focus()
                Exit Sub
            End If

            If Not txtToDate.Text = "" Then
                If Not IsDate(txtToDate.Text) Then
                    Call MessageError(True, "TO DATE SHOULD BE DATE TYPE !")
                    txtToDate.CssClass = "form-control  is-invalid"
                    txtToDate.Focus()
                    Exit Sub
                End If
            End If

            If msgError.InnerText = "" Then
                If txtToDate.Text = "" Then
                    txtToDate.Text = txtFromDate.Text
                End If

                If ddlExport.SelectedValue = "XML" Then
                    Dim url As String = String.Format("order.aspx?fromdate={0}&todate={1}&status={2}&type={3}&action=check", txtFromDate.Text, txtToDate.Text, ddlStatus.SelectedValue, ddlData.SelectedValue)

                    Response.Write("<script>")
                    Response.Write("window.open('" + url + "' ,'_blank')")
                    Response.Write("</script>")
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnSubmit_Click", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)
        ddlExport.CssClass = "form-select "
        ddlData.CssClass = "form-select "
        ddlStatus.CssClass = "form-select "
        txtFromDate.CssClass = "form-control "
        txtToDate.CssClass = "form-control "
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False
        ' msgError.InnerText = Msg
        If Show = True Then
            ' divError.Visible = True
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub
End Class
