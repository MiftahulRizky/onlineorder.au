Imports System.Data

Partial Class Setting_Store_Detail
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("storeDetail") = "" Then
            Response.Redirect("~/setting/store/", False)
            Exit Sub
        End If

        txtId.Text = Session("storeDetail")
        If Not IsPostBack Then
            Call BackColor()

            Call BindDataStore(txtId.Text)
        End If
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtId.Text = "" Then
                Call MessageError(True, "STORE ID IS REQUIRED !")
                txtId.CssClass = "form-control  is-invalid"
                txtId.Focus()
                Exit Sub
            End If

            If txtName.Text = "" Then
                Call MessageError(True, "STORE NAME IS REQUIRED !")
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

            If ddlRegion.SelectedValue = "" Then
                Call MessageError(True, "REGION / STATES IS REQUIRED !")
                ddlRegion.CssClass = "form-select  is-invalid"
                ddlRegion.Focus()
                Exit Sub
            End If

            If ddlType.SelectedValue = "" Then
                Call MessageError(True, "TYPE IS REQUIRED !")
                ddlType.CssClass = "form-select  is-invalid"
                ddlType.Focus()
                Exit Sub
            End If

            If txtEmail.Text <> "" Then
                If Not InStr(txtEmail.Text, "@") > 0 Or (Not InStr(txtEmail.Text, ".com") > 0 And Not InStr(txtEmail.Text, ".co.id")) > 0 Then
                    Call MessageError(True, "YOUR EMAIL NOT VALID !")
                    txtEmail.CssClass = "form-control  is-invalid"
                    txtEmail.Focus()
                    Exit Sub
                End If
            End If

            If msgError.InnerText = "" Then
                sdsStore.Update()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "UPDATE STORE. ID : " & txtId.Text)

                Response.Redirect("~/setting/store/", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/store/", False)
    End Sub

    Private Sub BindDataStore(Id As String)
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM Stores WHERE Id = '" + Id + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/store", False)
                Exit Sub
            End If

            Call BindRegion()
            Call BindCompany()

            txtId.Text = myData.Tables(0).Rows(0).Item("Id").ToString() : txtId.ReadOnly = True
            txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            ddlType.SelectedValue = myData.Tables(0).Rows(0).Item("Type").ToString()
            ddlRegion.SelectedValue = myData.Tables(0).Rows(0).Item("RegionId").ToString()
            ddlCompany.SelectedValue = myData.Tables(0).Rows(0).Item("Company").ToString()
            txtPhone.Text = myData.Tables(0).Rows(0).Item("Phone").ToString()
            txtEmail.Text = myData.Tables(0).Rows(0).Item("Email").ToString()
            txtFax.Text = myData.Tables(0).Rows(0).Item("Fax").ToString()
            txtAbn.Text = myData.Tables(0).Rows(0).Item("ABN").ToString()
            txtAddress.Text = myData.Tables(0).Rows(0).Item("Address").ToString()
            ddlActive.SelectedValue = myData.Tables(0).Rows(0).Item("Active").ToString()

        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindRegion()
        ddlRegion.Items.Clear()
        Try
            ddlRegion.DataSource = publicCfg.GetListData("SELECT * FROM Regions ORDER BY AliasName ASC")
            ddlRegion.DataTextField = "AliasName"
            ddlRegion.DataValueField = "Id"
            ddlRegion.DataBind()

            If ddlRegion.Items.Count > 1 Then
                ddlRegion.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Company ORDER BY Id ASC")
            ddlCompany.DataTextField = "NameText"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 1 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtId.CssClass = "form-control "
        txtName.CssClass = "form-control "
        ddlType.CssClass = "form-select "
        ddlRegion.CssClass = "form-select "
        ddlCompany.CssClass = "form-select "
        txtPhone.CssClass = "form-control "
        txtEmail.CssClass = "form-control "
        txtFax.CssClass = "form-control "
        txtAbn.CssClass = "form-control "
        txtAddress.CssClass = "form-control"
        ddlActive.CssClass = "form-select "
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
