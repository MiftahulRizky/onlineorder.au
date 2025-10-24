
Partial Class Setting_Kit_Add
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/kit", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColor()
            Call BindDataDesign()
            Call BindDataBlind(ddlDesign.SelectedValue)

            Call BindBracketType()
            Call BindTubeType()
            Call BindControlType()
            Call BindColourType()
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        Call BindDataBlind(ddlDesign.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtId.Text = "" Then
                Call MessageError(True, "KIT ID IS REQUIRED !")
                txtId.CssClass = "form-control  is-invalid"
                txtId.Focus()
                Exit Sub
            End If

            If txtName.Text = "" Then
                Call MessageError(True, "KIT NAME IS REQUIRED !")
                txtName.CssClass  = "form-control  is-invalid"
                txtName.Focus()
                Exit Sub
            End If

            If ddlDesign.SelectedValue = "" Then
                Call MessageError(True, "DESIGN TYPE IS REQUIRED !")
                ddlDesign.CssClass = "form-select  is-invalid"
                ddlDesign.Focus()
                Exit Sub
            End If

            If ddlBlind.SelectedValue = "" Then
                Call MessageError(True, "BLIND TYPE IS REQUIRED !")
                ddlBlind.CssClass = "form-select  is-invalid"
                ddlBlind.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT NEW HARDWARE KIT. ID : " & txtId.Text)

                Response.Redirect("~/setting/kit", False)
                Exit Sub
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/kit", False)
    End Sub

    Protected Sub btnBracketSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If Not txtBracketName.Text = "" Then
                sdsBracket.Insert()

                Call BindBracketType()
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnControlSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If Not txtControlName.Text = "" Then
                sdsControl.Insert()

                Call BindControlType()
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnTubeSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If Not txtTubeName.Text = "" Then
                sdsTube.Insert()

                Call BindTubeType()
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnColourSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If Not txtColourName.Text = "" Then
                sdsColour.Insert()

                Call BindColourType()
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDataDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs ORDER BY Name ASC")
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

    Private Sub BindDataBlind(DesignId As String)
        ddlBlind.Items.Clear()
        Try
            If Not DesignId = "" Then
                ddlBlind.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Blinds WHERE DesignId='" + UCase(DesignId).ToString() + "' ORDER BY Name ASC")
                ddlBlind.DataTextField = "NameText"
                ddlBlind.DataValueField = "Id"
                ddlBlind.DataBind()

                If ddlBlind.Items.Count > 0 Then
                    ddlBlind.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindBracketType()
        ddlBracketType.Items.Clear()
        Try
            ddlBracketType.DataSource = publicCfg.GetListData("SELECT Name AS NameValue, UPPER(Name) AS NameText FROM BracketType WHERE Active=1 ORDER BY Name ASC")
            ddlBracketType.DataTextField = "NameText"
            ddlBracketType.DataValueField = "NameValue"
            ddlBracketType.DataBind()

            If ddlBracketType.Items.Count > 0 Then
                ddlBracketType.Items.Insert(0, New ListItem("N/A", "N/A"))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindTubeType()
        ddlTubeType.Items.Clear()
        Try
            ddlTubeType.DataSource = publicCfg.GetListData("SELECT Name AS NameValue, UPPER(Name) AS NameText FROM TubeType WHERE Active=1 ORDER BY Name ASC")
            ddlTubeType.DataTextField = "NameText"
            ddlTubeType.DataValueField = "NameValue"
            ddlTubeType.DataBind()

            If ddlTubeType.Items.Count > 0 Then
                ddlTubeType.Items.Insert(0, New ListItem("N/A", "N/A"))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindControlType()
        ddlControlType.Items.Clear()
        Try
            ddlControlType.DataSource = publicCfg.GetListData("SELECT Name AS NameValue, UPPER(Name) AS NameText FROM ControlType WHERE Active=1 ORDER BY Name ASC")
            ddlControlType.DataTextField = "NameText"
            ddlControlType.DataValueField = "NameValue"
            ddlControlType.DataBind()

            If ddlControlType.Items.Count > 0 Then
                ddlControlType.Items.Insert(0, New ListItem("N/A", "N/A"))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindColourType()
        ddlColourType.Items.Clear()
        Try
            ddlColourType.DataSource = publicCfg.GetListData("SELECT Name AS NameValue, UPPER(Name) AS NameText FROM ColourType WHERE Active=1 ORDER BY Name ASC")
            ddlColourType.DataTextField = "NameText"
            ddlColourType.DataValueField = "NameValue"
            ddlColourType.DataBind()

            If ddlColourType.Items.Count > 0 Then
                ddlColourType.Items.Insert(0, New ListItem("N/A", "N/A"))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtId.CssClass = "form-control "
        txtName.CssClass = "form-control "
        ddlDesign.CssClass = "form-select "
        ddlBlind.CssClass = "form-select "
        ddlBracketType.CssClass = "form-select "
        ddlTubeType.CssClass = "form-select "
        ddlControlType.CssClass = "form-select "
        ddlColourType.CssClass = "form-select "
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
