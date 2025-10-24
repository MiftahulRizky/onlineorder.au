Imports System.Data
Partial Class Setting_Blind_Detail
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/blind", False)
            Exit Sub
        End If

        If Session("blindDetail") = "" Then
            Response.Redirect("~/setting/blind", False)
            Exit Sub
        End If

        lblId.Text = Session("blindDetail")
        If Not IsPostBack Then
            Call BackColor()
            Call BindDataBlind(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlDesign.SelectedValue = "" Then
                Call MessageError(True, "DESIGN NAME IS REQUIRED !")
                ddlDesign.CssClass = "form-select  is-invalid"
                ddlDesign.Focus()
                Exit Sub
            End If

            If txtName.Text = "" Then
                Call MessageError(True, "BLIND NAME IS REQUIRED !")
                txtName.CssClass = "form-control  is-invalid"
                txtName.Focus()
                Exit Sub
            End If

            If ddlCompany.SelectedValue = "" Then
                Call MessageError(True, "COMPANY IS REQUIRED !")
                ddlCompany.CssClass = "form-select  is-invalid"
                ddlCompany.Focus()
                Exit Sub
            End If

            If ddlActive.SelectedValue = "" Then
                Call MessageError(True, "BLIND ACTIVE IS REQUIRED !")
                ddlActive.CssClass = "form-select  is-invalid"
                ddlActive.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Update()

                Dim vUserId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(vUserId, Page.Title, "UPDATE BLIND TYPE. NAME : " & txtName.Text)

                Response.Redirect("~/setting/blind", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/blind", False)
    End Sub

    Private Sub BindDataBlind(Id As String)
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM Blinds WHERE Id = '" + UCase(Id).ToString() + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/blind", False)
                Exit Sub
            End If
            Call BindDataDesign()
            Call BindDataCompany()

            ddlDesign.SelectedValue = myData.Tables(0).Rows(0).Item("DesignId").ToString()
            ddlCompany.SelectedValue = myData.Tables(0).Rows(0).Item("Company").ToString()
            txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDataDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs WHERE Active=1 ORDER BY Name ASC")
            ddlDesign.DataTextField = "NameText"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDataCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = publicCfg.GetListData("SELECT * FROM Company ORDER BY Name ASC")
            ddlCompany.DataTextField = "Name"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)
        txtName.CssClass = "form-control "
        ddlDesign.CssClass = "form-control "
        ddlCompany.CssClass = "form-control "
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
